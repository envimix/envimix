using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class SoloMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadBack;
    [ManialinkControl] public required CMlFrame FrameCars;
    [ManialinkControl] public required CMlFrame FrameValidations;
    [ManialinkControl] public required CMlFrame FrameLeaderboards;
    [ManialinkControl] public required CMlQuad QuadTM2Cars;
    [ManialinkControl] public required CMlQuad QuadTMUFCars;
    [ManialinkControl] public required CMlFrame FrameCampaignOverview;
    [ManialinkControl] public required CMlFrame FrameMapOverview;
    [ManialinkControl] public required CMlFrame FrameCampaign;
    [ManialinkControl] public required CMlLabel LabelSelectedMapName;
    [ManialinkControl] public required CMlQuad QuadPlay;
    [ManialinkControl] public required CMlQuad QuadExplore;

    public ImmutableArray<string> TM2Cars;
    public ImmutableArray<string> TMUFCars;
    public ImmutableArray<string> FunnyCars;

    public CCampaign? Campaign;
    public int MapGroupNum = -1;
    public int MapInfoNum = -1;
    public int MapSelectedAt = -1;

    public SoloMenu()
    {
        QuadBack.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
        };

        QuadTM2Cars.MouseClick += () =>
        {
            SwitchCars(false);
        };

        QuadTMUFCars.MouseClick += () =>
        {
            SwitchCars(true);
        };

        QuadPlay.MouseClick += () =>
        {
            if (Campaign is not null && MapGroupNum != -1 && MapInfoNum != -1)
            {
                PlayMap(Campaign);
            }
        };

        QuadExplore.MouseClick += () =>
        {
            if (Campaign is not null && MapGroupNum != -1 && MapInfoNum != -1)
            {
                var fileName = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum].FileName;
                Log("Exploring map: " + fileName);
                TitleControl.EditNewMapFromBaseMap(fileName, ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "", "");
            }
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    ShowCampaignOverviewFrame();
                    ShowMapOverviewFrame();
                    break;
                case "AnimateClose":
                    HideCampaignOverviewFrame();
                    HideMapOverviewFrame();
                    break;
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapClick(control);
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapSelect(control);
            }
        };
    }

    public void Main()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        FrameMapOverview.RelativePosition_V3.Y = 10;

        TM2Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "" };
        TMUFCars = new() { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        FunnyCars = new() { "HighlandsCar", "DumpsterCar", "ToasterCar", "FunnyCar" };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        SwitchCars(false);
        SetupCampaign();
    }

    public void Loop()
    {
        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }
    }

    private void ShowCampaignOverviewFrame()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 80\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideCampaignOverviewFrame()
    {
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 -50\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void ShowMapOverviewFrame()
    {
        FrameMapOverview.RelativePosition_V3.Y = 10;
        AnimMgr.Add(FrameMapOverview, "<frame pos=\"-155 -41\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideMapOverviewFrame()
    {
        AnimMgr.Add(FrameMapOverview, "<frame pos=\"-155 10\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void SwitchCars(bool isTMUF)
    {
        ImmutableArray<string> cars;
        if (isTMUF)
        {
            cars = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
        }

        QuadTMUFCars.StyleSelected = isTMUF;
        QuadTM2Cars.StyleSelected = !isTMUF;

        for (int i = 0; i < FrameCars.Controls.Count; i++)
        {
            var controlCar = FrameCars.Controls[i];

            if (controlCar is not CMlLabel labelCar)
            {
                continue;
            }

            if (i >= cars.Length)
            {
                labelCar.Hide();
                continue;
            }

            var carName = cars[i];
            var missingCar = carName == "";
            if (missingCar)
            {
                var randomIndex = MathLib.Rand(0, FunnyCars.Length - 1);
                carName = FunnyCars[randomIndex] + "?";
            }

            labelCar.SetText(carName);

            var labelValidation = (FrameValidations.Controls[i] as CMlLabel)!;
            var frameLeaderboard = (FrameLeaderboards.Controls[i] as CMlFrame)!;

            if (missingCar)
            {
                labelValidation.SetText("$888...send us suggestions!");
                labelValidation.Show();
                frameLeaderboard.Hide();
            }
            else
            {
                labelValidation.SetText("$888you can validate this!");
                labelValidation.Hide(); // show after loading
                frameLeaderboard.Show();
            }

            labelCar.Show();
        }
    }

    private void SetupCampaign()
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        Campaign = DataFileMgr.Campaigns[0];

        var selectedNum = -1;

        for (int i = 0; i < FrameCampaign.Controls.Count; i++)
        {
            var controlDifficulty = FrameCampaign.Controls[i];

            if (controlDifficulty is not CMlFrame frameDifficulty)
            {
                continue;
            }

            CMapGroup? mapGroup = null;
            if (i < Campaign.MapGroups.Count)
            {
                mapGroup = Campaign.MapGroups[i];
            }

            var mapCounter = 0;
            foreach (var controlGroup in frameDifficulty.Controls)
            {
                if (controlGroup is not CMlFrame frameGroup)
                {
                    continue;
                }

                foreach (var controlMap in frameGroup.Controls)
                {
                    if (controlMap is not CMlFrame frameMap)
                    {
                        continue;
                    }

                    var quadMapThumbnail = (frameMap.GetFirstChild("QuadMapThumbnail") as CMlQuad)!;
                    var quadMapButton = (frameMap.GetFirstChild("QuadMapButton") as CMlQuad)!;
                    var quadMapName = (frameMap.GetFirstChild("QuadMapName") as CMlQuad)!;
                    var labelMapName = (frameMap.GetFirstChild("LabelMapName") as CMlLabel)!;
                    var labelSkillpoints = (frameMap.GetFirstChild("LabelSkillpoints") as CMlLabel)!;

                    if (mapGroup is null || mapCounter >= mapGroup.MapInfos.Count)
                    {
                        quadMapThumbnail.Hide();
                        quadMapButton.Hide();
                        quadMapName.Hide();
                        labelMapName.Hide();
                        labelSkillpoints.Hide();
                        continue;
                    }

                    var mapInfo = mapGroup.MapInfos[mapCounter];

                    quadMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                    quadMapThumbnail.Show();

                    labelMapName.SetText(TextLib.FormatInteger(mapCounter + 1, 3));
                    labelMapName.Show();

                    quadMapButton.DataAttributeSet("MapGroupNum", i.ToString());
                    quadMapButton.DataAttributeSet("MapInfoNum", mapCounter.ToString());
                    quadMapButton.Show();

                    var hovered = MapGroupNum == i && MapInfoNum == mapCounter;
                    quadMapButton.StyleSelected = MapSelectedAt != -1 && hovered;

                    if (hovered)
                    {
                        selectedNum = mapCounter;
                    }

                    quadMapName.Show();
                    labelSkillpoints.Hide();

                    mapCounter += 1;
                }
            }
        }

        if (MapGroupNum != -1 && MapInfoNum != -1)
        {
            var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];
            LabelSelectedMapName.SetText(TextLib.FormatInteger(selectedNum + 1, 3));
            QuadPlay.Visible = true;
        }
        else
        {
            LabelSelectedMapName.SetText("...");
            QuadPlay.Visible = false;
        }
    }

    private void PlayMap(CCampaign campaign)
    {
        TitleControl.PlayCampaign(campaign, campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum], "Modes/TrackMania/EnvimixSolo.Script.txt", "");
    }

    private void MapClick(CMlControl control)
    {
        if (Campaign is null)
        {
            return;
        }

        var mapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        var mapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        if (MapSelectedAt != -1 && MapGroupNum == mapGroupNum && MapInfoNum == mapInfoNum)
        {
            if ((Now - MapSelectedAt) < 500)
            {
                PlayMap(Campaign);
            }
            else
            {
                MapGroupNum = -1;
                MapInfoNum = -1;
                MapSelectedAt = -1;
                SetupCampaign();
            }
            return;
        }

        MapGroupNum = mapGroupNum;
        MapInfoNum = mapInfoNum;
        MapSelectedAt = Now;

        SetupCampaign();
    }

    private void MapSelect(CMlControl control)
    {
        if (MapSelectedAt != -1)
        {
            return;
        }

        MapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        MapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        SetupCampaign();
    }
}
