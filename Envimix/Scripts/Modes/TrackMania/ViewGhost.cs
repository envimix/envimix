namespace Envimix.Scripts.Modes.TrackMania;

public class ViewGhost : CTmMode, IContext
{
    [Setting] public string Car = "";
    [Setting] public string GhostUrl = "";

    public int GhostLength;
    public int StartTime;

    private void SpawnGhost(CGhost ghost)
    {
        var ghostIdent = RaceGhost_AddWithOffset(ghost, 0);

        UIManager.UIAll.SpectatorForcedTarget = ghostIdent;
        UIManager.UIAll.SpectatorForceCameraType = 1;
        UIManager.UIAll.SpectatorObserverMode = CUIConfig.EObserverMode.Forced;
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.EndRound;
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
        UIManager.UIAll.LabelsVisibility = CUIConfig.EHudVisibility.Nothing;
        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.SmallScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.PlayersPresentation;

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

        GhostLength = ghostTask.Ghost.Result.Time;
        StartTime = Now;

        SpawnGhost(ghostTask.Ghost);
    }

    public void Loop()
    {
        if (Now - StartTime > GhostLength + 3000)
        {
            StartTime = Now;
            UIManager.UIAll.UISequence = CUIConfig.EUISequence.PlayersPresentation;
            Sleep(100);
            UIManager.UIAll.UISequence = CUIConfig.EUISequence.EndRound;
        }
    }
}
