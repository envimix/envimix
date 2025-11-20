using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

/// <summary>
/// This code went through 5 stages of development from 2019-2025, the code is quite confusing, sorry.
/// </summary>
public class Menu : CTmMlScriptIngame, IContext
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
	[ManialinkControl] public required CMlFrame FrameSkinList;
    [ManialinkControl] public required CMlQuad QuadButtonSpectator;
    [ManialinkControl] public required CMlFrame FrameMenu;
    [ManialinkControl] public required CMlFrame FrameAdvancedSettings;
    [ManialinkControl] public required CMlFrame FrameSkins;
	[ManialinkControl] public required CMlFrame FrameButtonSpectator;
	[ManialinkControl] public required CMlQuad QuadButtonContinue;
	[ManialinkControl(IgnoreValidation = true)] public required CMlFrame FrameVehicles;
	[ManialinkControl] public required CMlFrame FrameVehicleList;
    //[ManialinkControl] public required CMlFrame FrameTimeLimit;
    [ManialinkControl] public required CMlFrame FrameTeamMessage;
    [ManialinkControl] public required CMlFrame FrameRedPlayerCount;
    [ManialinkControl] public required CMlFrame FrameBluePlayerCount;
    [ManialinkControl] public required CMlFrame FrameJoinTeam;
    [ManialinkControl] public required CMlFrame FrameJoinRed;
    [ManialinkControl] public required CMlFrame FrameJoinBlue;
    [ManialinkControl] public required CMlLabel LabelArrow;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlFrame FrameLabelMapType;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelSkinCar;
    [ManialinkControl] public required CMlLabel LabelPbNickname;
    [ManialinkControl] public required CMlLabel LabelPbTime;
    [ManialinkControl] public required CMlLabel LabelServerName;
    [ManialinkControl] public required CMlLabel LabelMode;
    [ManialinkControl] public required CMlLabel LabelPlayerCount;
    [ManialinkControl] public required CMlLabel LabelSpectatorCount;
    //[ManialinkControl] public required CMlLabel LabelTimeLimit;
    [ManialinkControl] public required CMlFrame FramePlayers;
    [ManialinkControl] public required CMlQuad QuadSkinScrollbar;
    [ManialinkControl] public required CMlQuad QuadSkinScrollable;
    [ManialinkControl] public required CMlQuad QuadBackground;
    [ManialinkControl] public required CMlLabel LabelLock;
    [ManialinkControl] public required CMlFrame FrameArrow;
    [ManialinkControl] public required CMlFrame FrameGhostArrow;
    //[ManialinkControl] public required CMlQuad QuadEnvimix;
    //[ManialinkControl] public required CMlQuad QuadEnvimixLoading;
    [ManialinkControl] public required CMlLabel LabelRedPlayerCount;
    [ManialinkControl] public required CMlLabel LabelBluePlayerCount;
    [ManialinkControl] public required CMlFrame FrameTeamInfo;
    [ManialinkControl] public required CMlQuad QuadJoinBlueLock;
    [ManialinkControl] public required CMlQuad QuadJoinBlue;
    [ManialinkControl] public required CMlQuad QuadJoinRedLock;
    [ManialinkControl] public required CMlQuad QuadJoinRed;
    //[ManialinkControl] public required CMlFrame FrameLabelTeamMessage;
    //[ManialinkControl] public required CMlFrame FrameLabelTimeLimit;
    [ManialinkControl] public required CMlQuad QuadButtonSkin;
    [ManialinkControl] public required CMlQuad QuadButtonAdvanced;
    [ManialinkControl] public required CMlQuad QuadButtonModeHelp;
    [ManialinkControl] public required CMlQuad QuadButtonServerSettings;
    [ManialinkControl] public required CMlQuad QuadButtonManageServer;
    [ManialinkControl] public required CMlQuad QuadButtonExit;
    [ManialinkControl] public required CMlQuad QuadButtonAdvancedSettings;
    [ManialinkControl] public required CMlQuad QuadButtonSkinPlay;
    [ManialinkControl] public required CMlQuad QuadButtonSkinBack;
    [ManialinkControl] public required CMlQuad QuadButtonSettingsBack;
    [ManialinkControl] public required CMlFrame FrameQuicktip;
    [ManialinkControl] public required CMlLabel LabelMapAuthor;
    [ManialinkControl] public required CMlFrame FrameMultiplayer;
    [ManialinkControl] public required CMlFrame FrameSingleplayer;
    [ManialinkControl] public required CMlFrame FrameButtonManageServer;
    [ManialinkControl] public required CMlFrame FrameButtonChooseSkin;
    [ManialinkControl] public required CMlFrame FrameButtonAdvancedOptions;
    [ManialinkControl] public required CMlFrame FrameGravity;
    [ManialinkControl] public required CMlQuad QuadGravityButton;
    [ManialinkControl] public required CMlQuad QuadGravityValue;
    [ManialinkControl] public required CMlLabel LabelGravityValue;
    [ManialinkControl] public required CMlFrame FrameGravitySlider;
    [ManialinkControl] public required CMlFrame FrameGravityForcedValue;
    [ManialinkControl] public required CMlFrame FrameGhosts;
    [ManialinkControl] public required CMlQuad QuadGhostSelectionPrev;
    [ManialinkControl] public required CMlQuad QuadGhostSelectionNext;
    [ManialinkControl] public required CMlLabel LabelGhostSelection;
    [ManialinkControl] public required CMlFrame FrameTooltip;
    [ManialinkControl] public required CMlQuad QuadLoading;
    [ManialinkControl] public required CMlLabel LabelLoadingResult;
    [ManialinkControl] public required CMlQuad QuadGhostRefresh;
    [ManialinkControl] public required CMlFrame FrameModeHelp;
    [ManialinkControl] public required CMlLabel LabelModeHelpName;
    [ManialinkControl] public required CMlLabel LabelModeHelpDescription;
    [ManialinkControl] public required CMlQuad QuadButtonModeHelpClose;
    [ManialinkControl] public required CMlLabel LabelValidator;
    [ManialinkControl] public required CMlQuad QuadStarButton;

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
	public bool PrevUseForcedClans;
    public bool GravityOpen;
    public int PrevGravityValue = 1;
    public IList<CReplayInfo> LocalReplays;
    public CTaskResult_ReplayList? LocalReplaysTask;
    public IList<string> Zones;
    public int CurrentZoneIndex = -1;
    public int PreviousZoneIndex = -1;
    public int PrevLocalGhostMetadataUpdatedAt = -1;
    public required Dictionary<string, bool> SelectedGhosts;
    public int PrevRatingsUpdatedAt;
    public bool PrevRatingEnabled;
    public int PrevValidationsUpdatedAt;
    public string MapNameInExplore = "";
    public bool PrevGhostToUpload;
    public string PrevClientCar;

    [Netwrite(NetFor.UI)] public string ClientCar { get; set; }
    [Netwrite(NetFor.UI)] public Dictionary<string, string> UserSkins { get; set; }
    [Netwrite(NetFor.UI)] public int ClientGravity { get; set; }
    [Netwrite(NetFor.UI)] public IList<string> LocalReplayFiles { get; set; }
    [Netread] public bool EnableDefaultCar { get; set; }
    [Netread] public string MapPlayerModelName { get; set; }
    [Netread] public int CutOffTimeLimit { get; set; }
    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }
    [Netread] public Dictionary<string, string> ItemCars { get; set; }
    [Netread] public Dictionary<string, Dictionary<string, SSkin>> Skins { get; set; }
    [Netread] public string EnvimixWebAPI { get; set; }
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

    public Menu()
    {
        MouseOver += Menu_MouseOver;
        MouseOut += Menu_MouseOut;
        MouseClick += Menu_MouseClick;

        QuadButtonContinue.MouseOver += Focus2;
        QuadButtonSpectator.MouseOver += Focus2;
        QuadButtonSkin.MouseOver += Focus2;
        QuadButtonAdvanced.MouseOver += Focus2;
        QuadButtonModeHelp.MouseOver += Focus3;
        QuadButtonServerSettings.MouseOver += Focus3;
        QuadButtonManageServer.MouseOver += Focus2;
        QuadButtonExit.MouseOver += Focus2;

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

        MenuNavigation += Menu_MenuNavigation;

        QuadGravityButton.MouseClick += QuadGravityButton_MouseClick;
        QuadGravityButton.MouseOver += () =>
        {
            Focus3();
        };
        QuadGravityValue.MouseClick += QuadGravityValue_MouseClick;

        QuadGravityValue.MouseOver += () =>
        {
            AnimMgr.Add(QuadGravityValue, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadGravityValue.MouseOut += () =>
        {
            if (!ChangingGravity)
            {
                AnimMgr.Add(QuadGravityValue, "<quad opacity=\"0.85\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        };

        QuadGhostSelectionPrev.MouseClick += () =>
        {
            if (CurrentZoneIndex > -1)
            {
                CurrentZoneIndex -= 1;
            }
            else
            {
                CurrentZoneIndex = Zones.Count - 1;
            }

            RequestAndUpdateRecords();
        };

        QuadGhostSelectionNext.MouseClick += () =>
        {
            if (CurrentZoneIndex < Zones.Count - 1)
            {
                CurrentZoneIndex += 1;
            }
            else
            {
                CurrentZoneIndex = -1;
            }

            RequestAndUpdateRecords();
        };

        LabelGhostSelection.MouseClick += RefreshRecords;
        QuadGhostRefresh.MouseClick += RefreshRecords;

        QuadButtonModeHelpClose.MouseClick += QuadButtonModeHelpClose_MouseClick;

        QuadButtonModeHelpClose.MouseOver += () =>
        {
            Focus2();
        };

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
    }

    private void QuadButtonModeHelpClose_MouseClick()
    {
        AnimMgr.Add(FrameModeHelp, "<frame pos=\"0 -130\" hidden=\"1\"/>", 800, CAnimManager.EAnimManagerEasing.QuadOut);
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

    bool IsSolo()
    {
        return CurrentServerLogin is "";
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

    private void CloseGravityMenu()
    {
        GravityOpen = false;
        AnimMgr.Add(FrameGravity, "<frame pos=\"5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void QuadGravityButton_MouseClick()
    {
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

        if (GravityOpen)
        {
            CloseGravityMenu();
        }
        else
        {
            GravityOpen = true;
            AnimMgr.Add(FrameGravity, "<frame pos=\"49.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    public bool ChangingGravity = false;
    public float ChangingGravityX;

    private void QuadGravityValue_MouseClick()
    {
        ChangingGravity = true;
        ChangingGravityX = MouseX - (float)FrameGravitySlider.RelativePosition_V3.X;
    }

    private int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
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

    private void UpdateVehicles()
    {
        for (var i = 0; i < FrameInnerVehicles.Controls.Count; i++)
        {
            var frame = (FrameInnerVehicles.Controls[i] as CMlFrame)!;
            var quadVehicle = (frame.GetFirstChild("QuadVehicle") as CMlQuad)!;
            var labelDefault = (frame.GetFirstChild("LabelDefault") as CMlLabel)!;
            var labelVehicle = (frame.GetFirstChild("LabelVehicle") as CMlLabel)!;

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

            if (ItemCars[DisplayedCars[i]] != MapPlayerModelName)
            {
                labelVehicle.Opacity = 1;
                labelDefault.Hide();
                continue;
            }

            if (EnableDefaultCar)
            {
                labelVehicle.Opacity = 1;
                labelDefault.Opacity = 1;
            }
            else
            {
                labelVehicle.Opacity = 0.5f;
                labelDefault.Opacity = 0.5f;
            }

            labelDefault.Show();
        }
    }

    private void UpdateSkins()
    {
        var carName = DisplayedCars[VehicleIndex];

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

    private void Menu_MouseClick(CMlControl control, string controlId)
    {
        switch (controlId)
        {
            case "QuadVehicle":
                PreviousVehicleIndex = VehicleIndex;
                var index = TextLib.ToInteger(control.Parent.DataAttributeGet("id"));
                FrameVehicles.Scroll(new Vec2(0f, (index - PreviousVehicleIndex) * 1f));

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

                if (Index == -1)
                {
                    var userSkins = UserSkins;
                    userSkins[CName] = "";
                    UserSkins = userSkins;

                    var persistent_EnvimixSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                    persistent_EnvimixSkins.Get()[CName] = "";
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

                        var persistent_EnvimixSkins = Persistent<Dictionary<string, string>>.For(LocalUser);
                        persistent_EnvimixSkins.Get()[CName] = SNames[Index];
                    }
                }
                UpdateSkins();
                SendCustomEvent("Skin", new[] { CName, UserSkins[CName] });
                break;
            case "QuadGhost":
                var file = control.Parent.DataAttributeGet("file");
                var gindex = control.Parent.DataAttributeGet("gindex");

                if (file is "")
                {
                    file = control.Parent.DataAttributeGet("url");
                }

                (control as CMlQuad)!.StyleSelected = false;

                if (file is not "")
                {
                    if (SelectedGhosts.ContainsKey(file))
                    {
                        SelectedGhosts.Remove(file);
                        SendCustomEvent("RemoveGhost", new[] { file, gindex });
                    }
                    else
                    {
                        SelectedGhosts[file] = true;
                        SendCustomEvent("AddGhost", new[] { file, gindex });
                    }

                    (control as CMlQuad)!.StyleSelected = SelectedGhosts.ContainsKey(file);
                }
                
                break;
        }

        if (control == QuadJoinRed)
        {
            JoinTeam1();
            SendCustomEvent("JoinTeam", new[] { "1" });
            control.Parent.Parent.RelativeScale = 1.05f;
            AnimMgr.Add(control.Parent.Parent, "<frame scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
        else if (control == QuadJoinBlue)
        {
            JoinTeam2();
            SendCustomEvent("JoinTeam", new[] { "2" });
            control.Parent.Parent.RelativeScale = 1.05f;
            AnimMgr.Add(control.Parent.Parent, "<frame scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
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

    private void Menu_MouseOver(CMlControl control, string controlId)
    {
        if (controlId == "QuadGhost")
        {
            var file = control.Parent.DataAttributeGet("file");

            if (file is "")
            {
                FrameTooltip.Hide();
                return;
            }

            IList<string> folders = TextLib.Split("\\", file);
            var name = folders[folders.Count - 1];

            var car = Netread<string>.For(GetPlayer());
            name = TextLib.Replace(name, car.Get(), $"$ff0{car.Get()}$z");

            UpdateTooltip(name);
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

    private void Focus2()
    {
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
    }

    private void Focus3()
    {
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 3, 1);
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

	private void QUAD_BUTTON_EXIT()
	{
        if (IsExplore())
        {
            ShowInGameMenu();
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
        CloseGravityMenu();
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
        CloseGravityMenu();
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
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
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

        AnimMgr.Add(FrameModeHelp, "<frame pos=\"0 0\" hidden=\"0\"/>", 800, CAnimManager.EAnimManagerEasing.QuadOut);
        NavFocusedControl = QuadButtonModeHelpClose;
    }

    private void Menu_MenuNavigation(CMlScriptEvent.EMenuNavAction action)
    {
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
                    else if (NavFocusedControl == QuadButtonServerSettings)
                    {

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
                    else if (NavFocusedControl == QuadButtonModeHelpClose)
                    {
                        QuadButtonModeHelpClose_MouseClick();
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
                            NavFocusedControl = QuadButtonServerSettings;
                            Focus2();
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
                        else if (NavFocusedControl == QuadButtonServerSettings)
                        {
                            NavFocusedControl = QuadButtonModeHelp;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonAdvanced)
                        {
                            NavFocusedControl = QuadButtonSkin;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonSkin)
                        {
                            if (IsSolo())
                            {
                                NavFocusedControl = QuadButtonContinue;
                            }
                            else
                            {
                                NavFocusedControl = QuadButtonSpectator;
                            }

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
                            if (IsSolo())
                            {
                                NavFocusedControl = QuadButtonSkin;
                            }
                            else
                            {
                                NavFocusedControl = QuadButtonSpectator;
                            }
                            
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
                            NavFocusedControl = QuadButtonServerSettings;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonServerSettings)
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

    private void UpdateLocalGhosts()
    {
        QuadLoading.Hide();

        if (LocalGhostMetadata.Count == 0)
        {
            LabelLoadingResult.SetText("No ghosts found");
        }
        else
        {
            LabelLoadingResult.SetText("");
        }

        for (var i = 0; i < FrameGhosts.Controls.Count; i++)
        {
            var frame = (FrameGhosts.Controls[i] as CMlFrame)!;

            if (i >= LocalGhostMetadata.Count)
            {
                frame.Hide();
                continue;
            }

            var metadata = LocalGhostMetadata[i];

            frame.DataAttributeSet("file", metadata.FileName);
            frame.DataAttributeSet("url", "");
            frame.DataAttributeSet("gindex", metadata.Index.ToString());

            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;
            var labelAutosave = (frame.GetFirstChild("LabelAutosave") as CMlLabel)!;
            var quadGhost = (frame.GetFirstChild("QuadGhost") as CMlQuad)!;

            labelRank.Hide();
            labelNickname.SetText(metadata.Nickname);

            if (metadata.Time == -1)
            {
                labelTime.SetText("-:--:---");
            }
            else
            {
                labelTime.SetText(TimeToTextWithMilli(metadata.Time));
            }

            labelAutosave.Visible = TextLib.StartsWith("autosave", metadata.FileName, false, false);

            quadGhost.StyleSelected = SelectedGhosts.ContainsKey(metadata.FileName);

            frame.Show();
        }
    }

    public required Dictionary<string, CHttpRequest> EnvimaniaRecordsRequests;
    public required Dictionary<string, SEnvimaniaRecordsFilter> EnvimaniaUnfinishedRecordsRequests;
    public required Dictionary<string, Dictionary<string, SEnvimaniaRecordsResponse>> EnvimaniaFinishedRecordsRequests;

    private string GetFullZone()
    {
        var zone = Zones[0];

        for (var i = 0; i < Zones.Count - 1; i++)
        {
            if (i == CurrentZoneIndex)
            {
                break;
            }

            zone = $"{zone}|{Zones[i + 1]}";
        }

        return zone;
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

    private void UpdateRecords()
    {
        if (CurrentZoneIndex == -1)
        {
            LabelGhostSelection.SetText("Local");
            UpdateLocalGhosts();
            return;
        }

        LabelGhostSelection.SetText("|Zone|" + Zones[CurrentZoneIndex]);

        var filter = GetFilter();
        var filterKey = ConstructFilterKey(filter);
        var zone = GetFullZone();

        LabelLoadingResult.SetText("");

        if (!EnvimaniaFinishedRecordsRequests.ContainsKey(filterKey) || !EnvimaniaFinishedRecordsRequests[filterKey].ContainsKey(zone))
        {
            foreach (var control in FrameGhosts.Controls)
            {
                control.Hide();
            }

            if (EnvimaniaRecordsRequests.ContainsKey(filterKey))
            {
                QuadLoading.Show();
            }
            else
            {
                QuadLoading.Hide();
                LabelLoadingResult.SetText("No records found");
            }

            return;
        }

        QuadLoading.Hide();

        var response = EnvimaniaFinishedRecordsRequests[filterKey][zone];

        if (response.Records.Length == 0)
        {
            LabelLoadingResult.SetText("No records found");
        }
        else
        {
            LabelLoadingResult.SetText("");
        }

        for (var i = 0; i < FrameGhosts.Controls.Count; i++)
        {
            var frame = (FrameGhosts.Controls[i] as CMlFrame)!;

            if (i >= response.Records.Length)
            {
                frame.Hide();
                continue;
            }

            var ghostUrl = response.Records[i].GhostUrl;

            frame.DataAttributeSet("file", "");
            frame.DataAttributeSet("url", ghostUrl);
            frame.DataAttributeSet("gindex", "0");
            frame.Show();

            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;
            var labelAutosave = (frame.GetFirstChild("LabelAutosave") as CMlLabel)!;
            var quadGhost = (frame.GetFirstChild("QuadGhost") as CMlQuad)!;

            labelRank.Show();
            labelRank.SetText(TextLib.FormatInteger(i + 1, 2));
            labelNickname.SetText(response.Records[i].User.Nickname);
            labelTime.SetText(TimeToTextWithMilli(response.Records[i].Time));
            labelAutosave.Hide();

            quadGhost.StyleSelected = SelectedGhosts.ContainsKey(ghostUrl);
        }
    }

    private bool RequestCurrentZoneRecords(bool refresh)
    {
        if (CurrentZoneIndex < 0)
        {
            Log("Zone index is considered local records but online records were requested. Skipped.");
            return false;
        }

        var filter = GetFilter();
        var filterKey = ConstructFilterKey(filter);
        var zone = GetFullZone();

        if ((!refresh && EnvimaniaFinishedRecordsRequests.ContainsKey(filterKey) && EnvimaniaFinishedRecordsRequests[filterKey].ContainsKey(zone)) || EnvimaniaRecordsRequests.ContainsKey(filterKey))
        {
            return false;
        }

        Log($"Requesting Envimania records... ({filter.Car}, G: {filter.Gravity}, Type: Time, Zone: {GetFullZone()})");

        EnvimaniaRecordsRequests[filterKey] = Http.CreateGet($"{EnvimixWebAPI}/envimania/records/{Map.MapInfo.MapUid}/{filter.Car}?gravity={filter.Gravity}&laps={filter.Laps}&zone={zone}", UseCache: false, $"Authorization: Bearer {EnvimixTurboUserToken}");
        EnvimaniaUnfinishedRecordsRequests[filterKey] = filter;

        QuadLoading.Show();

        UpdateRecords();

        return true;
    }

    public void RefreshRecords()
    {
        if (CurrentZoneIndex == -1)
        {
            Log("Refreshing replays on disk...");
            DataFileMgr.Replay_RefreshFromDisk();
            LocalReplaysTask = DataFileMgr.Replay_GetGameList("", true);
            return;
        }

        if (CurrentZoneIndex < Zones.Count)
        {
            RequestCurrentZoneRecords(refresh: true);
            return;
        }
    }

    private void RequestAndUpdateRecords()
    {
        if (CurrentZoneIndex > -1)
        {
            RequestCurrentZoneRecords(refresh: false);
        }

        UpdateRecords();
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

        FrameMultiplayer.Visible = !IsSolo();
        FrameSingleplayer.Visible = IsSolo();
        FrameButtonSpectator.Visible = !IsSolo();
        FrameButtonManageServer.Visible = !IsSolo();

        if (IsSolo())
        {
            FrameButtonChooseSkin.RelativePosition_V3.Y = 20;
            FrameButtonAdvancedOptions.RelativePosition_V3.Y = 10;
        }

        PreviousEnableDefaultCar = EnableDefaultCar;

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
        foreach (var car in DisplayedCars)
        {
            var userSkins = UserSkins;

            if (persistent_EnvimixSkins.Get().ContainsKey(car))
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
        FrameTeamInfo.RelativePosition_V3.Y = -135;

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

        PrevUseForcedClans = UseForcedClans;

        Zones = TextLib.Split("|", LocalUser.ZonePath);

        if (IsSolo())
        {
            var canListenToUIEvents = Netread<bool>.For(Teams[0]);
            Wait(() => canListenToUIEvents.Get());
            LocalReplaysTask = DataFileMgr.Replay_GetGameList("", true);
        }

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

                FrameTeamInfo.RelativePosition_V3.Y = -135;
                AnimMgr.Add(FrameTeamInfo, "<frame pos=\"0 -80\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                for (var i = 0; i < 2; i++)
                {
                    //FrameTimeLimit.Controls[i].Size.X = 0;
                    //AnimMgr.Add(FrameTimeLimit.Controls[i], "<quad size=\"50 7\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                    //FrameTeamMessage.Controls[i].Size.X = 0;
                    //AnimMgr.Add(FrameTeamMessage.Controls[i], "<quad size=\"55 7\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                /*FrameRedPlayerCount.RelativePosition_V3.X = 12.5;
                FrameRedPlayerCount.Parent.RelativePosition_V3.X = 10;
                AnimMgr.Add(FrameRedPlayerCount.Parent, "<frame pos=\"-21.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameRedPlayerCount, "<frame pos=\"0 0\"/>", 700, CAnimManager.EAnimManagerEasing.QuadOut);
                FrameBluePlayerCount.RelativePosition_V3.X = -12.5;
                FrameBluePlayerCount.Parent.RelativePosition_V3.X = -10;
                AnimMgr.Add(FrameBluePlayerCount.Parent, "<frame pos=\"21.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameBluePlayerCount, "<frame pos=\"0 0\"/>", 700, CAnimManager.EAnimManagerEasing.QuadOut);

                FrameJoinTeam.Controls[0].Size.X = 0;
                FrameJoinTeam.Controls[1].Size.X = 0;
                AnimMgr.Add(FrameJoinTeam.Controls[0], "<quad size=\"60 10\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameJoinTeam.Controls[1], "<quad size=\"60 10\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                FrameJoinRed.RelativePosition_V3.X = 29.5;
                AnimMgr.Add(FrameJoinRed, "<frame pos=\"0 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                FrameJoinBlue.RelativePosition_V3.X = -29.5;
                AnimMgr.Add(FrameJoinBlue, "<frame pos=\"0 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);*/

                MenuOpenTime = Now;
            }
            else
            {
                NavFocusedControl.StyleSelected = false;

                // Multiplayer specific spawning
                if (CutOffTimeLimit != -1 && InputPlayer.RaceStartTime > CutOffTimeLimit && !IsSpectator)
                {
                    SendCustomEvent("Car", new [] { DisplayedCars[VehicleIndex], "True", "False", "True" });
                }

                // Solo specific spawning
                // TODO: 3000 should be compatible with custom countdown
                if (IsSolo() && InputPlayer.RaceStartTime - 3000 > GameTime)
                {
                    SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True" });
                }

                AnimMgr.Flush(FrameModeHelp);
                FrameModeHelp.Visible = false;
                FrameModeHelp.RelativePosition_V3 = new Vec2(0, -130f);
            }
        }

        if (NavOnVehicle)
        {
            LabelArrow.TextColor = new Vec3(0.0, 0.2, 0.4);
        }
        else
        {
            LabelArrow.TextColor = new Vec3(1,1,1);
        }

        if (EnableDefaultCar != PreviousEnableDefaultCar)
        {
            UpdateVehicles();
            PreviousEnableDefaultCar = EnableDefaultCar;
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
                SetSlidingText(FrameLabelMapType, "$aaaNON-ENVIMIX MAP");
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
            LabelSkinCar.Value = car.Get();
            RequestAndUpdateRecords();

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

        /*if (CutOffTimeLimit == -1)
        {
            LabelTimeLimit.Value = "-:--";
        }
        else if (CutOffTimeLimit - GameTime < 0)
        {
            LabelTimeLimit.Value = "0:00";
        }
        else
        {
            LabelTimeLimit.Value = TextLib.TimeToText(CutOffTimeLimit - GameTime);
        }*/

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

        Dictionary<int, int> playerCounts = new() { { 1, 0 }, { 2, 0 } };

        foreach (var score in Scores)
        {
            if (!playerCounts.ContainsKey(score.TeamNum))
            {
                playerCounts[score.TeamNum] = 0;
            }

            playerCounts[score.TeamNum] += 1;
        }

        if (playerCounts.ContainsKey(1))
        {
            LabelRedPlayerCount.Value = playerCounts[1].ToString();
        }
        else
        {
            LabelRedPlayerCount.Value = "0";
        }

        if (playerCounts.ContainsKey(2))
        {
            LabelBluePlayerCount.Value = playerCounts[2].ToString();
        }
        else
        {
            LabelBluePlayerCount.Value = "0";
        }

        /*if (UseClans)
        {
            FrameTeamInfo.Show();

            if (MenuKind != PreviousMenuKind)
            {
                if (MenuKind == "Skin")
                {
                    AnimMgr.Add(FrameTeamInfo, "<frame pos=\"0 -135\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                else
                {
                    AnimMgr.Add(FrameTeamInfo, "<frame pos=\"0 -80\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                }
            }

            if (UseForcedClans)
            {
                QuadJoinBlueLock.Show();
                QuadJoinRedLock.Show();
                QuadJoinBlue.Hide();
                QuadJoinRed.Hide();
            }
            else if (playerCounts.ContainsKey(1) && playerCounts.ContainsKey(2))
            {
                if (playerCounts[1] > playerCounts[2])
                {
                    QuadJoinBlueLock.Hide();
                    QuadJoinRedLock.Show();
                    QuadJoinBlue.Show();
                    QuadJoinRed.Hide();
                }
                else if (playerCounts[2] > playerCounts[1])
                {
                    QuadJoinBlueLock.Show();
                    QuadJoinRedLock.Hide();
                    QuadJoinBlue.Hide();
                    QuadJoinRed.Show();
                }
                else if (playerCounts[1] == playerCounts[2])
                {
                    QuadJoinBlueLock.Show();
                    QuadJoinRedLock.Show();
                    QuadJoinBlue.Hide();
                    QuadJoinRed.Hide();
                }
                else
                {
                    QuadJoinBlueLock.Hide();
                    QuadJoinRedLock.Hide();
                    QuadJoinBlue.Show();
                    QuadJoinRed.Show();
                }
            }
        }
        else
        {
            FrameTeamInfo.Hide();
        }*/

        /*if (UseForcedClans != PrevUseForcedClans)
        {
            if (UseForcedClans)
            {
                AnimMgr.Add(FrameTeamInfo, "<frame scale=\"0.75\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Add(FrameTeamInfo, "<frame scale=\"1\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PrevUseForcedClans = UseForcedClans;
        }*/

        PreviousMenuKind = MenuKind;

        if (ChangingGravity)
        {
            if (MouseLeftButton)
            {
                var visualValue = MathLib.NearestInteger(MathLib.Clamp(MouseX - ChangingGravityX, 0, 30) * (9 / 30f)) * (30f / 9);
                var actualValue = MathLib.NearestInteger(visualValue * (9 / 30f)) - 9;
                var actualFloatValue = (actualValue + 10) / 10f;

                ClientGravity = actualValue;

                if (actualValue != PrevGravityValue)
                {
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
                    SendCustomEvent("Gravity", new[] { actualValue.ToString() });
                    PrevGravityValue = actualValue;
                }

                FrameGravitySlider.RelativePosition_V3.X = visualValue;
                QuadGravityValue.Opacity = 1;
                LabelGravityValue.Value = TextLib.FormatReal(actualFloatValue, 1, _HideZeroes: false, _HideDot: false);
            }
            else
            {
                ChangingGravity = false;
                AnimMgr.Add(QuadGravityValue, "<quad opacity=\"0.85\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        }

        var gravity = Netread<int>.For(GetPlayer());

        FrameGravityForcedValue.RelativePosition_V3.X = (gravity.Get() + 9) * (30f / 9);

        if (LocalReplaysTask is not null && !LocalReplaysTask.IsProcessing)
        {
            LocalReplays.Clear();
            SendCustomEvent("ResetReplays", new[] { "True" });

            if (LocalReplaysTask.HasSucceeded)
            {
                foreach (var replayInfo in LocalReplaysTask.ReplayInfos)
                {
                    if (Map.MapInfo.MapUid == "" || replayInfo.MapUid != Map.MapInfo.MapUid)
                    {
                        continue;
                    }

                    /*declare Continue = False;
                    foreach (Ghost, LoadedGhosts)
                    if (Replay.FileName == Ghost.File) Continue = True;
                    if (Continue) continue;*/

                    LocalReplays.Add(replayInfo);
                    LocalReplayFiles.Add(replayInfo.FileName);
                    SendCustomEvent("Replay", new[] { replayInfo.FileName });
                }
            }

            LocalReplaysTask = null;
        }

        if (FrameTooltip.Visible)
        {
            FrameTooltip.RelativePosition_V3 = new Vec2(MouseX, MouseY);
        }

        QuadLoading.RelativeRotation += Period / 5f;

        ImmutableArray<string> recsRequestsToRemove = new();

        foreach (var (filterKey, recsRequest) in EnvimaniaRecordsRequests)
        {
            if (recsRequest is null || !recsRequest.IsCompleted)
            {
                continue;
            }

            var filter = EnvimaniaUnfinishedRecordsRequests[filterKey];
            EnvimaniaUnfinishedRecordsRequests.Remove(filterKey);

            if (recsRequest.StatusCode == 200)
            {
                SEnvimaniaRecordsResponse response = new();

                if (response.FromJson(recsRequest.Result))
                {
                    if (EnvimaniaFinishedRecordsRequests.ContainsKey(filterKey))
                    {
                        EnvimaniaFinishedRecordsRequests[filterKey][response.Zone] = response;
                    }
                    else
                    {
                        EnvimaniaFinishedRecordsRequests[filterKey] = new()
                        {
                            { response.Zone, response }
                        };
                    }

                    Log($"Records received ({recsRequest.StatusCode}, {filter.Car}, G: {filter.Gravity}, L: {filter.Laps}, Type: Time, Zone: {response.Zone}).");
                }
                else
                {
                    Log($"Records retrieval failed (JSON issue, {filter.Car}, G: {filter.Gravity}, L: {filter.Laps}, Type: Time, Zone: [unknown]).");
                    Log(recsRequest.Result);
                }
            }
            else
            {
                Log($"Records retrieval failed ({recsRequest.StatusCode}, {filter.Car}, G: {filter.Gravity}, Type: Time, Zone: [unknown]).");
            }

            recsRequestsToRemove.Add(filterKey);
        }

        foreach (var filterKey in recsRequestsToRemove)
        {
            var request = EnvimaniaRecordsRequests[filterKey];
            EnvimaniaRecordsRequests.Remove(filterKey);
            Http.Destroy(request);

            UpdateRecords();
        }

        if (PrevLocalGhostMetadataUpdatedAt == -1 || LocalGhostMetadataUpdatedAt != PrevLocalGhostMetadataUpdatedAt)
        {
            if (CurrentZoneIndex == -1)
            {
                UpdateLocalGhosts();
            }

            PrevLocalGhostMetadataUpdatedAt = LocalGhostMetadataUpdatedAt;
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
                    AnimMgr.Add(gaugeDifficulty, "<gauge size=\"11 6.5\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(gaugeQuality, "<gauge size=\"11 6.5\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                else
                {
                    gaugeDifficulty.Size.X = 0;
                    gaugeQuality.Size.X = 0;
                }
            }

            PrevRatingEnabled = RatingEnabled;
        }

        var ratingsUpdatedAt = Netread<int>.For(Teams[0]);

        if (ratingsUpdatedAt.Get() != PrevRatingsUpdatedAt)
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

                var validationKey = ConstructValidationFilterKey(carName);

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
        }

        if (FrameModeHelp.Visible)
        {
            LabelModeHelpName.Value = Playground.ServerInfo.ModeName;
            LabelModeHelpDescription.Value = ModeHelp;
        }

        var validations = Netread<Dictionary<string, SEnvimaniaRecord>>.For(Teams[0]);
        var validationsUpdatedAt = Netread<int>.For(Teams[0]);

        /*if (validationsUpdatedAt.Get() != PrevValidationsUpdatedAt)
        {
            PrevValidationsUpdatedAt = validationsUpdatedAt.Get();
        }*/

        var currentFilterKey = ConstructValidationFilterKey(car.Get());

        if (validations.Get().ContainsKey(currentFilterKey))
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

        // to refresh the leaderboard when a new record is driven
        if (GhostToUpload != PrevGhostToUpload)
        {
            if (!GhostToUpload)
            {
                var filter = GetFilter();
                var filterKey = ConstructFilterKey(filter);
                EnvimaniaFinishedRecordsRequests.Remove(filterKey);
                RequestCurrentZoneRecords(true);
            }
            PrevGhostToUpload = GhostToUpload;
        }
    }
}
