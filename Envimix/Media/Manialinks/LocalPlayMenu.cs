using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class LocalPlayMenu : CManiaAppTitleLayer, IContext
{
    public const int VisibleLanServerCount = 13;
    public const float LanServerRowHeight = 10;

    [ManialinkControl] public required CMlFrame FrameLocalPlayMenu;
    [ManialinkControl] public required CMlFrame FrameMapPage;
    [ManialinkControl] public required CMlFrame FrameLanPage;
    [ManialinkControl] public required CMlFrame FrameMapList;
    [ManialinkControl] public required CMlFrame FrameMapListScrollable;
    [ManialinkControl] public required CMlQuad QuadPanelMapThumbnail;
    [ManialinkControl] public required CMlQuad QuadPanelMapEnvironment;
    [ManialinkControl] public required CMlLabel LabelPanelMapName;
    [ManialinkControl] public required CMlLabel LabelPanelAuthor;
    [ManialinkControl] public required CMlQuad QuadPlayMap;
    [ManialinkControl] public required CMlQuad QuadPlayMapBase;
    [ManialinkControl] public required CMlQuad QuadLegacyMenu;
    [ManialinkControl] public required CMlFrame FramePanelMap;
    [ManialinkControl] public required CMlLabel LabelPanelCost;
    [ManialinkControl] public required CMlLabel LabelCanyonCarTime;
    [ManialinkControl] public required CMlLabel LabelStadiumCarTime;
    [ManialinkControl] public required CMlLabel LabelValleyCarTime;
    [ManialinkControl] public required CMlLabel LabelLagoonCarTime;
    [ManialinkControl] public required CMlLabel LabelTrafficCarTime;
    [ManialinkControl] public required CMlLabel LabelDesertCarTime;
    [ManialinkControl] public required CMlLabel LabelSnowCarTime;
    [ManialinkControl] public required CMlLabel LabelRallyCarTime;
    [ManialinkControl] public required CMlLabel LabelIslandCarTime;
    [ManialinkControl] public required CMlLabel LabelBayCarTime;
    [ManialinkControl] public required CMlLabel LabelCoastCarTime;
    [ManialinkControl] public required CMlQuad QuadRefresh;
    [ManialinkControl] public required CMlQuad QuadScroller;
    [ManialinkControl] public required CMlQuad QuadLocalMaps;
    [ManialinkControl] public required CMlQuad QuadLocalNetwork;
    [ManialinkControl] public required CMlFrame FrameLanServerList;
    [ManialinkControl] public required CMlFrame FrameLanServerListScrollable;
    [ManialinkControl] public required CMlFrame FrameLanServerPanel;
    [ManialinkControl] public required CMlLabel LabelLanStatus;
    [ManialinkControl] public required CMlLabel LabelLanPanelName;
    [ManialinkControl] public required CMlLabel LabelLanPanelLogin;
    [ManialinkControl] public required CMlLabel LabelLanPanelPlayers;
    [ManialinkControl] public required CMlLabel LabelLanPanelSpectators;
    [ManialinkControl] public required CMlLabel LabelLanPanelZone;
    [ManialinkControl] public required CMlLabel LabelLanPanelMode;
    [ManialinkControl] public required CMlLabel LabelLanPanelLadder;
    [ManialinkControl] public required CMlLabel LabelLanPanelDescription;
    [ManialinkControl] public required CMlQuad QuadJoinLanServer;
    [ManialinkControl] public required CMlQuad QuadJoinLanServerBase;
    [ManialinkControl] public required CMlQuad QuadLanScroller;

    public CTaskResult_MapList MapListTask;
    public bool MapListLoaded;
    public string SelectedFolderPath;
    public string SelectedFilePath;

    public string ScoreContextPrefix = "";

    public bool HoldScrollbar;
    public float HoldScrollbarPos;
    public bool ScrollbarMouseOut;

    public bool LocalNetworkMode;
    public string SelectedLanServerLogin = "";
    public float PrevLanScrollY;
    public bool HoldLanScrollbar;
    public float HoldLanScrollbarPos;
    public bool LanScrollbarMouseOut;
    public int LocalServerDiscoveryStartedAt;

    public CAudioSource AudioClick;

    public LocalPlayMenu()
    {
        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    Show();
                    break;
                case "AnimateClose":
                    Hide();
                    break;
            }
        };

        QuadPlayMap.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
        };

        QuadPlayMap.MouseClick += () =>
        {
            AudioPlayClick();
            if (SelectedFilePath != "")
            {
                SendCustomEvent("PlayLocalMap", new[] { SelectedFilePath });
            }
        };

        QuadLegacyMenu.MouseClick += () =>
        {
            SendCustomEvent("MenuLocalLegacy", new[] { "" });
        };

        QuadLegacyMenu.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadRefresh.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            if (LocalNetworkMode)
            {
                DiscoverLocalServers();
            }
            else
            {
                var folderPath = MapListTask.Path;
                DataFileMgr.Map_RefreshFromDisk();
                DataFileMgr.TaskResult_Release(MapListTask.Id);
                LoadMapList(folderPath);
            }
        };

        QuadLocalMaps.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            ShowLocalMaps();
        };

        QuadLocalNetwork.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            ShowLocalNetwork();
        };

        QuadJoinLanServer.MouseClick += () =>
        {
            JoinSelectedLanServer();
        };

        QuadLanScroller.MouseClick += () =>
        {
            HoldLanScrollbar = true;
            HoldLanScrollbarPos = MouseY - (float)QuadLanScroller.RelativePosition_V3.Y;
        };

        QuadLanScroller.MouseOver += () =>
        {
            AnimMgr.Add(QuadLanScroller, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadLanScroller.MouseOut += () =>
        {
            if (HoldLanScrollbar)
                LanScrollbarMouseOut = true;
            else
                AnimMgr.Add(QuadLanScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadScroller.MouseClick += () =>
        {
            HoldScrollbar = true;
            HoldScrollbarPos = MouseY - (float)QuadScroller.RelativePosition_V3.Y;
        };

        QuadScroller.MouseOver += () =>
        {
            AnimMgr.Add(QuadScroller, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadScroller.MouseOut += () =>
        {
            if (HoldScrollbar)
            {
                ScrollbarMouseOut = true;
            }
            else
            {
                AnimMgr.Add(QuadScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadItem")
            {
                var quad = (control as CMlQuad)!;

                var folder = quad.DataAttributeGet("folder");
                var file = quad.DataAttributeGet("file");

                if (file != "")
                {
                    if (SelectedFilePath == file)
                    {
                        SendCustomEvent("PlayLocalMap", new[] { SelectedFilePath });
                    }
                    else
                    {
                        SelectedFilePath = file;
                    }

                    SelectedFolderPath = "";
                }
                else
                {
                    if (SelectedFolderPath == folder)
                    {
                        DataFileMgr.TaskResult_Release(MapListTask.Id);
                        LoadMapList(SelectedFolderPath);
                        SelectedFolderPath = "";
                    }
                    else
                    {
                        SelectedFolderPath = folder;
                    }

                    SelectedFilePath = "";
                }

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

                UpdateMapList();
                UpdateMapPanel();
            }
            else if (controlId == "QuadLanServer")
            {
                var serverLogin = control.DataAttributeGet("login");
                if (serverLogin == "")
                    return;

                if (SelectedLanServerLogin == serverLogin)
                    JoinSelectedLanServer();
                else
                    SelectedLanServerLogin = serverLogin;

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
                UpdateLanServerList();
                UpdateLanServerPanel();
            }
        };
    }

    private void AudioPlayClick()
    {
        AudioClick.Stop();
        AudioClick.Play();
    }

    public void Main()
    {
        FrameLocalPlayMenu.RelativePosition_V3.X = 180;

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        LoadMapList("");

        ShowLocalMaps();
        UpdateMapList();
        UpdateMapPanel();
    }

    public float PrevScrollY;

    public void Loop()
    {
        if (LocalNetworkMode)
        {
            UpdateLanScrolling();
            UpdateLanServerList();
            UpdateLanServerPanel();

            if (TitleControl.LocalServers_CurrentTitle.Count == 0 && Now - LocalServerDiscoveryStartedAt >= 1000)
                LabelLanStatus.SetText("No local servers found");
            else
                LabelLanStatus.SetText("");

            return;
        }

        if (!MapListLoaded && MapListTask is not null && MapListTask.HasSucceeded)
        {
            UpdateMapList();
            MapListLoaded = true;
        }

        if (HoldScrollbar && MouseLeftButton)
        {
            var newY = MathLib.Clamp(MouseY - HoldScrollbarPos, (float)QuadScroller.Size.Y - 130, 0);
            
            var targetScrollOffset = newY / ((float)QuadScroller.Size.Y - 130) * (float)FrameMapListScrollable.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger((float)targetScrollOffset / 10f);
            var steppedScrollOffset = stepIndex * 10f;
            
            steppedScrollOffset = MathLib.Clamp(steppedScrollOffset, 0f, (float)FrameMapListScrollable.ScrollMax.Y);
            
            if (FrameMapListScrollable.ScrollMax.Y > 0)
            {
                QuadScroller.RelativePosition_V3.Y = -(steppedScrollOffset / FrameMapListScrollable.ScrollMax.Y) * (130 - QuadScroller.Size.Y);
            }
            
            FrameMapListScrollable.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldScrollbar)
        {
            if (ScrollbarMouseOut)
            {
                AnimMgr.Add(QuadScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
                ScrollbarMouseOut = false;
            }
            HoldScrollbar = false;
        }

        if (FrameMapListScrollable.ScrollOffset.Y != PrevScrollY)
        {
            PrevScrollY = (float)FrameMapListScrollable.ScrollOffset.Y;
            FrameMapList.RelativePosition_V3.Y = -PrevScrollY;
            UpdateMapList();
        }
    }

    private void ShowLocalMaps()
    {
        LocalNetworkMode = false;
        QuadLocalMaps.StyleSelected = true;
        QuadLocalNetwork.StyleSelected = false;
        FrameMapPage.Show();
        FrameLanPage.Hide();
    }

    private void DiscoverLocalServers()
    {
        LocalServerDiscoveryStartedAt = Now;
        LabelLanStatus.SetText("Searching for local servers...");
        TitleControl.DiscoverLocalServers();
    }

    private void UpdateLanServerList()
    {
        FrameLanServerListScrollable.ScrollMax.Y = MathLib.Max(0, TitleControl.LocalServers_CurrentTitle.Count - VisibleLanServerCount) * LanServerRowHeight * 1f;
        var scrollOffset = MathLib.NearestInteger((float)FrameLanServerListScrollable.ScrollOffset.Y / LanServerRowHeight);

        if (TitleControl.LocalServers_CurrentTitle.Count > VisibleLanServerCount)
        {
            QuadLanScroller.Show();
            QuadLanScroller.Size.Y = VisibleLanServerCount * 1f / TitleControl.LocalServers_CurrentTitle.Count * 130f;
            QuadLanScroller.RelativePosition_V3.Y = -(float)FrameLanServerListScrollable.ScrollOffset.Y
                / FrameLanServerListScrollable.ScrollMax.Y * (130 - QuadLanScroller.Size.Y);
        }
        else
        {
            QuadLanScroller.Hide();
        }

        var index = 0;
        foreach (var control in FrameLanServerList.Controls)
        {
            var frame = (control as CMlFrame)!;
            var serverIndex = index + scrollOffset;

            if (serverIndex >= TitleControl.LocalServers_CurrentTitle.Count)
            {
                frame.Hide();
                index += 1;
                continue;
            }

            var server = TitleControl.LocalServers_CurrentTitle[serverIndex];
            var quadServer = (frame.GetFirstChild("QuadLanServer") as CMlQuad)!;
            var labelName = (frame.GetFirstChild("LabelLanServerName") as CMlLabel)!;
            var labelPlayers = (frame.GetFirstChild("LabelLanPlayers") as CMlLabel)!;
            var labelMode = (frame.GetFirstChild("LabelLanMode") as CMlLabel)!;

            quadServer.DataAttributeSet("login", server.ServerLogin);
            quadServer.StyleSelected = server.ServerLogin == SelectedLanServerLogin;
            labelName.SetText(server.ServerName);
            labelPlayers.SetText($"{server.PlayerCount}$888/{server.MaxPlayerCount}");
            labelMode.SetText(server.ModeName);
            if (server.IsPrivate)
                labelPlayers.SetText($"$ff0🔒 $fff{server.PlayerCount}$888/{server.MaxPlayerCount}");
            frame.Show();
            index += 1;
        }
    }

    private void UpdateLanServerPanel()
    {
        foreach (var server in TitleControl.LocalServers_CurrentTitle)
        {
            if (server.ServerLogin != SelectedLanServerLogin)
                continue;

            LabelLanPanelName.SetText(server.ServerName);
            LabelLanPanelLogin.SetText(server.ServerLogin);
            LabelLanPanelPlayers.SetText($"{server.PlayerCount}/{server.MaxPlayerCount}");
            if (server.IsPrivate)
                LabelLanPanelPlayers.SetText($"{server.PlayerCount}/{server.MaxPlayerCount}$ff0🔒");
            LabelLanPanelSpectators.SetText($"{server.SpectatorCount}/{server.MaxSpectatorCount}");
            if (server.IsPrivateForSpectator)
                LabelLanPanelSpectators.SetText($"{server.SpectatorCount}/{server.MaxSpectatorCount}$ff0🔒");
            LabelLanPanelZone.SetText("");
            LabelLanPanelMode.SetText(server.ModeName);
            LabelLanPanelLadder.SetText($"Ladder {server.LadderServerLimitMin}-{server.LadderServerLimitMax}");
            LabelLanPanelDescription.SetText(server.Comment);
            QuadJoinLanServerBase.Colorize = new Vec3(0, 1, 0);
            QuadJoinLanServer.Show();
            FrameLanServerPanel.Show();
            return;
        }

        SelectedLanServerLogin = "";
        QuadJoinLanServerBase.Colorize = new Vec3(0.1, 0.1, 0.1);
        QuadJoinLanServer.Hide();
        FrameLanServerPanel.Hide();
    }

    private void ShowLocalNetwork()
    {
        LocalNetworkMode = true;
        QuadLocalMaps.StyleSelected = false;
        QuadLocalNetwork.StyleSelected = true;
        FrameMapPage.Hide();
        FrameLanPage.Show();
        SelectedLanServerLogin = "";
        FrameLanServerListScrollable.ScrollOffset.Y = 0;
        PrevLanScrollY = 0;
        FrameLanServerList.RelativePosition_V3.Y = 0;
        DiscoverLocalServers();
        UpdateLanServerList();
        UpdateLanServerPanel();
    }

    private void JoinSelectedLanServer()
    {
        foreach (var server in TitleControl.LocalServers_CurrentTitle)
        {
            if (server.ServerLogin == SelectedLanServerLogin)
            {
                OpenLink(server.JoinLink, CMlScript.LinkType.Goto);
                return;
            }
        }
    }

    private void UpdateLanScrolling()
    {
        if (HoldLanScrollbar && MouseLeftButton)
        {
            var newY = MathLib.Clamp(MouseY - HoldLanScrollbarPos, (float)QuadLanScroller.Size.Y - 130f, 0);
            var targetScrollOffset = newY / ((float)QuadLanScroller.Size.Y - 130) * (float)FrameLanServerListScrollable.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger(targetScrollOffset / LanServerRowHeight);
            var steppedScrollOffset = MathLib.Clamp(stepIndex * LanServerRowHeight * 1f, 0, (float)FrameLanServerListScrollable.ScrollMax.Y);
            if (FrameLanServerListScrollable.ScrollMax.Y > 0)
                QuadLanScroller.RelativePosition_V3.Y = -(steppedScrollOffset / FrameLanServerListScrollable.ScrollMax.Y) * (130 - QuadLanScroller.Size.Y);
            FrameLanServerListScrollable.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldLanScrollbar)
        {
            if (LanScrollbarMouseOut)
            {
                AnimMgr.Add(QuadLanScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
                LanScrollbarMouseOut = false;
            }
            HoldLanScrollbar = false;
        }

        if (FrameLanServerListScrollable.ScrollOffset.Y != PrevLanScrollY)
        {
            PrevLanScrollY = (float)FrameLanServerListScrollable.ScrollOffset.Y;
            FrameLanServerList.RelativePosition_V3.Y = -PrevLanScrollY;
            UpdateLanServerList();
        }
    }

    private void LoadMapList(string folderPath)
    {
        MapListTask = DataFileMgr.Map_GetFilteredGameList(7, folderPath, false);
        MapListLoaded = false;
    }

    private void UpdateMapList()
    {
        var totalCount = 0;
        var hasParentEntry = false;
        ImmutableArray<string> visibleFolders = new();
        ImmutableArray<CMapInfo> playableMaps = new();
        if (MapListTask is not null)
        {
            foreach (var subFolder in MapListTask.SubFolders)
            {
                if (MapListTask.Path == "" && subFolder == "Campaigns\\")
                {
                    continue;
                }

                visibleFolders.Add(subFolder);
            }

            foreach (var mapInfo in MapListTask.MapInfos)
            {
                if (mapInfo.IsPlayable)
                {
                    playableMaps.Add(mapInfo);
                }
            }
            hasParentEntry = MapListTask.Path != "";
            totalCount = visibleFolders.Length + playableMaps.Length;
            if (hasParentEntry)
            {
                totalCount += 1;
            }
        }

        if (totalCount > 13)
        {
            QuadScroller.Show();
            QuadScroller.Size.Y = 13f / totalCount * 130;
        }
        else
        {
            QuadScroller.Hide();
        }

        FrameMapListScrollable.ScrollMax.Y = MathLib.Max(0, totalCount - 13) * 10f;
        var scrollOffset = MathLib.NearestInteger((float)FrameMapListScrollable.ScrollOffset.Y / 10);

        if (FrameMapListScrollable.ScrollMax.Y != 0)
        {
            QuadScroller.RelativePosition_V3.Y = -(float)FrameMapListScrollable.ScrollOffset.Y / FrameMapListScrollable.ScrollMax.Y * (130 - QuadScroller.Size.Y);
        }

        var i = 0;
        foreach (var control in FrameMapList.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (MapListTask is null)
            {
                frame.Visible = false;
                continue;
            }

            var labelMapName = (frame.GetFirstChild("LabelMapName") as CMlLabel)!;
            var quadIcon = (frame.GetFirstChild("QuadIcon") as CMlQuad)!;
            var quadEnv = (frame.GetFirstChild("QuadEnvironment") as CMlQuad)!;
            var quadItem = (frame.GetFirstChild("QuadItem") as CMlQuad)!;

            int adjustedIndex;
            if (hasParentEntry)
            {
                adjustedIndex = i - 1 + scrollOffset;
            }
            else
            {
                adjustedIndex = i + scrollOffset;
            }

            if (hasParentEntry && i + scrollOffset == 0)
            {
                labelMapName.Value = "..";
                quadIcon.Style = "UIConstruction_Buttons";
                quadIcon.Substyle = "Directory";
                quadIcon.ChangeImageUrl("");
                quadEnv.ChangeImageUrl("");
                if (MapListTask.ParentPath == "")
                {
                    quadItem.DataAttributeSet("folder", "/");
                }
                else
                {
                    quadItem.DataAttributeSet("folder", MapListTask.ParentPath);
                }
                quadItem.DataAttributeSet("file", "");
                quadItem.StyleSelected = SelectedFolderPath != "" && SelectedFolderPath == MapListTask.ParentPath;
                frame.Visible = true;
                i += 1;
                continue;
            }

            if (adjustedIndex < visibleFolders.Length)
            {
                var subFolder = visibleFolders[adjustedIndex];

                var folderName = "";
                foreach (var folderPath in TextLib.Split("\\", subFolder))
                {
                    folderName = folderPath;
                }

                labelMapName.Value = folderName;
                quadIcon.Style = "UIConstruction_Buttons";
                quadIcon.Substyle = "Directory";
                quadIcon.ChangeImageUrl("");
                quadEnv.ChangeImageUrl("");
                quadItem.DataAttributeSet("folder", subFolder);
                quadItem.DataAttributeSet("file", "");
                quadItem.StyleSelected = SelectedFolderPath == subFolder;

                frame.Visible = true;
            }
            else if (adjustedIndex - visibleFolders.Length < playableMaps.Length)
            {
                var itemIndex = adjustedIndex - visibleFolders.Length;
                var mapName = playableMaps[itemIndex].Name;
                var mapUid = playableMaps[itemIndex].MapUid;
                var environment = playableMaps[itemIndex].CollectionName;
                var filePath = playableMaps[itemIndex].FileName;

                labelMapName.Value = mapName;
                quadIcon.Style = "";
                quadIcon.Substyle = "";
                quadIcon.ChangeImageUrl($"file://Thumbnails/MapUid/{mapUid}");
                quadEnv.ChangeImageUrl($"file://Media/Images/Environments/{environment}TMT.png");
                quadItem.DataAttributeSet("folder", "");
                quadItem.DataAttributeSet("file", filePath);
                quadItem.StyleSelected = SelectedFilePath == filePath;

                frame.Visible = true;
            }
            else
            {
                frame.Visible = false;
            }

            i += 1;
        }
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private void SetTimeLabel(CMlLabel label, string car, CMapInfo mapInfo)
    {
        var scoreContext = $"{car}{ScoreContextPrefix}";

        // hacky but it works for TMT
        if ((mapInfo.CollectionName == "Canyon" && car == "CanyonCar")
            || (mapInfo.CollectionName == "Stadium" && car == "StadiumCar")
            || (mapInfo.CollectionName == "Valley" && car == "ValleyCar")
            || (mapInfo.CollectionName == "Lagoon" && car == "LagoonCar"))
        {
            scoreContext = ScoreContextPrefix;
        }

        var time = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);

        if (time < 0)
        {
            label.Value = "-:--.---";
        }
        else
        {
            label.Value = TimeToTextWithMilli(time);
        }
    }

    private void UpdateMapPanel()
    {
        if (SelectedFilePath == "")
        {
            FramePanelMap.Hide();
            QuadPanelMapThumbnail.Hide();
            QuadPlayMapBase.Colorize = new Vec3(0.1, 0.1, 0.1);
            QuadPlayMap.Hide();
        }
        else
        {
            foreach (var mapInfo in MapListTask.MapInfos)
            {
                if (mapInfo.FileName == SelectedFilePath)
                {
                    QuadPanelMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                    QuadPanelMapThumbnail.Show();
                    QuadPanelMapEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{mapInfo.CollectionName}TMT.png");
                    LabelPanelMapName.Value = mapInfo.Name;
                    LabelPanelAuthor.Value = mapInfo.AuthorNickName;
                    LabelPanelCost.Value = $"{mapInfo.CopperPrice}cc";
                    SetTimeLabel(LabelCanyonCarTime, "CanyonCar", mapInfo);
                    SetTimeLabel(LabelStadiumCarTime, "StadiumCar", mapInfo);
                    SetTimeLabel(LabelValleyCarTime, "ValleyCar", mapInfo);
                    SetTimeLabel(LabelLagoonCarTime, "LagoonCar", mapInfo);
                    SetTimeLabel(LabelTrafficCarTime, "TrafficCar", mapInfo);
                    SetTimeLabel(LabelDesertCarTime, "DesertCar", mapInfo);
                    SetTimeLabel(LabelSnowCarTime, "SnowCar", mapInfo);
                    SetTimeLabel(LabelRallyCarTime, "RallyCar", mapInfo);
                    SetTimeLabel(LabelIslandCarTime, "IslandCar", mapInfo);
                    SetTimeLabel(LabelBayCarTime, "BayCar", mapInfo);
                    SetTimeLabel(LabelCoastCarTime, "CoastCar", mapInfo);
                    QuadPlayMapBase.Colorize = new Vec3(0, 1, 0);
                    QuadPlayMap.Show();
                    FramePanelMap.Show();
                    return;
                }
            }
        }
    }

    private void Show()
    {
        AnimMgr.Add(FrameLocalPlayMenu, "<frame pos=\"0 0\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void Hide()
    {
        AnimMgr.Add(FrameLocalPlayMenu, "<frame pos=\"180 0\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }
}
