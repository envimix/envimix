namespace Envimix.Scripts.MapTypes.TrackMania;

public class EnvimixExplore : CTmMapType, IContext
{
    [Setting] public string EnvimixWebAPI = "https://api.envimix.gbx.tools";

    public CUILayer ExploreLayer;

    public CHttpRequest? VisitMapRequest;

    public EnvimixExplore()
    {
        StartTest += () =>
        {
            TestMapWithMode("Scripts/Modes/TrackMania/EnvimixSolo.Script.txt", "<mode_script_settings><setting name=\"S_MenuAsNormalLayer\" type=\"boolean\" value=\"1\" /><setting name=\"S_CustomCountdown\" type=\"integer\" value=\"0\" /><setting name=\"S_EnableDefaultCar\" type=\"boolean\" value=\"1\"/><setting name=\"S_ExploreMode\" type=\"boolean\" value=\"1\"/></mode_script_settings>");
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
        var originalMapUid = Metadata<string>.For(Map);
        if (VisitMapRequest is null && originalMapUid.Get() != "")
        {
            var envimixTurboUserToken = Local<string>.For(LocalUser);
            VisitMapRequest = Http.CreatePost($"{EnvimixWebAPI}/maps/{originalMapUid.Get()}", "", $"Authorization: Bearer {envimixTurboUserToken.Get()}");
        }

        if (VisitMapRequest is not null && VisitMapRequest.IsCompleted)
        {
            var forceQuit = false;

            if (VisitMapRequest.StatusCode != 200)
            {
                forceQuit = true;
            }

            Http.Destroy(VisitMapRequest);
            VisitMapRequest = null;

            // quit like this so that the http request is destroyed and wont cause an overflow
            if (forceQuit)
            {
                Log("Forcing quit due to authorization failure.");
                QuickQuit();
            }
        }
    }
}
