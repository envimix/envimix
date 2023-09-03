namespace Envimix.Scripts.Modes.TrackMania;

public class Envimix : UniverseModeBase
{
    public struct SSkin
    {
	    public string File;
        public string Icon;
    }

    [Setting(As = "Enable TM2 cars")]
    public bool EnableTM2Cars = true;

    [Setting(As = "Enable United cars")]
    public bool EnableUnitedCars = false;

    [Setting(As = "Enable default car")]
    public bool EnableDefaultCar = false;

    [Setting(As = "Enable custom cars")]
    public bool EnableCustomCars = false;

    [Setting(As = "* Enable Stadium envimix")]
    public bool EnableStadiumEnvimix = false; // Wrong usage can crash scripts

    [Setting(As = "* Use United models")]
    public bool UseUnitedModels = false; // Wrong usage can crash scripts

    [Setting(As = "* Vehicle folder")]
    public string VehicleFolder = "Vehicles/"; // Wrong usage can crash scripts

    [Setting(As = "* Vehicle file format")]
    public string VehicleFileFormat = "%1.Item.Gbx"; // Wrong usage can crash scripts

    [Setting(As = "* Cars.json file")]
    public string CarsFile = "Cars.json";

    [Setting(As = "* Skins.json file")]
    public string SkinsFile = "";

    [Setting(As = "Use skillpoints")]
    public bool UseSkillpoints = false;

    [Setting(As = "Use ladder")]
    public bool UseLadder = true;

    [Setting(As = "Allow respawn")]
    public bool AllowRespawn = true;

    public required Dictionary<string, Dictionary<string, Ident>> Cars;
    public required Dictionary<string, Dictionary<string, Ident>> SpecialCars;
    public required Dictionary<string, Dictionary<string, Ident>> UnitedCars;
    public required Dictionary<string, Dictionary<string, Ident>> CustomCars;

    [Netwrite] public required IList<string> DisplayedCars { get; set; }
    [Netwrite] public required Dictionary<string, string> ItemCars { get; set; }
    [Netwrite] public required Dictionary<string, Dictionary<string, SSkin>> Skins { get; set; }

    public override void OnServerInit()
    {
        Log("Envimix", "Initializing server...");

        UIManager.UIAll.OverlayHide321Go = true;
        UIManager.UIAll.OverlayHideChrono = true;
        UIManager.UIAll.OverlayHideBackground = true;
        UIManager.UIAll.OverlayHideCheckPointTime = true;
        UIManager.UIAll.OverlayHideChat = false;
        UIManager.UIAll.OverlayChatOffset = new Vec2(0, 0.15);
        UIManager.UIAll.OverlayChatHideAvatar = true;
        UIManager.UIAll.OverlayHideCountdown = true;
        UIManager.UIAll.OverlayHideSpeedAndDist = true;
        UIManager.UIAll.OverlayHidePosition = true;
        UIManager.UIAll.OverlayHideMapInfo = true;
        UIManager.UIAll.OverlayHideRoundScores = true;
        UIManager.UIAll.OverlayHidePersonnalBestAndRank = true;
        UIManager.UIAll.OverlayHideCheckPointList = true;
        UIManager.UIAll.OverlayHideEndMapLadderRecap = true;

        var tm2CarNames = new[] { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar" };
        var unitedCarNames = new[] { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

        Cars.Clear();
        SpecialCars.Clear();
        UnitedCars.Clear();
        CustomCars.Clear();
        DisplayedCars.Clear();
        
        if (SkinsFile != "")
        {
            Log("Envimix", $"Reading {SkinsFile}...");

            var skinContent = ReadFile(SkinsFile);

            if (skinContent == "")
            {
                Log("Envimix", $"NOTE: {SkinsFile} is empty or nonexisting. Skin system is going to be disabled.");
            }
            else if (!Skins.FromJson(skinContent))
            {
                Skins.Clear();
                Log("Envimix", $"NOTE: {SkinsFile} has a JSON issue. Skin system is going to be disabled.");
            }
        }

        ItemList_Begin();

        var itemCars = new Dictionary<string, string>();

        if (EnableTM2Cars)
        {
            foreach (var car in tm2CarNames)
            {
                var itemName = car;
                Log("Envimix", $"Adding {itemName}...");

                Cars[car] = new()
                {
                    [""] = ItemList_Add(itemName)
                };

                if (Skins.ContainsKey(car))
                {
                    foreach (var (name, skin) in Skins[car])
                    {
                        Log("Envimix", $"Adding {itemName} with skin {skin.File}...");
                        Cars[car][name] = ItemList_AddWithSkin(itemName, $"Skins/Models/{skin.File}");
                    }
                }

                itemCars[car] = itemName;

                if (EnableStadiumEnvimix && car != "StadiumCar")
                {
                    itemName = $"{VehicleFolder}{TextLib.Replace(VehicleFileFormat, "%1", car)}";
                    SpecialCars[car] = new()
                    {
                        [""] = ItemList_Add(itemName)
                    };

                    if (Skins.ContainsKey(car))
                    {
                        foreach (var (name, skin) in Skins[car])
                        {
                            Log("Envimix", $"Adding {itemName} with skin {skin.File}...");
                            SpecialCars[car][name] = ItemList_AddWithSkin(itemName, $"Skins/Models/{skin.File}");
                        }
                    }
                }
            }
        }

        if (EnableUnitedCars)
        {
            foreach (var car in unitedCarNames)
            {
                var itemName = car;

                if (UseUnitedModels)
                {
                    itemName = $"{VehicleFolder}{TextLib.Replace(VehicleFileFormat, "%1", car)}";
                }

                Log("Envimix", $"Adding {itemName}...");
                UnitedCars[car] = new()
                {
                    [""] = ItemList_Add(itemName)
                };

                if (Skins.ContainsKey(car))
                {
                    foreach (var (name, skin) in Skins[car])
                    {
                        Log("Envimix", $"Adding {itemName} with skin {skin.File}...");
                        UnitedCars[car][name] = ItemList_AddWithSkin(itemName, $"Skins/Models/{skin.File}");
                    }
                }

                itemCars[car] = itemName;
            }
        }

        ItemCars = itemCars;

        ItemList_End();

        Log(nameof(Envimix), "All items successfully added!");

        foreach (var (name, car) in Cars) DisplayedCars.Add(name);
        foreach (var (name, car) in UnitedCars) DisplayedCars.Add(name);
        foreach (var (name, car) in CustomCars) DisplayedCars.Add(name);

        Log(nameof(Envimix), "Creating manialinks...");

        CreateLayer("321Go");
        CreateLayer("Dashboard");
        CreateLayer("PrePostLoading");
    }

    public void CreateLayer(string layerName)
    {
        CreateLayer(layerName, $"Manialinks/Universe2/{layerName}.xml");
    }

    public string GetDefaultCar()
    {
        return MapPlayerModelName;
    }

    public override void OnGameLoop()
    {
        SpawnAllWaitingPlayers();
    }

    public Dictionary<string, Dictionary<string, Ident>> GetAllCars()
    {
        var allCars = new Dictionary<string, Dictionary<string, Ident>>();

        foreach (var (car, model) in Cars)
        {
            allCars[car] = model;
        }

        if (Map is not null && Map.CollectionName == "Stadium")
        {
            foreach (var (car, model) in SpecialCars)
            {
                allCars[car] = model;
            }
        }

        foreach (var (car, model) in UnitedCars)
        {
            allCars[car] = model;
        }

        foreach (var (car, model) in CustomCars)
        {
            allCars[car] = model;
        }

        return allCars;
    }
}
