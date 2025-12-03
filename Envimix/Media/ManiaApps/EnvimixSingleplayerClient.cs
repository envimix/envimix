using System.Collections.Immutable;

namespace Envimix.Media.ManiaApps;

public class EnvimixSingleplayerClient : CManiaAppPlayground, IContext
{
    public CAudioSourceMusic? Music;
    public IList<string> Songs;

    public CUILayer Layer321Go;
    public CUILayer LayerDashboard;
    public CUILayer LayerMap;
    public CUILayer LayerScore;
    public CUILayer LayerMenu;
    public CUILayer LayerMenuFake;
    public CUILayer LayerOutro;

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

                    if (e.CustomEventLayer == Layer321Go)
                    {
                        if (e.CustomEventType == "Countdown" && e.CustomEventData.Count == 1)
                        {
                            if (e.CustomEventData[0] == "Start")
                            {
                                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 0, 1);

                                if (Music is not null)
                                {
                                    Music.LPF_CutoffRatio = 1;
                                    Music.VolumedB = 1;
                                }
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
                        var menuOpen = e.CustomEventData[0] == "True";

                        if (IsMenuAsNormalLayer())
                        {
                            var layersToToggle = new[] { LayerDashboard, LayerMap, LayerScore };
                            foreach (var layer in layersToToggle)
                            {
                                LayerCustomEvent(layer, "MenuOpen", new[] { e.CustomEventData[0] });
                            }
                            LayerMenu.IsVisible = menuOpen;
                        }

                        if (Music is not null)
                        {
                            if (menuOpen)
                            {
                                Music.LPF_CutoffRatio = 0.3f;
                            }
                            else
                            {
                                Music.LPF_CutoffRatio = 1;
                            }
                        }
                    }

                    if (Music is not null && e.CustomEventType == "MusicSwitch")
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
        
        Layer321Go = CreateLayer("321Go", CUILayer.EUILayerType.Normal);
        LayerDashboard = CreateLayer("Dashboard", CUILayer.EUILayerType.Normal);
        LayerMap = CreateLayer("Map", CUILayer.EUILayerType.Normal);
        CreateLayer("Checkpoint2", CUILayer.EUILayerType.Normal);
        CreateLayer("Notice", CUILayer.EUILayerType.Normal);
        CreateLayer("Stunt", CUILayer.EUILayerType.Normal);
        LayerScore = CreateLayer("Score", CUILayer.EUILayerType.Normal);
        CreateLayer("Endscreen", CUILayer.EUILayerType.Normal);
        CreateLayer("MusicPlayer", CUILayer.EUILayerType.Normal);
        CreateLayer("Rating", CUILayer.EUILayerType.Normal);
        CreateLayer("RatingSolo", CUILayer.EUILayerType.Normal);
        CreateLayer("Multilap", CUILayer.EUILayerType.Normal);
        LayerOutro = CreateLayer("Outro", CUILayer.EUILayerType.Normal);
        LayerOutro.IsVisible = false;

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
            vehicleManialink = $"{vehicleManialink}        <label id=\"LabelSkillpoints\" textcolor=\"0F0\" textfont=\"RajdhaniMono\" pos=\"-29 6.5\" z-index=\"3\" size=\"15 5\" text=\"0\" valign=\"center\" halign=\"left\" textsize=\"1\" scale=\"0.75\" textemboss=\"1\" hidden=\"1\"/>";
            vehicleManialink = $"{vehicleManialink}        <label id=\"LabelActivityPoints\" textcolor=\"0FF\" textfont=\"RajdhaniMono\" pos=\"-29 4\" z-index=\"3\" size=\"15 5\" text=\"0\" valign=\"center\" halign=\"left\" textsize=\"1\" scale=\"0.75\" textemboss=\"1\" hidden=\"1\"/>";

            vehicleManialink = $"{vehicleManialink}     </frame>";
        }

        vehicleManialink = $"{vehicleManialink}</frame>";

        if (IsMenuAsNormalLayer())
        {
            Log("Menu layer type: Normal");
            LayerMenu = CreateLayer("Menu", CUILayer.EUILayerType.Normal, "<frame id=\"FrameInnerVehicles\" />", vehicleManialink);
        }
        else
        {
            Log("Menu layer type: InGameMenu");
            LayerMenu = CreateLayer("Menu", CUILayer.EUILayerType.InGameMenu, "<frame id=\"FrameInnerVehicles\" />", vehicleManialink);
        }

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

    bool IsMenuAsNormalLayer()
    {
        var menuAsNormalLayer = Netread<bool>.For(Playground.Teams[0]);
        return menuAsNormalLayer.Get();
    }

    private void PlayRandomSong()
    {
        Songs = new[] { "In The Past", "Grown", "These Times", "The Only Thing I Miss (Is You)", "Warning", "Passion", "Final Thing To See" };

        var randomSongPick = Songs[MathLib.Rand(0, Songs.Count - 1)];

        /*Music = Audio.CreateMusic($"file://Media/Musics/Album/Loops/{randomSongPick}.zip");
        Music.EnableSegment("loop");
        Music.FadeDuration = .35f;
        Music.FadeTracksDuration = 1;
        Music.FadeFiltersDuration = 1.5f;
        Music.UpdateMode = CAudioSourceMusic.EUpdateMode.OnNextBeat;
        Music.Volume = 1f;
        Music.LPF_CutoffRatio = 0.3f;
        Music.LPF_Q = 8;*/

        //Music.Play();
    }

    public int PrevFinishedAt = -1;
    public bool PrevOutro = false;

    public void Loop()
    {
        // cheese to disallow menu during endscreen
        var finishedAt = Netread<int>.For(Playground.Teams[0]);
        if (finishedAt.Get() != PrevFinishedAt)
        {
            PrevFinishedAt = finishedAt.Get();
            var isEndscreen = finishedAt.Get() != -1;

            if (isEndscreen)
            {
                LayerMenuFake = CreateLayer("MenuFake", CUILayer.EUILayerType.InGameMenu);
                LayerMenu.Type = CUILayer.EUILayerType.Normal;
                LayerMenu.IsVisible = false;
            }
            else
            {
                UILayerDestroy(LayerMenuFake);
                LayerMenu.Type = CUILayer.EUILayerType.InGameMenu;
                LayerMenu.IsVisible = true;
            }
        }

        var outro = Netread<bool>.For(Playground.Teams[0]);
        if (outro.Get() != PrevOutro)
        {
            PrevOutro = outro.Get();
            LayerOutro.IsVisible = outro.Get();
            SendCustomEvent("OutroVisible", new[] { outro.Get().ToString() });
        }
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

    public CUILayer CreateLayer(CUILayer.EUILayerType layerType, string manialinkXml, string toReplace, string replaceWith)
    {
        var layer = UILayerCreate();
        layer.Type = layerType;
        layer.ManialinkPage = TextLib.Replace(ReadFile(manialinkXml), toReplace, replaceWith);
        return layer;
    }

    public CUILayer CreateLayer(CUILayer.EUILayerType layerType, string manialinkXml)
    {
        return CreateLayer(layerType, manialinkXml, "", "");
    }

    public CUILayer CreateLayer(string layerName, CUILayer.EUILayerType layerType)
    {
        Log($"Creating layer {layerName}...");
        return CreateLayer(layerType, $"Manialinks/Universe2/{layerName}.xml");
    }

    public CUILayer CreateLayer(string layerName, CUILayer.EUILayerType layerType, string toReplace, string replaceWith)
    {
        Log($"Creating layer {layerName}...");
        return CreateLayer(layerType, $"Manialinks/Universe2/{layerName}.xml", toReplace, replaceWith);
    }
}
