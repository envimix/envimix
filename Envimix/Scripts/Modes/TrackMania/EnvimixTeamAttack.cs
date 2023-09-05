namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixTeamAttack : Envimix
{
    [Setting(As = "Time limit")]
    public int TimeLimit = 600;

    [Setting(As = "Car select time")]
    public int CarSelectTime = 10;

    [Netwrite] public bool CarSelectionMode { get; set; }

    public override void OnServerInit()
    {
        Users_DestroyAllFakes();
        //Users_CreateFake("longlonglonglonglong longlonglonglonglong name", 0);

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
                var player = GetPlayer(e.UI);
                ProcessUpdateSkinEvent(e);
        
                switch (e.CustomEventType)
                {
                    case "Car":
                        if (e.CustomEventData.Count > 0)
                        {
                            var carName = e.CustomEventData[0];
                            var car = Netwrite<string>.For(player);

                            if (DisplayedCars.Contains(carName))
                            {
                                car.Set(carName);
                            }

                            if (e.CustomEventData.Count > 1)
                            {
                                var respawn = e.CustomEventData[1] == "True";

                                if (respawn)
                                {
                                    var frozen = e.CustomEventData.Count > 2 && e.CustomEventData[2] == "True";
                                    var spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
                                }
                            }
                        }
                        break;
                    case "JoinTeam":
                        Log(nameof(EnvimixTeamAttack), "JoinTeam");
                        break;
                }
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

        PrespawnPlayers();
    }

    private void PrespawnPlayers()
    {
        if (!ItemCars.ContainsValue(MapPlayerModelName))
        {
            Log(nameof(EnvimixTeamAttack), "NOTE: No item car was found of the current MapPlayerModelName. Players were not pre-spawned.");
            return;
        }

        // Pre-spawn all non-spec players with default car
        foreach (var player in PlayersWaiting)
        {
            var car = Netwrite<string>.For(player);
            car.Set(ItemCars.KeyOf(MapPlayerModelName));

            if (car.Get() == "")
            {
                Log(nameof(EnvimixTeamAttack), $"NOTE: {player.User.Name} has Net_Car set to empty string. Player was not pre-spawned.");
                continue;
            }

            var spawned = SpawnEnvimixPlayer(player, car.Get(), frozen: true);

            if (spawned)
            {
                Log(nameof(EnvimixTeamAttack), $"{player.User.Name} spawned");
            }
        }
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
        while (CutOffTimeLimit - Now > 0)
        {
            foreach (var e in UIManager.PendingEvents)
            {
                switch (e.Type)
                {
                    case CUIConfigEvent.EType.OnLayerCustomEvent:
                        ProcessUpdateSkinEvent(e);
        
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

            foreach (var player in Players)
            {
                TrySpawnEnvimixPlayer(player, frozen: true);
            }

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

        // Minor copypaste behaviour, worth refactoring
        foreach (var player in Players)
        {
            // why to reset notice again?
            NoticeMessage(UIManager.GetUI(player), "");

            TrySpawnEnvimixPlayer(player, frozen: false);
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

    private bool TrySpawnEnvimixPlayer(CTmPlayer player, bool frozen)
    {
        var clientCar = Netread<string>.For(UIManager.GetUI(player));
        var car = Netwrite<string>.For(player);

        // Validation of available cars, invalid car currently ignores changing anything
        if (DisplayedCars.Contains(clientCar.Get()))
        {
            car.Set(clientCar.Get());
        }

        bool spawned;
        if (frozen)
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
        else if (CutOffTimeLimit - Now < TimeLimit * 1000)
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
        else
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), CutOffTimeLimit - TimeLimit * 1000);
        }

        if (spawned)
        {
            Log(nameof(EnvimixTeamAttack), $"{player.User.Name} spawned");
        }

        if (!EnableDefaultCar && ItemCars[car.Get()] == GetDefaultCar())
        {
            NoticeMessage(UIManager.GetUI(player), "Default car is currently disabled.\n$ff0Please select another car.");
        }
        else if (CarSelectionMode)
        {
            NoticeMessage(UIManager.GetUI(player), $"You have selected $ff0{car.Get()}$g!\nPlease wait before the game starts.");
        }
        else
        {
            NoticeMessage(UIManager.GetUI(player), "");
        }

        return spawned;
    }

    private void ProcessUpdateSkinEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "Skin":
                if (e.CustomEventData.Count > 0)
                {
                    var carName = e.CustomEventData[0];
                    var player = GetPlayer(e.UI);
                    var car = Netwrite<string>.For(player);

                    if (DisplayedCars.Contains(carName) && car.Get() == carName)
                    {
                        if (e.CustomEventData.Count > 1)
                        {
                            var skin = e.CustomEventData[1];
                            UpdateSkin(player, skin);
                        }
                    }
                }
                break;
        }
    }

    public void RespawnAllWaiting()
    {
        foreach (var player in PlayersWaiting)
        {
            // In game loop and in time attack, this means when full respawn
            TrySpawnEnvimixPlayer(player, frozen: false);
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
}
