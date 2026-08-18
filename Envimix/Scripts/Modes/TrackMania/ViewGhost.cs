namespace Envimix.Scripts.Modes.TrackMania;

public class ViewGhost : CTmMode, IContext
{
    public struct SUniverseWaypoint
    {
        public string Login;
        public bool IsEndLap;
        public int LapTime;
        public int RaceTime;
        public int CheckpointInLap;
        public int CheckpointInRace;
    }

    [Setting(As = "<hidden>")] public string Car = "";
    [Setting(As = "<hidden>")] public string GhostUrl = "";

    [Netwrite] public required int RaceStartTime { get; set; }
    [Netwrite] public required int CurrentNbLaps { get; set; }

    public int StartTime;
    public int GhostLength;
    public IList<int> GhostCheckpoints;
    public int GhostCheckpointIndex = -1;
    public CUILayer CheckpointLayer;
    public int LapFinishTime = -1;

    private void SpawnGhost(CGhost ghost)
    {
        var ghostIdent = RaceGhost_AddWithOffset(ghost, 0);

        UIManager.UIAll.SpectatorForcedTarget = ghostIdent;
        UIManager.UIAll.SpectatorForceCameraType = 1;
        UIManager.UIAll.SpectatorObserverMode = CUIConfig.EObserverMode.Forced;
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.EndRound;
    }

    private void Reset()
    {
        StartTime = Now;
        RaceStartTime = StartTime + 2500;
        GhostCheckpointIndex = -1;
        LapFinishTime = -1;

        SUniverseWaypoint resetCp = new()
        {
            RaceTime = -1,
            LapTime = -1,
            CheckpointInLap = -1,
            CheckpointInRace = -1,
            IsEndLap = false,
            Login = ""
        };

        var checkpoint = Netwrite<SUniverseWaypoint>.For(Players[0]);
        checkpoint.Set(resetCp);
    }

    public void Main()
    {
        ItemList_Begin();

        ItemList_Add("CanyonCar");
        ItemList_Add("StadiumCar");
        ItemList_Add("ValleyCar");
        ItemList_Add("LagoonCar");
        ItemList_Add("TrafficCar");
        ItemList_Add("DesertCar");
        ItemList_Add("SnowCar");
        ItemList_Add("RallyCar");
        ItemList_Add("BayCar");
        ItemList_Add("IslandCar");
        ItemList_Add("CoastCar");

        ItemList_Add("Vehicles/BayCar.Item.Gbx");
        ItemList_Add("Vehicles/CanyonCar.Item.Gbx");
        ItemList_Add("Vehicles/CanyonCarTurbo.Item.Gbx");
        ItemList_Add("Vehicles/CoastCar.Item.Gbx");
        ItemList_Add("Vehicles/DesertCar.Item.Gbx");
        ItemList_Add("Vehicles/IslandCar.Item.Gbx");
        ItemList_Add("Vehicles/LagoonCar.Item.Gbx");
        ItemList_Add("Vehicles/LagoonCarTurbo.Item.Gbx");
        ItemList_Add("Vehicles/RallyCar.Item.Gbx");
        ItemList_Add("Vehicles/SnowCar.Item.Gbx");
        ItemList_Add("Vehicles/StadiumCar.Item.Gbx");
        ItemList_Add("Vehicles/StadiumCarTurbo.Item.Gbx");
        ItemList_Add("Vehicles/TrafficCar.Item.Gbx");
        ItemList_Add("Vehicles/ValleyCar.Item.Gbx");
        ItemList_Add("Vehicles/ValleyCarTurbo.Item.Gbx");

        ItemList_End();

        UIManager.UIAll.OverlayHideSpectatorControllers = true;
        UIManager.UIAll.OverlayHidePosition = true;
        UIManager.UIAll.OverlayHideBackground = true;
        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.SmallScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.PlayersPresentation;
        UIManager.UIAll.ScoreTableOnlyManialink = true;

        UIManager.HoldLoadingScreen = true;

        CreateLayer("321GoViewGhost");
        CreateLayer("MultilapViewGhost");
        CheckpointLayer = CreateLayer("Checkpoint2");

        RequestLoadMap();
        Wait(() => MapLoaded && Players.Count > 0);

        Map.MapName = TextLib.StripFormatting($"{Map.MapInfo.Name}.{Car}");

        RaceGhost_RemoveAll();

        CTaskResult_Ghost ghostTask;
        if (GhostUrl == "")
        {
            ghostTask = ScoreMgr.Map_GetRecordGhost(null, Map.MapInfo.MapUid, Car);
        }
        else
        {
            ghostTask = DataFileMgr.Ghost_Download("Replays/Downloaded/Envimix_Turbo@bigbang1112/Temp.Ghost.Gbx", GhostUrl);
        }

        Wait(() => !ghostTask.IsProcessing);

        if (ghostTask.HasFailed)
        {
            return;
        }

        if (ghostTask.Ghost is null)
        {
            // PBs driven in test mode dont store ghosts, so this is edgecase
            return;
        }

        UIManager.HoldLoadingScreen = false;

        GhostLength = ghostTask.Ghost.Result.Time;

        GhostCheckpoints.Clear();
        foreach (var checkpoint in ghostTask.Ghost.Result.Checkpoints)
        {
            GhostCheckpoints.Add(checkpoint);
        }

        Reset();

        SpawnGhost(ghostTask.Ghost);
    }

    public void Loop()
    {
        if (Now - StartTime > GhostLength + 3500)
        {
            UIManager.UIAll.UISequence = CUIConfig.EUISequence.PlayersPresentation;
            Sleep(100);
            UIManager.UIAll.UISequence = CUIConfig.EUISequence.EndRound;
            Reset();
        }

        if (GhostCheckpoints.Count > 0)
        {
            if (GhostCheckpointIndex < GhostCheckpoints.Count - 1 && Now - StartTime - 2500 > GhostCheckpoints[GhostCheckpointIndex + 1])
            {
                GhostCheckpointIndex += 1;

                var raceTime = GhostCheckpoints[GhostCheckpointIndex];
                var lapTime = raceTime;
                var checkpointIndexInLap = 0;
                var isEndLap = false;
                if (MapIsLapRace && MapCheckpointPos.Count > 0)
                {
                    checkpointIndexInLap = GhostCheckpointIndex % MapCheckpointPos.Count;
                    isEndLap = (GhostCheckpointIndex + 1) % (MapCheckpointPos.Count + 1) == 0;

                    if (LapFinishTime != -1)
                    {
                        lapTime = raceTime - LapFinishTime;
                    }

                    if (isEndLap)
                    {
                        LapFinishTime = GhostCheckpoints[GhostCheckpointIndex];
                    }

                    CurrentNbLaps = (GhostCheckpointIndex + 1) / MapCheckpointPos.Count;

                    if (CurrentNbLaps >= MapNbLaps)
                    {
                        CurrentNbLaps = MapNbLaps;
                    }
                }

                SUniverseWaypoint newCp = new()
                {
                    RaceTime = raceTime,
                    LapTime = lapTime,
                    CheckpointInLap = checkpointIndexInLap,
                    CheckpointInRace = GhostCheckpointIndex,
                    IsEndLap = isEndLap,
                    Login = Players[0].User.Login
                };

                var checkpoint = Netwrite<SUniverseWaypoint>.For(Players[0]);
                checkpoint.Set(newCp);
            }
        }
    }

    public required Dictionary<string, CUILayer> Layers;

    public void DestroyLayer(string layerName)
    {
        UIManager.UILayerDestroy(Layers[layerName]);
    }

    public CUILayer CreateLayer(string layerName)
    {
        if (Layers.ContainsKey(layerName))
        {
            DestroyLayer(layerName);
        }

        var request = Http.CreateGet($"file://Media/Manialinks/Universe2/{layerName}.xml");
        Wait(() => request.IsCompleted);

        var layer = UIManager.UILayerCreate();
        layer.Type = CUILayer.EUILayerType.Normal;
        layer.ManialinkPage = request.Result;
        Layers[layerName] = layer;
        UIManager.UIAll.UILayers.Add(layer);

        Http.Destroy(request);

        return layer;
    }
}
