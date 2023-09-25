using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

/// <summary>
/// This code went through 4 stages of development from 2019-2023, the code is quite confusing, sorry.
/// </summary>
public class Menu : CTmMlScriptIngame, IContext
{
    public struct SSkin
	{
		public string File;
		public string Icon;
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

    public int VehicleIndex;
    public int PreviousVehicleIndex;
    public string MenuKind;
    public string PreviousMenuKind;
    public string PreviousMapUid;
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

    [Netwrite(NetFor.UI)] public string ClientCar { get; set; }
    [Netwrite(NetFor.UI)] public Dictionary<string, string> UserSkins { get; set; }
    [Netread] public bool EnableDefaultCar { get; set; }
    [Netread] public string MapPlayerModelName { get; set; }
    [Netread] public int CutOffTimeLimit { get; set; }
    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }
    [Netread] public Dictionary<string, string> ItemCars { get; set; }
    [Netread] public Dictionary<string, Dictionary<string, SSkin>> Skins { get; set; }

    public Menu()
    {
        MouseOver += Menu_MouseOver;
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

        MenuNavigation += Menu_MenuNavigation;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null) return GUIPlayer;
        return InputPlayer;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
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
                        CloseInGameMenu(CMlScriptIngame.EInGameMenuResult.Resume);
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
                    }
                }
                UpdateSkins();
                SendCustomEvent("Skin", new[] { CName, UserSkins[CName] });
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

    private void Menu_MouseOver(CMlControl control, string controlId)
    {
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
        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Resume);
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
        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Quit);
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

    private void QUAD_BUTTON_SKIN_PLAY()
	{
        AnimMgr.Add(FrameMenu, "<frame pos=\"-110 0\" hidden=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameSkins, "<frame pos=\"-110 0\" hidden=\"1\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        MenuKind = "";

        SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True" });
        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Resume);
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

    private void Menu_MenuNavigation(CMlScriptEvent.EMenuNavAction action)
    {
        switch (action)
        {
            case CMlScriptEvent.EMenuNavAction.Cancel:
                if (UI.UISequence == CUIConfig.EUISequence.Intro)
                {
                    if (ShowMenuLittleLater == -1)
                        CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Resume);
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
                    CloseInGameMenu(CTmMlScriptIngame.EInGameMenuResult.Resume);
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

                        CloseInGameMenu(CMlScriptIngame.EInGameMenuResult.Resume);
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
                            NavFocusedControl = QuadButtonManageServer;
                            Focus2();
                        }
                        else if (NavFocusedControl == QuadButtonManageServer)
                        {
                            NavFocusedControl = QuadButtonServerSettings;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonModeHelp)
                        {
                            NavFocusedControl = QuadButtonAdvanced;
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
                            NavFocusedControl = QuadButtonExit;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonModeHelp)
                        {
                            NavFocusedControl = QuadButtonServerSettings;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonServerSettings)
                        {
                            NavFocusedControl = QuadButtonManageServer;
                            Focus3();
                        }
                        else if (NavFocusedControl == QuadButtonAdvanced)
                        {
                            NavFocusedControl = QuadButtonModeHelp;
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

    public void Main()
    {
        Page.GetClassChildren("LOADING", Page.MainFrame, Recursive: true);

        (FrameButtonSpectator.GetFirstChild("LABEL") as CMlLabel)!.Value = "  $t" + TextLib.GetTranslatedText("Spectator");

        EnableMenuNavigationInputs = true;

        NavFirstControl = QuadButtonContinue;
        NavFocusedControl = NavFirstControl;

        while (!IsInGameMenuDisplayed)
        {
            Yield();
            ShowInGameMenu();
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

        foreach (var car in DisplayedCars)
        {
            var userSkins = UserSkins;
            userSkins[car] = "";
            UserSkins = userSkins;
        }

		FrameMenu.RelativePosition_V3.X = -110;
        FrameVehicleList.RelativePosition_V3.X = 110;
        FrameTeamInfo.RelativePosition_V3.Y = -135;

        if (ItemCars.ContainsKey(MapPlayerModelName))
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
    }

    public void Loop()
    {
        if (ShowMenuLittleLater != -1 && Now - ShowMenuLittleLater > 100 && Now - ShowMenuLittleLater < 300)
        {
            ShowInGameMenu();
        }

        var car = Netread<string>.For(GetPlayer());
        ClientCar = DisplayedCars[VehicleIndex];

        if (IsMenuOpen != IsMenuNavigationForeground)
        {
            IsMenuOpen = IsMenuNavigationForeground;
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

                if (CutOffTimeLimit != -1 && InputPlayer.RaceStartTime > CutOffTimeLimit && !IsSpectator)
                {
                    SendCustomEvent("Car", new [] { DisplayedCars[VehicleIndex], "True" });
                }

                // Solo specific spawning
                // TODO: 3000 should be compatible with custom countdown
                if (IsSolo() && InputPlayer.RaceStartTime - 3000 > GameTime)
                {
                    SendCustomEvent("Car", new[] { DisplayedCars[VehicleIndex], "True" });
                }
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
            SetSlidingText(FrameLabelMapName, Map.MapInfo.Name);
            if (Map.MapType == "Envimix" || Map.MapType == "TrackMania\\Envimix")
            {
                SetSlidingText(FrameLabelMapType, "$ff0ENVIMIX MAP");
            }
            else SetSlidingText(FrameLabelMapType, "$aaaNON-ENVIMIX MAP");
            PreviousMapUid = Map.MapInfo.Name;
        }

        MoveSlidingText(FrameLabelMapName, 10, -0.01f);

        if (Map.MapInfo.AuthorNickName != PreviousMapAuthor)
        {
            LabelMapAuthor.SetText(Map.MapInfo.AuthorNickName);
            PreviousMapAuthor = Map.MapInfo.AuthorNickName;
        }

        if (car.Get() != PreviousCar)
        {
            SetSlidingText(FrameLabelCar, car.Get());
            LabelSkinCar.Value = car.Get();
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
            LabelPbTime.Value = "-.--.---";
        else
            LabelPbTime.Value = TimeToTextWithMilli(GetPlayer().Score.BestRace.Time);

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

        if (FrameVehicles.ScrollOffset.Y != PreviousScrollOffset)
        {
            var difference = (float)(FrameVehicles.ScrollOffset.Y - PreviousScrollOffset);
            var indexChange = MathLib.NearestInteger((float)difference / 20);
            VehicleIndex += indexChange;
            PreviousScrollOffset = (float)FrameVehicles.ScrollOffset.Y;
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
        }

        if (FrameSkinList.Parent.ScrollOffset != PreviousSkinScrollOffset)
        {
            UpdateSkins();
            FrameSkinList.RelativePosition_V3.Y = -FrameSkinList.Parent.ScrollOffset.Y;
            PreviousSkinScrollOffset = FrameSkinList.Parent.ScrollOffset;
        }

        if (VehicleIndex != PreviousVehicleIndex)
        {
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
            FrameArrow.RelativePosition_V3.Y = FrameVehicles.ScrollAnimOffset.Y - FrameVehicles.ScrollOffset.Y;
        }

        FrameGhostArrow.RelativePosition_V3.Y = FrameVehicles.ScrollAnimOffset.Y - FrameVehicles.ScrollOffset.Y;

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

        if (UseClans)
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
        }

        if (MenuOpenTime != -1)
        {
            //FrameLabelTeamMessage.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - MenuOpenTime, 0, 55, 600);
            //FrameLabelTimeLimit.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - MenuOpenTime, 0, 50, 500);
        }

        if (UseForcedClans != PrevUseForcedClans)
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
        }

        PreviousMenuKind = MenuKind;
    }
}
