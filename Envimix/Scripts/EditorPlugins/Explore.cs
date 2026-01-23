namespace Envimix.Scripts.EditorPlugins;

public class Explore : CMapEditorPlugin, IContext
{
    [Setting]
    public string OriginalMapName = "";

    [Setting]
    public string OriginalMapUid = "";

    public void Main()
    {
        if (OriginalMapName != "")
        {
            Map.MapName = OriginalMapName;
        }

        var exploreMapName = Metadata<string>.For(Map);
        exploreMapName.Set(OriginalMapName);

        var originalMapUid = Metadata<string>.For(Map);
        originalMapUid.Set(OriginalMapUid);
    }

    public void Loop()
    {

    }
}
