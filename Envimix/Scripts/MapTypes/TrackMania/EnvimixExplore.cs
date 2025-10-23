namespace Envimix.Scripts.MapTypes.TrackMania;

public class EnvimixExplore : CTmMapType, IContext
{
    public CUILayer ExploreLayer;

    public EnvimixExplore()
    {
        StartTest += () =>
        {
            TestMapWithMode("Scripts/Modes/TrackMania/EnvimixSolo.Script.txt",);
            LayerCustomEvent(ExploreLayer, "StartTest", new[] { "" });
        };
    }

    public string ReadFile(string fileName)
    {
        var request = Http.CreateGet("file://Media/" + fileName);
        Wait(() => request.IsCompleted);

        var result = request.Result;
        if (result == "")
        {
            Log("Warning: File located in file://Media/" + fileName + " does not exist or is empty.");
        }

        Http.Destroy(request);
        return result;
    }

    public void Main()
    {
        HideEditorInterface = true;
        EnableMapTypeStartTest = true;

        ExploreLayer = UILayerCreate();
        ExploreLayer.ManialinkPage = ReadFile("Manialinks/Universe2/Explore.xml");
    }

    public void Loop()
    {

    }
}
