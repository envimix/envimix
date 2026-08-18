using System.Collections.Immutable;

namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixTeamAttack : Envimix
{
    [Setting(As = "Time limit")]
    public int TimeLimit = 900;

    [Setting(As = "Car select time")]
    public int CarSelectTime = 5;

    [Setting(As = "Custom countdown")]
    public int CustomCountdown = -1;

    [Setting(As = "Auto-respawn time")]
    public int AutoRespawnTime = 6;

    [Setting(As = "Clear scores on map end (WIP)")]
    public bool ClearScoresOnMapEnd = false;

    public required Dictionary<string, int> AutoRespawn;

    [Netwrite] public string ModeHelp { get; set; }

    public override void OnServerInit()
    {
        //ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixMultiplayerClient.Script.txt";

        CreateServersideLayers();
        CreateLayer("ScoreboardTeamAttack", CUILayer.EUILayerType.ScoresTable);
        IndependantLaps = true;
        ModeHelp = "OBJECTIVE: Two teams compare collective skill on different cars. Pick any car at any time. Receive points by finishing the track as fast as possible with the most amount of cars possible under a time limit.";
        ModeStatusMessage = ModeHelp;

        UseClans = true;
        UseForcedClans = true;
        Teams[0].Name = "Team Red";
        Teams[0].ColorPrimary = new Vec3(1, 0, 0);
        Teams[0].PresentationManialinkUrl = "envimix?team=red";
        Teams[1].Name = "Team Blue";
        Teams[1].ColorPrimary = new Vec3(0, 0, 1);
        Teams[1].PresentationManialinkUrl = "envimix?team=blue";

        if (AllowRespawn)
        {
            RespawnBehaviour = CTmMode.ETMRespawnBehaviour.GiveUpBeforeFirstCheckPoint;
        }
        else
        {
            RespawnBehaviour = CTmMode.ETMRespawnBehaviour.AlwaysGiveUp;
        }
    }

    public override void OnMapStart()
    {
        // Preliminary data shown before the Envimania session is established, overwritten once the session responds
        if (EnvimixWebAPI is not "")
        {
            RequestUnauthorizedMapInfo();
        }
    }

    public override void OnUIEvent(CUIConfigEvent e)
    {
        switch (e.Type)
        {
            case CUIConfigEvent.EType.OnLayerCustomEvent:
                ProcessGeneralEnvimixEvents(e);
                ProcessUpdateSkinEvent(e);
                ProcessUpdateCarEvent(e, forceFreeze: false);
                break;
        }

        // +++OnUIChatEvent+++
    }

    public override void OnMapInit()
    {
        if (!ClearScoresOnMapEnd)
        {
            ClearScores();
        }
    }

    public override void OnMapLoad()
    {
        SetLaps(); // Define independent laps or forced amount of laps

        PrespawnEnvimixPlayers();

        // Period to select a starting car
        CarSelectionMode = true;
        TeamSelectionMode = true;

        foreach (var player in Players)
        {
            NoticeMessage(UIManager.GetUI(player), "$ff0Select your starting car!$g\nYou will be able to change it anytime later.");
        }
    }

    public override void WhileMapIntro()
    {
        ProcessUiEvents();
    }

    public override void WhileSynchro()
    {
        ProcessUiEvents();
    }

    public override void OnGameStart()
    {
        // Set the countdown
        CutOffTimeLimit = Now + CarSelectTime * 1000;

        // Loop during the countdown
        while (CutOffTimeLimit - Now > 0 && !TerminatedMatch())
        {
            ProcessUiEvents();

            foreach (var e in PendingEvents)
            {
                switch (e.Type)
                {
                    case CTmModeEvent.EType.OnPlayerAdded:
                        PrespawnPlayer(e.Player);
                        break;
                }
            }
            
            foreach (var player in PlayersWaiting)
            {
                PrespawnPlayer(player);
            }

            CheckEnvimaniaSession();
            CheckMapInfoUnauthorized();
            CheckRatings();
            CheckUserInfoRequests();
            UpdateSpectatorLists();

            Yield();
        }

        AutobalanceTeams();

        CarSelectionMode = false;
        TeamSelectionMode = false;

        OpenNewLadder();

        foreach (var player in Players)
        {
            NoticeMessage(UIManager.GetUI(player), "");
        }

        if (TimeLimit < 0)
        {
            CutOffTimeLimit = -1;
        }
        else
        {
            CutOffTimeLimit = Now + TimeLimit * 1000 + 3000;
        }

        foreach (var player in Players)
        {
            // why to reset notice again?
            NoticeMessage(UIManager.GetUI(player), "");

            TrySpawnEnvimixTeamAttackPlayer(player, frozen: false);
        }
    }

    private bool TrySpawnEnvimixTeamAttackPlayer(CTmPlayer player, bool frozen)
    {
        if (frozen)
        {
            return TrySpawnEnvimixPlayer(player, frozen);
        }

        if (CutOffTimeLimit - Now >= TimeLimit * 1000)
        {
            return TrySpawnEnvimixPlayer(player, CutOffTimeLimit - TimeLimit * 1000);
        }

        if (CustomCountdown < 0)
        {
            return TrySpawnEnvimixPlayer(player, -1);
        }

        return TrySpawnEnvimixPlayer(player, Now + CustomCountdown);
    }

    public bool SpawnEnvimixTeamAttackPlayer(CTmPlayer player, string car, bool frozen)
    {
        bool spawned;

        if (frozen)
        {
            spawned = SpawnEnvimixPlayer(player, car, frozen);
        }
        else if (CutOffTimeLimit - Now >= TimeLimit * 1000)
        {
            spawned = SpawnEnvimixPlayer(player, car, CutOffTimeLimit - TimeLimit * 1000);
        }
        else if (CustomCountdown < 0)
        {
            spawned = SpawnEnvimixPlayer(player, car, -1);
        }
        else
        {
            spawned = SpawnEnvimixPlayer(player, car, Now + CustomCountdown);
        }

        UpdateDisabledCarNotice(player, "Default car is currently disabled.");

        return spawned;
    }

    public void ChangePlayerClan(CTmPlayer player, int clan)
    {
        UnspawnPlayer(player);
        SetPlayerClan(player, clan);
        var car = Netwrite<string>.For(player);
        SpawnEnvimixTeamAttackPlayer(player, car.Get(), frozen: true);
    }

    private void AutobalanceTeams()
    {
        ImmutableArray<CTmScore> team1 = new();
        ImmutableArray<CTmScore> team2 = new();

        foreach (var score in Scores)
        {
            if (score.TeamNum == 1) team1.Add(score);
            else if (score.TeamNum == 2) team2.Add(score);
        }

        if (MathLib.Abs(team1.Length - team2.Length) <= 1)
        {
            return;
        }

        ImmutableArray<CTmScore> largerTeam;
        int targetTeamNum;

        if (team1.Length > team2.Length)
        {
            largerTeam = team1;
            targetTeamNum = 2;
        }
        else
        {
            largerTeam = team2;
            targetTeamNum = 1;
        }

        string newTeamName;
        if (targetTeamNum == 1)
        {
            newTeamName = Teams[0].ColorizedName;
        }
        else
        {
            newTeamName = Teams[1].ColorizedName;
        }

        var amountToMove = MathLib.Abs(team1.Length - team2.Length) / 2;

        for (var i = 0; i < amountToMove; i++)
        {
            var randomIndex = MathLib.Rand(0, largerTeam.Length - 1);
            var scoreToMove = largerTeam[randomIndex];
            largerTeam.RemoveAt(randomIndex);

            var player = GetPlayer(scoreToMove.User.Login);

            if (player is null)
            {
                continue;
            }

            ChangePlayerClan(player, targetTeamNum);
            UIManager.UIAll.SendChat($"$<{scoreToMove.User.Name}$> has been autobalanced to {newTeamName}.");
        }
    }

    public void RespawnAllWaiting()
    {
        foreach (var player in PlayersWaiting)
        {
            if (IsWaitingDueToDisabledDefaultCar(player))
            {
                continue;
            }

            // In game loop and in time attack, this means when full respawn
            TrySpawnEnvimixTeamAttackPlayer(player, frozen: false);
        }
    }

    public void CheckForAutoRespawn()
    {
        ImmutableArray<string> autoRespawnToClean = new();

        foreach (var (playerToAutoRespawn, whenFinished) in AutoRespawn)
        {
            if (AutoRespawnTime < 0)
            {
                autoRespawnToClean.Add(playerToAutoRespawn);
                continue;
            }

            if (Now > whenFinished + AutoRespawnTime * 1000)
            {
                TrySpawnEnvimixTeamAttackPlayer(GetPlayer(playerToAutoRespawn), frozen: false);

                autoRespawnToClean.Add(playerToAutoRespawn);
            }
        }

        foreach (var playerToAutoRespawn in autoRespawnToClean)
        {
            AutoRespawn.Remove(playerToAutoRespawn);
        }
    }

    public override void OnEvent(CTmModeEvent e)
    {
        switch (e.Type)
        {
            case CTmModeEvent.EType.OnPlayerAdded:
                PrespawnPlayer(e.Player);
                break;
            case CTmModeEvent.EType.WayPoint:
                if (e.IsEndRace && AutoRespawnTime > -1)
                {
                    AutoRespawn[e.Player.User.Login] = Now;
                }
                break;
            case CTmModeEvent.EType.GiveUp:
                if (AutoRespawn.ContainsKey(e.Player.User.Login))
                {
                    AutoRespawn.Remove(e.Player.User.Login);
                }
                break;
        }
    }

    public override void OnWarmUpLoop()
    {
        RespawnAllWaiting();
    }

    public override void OnGameLoop()
    {
        // TODO: check why. because of switching to spec while having a notice message displayed?
        foreach (var spectator in Spectators)
        {
            NoticeMessage(UIManager.GetUI(spectator), "");
        }

        RespawnAllWaiting();

        if (!IsWarmUp && CutOffTimeLimit != -1 && CutOffTimeLimit < Now)
        {
            MatchEndRequested = true;
        }

        CheckForAutoRespawn();
    }

    public override void OnGameEnd()
    {
        AutoRespawn.Clear();

        Ladder_ComputeRank(CTmMode.ETmScoreSortOrder.TotalPoints);

        foreach (var score in Scores)
        {
            score.LadderRankSortValue = -1 - score.Points;
            score.LadderMatchScoreValue = score.Points * 1f;
        }

        CloseLadder();
    }

    public override void OnPodiumStart()
    {
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.None;
        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.ForcedVisible;
    }

    public override void OnPodiumLoop()
    {
        foreach (var e in UIManager.PendingEvents)
        {
            // +++OnUIChatEvent+++
        }
    }

    public override void OnMapEnd()
    {
        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.Normal;
        CutOffTimeLimit = -1;

        if (ClearScoresOnMapEnd)
        {
            ClearScores();
        }
    }

    private void ProcessUpdateCarEvent(CUIConfigEvent e, bool forceFreeze)
    {
        switch (e.CustomEventType)
        {
            case "Car":
                if (e.CustomEventData.Count > 0)
                {
                    var carName = e.CustomEventData[0];
                    var player = GetPlayer(e.UI);
                    SetValidClientCar(player, carName);

                    var car = Netwrite<string>.For(player);

                    AutoRespawn.Remove(player.User.Login);

                    var frozen = forceFreeze || e.CustomEventData.Count > 2 && e.CustomEventData[2] == "True";
                    var spawned = SpawnEnvimixTeamAttackPlayer(player, car.Get(), frozen);

                    var isMenuEscape = e.CustomEventData.Count > 3 && e.CustomEventData[3] == "True";

                    if (spawned || isMenuEscape)
                    {
                        RequestEnvimaniaRecords(carName, MathLib.NearestInteger(player.GravityCoef * 10) - 10, GetLaps());
                    }
                }
                break;
        }
    }

    private void ProcessUiEvents()
    {
        foreach (var e in UIManager.PendingEvents)
        {
            switch (e.Type)
            {
                case CUIConfigEvent.EType.OnLayerCustomEvent:
                    ProcessGeneralEnvimixEvents(e);
                    ProcessUpdateSkinEvent(e);
                    ProcessUpdateCarEvent(e, forceFreeze: true);

                    switch (e.CustomEventType)
                    {
                        case "JoinTeam":
                            var team = TextLib.ToInteger(e.CustomEventData[0]);

                            if (team == 1 || team == 2)
                            {
                                Dictionary<int, int> teamPlayerCounts = new()
                                    {
                                        { 1, 0 },
                                        { 2, 0 }
                                    };

                                foreach (var score in Scores)
                                {
                                    if (teamPlayerCounts.ContainsKey(score.TeamNum))
                                    {
                                        teamPlayerCounts[score.TeamNum] += 1;
                                    }
                                }

                                foreach (var (t, count) in teamPlayerCounts)
                                {
                                    if (teamPlayerCounts[team] < count)
                                    {
                                        ChangePlayerClan(GetPlayer(e.UI), team);
                                        break;
                                    }
                                }
                            }

                            break;
                    }
                    break;
            }
            //+++OnUIChatEvent+++
        }
    }
}
