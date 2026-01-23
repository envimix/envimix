namespace Envimix.Media.Manialinks;

public class MainMenu : CManiaAppTitleLayer, IContext
{
    public struct SMapInfo
    {
        public string Name;
        public string Uid;
        public string Collection;
        public int Order;
    }

    public struct STotdInfo
    {
        public SMapInfo Map;
        public string NextAt;
    }

    [ManialinkControl] public required CMlQuad QuadSolo;
    [ManialinkControl] public required CMlQuad QuadLocal;
    [ManialinkControl] public required CMlQuad QuadInternet;
    [ManialinkControl] public required CMlQuad QuadEditor;
    [ManialinkControl] public required CMlQuad QuadQuit;
    [ManialinkControl] public required CMlFrame FrameMainMenu;
    [ManialinkControl] public required CMlLabel LabelBuild;
    [ManialinkControl] public required CMlLabel LabelSubmitCampaignMaps;
    [ManialinkControl] public required CMlLabel LabelSubmitTitle;
    [ManialinkControl] public required CMlQuad QuadTotdThumbnail;
    [ManialinkControl] public required CMlLabel LabelTotdName;
    [ManialinkControl] public required CMlLabel LabelTotdEnv;
    [ManialinkControl] public required CMlLabel LabelTotdNextAt;
    [ManialinkControl] public required CMlQuad QuadTotdLoading;
    [ManialinkControl] public required CMlFrame FrameTotd;
    [ManialinkControl] public required CMlQuad QuadTotd;
    [ManialinkControl] public required CMlLabel LabelRestoreRecords;
    [ManialinkControl] public required CMlQuad QuadLeaderboards;
    [ManialinkControl] public required CMlLabel LabelBanReason;

    public STotdInfo TotdInfo;

    public CAudioSource AudioClick;

    [Local(LocalFor.LocalUser)] public string EnvimixOpenMapUid { get; set; } = "";

    public MainMenu()
    {
        QuadSolo.MouseClick += () =>
        {
            SendCustomEvent("MenuSolo", new[] {""});
            AudioClick.Play();
        };

        QuadSolo.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        QuadLocal.MouseClick += () =>
        {
            SendCustomEvent("MenuLocal", new[] { "" });
        };

        QuadLocal.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadInternet.MouseClick += () =>
        {
            SendCustomEvent("MenuInternet", new[] { "" });
        };

        QuadInternet.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadEditor.MouseClick += () =>
        {
            SendCustomEvent("MenuEditor", new[] { "" });
        };

        QuadEditor.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadQuit.MouseClick += () =>
        {
            AudioClick.Play();
            ParentApp.Menu_Quit();
        };

        QuadQuit.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadLeaderboards.MouseClick += () =>
        {
            SendCustomEvent("Leaderboards", new[] { "" });
            AudioClick.Play();
        };

        QuadLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        LabelSubmitCampaignMaps.MouseClick += () =>
        {
            SendCustomEvent("SubmitCampaignMaps", new[] { "" });
        };

        LabelSubmitTitle.MouseClick += () =>
        {
            SendCustomEvent("SubmitTitle", new[] { "" });
        };

        LabelRestoreRecords.MouseClick += () =>
        {
            SendCustomEvent("RestoreRecords", new[] { "" });
        };

        QuadTotd.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            if (TotdInfo.Map.Uid != "")
            {
                EnvimixOpenMapUid = TotdInfo.Map.Uid;
            }
        };

        QuadTotd.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    ShowMenuFrame();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    HideMenuFrame();
                    break;
                case "Totd":
                    if (data.Length < 1)
                        break;
                    SetTotd(data[0]);
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("Quit", new[] { "" });
                    break;
            }
        };
    }

    public void Main()
    {
        LabelSubmitCampaignMaps.Hide();
        LabelSubmitTitle.Hide();
        LabelRestoreRecords.Hide();

        EnableMenuNavigationInputs = true;

        FrameMainMenu.RelativePosition_V3.X = 210;
        ShowMenuFrame();

        LabelBuild.SetText(TextLib.Split(" ", LoadedTitle.TitleVersion)[0]);

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");
    }

    public void Loop()
    {
        var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);

        if (envimixTurboUserIsAdmin.Get() && EnableMenuNavigationInputs)
        {
            LabelSubmitCampaignMaps.Visible = true;
            LabelSubmitTitle.Visible = true;
            LabelRestoreRecords.Visible = true;
        }
        else
        {
            LabelSubmitCampaignMaps.Visible = false;
            LabelSubmitTitle.Visible = false;
            LabelRestoreRecords.Visible = false;
        }

        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }

        SetNextAt();

        if (TotdInfo.NextAt != "" && TimeLib.Compare(TotdInfo.NextAt, TimeLib.GetCurrent()) <= 0)
        {
            TotdInfo.NextAt = "";

            QuadTotdLoading.Show();
            FrameTotd.Hide();
            SendCustomEvent("Totd", new[] { "" });
        }

        var envimixTurboUserBanReason = Local<string>.For(LocalUser);
        LabelBanReason.Value = envimixTurboUserBanReason.Get();
    }

    private void SetNextAt()
    {
        if (TotdInfo.NextAt == "")
        {
            LabelTotdNextAt.Value = "";
        }
        else
        {
            LabelTotdNextAt.Value = $"$AAAends in {TimeLib.FormatDelta(TimeLib.GetCurrent(), TotdInfo.NextAt, TimeLib.EDurationFormats.Full)}";
        }
    }

    private void SetTotd(string json)
    {
        TotdInfo.FromJson(json);

        QuadTotdThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{TotdInfo.Map.Uid}");
        LabelTotdName.SetText(TotdInfo.Map.Name);

        var environment = TotdInfo.Map.Collection;
        /*foreach (var campaign in DataFileMgr.Campaigns)
        {
            foreach (var group in campaign.MapGroups)
            {
                foreach (var map in group.MapInfos)
                {
                    if (map.MapUid == TotdInfo.Map.Uid)
                    {
                        environment = map.CollectionName;
                        break;
                    }
                }

                if (environment != "")
                {
                    break;
                }
            }

            if (environment != "")
            {
                break;
            }
        }*/

        LabelTotdEnv.SetText(environment);

        SetNextAt();

        QuadTotdLoading.Hide();
        FrameTotd.Show();
    }

    private void ShowMenuFrame()
    {
        AnimMgr.Add(FrameMainMenu, "<frame pos=\"90 75\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideMenuFrame()
    {
        AnimMgr.Add(FrameMainMenu, "<frame pos=\"210 75\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }
}
