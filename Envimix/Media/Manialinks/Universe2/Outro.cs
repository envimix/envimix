using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Outro : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlQuad QuadSaveReplayBg;
    [ManialinkControl] public required CMlQuad QuadSaveReplay;
    [ManialinkControl] public required CMlQuad QuadPrevMap;
    [ManialinkControl] public required CMlQuad QuadPrevCar;
    [ManialinkControl] public required CMlQuad QuadContinue;
    [ManialinkControl] public required CMlQuad QuadNextCar;
    [ManialinkControl] public required CMlQuad QuadNextMap;
    [ManialinkControl] public required CMlQuad QuadExit;

    [ManialinkControl] public required CMlLabel LabelMapName;
    [ManialinkControl] public required CMlLabel LabelMapAuthor;

    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }
    [Netwrite(NetFor.UI)] public string ClientCar { get; set; }

    [Local(LocalFor.LocalUser)] public string EnvimixOpenMapUid { get; set; } = "";

    [Netread] public bool ReplaySaved { get; set; }

    public CMlQuad? SelectedButton;

    public Outro()
    {
        QuadSaveReplay.MouseClick += () =>
        {
            SaveReplay();
        };

        QuadPrevMap.MouseClick += () =>
        {
            PreviousMap();
        };

        QuadPrevCar.MouseClick += () =>
        {
            PreviousCar();
        };

        QuadContinue.MouseClick += () =>
        {
            Continue();
        };

        QuadNextCar.MouseClick += () =>
        {
            NextCar();
        };

        QuadNextMap.MouseClick += () =>
        {
            NextMap();
        };

        QuadExit.MouseClick += () =>
        {
            Exit();
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "OutroVisible":
                    if (data[0] == "True")
                    {
                        SelectedButton = QuadContinue;
                        SelectedButton.StyleSelected = true;
                    }
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Select:
                    SelectedButton.StyleSelected = false;
                    if (SelectedButton == QuadSaveReplay)
                    {
                        SaveReplay();
                    }
                    else if (SelectedButton == QuadPrevMap)
                    {
                        PreviousMap();
                    }
                    else if (SelectedButton == QuadPrevCar)
                    {
                        PreviousCar();
                    }
                    else if (SelectedButton == QuadContinue)
                    {
                        Continue();
                    }
                    else if (SelectedButton == QuadNextCar)
                    {
                        NextCar();
                    }
                    else if (SelectedButton == QuadNextMap)
                    {
                        NextMap();
                    }
                    else if (SelectedButton == QuadExit)
                    {
                        Exit();
                    }
                    break;
                case CMlScriptEvent.EMenuNavAction.Left:
                    if (SelectedButton is null)
                    {
                        SelectedButton = QuadContinue;
                        SelectedButton.StyleSelected = true;
                        break;
                    }

                    SelectedButton.StyleSelected = false;

                    if (SelectedButton == QuadContinue)
                    {
                        SelectedButton = QuadPrevCar;
                    }
                    else if (SelectedButton == QuadPrevCar)
                    {
                        SelectedButton = QuadPrevMap;
                    }
                    else if (SelectedButton == QuadPrevMap)
                    {
                        SelectedButton = QuadSaveReplay;
                    }
                    else if (SelectedButton == QuadSaveReplay)
                    {
                        SelectedButton = QuadExit;
                    }
                    else if (SelectedButton == QuadExit)
                    {
                        SelectedButton = QuadNextMap;
                    }
                    else if (SelectedButton == QuadNextMap)
                    {
                        SelectedButton = QuadNextCar;
                    }
                    else if (SelectedButton == QuadNextCar)
                    {
                        SelectedButton = QuadContinue;
                    }

                    SelectedButton.StyleSelected = true;
                    break;
                case CMlScriptEvent.EMenuNavAction.Right:
                    if (SelectedButton is null)
                    {
                        SelectedButton = QuadContinue;
                        SelectedButton.StyleSelected = true;
                        break;
                    }

                    SelectedButton.StyleSelected = false;

                    if (SelectedButton == QuadContinue)
                    {
                        SelectedButton = QuadNextCar;
                    }
                    else if (SelectedButton == QuadNextCar)
                    {
                        SelectedButton = QuadNextMap;
                    }
                    else if (SelectedButton == QuadNextMap)
                    {
                        SelectedButton = QuadExit;
                    }
                    else if (SelectedButton == QuadExit)
                    {
                        SelectedButton = QuadSaveReplay;
                    }
                    else if (SelectedButton == QuadSaveReplay)
                    {
                        SelectedButton = QuadPrevMap;
                    }
                    else if (SelectedButton == QuadPrevMap)
                    {
                        SelectedButton = QuadPrevCar;
                    }
                    else if (SelectedButton == QuadPrevCar)
                    {
                        SelectedButton = QuadContinue;
                    }

                    SelectedButton.StyleSelected = true;
                    break;
            }
        };
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public void Main()
    {
        EnableMenuNavigationInputs = true;

        SelectedButton = QuadContinue;
    }

    public void Loop()
    {
        LabelMapName.Value = Map.MapInfo.Name;
        LabelMapAuthor.Value = Map.MapInfo.AuthorNickName;

        if (ReplaySaved)
        {
            QuadSaveReplayBg.ModulateColor = new Vec3(0.125f, 0.125f, 0.125f);
            QuadSaveReplay.Visible = false;
        }
        else
        {
            QuadSaveReplayBg.ModulateColor = new Vec3(0, 0.1875f, 0.375f);
            QuadSaveReplay.Visible = true;
        }
    }

    private void SaveReplay()
    {
        SendCustomEvent("SaveOutroReplay", new[] { "" });
    }

    private ImmutableArray<string> GetAllMaps()
    {
        ImmutableArray<string> allMaps = new();
        foreach (var campaign in DataFileMgr.Campaigns)
        {
            foreach (var mapGroup in campaign.MapGroups)
            {
                for (var i = 0; i < mapGroup.MapInfos.Count; i++)
                {
                    var mapInfo = mapGroup.MapInfos[i];
                    allMaps.Add(mapInfo.MapUid);
                }
            }
        }
        return allMaps;
    }

    private void Exit()
    {
        Playground.QuitServer(true);
    }

    private void PreviousMap()
    {
        var allMaps = GetAllMaps();
        var currentIndex = allMaps.IndexOf(Map.MapInfo.MapUid);
        if (currentIndex > 0)
        {
            EnvimixOpenMapUid = allMaps[currentIndex - 1];
        }
        else
        {
            EnvimixOpenMapUid = allMaps[allMaps.Length - 1];
        }
        Exit();
    }

    private void Continue()
    {
        SendCustomEvent("OutroContinue", new[] { "" });

        // reset button to Continue state
        SelectedButton = QuadContinue;
        SelectedButton.StyleSelected = true;
    }

    private void PreviousCar()
    {
        var car = Netread<string>.For(GetPlayer());
        var currentIndex = DisplayedCars.IndexOf(car.Get());

        if (currentIndex > 0)
        {
            ClientCar = DisplayedCars[currentIndex - 1];
        }
        else
        {
            ClientCar = DisplayedCars[DisplayedCars.Length - 1];
        }

        Continue();
    }

    private void NextCar()
    {
        var car = Netread<string>.For(GetPlayer());
        var currentIndex = DisplayedCars.IndexOf(car.Get());

        if (currentIndex < DisplayedCars.Length - 1)
        {
            ClientCar = DisplayedCars[currentIndex + 1];
        }
        else
        {
            ClientCar = DisplayedCars[0];
        }

        Continue();
    }

    private void NextMap()
    {
        var allMaps = GetAllMaps();
        var currentIndex = allMaps.IndexOf(Map.MapInfo.MapUid);
        if (currentIndex < allMaps.Length - 1)
        {
            EnvimixOpenMapUid = allMaps[currentIndex + 1];
        }
        else
        {
            EnvimixOpenMapUid = allMaps[0];
        }
        Exit();
    }
}
