using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class SoloMenu : CManiaAppTitleLayer, IContext
{
    public struct STitleUserInfo
    {
        public string N;
        public string Z;
    }

    public struct SCombinationStat
    {
        public string VL;
        public string VD;
        public float D;
        public float Q;
        public IList<int> S;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    public struct SEnvimaniaRecordsFilter
    {
        public string Car;
        public int Gravity;
        public int Laps;
        public string Type;
    }

    public struct SUserInfo
    {
        public string Login;
        public string Nickname;
        public string Zone;
        public string AvatarUrl;
        public string Language;
        public string Description;
        public Vec3 Color;
        public string SteamUserId;
        public int FameStars;
        public float LadderPoints;
    }

    public struct SEnvimaniaRecord
    {
        public SUserInfo User;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public bool Verified;
        public bool Projected;
        public string GhostUrl;
        public string DrivenAt;
        public bool Removed;
    }

    public struct SEnvimaniaRecordsResponse
    {
        public SEnvimaniaRecordsFilter Filter;
        public string Zone;
        public ImmutableArray<SEnvimaniaRecord> Records;
        public ImmutableArray<SEnvimaniaRecord> Validation;
        public ImmutableArray<int> Skillpoints;
        public string TitlePackReleaseTimestamp;
    }

    public struct SSuperMedals
    {
        public int Duck;
        public int STM;
    }

    [ManialinkControl] public required CMlQuad QuadBack;
    [ManialinkControl] public required CMlFrame FrameCars;
    [ManialinkControl] public required CMlFrame FrameValidations;
    [ManialinkControl] public required CMlFrame FrameRanks;
    [ManialinkControl] public required CMlFrame FrameLeaderboards;
    [ManialinkControl] public required CMlQuad QuadTM2Cars;
    [ManialinkControl] public required CMlQuad QuadTMUFCars;
    [ManialinkControl] public required CMlFrame FrameCampaignOverview;
    [ManialinkControl] public required CMlFrame FrameMapOverview;
    [ManialinkControl] public required CMlFrame FrameCampaign;
    [ManialinkControl] public required CMlLabel LabelSelectedMapName;
    [ManialinkControl] public required CMlQuad QuadPlay;
    [ManialinkControl] public required CMlQuad QuadExplore;
    [ManialinkControl] public required CMlLabel LabelSkillpoints;
    [ManialinkControl] public required CMlLabel LabelActivityPoints;
    [ManialinkControl] public required CMlLabel LabelCompletionPercentage;
    [ManialinkControl] public required CMlFrame FrameDifficultyRatings;
    [ManialinkControl] public required CMlFrame FrameQualityRatings;
    [ManialinkControl] public required CMlFrame FrameStars;
    [ManialinkControl] public required CMlFrame FrameRatingsCars;
    [ManialinkControl] public required CMlQuad QuadQuickplay;

    [ManialinkControl] public required CMlQuad QuadOfficialCampaign;
    [ManialinkControl] public required CMlQuad QuadVRCampaign;
    [ManialinkControl] public required CMlLabel LabelVRCampaign;

    public ImmutableArray<string> TM2Cars;
    public ImmutableArray<string> TMUFCars;
    public ImmutableArray<string> FunnyCars;
    public ImmutableArray<string> AllCars;

    public CCampaign? Campaign;
    public int MapGroupNum = -1;
    public int MapInfoNum = -1;
    public int MapSelectedAt = -1;
    public int StatsAt = -1;
    public int StatsLocalAt = -1;
    public int SkillpointsReceivedAt = -1;
    public int ActivityPointsReceivedAt = -1;

    public int CurrentSkillpoints;
    public int CurrentActivityPoints;
    public float CurrentCompletionPercentage;
    public int ExpectedSkillpoints;
    public int ExpectedActivityPoints;
    public float ExpectedCompletionPercentage;

    public string ScoreContextPrefix = "";

    public bool IsTMUF;

    /*[Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SRating>> TitleRatings { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SStar>> TitleStars { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SValidationInfo>> TitleValidations { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, IList<int>>> TitleSkillpoints { get; set; }*/
    [Local(LocalFor.LocalUser)] public Dictionary<string, string> CampaignsReleasedAt { get; set; }

    public Dictionary<string, Dictionary<string, SEnvimaniaRecordsResponse>> Leaderboards;
    public Dictionary<string, int> LeaderboardRequestTimestamps;

    public Dictionary<string, Dictionary<string, string>> TitleStars;
    public Dictionary<string, Dictionary<string, SCombinationStat>> TitleCombinations;
    public Dictionary<string, STitleUserInfo> SoloUserInfos;

    public CAudioSource AudioClick;

    public CMlQuad SelectedCampaignQuad;

    public CMlQuad? FocusedControl;

    public bool VRCampaignReleased;
    public bool LeaderboardsLoadedOrLoading;

    public Dictionary<CMlFrame, float> PrevLeaderboardScrollY;

    public CHttpRequest? SuperMedalsRequest;
    public Dictionary<string, SSuperMedals> SuperMedals;

    public SoloMenu()
    {
        QuadBack.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
            AudioPlayClick();
        };

        QuadBack.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadTM2Cars.MouseClick += () =>
        {
            SwitchCars(false);
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadTM2Cars.MouseOver += () =>
        {
            if (IsTMUF)
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
            }
        };

        QuadTMUFCars.MouseClick += () =>
        {
            SwitchCars(true);
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadTMUFCars.MouseOver += () =>
        {
            if (!IsTMUF)
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
            }
        };

        QuadPlay.MouseClick += () =>
        {
            AudioPlayClick();
            if (MapGroupNum != -1 && MapInfoNum != -1)
            {
                PlaySelectedMap();
            }
        };

        QuadPlay.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        QuadExplore.MouseClick += () =>
        {
            AudioPlayClick();
            if (MapGroupNum != -1 && MapInfoNum != -1)
            {
                ExploreSelectedMap();
            }
        };

        QuadExplore.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        QuadQuickplay.MouseClick += () =>
        {
            AudioPlayClick();
            SendCustomEvent("Quickplay", new[] { "" });
        };

        QuadQuickplay.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        QuadOfficialCampaign.MouseClick += () =>
        {
            SelectOfficialCampaign();
        };

        QuadVRCampaign.MouseClick += () =>
        {
            SelectVRCampaign();
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    ShowCampaignOverviewFrame();
                    ShowMapOverviewFrame();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    HideCampaignOverviewFrame();
                    HideMapOverviewFrame();
                    break;
                case "GeneralStats":
                    var titleStars = Local<Dictionary<string, Dictionary<string, string>>>.For(Page);
                    var soloUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);
                    var titleCombinations = Local<Dictionary<string, Dictionary<string, SCombinationStat>>>.For(Page);
                    TitleStars = titleStars.Get();
                    SoloUserInfos = soloUserInfos.Get();
                    TitleCombinations = titleCombinations.Get();

                    // weird to call it at SetPoints, but it allows updating the solo menu stats when nothing is clicked
                    if (DataFileMgr.Campaigns.Count > 0 && MapGroupNum != -1 && MapInfoNum != -1)
                    {
                        UpdateGeneralStats(GetCampaignForMaps().MapGroups[MapGroupNum].MapInfos[MapInfoNum]);
                    }
                    SetupCampaign(false, false);
                    break;
                case "Skillpoints":
                    ExpectedSkillpoints = TextLib.ToInteger(data[0]);
                    SkillpointsReceivedAt = Now;
                    if (EnableMenuNavigationInputs && ExpectedSkillpoints > CurrentSkillpoints)
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: i * 100);
                        }
                    }
                    break;
                case "ActivityPoints":
                    ExpectedActivityPoints = TextLib.ToInteger(data[0]);
                    ActivityPointsReceivedAt = Now;
                    if (EnableMenuNavigationInputs && ExpectedActivityPoints > CurrentActivityPoints)
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: i * 100);
                        }
                    }
                    break;
                case "LeaderboardData":
                    var mapUid = data[0];
                    var car = data[1];
                    var recordsJson = data[2];
                    SEnvimaniaRecordsResponse response = new();
                    response.FromJson(recordsJson);
                    ProcessLeaderboardData(mapUid, car, response);
                    break;
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapClick(control);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            }
            if (control.Parent.ControlId == "FrameRecords")
            {
                var car = control.DataAttributeGet("Car");
                var ghostUrl = control.DataAttributeGet("GhostUrl");
                if (ghostUrl != "")
                {
                    ViewGhostSelectedMap(car, ghostUrl);
                }
            }
            if (controlId == "LabelPersonalBest")
            {
                var car = control.DataAttributeGet("Car");
                var time = control.DataAttributeGet("Time");
                if (time != "-1")
                {
                    ViewGhostSelectedMap(car, "");
                }
            }
            if (FocusedControl is not null)
            {
                FocusedControl.StyleSelected = false;
                FocusedControl = null;
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapSelect(control);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("MainMenu", new[] { "" });
                    break;
                case CMlScriptEvent.EMenuNavAction.Up:
                    if (FocusedControl is null)
                    {
                        if (MapInfoNum == -1)
                        {
                            MapInfoNum = 0;
                            MapGroupNum = 0;
                        }
                        else
                        {
                            if (MapInfoNum % 10 < 5)
                            {
                                if (MapGroupNum == 0)
                                {
                                    if (MapInfoNum is 0 or 1 or 5 or 6)
                                    {
                                        FocusedControl = QuadQuickplay;
                                    }
                                    if (MapInfoNum is 2 or 3 or 4 or 7 or 8 or 9)
                                    {
                                        FocusedControl = QuadOfficialCampaign;
                                    }
                                    if (MapInfoNum >= 10)
                                    {
                                        FocusedControl = QuadVRCampaign;
                                    }
                                }
                                else
                                {
                                    MapGroupNum -= 1;
                                    MapInfoNum += 5;
                                }
                            }
                            else
                            {
                                MapInfoNum -= 5;
                            }
                        }
                        SetupCampaign(false, true);
                        UnloadLeaderboards();
                    }
                    if (FocusedControl is not null)
                    {
                        FocusedControl.StyleSelected = true;
                    }
                    break;
                case CMlScriptEvent.EMenuNavAction.Down:
                    if (FocusedControl is null)
                    {
                        if (MapInfoNum == -1)
                        {
                            MapInfoNum = 0;
                            MapGroupNum = 0;
                        }
                        else
                        {
                            if (MapInfoNum % 10 < 5)
                            {
                                MapInfoNum += 5;
                            }
                            else
                            {
                                if (MapGroupNum == GetCampaignForMaps().MapGroups.Count - 1)
                                {
                                }
                                else
                                {
                                    MapGroupNum += 1;
                                    MapInfoNum -= 5;
                                }
                            }
                        }
                        SetupCampaign(false, true);
                        UnloadLeaderboards();
                    }
                    else if (FocusedControl == QuadQuickplay || FocusedControl == QuadOfficialCampaign || FocusedControl == QuadVRCampaign)
                    {
                        SetupCampaign(false, false);
                        UnloadLeaderboards();
                        FocusedControl.StyleSelected = SelectedCampaignQuad == FocusedControl;
                        FocusedControl = null;
                    }
                    if (FocusedControl is not null)
                    {
                        FocusedControl.StyleSelected = true;
                    }
                    break;
                case CMlScriptEvent.EMenuNavAction.Left:
                    if (FocusedControl is null)
                    {
                        if (MapInfoNum == -1)
                        {
                            MapInfoNum = 0;
                            MapGroupNum = 0;
                        }
                        else
                        {
                            if (MapInfoNum == 0)
                            {
                                MapInfoNum = 34;
                            }
                            else if (MapInfoNum == 5)
                            {
                                MapInfoNum = 39;
                            }
                            else
                            {
                                if (MapInfoNum % 5 == 0)
                                {
                                    MapInfoNum -= 6;
                                }
                                else
                                {
                                    MapInfoNum -= 1;
                                }
                            }
                        }
                        SetupCampaign(false, true);
                        UnloadLeaderboards();
                    }
                    else
                    {
                        FocusedControl.StyleSelected = SelectedCampaignQuad == FocusedControl;
                    }
                    if (FocusedControl == QuadQuickplay)
                    {
                        FocusedControl = QuadVRCampaign;
                    }
                    else if (FocusedControl == QuadOfficialCampaign)
                    {
                        FocusedControl = QuadQuickplay;
                    }
                    else if (FocusedControl == QuadVRCampaign)
                    {
                        FocusedControl = QuadOfficialCampaign;
                    }
                    if (FocusedControl is not null)
                    {
                        FocusedControl.StyleSelected = true;
                    }
                    break;
                case CMlScriptEvent.EMenuNavAction.Right:
                    if (FocusedControl is null)
                    {
                        if (MapInfoNum == -1)
                        {
                            MapInfoNum = 0;
                            MapGroupNum = 0;
                        }
                        else
                        {
                            if (MapInfoNum == 34)
                            {
                                MapInfoNum = 0;
                            }
                            else if (MapInfoNum == 39)
                            {
                                MapInfoNum = 5;
                            }
                            else
                            {
                                if (MapInfoNum % 5 == 4)
                                {
                                    MapInfoNum += 6;
                                }
                                else
                                {
                                    MapInfoNum += 1;
                                }
                            }
                        }
                        SetupCampaign(false, true);
                        UnloadLeaderboards();
                    }
                    else
                    {
                        FocusedControl.StyleSelected = SelectedCampaignQuad == FocusedControl;
                    }
                    if (FocusedControl == QuadQuickplay)
                    {
                        FocusedControl = QuadOfficialCampaign;
                    }
                    else if (FocusedControl == QuadOfficialCampaign)
                    {
                        FocusedControl = QuadVRCampaign;
                    }
                    else if (FocusedControl == QuadVRCampaign)
                    {
                        FocusedControl = QuadQuickplay;
                    }
                    if (FocusedControl is not null)
                    {
                        FocusedControl.StyleSelected = true;
                    }
                    break;
                case CMlScriptEvent.EMenuNavAction.Select:
                    if (FocusedControl is null)
                    {
                        if (MapGroupNum != -1 && MapInfoNum != -1)
                        {
                            if (LeaderboardsLoadedOrLoading)
                            {
                                PlaySelectedMap();
                            }
                            else
                            {
                                LoadLeaderboards(true);
                            }
                        }
                    }
                    else
                    {
                        if (FocusedControl == QuadQuickplay)
                        {
                            SendCustomEvent("Quickplay", new[] { "" });
                        }
                        else if (FocusedControl == QuadOfficialCampaign)
                        {
                            SelectOfficialCampaign();
                        }
                        else if (FocusedControl == QuadVRCampaign)
                        {
                            SelectVRCampaign();
                        }
                    }
                    break;
            }
        };
    }

    public void Main()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        FrameMapOverview.RelativePosition_V3.Y = 10;

        TM2Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "" };
        TMUFCars = new() { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        FunnyCars = new() { "HighlandCar", "DumpsterCar", "ToasterCar", "FunnyCar" };
        AllCars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        SelectedCampaignQuad = QuadOfficialCampaign;

        SwitchCars(false);
        SetupCampaign(false, false);

        SuperMedalsRequest = Http.CreateGet("file://Media/Medals.json");
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

        // Update stats every minute
        if (StatsAt == -1 || (Now - StatsAt) > 60000)
        {
            Log("Updating stats...");

            var finishedEnvimixCount = 0;
            var totalEnvimixCount = 0;

            // takes the official campaign only
            for (var i = 0; i < MathLib.Min(12, DataFileMgr.Campaigns.Count); i++)
            {
                var campaign = DataFileMgr.Campaigns[i];

                foreach (var mapGroup in campaign.MapGroups)
                {
                    foreach (var mapInfo in mapGroup.MapInfos)
                    {
                        if (campaign.ScoreContext == ScoreContextPrefix)
                        {
                            continue;
                        }

                        // a bit of a hack but clientside on turbo maps it works - ensures no false positives
                        if (mapInfo.CollectionName == "Canyon" && campaign.ScoreContext == ScoreContextPrefix + "CanyonCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Stadium" && campaign.ScoreContext == ScoreContextPrefix + "StadiumCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Valley" && campaign.ScoreContext == ScoreContextPrefix + "ValleyCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Lagoon" && campaign.ScoreContext == ScoreContextPrefix + "LagoonCar")
                        {
                            continue;
                        }

                        if (ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, campaign.ScoreContext) != -1)
                        {
                            finishedEnvimixCount += 1;
                        }

                        totalEnvimixCount += 1;
                    }
                }
            }

            LoadLeaderboards(false);

            SendCustomEvent("Stats", new[] { "" });

            if (totalEnvimixCount == 0)
            {
                ExpectedCompletionPercentage = 0;
            }
            else
            {
                ExpectedCompletionPercentage = finishedEnvimixCount * 1f / totalEnvimixCount * 100;
            }

            StatsAt = Now;
            StatsLocalAt = Now;
        }

        if (StatsLocalAt != -1)
        {
            var duration = 1000;
            var time = Now - StatsLocalAt;

            var percentageDiff = ExpectedCompletionPercentage - CurrentCompletionPercentage;
            var percentage = AnimLib.EaseOutQuad(time, CurrentCompletionPercentage, percentageDiff, duration);

            var percentageText = TextLib.FormatReal(percentage, 2, false, true);

            LabelCompletionPercentage.Value = $"Envimix $ff0Turbo$g completion: $o{percentageText}%";

            if (time >= duration)
            {
                StatsLocalAt = -1;
                CurrentCompletionPercentage = ExpectedCompletionPercentage;
            }
        }

        if (SkillpointsReceivedAt != -1)
        {
            var duration = 1000;
            var time = Now - SkillpointsReceivedAt;

            var skillpointsDiff = ExpectedSkillpoints - CurrentSkillpoints;
            var skillpoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentSkillpoints), MathLib.ToReal(skillpointsDiff), duration));

            LabelSkillpoints.Value = FormatNumberSpace(skillpoints);

            var noPointDiff = skillpointsDiff == 0;

            if (time >= duration || noPointDiff)
            {
                SkillpointsReceivedAt = -1;
                CurrentSkillpoints = ExpectedSkillpoints;
            }
        }

        if (ActivityPointsReceivedAt != -1)
        {
            var duration = 1000;
            var time = Now - ActivityPointsReceivedAt;

            var activityPointsDiff = ExpectedActivityPoints - CurrentActivityPoints;
            var activityPoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentActivityPoints), MathLib.ToReal(activityPointsDiff), duration));

            LabelActivityPoints.Value = FormatNumberSpace(activityPoints);

            var noPointDiff = activityPointsDiff == 0;

            if (time >= duration || noPointDiff)
            {
                ActivityPointsReceivedAt = -1;
                CurrentActivityPoints = ExpectedActivityPoints;
            }
        }

        if (CampaignsReleasedAt.ContainsKey("VR"))
        {
            var releasedAt = CampaignsReleasedAt["VR"];
            
            if (TimeLib.Compare(releasedAt, TimeLib.GetCurrent()) > 0)
            {
                LabelVRCampaign.Value = TimeLib.FormatDelta(releasedAt, TimeLib.GetCurrent(), TimeLib.EDurationFormats.Abbreviated);
            }
            else
            {
                LabelVRCampaign.Value = "VR campaign";
                VRCampaignReleased = true;
            }
        }

        foreach (var control in FrameLeaderboards.Controls)
        {
            var frameScrollRecords = ((control as CMlFrame)!.GetFirstChild("FrameScrollRecords") as CMlFrame)!;

            if (!PrevLeaderboardScrollY.ContainsKey(frameScrollRecords))
            {
                PrevLeaderboardScrollY[frameScrollRecords] = 0;
            }

            if (frameScrollRecords.ScrollOffset.Y != PrevLeaderboardScrollY[frameScrollRecords])
            {
                PrevLeaderboardScrollY[frameScrollRecords] = (float)frameScrollRecords.ScrollOffset.Y;

                var frameRecords = (frameScrollRecords.GetFirstChild("FrameRecords") as CMlFrame)!;
                var quadRecordsScrollArea = (frameScrollRecords.GetFirstChild("QuadRecordsScrollArea") as CMlQuad)!;

                frameRecords.RelativePosition_V3.Y = -PrevLeaderboardScrollY[frameScrollRecords];
                quadRecordsScrollArea.RelativePosition_V3.Y = -PrevLeaderboardScrollY[frameScrollRecords];

                UpdateLeaderboards();
            }
        }

        if (SuperMedalsRequest is not null && SuperMedalsRequest.IsCompleted)
        {
            if (SuperMedalsRequest.StatusCode == 200)
            {
                SuperMedals.FromJson(SuperMedalsRequest.Result);
            }
            Http.Destroy(SuperMedalsRequest);
            SuperMedalsRequest = null;

            SetupCampaign(false, false);
        }
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private string FormatNumberSpace(int number)
    {
        var txt = TextLib.ToText(number);
        if (number < 0)
        {
            txt = TextLib.SubText(txt, 1, TextLib.Length(txt) - 1);
        }
        var result = "";
        var len = TextLib.Length(txt);
        var count = 0;

        for (var i = 0; i < len; i++)
        {
            result = $"{TextLib.SubText(txt, len - 1 - i, 1)}{result}";
            count += 1;

            if (count == 3 && i < len - 1)
            {
                result = $" {result}";
                count = 0;
            }
        }

        if (number < 0)
        {
            result = $"-{result}";
        }

        return result;
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

    private static int GetLaps(CMapInfo mapInfo)
    {
        if (!mapInfo.TMObjective_IsLapRace)
        {
            return 1;
        }

        return mapInfo.TMObjective_NbLaps;
    }

    private void ResetPBs()
    {
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }
            var labelPersonalBest = (frameLeaderboard.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            labelPersonalBest.SetText("--:--.---");
        }
    }

    private void ResetValidators()
    {
        foreach (var control in FrameValidations.Controls)
        {
            control.Hide();
        }
    }

    private void ResetRatings()
    {
        foreach (var control in FrameDifficultyRatings.Controls)
        {
            if (control is not CMlGauge gaugeDifficulty)
            {
                continue;
            }
            gaugeDifficulty.Ratio = 0;
            gaugeDifficulty.Color = new Vec3(1, 0, 0);
        }
        foreach (var control in FrameQualityRatings.Controls)
        {
            if (control is not CMlGauge gaugeQuality)
            {
                continue;
            }
            gaugeQuality.Ratio = 0;
            gaugeQuality.Color = new Vec3(1, 0, 0);
        }
    }

    private void ResetStars()
    {
        foreach (var control in FrameStars.Controls)
        {
            control.Hide();
        }
    }

    private void ResetRanks()
    {
        foreach (var control in FrameRanks.Controls)
        {
            if (control is not CMlLabel labelRank)
            {
                continue;
            }
            labelRank.SetText("-/-");
        }
    }

    private void UpdatePBs(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        var carIndex = 0;
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            var scoreContext = $"{ScoreContextPrefix}{carName}";

            // hacky but it works for TMT
            if ((mapInfo.CollectionName == "Canyon" && carName == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && carName == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && carName == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && carName == "LagoonCar"))
            {
                scoreContext = ScoreContextPrefix;
            }

            var labelPersonalBest = (frameLeaderboard.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            labelPersonalBest.DataAttributeSet("Car", carName);

            var time = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);
            labelPersonalBest.DataAttributeSet("Time", time.ToString());

            if (time == -1)
            {
                labelPersonalBest.SetText("--:--.---");
            }
            else
            {
                labelPersonalBest.SetText(TimeToTextWithMilli(time));
            }

            carIndex += 1;
        }
    }

    private void UpdateValidators(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        var carIndex = 0;
        foreach (var control in FrameValidations.Controls)
        {
            if (control is not CMlLabel labelValidation)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            labelValidation.Show();

            if (carName == "")
            {
                labelValidation.SetText("$888...send us suggestions!");
                carIndex += 1;
                continue;
            }

            // hacky but it works for TMT
            if ((mapInfo.CollectionName == "Canyon" && carName == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && carName == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && carName == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && carName == "LagoonCar"))
            {
                labelValidation.SetText($"validated by {mapInfo.AuthorNickName}");
                carIndex += 1;
                continue;
            }

            var combinationKey = $"{carName}_0";

            if (!TitleCombinations.ContainsKey(mapInfo.MapUid) || !TitleCombinations[mapInfo.MapUid].ContainsKey(combinationKey))
            {
                labelValidation.SetText("$888you can validate this!");
                carIndex += 1;
                continue;
            }

            var combination = TitleCombinations[mapInfo.MapUid][combinationKey];
            var validationNickname = combination.VL;
            if (SoloUserInfos.ContainsKey(validationNickname))
            {
                validationNickname = SoloUserInfos[validationNickname].N;
            }
            labelValidation.SetText($"validated by {validationNickname}");
            carIndex += 1;
        }
    }

    private void UpdateRanks(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        var carIndex = 0;

        foreach (var control in FrameRanks.Controls)
        {
            if (control is not CMlLabel labelRank)
            {
                continue;
            }

            var carName = cars[carIndex];

            var scoreContext = $"{ScoreContextPrefix}{carName}";

            // hacky but it works for TMT
            if ((mapInfo.CollectionName == "Canyon" && carName == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && carName == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && carName == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && carName == "LagoonCar"))
            {
                scoreContext = ScoreContextPrefix;
            }

            var playerTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);

            var combinationKey = $"{carName}_0";

            if (!TitleCombinations.ContainsKey(mapInfo.MapUid) || !TitleCombinations[mapInfo.MapUid].ContainsKey(combinationKey))
            {
                labelRank.SetText("-/0");
                carIndex += 1;
                continue;
            }

            var skillpointsList = TitleCombinations[mapInfo.MapUid][combinationKey].S;

            var rank = 1;
            var totalCount = 0;
            for (var i = 0; i < skillpointsList.Count / 2; i++)
            {
                var time = skillpointsList[i * 2];
                var count = skillpointsList[i * 2 + 1];
                totalCount += count;

                if (time < playerTime)
                {
                    rank += count;
                    continue;
                }
            }

            if (playerTime == -1)
            {
                labelRank.SetText($"-/{totalCount}");
            }
            else
            {
                labelRank.SetText($"{rank}/{totalCount}");
            }

            carIndex += 1;
        }
    }

    private void UpdateRatings(CMapInfo mapInfo)
    {
        var carIndex = 0;
        foreach (var car in AllCars)
        {
            var difficultyGauge = (FrameDifficultyRatings.Controls[carIndex] as CMlGauge)!;
            var qualityGauge = (FrameQualityRatings.Controls[carIndex] as CMlGauge)!;
            var carLabel = (FrameRatingsCars.Controls[carIndex] as CMlLabel)!;

            // hacky but it works for TMT
            var isDefaultCar = (mapInfo.CollectionName == "Canyon" && car == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && car == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && car == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && car == "LagoonCar");

            var combinationKey = $"{car}_0";

            if (TitleCombinations.ContainsKey(mapInfo.MapUid) && TitleCombinations[mapInfo.MapUid].ContainsKey(combinationKey))
            {
                var combination = TitleCombinations[mapInfo.MapUid][combinationKey];
                if (combination.D < 0)
                {
                    difficultyGauge.Ratio = 0;
                }
                else
                {
                    difficultyGauge.Ratio = combination.D;
                }
                if (combination.Q < 0)
                {
                    qualityGauge.Ratio = 0;
                }
                else
                {
                    qualityGauge.Ratio = combination.Q;
                }
                difficultyGauge.Color = new Vec3(1, 1, 1);
                qualityGauge.Color = new Vec3(1, 1, 1);
                carLabel.TextColor = new Vec3(1, 1, 1);
            }
            else
            {
                difficultyGauge.Ratio = 0;
                qualityGauge.Ratio = 0;

                if (isDefaultCar)
                {
                    difficultyGauge.Color = new Vec3(1, 1, 1);
                    qualityGauge.Color = new Vec3(1, 1, 1);
                    carLabel.TextColor = new Vec3(1, 1, 1);
                }
                else
                {
                    difficultyGauge.Color = new Vec3(1, 0, 0);
                    qualityGauge.Color = new Vec3(1, 0, 0);
                    carLabel.TextColor = new Vec3(1, 0, 0);
                }
            }

            carIndex += 1;
        }
    }

    private void UpdateStars(CMapInfo mapInfo)
    {
        var carIndex = 0;
        
        foreach (var car in AllCars)
        {
            var controlStar = FrameStars.Controls[carIndex];

            var filterKey = $"{car}_0_Time";

            if (TitleStars.ContainsKey(mapInfo.MapUid) && TitleStars[mapInfo.MapUid].ContainsKey(filterKey))
            {
                var star = TitleStars[mapInfo.MapUid][filterKey];
                controlStar.Show();
            }
            else
            {
                controlStar.Hide();
            }

            carIndex += 1;
        }
    }

    private void UpdateGeneralStats(CMapInfo selectedMapInfo)
    {
        if (IsTMUF)
        {
            UpdatePBs(selectedMapInfo, TMUFCars);
            UpdateValidators(selectedMapInfo, TMUFCars);
            UpdateRanks(selectedMapInfo, TMUFCars);
        }
        else
        {
            UpdatePBs(selectedMapInfo, TM2Cars);
            UpdateValidators(selectedMapInfo, TM2Cars);
            UpdateRanks(selectedMapInfo, TM2Cars);
        }

        UpdateRatings(selectedMapInfo);
        UpdateStars(selectedMapInfo);
    }

    private CCampaign GetCampaignForMaps()
    {
        if (SelectedCampaignQuad == QuadOfficialCampaign)
        {
            return DataFileMgr.Campaigns[0];
        }

        if (SelectedCampaignQuad == QuadVRCampaign)
        {
            return DataFileMgr.Campaigns[12];
        }

        return DataFileMgr.Campaigns[0];
    }

    private void UpdateLeaderboards()
    {
        if (DataFileMgr.Campaigns.Count == 0 || MapGroupNum == -1 || MapInfoNum == -1 || MapSelectedAt == -1)
        {
            return;
        }

        Campaign = GetCampaignForMaps();
        var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];

        ImmutableArray<string> cars = new();
        if (IsTMUF)
        {
            cars = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
        }

        var carIndex = 0;
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
            var frameScrollRecords = (frameLeaderboard.GetFirstChild("FrameScrollRecords") as CMlFrame)!;
            var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;
            var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;

            var scrollIndex = MathLib.NearestInteger((float)frameScrollRecords.ScrollOffset.Y / 5);

            labelConfirm.Hide();

            carIndex += 1;

            if (!Leaderboards.ContainsKey(selectedMapInfo.MapUid) || !Leaderboards[selectedMapInfo.MapUid].ContainsKey(carName))
            {
                // check for any error
                continue;
            }

            var lb = Leaderboards[selectedMapInfo.MapUid][carName];

            frameScrollRecords.ScrollMax.Y = MathLib.Max(0f, (lb.Records.Length - 10) * 5f);

            quadLoadingLeaderboard.Hide();

            if (lb.Records.Length == 0)
            {
                var labelYouCouldBeHere = (frameRecords.Controls[0] as CMlLabel)!;
                labelYouCouldBeHere.SetText("01 -:--.---  $i$888you could be here!");
                labelYouCouldBeHere.DataAttributeSet("Car", carName);
                labelYouCouldBeHere.DataAttributeSet("GhostUrl", "");
                labelYouCouldBeHere.Show();
                labelYouCouldBeHere.Opacity = 1;

                for (var i = 1; i < frameRecords.Controls.Count; i++)
                {
                    frameRecords.Controls[i].Hide();
                }
                continue;
            }

            var rankIndex = scrollIndex;
            var prevTime = -1;
            var rankOffset = 0;

            foreach (var controlRec in frameRecords.Controls)
            {
                if (lb.Records.Length <= rankIndex)
                {
                    controlRec.Hide();
                    continue;
                }

                var record = lb.Records[rankIndex];

                if (prevTime == record.Time)
                {
                    rankOffset += 1;
                }
                else
                {
                    prevTime = record.Time;
                    rankOffset = 0;
                }

                var labelRec = (controlRec as CMlLabel)!;
                labelRec.SetText($"{TextLib.FormatInteger(rankIndex + 1 - rankOffset, 2)} {TimeToTextWithMilli(record.Time)}  {record.User.Nickname}");
                labelRec.DataAttributeSet("Car", carName);
                labelRec.DataAttributeSet("GhostUrl", record.GhostUrl);
                labelRec.Show();

                if (record.Removed)
                {
                    labelRec.Opacity = 0.5f;
                }
                else
                {
                    labelRec.Opacity = 1;
                }

                rankIndex += 1;
            }
        }
    }

    private void LoadLeaderboards(bool showLoader)
    {
        if (DataFileMgr.Campaigns.Count == 0 || MapGroupNum == -1 || MapInfoNum == -1)
        {
            return;
        }

        LeaderboardsLoadedOrLoading = true;

        Campaign = GetCampaignForMaps();
        var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];

        var mapUid = selectedMapInfo.MapUid;
        var laps = GetLaps(selectedMapInfo);

        ImmutableArray<string> cars = new();
        ImmutableArray<string> carsToRequest = new();

        if (IsTMUF)
        {
            cars = TMUFCars;
            carsToRequest = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
            carsToRequest = TM2Cars;
        }

        ImmutableArray<string> carsToNotRequest = new();

        foreach (var car in carsToRequest)
        {
            var timestampKey = $"{mapUid}_{car}_{laps}";

            if (LeaderboardRequestTimestamps.ContainsKey(timestampKey) && Now - LeaderboardRequestTimestamps[timestampKey] < 50000)
            {
                carsToNotRequest.Add(car);
            }
            else
            {
                LeaderboardRequestTimestamps[timestampKey] = Now;
            }
        }

        foreach (var car in carsToNotRequest)
        {
            carsToRequest.Remove(car);
        }

        if (carsToRequest.Length == 0)
        {
            UpdateLeaderboards();
            return;
        }

        if (showLoader)
        {
            var carIndex = 0;
            foreach (var control in FrameLeaderboards.Controls)
            {
                if (control is not CMlFrame frameLeaderboard)
                {
                    continue;
                }

                var carName = cars[carIndex];
                if (!carsToRequest.Contains(carName))
                {
                    continue;
                }

                var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
                var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;
                var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

                quadLoadingLeaderboard.Show();
                labelConfirm.Hide();

                foreach (var controlRec in frameRecords.Controls)
                {
                    controlRec.Hide();
                }

                carIndex += 1;
            }
        }

        SendCustomEvent("LoadLeaderboards", new[] { mapUid, laps.ToString(), carsToRequest.ToJson() });
    }

    private void ProcessLeaderboardData(string mapUid, string car, SEnvimaniaRecordsResponse response)
    {
        if (!Leaderboards.ContainsKey(mapUid))
        {
            Leaderboards[mapUid] = new();
        }

        Leaderboards[mapUid][car] = response;
        UpdateLeaderboards();
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

        IsTMUF = isTMUF;

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
            var frameScrollRecords = (frameLeaderboard.GetFirstChild("FrameScrollRecords") as CMlFrame)!;
            var labelRank = (FrameRanks.Controls[i] as CMlLabel)!;

            frameScrollRecords.ScrollOffset.Y = 0;

            if (missingCar)
            {
                labelValidation.SetText("$888...send us suggestions!");
                labelValidation.Show();
                frameLeaderboard.Hide();
                labelRank.Hide();
            }
            else
            {
                labelValidation.Hide();
                frameLeaderboard.Show();
                labelRank.Show();
            }

            labelCar.Show();
        }

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        Campaign = GetCampaignForMaps();

        if (MapGroupNum != -1 && MapInfoNum != -1)
        {
            UpdateGeneralStats(Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum]);
        }

        if (MapSelectedAt != -1)
        {
            LoadLeaderboards(true);
        }
    }

    private void SetupCampaign(bool optimizedMode, bool focusOnSelected)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        QuadOfficialCampaign.StyleSelected = SelectedCampaignQuad == QuadOfficialCampaign;
        QuadVRCampaign.StyleSelected = SelectedCampaignQuad == QuadVRCampaign;

        Campaign = GetCampaignForMaps();

        var selectedNum = -1;
        var totalMapCounter = 0;

        if (!optimizedMode)
        {
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
                        var labelLaps = (frameMap.GetFirstChild("LabelLaps") as CMlLabel)!;
                        var labelCompleted = (frameMap.GetFirstChild("LabelCompleted") as CMlLabel)!;
                        var quadMedal = (frameMap.GetFirstChild("QuadMedal") as CMlQuad)!;

                        if (mapGroup is null || mapCounter >= mapGroup.MapInfos.Count)
                        {
                            quadMapThumbnail.Hide();
                            quadMapButton.Hide();
                            quadMapName.Hide();
                            labelMapName.Hide();
                            labelSkillpoints.Hide();
                            labelLaps.Hide();
                            labelCompleted.Hide();
                            quadMedal.Hide();
                            continue;
                        }

                        var visualMapNumber = totalMapCounter;

                        if (SelectedCampaignQuad == QuadVRCampaign)
                        {
                            visualMapNumber = visualMapNumber % 10;
                        }

                        var mapInfo = mapGroup.MapInfos[mapCounter];

                        quadMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                        quadMapThumbnail.Show();

                        labelMapName.SetText(TextLib.FormatInteger(visualMapNumber + 1, 3));
                        labelMapName.Show();

                        quadMapButton.DataAttributeSet("MapGroupNum", i.ToString());
                        quadMapButton.DataAttributeSet("MapInfoNum", mapCounter.ToString());
                        quadMapButton.Show();

                        var hovered = MapGroupNum == i && MapInfoNum == mapCounter;
                        quadMapButton.StyleSelected = (MapSelectedAt != -1 || focusOnSelected) && hovered;

                        if (quadMapButton.StyleSelected && focusOnSelected)
                        {
                            quadMapButton.Focus();
                        }

                        if (hovered)
                        {
                            selectedNum = visualMapNumber;
                        }

                        quadMapName.Show();
                        labelSkillpoints.Hide();

                        labelLaps.Visible = mapInfo.TMObjective_IsLapRace && mapInfo.TMObjective_NbLaps > 1;

                        // completion isnt considered when data is missing
                        var completed = TitleCombinations.Count > 0;

                        foreach (var carName in AllCars)
                        {
                            var scoreContext = $"{ScoreContextPrefix}{carName}";

                            // hacky but it works for TMT
                            var isDefaultCar = (mapInfo.CollectionName == "Canyon" && carName == "CanyonCar")
                                || (mapInfo.CollectionName == "Stadium" && carName == "StadiumCar")
                                || (mapInfo.CollectionName == "Valley" && carName == "ValleyCar")
                                || (mapInfo.CollectionName == "Lagoon" && carName == "LagoonCar");

                            if (isDefaultCar)
                            {
                                scoreContext = ScoreContextPrefix;
                                continue; // TODO configurable if default cars should count or not
                            }
                            else if (!TitleCombinations.ContainsKey(mapInfo.MapUid) || !TitleCombinations[mapInfo.MapUid].ContainsKey($"{carName}_0"))
                            {
                                // if envimix car doesnt have a validation, skip
                                continue;
                            }

                            var playerTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);

                            if (playerTime == -1)
                            {
                                completed = false;
                                break;
                            }
                        }

                        labelCompleted.Visible = completed;

                        var defaultCarTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, ScoreContextPrefix);

                        if (defaultCarTime < 0)
                        {
                            quadMedal.Hide();
                        }
                        else
                        {
                            quadMedal.ImageUrl = "";
                            quadMedal.Substyle = "";

                            if (SuperMedals.ContainsKey(mapInfo.MapUid))
                            {
                                var superMedals = SuperMedals[mapInfo.MapUid];
                                if (defaultCarTime <= superMedals.Duck)
                                {
                                    quadMedal.ImageUrl = "file://Media/Images/Medals/duck.png";
                                }
                                else if (defaultCarTime <= superMedals.STM)
                                {
                                    quadMedal.ImageUrl = "file://Media/Images/Medals/stm.png";
                                }
                                else
                                {
                                    var superGold = superMedals.STM - MathLib.FloorInteger((superMedals.STM - mapInfo.TMObjective_AuthorTime) * 0.125f);
                                    var superSilver = superMedals.STM - MathLib.FloorInteger((superMedals.STM - mapInfo.TMObjective_AuthorTime) * 0.25f);
                                    var superBronze = superMedals.STM - MathLib.FloorInteger((superMedals.STM - mapInfo.TMObjective_AuthorTime) * 0.5f);

                                    if (defaultCarTime <= superGold)
                                    {
                                        quadMedal.ImageUrl = "file://Media/Images/Medals/supergold.png";
                                    }
                                    else if (defaultCarTime <= superSilver)
                                    {
                                        quadMedal.ImageUrl = "file://Media/Images/Medals/supersilver.png";
                                    }
                                    else if (defaultCarTime <= superBronze)
                                    {
                                        quadMedal.ImageUrl = "file://Media/Images/Medals/superbronze.png";
                                    }
                                }
                            }

                            if (quadMedal.ImageUrl == "")
                            {
                                if (defaultCarTime <= mapInfo.TMObjective_AuthorTime)
                                {
                                    quadMedal.Substyle = "MedalNadeo";
                                }
                                else if (defaultCarTime <= mapInfo.TMObjective_GoldTime)
                                {
                                    quadMedal.Substyle = "MedalGold";
                                }
                                else if (defaultCarTime <= mapInfo.TMObjective_SilverTime)
                                {
                                    quadMedal.Substyle = "MedalSilver";
                                }
                                else if (defaultCarTime <= mapInfo.TMObjective_BronzeTime)
                                {
                                    quadMedal.Substyle = "MedalBronze";
                                }
                                else
                                {
                                    quadMedal.Substyle = "MedalSlot";
                                }
                            }

                            quadMedal.Show();
                        }

                        mapCounter += 1;
                        totalMapCounter += 1;
                    }
                }
            }
        }

        if (MapGroupNum != -1 && MapInfoNum != -1)
        {
            var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];
            if (optimizedMode)
            {
                var visualMapNumber = 0;
                for (int i = 0; i < MapGroupNum; i++)
                {
                    visualMapNumber += Campaign.MapGroups[i].MapInfos.Count;
                }
                visualMapNumber += MapInfoNum;
                if (SelectedCampaignQuad == QuadVRCampaign)
                {
                    visualMapNumber = visualMapNumber % 10;
                }
                selectedNum = visualMapNumber;
            }

            LabelSelectedMapName.SetText(TextLib.FormatInteger(selectedNum + 1, 3));
            QuadPlay.Visible = true;

            UpdateGeneralStats(selectedMapInfo);
        }
        else
        {
            LabelSelectedMapName.SetText("...");
            QuadPlay.Visible = false;

            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
        }
    }

    private void UnloadLeaderboards()
    {
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
            var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;
            var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

            quadLoadingLeaderboard.Hide();
            labelConfirm.Show();

            foreach (var controlRec in frameRecords.Controls)
            {
                controlRec.Hide();
            }
        }

        LeaderboardsLoadedOrLoading = false;
    }

    private void PlaySelectedMap()
    {
        SendCustomEvent("PlayMap", new[] { MapGroupNum.ToString(), MapInfoNum.ToString() });
    }

    private void ExploreSelectedMap()
    {
        SendCustomEvent("ExploreMap", new[] { MapGroupNum.ToString(), MapInfoNum.ToString() });
    }

    private void ViewGhostSelectedMap(string car, string ghostUrl)
    {
        SendCustomEvent("ViewGhost", new[] { MapGroupNum.ToString(), MapInfoNum.ToString(), car, ghostUrl });
    }

    private void MapClick(CMlControl control)
    {
        var mapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        var mapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        if (MapSelectedAt != -1 && MapGroupNum == mapGroupNum && MapInfoNum == mapInfoNum)
        {
            if ((Now - MapSelectedAt) < 500)
            {
                PlaySelectedMap();
            }
            else
            {
                MapSelectedAt = -1;
                SetupCampaign(false, false);
                UnloadLeaderboards();
            }
            return;
        }

        MapGroupNum = mapGroupNum;
        MapInfoNum = mapInfoNum;
        MapSelectedAt = Now;

        SetupCampaign(false, false);
        LoadLeaderboards(true);
    }

    private void MapSelect(CMlControl control)
    {
        if (MapSelectedAt != -1)
        {
            return;
        }

        MapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        MapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        SetupCampaign(true, false);
    }

    private void AudioPlayClick()
    {
        AudioClick.Stop();
        AudioClick.Play();
    }

    private void SelectOfficialCampaign()
    {
        if (SelectedCampaignQuad != QuadOfficialCampaign)
        {
            MapGroupNum = -1;
            MapInfoNum = -1;
            MapSelectedAt = -1;

            AudioPlayClick();
            SelectedCampaignQuad = QuadOfficialCampaign;
            SetupCampaign(false, false);
            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
            ResetRanks();
            UnloadLeaderboards();

            var selectedCampaign = Local<string>.For(Page);
            selectedCampaign.Set("");
        }
    }

    private void SelectVRCampaign()
    {
        if (VRCampaignReleased && SelectedCampaignQuad != QuadVRCampaign)
        {
            MapGroupNum = -1;
            MapInfoNum = -1;
            MapSelectedAt = -1;

            AudioPlayClick();
            SelectedCampaignQuad = QuadVRCampaign;
            SetupCampaign(false, false);
            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
            ResetRanks();
            UnloadLeaderboards();

            var selectedCampaign = Local<string>.For(Page);
            selectedCampaign.Set("VR");
        }
    }
}
