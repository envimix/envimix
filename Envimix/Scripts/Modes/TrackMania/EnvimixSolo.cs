using Envimix.Scripts.Libs.BigBang1112;
using Envimix.Scripts.Libs.Envimix;
using System.Collections.Immutable;
using System.Numerics;

namespace Envimix.Scripts.Modes.TrackMania;

[Include(typeof(Envimania))]
public class EnvimixSolo : Envimix
{
    public struct SGhostMetadata
    {
        public string FileName;
        public int Index;
        public string Nickname;
        public int Time;
    }

    public struct STitleDto
    {
        public string Id;
        public string DisplayName;
    }

    public struct SMapInfoResponse
    {
        public string Name;
        public string Uid;
        public STitleDto TitlePack;
        public ImmutableArray<Envimania.SFilteredRating> Ratings;
        public IList<Envimania.SFilteredRating> UserRatings;
        public Dictionary<string, Envimania.SEnvimaniaRecord> Validations;
    }

    public struct SRatingClientRequest
    {
        public Envimania.SMapInfo Map;
        public string Car;
        public int Gravity;
        public Envimania.SRating Rating;
    }

    public struct SRatingClientResponse
    {
        public Envimania.SFilteredRating Rating;
    }

    [Setting] public new bool EnableDefaultCar = false; // TODO: enable after finished envimix car run
    [Setting] public new bool EnableStadiumEnvimix = true;
    [Setting] public new bool EnableUnitedCars = true;
    [Setting] public new bool EnableTrafficCar = true;
    [Setting] public new bool EnableTrafficCarInStadium = true;
    [Setting] public new bool UseUnitedModels = true;
    [Setting] public new bool AlwaysUseVehicleItems = false;
    [Setting] public new string EnvimixWebAPI = "http://localhost:5118";
    [Setting] public new string SkinsFile = "Skins_Turbo.json";

    [Setting(As = "Custom countdown")]
    public int CustomCountdown = -1;

    public Dictionary<Ident, string> LocalGhostsTaskFiles;
    public IList<CTaskResult_GhostList> LocalGhostsTasks;
    public Dictionary<string, IList<CGhost>> LocalGhosts;
    public Dictionary<CGhost, Ident> SpawnedGhosts;
    public Dictionary<CGhost, Ident> SpawnedPersonalGhosts;

    [Netwrite] public bool Outro { get; set; }
    public CGhost? OutroGhost;
    public int OutroGhostMaxFinishLength;
    public bool OutroGhostReachedMaxFinishLength;
    public Ident? OutroGhostViewInst;
    public int OutroGhostEndTime;
    public CTaskResult? GhostUploadTask;
    [Netwrite] public bool GhostToUpload { get; set; }
    public string? GhostFinishTimestamp;
    public CTaskResult? NewRecordTask;
    public CHttpRequest? MapInfoRequest;
    public string PersonalRatingUpdatedAt;

    public IList<CTaskResult_Ghost> PersonalGhostTasks;
    public Dictionary<Ident, string> PersonalGhostFilterKeys;
    public Dictionary<string, CGhost> PersonalBestGhosts;
    public bool NewPersonalBest;

    [Netwrite] public IList<SGhostMetadata> LocalGhostMetadata { get; set; }
    [Netwrite] public int LocalGhostMetadataUpdatedAt { get; set; }
    [Netwrite] public bool CanListenToUIEvents { get; set; }
    [Local(LocalFor.Users0)] public string EnvimixTurboUserToken { get; set; } = "";

    [Netwrite] public bool RatingOpen { get; set; }

    public override void OnServerInit()
    {
        ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixSingleplayerClient.Script.txt";

        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.SmallScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.ScoreTableOnlyManialink = true;
        UIManager.UIAll.OverlayHideSpectatorControllers = true;

        Outro = false;
        OutroGhostMaxFinishLength = 1500;
        RatingOpen = false;
    }

    public override void OnMapLoad()
    {
        Wait(() => Players.Count > 0); // Sync the player, as it's not available right after map load

        RemoveAllGhosts();

        CanListenToUIEvents = true;

        PrespawnEnvimixPlayers();

        RequestPersonalGhosts();
    }

    public override void OnMapStart()
    {
        RequestMapInfo();
    }

    public override void OnUIEvent(CUIConfigEvent e)
    {
        switch (e.Type)
        {
            case CUIConfigEvent.EType.OnLayerCustomEvent:
                ProcessGeneralEnvimixEvents(e);
                ProcessUpdateSkinEvent(e);
                ProcessUpdateCarEvent(e);
                ProcessReplayEvent(e);
                ProcessEndscreenEvent(e);
                // ProcessUpdateGravityEvent(e); unexpected behavior due to GravityCoef being applied once after spawning
                break;
        }
    }

    public override void OnPlayerAdded(CTmModeEvent e)
    {
        PrespawnPlayer(e.Player);
    }

    public override void OnPlayerGiveUp(CTmModeEvent e)
    {
        if (FinishedAt != -1)
        {
            Discard(e);
        }
    }

    public override void OnPlayerFinish(CTmModeEvent e)
    {
        FinishedAt = Now;
        UIManager.GetUI(e.Player).UISequence = CUIConfig.EUISequence.Outro;
        OutroGhost = ScoreMgr.Playground_GetPlayerGhost(e.Player);
        GhostFinishTimestamp = TimeLib.GetCurrent();
    }

    public override void OnPlayerFirstFinishOrImprovement(CTmModeEvent e)
    {
        NewPersonalBest = true;
    }

    public override void OnGameLoop()
    {
        if (!Outro)
        {
            foreach (var player in PlayersWaiting)
            {
                TrySpawnEnvimixSoloPlayer(player, frozen: false);
            }
        }

        // Extends outro ghost until maximum safe
        if (FinishedAt != -1 && Now > FinishedAt + OutroGhostMaxFinishLength && !OutroGhostReachedMaxFinishLength)
        {
            Log(nameof(EnvimixSolo), "Extending outro ghost to maximum length.");
            OutroGhost = ScoreMgr.Playground_GetPlayerGhost(GetPlayer());
            if (NewPersonalBest)
            {
                AcknowledgePersonalBest(GetPlayer(), OutroGhost);
            }
            OutroGhostReachedMaxFinishLength = true;
        }

        if (GhostUploadTask is not null && !GhostUploadTask.IsProcessing)
        {
            if (GhostUploadTask.HasSucceeded)
            {
                Log(nameof(EnvimixSolo), "Ghost upload succeeded.");
                DataFileMgr.TaskResult_Release(GhostUploadTask.Id);
                GhostUploadTask = null;
                GhostToUpload = false;
                RequestMapInfo();
            }
            else
            {
                Log(nameof(EnvimixSolo), $"Ghost upload failed - {GhostUploadTask.ErrorType} {GhostUploadTask.ErrorCode} {GhostUploadTask.ErrorDescription}. Retrying...");
                DataFileMgr.TaskResult_Release(GhostUploadTask.Id);
                GhostUploadTask = null;
                Sleep(1000);
                UploadGhost(OutroGhost!);
            }
        }

        if (NewRecordTask is not null && !NewRecordTask.IsProcessing)
        {
            if (NewRecordTask.HasSucceeded)
            {
                Log(nameof(EnvimixSolo), "New record registered successfully.");
            }
            else
            {
                Log(nameof(EnvimixSolo), $"New record registration failed - {NewRecordTask.ErrorType} {NewRecordTask.ErrorCode} {NewRecordTask.ErrorDescription}.");
            }
            DataFileMgr.TaskResult_Release(NewRecordTask.Id);
            NewRecordTask = null;
        }

        if (MapInfoRequest is not null && MapInfoRequest.IsCompleted)
        {
            if (MapInfoRequest.StatusCode == 200)
            {
                SMapInfoResponse mapInfoResponse = new();

                if (mapInfoResponse.FromJson(MapInfoRequest.Result))
                {
                    var validations = Netwrite<Dictionary<string, Envimania.SEnvimaniaRecord>>.For(Teams[0]);

                    foreach (var (key, validationRec) in mapInfoResponse.Validations!)
                    {
                        validations.Get()[key] = validationRec;
                    }

                    Dictionary<string, Envimania.SRating> ratings = new();

                    foreach (var filteredRating in mapInfoResponse.Ratings)
                    {
                        ratings[ConstructRatingFilterKey(filteredRating.Filter)] = filteredRating.Rating;
                    }

                    Ratings = ratings;

                    var myRatings = Netwrite<IList<Envimania.SFilteredRating>>.For(UIManager.GetUI(GetPlayer()));
                    myRatings.Set(mapInfoResponse.UserRatings!);

                    RatingEnabled = true;

                    // this is overflow simulator but the game is paused during ratings often so Now doesnt update to this properly so this just works okay?? be cool with it
                    RatingsUpdatedAt = TextLib.ToInteger(TimeLib.GetCurrent());

                    ValidationsUpdatedAt = Now;

                    Log(nameof(EnvimixSolo), $"Map info for '{mapInfoResponse.Name}' ({mapInfoResponse.Uid}) received from webapi.");
                }
                else
                {
                    Log(nameof(EnvimixSolo), $"Failed to get map info from webapi (JSON issue).");
                    Log(nameof(EnvimixSolo), MapInfoRequest.Result);
                }
            }
            else
            {
                Log(nameof(EnvimixSolo), $"Failed to get map info from webapi (status code: {MapInfoRequest.StatusCode})");
            }
            Http.Destroy(MapInfoRequest);
            MapInfoRequest = null;
        }

        CheckForPersonalGhosts();
        CheckForLocalGhosts();
        CheckPersonalRatings();
    }

    private CTmPlayer GetPlayer()
    {
        return Players[0];
    }

    private void RequestPersonalGhosts()
    {
        foreach (var car in DisplayedCars)
        {
            // alternatively, fetch personal best from envimix webapi

            var key = ConstructFilterKey(car);
            var task = ScoreMgr.Map_GetRecordGhost(null, Map.MapInfo.MapUid, "Test" + car);

            if (task is null)
            {
                continue;
            }

            PersonalGhostFilterKeys[task.Id] = key;
            PersonalGhostTasks.Add(task);
        }
    }

    private void SpawnPersonalGhost(CTmPlayer player)
    {
        foreach (var (ghost, spawnIdent) in SpawnedPersonalGhosts)
        {
            RaceGhost_Remove(spawnIdent);
        }
        SpawnedPersonalGhosts.Clear();

        var key = ConstructFilterKey(player);
        if (PersonalBestGhosts.ContainsKey(key))
        {
            var pbGhost = PersonalBestGhosts[key];
            SpawnedPersonalGhosts[pbGhost] = RaceGhost_Add(pbGhost, DisplayAsPlayerBest: true);
        }
    }

    private void RequestMapInfo()
    {
        var envimixTurboUserToken = Local<string>.For(GetPlayer().User);
        MapInfoRequest = Http.CreateGet($"{EnvimixWebAPI}/maps/{Map.MapInfo.MapUid}", false, $"Authorization: Bearer {envimixTurboUserToken.Get()}");
    }

    private bool TrySpawnEnvimixSoloPlayer(CTmPlayer player, bool frozen)
    {
        SpawnPersonalGhost(player);

        if (frozen)
        {
            return TrySpawnEnvimixPlayer(player, frozen);
        }

        if (CustomCountdown < 0)
        {
            return TrySpawnEnvimixPlayer(player, -1);
        }

        return TrySpawnEnvimixPlayer(player, Now + CustomCountdown);
    }

    public bool SpawnEnvimixSoloPlayer(CTmPlayer player, string car, bool frozen)
    {
        SpawnPersonalGhost(player);

        if (frozen)
        {
            return SpawnEnvimixPlayer(player, car, frozen);
        }

        if (CustomCountdown < 0)
        {
            return SpawnEnvimixPlayer(player, car, -1);
        }

        return SpawnEnvimixPlayer(player, car, Now + CustomCountdown);
    }

    private void ProcessUpdateCarEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "Car":
                if (e.CustomEventData.Count > 0)
                {
                    var carName = e.CustomEventData[0];
                    var player = GetPlayer(e.UI);
                    SetValidClientCar(player, carName);

                    var car = Netwrite<string>.For(player);

                    if (e.CustomEventData.Count > 1)
                    {
                        var respawn = true; // always respawn on Car event

                        if (respawn)
                        {
                            var frozen = e.CustomEventData.Count > 2 && e.CustomEventData[2] == "True";
                            var spawned = SpawnEnvimixSoloPlayer(player, car.Get(), frozen);
                        }
                    }
                }
                break;
        }
    }

    private void ProcessReplayEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "ResetReplays":
                LocalGhostMetadata.Clear();
                break;
            case "Replay":
                var fileName = e.CustomEventData[0];
                var task = DataFileMgr.Replay_Load(fileName);
                LocalGhostsTaskFiles[task.Id] = fileName;
                LocalGhostsTasks.Add(task);
                break;
            case "AddGhost":
                var replayFileNameAdd = e.CustomEventData[0];
                var ghostIndexAdd = e.CustomEventData[1];
                var ghostToAdd = LocalGhosts[replayFileNameAdd][TextLib.ToInteger(ghostIndexAdd)];
                SpawnedGhosts[ghostToAdd] = RaceGhost_Add(ghostToAdd, DisplayAsPlayerBest: false);

                Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameAdd}' (#{ghostIndexAdd}) added.");

                break;
            case "RemoveGhost":
                var replayFileNameRemove = e.CustomEventData[0];
                var ghostIndexRemove = e.CustomEventData[1];
                var ghostToRemove = LocalGhosts[replayFileNameRemove][TextLib.ToInteger(ghostIndexRemove)];

                if (SpawnedGhosts.ContainsKey(ghostToRemove))
                {
                    RaceGhost_Remove(SpawnedGhosts[ghostToRemove]);
                    SpawnedGhosts.Remove(ghostToRemove);

                    Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameRemove}' (#{ghostIndexRemove}) removed.");
                }
                else
                {
                    Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameRemove}' (#{ghostIndexRemove}) is missing but shouldn't be.");
                }

                break;
            case "OpenRating":
                RatingOpen = true;
                break;
            case "CloseRating":
                RatingOpen = false;
                break;
        }
    }

    private void UploadGhost(CGhost ghost)
    {
        GhostToUpload = true;

        var envimixTurboUserToken = Local<string>.For(GetPlayer().User);
        GhostUploadTask = DataFileMgr.Ghost_Upload($"{EnvimixWebAPI}/envimania/record", ghost, $"Authorization: Bearer {envimixTurboUserToken.Get()}\nX-Envimix-Timestamp: {GhostFinishTimestamp}");
    }

    public void AcknowledgePersonalBest(CPlayer player, CGhost ghost)
    {
        PersonalBestGhosts[ConstructFilterKey(player)] = ghost;

        UploadGhost(ghost);

        var car = Netwrite<string>.For(player);
        NewRecordTask = ScoreMgr.Map_SetNewRecord(null, Map.MapInfo.MapUid, $"Test{car.Get()}", ghost);

        NewPersonalBest = false;
    }

    public void RemoveAllGhosts()
    {
        RaceGhost_RemoveAll();
        SpawnedPersonalGhosts.Clear();
    }

    private void SwitchToOutro(CUIConfig ui)
    {
        if (FinishedAt == -1)
        {
            return;
        }

        FinishedAt = -1;

        if (!OutroGhostReachedMaxFinishLength)
        {
            Log(nameof(EnvimixSolo), "Extending outro ghost for outro switch...");
            OutroGhost = ScoreMgr.Playground_GetPlayerGhost(GetPlayer());

            if (NewPersonalBest)
            {
                AcknowledgePersonalBest(GetPlayer(), OutroGhost);
            }
        }

        OutroGhostReachedMaxFinishLength = false; // hack?

        // possibly best option for outro sequence
        ui.UISequence = CUIConfig.EUISequence.EndRound;

        UnspawnAllPlayers();
        RemoveAllGhosts();

        OutroGhostViewInst = RaceGhost_Add(OutroGhost, false);
        OutroGhostEndTime = Now + 2500 + OutroGhost!.Result.Time;

        Outro = true;

        ui.SpectatorForcedTarget = OutroGhostViewInst;
        ui.SpectatorForceCameraType = 1;
    }

    private void SwitchFromOutro(CUIConfig ui)
    {
        if (!Outro)
        {
            return;
        }

        Outro = false;
        RemoveAllGhosts();

        ui.UISequence = CUIConfig.EUISequence.Playing;
    }

    private void ProcessEndscreenEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "EndscreenContinue":
                if (!GhostToUpload)
                {
                    SwitchToOutro(e.UI);
                }
                break;
            case "OutroContinue":
                SwitchFromOutro(e.UI);
                break;
        }
    }

    private void CheckForLocalGhosts()
    {
        ImmutableArray<CTaskResult_GhostList> completedGhostTasks = new();

        foreach (var ghostTask in LocalGhostsTasks)
        {
            if (ghostTask.IsProcessing)
            {
                continue;
            }

            if (ghostTask.HasSucceeded && ghostTask.Ghosts.Count > 0)
            {
                ImmutableArray<CGhost> ghosts = new();

                for (int i = 0; i < ghostTask.Ghosts.Count; i++)
                {
                    var ghost = ghostTask.Ghosts[i];

                    ghosts.Add(ghost);

                    SGhostMetadata metadata = new()
                    {
                        FileName = LocalGhostsTaskFiles[ghostTask.Id],
                        Index = i,
                        Nickname = ghost.Nickname,
                        Time = ghost.Result.Time
                    };

                    LocalGhostMetadata.Add(metadata);
                }

                LocalGhosts[LocalGhostsTaskFiles[ghostTask.Id]] = ghosts;
            }

            completedGhostTasks.Add(ghostTask);

            LocalGhostsTaskFiles.Remove(ghostTask.Id);

            LocalGhostMetadataUpdatedAt = Now;
        }

        foreach (var ghostTask in completedGhostTasks)
        {
            LocalGhostsTasks.Remove(ghostTask);
        }

        if (completedGhostTasks.Length > 0)
        {
            completedGhostTasks.Clear();
        }
    }

    private void CheckForPersonalGhosts()
    {
        ImmutableArray<CTaskResult_Ghost> completedGhostTasks = new();

        foreach (var ghostTask in PersonalGhostTasks)
        {
            if (ghostTask.IsProcessing)
            {
                continue;
            }

            var key = PersonalGhostFilterKeys[ghostTask.Id];

            if (ghostTask.HasSucceeded)
            {
                if (ghostTask.Ghost is null)
                {
                    continue;
                }

                var envimixBestRace = Netwrite<Dictionary<string, Record.SRecord>>.For(GetPlayer().Score);
                envimixBestRace.Get()[key] = Record.ToRecord(ghostTask.Ghost.Result, -1, GetPlayer());
                PersonalBestGhosts[key] = ghostTask.Ghost;

                if (key == ConstructFilterKey(GetPlayer()))
                {
                    GetPlayer().Score.BestRace = ghostTask.Ghost.Result;
                }
            }
            else
            {
                Log(nameof(EnvimixSolo), $"Failed to get best ghost for {key}: {ghostTask.ErrorType} {ghostTask.ErrorCode} {ghostTask.ErrorDescription}");
            }

            completedGhostTasks.Add(ghostTask);
            PersonalGhostFilterKeys.Remove(ghostTask.Id);
        }

        foreach (var ghostTask in completedGhostTasks)
        {
            PersonalGhostTasks.Remove(ghostTask);
        }
    }

    public void CheckPersonalRatings()
    {
        if (UserRatingRequest is null && UserRatingsToRequest.Count > 0 && (PersonalRatingUpdatedAt == "" || TimeLib.GetDelta(TimeLib.GetCurrent(), PersonalRatingUpdatedAt) > 0))
        {
            SRatingClientRequest ratingReq = new();

            Envimania.SMapInfo mapInfo = new()
            {
                Name = Map.MapInfo.Name,
                Uid = Map.MapInfo.MapUid
            };

            // transform server request to client request because I cant be bothered with this shit xdd

            foreach (var (key, req) in UserRatingsToRequest)
            {
                ratingReq.Map = mapInfo;
                ratingReq.Rating = req.Rating;
                ratingReq.Car = req.Car;
                ratingReq.Gravity = req.Gravity;

                var difficulty = req.Rating.Difficulty;
                var quality = req.Rating.Quality;

                var isDifficultyDifferent = true;
                var isQualityDifferent = true;

                if (UserRatings.ContainsKey(key))
                {
                    isDifficultyDifferent = UserRatings[key].Difficulty != difficulty;
                    isQualityDifferent = UserRatings[key].Quality != quality;
                }

                UserRatings[key] = req.Rating;
            }

            Log(nameof(EnvimixSolo), "Submitting rating...");

            UserRatingRequest = Http.CreatePost($"{EnvimixWebAPI}/rate", ratingReq.ToJson(), $"Authorization: Bearer {EnvimixTurboUserToken}\nContent-Type: application/json");

            UserRatingsToRequest.Clear();

            PersonalRatingUpdatedAt = TimeLib.GetCurrent();
        }

        if (UserRatingRequest is not null && UserRatingRequest.IsCompleted)
        {
            if (UserRatingRequest.StatusCode == 200)
            {
                Log(nameof(Envimix), "Rating submitted (200).");

                SRatingClientResponse response = new();

                if (response.FromJson(UserRatingRequest.Result))
                {
                    var ratings = Ratings;
                    ratings[ConstructRatingFilterKey(response.Rating.Filter)] = response.Rating.Rating;

                    Ratings = ratings;

                    // this is overflow simulator but the game is paused during ratings often so Now doesnt update to this properly so this just works okay?? be cool with it
                    RatingsUpdatedAt = TextLib.ToInteger(TimeLib.GetCurrent());
                }
                else
                {
                    Log(nameof(Envimix), $"Rating submission failed (JSON issue). Reported in server logs.");
                    Log(nameof(Envimix), UserRatingRequest.Result);
                }
            }
            else
            {
                Log(nameof(Envimix), $"Rating submission failed ({UserRatingRequest.StatusCode}).");
            }

            Http.Destroy(UserRatingRequest);
            UserRatingRequest = null;
        }
    }
}
