using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Explore : CMapEditorPluginLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadTest;
    [ManialinkControl] public required CMlQuad QuadStart;
    [ManialinkControl] public required CMlQuad QuadStartIcon;
    [ManialinkControl] public required CMlLabel LabelStart;
    [ManialinkControl] public required CMlQuad QuadSwitchEditor;
    [ManialinkControl] public required CMlLabel LabelSwitchEditor;
    [ManialinkControl] public required CMlFrame FrameExplore;
    [ManialinkControl] public required CMlLabel LabelCp;
    [ManialinkControl] public required CMlQuad QuadCpIcon;
    [ManialinkControl] public required CMlLabel LabelFinish;
    [ManialinkControl] public required CMlQuad QuadFinishIcon;

    public bool Testing;
    public bool NormalEditor;

    public Explore()
    {
        QuadTest.MouseClick += () =>
        {
            SetTesting(!Testing);
        };

        QuadSwitchEditor.MouseClick += () =>
        {
            NormalEditor = !NormalEditor;
            QuadSwitchEditor.StyleSelected = NormalEditor;
            Editor.HideEditorInterface = !NormalEditor;

            if (NormalEditor)
            {
                LabelSwitchEditor.SetText("Switch to Explore mode");
                SetTesting(false);
            }
            else
            {
                LabelSwitchEditor.SetText("Switch to normal editor");
            }

            FrameExplore.Visible = !NormalEditor;
        };

        QuadStart.MouseClick += () =>
        {
            MoveToStart();
        };

        PluginCustomEvent += (eventName, eventParams) =>
        {
            switch (eventName)
            {
                case "StartTest":
                    //SetTesting(false);
                    break;
            }
        };
    }

    public void Main()
    {
        SetupWaypoints();
    }

    private void SetTesting(bool testing)
    {
        Testing = testing;
        QuadTest.StyleSelected = testing;
    }

    private void MoveToStart()
    {
        Vec3 startPos = new(0, 0, 0);
        
        foreach (var block in Editor.Blocks)
        {
            if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Start)
            {
                startPos = Editor.GetVec3FromCoord(block.Coord);
                break;
            }
        }

        foreach (var anchor in Editor.AnchorData)
        {
            if (anchor.WaypointType == CAnchorData.EWaypointType.Start)
            {
                startPos = anchor.Position;
                break;
            }
        }

        Editor.CameraTargetPosition = startPos;
        Editor.Camera.Zoom(CMapEditorCamera.EZoomLevel.Close, Smooth: true);
    }

    public void Loop()
    {
        QuadStartIcon.Hide();
        LabelStart.Show();


        if (Input.IsKeyPressed(36))
        {
            SetTesting(false);
        }

        if (Testing)
        {
            Editor.PlaceModeE = CMapEditorPlugin.PlaceMode.Test;
            Editor.EditModeE = CMapEditorPlugin.EditMode.Place;
        }
        else if (!NormalEditor)
        {
            Editor.PlaceModeE = CMapEditorPlugin.PlaceMode.Block;
            Editor.EditModeE = CMapEditorPlugin.EditMode.FreeLook;
        }
    }

    private void SetupWaypoints()
    {
        var start = "";
        ImmutableArray<CBlock> cps = new();
        ImmutableArray<CBlock> finishes = new();

        foreach (var block in Editor.Blocks)
        {
            if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Start && start is "")
            {
                QuadStartIcon.Show();
                QuadStartIcon.Image = block.BlockModel.Icon;
                LabelStart.Hide();
                start = block.BlockModel.Name;
            }
            else if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Checkpoint)
            {
                if (cps.Length == 0)
                {
                    QuadCpIcon.Show();
                    QuadCpIcon.Image = block.BlockModel.Icon;
                    LabelCp.Hide();
                }

                cps.Add(block);
            }
            else if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Finish)
            {
                if (finishes.Length == 0)
                {
                    QuadFinishIcon.Show();
                    QuadFinishIcon.Image = block.BlockModel.Icon;
                    LabelFinish.Hide();
                }

                finishes.Add(block);
            }
        }
    }
}