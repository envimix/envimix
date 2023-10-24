namespace Envimix.Media.ManiaApps;

public class EnvimixMultiplayerClient : CManiaAppPlayground, IContext
{
    public required Dictionary<string, CUILayer> Layers;

    public EnvimixMultiplayerClient()
    {
        PendingEvent += (e) =>
        {
            switch (e.Type)
            {
                case CManiaAppPlaygroundEvent.EType.LayerCustomEvent:
                    if (Layers.ContainsKey("321Go") && e.CustomEventLayer == Layers["321Go"])
                    {
                        if (e.CustomEventType == "Countdown" && e.CustomEventData.Count == 1)
                        {
                            if (e.CustomEventData[0] == "Start")
                            {
                                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 0, 1);
                            }
                            else
                            {
                                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 1, 1);
                            }
                        }
                    }
                    break;
            }
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

    public void Main()
    {
        ClientUI.OverlayHide321Go = true;

        CreateLayer("321Go", CUILayer.EUILayerType.Normal);
    }

    public void Loop()
    {
        
    }
}
