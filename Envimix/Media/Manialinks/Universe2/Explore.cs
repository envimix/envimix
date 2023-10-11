namespace Envimix.Media.Manialinks.Universe2;

public class Explore : CMapEditorPluginLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadTest;
    [ManialinkControl] public required CMlQuad QuadStartIcon;

    public bool Testing;

    public Explore()
    {
        QuadTest.MouseClick += () =>
        {
            Testing = !Testing;
            QuadTest.StyleSelected = Testing;
        };
    }

    public void Main()
    {
        foreach (var block in Editor.Blocks)
        {
            if (block.BlockModel.WaypointType == CBlockModel.EWayPointType.Start)
            {
                Log(block);
                QuadStartIcon.Image = block.BlockModel.Icon;
                break;
            }
        }
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