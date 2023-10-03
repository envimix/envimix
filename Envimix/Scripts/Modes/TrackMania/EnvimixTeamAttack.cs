using System.Numerics;

namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixTeamAttack : Envimix
{
    [Setting(As = "Time limit")]
    public int TimeLimit = 600;

    [Setting(As = "Car select time")]
    public int CarSelectTime = 10;

    [Setting(As = "Custom countdown")]
    public int CustomCountdown = -1;

    public override void OnServerInit()
    {
        CreateServersideLayers();

        UseClans = true;
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

    public override void OnUIEvent(CUIConfigEvent e)
    {
        switch (e.Type)
        {
            case CUIConfigEvent.EType.OnLayerCustomEvent:
                ProcessUpdateSkinEvent(e);
                ProcessUpdateCarEvent(e, forceFreeze: false);
                break;
        }

        // +++OnUIChatEvent+++
    }

    public override void OnMapInit()
    {
        ClearScores();
    }

    public override void OnMapLoad()
    {
        UseForcedClans = false; // Allow free team joining, won't be used soon

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
                        ProcessUpdateSkinEvent(e);
                        ProcessUpdateCarEvent(e, forceFreeze: true);
        
                        switch (e.CustomEventType)
                        {
                            case "JoinTeam":
                                Log(nameof(EnvimixTeamAttack), "JoinTeam");
                                break;
                        }
                        break;
                }
                //+++OnUIChatEvent+++
            }

            foreach (var e in PendingEvents)
            {
                switch (e.Type)
                {
                    case CTmModeEvent.EType.OnPlayerAdded:
                        PrepareJoinedPlayer(e.Player);
                        break;
                }
            }

            foreach (var player in PlayersWaiting)
            {
                TrySpawnEnvimixTeamAttackPlayer(player, frozen: true);
            }

            CheckEnvimaniaSession();
            Yield();
        }

        CarSelectionMode = false;

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

        /*declare Integer[Text] PlayerTeams;
        declare Integer[Integer] PlayerCounts;
        foreach(Score in Scores) {
            PlayerTeams[Score.User.Login] = Score.TeamNum;
            if(!PlayerCounts.existskey(Score.TeamNum))
                PlayerCounts[Score.TeamNum] = 0;
            PlayerCounts[Score.TeamNum] += 1;
        }

        if(PlayerCounts.existskey(1) && PlayerCounts.existskey(2)) {
            if(MathLib::Abs(PlayerCounts[1]-PlayerCounts[2]) > 1) {
                ServerAdmin.AutoTeamBalance();
            }
        }

        declare Barrier_Start = Synchro_AddBarrier();
        wait(Synchro_BarrierReached(Barrier_Start));

        foreach(Score in Scores) {
            if(PlayerTeams.existskey(Score.User.Login)) {
                if(PlayerTeams[Score.User.Login] != Score.TeamNum) {
                    log(Score.User.Login ^ " has been autobalanced from " ^ PlayerTeams[Score.User.Login] ^ " to " ^ Score.TeamNum);
                }
            }
        }*/

        UseForcedClans = true;
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
        if (frozen)
        {
            return SpawnEnvimixPlayer(player, car, frozen);
        }

        if (CutOffTimeLimit - Now >= TimeLimit * 1000)
        {
            return SpawnEnvimixPlayer(player, car, CutOffTimeLimit - TimeLimit * 1000);
        }

        if (CustomCountdown < 0)
        {
            return SpawnEnvimixPlayer(player, car, -1);
        }

        return SpawnEnvimixPlayer(player, car, Now + CustomCountdown);
    }

    public void RespawnAllWaiting()
    {
        foreach (var player in PlayersWaiting)
        {
            // In game loop and in time attack, this means when full respawn
            TrySpawnEnvimixTeamAttackPlayer(player, frozen: false);
        }
    }

    public override void OnEvent(CTmModeEvent e)
    {
        switch (e.Type)
        {
            case CTmModeEvent.EType.OnPlayerAdded:
                PrepareJoinedPlayer(e.Player);
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
    }

    public override void OnGameEnd()
    {
        Ladder_ComputeRank(CTmMode.ETmScoreSortOrder.TotalPoints);

        foreach (var score in Scores)
        {
            score.LadderRankSortValue = -1 - score.Points;
            score.LadderMatchScoreValue = score.Points * 1f;

            // log spam to understand the behaviour
            foreach (var s in Scores)
            {
                Log(nameof(EnvimixTeamAttack), $"Ladder: {s.User.Login} {s.LadderMatchScoreValue} {s.LadderRankSortValue} {s.User.ReferenceScore}");
            }
        }

        CloseLadder();
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
        CutOffTimeLimit = -1;
    }

    public void SetLaps()
    {
        IndependantLaps = ForceLapsNb == 0;

        if (ForceLapsNb > 0)
        {
            NbLaps = ForceLapsNb;
        }
        else
        {
            NbLaps = -1;
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

                    if (e.CustomEventData.Count > 1)
                    {
                        var respawn = e.CustomEventData[1] == "True";

                        if (respawn)
                        {
                            var frozen = forceFreeze || e.CustomEventData.Count > 2 && e.CustomEventData[2] == "True";
                            var spawned = SpawnEnvimixTeamAttackPlayer(player, car.Get(), frozen);

                            var isMenuEscape = e.CustomEventData.Count > 3 && e.CustomEventData[3] == "True";

                            if (spawned || isMenuEscape)
                            {
                                RequestEnvimaniaRecords(carName, MathLib.NearestInteger(player.GravityCoef * 10), IndependantLaps);
                            }
                        }
                    }
                }
                break;
        }
    }
}
