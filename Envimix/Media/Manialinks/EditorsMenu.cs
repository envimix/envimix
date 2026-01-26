using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class EditorsMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlFrame FrameLocalPlayMenu;
    [ManialinkControl] public required CMlFrame FrameMapList;
    [ManialinkControl] public required CMlFrame FrameMapListScrollable;
    [ManialinkControl] public required CMlQuad QuadPanelMapThumbnail;
    [ManialinkControl] public required CMlQuad QuadPanelMapEnvironment;
    [ManialinkControl] public required CMlLabel LabelPanelMapName;
    [ManialinkControl] public required CMlLabel LabelPanelAuthor;
    [ManialinkControl] public required CMlQuad QuadAction;
    [ManialinkControl] public required CMlQuad QuadActionBase;
    [ManialinkControl] public required CMlLabel LabelAction;
    [ManialinkControl] public required CMlQuad QuadLegacyMenu;
    [ManialinkControl] public required CMlFrame FramePanelMap;
    [ManialinkControl] public required CMlLabel LabelPanelCost;
    [ManialinkControl] public required CMlQuad QuadRefresh;
    [ManialinkControl] public required CMlQuad QuadScroller;
    [ManialinkControl] public required CMlQuad QuadInterfaceDesigner;
    [ManialinkControl] public required CMlQuad QuadLoadMap;
    [ManialinkControl] public required CMlQuad QuadEditReplay;
    [ManialinkControl] public required CMlQuad QuadNewMap;
    [ManialinkControl] public required CMlFrame FrameList;
    [ManialinkControl] public required CMlFrame FrameNewMap;
    [ManialinkControl] public required CMlQuad QuadCanyon;
    [ManialinkControl] public required CMlQuad QuadStadium;
    [ManialinkControl] public required CMlQuad QuadValley;
    [ManialinkControl] public required CMlQuad QuadLagoon;

    public CTaskResult_MapList MapListTask;
    public bool MapListLoaded;
    public string MapSelectedFolderPath;
    public string MapSelectedFilePath;
    public CTaskResult_ReplayList ReplayListTask;
    public bool ReplayListLoaded;
    public string ReplaySelectedFolderPath;
    public IList<string> ReplaySelectedFilePaths;

    public string ScoreContextPrefix = "";

    public bool HoldScrollbar;
    public float HoldScrollbarPos;
    public bool ScrollbarMouseOut;

    public CMlQuad SelectedMode;

    public CAudioSource AudioClick;

    public EditorsMenu()
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

        QuadLoadMap.MouseClick += () =>
        {
            if (SelectedMode != QuadLoadMap)
            {
                SelectedMode = QuadLoadMap;
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
                MapSelectedFilePath = "";
                MapSelectedFolderPath = "";
                ReplaySelectedFilePaths.Clear();
                ReplaySelectedFolderPath = "";
                UpdateMapList();
                LoadMapList(MapSelectedFolderPath);
                QuadEditReplay.StyleSelected = false;
                QuadLoadMap.StyleSelected = true;
                QuadNewMap.StyleSelected = false;
                UpdateMapPanel();
                FrameList.Show();
                FrameNewMap.Hide();
                LabelAction.Show();
                QuadRefresh.Show();
            }
        };

        QuadEditReplay.MouseClick += () =>
        {
            if (SelectedMode != QuadEditReplay)
            {
                SelectedMode = QuadEditReplay;
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
                MapSelectedFilePath = "";
                MapSelectedFolderPath = "";
                ReplaySelectedFilePaths.Clear();
                ReplaySelectedFolderPath = "";
                UpdateReplayList();
                LoadReplayList(ReplaySelectedFolderPath);
                QuadEditReplay.StyleSelected = true;
                QuadLoadMap.StyleSelected = false;
                QuadNewMap.StyleSelected = false;
                UpdateReplayPanel();
                FrameList.Show();
                FrameNewMap.Hide();
                LabelAction.Show();
                QuadRefresh.Show();
            }
        };

        QuadNewMap.MouseClick += () =>
        {
            SelectedMode = QuadNewMap;
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            MapSelectedFilePath = "";
            MapSelectedFolderPath = "";
            ReplaySelectedFilePaths.Clear();
            ReplaySelectedFolderPath = "";
            QuadEditReplay.StyleSelected = false;
            QuadLoadMap.StyleSelected = false;
            QuadNewMap.StyleSelected = true;
            UpdateMapPanel();
            FrameList.Hide();
            FrameNewMap.Show();
            LabelAction.Hide();
            QuadRefresh.Hide();
        };

        QuadAction.MouseClick += () =>
        {
            if (SelectedMode == QuadEditReplay)
            {
                SendCustomEvent("EditReplay", (string[])ReplaySelectedFilePaths);
                ReplaySelectedFilePaths.Clear();
                UpdateReplayList();
                UpdateReplayPanel();
            }
            else if (SelectedMode == QuadLoadMap)
            {
                if (MapSelectedFilePath != "")
                {
                    SendCustomEvent("EditMap", new[] { MapSelectedFilePath });
                }
            }

            AudioPlayClick();
        };

        QuadInterfaceDesigner.MouseClick += () =>
        {
            SendCustomEvent("InterfaceDesigner", new[] { "" });
        };

        QuadLegacyMenu.MouseClick += () =>
        {
            SendCustomEvent("MenuEditorLegacy", new[] { "" });
        };

        QuadLegacyMenu.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadRefresh.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

            if (SelectedMode == QuadEditReplay)
            {
                var folderPath = ReplayListTask.Path;
                DataFileMgr.Replay_RefreshFromDisk();
                DataFileMgr.TaskResult_Release(ReplayListTask.Id);
                LoadReplayList(folderPath);
            }
            else if (SelectedMode == QuadLoadMap)
            {
                var folderPath = MapListTask.Path;
                DataFileMgr.Map_RefreshFromDisk();
                DataFileMgr.TaskResult_Release(MapListTask.Id);
                LoadMapList(folderPath);
            }
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

        QuadCanyon.MouseClick += () =>
        {
            SendCustomEvent("NewMap", new[] { "Canyon" });
        };

        QuadStadium.MouseClick += () =>
        {
            SendCustomEvent("NewMap", new[] { "Stadium" });
        };

        QuadValley.MouseClick += () =>
        {
            SendCustomEvent("NewMap", new[] { "Valley" });
        };

        QuadLagoon.MouseClick += () =>
        {
            SendCustomEvent("NewMap", new[] { "Lagoon" });
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadItem")
            {
                var quad = (control as CMlQuad)!;

                var folder = quad.DataAttributeGet("folder");
                var file = quad.DataAttributeGet("file");

                if (SelectedMode == QuadEditReplay)
                {
                    if (file != "")
                    {
                        if (ReplaySelectedFilePaths.Contains(file))
                        {
                            ReplaySelectedFilePaths.Remove(file);
                        }
                        else
                        {
                            ReplaySelectedFilePaths.Add(file);
                        }
                    }
                    else
                    {
                        if (ReplaySelectedFolderPath == folder)
                        {
                            DataFileMgr.TaskResult_Release(ReplayListTask.Id);
                            LoadReplayList(ReplaySelectedFolderPath);
                            ReplaySelectedFolderPath = "";
                        }
                        else
                        {
                            ReplaySelectedFolderPath = folder;
                        }
                    }

                    UpdateReplayList();
                    UpdateReplayPanel();
                }
                else if (SelectedMode == QuadLoadMap)
                {
                    if (file != "")
                    {
                        if (MapSelectedFilePath == file)
                        {
                            SendCustomEvent("EditMap", new[] { MapSelectedFilePath });
                        }
                        else
                        {
                            MapSelectedFilePath = file;
                        }

                        MapSelectedFolderPath = "";
                    }
                    else
                    {
                        if (MapSelectedFolderPath == folder)
                        {
                            DataFileMgr.TaskResult_Release(MapListTask.Id);
                            LoadMapList(MapSelectedFolderPath);
                            MapSelectedFolderPath = "";
                        }
                        else
                        {
                            MapSelectedFolderPath = folder;
                        }

                        MapSelectedFilePath = "";
                    }

                    UpdateMapList();
                    UpdateMapPanel();
                }

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
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

        SelectedMode = QuadEditReplay;

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        LoadReplayList("");

        UpdateReplayList();
        UpdateReplayPanel();
    }

    public float PrevScrollY;

    public void Loop()
    {
        if (SelectedMode == QuadEditReplay && !ReplayListLoaded && ReplayListTask is not null && ReplayListTask.HasSucceeded)
        {
            UpdateReplayPanel();
            UpdateReplayList();
            ReplayListLoaded = true;
        }

        if (SelectedMode == QuadLoadMap && !MapListLoaded && MapListTask is not null && MapListTask.HasSucceeded)
        {
            UpdateMapPanel();
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

            if (SelectedMode == QuadEditReplay)
            {
                UpdateReplayList();
            }
            else if (SelectedMode == QuadLoadMap)
            {
                UpdateMapList();
            }
        }
    }

    private void LoadReplayList(string folderPath)
    {
        ReplayListTask = DataFileMgr.Replay_GetGameList(folderPath, false);
        ReplayListLoaded = false;
    }

    private void UpdateReplayList()
    {
        var totalCount = 0;
        var hasParentEntry = false;
        if (ReplayListTask is not null)
        {
            hasParentEntry = ReplayListTask.Path != "";
            totalCount = ReplayListTask.SubFolders.Count + ReplayListTask.ReplayInfos.Count;
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

            if (ReplayListTask is null)
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
                if (ReplayListTask.ParentPath == "")
                {
                    quadItem.DataAttributeSet("folder", "/");
                }
                else
                {
                    quadItem.DataAttributeSet("folder", ReplayListTask.ParentPath);
                }
                quadItem.DataAttributeSet("file", "");
                quadItem.StyleSelected = ReplaySelectedFolderPath != "" && ReplaySelectedFolderPath == ReplayListTask.ParentPath;
                frame.Visible = true;
                i += 1;
                continue;
            }

            if (adjustedIndex < ReplayListTask.SubFolders.Count)
            {
                var subFolder = ReplayListTask.SubFolders[adjustedIndex];

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
                quadItem.StyleSelected = ReplaySelectedFolderPath == subFolder;

                frame.Visible = true;
            }
            else if (adjustedIndex - ReplayListTask.SubFolders.Count < ReplayListTask.ReplayInfos.Count)
            {
                var itemIndex = adjustedIndex - ReplayListTask.SubFolders.Count;
                var mapName = ReplayListTask.ReplayInfos[itemIndex].Name;
                var mapUid = ReplayListTask.ReplayInfos[itemIndex].MapUid;
                var filePath = ReplayListTask.ReplayInfos[itemIndex].FileName;

                labelMapName.Value = mapName;
                quadIcon.Style = "";
                quadIcon.Substyle = "";
                quadIcon.ChangeImageUrl($"file://Thumbnails/MapUid/{mapUid}");
                quadEnv.ChangeImageUrl("");
                quadItem.DataAttributeSet("folder", "");
                quadItem.DataAttributeSet("file", filePath);
                quadItem.StyleSelected = ReplaySelectedFilePaths.Contains(filePath);

                frame.Visible = true;
            }
            else
            {
                frame.Visible = false;
            }

            i += 1;
        }
    }

    private void LoadMapList(string folderPath)
    {
        MapListTask = DataFileMgr.Map_GetFilteredGameList(4, folderPath, false);
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
                quadItem.StyleSelected = MapSelectedFolderPath != "" && MapSelectedFolderPath == MapListTask.ParentPath;
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
                quadItem.StyleSelected = MapSelectedFolderPath == subFolder;

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
                quadItem.StyleSelected = MapSelectedFilePath == filePath;

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

    private void UpdateReplayPanel()
    {
        LabelAction.Value = "Edit Replay";
        LabelPanelAuthor.Hide();
        LabelPanelCost.Value = "";
        if (ReplaySelectedFilePaths.Count == 0 || ReplayListTask is null)
        {
            FramePanelMap.Hide();
            QuadPanelMapThumbnail.Hide();
            QuadActionBase.Colorize = new Vec3(0.1, 0.1, 0.1);
            QuadAction.Hide();
        }
        else
        {
            if (ReplaySelectedFilePaths.Count > 1)
            {
                QuadPanelMapThumbnail.ChangeImageUrl("");
                QuadPanelMapThumbnail.Show();
                QuadPanelMapEnvironment.ChangeImageUrl("");
                LabelPanelMapName.Value = $"{ReplaySelectedFilePaths.Count} replays selected";
                QuadActionBase.Colorize = new Vec3(0, 1, 0);
                QuadAction.Show();
                FramePanelMap.Show();
                return;
            }

            foreach (var replayInfo in ReplayListTask.ReplayInfos)
            {
                if (ReplaySelectedFilePaths.Contains(replayInfo.FileName))
                {
                    QuadPanelMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{replayInfo.MapUid}");
                    QuadPanelMapThumbnail.Show();
                    QuadPanelMapEnvironment.ChangeImageUrl("");
                    LabelPanelMapName.Value = replayInfo.Name;
                    QuadActionBase.Colorize = new Vec3(0, 1, 0);
                    QuadAction.Show();
                    FramePanelMap.Show();
                    return;
                }
            }
        }
    }

    private void UpdateMapPanel()
    {
        LabelAction.Value = "Edit Map";
        LabelPanelAuthor.Show();

        if (MapSelectedFilePath == "" || MapListTask is null)
        {
            FramePanelMap.Hide();
            QuadPanelMapThumbnail.Hide();
            QuadActionBase.Colorize = new Vec3(0.1, 0.1, 0.1);
            QuadAction.Hide();
        }
        else
        {
            foreach (var mapInfo in MapListTask.MapInfos)
            {
                if (mapInfo.FileName == MapSelectedFilePath)
                {
                    QuadPanelMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                    QuadPanelMapThumbnail.Show();
                    QuadPanelMapEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{mapInfo.CollectionName}TMT.png");
                    LabelPanelMapName.Value = mapInfo.Name;
                    LabelPanelAuthor.Value = mapInfo.AuthorNickName;
                    LabelPanelCost.Value = $"{mapInfo.CopperPrice}cc";
                    QuadActionBase.Colorize = new Vec3(0, 1, 0);
                    QuadAction.Show();
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
