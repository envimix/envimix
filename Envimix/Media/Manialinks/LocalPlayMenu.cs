using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class LocalPlayMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlFrame FrameLocalPlayMenu;
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
    [ManialinkControl] public required CMlLabel LabelDesertCarTime;
    [ManialinkControl] public required CMlLabel LabelSnowCarTime;
    [ManialinkControl] public required CMlLabel LabelRallyCarTime;
    [ManialinkControl] public required CMlLabel LabelIslandCarTime;
    [ManialinkControl] public required CMlLabel LabelBayCarTime;
    [ManialinkControl] public required CMlLabel LabelCoastCarTime;
    [ManialinkControl] public required CMlQuad QuadRefresh;
    [ManialinkControl] public required CMlQuad QuadScroller;

    public CTaskResult_MapList MapListTask;
    public bool MapListLoaded;
    public string SelectedFolderPath;
    public string SelectedFilePath;

    public string ScoreContextPrefix = "";

    public bool HoldScrollbar;
    public float HoldScrollbarPos;
    public bool ScrollbarMouseOut;

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
            var folderPath = MapListTask.Path;
            DataFileMgr.Map_RefreshFromDisk();
            DataFileMgr.TaskResult_Release(MapListTask.Id);
            LoadMapList(folderPath);
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

        UpdateMapList();
        UpdateMapPanel();
    }

    public float PrevScrollY;

    public void Loop()
    {
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

    private void LoadMapList(string folderPath)
    {
        MapListTask = DataFileMgr.Map_GetGameList(folderPath, false);
        MapListLoaded = false;
    }

    private void UpdateMapList()
    {
        var totalCount = 0;
        var hasParentEntry = false;
        ImmutableArray<CMapInfo> playableMaps = new();
        if (MapListTask is not null)
        {
            foreach (var mapInfo in MapListTask.MapInfos)
            {
                if (mapInfo.IsPlayable)
                {
                    playableMaps.Add(mapInfo);
                }
            }
            hasParentEntry = MapListTask.Path != "";
            totalCount = MapListTask.SubFolders.Count + playableMaps.Length;
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

            if (adjustedIndex < MapListTask.SubFolders.Count)
            {
                var subFolder = MapListTask.SubFolders[adjustedIndex];

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
            else if (adjustedIndex - MapListTask.SubFolders.Count < playableMaps.Length)
            {
                var itemIndex = adjustedIndex - MapListTask.SubFolders.Count;
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

    private void SetTimeLabel(CMlLabel label, string car, string mapUid)
    {
        var time = ScoreMgr.Map_GetRecord(null, mapUid, $"{car}{ScoreContextPrefix}");

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
                    SetTimeLabel(LabelCanyonCarTime, "CanyonCar", mapInfo.MapUid);
                    SetTimeLabel(LabelStadiumCarTime, "StadiumCar", mapInfo.MapUid);
                    SetTimeLabel(LabelValleyCarTime, "ValleyCar", mapInfo.MapUid);
                    SetTimeLabel(LabelLagoonCarTime, "LagoonCar", mapInfo.MapUid);
                    SetTimeLabel(LabelDesertCarTime, "DesertCar", mapInfo.MapUid);
                    SetTimeLabel(LabelSnowCarTime, "SnowCar", mapInfo.MapUid);
                    SetTimeLabel(LabelRallyCarTime, "RallyCar", mapInfo.MapUid);
                    SetTimeLabel(LabelIslandCarTime, "IslandCar", mapInfo.MapUid);
                    SetTimeLabel(LabelBayCarTime, "BayCar", mapInfo.MapUid);
                    SetTimeLabel(LabelCoastCarTime, "CoastCar", mapInfo.MapUid);
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
