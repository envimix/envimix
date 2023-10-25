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
                    if (e.CustomEventType == "Countdown" && e.CustomEventData.Count == 1 && Layers.ContainsKey("321Go") && e.CustomEventLayer == Layers["321Go"])
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
                    else if (e.CustomEventLayer == Layers["MultiplayerEvents"])
                    {
                        switch (e.CustomEventType)
                        {
                            case "Finish":
                                //Log(ScoreMgr.Playground_GetPlayerGhost());
                                break;
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

    public CUILayer CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml, string toReplace, string replaceWith)
    {
        if (Layers.ContainsKey(layerName))
        {
            DestroyLayer(layerName);
        }

        var layer = UILayerCreate();
        layer.Type = layerType;
        layer.ManialinkPage = TextLib.Replace(ReadFile(manialinkXml), toReplace, replaceWith);
        Layers[layerName] = layer;
        return layer;
    }

    public CUILayer CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml)
    {
        return CreateLayer(layerName, layerType, manialinkXml, "", "");
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
        CreateLayer("MultiplayerEvents", CUILayer.EUILayerType.Normal);
    }

    public void Loop()
    {

    }
}
