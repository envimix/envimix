using System.Collections.Immutable;

namespace Envimix.Media.ManiaApps;

public class EnvimixSingleplayerClient : CManiaAppPlayground, IContext
{
    public required Dictionary<string, CUILayer> Layers;
    public CAudioSourceMusic Music;

    public IList<string> Songs;

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

                    if (Layers.ContainsKey("321Go") && e.CustomEventLayer == Layers["321Go"])
                    {
                        if (e.CustomEventType == "Countdown" && e.CustomEventData.Count == 1)
                        {
                            if (e.CustomEventData[0] == "Start")
                            {
                                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 0, 1);
                                Music.LPF_CutoffRatio = 1;
                                Music.VolumedB = 1;
                            }
                            else
                            {
                                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 1, 1);
                            }
                        }
                    }

                    // 20% chance to continue the song, otherwise switch to another one
                    /*if (e.CustomEventType == "Car" && e.CustomEventData.Count >= 2 && e.CustomEventData[1] == "False")
                    {
                        Music.LPF_CutoffRatio = 1;
                    }*/

                    if (e.CustomEventType == "MenuOpen" && e.CustomEventData.Count == 1)
                    {
                        if (e.CustomEventData[0] == "True")
                        {
                            Music.LPF_CutoffRatio = 0.3f;
                        }
                        else if (e.CustomEventData[0] == "False")
                        {
                            Music.LPF_CutoffRatio = 1;
                        }
                    }

                    if (e.CustomEventType == "MusicSwitch")
                    {
                        PlayRandomSong();
                        Music.NextVariant();
                    }

                    break;
            }
        };
    }

    public void Main()
    {
        ClientUI.OverlayHide321Go = true;

        Log("Creating manialinks...");
        
        CreateLayer("321Go", CUILayer.EUILayerType.Normal);
        CreateLayer("Dashboard", CUILayer.EUILayerType.Normal);
        CreateLayer("Map", CUILayer.EUILayerType.Normal);
        CreateLayer("Checkpoint2", CUILayer.EUILayerType.Normal);
        CreateLayer("Notice", CUILayer.EUILayerType.Normal);
        CreateLayer("Stunt", CUILayer.EUILayerType.Normal);
        CreateLayer("Score", CUILayer.EUILayerType.Normal);
        CreateLayer("Endscreen", CUILayer.EUILayerType.Normal);
        CreateLayer("MusicPlayer", CUILayer.EUILayerType.Normal);

        var displayedCars = Netread<IList<string>>.For(Playground.Teams[0]);

        var vehicleManialink = $"<quad z-index=\"-1\" pos=\"0 {-displayedCars.Get().Count * 20 / 2}\" size=\"320 {displayedCars.Get().Count * 20 + 160}\" halign=\"center\" valign=\"center\" style=\"Bgs1InRace\" substyle=\"BgEmpty\" scriptevents=\"1\"/>";
        vehicleManialink = $"{vehicleManialink}<frame id=\"FrameInnerVehicles\">";

        for (var i = 0; i < displayedCars.Get().Count; i++)
        {
            var vehicle = displayedCars.Get()[i];
            vehicleManialink = $"{vehicleManialink}    <frame pos=\"0 {-i * 20}\" data-id=\"{i}\" data-car=\"{vehicle}\">";
            vehicleManialink = $"{vehicleManialink}        <frame z-index=\"0\" id=\"FrameBackground\">";
            vehicleManialink = $"{vehicleManialink}            <quad z-index=\"0\" size=\"80 19\" valign=\"center\" halign=\"center\" style=\"Bgs1\" substyle=\"BgCardList\" opacity=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        </frame>";
            vehicleManialink = $"{vehicleManialink}        <quad z-index=\"1\" size=\"80 19\" id=\"QuadVehicle\" valign=\"center\" halign=\"center\" style=\"Bgs1\" substyle=\"BgCardInventoryItem\" scriptevents=\"1\" modulatecolor=\"036\" opacity=\".5\"/>";
            vehicleManialink = $"{vehicleManialink}        <quad pos=\"-34.5 5\" z-index=\"2\" size=\"7.5 7.5\" halign=\"center\" valign=\"center\" image=\"file://Media/Images/Cars/{vehicle}.png\"/>";
            vehicleManialink = $"{vehicleManialink}        <label pos=\"0 -0.5\" z-index=\"2\" size=\"70 10\" text=\"{vehicle}\" halign=\"center\" valign=\"center2\" textsize=\"6\" textfont=\"RajdhaniMono\" id=\"LabelVehicle\"/>";
            vehicleManialink = $"{vehicleManialink}        <label pos=\"37.5 -8\" z-index=\"2\" size=\"75 5\" text=\"Default\" textprefix=\"$t\" halign=\"right\" valign=\"bottom\" textfont=\"Oswald\" textsize=\"2\" textcolor=\"FF0\" id=\"LabelDefault\" translate=\"1\" hidden=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        <quad pos=\"35 5\" z-index=\"3\" size=\"7.5 7.5\" halign=\"center\" valign=\"center\" style=\"BgRaceScore2\" substyle=\"Fame\" id=\"QuadStar\" hidden=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        <gauge id=\"GaugeDifficulty\" pos=\"-40 -5\" z-index=\"3\" size=\"11 6.5\" drawbg=\"0\" valign=\"center\" ratio=\"0\"/>";
            vehicleManialink = $"{vehicleManialink}        <gauge id=\"GaugeQuality\" pos=\"-40 -7\" z-index=\"3\" size=\"11 6.5\" drawbg=\"0\" valign=\"center\" ratio=\"0\"/>";
            vehicleManialink = $"{vehicleManialink}        <quad id=\"QuadFlash\" z-index=\"4\" size=\"80 19\" valign=\"center\" halign=\"center\" style=\"Bgs1\" substyle=\"BgWindow4\" opacity=\"0\"/>";
            vehicleManialink = $"{vehicleManialink}     </frame>";
        }

        vehicleManialink = $"{vehicleManialink}</frame>";

        CreateLayer("Menu", CUILayer.EUILayerType.InGameMenu, "<frame id=\"FrameInnerVehicles\" />", vehicleManialink);

        Log("All manialinks successfully created!");
        
        if (LoadedTitle.TitleId == "Envimix_Turbo@bigbang1112")
        {
            if (MathLib.Rand(0, 3) == 0)
            {
                Audio.PlaySoundEvent("file://Media/Sounds/Voices/voice-welcome.wav", 1);
            }

            PlayRandomSong();
        }
    }

    private void PlayRandomSong()
    {
        Songs = new[] { "In The Past", "Grown", "These Times", "The Only Thing I Miss (Is You)", "Warning", "Final Thing To See" };

        var randomSongPick = Songs[MathLib.Rand(0, Songs.Count - 1)];

        Music = Audio.CreateMusic($"file://Media/Musics/Album/Loops/{randomSongPick}.zip");
        Music.EnableSegment("loop");
        Music.FadeDuration = .35f;
        Music.FadeTracksDuration = 1;
        Music.FadeFiltersDuration = 1.5f;
        Music.UpdateMode = CAudioSourceMusic.EUpdateMode.OnNextBeat;
        Music.Volume = 1f;
        Music.LPF_CutoffRatio = 0.3f;
        Music.LPF_Q = 8;

        Music.Play();
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
