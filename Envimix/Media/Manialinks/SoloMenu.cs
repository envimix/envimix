using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class SoloMenu : CManiaAppTitleLayer, IContext
{
    public const int OpreaxCampaignNum = 36;

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

    public struct SWorldMapMedals
    {
        public int Author;
        public int Gold;
        public int Silver;
        public int Bronze;
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
    [ManialinkControl] public required CMlFrame FrameCampaignMaps;
    [ManialinkControl] public required CMlFrame FrameCampaignGrid;
    [ManialinkControl] public required CMlFrame FrameWorld;
    [ManialinkControl] public required CMlFrame FrameWorldDiamonds;
    [ManialinkControl] public required CMlLabel LabelWorldUnlockProgress;
    [ManialinkControl] public required CMlGauge GaugeWorldNextUnlock;
    [ManialinkControl] public required CMlLabel LabelWorldNextUnlockProgress;
    [ManialinkControl] public required CMlGauge GaugeWorldCampaignProgress;
    [ManialinkControl] public required CMlLabel LabelWorldCampaignProgress;
    [ManialinkControl] public required CMlFrame FrameWorldMapPanel;
    [ManialinkControl] public required CMlFrame FrameWorldMapPanelClip;
    [ManialinkControl] public required CMlQuad QuadWorldMapPanelBackground;
    [ManialinkControl] public required CMlQuad QuadWorldMapPanelBackgroundOverlay;
    [ManialinkControl] public required CMlLabel LabelWorldMapName;
    [ManialinkControl] public required CMlLabel LabelWorldMapLocation;
    [ManialinkControl] public required CMlQuad QuadWorldMapThumbnail;
    [ManialinkControl] public required CMlLabel LabelWorldMapEnvironment;
    [ManialinkControl] public required CMlLabel LabelWorldMapVehicle;
    [ManialinkControl] public required CMlLabel LabelWorldMapStyle;
    [ManialinkControl] public required CMlLabel LabelWorldMapTrackmaster;
    [ManialinkControl] public required CMlLabel LabelWorldMapSecretPlace;
    [ManialinkControl] public required CMlQuad QuadWorldMapMedalAuthor;
    [ManialinkControl] public required CMlQuad QuadWorldMapMedalGold;
    [ManialinkControl] public required CMlQuad QuadWorldMapMedalSilver;
    [ManialinkControl] public required CMlQuad QuadWorldMapMedalBronze;
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
    [ManialinkControl] public required CMlQuad QuadVROffzoneCampaign;
    [ManialinkControl] public required CMlQuad QuadVROffzoneDisabled;
    [ManialinkControl] public required CMlQuad QuadOpreaxCampaign;
    [ManialinkControl] public required CMlQuad QuadOpreaxCampaignGlow;
    [ManialinkControl] public required CMlLabel LabelOpreaxCampaign;
    [ManialinkControl] public required CMlLabel LabelOpreaxCampaignNew;
    [ManialinkControl] public required CMlLabel LabelVROffzoneCountdown;

    [ManialinkControl] public required CMlFrame FrameTooltip;

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
    [Local(LocalFor.LocalUser)] public bool CampaignsReleaseChecked { get; set; }
    [Local(LocalFor.LocalUser)] public bool EnvimixTurboUserIsAdmin { get; set; }
    [Local(LocalFor.LocalUser)] public bool EnvimixTurboUserCanAccessUnreleased { get; set; }

    public Dictionary<string, Dictionary<string, SEnvimaniaRecordsResponse>> Leaderboards;
    public Dictionary<string, int> LeaderboardRequestTimestamps;
    public Dictionary<string, Dictionary<string, int>> LeaderboardErrors;

    public Dictionary<string, Dictionary<string, string>> TitleStars;
    public Dictionary<string, Dictionary<string, SCombinationStat>> TitleCombinations;
    public Dictionary<string, STitleUserInfo> SoloUserInfos;

    public CAudioSource AudioClick;

    public CMlQuad SelectedCampaignQuad;

    public CMlFrame? HoveredWorldDiamond;
    public CMlFrame? SelectedWorldDiamond;
    public CMlFrame? WorldMapPanelDiamond;
    public string HoveredWorldMapUid = "";
    public string SelectedWorldMapUid = "";
    public string DisplayedWorldMapUid = "";
    public int WorldMapCampaignNum = -1;
    public string WorldMapMedalsMapUid = "";
    public int WorldMapMedalsTime = -1;
    public int WorldDiamondSelectedAt = -1;
    public float WorldMapPanelHeight;
    public float WorldMapPanelFromHeight;
    public float WorldMapPanelTargetHeight;
    public int WorldMapPanelResizeAt = -1;
    public float WorldMapPanelWidth;
    public int WorldMapPanelOpenAt = -1;
    public bool WorldMapPanelFlipped;

    public Dictionary<string, int> WorldMapUnlocks;
    public Dictionary<string, SWorldMapMedals> WorldMapMedals;
    public int WorldMapMedalsCampaignCount = -1;
    public int WorldMedalPoints;
    public bool WorldUnlocksInitialized;

    public CMlQuad? FocusedControl;
    public CMlFrame? SelectedRecordFrame;
    public string SelectedRecordCar = "";
    public string SelectedRecordGhostUrl = "";
    public CMlFrame? SelectedPersonalBestFrame;
    public string SelectedPersonalBestCar = "";
    public string SelectedPersonalBestTime = "";

    public bool VRCampaignReleased;
    public bool VROffzoneCampaignReleased;
    public bool OpreaxCampaignReleased;
    public bool VROffzoneEnabled = true;
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
            Quickplay();
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

        QuadVROffzoneCampaign.MouseClick += () =>
        {
            ToggleVROffzoneCampaign();
        };

        QuadVROffzoneCampaign.MouseOver += () =>
        {
            if (VROffzoneEnabled)
            {
                UpdateTooltip("Disable offzone mode");
            }
            else
            {
                UpdateTooltip("Enable offzone mode");
            }
        };

        QuadOpreaxCampaign.MouseClick += () =>
        {
            SelectOpreaxCampaign();
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    // delayed so the reveal happens after the menu slides in
                    UpdateWorldUnlocks(700);
                    if (FrameWorld.Visible)
                    {
                        UpdateWorldMapPanel();
                    }
                    ShowCampaignOverviewFrame();
                    ShowMapOverviewFrame();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    ClearWorldDiamondHover(false);
                    FrameWorldMapPanel.Hide();
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
                    UpdateWorldUnlocks(0);
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
                case "LeaderboardError":
                    var errorMapUid = data[0];
                    var errorCar = data[1];
                    var errorCode = data[2];
                    ProcessLeaderboardError(errorMapUid, errorCar, TextLib.ToInteger(errorCode));
                    break;
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (control.HasClass("WorldMapBackground") && FrameWorld.Visible && EnableMenuNavigationInputs)
            {
                DeselectWorldDiamond();
                ClearWorldDiamondHover(false);
                WorldMapPanelDiamond = null;
                FrameWorldMapPanel.Hide();
                SetWorldMapPanelExpanded(false);
            }
            if (control.HasClass("WorldDiamond") && FrameWorld.Visible && EnableMenuNavigationInputs)
            {
                SelectWorldDiamond((control.Parent as CMlFrame)!);
            }
            if (controlId == "QuadMapButton")
            {
                MapClick(control);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            }
            if (control.Parent.Parent.ControlId == "FrameRecords")
            {
                var car = control.DataAttributeGet("Car");
                var ghostUrl = control.DataAttributeGet("GhostUrl");
                if (ghostUrl != "")
                {
                    var recordFrame = (control.Parent as CMlFrame)!;
                    var recordQuad = (recordFrame.Controls[1] as CMlQuad)!;

                    if (SelectedRecordFrame == recordFrame)
                    {
                        recordQuad.Hide();
                        SelectedRecordFrame = null;
                        SelectedRecordCar = "";
                        SelectedRecordGhostUrl = "";
                        ViewGhostSelectedMap(car, ghostUrl);
                    }
                    else
                    {
                        if (SelectedRecordFrame is not null)
                        {
                            var selectedRecordQuad = (SelectedRecordFrame.Controls[1] as CMlQuad)!;
                            selectedRecordQuad.Hide();
                        }

                        recordQuad.Show();
                        SelectedRecordFrame = recordFrame;
                        SelectedRecordCar = car;
                        SelectedRecordGhostUrl = ghostUrl;
                    }
                }
            }
            if (controlId == "LabelPersonalBest")
            {
                var car = control.DataAttributeGet("Car");
                var time = control.DataAttributeGet("Time");
                if (time != "-1")
                {
                    var personalBestFrame = (control.Parent as CMlFrame)!;
                    var personalBestLabel = (control as CMlLabel)!;

                    if (SelectedPersonalBestFrame == personalBestFrame)
                    {
                        personalBestLabel.TextColor = new Vec3(1, 1, 1);
                        SelectedPersonalBestFrame = null;
                        SelectedPersonalBestCar = "";
                        SelectedPersonalBestTime = "";
                        ViewGhostSelectedMap(car, "");
                    }
                    else
                    {
                        if (SelectedPersonalBestFrame is not null)
                        {
                            var selectedPersonalBestLabel = (SelectedPersonalBestFrame.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
                            selectedPersonalBestLabel.TextColor = new Vec3(1, 1, 1);
                        }

                        personalBestLabel.TextColor = new Vec3(0.5f, 0.8f, 1);
                        SelectedPersonalBestFrame = personalBestFrame;
                        SelectedPersonalBestCar = car;
                        SelectedPersonalBestTime = time;
                    }
                }
            }
            if (FocusedControl is not null)
            {
                FocusedControl.StyleSelected = IsCampaignSelected(FocusedControl);
                FocusedControl = null;
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (control.HasClass("WorldDiamond") && FrameWorld.Visible && EnableMenuNavigationInputs)
            {
                HoverWorldDiamond((control.Parent as CMlFrame)!);
            }
            if (controlId == "QuadMapButton")
            {
                MapSelect(control);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
            if (control.Parent.ControlId == "FrameStars")
            {
                AnimMgr.Add(control, "<quad scale=\"1.2\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                var nickname = control.DataAttributeGet("Login");
                if (SoloUserInfos.ContainsKey(nickname))
                {
                    nickname = SoloUserInfos[nickname].N;
                }
                UpdateTooltip(nickname);
            }
        };

        MouseOut += (control, controlId) =>
        {
            if (control.HasClass("WorldDiamond") && control.Parent == HoveredWorldDiamond)
            {
                ClearWorldDiamondHover(true);
            }
            if (control.Parent.ControlId == "FrameStars")
            {
                AnimMgr.Add(control, "<quad scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            FrameTooltip.Hide();
        };

        MenuNavigation += (action) =>
        {
            if (SelectedCampaignQuad == QuadOpreaxCampaign && FocusedControl is null)
            {
                FocusedControl = QuadOpreaxCampaign;
            }

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
                    else if (SelectedCampaignQuad != QuadOpreaxCampaign)
                    {
                        SetupCampaign(false, false);
                        UnloadLeaderboards();
                        FocusedControl.StyleSelected = IsCampaignSelected(FocusedControl);
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
                        FocusedControl.StyleSelected = IsCampaignSelected(FocusedControl);
                    }
                    if (FocusedControl == QuadQuickplay)
                    {
                        FocusedControl = QuadOpreaxCampaign;
                    }
                    else if (FocusedControl == QuadOfficialCampaign)
                    {
                        FocusedControl = QuadQuickplay;
                    }
                    else if (FocusedControl == QuadVRCampaign)
                    {
                        FocusedControl = QuadOfficialCampaign;
                    }
                    else if (FocusedControl == QuadVROffzoneCampaign)
                    {
                        FocusedControl = QuadVRCampaign;
                    }
                    else if (FocusedControl == QuadOpreaxCampaign)
                    {
                        FocusedControl = QuadVROffzoneCampaign;
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
                        FocusedControl.StyleSelected = IsCampaignSelected(FocusedControl);
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
                        FocusedControl = QuadVROffzoneCampaign;
                    }
                    else if (FocusedControl == QuadVROffzoneCampaign)
                    {
                        FocusedControl = QuadOpreaxCampaign;
                    }
                    else if (FocusedControl == QuadOpreaxCampaign)
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
                            Quickplay();
                        }
                        else if (FocusedControl == QuadOfficialCampaign)
                        {
                            SelectOfficialCampaign();
                        }
                        else if (FocusedControl == QuadVRCampaign)
                        {
                            SelectVRCampaign();
                        }
                        else if (FocusedControl == QuadVROffzoneCampaign)
                        {
                            ToggleVROffzoneCampaign();
                        }
                        else if (FocusedControl == QuadOpreaxCampaign)
                        {
                            SelectOpreaxCampaign();
                        }
                    }
                    break;
            }
        };
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
        FrameCampaignGrid.RelativePosition_V3.Y = -50;
        FrameWorldDiamonds.RelativePosition_V3.Y = -50;
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 80\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameCampaignGrid, "<frame pos=\"0 80\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameWorldDiamonds, "<frame pos=\"0 80\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideCampaignOverviewFrame()
    {
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 -50\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameCampaignGrid, "<frame pos=\"0 -50\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameWorldDiamonds, "<frame pos=\"0 -50\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
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
        if (SelectedPersonalBestFrame is not null)
        {
            var selectedPersonalBestLabel = (SelectedPersonalBestFrame.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            selectedPersonalBestLabel.TextColor = new Vec3(1, 1, 1);
        }

        SelectedPersonalBestFrame = null;
        SelectedPersonalBestCar = "";
        SelectedPersonalBestTime = "";

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
            if (TitleCombinations.Count > 0)
            {
                gaugeDifficulty.Color = new Vec3(1, 0, 0);
            }
            else
            {
                gaugeDifficulty.Color = new Vec3(1, 1, 1);
            }
        }
        foreach (var control in FrameQualityRatings.Controls)
        {
            if (control is not CMlGauge gaugeQuality)
            {
                continue;
            }
            gaugeQuality.Ratio = 0;
            if (TitleCombinations.Count > 0)
            {
                gaugeQuality.Color = new Vec3(1, 0, 0);
            }
            else
            {
                gaugeQuality.Color = new Vec3(1, 1, 1);
            }
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

    private string GetDefaultCar(CMapInfo mapInfo)
    {
        var mapDefaultCars = Local<Dictionary<string, string>>.For(LocalUser);
        if (mapDefaultCars.Get().ContainsKey(mapInfo.MapUid))
        {
            return mapDefaultCars.Get()[mapInfo.MapUid];
        }

        // Maps without explicit metadata retain the legacy collection default.
        return $"{mapInfo.CollectionName}Car";
    }

    private bool IsDefaultCar(CMapInfo mapInfo, string car)
    {
        return car == GetDefaultCar(mapInfo);
    }

    private string GetScoreContext(CMapInfo mapInfo, string car, string prefix)
    {
        if (IsDefaultCar(mapInfo, car))
        {
            return prefix;
        }
        return $"{prefix}{car}";
    }

    private void UpdatePBs(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        if (SelectedPersonalBestFrame is not null)
        {
            var selectedPersonalBestLabel = (SelectedPersonalBestFrame.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            selectedPersonalBestLabel.TextColor = new Vec3(1, 1, 1);
            SelectedPersonalBestFrame = null;
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

            var scoreContext = GetScoreContext(mapInfo, carName, ScoreContextPrefix);

            var labelPersonalBest = (frameLeaderboard.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            labelPersonalBest.DataAttributeSet("Car", carName);
            labelPersonalBest.TextColor = new Vec3(1, 1, 1);

            var personalBestFrame = (frameLeaderboard.GetFirstChild("FramePersonalBest") as CMlFrame)!;

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

            if (SelectedPersonalBestCar == carName && SelectedPersonalBestTime == time.ToString())
            {
                labelPersonalBest.TextColor = new Vec3(0.5f, 0.8f, 1);
                SelectedPersonalBestFrame = personalBestFrame;
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

            if (IsDefaultCar(mapInfo, carName))
            {
                labelValidation.SetText($"validated by {mapInfo.AuthorNickName}");
                carIndex += 1;
                continue;
            }

            var combinationKey = $"{carName}_0";

            if (!TitleCombinations.ContainsKey(mapInfo.MapUid) || !TitleCombinations[mapInfo.MapUid].ContainsKey(combinationKey))
            {
                if (TitleCombinations.Count == 0)
                {
                    labelValidation.SetText("$888unknown validator");
                }
                else
                {
                    labelValidation.SetText("$888you can validate this!");
                }
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

            var scoreContext = GetScoreContext(mapInfo, carName, ScoreContextPrefix);

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

            var isDefaultCar = IsDefaultCar(mapInfo, car);

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

                if (isDefaultCar || TitleCombinations.Count == 0)
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
                var login = TitleStars[mapInfo.MapUid][filterKey];
                controlStar.DataAttributeSet("Login", login);
                controlStar.Show();
            }
            else
            {
                controlStar.DataAttributeSet("Login", "");
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

        if (SelectedCampaignQuad == QuadVROffzoneCampaign)
        {
            return DataFileMgr.Campaigns[24];
        }

        if (SelectedCampaignQuad == QuadOpreaxCampaign && WorldMapCampaignNum != -1)
        {
            return DataFileMgr.Campaigns[WorldMapCampaignNum];
        }

        return DataFileMgr.Campaigns[0];
    }

    private void UpdateLeaderboards(bool resetScroll)
    {
        if (DataFileMgr.Campaigns.Count == 0 || MapGroupNum == -1 || MapInfoNum == -1 || MapSelectedAt == -1)
        {
            return;
        }

        if (SelectedRecordFrame is not null)
        {
            var selectedRecordQuad = (SelectedRecordFrame.Controls[1] as CMlQuad)!;
            selectedRecordQuad.Hide();
            SelectedRecordFrame = null;
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
            var labelStatus = (frameLeaderboard.GetFirstChild("LabelStatus") as CMlLabel)!;
            labelStatus.SetText("...confirm map to load...");

            if (resetScroll)
            {
                frameScrollRecords.ScrollOffset.Y = 0;
            }

            var scrollIndex = MathLib.NearestInteger((float)frameScrollRecords.ScrollOffset.Y / 5);

            labelStatus.Hide();

            carIndex += 1;

            if (!Leaderboards.ContainsKey(selectedMapInfo.MapUid) || !Leaderboards[selectedMapInfo.MapUid].ContainsKey(carName))
            {
                // check for any error
                if (LeaderboardErrors.ContainsKey(selectedMapInfo.MapUid) && LeaderboardErrors[selectedMapInfo.MapUid].ContainsKey(carName))
                {
                    var errorCode = LeaderboardErrors[selectedMapInfo.MapUid][carName];

                    if (errorCode == 10006)
                    {
                        labelStatus.Value = "offline mode";
                    }
                    else
                    {
                        labelStatus.Value = $"error {errorCode}";
                    }
                }
                else
                {
                    labelStatus.Value = "unknown error";
                }

                labelStatus.Show();
                quadLoadingLeaderboard.Hide();
                continue;
            }

            var lb = Leaderboards[selectedMapInfo.MapUid][carName];

            frameScrollRecords.ScrollMax.Y = MathLib.Max(0f, (lb.Records.Length - 10) * 5f);

            quadLoadingLeaderboard.Hide();

            if (lb.Records.Length == 0)
            {
                var firstRecordFrame = (frameRecords.Controls[0] as CMlFrame)!;
                var labelYouCouldBeHere = (firstRecordFrame.Controls[0] as CMlLabel)!;
                labelYouCouldBeHere.SetText("01 -:--.---  $i$888you could be here!");
                labelYouCouldBeHere.DataAttributeSet("Car", carName);
                labelYouCouldBeHere.DataAttributeSet("GhostUrl", "");
                firstRecordFrame.Show();
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

                var recordFrame = (controlRec as CMlFrame)!;
                var labelRec = (recordFrame.Controls[0] as CMlLabel)!;
                labelRec.SetText($"{TextLib.FormatInteger(rankIndex + 1 - rankOffset, 2)} {TimeToTextWithMilli(record.Time)}  {record.User.Nickname}");
                labelRec.DataAttributeSet("Car", carName);
                labelRec.DataAttributeSet("GhostUrl", record.GhostUrl);
                recordFrame.Show();
                labelRec.Show();

                if (SelectedRecordCar == carName && SelectedRecordGhostUrl == record.GhostUrl)
                {
                    var recordQuad = (recordFrame.Controls[1] as CMlQuad)!;
                    recordQuad.Show();
                    SelectedRecordFrame = recordFrame;
                }

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

        if (SelectedCampaignQuad == QuadOpreaxCampaign && MapSelectedAt == -1)
        {
            return;
        }

        if (showLoader)
        {
            if (SelectedRecordFrame is not null)
            {
                var selectedRecordQuad = (SelectedRecordFrame.Controls[1] as CMlQuad)!;
                selectedRecordQuad.Hide();
            }

            SelectedRecordFrame = null;
            SelectedRecordCar = "";
            SelectedRecordGhostUrl = "";

            if (SelectedPersonalBestFrame is not null)
            {
                var selectedPersonalBestLabel = (SelectedPersonalBestFrame.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
                selectedPersonalBestLabel.TextColor = new Vec3(1, 1, 1);
            }

            SelectedPersonalBestFrame = null;
            SelectedPersonalBestCar = "";
            SelectedPersonalBestTime = "";
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
            UpdateLeaderboards(true);
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
                var labelStatus = (frameLeaderboard.GetFirstChild("LabelStatus") as CMlLabel)!;
                var frameScrollRecords = (frameLeaderboard.GetFirstChild("FrameScrollRecords") as CMlFrame)!;
                var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

                frameScrollRecords.ScrollOffset.Y = 0;

                quadLoadingLeaderboard.Show();
                labelStatus.Hide();

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
        UpdateLeaderboards(false);
    }

    private void ProcessLeaderboardError(string mapUid, string car, int code)
    {
        if (!LeaderboardErrors.ContainsKey(mapUid))
        {
            LeaderboardErrors[mapUid] = new();
        }

        LeaderboardErrors[mapUid][car] = code;
        UpdateLeaderboards(false);
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

    private bool IsCampaignSelected(CMlQuad quad)
    {
        return SelectedCampaignQuad == quad || (quad == QuadVRCampaign && SelectedCampaignQuad == QuadVROffzoneCampaign);
    }

    private void UnloadLeaderboards()
    {
        if (SelectedRecordFrame is not null)
        {
            var selectedRecordQuad = (SelectedRecordFrame.Controls[1] as CMlQuad)!;
            selectedRecordQuad.Hide();
        }

        SelectedRecordFrame = null;
        SelectedRecordCar = "";
        SelectedRecordGhostUrl = "";

        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
            var labelStatus = (frameLeaderboard.GetFirstChild("LabelStatus") as CMlLabel)!;
            var frameScrollRecords = (frameLeaderboard.GetFirstChild("FrameScrollRecords") as CMlFrame)!;
            var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

            frameScrollRecords.ScrollOffset.Y = 0;

            quadLoadingLeaderboard.Hide();
            labelStatus.Show();

            foreach (var controlRec in frameRecords.Controls)
            {
                controlRec.Hide();
            }
        }

        LeaderboardsLoadedOrLoading = false;
    }

    private void ResolveWorldMap(string mapUid)
    {
        if (DataFileMgr.Campaigns.Count <= OpreaxCampaignNum)
        {
            return;
        }

        var campaign = DataFileMgr.Campaigns[OpreaxCampaignNum];
        for (var groupNum = 0; groupNum < campaign.MapGroups.Count; groupNum++)
        {
            var mapGroup = campaign.MapGroups[groupNum];
            for (var infoNum = 0; infoNum < mapGroup.MapInfos.Count; infoNum++)
            {
                if (mapGroup.MapInfos[infoNum].MapUid == mapUid)
                {
                    WorldMapCampaignNum = OpreaxCampaignNum;
                    MapGroupNum = groupNum;
                    MapInfoNum = infoNum;
                    return;
                }
            }
        }
    }

    private void SetWorldMapPanelExpanded(bool expanded)
    {
        var targetHeight = 12f;
        if (expanded)
        {
            targetHeight = 40f;
        }

        if (WorldMapPanelTargetHeight == targetHeight)
        {
            return;
        }

        WorldMapPanelFromHeight = WorldMapPanelHeight;
        WorldMapPanelTargetHeight = targetHeight;
        WorldMapPanelResizeAt = Now;
    }

    private void ApplyWorldMapPanelWidth()
    {
        var left = 0f;
        if (WorldMapPanelFlipped)
        {
            left = 65 - WorldMapPanelWidth;
        }

        QuadWorldMapPanelBackground.Size.X = WorldMapPanelWidth;
        QuadWorldMapPanelBackgroundOverlay.Size.X = WorldMapPanelWidth;
        QuadWorldMapPanelBackground.RelativePosition_V3.X = left;
        QuadWorldMapPanelBackgroundOverlay.RelativePosition_V3.X = left;
        FrameWorldMapPanelClip.ClipWindowSize.X = WorldMapPanelWidth;
        FrameWorldMapPanelClip.ClipWindowRelativePosition.X = left + WorldMapPanelWidth * 0.5f;
    }

    private void OpenWorldMapPanel()
    {
        WorldMapPanelWidth = 0;
        WorldMapPanelOpenAt = Now;
        ApplyWorldMapPanelWidth();
    }

    private void UpdateWorldMapPanelAnimation()
    {
        if (WorldMapPanelOpenAt != -1)
        {
            var openingElapsed = MathLib.Clamp(Now - WorldMapPanelOpenAt, 0, 220);
            var width = AnimLib.EaseOutQuad(openingElapsed, 0, 65, 220);
            WorldMapPanelWidth = width;
            ApplyWorldMapPanelWidth();

            if (openingElapsed >= 220)
            {
                WorldMapPanelOpenAt = -1;
            }
        }

        if (WorldMapPanelResizeAt == -1)
        {
            return;
        }

        var elapsed = MathLib.Clamp(Now - WorldMapPanelResizeAt, 0, 220);
        var height = AnimLib.EaseOutQuad(elapsed, WorldMapPanelFromHeight, WorldMapPanelTargetHeight - WorldMapPanelFromHeight, 220);
        WorldMapPanelHeight = height;
        QuadWorldMapPanelBackground.Size.Y = height;
        QuadWorldMapPanelBackgroundOverlay.Size.Y = height;
        FrameWorldMapPanelClip.ClipWindowSize.Y = height;
        FrameWorldMapPanelClip.ClipWindowRelativePosition.Y = 4 - height * 0.5f;

        if (elapsed >= 220)
        {
            WorldMapPanelResizeAt = -1;
        }
    }

    private void UpdateWorldMapPanelPosition()
    {
        if (WorldMapPanelDiamond is null)
        {
            return;
        }

        var diamondPosition = WorldMapPanelDiamond.AbsolutePosition_V3;
        var worldPosition = FrameWorld.AbsolutePosition_V3;
        var worldScale = (float)FrameWorld.AbsoluteScale;
        var panelScale = (float)FrameWorldMapPanel.AbsoluteScale;
        var panelWidth = 65 * panelScale;
        var padding = worldScale;
        var gap = 5 * worldScale;
        var left = (float)worldPosition.X - 153.75f * worldScale + padding;
        var right = (float)worldPosition.X + 153.75f * worldScale - padding;
        var top = (float)worldPosition.Y - worldScale - padding;
        var bottom = (float)worldPosition.Y - 118.5f * worldScale + padding;

        var x = (float)diamondPosition.X + gap;
        var flipped = x + panelWidth > right;
        if (WorldMapPanelFlipped != flipped)
        {
            WorldMapPanelFlipped = flipped;
            ApplyWorldMapPanelWidth();
        }
        if (flipped)
        {
            x = (float)diamondPosition.X - gap - panelWidth;
        }
        x = MathLib.Clamp(x, left, right - panelWidth);
        var y = MathLib.Clamp((float)diamondPosition.Y + 1.7f * worldScale, bottom + (WorldMapPanelHeight - 4) * panelScale, top - 4 * panelScale);
        FrameWorldMapPanel.RelativePosition_V3 = new Vec2(x, y);
    }

    private void UpdateWorldMapMedals(CMapInfo mapInfo)
    {
        // default car medals stack up like a pile, unearned ones are hidden
        var time = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, ScoreContextPrefix);
        WorldMapMedalsMapUid = mapInfo.MapUid;
        WorldMapMedalsTime = time;
        var hasTime = time >= 0;
        var hasBronze = hasTime && time <= mapInfo.TMObjective_BronzeTime;

        QuadWorldMapMedalAuthor.Visible = hasTime && time <= mapInfo.TMObjective_AuthorTime;
        QuadWorldMapMedalGold.Visible = hasTime && time <= mapInfo.TMObjective_GoldTime;
        QuadWorldMapMedalSilver.Visible = hasTime && time <= mapInfo.TMObjective_SilverTime;

        // bronze quad doubles as the empty slot when no medal is earned yet
        QuadWorldMapMedalBronze.Show();
        if (hasBronze)
        {
            QuadWorldMapMedalBronze.Substyle = "MedalBronze";
            QuadWorldMapMedalBronze.Opacity = 1;
        }
        else
        {
            QuadWorldMapMedalBronze.Substyle = "MedalSlot";
            QuadWorldMapMedalBronze.Opacity = 0.5f;
        }
    }

    // the panel isn't rebuilt after returning from a map, so pick up a new record as soon as the score manager has it
    private void RefreshWorldMapMedals()
    {
        if (Campaign is null || WorldMapCampaignNum == -1 || MapGroupNum == -1 || MapInfoNum == -1)
        {
            return;
        }

        var mapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];
        if (mapInfo.MapUid == WorldMapMedalsMapUid && ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, ScoreContextPrefix) == WorldMapMedalsTime)
        {
            return;
        }

        UpdateWorldMapMedals(mapInfo);
    }

    private void UpdateWorldMapDetails(CMapInfo mapInfo)
    {
        LabelWorldMapName.SetText(mapInfo.Name);
        var diamond = SelectedWorldDiamond;
        if (diamond is null)
        {
            diamond = HoveredWorldDiamond;
        }
        if (diamond is not null)
        {
            WorldMapPanelDiamond = diamond;
            LabelWorldMapLocation.SetText(diamond.Parent.DataAttributeGet("location"));
        }

        var thumbnailUrl = $"file://Thumbnails/MapUid/{mapInfo.MapUid}";
        if (QuadWorldMapThumbnail.ImageUrl != thumbnailUrl)
        {
            QuadWorldMapThumbnail.ChangeImageUrl(thumbnailUrl);
        }
        LabelWorldMapEnvironment.SetText(mapInfo.CollectionName);
        LabelWorldMapVehicle.SetText(GetDefaultCar(mapInfo));
        if (mapInfo.MapStyle == "")
        {
            LabelWorldMapStyle.SetText("Unspecified");
        }
        else
        {
            LabelWorldMapStyle.SetText(mapInfo.MapStyle);
        }
        if (mapInfo.TMObjective_AuthorTime < 0)
        {
            LabelWorldMapTrackmaster.SetText("-:--.---");
        }
        else
        {
            LabelWorldMapTrackmaster.SetText(TimeToTextWithMilli(mapInfo.TMObjective_AuthorTime));
        }

        var persistent_SecretPlace = Persistent<Dictionary<string, int>>.For(LocalUser);
        if (persistent_SecretPlace.Get().ContainsKey(mapInfo.MapUid) && persistent_SecretPlace.Get()[mapInfo.MapUid] > 0)
        {
            LabelWorldMapSecretPlace.SetText("Discovered");
        }
        else
        {
            LabelWorldMapSecretPlace.SetText("Undiscovered");
        }

        UpdateWorldMapMedals(mapInfo);

        var showPanel = FrameWorld.Visible && EnableMenuNavigationInputs && diamond is not null;
        if (showPanel && !FrameWorldMapPanel.Visible)
        {
            OpenWorldMapPanel();
        }
        FrameWorldMapPanel.Visible = showPanel;
        SetWorldMapPanelExpanded(SelectedWorldDiamond is not null && SelectedWorldMapUid == mapInfo.MapUid);
        UpdateWorldMapPanelPosition();
    }

    private void UpdateWorldMapPanel()
    {
        LabelSelectedMapName.TextSizeReal = 3;
        LabelSelectedMapName.RelativePosition_V3.Y = -3;

        var mapUid = SelectedWorldMapUid;
        if (mapUid == "")
        {
            mapUid = DisplayedWorldMapUid;
            if (HoveredWorldMapUid != "")
            {
                mapUid = HoveredWorldMapUid;
            }
        }

        var mapChanged = mapUid != DisplayedWorldMapUid || WorldMapCampaignNum == -1 || MapGroupNum == -1 || MapInfoNum == -1;
        if (mapChanged)
        {
            DisplayedWorldMapUid = mapUid;
            WorldMapCampaignNum = -1;
            MapGroupNum = -1;
            MapInfoNum = -1;
            MapSelectedAt = -1;
            UnloadLeaderboards();

            if (mapUid != "")
            {
                ResolveWorldMap(mapUid);
            }

            var worldMapCampaignNum = Local<int>.For(Page);
            worldMapCampaignNum.Set(WorldMapCampaignNum);
        }

        if (WorldMapCampaignNum == -1)
        {
            FrameWorldMapPanel.Hide();
            SetWorldMapPanelExpanded(false);
            LabelSelectedMapName.SetText("...");
            QuadPlay.Hide();
            QuadExplore.Hide();
            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
            ResetRanks();
            return;
        }

        Campaign = GetCampaignForMaps();
        var mapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];
        LabelSelectedMapName.SetText(mapInfo.Name);
        var nameWidth = LabelSelectedMapName.ComputeWidth("$o" + mapInfo.Name);
        if (nameWidth > LabelSelectedMapName.Size.X)
        {
            LabelSelectedMapName.TextSizeReal *= (float)LabelSelectedMapName.Size.X / nameWidth;
        }
        QuadPlay.Show();
        QuadExplore.Show();
        UpdateGeneralStats(mapInfo);
        UpdateWorldMapDetails(mapInfo);

        if (mapUid == SelectedWorldMapUid && MapSelectedAt == -1)
        {
            MapSelectedAt = Now;
            LoadLeaderboards(true);
        }
    }

    private void SetupCampaign(bool optimizedMode, bool focusOnSelected)
    {
        QuadOfficialCampaign.StyleSelected = SelectedCampaignQuad == QuadOfficialCampaign;
        QuadVRCampaign.StyleSelected = IsCampaignSelected(QuadVRCampaign);
        QuadVROffzoneCampaign.StyleSelected = SelectedCampaignQuad == QuadVROffzoneCampaign;
        QuadOpreaxCampaign.StyleSelected = SelectedCampaignQuad == QuadOpreaxCampaign;
        QuadVROffzoneDisabled.Visible = !VROffzoneEnabled;

        FrameWorld.Visible = SelectedCampaignQuad == QuadOpreaxCampaign;
        FrameWorldDiamonds.Visible = FrameWorld.Visible;
        FrameCampaignMaps.Visible = !FrameWorld.Visible;
        FrameCampaignGrid.Visible = !FrameWorld.Visible;

        if (!FrameWorld.Visible)
        {
            LabelSelectedMapName.TextSizeReal = 9;
            LabelSelectedMapName.RelativePosition_V3.Y = 0;
            FrameWorldMapPanel.Hide();
            SetWorldMapPanelExpanded(false);
        }

        if (FrameWorld.Visible)
        {
            UpdateWorldMapPanel();
            return;
        }

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

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

                        if (SelectedCampaignQuad == QuadVRCampaign || SelectedCampaignQuad == QuadVROffzoneCampaign)
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
                        var nothingIsFinished = true;

                        foreach (var carName in AllCars)
                        {
                            var scoreContext = $"{ScoreContextPrefix}{carName}";

                            var isDefaultCar = IsDefaultCar(mapInfo, carName);

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

                            nothingIsFinished = false;

                            var playerTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);

                            if (playerTime == -1)
                            {
                                completed = false;
                                break;
                            }
                        }

                        labelCompleted.Visible = completed && !nothingIsFinished;

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
                if (SelectedCampaignQuad == QuadVRCampaign || SelectedCampaignQuad == QuadVROffzoneCampaign)
                {
                    visualMapNumber = visualMapNumber % 10;
                }
                selectedNum = visualMapNumber;
            }

            LabelSelectedMapName.SetText(TextLib.FormatInteger(selectedNum + 1, 3));
            QuadPlay.Visible = true;
            QuadExplore.Visible = true;

            UpdateGeneralStats(selectedMapInfo);
        }
        else
        {
            LabelSelectedMapName.SetText("...");
            QuadPlay.Visible = false;
            QuadExplore.Visible = false;

            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
        }
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
        if (MapGroupNum == -1 || MapInfoNum == -1)
        {
            return;
        }

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

    private void ClearWorldDiamondHover(bool updatePanel)
    {
        if (HoveredWorldDiamond is not null)
        {
            AnimMgr.Flush(HoveredWorldDiamond);
            AnimMgr.Add(HoveredWorldDiamond, "<frame scale=\"1\"/>", 180, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        HoveredWorldDiamond = null;
        HoveredWorldMapUid = "";
        if (updatePanel && FrameWorld.Visible && SelectedWorldDiamond is null)
        {
            UpdateWorldMapPanel();
        }
    }

    private void HoverWorldDiamond(CMlFrame diamond)
    {
        if (HoveredWorldDiamond == diamond)
        {
            return;
        }

        ClearWorldDiamondHover(false);
        HoveredWorldDiamond = diamond;
        HoveredWorldMapUid = diamond.Parent.DataAttributeGet("mapuid");
        AnimMgr.Flush(diamond);
        AnimMgr.Add(diamond, "<frame scale=\"1.2\"/>", 180, CAnimManager.EAnimManagerEasing.QuadOut);
        if (SelectedWorldDiamond is null)
        {
            UpdateWorldMapPanel();
        }
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
    }

    private void DeselectWorldDiamond()
    {
        if (SelectedWorldDiamond is null)
        {
            return;
        }

        var glow = (SelectedWorldDiamond.Controls[0] as CMlQuad)!;
        var icon = (SelectedWorldDiamond.Controls[1] as CMlQuad)!;
        AnimMgr.Flush(glow);
        AnimMgr.Add(glow, "<quad scale=\"0\"/>", 180, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(glow, "<quad hidden=\"1\"/>", Now + 180, 0, CAnimManager.EAnimManagerEasing.Linear);
        AnimMgr.Flush(icon);
        AnimMgr.Add(icon, "<quad rot=\"0\"/>", 180, CAnimManager.EAnimManagerEasing.QuadOut);

        SelectedWorldDiamond = null;
        SelectedWorldMapUid = "";
        WorldDiamondSelectedAt = -1;
        MapSelectedAt = -1;
        UnloadLeaderboards();
    }

    private void SelectWorldDiamond(CMlFrame diamond)
    {
        if (HoveredWorldDiamond != diamond)
        {
            ClearWorldDiamondHover(false);
        }

        if (SelectedWorldDiamond == diamond)
        {
            PlaySelectedMap();
            AudioPlayClick();
            return;
        }

        DeselectWorldDiamond();

        SelectedWorldDiamond = diamond;
        SelectedWorldMapUid = diamond.Parent.DataAttributeGet("mapuid");
        WorldDiamondSelectedAt = Now;

        var glow = (diamond.Controls[0] as CMlQuad)!;
        var icon = (diamond.Controls[1] as CMlQuad)!;
        AnimMgr.Flush(icon);
        icon.RelativeRotation = 0;
        AnimMgr.Flush(glow);
        if (!glow.Visible)
        {
            glow.RelativeScale = 0;
        }
        glow.Show();
        AnimMgr.Add(glow, "<quad scale=\"1\"/>", 180, CAnimManager.EAnimManagerEasing.QuadOut);
        UpdateWorldMapPanel();
        AudioPlayClick();
    }

    private void SelectCampaign(CMlQuad quad, string campaignName)
    {
        if (SelectedCampaignQuad == quad)
        {
            return;
        }

        ClearWorldDiamondHover(false);

        MapGroupNum = -1;
        MapInfoNum = -1;
        MapSelectedAt = -1;

        DisplayedWorldMapUid = "";
        WorldMapCampaignNum = -1;

        AudioPlayClick();
        SelectedCampaignQuad = quad;

        var selectedCampaign = Local<string>.For(Page);
        selectedCampaign.Set(campaignName);

        ResetPBs();
        ResetValidators();
        ResetRatings();
        ResetStars();
        ResetRanks();
        UnloadLeaderboards();
        FrameCampaign.ScrollOffset.Y = 0;
        FrameTooltip.Hide();
        SetupCampaign(false, false);
    }

    private void SelectOfficialCampaign()
    {
        SelectCampaign(QuadOfficialCampaign, "");
    }

    private void SelectVRCampaign()
    {
        if (VROffzoneEnabled && VROffzoneCampaignReleased)
        {
            SelectCampaign(QuadVROffzoneCampaign, "VROffzone");
        }
        else if (!VROffzoneEnabled && VRCampaignReleased)
        {
            SelectCampaign(QuadVRCampaign, "VR");
        }
    }

    private void ToggleVROffzoneCampaign()
    {
        if ((VROffzoneEnabled && !VRCampaignReleased) || (!VROffzoneEnabled && !VROffzoneCampaignReleased))
        {
            return;
        }

        VROffzoneEnabled = !VROffzoneEnabled;
        SelectVRCampaign();
    }

    private void RevealWorldDiamond(CMlFrame location, int delay)
    {
        var icon = ((location.Controls[0] as CMlFrame)!.Controls[1] as CMlQuad)!;

        location.DataAttributeSet("revealed", "1");

        AnimMgr.Flush(location);
        AnimMgr.Flush(icon);
        location.RelativeScale = 4;
        icon.Opacity = 0;
        location.Show();

        AnimMgr.Add(location, "<frame scale=\"1\"/>", Now + delay, 700, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(icon, "<quad opacity=\"1\"/>", Now + delay, 500, CAnimManager.EAnimManagerEasing.QuadOut);
        Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: delay);
    }

    private void LoadWorldMapMedals()
    {
        if (DataFileMgr.Campaigns.Count == WorldMapMedalsCampaignCount)
        {
            return;
        }

        WorldMapMedalsCampaignCount = DataFileMgr.Campaigns.Count;
        WorldMapMedals.Clear();

        if (DataFileMgr.Campaigns.Count <= OpreaxCampaignNum)
        {
            return;
        }

        var campaign = DataFileMgr.Campaigns[OpreaxCampaignNum];
        foreach (var mapGroup in campaign.MapGroups)
        {
            foreach (var mapInfo in mapGroup.MapInfos)
            {
                SWorldMapMedals medals = new()
                {
                    Author = mapInfo.TMObjective_AuthorTime,
                    Gold = mapInfo.TMObjective_GoldTime,
                    Silver = mapInfo.TMObjective_SilverTime,
                    Bronze = mapInfo.TMObjective_BronzeTime
                };
                WorldMapMedals[mapInfo.MapUid] = medals;
            }
        }
    }

    private int GetWorldMapMedalPoints(string mapUid)
    {
        if (!WorldMapMedals.ContainsKey(mapUid))
        {
            return 0;
        }

        // default car only
        var time = ScoreMgr.Map_GetRecord(null, mapUid, ScoreContextPrefix);
        if (time < 0)
        {
            return 0;
        }

        var medals = WorldMapMedals[mapUid];
        if (time <= medals.Author)
        {
            return 5;
        }
        if (time <= medals.Gold)
        {
            return 3;
        }
        if (time <= medals.Silver)
        {
            return 2;
        }
        if (time <= medals.Bronze)
        {
            return 1;
        }
        return 0;
    }

    private void UpdateWorldProgress(int points, int nextUnlock)
    {
        GaugeWorldNextUnlock.Ratio = 0;
        GaugeWorldCampaignProgress.Ratio = 0;

        if (WorldMapMedals.Count == 0)
        {
            LabelWorldUnlockProgress.SetText("...waiting for the Opreax campaign maps...");
            LabelWorldNextUnlockProgress.SetText("-- (next unlock)");
            LabelWorldCampaignProgress.SetText("-- (campaign)");
            return;
        }

        var campaignPoints = 0;
        foreach (var (mapUid, medals) in WorldMapMedals)
        {
            campaignPoints += GetWorldMapMedalPoints(mapUid);
        }
        var maximumPoints = WorldMapMedals.Count * 5;
        GaugeWorldCampaignProgress.Ratio = MathLib.Clamp(campaignPoints * 1f / maximumPoints, 0f, 1f);
        LabelWorldCampaignProgress.SetText($"{campaignPoints}/{maximumPoints} $o(campaign)");
        LabelWorldUnlockProgress.SetText("...collect medals with the default car to earn medal points");

        if (nextUnlock == -1)
        {
            GaugeWorldNextUnlock.Ratio = 1;
            LabelWorldNextUnlockProgress.SetText("All maps unlocked!");
        }
        else
        {
            GaugeWorldNextUnlock.Ratio = MathLib.Clamp(points * 1f / nextUnlock, 0f, 1f);
            LabelWorldNextUnlockProgress.SetText($"{points}/{nextUnlock} $o(next unlock)");
        }
    }

    private void UpdateWorldUnlocks(int revealDelay)
    {
        LoadWorldMapMedals();

        var points = 0;
        foreach (var control in FrameWorldDiamonds.Controls)
        {
            if (control is not CMlFrame location)
            {
                continue;
            }

            var medalPoints = GetWorldMapMedalPoints(location.DataAttributeGet("mapuid"));
            location.DataAttributeSet("medalpoints", medalPoints.ToString());
            points += medalPoints;
        }

        WorldMedalPoints = points;

        // only animate newly unlocked pairs when the player can actually see them, otherwise keep them hidden until then
        var canAnimate = WorldUnlocksInitialized && FrameWorld.Visible && EnableMenuNavigationInputs;
        var nextUnlock = -1;
        var revealCount = 0;

        foreach (var control in FrameWorldDiamonds.Controls)
        {
            if (control is not CMlFrame location)
            {
                continue;
            }

            if (!WorldMapMedals.ContainsKey(location.DataAttributeGet("mapuid")))
            {
                location.Hide();
                location.DataAttributeSet("revealed", "");
                continue;
            }

            var unlock = TextLib.ToInteger(location.DataAttributeGet("unlock"));

            if (unlock > points)
            {
                if (nextUnlock == -1 || unlock < nextUnlock)
                {
                    nextUnlock = unlock;
                }
                location.Hide();
                location.DataAttributeSet("revealed", "");
                continue;
            }

            if (location.DataAttributeGet("revealed") == "1")
            {
                continue;
            }

            if (!WorldUnlocksInitialized)
            {
                location.DataAttributeSet("revealed", "1");
                location.Show();
                continue;
            }

            if (!canAnimate)
            {
                continue;
            }

            RevealWorldDiamond(location, revealDelay + revealCount * 250);
            revealCount += 1;
        }

        // the first pass with loaded campaigns shows already unlocked maps without animation
        if (DataFileMgr.Campaigns.Count > OpreaxCampaignNum)
        {
            WorldUnlocksInitialized = true;
        }

        UpdateWorldProgress(points, nextUnlock);
    }

    private void UpdateWorldDiamondHeartbeats()
    {
        var phase = Now % 5000;
        var scale = 1f;
        if (phase < 360)
        {
            scale += MathLib.Sin(phase / 360f * MathLib.PI()) * 0.12f;
        }
        else if (phase < 660)
        {
            scale += MathLib.Sin((phase - 360) / 300f * MathLib.PI()) * 0.07f;
        }

        foreach (var control in FrameWorldDiamonds.Controls)
        {
            if (control is not CMlFrame location)
            {
                continue;
            }

            var diamond = (location.Controls[0] as CMlFrame)!;
            var icon = (diamond.Controls[1] as CMlQuad)!;
            if (FrameWorld.Visible && EnableMenuNavigationInputs && location.Visible
                && location.DataAttributeGet("revealed") == "1" && location.DataAttributeGet("medalpoints") != "5")
            {
                icon.RelativeScale = scale;
            }
            else
            {
                icon.RelativeScale = 1;
            }
        }
    }

    public void Main()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        FrameCampaignGrid.RelativePosition_V3.Y = -50;
        FrameWorldDiamonds.RelativePosition_V3.Y = -50;
        FrameMapOverview.RelativePosition_V3.Y = 10;

        WorldMapPanelHeight = 12;
        WorldMapPanelFromHeight = 12;
        WorldMapPanelTargetHeight = 12;

        TM2Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "" };
        TMUFCars = new() { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        FunnyCars = new() { "HighlandCar", "DumpsterCar", "ToasterCar", "FunnyCar" };
        AllCars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        SelectedCampaignQuad = QuadOfficialCampaign;

        var mapDefaultCars = Local<Dictionary<string, string>>.For(LocalUser);
        mapDefaultCars.Get().Clear();

        foreach (var control in FrameWorldDiamonds.Controls)
        {
            if (control is not CMlFrame location)
            {
                continue;
            }

            var mapUid = location.DataAttributeGet("mapuid");
            var car = location.DataAttributeGet("car");
            if (mapUid != "" && car != "")
            {
                mapDefaultCars.Get()[mapUid] = car;
            }
            WorldMapUnlocks[mapUid] = TextLib.ToInteger(location.DataAttributeGet("unlock"));
        }

        SwitchCars(false);
        SetupCampaign(false, false);
        UpdateWorldUnlocks(0);

        SuperMedalsRequest = Http.CreateGet("file://Media/Medals.json");
    }

    public void Loop()
    {
        UpdateWorldDiamondHeartbeats();
        UpdateWorldMapPanelAnimation();
        if (FrameWorldMapPanel.Visible)
        {
            UpdateWorldMapPanelPosition();
            RefreshWorldMapMedals();
        }

        if (SelectedWorldDiamond is not null && FrameWorld.Visible && EnableMenuNavigationInputs)
        {
            var diamondIcon = (SelectedWorldDiamond.Controls[1] as CMlQuad)!;
            var elapsed = Now - WorldDiamondSelectedAt;
            var amplitude = MathLib.Min(1f, elapsed / 180f) * 5f;
            diamondIcon.RelativeRotation = MathLib.Sin(elapsed * 0.006f) * amplitude;
        }

        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }

        if (FrameTooltip.Visible)
        {
            FrameTooltip.RelativePosition_V3 = new Vec2(MouseX, MouseY);
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

                        if (campaign.ScoreContext == $"{ScoreContextPrefix}{GetDefaultCar(mapInfo)}")
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
            UpdateWorldUnlocks(0);

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

        /*if (CampaignsReleasedAt.ContainsKey("VR"))
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

        if (CampaignsReleasedAt.ContainsKey("VROffzone"))
        {
            var releasedAt = CampaignsReleasedAt["VROffzone"];

            if (TimeLib.Compare(releasedAt, TimeLib.GetCurrent()) > 0)
            {
                LabelVROffzoneCountdown.Value = TimeLib.FormatDelta(releasedAt, TimeLib.GetCurrent(), TimeLib.EDurationFormats.Abbreviated);
                LabelVROffzoneCountdown.Show();
            }
            else
            {
                VROffzoneCampaignReleased = true;
                LabelVROffzoneCountdown.Hide();
            }
        }
        else
        {
            LabelVROffzoneCountdown.Visible = false;
        }*/

        LabelVRCampaign.Value = "VR campaign";
        VRCampaignReleased = true;

        VROffzoneCampaignReleased = true;
        LabelVROffzoneCountdown.Hide();

        UpdateOpreaxCampaignRelease();

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

                UpdateLeaderboards(false);
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

    private void Quickplay()
    {
        if (SelectedCampaignQuad != QuadOpreaxCampaign)
        {
            SendCustomEvent("Quickplay", new[] { "" });
            return;
        }

        UpdateWorldUnlocks(0);
        ImmutableArray<string> unlockedMapUids = new();
        foreach (var control in FrameWorldDiamonds.Controls)
        {
            if (control is not CMlFrame location)
            {
                continue;
            }

            var mapUid = location.DataAttributeGet("mapuid");
            if (WorldMapMedals.ContainsKey(mapUid) && WorldMapUnlocks[mapUid] <= WorldMedalPoints)
            {
                unlockedMapUids.Add(mapUid);
            }
        }
        SendCustomEvent("Quickplay", (string[])(IList<string>)unlockedMapUids);
    }

    private void UpdateOpreaxCampaignRelease()
    {
        // locked until the release info request finishes, playing an unreleased campaign map gets the player auto-banned
        // Opreax temporarily requires a connection and a successful release check, even for unlocked maps
        OpreaxCampaignReleased = CampaignsReleaseChecked && LocalUser.IsConnectedToMasterServer;
        var labelText = "World of Opreax";

        if (CampaignsReleasedAt.ContainsKey("Opreax"))
        {
            var releasedAt = CampaignsReleasedAt["Opreax"];

            // admins and testers keep access before the release, the webapi allows them as well
            if (TimeLib.Compare(releasedAt, TimeLib.GetCurrent()) > 0 && !EnvimixTurboUserIsAdmin && !EnvimixTurboUserCanAccessUnreleased)
            {
                OpreaxCampaignReleased = false;
                labelText = TimeLib.FormatDelta(releasedAt, TimeLib.GetCurrent(), TimeLib.EDurationFormats.Abbreviated);
            }
        }

        if (!LocalUser.IsConnectedToMasterServer)
        {
            labelText = "Unavailable offline";
        }

        if (LabelOpreaxCampaign.Value != labelText)
        {
            LabelOpreaxCampaign.Value = labelText;
        }

        var pulse = (MathLib.Sin(Now * 0.003f) + 1) * 0.5f;
        QuadOpreaxCampaignGlow.Opacity = 0.25f + pulse * 0.65f;
        LabelOpreaxCampaignNew.Visible = OpreaxCampaignReleased;
        LabelOpreaxCampaignNew.Opacity = 0.4f + pulse * 0.6f;

        // release date could have been moved later while the campaign was already open
        if (!OpreaxCampaignReleased && SelectedCampaignQuad == QuadOpreaxCampaign)
        {
            SelectOfficialCampaign();
        }
    }

    private void SelectOpreaxCampaign()
    {
        if (!OpreaxCampaignReleased)
        {
            return;
        }

        SelectCampaign(QuadOpreaxCampaign, "Opreax");
        // reveals pairs unlocked while the world map wasn't shown
        UpdateWorldUnlocks(300);
    }

    private void UpdateTooltip(string text)
    {
        var labelText = (FrameTooltip.GetFirstChild("LabelText") as CMlLabel)!;
        var quadTooltip = (FrameTooltip.GetFirstChild("QuadTooltip") as CMlQuad)!;

        labelText.Size.X = labelText.ComputeWidth(text);
        quadTooltip.Size.X = labelText.ComputeWidth(text) + 4;
        labelText.SetText(text);

        FrameTooltip.Show();
    }
}
