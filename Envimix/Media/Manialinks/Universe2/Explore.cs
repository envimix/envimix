namespace Envimix.Media.Manialinks.Universe2;

public class Explore : CMapEditorPluginLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadTest;
    [ManialinkControl] public required CMlQuad QuadStart;
    [ManialinkControl] public required CMlQuad QuadStartIcon;

    public bool Testing;

    public Explore()
    {
        QuadTest.MouseClick += () =>
        {
            Testing = !Testing;
            QuadTest.StyleSelected = Testing;
        };

        QuadStart.MouseClick += () =>
        {
            MoveToStart();
        };
    }

    public void Main()
    {
        
    }

    private void MoveToStart()
    {
        Vec3 startPos = new(0, 0, 0);

        foreach (var block in Editor.Blocks)
        {
            if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Start)
            {
                QuadStartIcon.Image = block.BlockModel.Icon;
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
        if (Testing)
        {
            Editor.PlaceModeE = CMapEditorPlugin.PlaceMode.Test;
            Editor.EditModeE = CMapEditorPlugin.EditMode.Place;
        }
        else
        {
            Editor.PlaceModeE = CMapEditorPlugin.PlaceMode.Block;
            Editor.EditModeE = CMapEditorPlugin.EditMode.FreeLook;
        }
    }
}