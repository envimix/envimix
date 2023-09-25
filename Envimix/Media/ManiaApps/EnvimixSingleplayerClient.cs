using System.Collections.Immutable;

namespace Envimix.Media.ManiaApps;

public class EnvimixSingleplayerClient : CManiaAppPlayground, IContext
{
    public required Dictionary<string, CUILayer> Layers;

    public EnvimixSingleplayerClient()
    {
        PendingEvent += (e) =>
        {
            switch (e.Type)
            {
                case CManiaAppPlaygroundEvent.EType.LayerCustomEvent:
                    ImmutableArray<string> data = new();

                    foreach (var d in e.CustomEventData)
                    {
                        data.Add(d);
                    }

                    SendCustomEvent(e.CustomEventType, data.ToArray());
                    break;
            }
        };
    }

    public void Main()
    {
        Log("Creating manialinks...");
        
        CreateLayer("321Go", CUILayer.EUILayerType.Normal);
        CreateLayer("Dashboard", CUILayer.EUILayerType.Normal);
        CreateLayer("Map", CUILayer.EUILayerType.Normal);
        CreateLayer("Checkpoint", CUILayer.EUILayerType.Normal);
        CreateLayer("Notice", CUILayer.EUILayerType.Normal);
        CreateLayer("Stunt", CUILayer.EUILayerType.Normal);
        //CreateLayer("MusicPlayer", CUILayer.EUILayerType.Normal);

        var displayedCars = Netread<IList<string>>.For(Playground.Teams[0]);

        var vehicleManialink = $"<quad z-index=\"-1\" pos=\"0 {-displayedCars.Get().Count * 20 / 2}\" size=\"320 {displayedCars.Get().Count * 20 + 160}\" halign=\"center\" valign=\"center\" style=\"Bgs1InRace\" substyle=\"BgEmpty\" scriptevents=\"1\"/>";
        vehicleManialink = $"{vehicleManialink}<frame id=\"FrameInnerVehicles\">";

        for (var i = 0; i < displayedCars.Get().Count; i++)
        {
            var vehicle = displayedCars.Get()[i];
            vehicleManialink = $"{vehicleManialink}    <frame pos=\"0 {-i * 20}\" data-id=\"{i}\">";
            vehicleManialink = $"{vehicleManialink}        <frame z-index=\"0\" id=\"FrameBackground\">";
            vehicleManialink = $"{vehicleManialink}            <quad z-index=\"0\" size=\"80 19\" valign=\"center\" halign=\"center\" style=\"Bgs1\" substyle=\"BgCardList\" opacity=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        </frame>";
            vehicleManialink = $"{vehicleManialink}        <quad z-index=\"1\" size=\"80 19\" id=\"QuadVehicle\" valign=\"center\" halign=\"center\" style=\"Bgs1\" substyle=\"BgCardInventoryItem\" scriptevents=\"1\" modulatecolor=\"036\" opacity=\".5\"/>";
            vehicleManialink = $"{vehicleManialink}        <label pos=\"0 -0.5\" z-index=\"2\" size=\"70 10\" text=\"{vehicle}\" halign=\"center\" valign=\"center2\" textsize=\"6\" textfont=\"RajdhaniMono\" id=\"LabelVehicle\"/>";
            vehicleManialink = $"{vehicleManialink}        <label pos=\"37.5 -8\" z-index=\"2\" size=\"75 5\" text=\"Default\" textprefix=\"$t\" halign=\"right\" valign=\"bottom\" textfont=\"Oswald\" textsize=\"2\" textcolor=\"FF0\" id=\"LabelDefault\" translate=\"1\" hidden=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        <quad pos=\"35 5\" z-index=\"3\" size=\"7.5 7.5\" halign=\"center\" valign=\"center\" style=\"BgRaceScore2\" substyle=\"Fame\" id=\"QuadStar\" hidden=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}     </frame>";
        }

        vehicleManialink = $"{vehicleManialink}</frame>";

        CreateLayer("Menu", CUILayer.EUILayerType.InGameMenu, "<frame id=\"FrameInnerVehicles\" />", vehicleManialink);

        Log("All manialinks successfully created!");
    }

    public void Loop()
    {

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

    public void DestroyLayer(string layerName)
    {
        UILayerDestroy(Layers[layerName]);
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml, string toReplace, string replaceWith)
    {
        if (Layers.ContainsKey(layerName))
        {
            DestroyLayer(layerName);
        }

        var layer = UILayerCreate();
        layer.Type = layerType;
        layer.ManialinkPage = TextLib.Replace(ReadFile(manialinkXml), toReplace, replaceWith);
        Layers[layerName] = layer;
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml)
    {
        CreateLayer(layerName, layerType, manialinkXml, "", "");
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType)
    {
        Log("Creating layer " + layerName + "...");
        CreateLayer(layerName, layerType, $"Manialinks/Universe2/{layerName}.xml");
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType, string toReplace, string replaceWith)
    {
        Log("Creating layer " + layerName + "...");
        CreateLayer(layerName, layerType, $"Manialinks/Universe2/{layerName}.xml", toReplace, replaceWith);
    }
}
