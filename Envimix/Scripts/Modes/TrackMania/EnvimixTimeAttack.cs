using System.Collections.Immutable;

namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixTimeAttack : Envimix
{
    [Setting(As = "Time limit")]
    public int TimeLimit = 900;

    [Setting(As = "Car select time")]
    public int CarSelectTime = 5;

    [Setting(As = "Custom countdown")]
    public int CustomCountdown = -1;

    [Setting(As = "Auto-respawn time")]
    public int AutoRespawnTime = 6;

    [Setting(As = "Show individual ladder points difference")]
    public bool ShowIndividualLadderPointsDiff = true;

    [Setting(As = "<hidden>")]
    public bool ClearScoresOnMapEnd = false;

    public required Dictionary<string, int> AutoRespawn;

    [Netwrite] public string ModeHelp { get; set; }
    [Netwrite] public bool ShowIndividualLadderPointsDiffEnabled { get; set; }

    public override void OnServerInit()
    {
        //ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixMultiplayerClient.Script.txt";

        CreateServersideLayers();
        CreateLayer("ScoreboardTimeAttack", CUILayer.EUILayerType.ScoresTable);
        ShowIndividualLadderPointsDiffEnabled = ShowIndividualLadderPointsDiff;
        IndependantLaps = true;
        ModeHelp = "OBJECTIVE: Finish the track as fast as possible with different cars under a time limit. Pick any car at any time.";
        ModeStatusMessage = ModeHelp;

        if (AllowRespawn)
        {
            RespawnBehaviour = CTmMode.ETMRespawnBehaviour.GiveUpBeforeFirstCheckPoint;
        }
        else
        {
            RespawnBehaviour = CTmMode.ETMRespawnBehaviour.AlwaysGiveUp;
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

    public override void OnMapStart()
    {
        // Preliminary data shown before the Envimania session is established, overwritten once the session responds
        if (EnvimixWebAPI is not "")
        {
            RequestUnauthorizedMapInfo();
        }
    }

    public override void OnMapLoad()
    {
        SetLaps(); // Define independent laps or forced amount of laps

        PrespawnEnvimixPlayers();
    }

    public override void OnMapIntroStart()
    {
        foreach (var player in Players)
        {
            NoticeMessage(UIManager.GetUI(player), "You will be able to switch the car after the intro ends for everyone.");
        }
    }

    public override void OnMapIntroEnd()
    {
        // Reset notice message for everyone
        foreach (var player in Players)
        {
            NoticeMessage(UIManager.GetUI(player), "");
        }
    }

    public override void WhileMapIntro()
    {
        foreach (var e in UIManager.PendingEvents)
        {
            ProcessGeneralEnvimixEvents(e);
        }
    }

    public override void OnGameStart()
    {
        // Period to select a starting car once the map is fully loaded
        CarSelectionMode = true;

        foreach (var player in Players)
        {
            NoticeMessage(UIManager.GetUI(player), "$ff0Select your starting car!$g\nYou will be able to change it anytime later.");
        }

        // Set the countdown
        CutOffTimeLimit = Now + CarSelectTime * 1000;

        // Might be after car select time instead
        OpenNewLadder();

        // Loop during the countdown
        while (CutOffTimeLimit - Now > 0 && !TerminatedMatch())
        {
            foreach (var e in UIManager.PendingEvents)
            {
                switch (e.Type)
                {
                    case CUIConfigEvent.EType.OnLayerCustomEvent:
                        ProcessGeneralEnvimixEvents(e);
                        ProcessUpdateSkinEvent(e);
                        ProcessUpdateCarEvent(e, forceFreeze: true);
                        break;
                }
                //+++OnUIChatEvent+++
            }

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

        CarSelectionMode = false;
        TeamSelectionMode = false;

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

            TrySpawnEnvimixTimeAttackPlayer(player, frozen: false);
        }
    }

    private bool TrySpawnEnvimixTimeAttackPlayer(CTmPlayer player, bool frozen)
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

    public bool SpawnEnvimixTimeAttackPlayer(CTmPlayer player, string car, bool frozen)
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

    public void RespawnAllWaiting()
    {
        foreach (var player in PlayersWaiting)
        {
            if (IsWaitingDueToDisabledDefaultCar(player))
            {
                continue;
            }

            // In game loop and in time attack, this means when full respawn
            TrySpawnEnvimixTimeAttackPlayer(player, frozen: false);
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
                TrySpawnEnvimixTimeAttackPlayer(GetPlayer(playerToAutoRespawn), frozen: false);

                autoRespawnToClean.Add(playerToAutoRespawn);
            }
        }

        foreach (var playerToAutoRespawn in autoRespawnToClean)
        {
            AutoRespawn.Remove(playerToAutoRespawn);
            UIManager.GetUI(GetPlayer(playerToAutoRespawn)).ScoreTableVisibility = CUIConfig.EVisibility.None;
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
                    UIManager.GetUI(e.Player).ScoreTableVisibility = CUIConfig.EVisibility.ForcedVisible;
                }
                break;
            case CTmModeEvent.EType.GiveUp:
                if (AutoRespawn.ContainsKey(e.Player.User.Login))
                {
                    AutoRespawn.Remove(e.Player.User.Login);
                    UIManager.GetUI(e.Player).ScoreTableVisibility = CUIConfig.EVisibility.None;
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

            if (AutoRespawn.ContainsKey(spectator.User.Login))
            {
                AutoRespawn.Remove(spectator.User.Login);
                UIManager.GetUI(spectator).ScoreTableVisibility = CUIConfig.EVisibility.None;
            }
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
                    var spawned = SpawnEnvimixTimeAttackPlayer(player, car.Get(), frozen);

                    var isMenuEscape = e.CustomEventData.Count > 3 && e.CustomEventData[3] == "True";

                    if (spawned || isMenuEscape)
                    {
                        RequestEnvimaniaRecords(carName, MathLib.NearestInteger(player.GravityCoef * 10) - 10, GetLaps());
                    }
                }
                break;
        }
    }
}
