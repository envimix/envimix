using Envimix.Scripts.Libs.BigBang1112;
using System.Collections.Immutable;

namespace Envimix.Scripts.Modes.TrackMania;

[Include(typeof(Record))]
public class Envimix : UniverseModeBase
{
    public struct SSkin
    {
	    public string File;
        public string Icon;
    }

    public struct SLoginRequest
    {
        public string Login;
        public string TokenType;
        public string Token;
    }

    public const string EnvimixWebAPI = "http://localhost:32771";

    [Setting(As = "Enable TM2 cars")]
    public bool EnableTM2Cars = true;

    [Setting(As = "Enable TrafficCar")]
    public bool EnableTrafficCar = true;

    [Setting(As = "Enable United cars")]
    public bool EnableUnitedCars = false;

    [Setting(As = "Enable custom cars")]
    public bool EnableCustomCars = false;

    [Setting(As = "Enable default car")]
    public bool EnableDefaultCar = true;

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
    [Netwrite] public bool CarSelectionMode { get; set; }

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

        //ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixMultiplayerClient.Script.txt";

        ImmutableArray<string> tm2CarNames = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar" };

        if (EnableTrafficCar)
        {
            tm2CarNames.Add("TrafficCar");
        }
        
        ImmutableArray<string> unitedCarNames = new() { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

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

        // Minor copypaste behaviour, worth refactoring
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
        CreateLayer("TimeLimit");
        CreateLayer("Map");
        CreateLayer("Checkpoint");

        Log(nameof(Envimix), "All manialinks successfully created!");

        foreach (var player in AllPlayers)
        {
            var prepareLoading = Netwrite<int>.For(UIManager.GetUI(player));
            prepareLoading.Set(-1);
        }

        LoginToEnvimixWebApi();
    }

    public void LoginToEnvimixWebApi()
    {
        ServerAdmin.Authentication_GetToken(null, "Envimix");
        Wait(() => ServerAdmin.Authentication_GetTokenResponseReceived);

        SLoginRequest loginRequest = new()
        {
            Login = ServerLogin,
            TokenType = "Ingame",
            Token = ServerAdmin.Authentication_Token
        };

        var request = Http.CreatePost($"{EnvimixWebAPI}/login", loginRequest.ToJson(), "Content-Type: application/json");
        Wait(() => request.IsCompleted);
        Log(nameof(Envimix), $"Login response: {request}");
    }

    public override void OnServerStart()
    {
        CutOffTimeLimit = -1;

        UiRounds = true;
        Scores_Sort(CTmMode.ETmScoreSortOrder.TotalPoints);

        if (!EnableStadiumEnvimix)
        {
            var amountOfStadiumMaps = GetMapCountByEnvironment("Stadium");

            if (amountOfStadiumMaps == MapList.Count)
            {
                Log(nameof(Envimix), "PROBLEM: Current map list contains only Stadium maps. Make sure you have enabled S_EnableStadiumEnvimix or choose a different map list. Terminating the server...");
                Sleep(5000);
                Terminate = true;
            }
            else if (amountOfStadiumMaps > 0)
            {
                Log(nameof(Envimix), $"NOTE: Current map list contains one or more Stadium maps. They are going to be skipped. To allow Stadium envimix, enable setting S_EnableStadiumEnvimix and make sure you provide the cars in the Items/{VehicleFolder} folder.");
            }
        }
    }

    public override void OnMapLoad()
    {
        if (!EnableStadiumEnvimix && Map.MapInfo.CollectionName == "Stadium")
        {
            Log(nameof(Envimix), $"PROBLEM: Stadium environment is not currently supported. You can enable Stadium envimix by setting S_EnableStadiumEnvimix to True. Make sure you provide the cars in the Items/{VehicleFolder} folder.");
            MatchEndRequested = true;
        }
    }

    public override void OnMapStart()
    {
        Log(nameof(Envimix), $"Starting map {TextLib.StripFormatting(Map.MapInfo.Name)}...");
    }

    public override void OnMapEnd()
    {
        Log(nameof(Envimix), $"Ending map {TextLib.StripFormatting(Map.MapInfo.Name)}...");

        foreach (var player in Players)
        {
            UnspawnPlayer(player);
        }

        if (!Reload)
        {
            if (MapQueue.Length > 0)
            {
                if (!EnableStadiumEnvimix && MapList[MapQueue[0]].CollectionName == "Stadium")
                {
                    Log(nameof(Envimix), $"Skipping {MapList[MapQueue[0]].Name} from manual queue: Stadium environment");
                }
                else
                {
                    NextMapIndex = MapQueue[0];
                }

                MapQueue.Remove(0);
            }

            // Minor copypaste behaviour, worth refactoring
            while (!EnableStadiumEnvimix)
            {
                var mapName = MapList[NextMapIndex].Name;

                if (MapList[NextMapIndex].CollectionName == "Stadium")
                {
                    NextMapIndex += 1;
                }
                else
                {
                    break;
                }

                Log(nameof(Envimix), $"Skipping {mapName}: Stadium environment");
                Yield();
            }
        }
    }

    public void CreateLayer(string layerName)
    {
        CreateLayer(layerName, $"Manialinks/Universe2/{layerName}.xml");
    }

    public static void NoticeMessage(IList<CUIConfig> uis, string text)
    {
        foreach (var ui in uis)
        {
            var noticeMessage = Netwrite<string>.For(ui);
            noticeMessage.Set(text);
        }
    }

    public static void NoticeMessage(CUIConfig ui, string text)
    {
        var noticeMessage = Netwrite<string>.For(ui);
        noticeMessage.Set(text);
    }

    public string GetDefaultCar()
    {
        return MapPlayerModelName;
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

    public bool SpawnEnvimixPlayer(CTmPlayer player, string carName, int raceStartTime)
    {
        var allCars = GetAllCars();

        if (player is null || !allCars.ContainsKey(carName))
        {
            return false;
        }

        var userSkins = Netread<Dictionary<string, string>>.For(UIManager.GetUI(player));
        var skins = userSkins.Get();

        var skin = "";
        if (skins.ContainsKey(carName) && allCars[carName].ContainsKey(skins[carName]))
        {
            skin = skins[carName];
        }

        player.ForceModelId = allCars[carName][skin];

        var car = Netwrite<string>.For(player);
        car.Set(carName);

        var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(player.Score);

        if (envimixBestRace.Get().ContainsKey(carName))
        {
            Record.ToResult(player.Score.BestRace, envimixBestRace.Get()[carName]);
        }
        else
        {
            player.Score.BestRace = null;
        }

        var envimixPrevRace = Netwrite<Dictionary<string, Record.SRecord>>.For(player.Score);

        if (envimixPrevRace.Get().ContainsKey(carName))
        {
            Record.ToResult(player.Score.PrevRace, envimixPrevRace.Get()[carName]);
        }
        else
        {
            player.Score.PrevRace = null;
        }

        var spawned = true;

        var rst = raceStartTime;

        if (rst == -2)
        {
            rst = -1;
        }

        SpawnPlayer(player, player.CurrentClan, rst);

        if (raceStartTime == -2)
        {
            if (CutOffTimeLimit < 0)
            {
                player.RaceStartTime = Now + 9999999 + DisplayedCars.IndexOf(carName) * 3000 + 3000;
            }
            else
            {
                player.RaceStartTime = CutOffTimeLimit + DisplayedCars.IndexOf(carName) * 3000 + 3000;
            }

            spawned = false;
        }
        else if (!EnableDefaultCar && ItemCars[carName] == GetDefaultCar())
        {
            player.RaceStartTime = -1;
            spawned = false;
        }

        Ladder_AddPlayer(player.Score);
        return spawned;
    }

    public bool SpawnEnvimixPlayer(CTmPlayer player, string car, bool frozen)
    {
        if (frozen)
        {
            return SpawnEnvimixPlayer(player, car, -2);
        }

        return SpawnEnvimixPlayer(player, car, -1);
    }

    public void SpawnAllEnvimixPlayers(string carName, bool frozen)
    {
        foreach (var player in PlayersWaiting)
        {
            var car = Netwrite<string>.For(player);
            car.Set(carName);
            var spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
    }

    public void SpawnAllEnvimixPlayers(bool frozen)
    {
        foreach (var player in PlayersWaiting)
        {
            var car = Netwrite<string>.For(player);
            var spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
    }

    public void SpawnAllEnvimixPlayers()
    {
        SpawnAllEnvimixPlayers(false);
    }

    public bool TrySpawnEnvimixPlayer(CTmPlayer player, bool frozen, int timeLimit)
    {
        var clientCar = Netread<string>.For(UIManager.GetUI(player));
        var car = Netwrite<string>.For(player);

        // Validation of available cars, invalid car currently ignores changing anything but passes the spawn attempt
        if (DisplayedCars.Contains(clientCar.Get()))
        {
            car.Set(clientCar.Get());
        }

        bool spawned;
        if (frozen)
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
        else if (timeLimit < 0 || CutOffTimeLimit - Now < timeLimit * 1000)
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
        }
        else
        {
            spawned = SpawnEnvimixPlayer(player, car.Get(), CutOffTimeLimit - timeLimit * 1000);
        }

        if (spawned)
        {
            Log(nameof(EnvimixTeamAttack), $"{player.User.Name} spawned");
        }

        if (!EnableDefaultCar && ItemCars[car.Get()] == GetDefaultCar())
        {
            NoticeMessage(UIManager.GetUI(player), "Default car is currently disabled.\n$ff0Please select another car.");
        }
        else if (CarSelectionMode)
        {
            NoticeMessage(UIManager.GetUI(player), $"You have selected $ff0{car.Get()}$g!\nPlease wait before the game starts.");
        }
        else
        {
            NoticeMessage(UIManager.GetUI(player), "");
        }

        return spawned;
    }

    public bool TrySpawnEnvimixPlayer(CTmPlayer player, bool frozen)
    {
        return TrySpawnEnvimixPlayer(player, frozen, timeLimit: -1);
    }

    public void UpdateSkin(CTmPlayer player, string skin)
    {
        var car = Netwrite<string>.For(player);
	
	    var allCars = GetAllCars();
	
	    ImmutableArray<string> sortedNames = new();
	    if (Skins.ContainsKey(car.Get()))
        {
            foreach (var (name, carSkin) in Skins[car.Get()])
            {
                sortedNames.Add(name);
            }

            sortedNames = sortedNames.Sort();
	    }
	
	    var actualSkin = "";
        if (allCars[car.Get()].ContainsKey(skin))
        {
            actualSkin = skin;
        }
	
	    player.ForceModelId = allCars[car.Get()][actualSkin];
	
	    if (skin == "")
        {
		    if (CutOffTimeLimit < 0)
            {
                player.RaceStartTime = Now + 9999999 + DisplayedCars.IndexOf(car.Get()) * 3000 + 3000;
            }
            else
            {
                player.RaceStartTime = CutOffTimeLimit + DisplayedCars.IndexOf(car.Get()) * 3000 + 3000;
            }
        }
	    else
        {
		    if (CutOffTimeLimit < 0)
            {
                player.RaceStartTime = Now + 9999999 + DisplayedCars.IndexOf(car.Get()) * 3000 + (sortedNames.IndexOf(skin) + 1) * 3000 + 3000;
            }
            else
            {
                player.RaceStartTime = CutOffTimeLimit + DisplayedCars.IndexOf(car.Get()) * 3000 + (sortedNames.IndexOf(skin) + 1) * 3000 + 3000;
            }
        }
    }

    public static ImmutableArray<CTmResult> GetRecords(bool referenceCondition, bool similarityCondition)
    {
	    return new();
    }

    public ImmutableArray<Record.SRecord> GetBestRecords(string car)
    {
        ImmutableArray<Record.SRecord> records = new();

        Record.SRecord record = new()
        {
            Time = -1
        };

        foreach (var score in Scores)
        {
            var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);

            if (envimixBestRace.Get().ContainsKey(car) && (record.Time == -1 || envimixBestRace.Get()[car].Time < record.Time))
            {
                record = envimixBestRace.Get()[car];
            }
        }

	    if (record.Time != -1)
        {
		    foreach (var score in Scores)
            {
                var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);

                if (envimixBestRace.Get().ContainsKey(car) && envimixBestRace.Get()[car].Time == record.Time)
                {
                    records.Add(envimixBestRace.Get()[car]);
                }
            }
	    }

	    return records;
    }

    public int GetBestTime(string car)
    {
	    var time = -1;
	    var records = GetBestRecords(car);

	    if (records.Length > 0)
        {
            time = records[0].Time;
        }

        return time;
    }

    public static ImmutableArray<Record.SRecord> GetWorstRecords(string car)
    {
	    return new();
    }

    public static int GetWorstTime(string car)
    {
	    var time = -1;
	    var records = GetWorstRecords(car);

	    if (records.Length > 0)
        {
            time = records[0].Time;
        }

        return time;
    }

    public ImmutableArray<Record.SRecord> GetRecords(string car)
    {
	    ImmutableArray<Record.SRecord> records = new();

	    foreach (var score in Scores)
        {
            var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);

		    if (envimixBestRace.Get().ContainsKey(car) && envimixBestRace.Get()[car].Time != -1)
            {
                records.Add(envimixBestRace.Get()[car]);
            }
        }

	    return records;
    }

    public Dictionary<string, float> GetPlayerWRPB(CTmScore score)
    {
        Dictionary<string, float> wrPbs = new();

	    foreach (var (car, model) in GetAllCars())
        {
		    var bestTime = GetBestTime(car);

            var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);
            var wrPb = 0f;

		    if (envimixBestRace.Get().ContainsKey(car))
            {
                if (envimixBestRace.Get()[car].Time == 0)
                {
                    wrPb = 1;
                }
                else
                {
                    wrPb = bestTime / MathLib.ToReal(envimixBestRace.Get()[car].Time);
                }
            }

            wrPbs[car] = wrPb;
	    }

	    return wrPbs;
    }

    public Dictionary<string, int> GetPlayerActivityPoints(CTmScore score)
    {
	    Dictionary<string, int> activityPoints = new();

	    foreach (var (car, wrPb) in GetPlayerWRPB(score))
        {
		    var records = GetRecords(car);

		    if (records.Length > 0 && wrPb > 0)
            {
                activityPoints[car] = MathLib.NearestInteger(1000 * MathLib.Exp(GetRecords(car).Length * (wrPb - 1)));
            }
            else
            {
                activityPoints[car] = 0;
            }
        }

	    return activityPoints;
    }

    public static Dictionary<string, int> GetPlayerSkillpoints(CTmScore score)
    {
	    return new();
    }

    public float GetPlayerWRPB(CTmScore score, string car)
    {
	    return GetPlayerWRPB(score)[car];
    }

    public int GetPlayerActivityPoints(CTmScore score, string car)
    {
	    return GetPlayerActivityPoints(score)[car];
    }

    public static int GetPlayerSkillpoints(CTmScore score, string car)
    {
	    return -1;
    }

    public void UpdateScores()
    {
        for (var i = 0; i < ClanScores.Count; i++)
        {
            ClanScores[i] = 0;
        }

	    foreach (var score in Scores)
        {
            var envimixPoints = Netwrite<Dictionary<string, int>>.For(score);
            envimixPoints.Set(GetPlayerActivityPoints(score));
		    score.Points = 0;

		    foreach (var (car, points) in envimixPoints.Get())
            {
                score.Points += points;
            }

            ClanScores[score.TeamNum] += score.Points;
	    }

	    Scores_Sort(CTmMode.ETmScoreSortOrder.TotalPoints);
    }

    public void ClearScores()
    {
	    foreach (var score in Scores)
        {
            var envimixPoints = Netwrite<Dictionary<string, int>>.For(score);
            var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);
            var envimixBestLap = Netwrite<Dictionary<string, Record.SRecord>>.For(score);
            var envimixPrevRace = Netwrite<Dictionary<string, Record.SRecord>>.For(score);
            envimixPoints.Get().Clear();
            envimixBestRace.Get().Clear();
            envimixBestLap.Get().Clear();
            envimixPrevRace.Get().Clear();
	    }

	    Scores_Clear();
    }

    public void PrepareJoinedPlayer(CTmPlayer player)
    {
        var car = Netwrite<string>.For(player);
        car.Set(ItemCars.KeyOf(MapPlayerModelName));
    }
}
