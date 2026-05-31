namespace Envimix.Scripts.EditorPlugins;

public class Explore : CMapEditorPlugin, IContext
{
    [Setting]
    public string OriginalMapName = "";

    [Setting]
    public string OriginalMapUid = "";

    [Setting]
    public string OriginalAuthorNickName = "";

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

        var originalMapAuthorNickName = Metadata<string>.For(Map);
        originalMapAuthorNickName.Set(OriginalAuthorNickName);
    }

    public void Loop()
    {

    }
}
