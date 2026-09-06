using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

/// <summary>
/// This code went through 5 stages of development from 2019-2025, the code is quite confusing, sorry.
/// </summary>
public class MultiplayerMenu : CTmMlScriptIngame, IContext
{
    public struct SSkin
    {
        public string Model;
        public string File;
		public string Icon;
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

    public struct SGhostMetadata
    {
        public string FileName;
        public int Index;
        public string Nickname;
        public int Time;
    }

    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    [ManialinkControl] public required CMlFrame FrameInnerVehicles;
    [ManialinkControl] public required CMlFrame FrameOuterSkinList;
    [ManialinkControl] public required CMlFrame FrameSkinList;
    [ManialinkControl] public required CMlQuad QuadButtonSpectator;
    [ManialinkControl] public required CMlFrame FrameMenu;
    [ManialinkControl] public required CMlFrame FrameAdvancedSettings;
    [ManialinkControl] public required CMlFrame FrameSkins;
	[ManialinkControl] public required CMlFrame FrameButtonSpectator;
	[ManialinkControl] public required CMlQuad QuadButtonContinue;
	[ManialinkControl(IgnoreValidation = true)] public required CMlFrame FrameVehicles;
	[ManialinkControl] public required CMlFrame FrameVehicleList;
    [ManialinkControl] public required CMlLabel LabelArrow;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlFrame FrameLabelMapType;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelSkinCar;
    [ManialinkControl] public required CMlLabel LabelPbNickname;
    [ManialinkControl] public required CMlLabel LabelPbTime;
    [ManialinkControl] public required CMlLabel LabelServerName;
    [ManialinkControl] public required CMlLabel LabelMode;
    [ManialinkControl] public required CMlLabel LabelTimeLimit;
    [ManialinkControl] public required CMlLabel LabelPlayerCount;
    [ManialinkControl] public required CMlLabel LabelSpectatorCount;
    [ManialinkControl] public required CMlFrame FramePlayers;
    [ManialinkControl] public required CMlQuad QuadSkinScrollbar;
    [ManialinkControl] public required CMlQuad QuadSkinScrollable;
    [ManialinkControl] public required CMlQuad QuadBackground;
    [ManialinkControl] public required CMlLabel LabelLock;
    [ManialinkControl] public required CMlFrame FrameArrow;
    [ManialinkControl] public required CMlFrame FrameGhostArrow;
    [ManialinkControl] public required CMlQuad QuadButtonSkin;
    [ManialinkControl] public required CMlQuad QuadButtonAdvanced;
    [ManialinkControl] public required CMlQuad QuadButtonModeHelp;
    [ManialinkControl] public required CMlQuad QuadButtonServerDetails;
    [ManialinkControl] public required CMlQuad QuadButtonSessionDetails;
    [ManialinkControl] public required CMlLabel LabelButtonServerDetails;
    [ManialinkControl] public required CMlLabel LabelButtonSessionDetails;
    [ManialinkControl] public required CMlQuad QuadButtonManageServer;
    [ManialinkControl] public required CMlQuad QuadButtonExit;
    [ManialinkControl] public required CMlQuad QuadButtonAdvancedSettings;
    [ManialinkControl] public required CMlQuad QuadButtonSkinPlay;
    [ManialinkControl] public required CMlQuad QuadButtonSkinBack;
    [ManialinkControl] public required CMlQuad QuadButtonSettingsBack;
    [ManialinkControl] public required CMlFrame FrameQuicktip;
    [ManialinkControl] public required CMlLabel LabelMapAuthor;
    [ManialinkControl] public required CMlFrame FrameMultiplayer;
    [ManialinkControl] public required CMlFrame FrameButtonManageServer;
    [ManialinkControl] public required CMlFrame FrameButtonChooseSkin;
    [ManialinkControl] public required CMlFrame FrameButtonAdvancedOptions;
    [ManialinkControl] public required CMlFrame FrameTooltip;
    [ManialinkControl] public required CMlFrame FrameMessageBox;
    [ManialinkControl] public required CMlFrame FrameMessageBoxConfirm;
    [ManialinkControl] public required CMlLabel LabelMessageBoxName;
    [ManialinkControl] public required CMlLabel LabelMessageBoxDescription;
    [ManialinkControl] public required CMlQuad QuadButtonMessageBoxClose;
    [ManialinkControl] public required CMlQuad QuadButtonMessageBoxConfirm;
    [ManialinkControl] public required CMlLabel LabelValidator;
    [ManialinkControl] public required CMlQuad QuadStarButton;
    [ManialinkControl] public required CMlQuad QuadButtonEnableVoiceOnImpact;
    [ManialinkControl] public required CMlQuad QuadButtonEnableVoiceOnWaypoint;

    public int VehicleIndex;
    public int PreviousVehicleIndex;
    public string MenuKind;
    public string PreviousMenuKind;
    public string PreviousMapUid = " ";
    public string PreviousCar;
    public string PreviousMapAuthor;
    public bool IsMenuOpen;
    public int ShowMenuLittleLater = -1;
    public bool NavOnVehicle;
    public CMlQuad NavFirstControl;
    public CMlQuad NavFocusedControl;
    public bool PreviousEnableDefaultCar;
	public int UserShift;
    public float PreviousScrollOffset;
    public Vec2 PreviousSkinScrollOffset;
	public CUIConfig.EUISequence PreviousUISequence;
	public int MenuOpenTime = -1;
    public bool GravityOpen;
    public int PrevGravityValue = 1;
    public IList<CReplayInfo> LocalReplays;
    public CTaskResult_ReplayList? LocalReplaysTask;
    public IList<string> Zones;
    public int CurrentZoneIndex = 0;
    public int PrevLocalGhostMetadataUpdatedAt = -1;
    public required Dictionary<string, bool> SelectedGhosts;
    public int PrevRatingsUpdatedAt;
    public bool PrevRatingEnabled;
    public int PrevValidationsUpdatedAt;
    public string MapNameInExplore = "";
    public bool PrevGhostToUpload;
    public string PrevClientCar;
    public Dictionary<CTaskResult_Ghost, string> DownloadGhostTasks;
    public Dictionary<CTaskResult, string> SaveReplayTasks;
    public IList<string> DownloadedReplayFiles;
    public float PrevGhostsScrollY;
    public bool HoldRecordsScrollbar;
    public float HoldRecordsScrollbarPos;
    public bool ScrollbarRecordsMouseOut;
    public bool HoldSkinsScrollbar;
    public float HoldSkinsScrollbarPos;
    public bool ScrollbarSkinsMouseOut;
    public int PrevGameTime;
    public string PreviousEnvimaniaSessionId = "";
    public bool IsMessageBoxOpen;
    public CMlQuad MessageBoxReturnControl;

    [Netwrite(NetFor.UI)] public string ClientCar { get; set; }
    [Netwrite(NetFor.UI)] public Dictionary<string, string> UserSkins { get; set; }
    [Netwrite(NetFor.UI)] public int ClientGravity { get; set; }
    [Netwrite(NetFor.UI)] public IList<string> LocalReplayFiles { get; set; }
    [Netread] public bool EnableDefaultCar { get; set; }
    [Netread] public bool OverrideEnableDefaultCar { get; set; }
    [Netread] public string MapPlayerModelName { get; set; }
    [Netread] public int CutOffTimeLimit { get; set; }
    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }
    [Netread] public Dictionary<string, string> ItemCars { get; set; }
    [Netread] public Dictionary<string, Dictionary<string, SSkin>> Skins { get; set; }
    [Netread] public string EnvimixWebAPI { get; set; }
    [Netread] public string EnvimaniaSessionId { get; set; }
    [Netread] public IList<SGhostMetadata> LocalGhostMetadata { get; }
    [Netread] public int LocalGhostMetadataUpdatedAt { get; }

    [Netread] public bool RatingEnabled { get; }
    //[Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    //[Netread] public required int RatingsUpdatedAt { get; set; }
    [Netread] public string ModeHelp { get; set; }
    //[Netread] public required Dictionary<string, SEnvimaniaRecord> Validations { get; set; }
    //[Netread] public int ValidationsUpdatedAt { get; set; }

    [Netread] public bool GhostToUpload { get; set; }

    [Netread] public required Dictionary<string, SStar> Stars { get; set; }

    [Netread] public required Dictionary<string, int> Skillpoints { get; set; }
    [Netread] public required Dictionary<string, int> ActivityPoints { get; set; }

    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";

    public MultiplayerMenu()
    {
        MouseOver += Menu_MouseOver;
        MouseOut += Menu_MouseOut;
        MouseClick += Menu_MouseClick;

        QuadButtonContinue.MouseOver += Focus2;
        QuadButtonSpectator.MouseOver += Focus2;
        QuadButtonSkin.MouseOver += Focus2;
        QuadButtonSkinPlay.MouseOver += Focus2;
        QuadButtonSkinBack.MouseOver += Focus2;
        QuadButtonAdvanced.MouseOver += Focus2;
        QuadButtonModeHelp.MouseOver += () =>
        {
            Focus3();
            UpdateTooltip("Mode help");
        };
        QuadButtonServerDetails.MouseOver += () =>
        {
            if (EnvimaniaSessionId == "")
            {
                UpdateTooltip("No active Envimania session");
            }
            else
            {
                Focus3();
                UpdateTooltip("Server details");
            }
        };
        QuadButtonSessionDetails.MouseOver += () =>
        {
            if (EnvimaniaSessionId == "")
            {
                UpdateTooltip("No active Envimania session");
            }
            else
            {
                Focus3();
                UpdateTooltip("Session details");
            }
        };
        QuadButtonManageServer.MouseOver += Focus2;
        QuadButtonExit.MouseOver += Focus2;
        QuadButtonAdvancedSettings.MouseOver += Focus2;
        QuadButtonSettingsBack.MouseOver += Focus2;

        QuadButtonContinue.MouseClick += QUAD_BUTTON_CONTINUE;
        QuadButtonSpectator.MouseClick += QUAD_BUTTON_SPECTATOR;
        QuadButtonManageServer.MouseClick += QUAD_BUTTON_MANAGESERVER;
        QuadButtonExit.MouseClick += QUAD_BUTTON_EXIT;
        QuadButtonAdvanced.MouseClick += QUAD_BUTTON_ADVANCED;
        QuadButtonAdvancedSettings.MouseClick += QUAD_BUTTON_ADVANCEDSETTINGS;
        QuadButtonSkin.MouseClick += QUAD_BUTTON_SKIN;
        QuadButtonSkinPlay.MouseClick += QUAD_BUTTON_SKIN_PLAY;
        QuadButtonSkinBack.MouseClick += QUAD_BUTTON_SKIN_BACK;
        QuadButtonSettingsBack.MouseClick += QUAD_BUTTON_SETTINGS_BACK;
        QuadButtonModeHelp.MouseClick += ShowCustomModeHelp;
        QuadButtonServerDetails.MouseClick += () =>
        {
            if (EnvimaniaSessionId != "")
            {
                OpenLink($"https://envimix.gbx.tools/envimania/servers/{CurrentServerLogin}", CMlScript.LinkType.ExternalBrowser);
            }
        };
        QuadButtonSessionDetails.MouseClick += () =>
        {
            if (EnvimaniaSessionId != "")
            {
                OpenLink($"https://envimix.gbx.tools/envimania/sessions/{EnvimaniaSessionId}", CMlScript.LinkType.ExternalBrowser);
            }
        };

        MenuNavigation += Menu_MenuNavigation;

        QuadButtonMessageBoxClose.MouseClick += QuadButtonMessageBoxClose_MouseClick;

        QuadButtonMessageBoxClose.MouseOver += () =>
        {
            Focus2();
        };
        QuadButtonMessageBoxConfirm.MouseOver += Focus2;

        QuadStarButton.MouseOver += () =>
        {
            AnimMgr.Add(QuadStarButton, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            Focus2();
        };

        QuadStarButton.MouseOut += () =>
        {
            AnimMgr.Add(QuadStarButton, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            Focus2();
        };

        QuadButtonEnableVoiceOnImpact.MouseClick += () =>
        {
            var persistent_EnvimixVoiceOnImpact = Persistent<float>.For(LocalUser);
            var parent = QuadButtonEnableVoiceOnImpact.Parent;
            if (persistent_EnvimixVoiceOnImpact.Get() < 0.01f)
            {
                persistent_EnvimixVoiceOnImpact.Set(0.1f);
                (parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on impact";
            }
            else
            {
                persistent_EnvimixVoiceOnImpact.Set(0f);
                (parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on impact";
            }
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadButtonEnableVoiceOnImpact.MouseOver += () =>
        {
            Focus3();
        };

        QuadButtonEnableVoiceOnWaypoint.MouseClick += () =>
        {
            var persistent_EnvimixVoiceOnWaypoint = Persistent<float>.For(LocalUser);
            var parent = QuadButtonEnableVoiceOnWaypoint.Parent;
            if (persistent_EnvimixVoiceOnWaypoint.Get() < 0.01f)
            {
                persistent_EnvimixVoiceOnWaypoint.Set(0.15f);
                (parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on waypoint";
            }
            else
            {
                persistent_EnvimixVoiceOnWaypoint.Set(0f);
                (parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on waypoint";
            }
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadButtonEnableVoiceOnWaypoint.MouseOver += () =>
        {
            Focus3();
        };

        QuadSkinScrollbar.MouseClick += () =>
        {
            HoldSkinsScrollbar = true;
            HoldSkinsScrollbarPos = MouseY - (float)QuadSkinScrollbar.RelativePosition_V3.Y;
        };

        QuadSkinScrollbar.MouseOver += () =>
        {
            AnimMgr.Add(QuadSkinScrollbar, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadSkinScrollbar.MouseOut += () =>
        {
            if (HoldSkinsScrollbar)
            {
                ScrollbarSkinsMouseOut = true;
            }
            else
            {
                AnimMgr.Add(QuadSkinScrollbar, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        };

        QuadButtonMessageBoxConfirm.MouseClick += () =>
        {
            CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Quit);
        };
    }

    private void QuadButtonMessageBoxClose_MouseClick()
    {
        AnimMgr.Add(FrameMessageBox, "<frame pos=\"0 -130\" hidden=\"1\"/>", 800, CAnimManager.EAnimManagerEasing.QuadOut);
        NavFocusedControl.StyleSelected = false;
        NavFocusedControl = MessageBoxReturnControl;
        NavFocusedControl.StyleSelected = true;
        IsMessageBoxOpen = false;
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null) return GUIPlayer;
        return InputPlayer;
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    string GetCar()
    {
        var car = Netread<string>.For(GetPlayer());
        return car.Get();
    }

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    string ConstructRatingFilterKey(string car)
    {
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car}_{gravity.Get()}_Time";
    }

    private int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
        }

        if (IndependantLaps)
        {
            return 0;
        }

        if (NbLaps == -1)
        {
            return Map.TMObjective_NbLaps;
        }

        return NbLaps;
    }

    string ConstructValidationFilterKey(string car)
    {
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car}_{gravity.Get()}_{GetLaps()}";
    }

    private void UpdateEnvimaniaSessionButtons()
    {
        var sessionAvailable = EnvimaniaSessionId != "";

        if (sessionAvailable)
        {
            QuadButtonServerDetails.Opacity = 1;
            QuadButtonSessionDetails.Opacity = 1;
            LabelButtonServerDetails.Opacity = 1;
            LabelButtonSessionDetails.Opacity = 1;
            QuadButtonServerDetails.DataAttributeSet("nav", "True");
            QuadButtonSessionDetails.DataAttributeSet("nav", "True");
        }
        else
        {
            QuadButtonServerDetails.Opacity = 0;
            QuadButtonSessionDetails.Opacity = 0;
            LabelButtonServerDetails.Opacity = 0.5f;
            LabelButtonSessionDetails.Opacity = 0.5f;
            QuadButtonServerDetails.DataAttributeSet("nav", "False");
            QuadButtonSessionDetails.DataAttributeSet("nav", "False");
            QuadButtonServerDetails.StyleSelected = false;
            QuadButtonSessionDetails.StyleSelected = false;

            if (NavFocusedControl == QuadButtonServerDetails || NavFocusedControl == QuadButtonSessionDetails)
            {
                NavFocusedControl = QuadButtonModeHelp;
                NavFocusedControl.StyleSelected = true;
            }
        }

        PreviousEnvimaniaSessionId = EnvimaniaSessionId;
    }

    bool IsDefaultCar(string car)
    {
        if (ItemCars.ContainsKey(car))
        {
            return ItemCars[car] == MapPlayerModelName;
        }

        return car == MapPlayerModelName;
    }

    private void UpdateVehicles()
    {
        for (var i = 0; i < FrameInnerVehicles.Controls.Count; i++)
        {
            var frame = (FrameInnerVehicles.Controls[i] as CMlFrame)!;
            var quadVehicle = (frame.GetFirstChild("QuadVehicle") as CMlQuad)!;
            var labelDefault = (frame.GetFirstChild("LabelDefault") as CMlLabel)!;
            var labelVehicle = (frame.GetFirstChild("LabelVehicle") as CMlLabel)!;
            var quadVehicleIcon = (frame.GetFirstChild("QuadVehicleIcon") as CMlQuad)!;

            if ((DisplayedCars.Contains(GetCar()) || GetCar() == "") && i == VehicleIndex)
            {
                quadVehicle.StyleSelected = true;
            }
            else
            {
                quadVehicle.StyleSelected = false;
            }

            if (DisplayedCars.Length <= i)
            {
                continue;
            }

            if (ItemCars.ContainsKey(DisplayedCars[i]) && ItemCars[DisplayedCars[i]] != MapPlayerModelName)
            {
                labelVehicle.Opacity = 1;
                quadVehicleIcon.Opacity = 1;
                labelDefault.Hide();
                continue;
            }

            if (EnableDefaultCar || OverrideEnableDefaultCar)
            {
                labelVehicle.Opacity = 1;
                labelDefault.Opacity = 1;
                quadVehicleIcon.Opacity = 1;
            }
            else
            {
                labelVehicle.Opacity = 0.5f;
                labelDefault.Opacity = 0.5f;
                quadVehicleIcon.Opacity = 0.5f;
            }

            labelDefault.Show();
        }
    }

    private bool IsTM2CarOnStadium(string carName)
    {
        return Map.MapInfo.CollectionName == "Stadium" && (carName == "CanyonCar" || carName == "LagoonCar" || carName == "ValleyCar");
    }

    private void UpdateSkins()
    {
        var carName = DisplayedCars[VehicleIndex];

        LabelSkinCar.Value = carName;

        ImmutableArray<string> sortedNames = new();

        if (Skins.ContainsKey(carName))
        {
            foreach (var (name, skin) in Skins[carName])
            {
                sortedNames.Add(name);
            }

            sortedNames = sortedNames.Sort();
        }

        var offset = MathLib.NearestInteger((float)FrameSkinList.Parent.ScrollOffset.Y / 15f);

        for (var i = 0; i < FrameSkinList.Controls.Count; i++)
        {
            var frame = (FrameSkinList.Controls[i] as CMlFrame)!;
            var quadSkin = (frame.GetFirstChild("QuadSkin") as CMlQuad)!;
            var quadIcon = (frame.GetFirstChild("QuadIcon") as CMlQuad)!;
            var labelName = (frame.GetFirstChild("LabelName") as CMlLabel)!;

            if (i + offset == 0)
            {
                labelName.Value = TextLib.GetTranslatedText("Default");
                quadIcon.ChangeImageUrl("");

                if (UserSkins.ContainsKey(carName))
                {
                    if (UserSkins[carName] == "")
                    {
                        quadSkin.StyleSelected = true;
                    }
                    else
                    {
                        quadSkin.StyleSelected = false;
                    }
                }
                else
                {
                    quadSkin.StyleSelected = true;
                }

                frame.Show();

                continue;
            }

            if (!Skins.ContainsKey(carName) || Skins[carName].Count <= i + offset - 1)
            {
                frame.Hide();
                continue;
            }

            var name = sortedNames[i + offset - 1];
            var skin = Skins[carName][name];

            labelName.Value = name;
            quadIcon.ChangeImageUrl("file://Media/" + skin.Icon);

            if (UserSkins.ContainsKey(carName))
            {
                if (UserSkins[carName] == name)
                {
                    quadSkin.StyleSelected = true;
                }
                else
                {
                    quadSkin.StyleSelected = false;
                }
            }
            else
            {
                quadSkin.StyleSelected = true;
            }

            frame.Show();
        }
    }

    private void ResumeMenu()
    {
        if (IsExplore())
        {
            SendCustomEvent("MenuOpen", new[] { "False" });
        }
        else
        {
            CloseInGameMenu(CMlScriptIngame.EInGameMenuResult.Resume);
        }
    }

    private void QUAD_BUTTON_SKIN_PLAY()
    {
        AnimMgr.Add(FrameMenu, "<frame pos=\"-110 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "";

        SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True" });
        ResumeMenu();
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    private void Menu_MouseClick(CMlControl control, string controlId)
    {
        switch (controlId)
        {
            case "QuadVehicle":
                PreviousVehicleIndex = VehicleIndex;
                var index = TextLib.ToInteger(control.Parent.DataAttributeGet("id"));
                FrameVehicles.Scroll(new Vec2(0f, (index - PreviousVehicleIndex) * 1f));

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

                if (PreviousVehicleIndex - index == 0)
                {
                    if (index < DisplayedCars.Length)
                    {
                        if (IsSpectator)
                        {
                            // suggest the player to play that car or something lol
                        }
                        else
                        {
                            if (InputPlayer.RaceStartTime - GameTime < 0)
                            {
                                SendCustomEvent("Car", new[] { DisplayedCars[index], "True" });
                            }
                            else
                            {
                                SendCustomEvent("Car", new[] { DisplayedCars[index], "False" });
                                SendCustomEvent("MusicSwitch", new[] { "" });
                            }
                        }

                        if (MenuKind == "Skin")
                        {
                            AnimMgr.Flush(FrameMenu);
                            AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            MenuKind = "";
                        }
                        else if (MenuKind == "Settings")
                        {
                            AnimMgr.Flush(FrameMenu);
                            AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            MenuKind = "";
                        }
                        ResumeMenu();
                    }
                }
                else
                {
                    if (!FrameQuicktip.Visible)
                    {
                        foreach (var quicktipControl in FrameQuicktip.Controls)
                        {
                            if (quicktipControl is CMlQuad quad)
                            {
                                quad.Opacity = 0;
                            }

                            if (quicktipControl is CMlLabel label)
                            {
                                label.Opacity = 0;
                            }
                            AnimMgr.Flush(quicktipControl);
                            AnimMgr.Add(quicktipControl, "<control opacity=\"1\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                        }
                    }

                    AnimMgr.Flush(FrameQuicktip);
                    FrameQuicktip.Show();
                    AnimMgr.Add(FrameQuicktip, "<frame hidden=\"1\"/>", Now + 2000, 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                UpdateVehicles();
                UpdateSkins();
                break;
            case "QuadSkin":
                var SOffset = MathLib.NearestInteger((float)FrameSkinList.Parent.ScrollOffset.Y / 15f);
                var Index = control.Parent.Parent.Controls.IndexOf(control.Parent) + SOffset - 1;
                var CName = DisplayedCars[VehicleIndex];
                var PreviousSkin = "";
                if (UserSkins.ContainsKey(CName))
                {
                    PreviousSkin = UserSkins[CName];
                }
                var SkinSelectionValid = false;

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

                if (Index == -1)
                {
                    var userSkins = UserSkins;
                    userSkins[CName] = "";
                    UserSkins = userSkins;

                    SkinSelectionValid = true;

                    if (IsTM2CarOnStadium(CName))
                    {
                        var persistent_EnvimixStadiumSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                        persistent_EnvimixStadiumSkins.Get()[CName] = "";
                    }
                    else
                    {
                        var persistent_EnvimixSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                        persistent_EnvimixSkins.Get()[CName] = "";
                    }
                }
                else
                {
                    ImmutableArray<string> SNames = new();

                    if (Skins.ContainsKey(CName))
                    {
                        foreach (var (name, skin) in Skins[CName])
                        {
                            SNames.Add(name);
                        }

                        SNames = SNames.Sort();
                    }

                    if (Index < SNames.Length)
                    {
                        var userSkins = UserSkins;
                        userSkins[CName] = SNames[Index];
                        UserSkins = userSkins;

                        SkinSelectionValid = true;

                        if (IsTM2CarOnStadium(CName))
                        {
                            var persistent_EnvimixStadiumSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                            persistent_EnvimixStadiumSkins.Get()[CName] = SNames[Index];
                        }
                        else
                        {
                            var persistent_EnvimixSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                            persistent_EnvimixSkins.Get()[CName] = SNames[Index];
                        }
                    }
                }
                UpdateSkins();
                SendCustomEvent("Skin", new[] { CName, UserSkins[CName] });

                // Clicking the already-selected skin again plays with it, like the Play button
                if (SkinSelectionValid && UserSkins[CName] == PreviousSkin)
                {
                    QUAD_BUTTON_SKIN_PLAY();
                }
                break;
        }

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

    private void Focus2()
    {
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
    }

    private void Focus3()
    {
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 3, 1);
    }

    private void Menu_MouseOver(CMlControl control, string controlId)
    {
        if (controlId == "QuadSkin")
        {
            Focus3();
            return;
        }

        if (control.DataAttributeGet("nav") != "True")
        {
            return;
        }

        NavFocusedControl.StyleSelected = false;

        if (control is CMlQuad quad)
        {
            NavFocusedControl = quad;
        }
    }

    private void Menu_MouseOut(CMlControl control, string controlId)
    {
        FrameTooltip.Hide();
    }

	private void SetSlidingText(CMlFrame frame, string value)
	{
		var l1 = (frame.Controls[0] as CMlLabel)!;
		l1.Value = value;
		l1.Size.X = l1.ComputeWidth(value);

		var l2 = (frame.Controls[1] as CMlLabel)!;
		l2.Value = value;
		l2.Size.X = l2.ComputeWidth(value);
	}

	private void MoveSlidingText(CMlFrame frame, int distance, float speed) {
		var l1 = (frame.Controls[0] as CMlLabel)!;
		var l2 = (frame.Controls[1] as CMlLabel)!;

        if (frame.ClipWindowSize.X >= l1.Size.X)
        {
            l2.Hide();
            l1.RelativePosition_V3.X = 0;
			return;
        }

        l1.RelativePosition_V3.X -= Period * speed;
        l2.RelativePosition_V3.X -= Period * speed;
        l2.Show();

        if (speed > 0)
        {
			if (l1.RelativePosition_V3.X + l1.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X > l2.RelativePosition_V3.X)
			{
				l1.RelativePosition_V3.X = l2.RelativePosition_V3.X + l2.Size.X + distance;
			}
			if (l2.RelativePosition_V3.X + l2.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X < l2.RelativePosition_V3.X)
			{
				l2.RelativePosition_V3.X = l1.RelativePosition_V3.X + l1.Size.X + distance;
			}
        }
        else if (speed < 0)
        {
			if (l1.RelativePosition_V3.X - l1.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X < l2.RelativePosition_V3.X)
			{
				l1.RelativePosition_V3.X = l2.RelativePosition_V3.X - l2.Size.X - distance;
			}
			if (l2.RelativePosition_V3.X - l2.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X > l2.RelativePosition_V3.X)
			{
				l2.RelativePosition_V3.X = l1.RelativePosition_V3.X - l1.Size.X - distance;
			}
        }
    }

	private void QUAD_BUTTON_CONTINUE()
    {
        ResumeMenu();
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

	private void QUAD_BUTTON_SPECTATOR()
	{
		var parent = QuadButtonSpectator.Parent;

		if (parent.DataAttributeGet("checked") == "True")
		{
			RequestSpectatorClient(false);
			parent.DataAttributeSet("startanimate", "-1");
			(parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");
			parent.DataAttributeSet("checked", "False");
		}
		else
		{
			RequestSpectatorClient(true);
			parent.DataAttributeSet("startanimate", Now.ToString());
			(parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");
			parent.DataAttributeSet("checked", "True");
		}

		Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
	}

    private void QUAD_BUTTON_MANAGESERVER()
	{
        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.ServerSettings);
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    private void ShowCloseServerMessageBox()
    {
        FrameMessageBoxConfirm.Show();
        LabelMessageBoxName.Value = TextLib.GetTranslatedText("Close server?");
        LabelMessageBoxDescription.Value = TextLib.GetTranslatedText("Are you sure you want to close the server?");
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        AnimMgr.Add(FrameMessageBox, "<frame pos=\"0 0\" hidden=\"0\"/>", 800, CAnimManager.EAnimManagerEasing.QuadOut);
        IsMessageBoxOpen = true;
        MessageBoxReturnControl = QuadButtonExit;
        NavFocusedControl.StyleSelected = false;
        NavFocusedControl = QuadButtonMessageBoxClose;
        NavFocusedControl.StyleSelected = true;
    }

    private void QUAD_BUTTON_EXIT()
	{
        if (IsExplore())
        {
            ShowInGameMenu();
        }
        else if (CurrentServerLogin == LocalUser.Login)
        {
            ShowCloseServerMessageBox();
        }
        else
        {
            CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Quit);
        }
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

	private void QUAD_BUTTON_ADVANCED()
	{
        AnimMgr.Add(FrameMenu, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "Settings";
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    private void QUAD_BUTTON_ADVANCEDSETTINGS()
	{
        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.AdvancedMenu);
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

	private void QUAD_BUTTON_SKIN()
	{
		UpdateSkins();
        AnimMgr.Add(FrameMenu, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameSkins, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "Skin";
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

	private void QUAD_BUTTON_SKIN_BACK()
	{
        AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "";
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    private void QUAD_BUTTON_SETTINGS_BACK()
	{
        AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "";
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
    }

    private bool IsCarLocked()
	{
		return (GetPlayer().RaceStartTime > 0 && GameTime - GetPlayer().RaceStartTime >= 0)
			|| UI.UISequence == CUIConfig.EUISequence.Intro || IsSpectator;
        //return (InputPlayer.RaceStartTime > 0 && GameTime - InputPlayer.RaceStartTime >= 0)
        //	|| (Net_CutOffTimeLimit == -1 && InputPlayer.RaceStartTime == 0);  - issues with disabled default car and no time limit
    }

    private void ShowCustomModeHelp()
    {
        FrameMessageBoxConfirm.Hide();
        LabelMessageBoxName.Value = Playground.ServerInfo.ModeName;
        LabelMessageBoxDescription.Value = ModeHelp;
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        AnimMgr.Add(FrameMessageBox, "<frame pos=\"0 0\" hidden=\"0\"/>", 800, CAnimManager.EAnimManagerEasing.QuadOut);
        IsMessageBoxOpen = true;
        MessageBoxReturnControl = QuadButtonModeHelp;
        NavFocusedControl = QuadButtonMessageBoxClose;
    }

    private bool HandleMessageBoxNavigation(CMlScriptEvent.EMenuNavAction action)
    {
        if (!IsMessageBoxOpen)
        {
            return false;
        }

        if (action == CMlScriptEvent.EMenuNavAction.Select)
        {
            if (NavFocusedControl == QuadButtonMessageBoxConfirm)
            {
                CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Quit);
            }
            else
            {
                QuadButtonMessageBoxClose_MouseClick();
            }

            return true;
        }

        if (action == CMlScriptEvent.EMenuNavAction.Cancel)
        {
            QuadButtonMessageBoxClose_MouseClick();
            return true;
        }

        if (FrameMessageBoxConfirm.Visible && (action == CMlScriptEvent.EMenuNavAction.Up || action == CMlScriptEvent.EMenuNavAction.Down || action == CMlScriptEvent.EMenuNavAction.Left || action == CMlScriptEvent.EMenuNavAction.Right))
        {
            NavFocusedControl.StyleSelected = false;

            if (NavFocusedControl == QuadButtonMessageBoxClose)
            {
                NavFocusedControl = QuadButtonMessageBoxConfirm;
            }
            else
            {
                NavFocusedControl = QuadButtonMessageBoxClose;
            }

            NavFocusedControl.StyleSelected = true;
            Focus2();
        }

        return true;
    }

    private void Menu_MenuNavigation(CMlScriptEvent.EMenuNavAction action)
    {
        if (HandleMessageBoxNavigation(action))
        {
            return;
        }

        switch (action)
        {
            case CMlScriptEvent.EMenuNavAction.Cancel:
                if (UI.UISequence == CUIConfig.EUISequence.Intro)
                {
                    if (ShowMenuLittleLater == -1)
                        ResumeMenu();
                    ShowMenuLittleLater = Now;
                }
                else if (MenuKind == "Skin")
                {
                    AnimMgr.Flush(FrameMenu);
                    AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    MenuKind = "";
                }
                else if (MenuKind == "Settings")
                {
                    AnimMgr.Flush(FrameMenu);
                    AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    MenuKind = "";
                }
                else
                    ResumeMenu();
                break;
            case CMlScriptEvent.EMenuNavAction.Select:
                if (NavOnVehicle)
                {
                    if (VehicleIndex < DisplayedCars.Length)
                    {
                        if (IsSpectator)
                        {
                            // suggest the player to play that car or something lol
                        }
                        else
                        {
                            if (InputPlayer.RaceStartTime - GameTime < 0)
                            {
                                SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True" });
                            }
                            else
                            {
                                SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "False" });
                            }
                        }

                        if (MenuKind == "Skin")
                        {
                            AnimMgr.Flush(FrameMenu);
                            AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            MenuKind = "";
                        }
                        else if (MenuKind == "Settings")
                        {
                            AnimMgr.Flush(FrameMenu);
                            AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                            MenuKind = "";
                        }

                        ResumeMenu();
                    }
                }
                else
                {
                    if (NavFocusedControl == QuadButtonContinue)
                    {
                        QUAD_BUTTON_CONTINUE();
                    }
                    else if (NavFocusedControl == QuadButtonExit)
                    {
                        QUAD_BUTTON_EXIT();
                    }
                    else if (NavFocusedControl == QuadButtonManageServer)
                    {
                        QUAD_BUTTON_MANAGESERVER();
                    }
                    else if (NavFocusedControl == QuadButtonModeHelp)
                    {
                        NavFocusedControl.StyleSelected = false;
                        ShowCustomModeHelp();
                        NavFocusedControl.StyleSelected = true;
                    }
                    else if (NavFocusedControl == QuadButtonServerDetails && EnvimaniaSessionId != "")
                    {
                        OpenLink($"https://envimix.gbx.tools/envimania/servers/{CurrentServerLogin}", CMlScript.LinkType.ExternalBrowser);
                    }
                    else if (NavFocusedControl == QuadButtonSessionDetails && EnvimaniaSessionId != "")
                    {
                        OpenLink($"https://envimix.gbx.tools/envimania/sessions/{EnvimaniaSessionId}", CMlScript.LinkType.ExternalBrowser);
                    }
                    else if (NavFocusedControl == QuadButtonAdvanced)
                    {
                        QUAD_BUTTON_ADVANCED();
                    }
                    else if (NavFocusedControl == QuadButtonSkin)
                    {
                        QUAD_BUTTON_SKIN();
                    }
                    else if (NavFocusedControl == QuadButtonSpectator)
                    {
                        QUAD_BUTTON_SPECTATOR();
                    }
                    else if (NavFocusedControl == QuadButtonMessageBoxClose)
                    {
                        QuadButtonMessageBoxClose_MouseClick();
                    }
                }
                break;
            case CMlScriptEvent.EMenuNavAction.Up:
                if (NavOnVehicle)
                {
                    FrameVehicles.Scroll(new Vec2(0, -1f));
                }
                else
                {
                    if (NavFocusedControl.StyleSelected)
                    {
                        NavFocusedControl.StyleSelected = false;

                        if (NavFocusedControl == QuadButtonContinue)
                        {
                            NavFocusedControl = QuadButtonExit;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonExit)
                        {
                            if (EnvimaniaSessionId == "")
                            {
                                NavFocusedControl = QuadButtonModeHelp;
                            }
                            else
                            {
                                NavFocusedControl = QuadButtonSessionDetails;
                            }

                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonSessionDetails)
                        {
                            NavFocusedControl = QuadButtonServerDetails;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonServerDetails)
                        {
                            NavFocusedControl = QuadButtonModeHelp;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonManageServer)
                        {
                            NavFocusedControl = QuadButtonAdvanced;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonModeHelp)
                        {
                            NavFocusedControl = QuadButtonManageServer;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonAdvanced)
                        {
                            NavFocusedControl = QuadButtonSkin;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonSkin)
                        {
                            NavFocusedControl = QuadButtonSpectator;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonSpectator)
                        {
                            NavFocusedControl = QuadButtonContinue;
                            Focus2();
                        }
                    }

                    NavFocusedControl.StyleSelected = true;
                }
                break;
            case CMlScriptEvent.EMenuNavAction.Down:
                if (NavOnVehicle)
                {
                    FrameVehicles.Scroll(new Vec2(0, 1));
                }
                else
                {
                    if (NavFocusedControl.StyleSelected)
                    {
                        NavFocusedControl.StyleSelected = false;

                        if (NavFocusedControl == QuadButtonContinue)
                        {
                            NavFocusedControl = QuadButtonSpectator;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonExit)
                        {
                            NavFocusedControl = QuadButtonContinue;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonManageServer)
                        {
                            NavFocusedControl = QuadButtonModeHelp;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonModeHelp)
                        {
                            if (EnvimaniaSessionId == "")
                            {
                                NavFocusedControl = QuadButtonExit;
                            }
                            else
                            {
                                NavFocusedControl = QuadButtonServerDetails;
                            }

                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonServerDetails)
                        {
                            NavFocusedControl = QuadButtonSessionDetails;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonSessionDetails)
                        {
                            NavFocusedControl = QuadButtonExit;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonAdvanced)
                        {
                            NavFocusedControl = QuadButtonManageServer;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonSkin)
                        {
                            NavFocusedControl = QuadButtonAdvanced;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonSpectator)
                        {
                            NavFocusedControl = QuadButtonSkin;
                            Focus2();
                        }
                    }

                    NavFocusedControl.StyleSelected = true;
                }
                break;
            case CMlScriptEvent.EMenuNavAction.Left:
                NavFocusedControl.StyleSelected = false;
                NavOnVehicle = !NavOnVehicle;
                if (!NavOnVehicle)
                {
                    NavFirstControl.StyleSelected = true;
                    NavFocusedControl = NavFirstControl;
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
                }
                break;
            case CMlScriptEvent.EMenuNavAction.Right:
                NavFocusedControl.StyleSelected = false;
                NavOnVehicle = !NavOnVehicle;
                if (!NavOnVehicle)
                {
                    NavFirstControl.StyleSelected = true;
                    NavFocusedControl = NavFirstControl;
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
                }
                break;
        }
    }

    private SEnvimaniaRecordsFilter GetFilter()
    {
        var car = Netread<string>.For(GetPlayer());
        var gravity = Netread<int>.For(GetPlayer());

        SEnvimaniaRecordsFilter filter = new()
        {
            Car = car.Get(),
            Gravity = gravity.Get(),
            Laps = GetLaps(),
            Type = "Time" // TODO: Add support for other types
        };

        return filter;
    }

    public static string ConstructFilterKey(SEnvimaniaRecordsFilter filter)
    {
        return $"{filter.Car}_{filter.Gravity}_{filter.Laps}_{filter.Type}";
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

    public void Main()
    {
        Page.GetClassChildren("LOADING", Page.MainFrame, Recursive: true);

        (FrameButtonSpectator.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");

        EnableMenuNavigationInputs = true;

        NavFirstControl = QuadButtonContinue;
        NavFocusedControl = NavFirstControl;
        UpdateEnvimaniaSessionButtons();

        if (IsExplore())
        {
            var exploreMapName = Metadata<string>.For(Map);
            MapNameInExplore = exploreMapName.Get();
        }
        else
        {
            while (!IsInGameMenuDisplayed)
            {
                Yield();
                ShowInGameMenu();
            }
        }

        PreviousEnableDefaultCar = EnableDefaultCar || OverrideEnableDefaultCar;

        FrameButtonSpectator.DataAttributeSet("checked", IsSpectatorClient.ToString());

        if (IsSpectatorClient)
        {
            FrameButtonSpectator.DataAttributeSet("startanimate", Now.ToString());
            (FrameButtonSpectator.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");
        }
        else
        {
            FrameButtonSpectator.DataAttributeSet("startanimate", "-1");
            (FrameButtonSpectator.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");
        }

        if (LoadedTitle.TitleId != "Envimix_Turbo@bigbang1112")
        {
            QuadButtonEnableVoiceOnImpact.Parent.Hide();
            QuadButtonEnableVoiceOnWaypoint.Parent.Hide();
        }

        var persistent_EnvimixVoiceOnImpact = Persistent<float>.For(LocalUser);
        var persistent_EnvimixVoiceOnWaypoint = Persistent<float>.For(LocalUser);

        var persistent_EnvimixVoiceSettingsIntialized = Persistent<bool>.For(LocalUser);
        if (!persistent_EnvimixVoiceSettingsIntialized.Get())
        {
            persistent_EnvimixVoiceOnImpact.Set(0.1f);
            persistent_EnvimixVoiceOnWaypoint.Set(0.15f);
        }
        persistent_EnvimixVoiceSettingsIntialized.Set(true);

        if (persistent_EnvimixVoiceOnImpact.Get() < 0.01f)
        {
            (QuadButtonEnableVoiceOnImpact.Parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on impact";
        }
        else
        {
            (QuadButtonEnableVoiceOnImpact.Parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on impact";
        }

        if (persistent_EnvimixVoiceOnWaypoint.Get() < 0.01f)
        {
            (QuadButtonEnableVoiceOnWaypoint.Parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on waypoint";
        }
        else
        {
            (QuadButtonEnableVoiceOnWaypoint.Parent.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $tEnable voice on waypoint";
        }

        UserShift = 0;

        Wait(() => GetPlayer() is not null);
        Wait(() => MapPlayerModelName != "");

        var mapVehicleIndex = 0;

        if (DisplayedCars.Length > 0)
        {
            FrameVehicles.ScrollActive = true;
            FrameVehicles.ScrollMax = new Vec2(0, (DisplayedCars.Length - mapVehicleIndex - 1) * 20f);
            FrameVehicles.ScrollMin = new Vec2(0, -mapVehicleIndex * 20f);
            FrameVehicles.ScrollGridSnap = true;
            FrameVehicles.ScrollGrid = new Vec2(0, 20);
        }

        var persistent_EnvimixSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
        var persistent_EnvimixStadiumSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
        foreach (var car in DisplayedCars)
        {
            var userSkins = UserSkins;

            if (IsTM2CarOnStadium(car))
            {
                if (persistent_EnvimixStadiumSkins.Get().ContainsKey(car))
                {
                    userSkins[car] = persistent_EnvimixStadiumSkins.Get()[car];
                }
                else
                {
                    userSkins[car] = "";
                }
            }
            else if (persistent_EnvimixSkins.Get().ContainsKey(car))
            {
                userSkins[car] = persistent_EnvimixSkins.Get()[car];
            }
            else
            {
                userSkins[car] = "";
            }

            UserSkins = userSkins;
        }

        FrameMenu.RelativePosition_V3.X = -110;
        FrameVehicleList.RelativePosition_V3.X = 110;

        if (ItemCars.ContainsValue(MapPlayerModelName))
        {
            ClientCar = ItemCars.KeyOf(MapPlayerModelName);
        }
        else
        {
            ClientCar = "";
        }

        if (DisplayedCars.Contains(ClientCar))
        {
            VehicleIndex = DisplayedCars.IndexOf(ClientCar);
        }
        else
        {
            VehicleIndex = 0;
        }

        while (VehicleIndex > 0 && FrameVehicles.ScrollOffset.Y != VehicleIndex * 20)
        {
            Yield();
            FrameVehicles.ScrollOffset.Y = VehicleIndex * 20f;
        }

        PreviousScrollOffset = (float)FrameVehicles.ScrollOffset.Y;
        PreviousSkinScrollOffset = FrameSkinList.Parent.ScrollOffset;
        PreviousUISequence = UI.UISequence;
		
	    UpdateVehicles();

        Zones = TextLib.Split("|", LocalUser.ZonePath);

        for (var i = 0; i < FrameInnerVehicles.Controls.Count; i++)
        {
            var frame = (FrameInnerVehicles.Controls[i] as CMlFrame)!;
            var quad = (frame.GetFirstChild("QuadFlash") as CMlQuad)!;

            AnimMgr.Add(quad, "<quad opacity=\"0.1\"/>", Now + 1000 + 300 * i, 300, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.AddChain(quad, "<quad opacity=\"0\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    public void Loop()
    {
        if (ShowMenuLittleLater != -1 && Now - ShowMenuLittleLater > 100 && Now - ShowMenuLittleLater < 300)
        {
            ShowInGameMenu();
        }

        var car = Netread<string>.For(GetPlayer());

        if (EnvimaniaSessionId != PreviousEnvimaniaSessionId)
        {
            UpdateEnvimaniaSessionButtons();
        }

        if (IsMenuOpen != IsMenuNavigationForeground)
        {
            IsMenuOpen = IsMenuNavigationForeground;

            SendCustomEvent("MenuOpen", new[] { IsMenuOpen.ToString() });

            if (IsMenuOpen)
            {
                if (NavOnVehicle)
                {

                }
                else
                {
                    NavFirstControl.StyleSelected = true;
                    NavFocusedControl = NavFirstControl;
                }

                //ShowMenuLittleLater = -1;

                if (MenuKind == "")
                {
                    FrameMenu.RelativePosition_V3.X = -110;
                    AnimMgr.Add(FrameMenu, "<frame pos=\"0 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    FrameAdvancedSettings.RelativePosition_V3.X = -110;
                }
                else if (MenuKind == "Settings")
                {
                    FrameAdvancedSettings.RelativePosition_V3.X = -110;
                    AnimMgr.Add(FrameAdvancedSettings, "<frame pos=\"0 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    FrameMenu.RelativePosition_V3.X = -110;
                }

                FrameVehicleList.RelativePosition_V3.X = 110;
                AnimMgr.Add(FrameVehicleList, "<frame pos=\"0 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                MenuOpenTime = Now;
            }
            else
            {
                NavFocusedControl.StyleSelected = false;

                // Multiplayer specific spawning
                if (CutOffTimeLimit != -1 && InputPlayer.RaceStartTime > CutOffTimeLimit && !IsSpectator)
                {
                    SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True", "False", "True" });
                }

                AnimMgr.Flush(FrameMessageBox);
                FrameMessageBox.Visible = false;
                FrameMessageBox.RelativePosition_V3 = new Vec2(0, -130f);
            }
        }

        if (NavOnVehicle)
        {
            LabelArrow.TextColor = new Vec3(0.0, 0.2, 0.4);
        }
        else
        {
            LabelArrow.TextColor = new Vec3(1, 1, 1);
        }

        if ((EnableDefaultCar || OverrideEnableDefaultCar) != PreviousEnableDefaultCar)
        {
            UpdateVehicles();
            PreviousEnableDefaultCar = EnableDefaultCar || OverrideEnableDefaultCar;
        }

        /*if(UI.UISequence != PreviousUISequence) {
            if(PreviousUISequence == CUIConfig::EUISequence::Intro
            && UI.UISequence == CUIConfig::EUISequence::Playing) {
                SendCustomEvent("Car", [DisplayedCars[VehicleIndex], "True"]);
            }
            PreviousUISequence = UI.UISequence;
        }*/

        /*if(ShowMenuLittleLater != -1 && Now - ShowMenuLittleLater > 150) {
            ShowInGameMenu();
            ShowMenuLittleLater = -1;
        }*/

        if (Map.MapInfo.MapUid != PreviousMapUid)
        {
            string mapName;
            if (IsExplore())
            {
                mapName = MapNameInExplore;
            }
            else
            {
                mapName = Map.MapInfo.Name;
            }

            SetSlidingText(FrameLabelMapName, mapName);
            if (Map.MapType == "Envimix" || Map.MapType == "TrackMania\\Envimix")
            {
                SetSlidingText(FrameLabelMapType, "$ff0ENVIMIX MAP");
            }
            else if (Map.MapType == "EnvimixExplore" || Map.MapType == "TrackMania\\EnvimixExplore")
            {
                SetSlidingText(FrameLabelMapType, "$4afEXPLORE MODE");
            }
            else
            {
                SetSlidingText(FrameLabelMapType, "$aaaRACE MAP");
            }
            PreviousMapUid = Map.MapInfo.MapUid;
        }

        MoveSlidingText(FrameLabelMapName, 10, 0.01f);

        if (Map.MapInfo.AuthorNickName != PreviousMapAuthor)
        {
            LabelMapAuthor.SetText(Map.MapInfo.AuthorNickName);
            PreviousMapAuthor = Map.MapInfo.AuthorNickName;
        }

        if (car.Get() != PreviousCar)
        {
            SetSlidingText(FrameLabelCar, car.Get());

            PreviousCar = car.Get();
        }

        if (FrameButtonSpectator.DataAttributeGet("checked") == "True")
        {
            var startTime = TextLib.ToInteger(FrameButtonSpectator.DataAttributeGet("startanimate"));
            FrameButtonSpectator.GetFirstChild("LABEL").RelativeScale = (MathLib.Sin((Now - startTime) / 1000f * MathLib.PI() * 2 - MathLib.PI() / 2) + 1) / 2 * .1f + 1;
        }
        else
        {
            FrameButtonSpectator.GetFirstChild("LABEL").RelativeScale = 1;
        }

        LabelPbNickname.Value = GetPlayer().User.Name;
        if (GetPlayer().Score is null || GetPlayer().Score.BestRace.Time < 0)
        {
            LabelPbTime.Value = "-.--.---";
        }
        else
        {
            LabelPbTime.Value = TimeToTextWithMilli(GetPlayer().Score.BestRace.Time);
        }

        LabelServerName.Value = Playground.ServerInfo.ServerName;
        LabelMode.Value = Playground.ServerInfo.ModeName;

        if (CutOffTimeLimit > 0)
        {
            LabelTimeLimit.Show();

            if (CutOffTimeLimit <= GameTime)
            {
                LabelTimeLimit.Value = "0:00";
            }
            else
            {
                LabelTimeLimit.Value = TextLib.TimeToText(CutOffTimeLimit - GameTime);
            }
        }
        else
        {
            LabelTimeLimit.Hide();
        }

        if (Playground.ServerInfo.IsPrivate)
        {
            LabelPlayerCount.Value = $"{Playground.ServerInfo.PlayerCount}/{Playground.ServerInfo.MaxPlayerCount}$ff0🔒";
        }
        else
        {
            LabelPlayerCount.Value = $"{Playground.ServerInfo.PlayerCount}/{Playground.ServerInfo.MaxPlayerCount}";
        }

        if (Playground.ServerInfo.IsPrivateForSpectator)
        {
            LabelSpectatorCount.Value = $"{Playground.ServerInfo.SpectatorCount}/{Playground.ServerInfo.MaxSpectatorCount}$ff0🔒";
        }
        else
        {
            LabelSpectatorCount.Value = $"{Playground.ServerInfo.SpectatorCount}/{Playground.ServerInfo.MaxSpectatorCount}";
        }

        if (Players.Count > 5)
        {
            FramePlayers.RelativePosition_V3.Y += 0.01f;

            if (FramePlayers.RelativePosition_V3.Y > 3.5)
            {
                FramePlayers.RelativePosition_V3.Y = 0;
                UserShift += 1;
                //UserShift = UserShift % Players.count;
            }
        }
        else
        {
            UserShift = 0;
            FramePlayers.RelativePosition_V3.Y = 0;
        }

        for (var i = 0; i < FramePlayers.Controls.Count; i++)
        {
            var label = (FramePlayers.Controls[i] as CMlLabel)!;

            if (Players.Count > i + UserShift)
            {
                label.Value = Players[i + UserShift].User.Name;
                label.Show();
            }
            else if (Players.Count > 5)
            {
                label.Value = Players[(i + UserShift) % 5].User.Name;
                label.Show();
            }
            else
            {
                label.Hide();
            }
        }

        var vehiclesScrollOffsetY = MathLib.NearestInteger((float)FrameVehicles.ScrollOffset.Y / 20) * 20f;

        if (vehiclesScrollOffsetY != PreviousScrollOffset)
        {
            var difference = (float)(vehiclesScrollOffsetY - PreviousScrollOffset);
            var indexChange = MathLib.NearestInteger((float)difference / 20);
            VehicleIndex += indexChange;
            PreviousScrollOffset = vehiclesScrollOffsetY;
        }

        if (HoldSkinsScrollbar && MouseLeftButton)
        {
            var newY = MathLib.Clamp(MouseY - HoldSkinsScrollbarPos, (float)QuadSkinScrollbar.Size.Y - 105, 0);

            var targetScrollOffset = newY / ((float)QuadSkinScrollbar.Size.Y - 105f) * (float)FrameOuterSkinList.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger((float)targetScrollOffset / 15);
            var steppedScrollOffset = stepIndex * 15f;

            steppedScrollOffset = MathLib.Clamp(steppedScrollOffset, 0f, (float)FrameOuterSkinList.ScrollMax.Y);

            if (FrameOuterSkinList.ScrollMax.Y > 0)
            {
                QuadSkinScrollbar.RelativePosition_V3.Y = -(steppedScrollOffset / FrameOuterSkinList.ScrollMax.Y) * (105f - QuadSkinScrollbar.Size.Y);
            }

            FrameOuterSkinList.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldSkinsScrollbar)
        {
            if (ScrollbarSkinsMouseOut)
            {
                AnimMgr.Add(QuadSkinScrollbar, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
                ScrollbarSkinsMouseOut = false;
            }
            HoldSkinsScrollbar = false;
        }

        if (Skins.ContainsKey(DisplayedCars[VehicleIndex]))
        {
            var skinsForCar = Skins[DisplayedCars[VehicleIndex]];

            if (skinsForCar.Count > 7)
            {
                FrameSkinList.Parent.ScrollMax.Y = (skinsForCar.Count + 1) * 15f - (7 * 15f);
                QuadSkinScrollbar.Size.Y = 7f / skinsForCar.Count * QuadSkinScrollable.Size.Y;
                QuadSkinScrollbar.RelativePosition_V3.Y = -FrameSkinList.Parent.ScrollOffset.Y / FrameSkinList.Parent.ScrollMax.Y * (QuadSkinScrollable.Size.Y - QuadSkinScrollbar.Size.Y);
                QuadSkinScrollbar.Visible = true;
            }
            else
            {
                FrameSkinList.Parent.ScrollMax.Y = 0;
                QuadSkinScrollbar.Visible = false;
            }
        }
        else
        {
            FrameSkinList.Parent.ScrollMax.Y = 0;
            QuadSkinScrollbar.Visible = false;
        }

        if (FrameSkinList.Parent.ScrollOffset != PreviousSkinScrollOffset)
        {
            UpdateSkins();
            FrameSkinList.RelativePosition_V3.Y = -FrameSkinList.Parent.ScrollOffset.Y;
            PreviousSkinScrollOffset = FrameSkinList.Parent.ScrollOffset;
        }

        if (ClientCar != PrevClientCar)
        {
            if (DisplayedCars.Contains(ClientCar))
            {
                VehicleIndex = DisplayedCars.IndexOf(ClientCar);
                FrameVehicles.ScrollOffset.Y = VehicleIndex * 20f;
                PreviousScrollOffset = (float)FrameVehicles.ScrollOffset.Y;
            }
            PrevClientCar = ClientCar;
        }

        if (VehicleIndex != PreviousVehicleIndex)
        {
            ClientCar = DisplayedCars[VehicleIndex];
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            UpdateVehicles();
            UpdateSkins();
            PreviousVehicleIndex = VehicleIndex;
            if ((InputPlayer.RaceStartTime == 0 || GameTime - InputPlayer.RaceStartTime < 0) && DisplayedCars.Length > VehicleIndex && !IsSpectator)
            {
                SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True", "True" });
            }
        }

        QuadBackground.Visible = IsCarLocked();
        LabelLock.Visible = IsCarLocked();

        if (IsCarLocked() && DisplayedCars.Contains(car.Get()))
        {
            FrameArrow.RelativePosition_V3.Y = FrameVehicles.ScrollAnimOffset.Y + DisplayedCars.IndexOf(car.Get()) * -20f;
        }
        else
        {
            FrameArrow.RelativePosition_V3.Y = FrameVehicles.ScrollAnimOffset.Y - vehiclesScrollOffsetY;
        }

        FrameGhostArrow.RelativePosition_V3.Y = FrameVehicles.ScrollAnimOffset.Y - vehiclesScrollOffsetY;

        /*if (QuadEnvimix.DownloadInProgress)
        {
            QuadEnvimixLoading.Show();
        }
        else
        {
            QuadEnvimixLoading.Hide();
        }*/

        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }

        PreviousMenuKind = MenuKind;

        var gravity = Netread<int>.For(GetPlayer());

        if (FrameTooltip.Visible)
        {
            FrameTooltip.RelativePosition_V3 = new Vec2(MouseX, MouseY);
        }

        if (RatingEnabled != PrevRatingEnabled)
        {
            foreach (var control in FrameInnerVehicles.Controls)
            {
                var frame = (control as CMlFrame)!;
                var gaugeDifficulty = (frame.GetFirstChild("GaugeDifficulty") as CMlGauge)!;
                var gaugeQuality = (frame.GetFirstChild("GaugeQuality") as CMlGauge)!;

                if (RatingEnabled)
                {
                    gaugeDifficulty.Size = new Vec2(11, 6.5f);
                    gaugeQuality.Size = new Vec2(11, 6.5f);
                }
                else
                {
                    gaugeDifficulty.Size.X = 0;
                    gaugeQuality.Size.X = 0;
                }
            }

            PrevRatingEnabled = RatingEnabled;
        }

        var validations = Netread<Dictionary<string, SEnvimaniaRecord>>.For(Teams[0]);
        var validationsUpdatedAt = Netread<int>.For(Teams[0]);

        var ratingsUpdatedAt = Netread<int>.For(Teams[0]);

        if (ratingsUpdatedAt.Get() != PrevRatingsUpdatedAt || validationsUpdatedAt.Get() != PrevValidationsUpdatedAt)
        {
            var ratings = Netread<Dictionary<string, SRating>>.For(Teams[0]);
            var stars = Netread<Dictionary<string, SStar>>.For(Teams[0]);

            // skillpoints are retrieved from the same request as ratings, so this is just weirdness

            foreach (var control in FrameInnerVehicles.Controls)
            {
                var frame = (control as CMlFrame)!;
                var gaugeDifficulty = (frame.GetFirstChild("GaugeDifficulty") as CMlGauge)!;
                var gaugeQuality = (frame.GetFirstChild("GaugeQuality") as CMlGauge)!;

                var carName = frame.DataAttributeGet("car");

                var validationKey = ConstructValidationFilterKey(carName);

                // if validated or is the default car
                if (validations.Get().ContainsKey(validationKey) || IsDefaultCar(carName))
                {
                    gaugeDifficulty.Color = new Vec3(1, 1, 1);
                    gaugeQuality.Color = new Vec3(1, 1, 1);
                }
                else
                {
                    // otherwise use the impossible red color
                    gaugeDifficulty.Color = new Vec3(1, 0, 0);
                    gaugeQuality.Color = new Vec3(1, 0, 0);
                }

                var filterKey = ConstructRatingFilterKey(carName);

                if (!ratings.Get().ContainsKey(filterKey))
                {
                    gaugeDifficulty.Ratio = 0;
                    gaugeQuality.Ratio = 0;
                }
                else
                {
                    var rating = ratings.Get()[filterKey];

                    if (rating.Difficulty < 0)
                    {
                        AnimMgr.Add(gaugeDifficulty, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                    else
                    {
                        AnimMgr.Add(gaugeDifficulty, $"<gauge ratio=\"{rating.Difficulty * .6f + .4f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                    }

                    if (rating.Quality < 0)
                    {
                        AnimMgr.Add(gaugeQuality, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                    else
                    {
                        AnimMgr.Add(gaugeQuality, $"<gauge ratio=\"{rating.Quality * .6f + .4f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                }

                var quadStar = (frame.GetFirstChild("QuadStar") as CMlQuad)!;
                quadStar.Visible = stars.Get().ContainsKey(filterKey);

                var labelSkillpoints = (frame.GetFirstChild("LabelSkillpoints") as CMlLabel);
                var labelActivityPoints = (frame.GetFirstChild("LabelActivityPoints") as CMlLabel);

                if (labelSkillpoints is not null)
                {
                    if (Skillpoints.ContainsKey(validationKey))
                    {
                        var skillpoints = Skillpoints[validationKey];
                        labelSkillpoints.SetText(FormatNumberSpace(skillpoints));
                        labelSkillpoints.Show();
                    }
                    else
                    {
                        labelSkillpoints.Hide();
                    }
                }

                if (labelActivityPoints is not null)
                {
                    if (ActivityPoints.ContainsKey(validationKey))
                    {
                        var activityPoints = ActivityPoints[validationKey];
                        labelActivityPoints.SetText(FormatNumberSpace(activityPoints));
                        labelActivityPoints.Show();
                    }
                    else
                    {
                        labelActivityPoints.Hide();
                    }
                }
            }

            PrevRatingsUpdatedAt = ratingsUpdatedAt.Get();
            PrevValidationsUpdatedAt = validationsUpdatedAt.Get();
        }

        // every second, update total time from persistent or net if different player
        if (IsMenuOpen && (GameTime / 100) != (PrevGameTime / 100))
        {
            if (InputPlayer != GUIPlayer)
            {
                foreach (var control in FrameInnerVehicles.Controls)
                {
                    var frame = (control as CMlFrame)!;
                    var controlTotalTime = frame.GetFirstChild("LabelTotalTime");
                    controlTotalTime.Hide();
                }
            }
            else
            {
                var persistent_EnvimixTotalTime = Persistent<Dictionary<string, int>>.For(Map);
                if (persistent_EnvimixTotalTime.Get().Count > 0)
                {
                    foreach (var control in FrameInnerVehicles.Controls)
                    {
                        var frame = (control as CMlFrame)!;

                        var carName = frame.DataAttributeGet("car");

                        var controlTotalTime = frame.GetFirstChild("LabelTotalTime");

                        if (persistent_EnvimixTotalTime.Get().ContainsKey(carName))
                        {
                            (controlTotalTime as CMlLabel)!.Value = TimeLib.FormatDelta("0", persistent_EnvimixTotalTime.Get()[carName].ToString(), TimeLib.EDurationFormats.Abbreviated);
                            controlTotalTime.Show();
                        }
                        else
                        {
                            controlTotalTime.Hide();
                        }
                    }
                }
            }

            PrevGameTime = GameTime;
        }

        var currentFilterKey = ConstructValidationFilterKey(car.Get());

        if (IsDefaultCar(car.Get()))
        {
            LabelValidator.Value = Map.MapInfo.AuthorNickName;
        }
        else if (validations.Get().ContainsKey(currentFilterKey))
        {
            LabelValidator.Value = validations.Get()[currentFilterKey].User.Nickname;
        }
        else if (validationsUpdatedAt.Get() == 0)
        {
            LabelValidator.Value = "$aaa[unknown]";
        }
        else
        {
            LabelValidator.Value = "$aaanobody";
        }

        var forceQuit = Netread<bool>.For(Playground.Teams[0]);
        if (forceQuit.Get())
        {
            CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Quit);
        }
    }
}
