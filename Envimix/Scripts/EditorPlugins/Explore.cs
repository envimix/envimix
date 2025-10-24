namespace Envimix.Scripts.EditorPlugins;

public class Explore : CMapEditorPlugin, IContext
{
    [Setting]
    public string NewMapName = "";

    public void Main()
    {
        if (NewMapName != "")
        {
            Map.MapName = NewMapName;
        }

        var exploreMapName = Metadata<string>.For(Map);
        exploreMapName.Set(NewMapName);
    }

    public void Loop()
    {

    }
}
