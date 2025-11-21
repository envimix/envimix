using Envimix.Scripts.Libs.Envimix;
using System.Collections.Immutable;

namespace Envimix.Scripts;

[Include(typeof(Loading))]
public class MainMenu : CManiaAppTitle, IContext
{
    public struct SUserInfo
    {
        public string Login;
        public string Nickname;
        public string Zone;
        public string AvatarUrl;
        public string Language;
        public string Description;
        public Vec3 Color;
        public string SteamUserId;
        public int FameStars;
        public float LadderPoints;
    }

    public struct SAuthenticateUserRequest
    {
        public string Token;
        public SUserInfo User;
    }

    public struct SAuthenticateUserReponse
    {
        public string Login;
        public string Token;
        public bool IsAdmin;
    }

    public struct STitleReleaseInfo
    {
        public string ReleasedAt;
        public string Key;
    }

    public struct SMapInfo
    {
        public string Name;
        public string Uid;
        public int Order;
    }

    public struct SSubmitMapsRequest
    {
        public string TitleId;
        public IList<SMapInfo> Maps;
    }

    public struct SSubmitTitleRequest
    {
        public string TitleId;
        public string Name;
        public string Version;
    }

    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    public struct SValidationInfo
    {
        public string Login;
        public string Nickname;
        public string DrivenAt;
    }

    public struct STitleStats
    {
        public Dictionary<string, Dictionary<string, SRating>> Ratings;
        public Dictionary<string, Dictionary<string, SStar>> Stars;
        public Dictionary<string, Dictionary<string, SValidationInfo>> Validations;
        public Dictionary<string, Dictionary<string, IList<int>>> Skillpoints;
    }

    public bool ManiaPlanetAuthenticationRequested;
    public required string ManiaPlanetAuthenticationToken;
    public CHttpRequest? UserTokenRequest;
    public int UserTokenRequestTimeout = -1;
    public int UserTokenFirstRequestTimeout = -1;
    public int UserTokenReceived = -1;
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";
    [Local(LocalFor.LocalUser)] public bool EnvimixTurboUserIsAdmin { get; set; }
    public int ManiaPlanetAuthReceivedAt = -1;

    [Local(LocalFor.LocalUser)] public string TitleRelease { get; set; } = "";
    [Local(LocalFor.LocalUser)] public string TitleKey { get; set; } = "";

    [Local(LocalFor.LocalUser)] public string EnvimixOpenMapUid { get; set; } = "";

    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SRating>> TitleRatings { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SStar>> TitleStars { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SValidationInfo>> TitleValidations { get; set; }

    [Local(LocalFor.LocalUser)] public bool IntroEnded { get; set; }

    public CUILayer MainMenuLayer;
    public CUILayer SoloMenuLayer;
    public CUILayer LoadingLayer;
    public CUILayer ReleaseLayer;

    public CHttpRequest? SubmitMapsRequest;
    public CHttpRequest? SubmitTitleRequest;
    public CHttpRequest? TotdRequest;
    public CHttpRequest? StatsRequest;
    public Dictionary<string, Dictionary<string, CHttpRequest>> LeaderboardRequests;

    public string ScoreContextPrefix = "Test";

    public ImmutableArray<string> Cars;

    public const string EnvimixWebAPI = "https://api.envimix.gbx.tools";

    public void Main()
    {        
        Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        
        ImmutableArray<string> allowedLogins = new()
        {
            "bigbang1112",
            "poutrel",
            "riolu",
            "dragontm",
            "mystixor",
            "spookykoala2113",
            "ajsasflaym",
            "arkes910",
            "ydoowoody",
            "hot_wheeler",
            "totokill13",
            "tushy444trackmaniagamer",
            "naruto42",
            "trysketuri",
            "hetna"
        };

        if (!allowedLogins.Contains(LocalUser.Login))
        {
            while (true)
            {
                Menu_Quit();
                Yield();
            }
        }

        ManiaPlanetAuthenticationRequested = true;
        Authentication_GetToken(null, "Envimix");

        var introLayer = UILayerCreate();
        introLayer.ManialinkPage = "file://Media/Manialinks/Intro.xml";

        ReleaseLayer = UILayerCreate();
        ReleaseLayer.ManialinkPage = "file://Media/Manialinks/Release.xml";

        var releaseReceivedAt = -1;
        TitleRelease = "";
        var released = false;
        var countdownShown = false;

        while (TitleRelease == "" || TimeLib.Compare(TimeLib.GetCurrent(), TitleRelease) < 0 || TitleKey == "")
        {
            // this is some REAL maniascript bullshit bug right here
            if (TitleRelease != "" && TimeLib.Compare(TimeLib.GetCurrent(), TitleRelease) >= 0 && TitleKey != "")
            {
                break;
            }

            if (!released && TitleRelease != "" && TimeLib.Compare(TimeLib.GetCurrent(), TitleRelease) >= 0 && TitleKey == "")
            {
                Log("Title has been released!");
                released = true;
                releaseReceivedAt = -1;
            }

            var waitTime = 30000;
            if (released)
            {
                waitTime = 2000;
            }

            // if we received release info recently, wait a bit before requesting again
            if (releaseReceivedAt != -1 && Now - releaseReceivedAt < waitTime)
            {
                Yield();
                if (!countdownShown && IntroEnded)
                {
                    LayerCustomEvent(ReleaseLayer, "Show", new[] { "" });
                    countdownShown = true;
                }
                CheckToken();
                continue;
            }

            var titleReleaseRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/release");
            Wait(() => titleReleaseRequest.IsCompleted);

            if (titleReleaseRequest.StatusCode == 200)
            {
                STitleReleaseInfo releaseInfo = new();
                if (releaseInfo.FromJson(titleReleaseRequest.Result))
                {
                    if (TitleRelease != releaseInfo.ReleasedAt)
                    {
                        released = false;
                    }

                    TitleRelease = releaseInfo.ReleasedAt!;
                    TitleKey = releaseInfo.Key!;

                    var expectation = "";
                    if (released)
                    {
                        expectation = " (expecting release sometime now)";
                    }

                    Log($"Title release info received. Release date: {TimeLib.FormatDate(TitleRelease, TimeLib.EDateFormats.Full)}{expectation}");

                    releaseReceivedAt = Now;
                }
                else
                {
                    Log("Failed to parse title release info. You can press ESC to escape.");
                    Sleep(10000);
                }
            }
            else if (titleReleaseRequest.StatusCode == 404)
            {
                Log("No title release info found. You can press ESC to escape.");
                //releaseReceivedAt = Now;
                Sleep(10000);
            }
            else
            {
                Log($"Failed to fetch title release info ({titleReleaseRequest.StatusCode}). You can press ESC to escape.");
                Sleep(10000);
            }
            Http.Destroy(titleReleaseRequest);
        }

        if (TitleKey != "OEQCw9quJuaDak8Mz1KJTNIvXCzX")
        {
            Assert(false, "HELLO HACKER! Invalid title key.");
        }

        LayerCustomEvent(ReleaseLayer, "Hide", new[] { "" });

        MainMenuLayer = UILayerCreate();
        MainMenuLayer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";

        SoloMenuLayer = UILayerCreate();
        SoloMenuLayer.ManialinkPage = "file://Media/Manialinks/SoloMenu.xml";

        LoadingLayer = UILayerCreate();
        LoadingLayer.Type = CUILayer.EUILayerType.LoadingScreen;

        RequestTotd();
    }

    private static SUserInfo CreateUserInfo(CUser user)
    {
        SUserInfo userInfo = new()
        {
            Login = user.Login,
            Nickname = user.Name,
            Zone = user.ZonePath,
            AvatarUrl = user.AvatarUrl,
            Language = user.Language,
            Description = user.Description,
            Color = user.Color,
            SteamUserId = user.SteamUserId,
            FameStars = user.FameStars,
            LadderPoints = user.LadderPoints
        };

        return userInfo;
    }

    public void Loop()
    {
        // antilag weirdness when needing to request multiple leaderboards
        // events cannot use yield inside, so we collect lb request and process it after event loop
        var lbRequestMapUid = "";
        var lbRequestLaps = 0;
        ImmutableArray<string> lbRequestCars = new();

        foreach (var e in PendingEvents)
        {
            switch (e.Type)
            {
                case CManiaAppEvent.EType.LayerCustomEvent:
                    switch (e.CustomEventType)
                    {
                        case "MenuSolo":
                            Log("Switching to Solo Menu...");
                            LayerCustomEvent(SoloMenuLayer, "AnimateOpen", new[] { "" });
                            LayerCustomEvent(MainMenuLayer, "AnimateClose", new[] { "" });
                            break;
                        case "MainMenu":
                            SwitchToMainMenu();
                            break;
                        case "Quit":
                            Menu_Quit();
                            break;
                        case "PlayMap":
                            var playMapGroupNum = TextLib.ToInteger(e.CustomEventData[0]);
                            var playMapInfoNum = TextLib.ToInteger(e.CustomEventData[1]);
                            PlayMap(playMapGroupNum, playMapInfoNum);
                            break;
                        case "ExploreMap":
                            var exploreMapGroupNum = TextLib.ToInteger(e.CustomEventData[0]);
                            var exploreMapInfoNum = TextLib.ToInteger(e.CustomEventData[1]);
                            ExploreMap(exploreMapGroupNum, exploreMapInfoNum);
                            break;
                        case "SubmitCampaignMaps":
                            SubmitCampaignMaps();
                            break;
                        case "SubmitTitle":
                            SubmitTitle();
                            break;
                        case "Totd":
                            RequestTotd();
                            break;
                        case "Stats":
                            RequestStats();
                            break;
                        case "LoadLeaderboards":
                            lbRequestMapUid = e.CustomEventData[0];
                            lbRequestLaps = TextLib.ToInteger(e.CustomEventData[1]);
                            lbRequestCars.FromJson(e.CustomEventData[2]);
                            break;
                    }
                    break;
            }
        }

        if (lbRequestMapUid != "")
        {
            RequestLeaderboards(lbRequestMapUid, lbRequestLaps, lbRequestCars);
        }

        CheckToken();

        // so that there's a chance to refresh the token before trying to open the next map
        // must run after CheckToken()
        if (!ManiaPlanetAuthenticationRequested && Now - ManiaPlanetAuthReceivedAt < 1800000 && UserTokenRequest is null)
        {
            TryOpenRequestedMap();
        }

        if (SubmitMapsRequest is not null && SubmitMapsRequest.IsCompleted)
        {
            if (SubmitMapsRequest.StatusCode == 200)
            {
                Log("Campaign maps submitted successfully (200).");
            }
            else
            {
                Log($"Campaign maps submission failed ({SubmitMapsRequest.StatusCode}).");
            }
            Http.Destroy(SubmitMapsRequest);
            SubmitMapsRequest = null;
        }

        if (SubmitTitleRequest is not null && SubmitTitleRequest.IsCompleted)
        {
            if (SubmitTitleRequest.StatusCode == 200)
            {
                Log("Title submitted successfully (200).");
            }
            else
            {
                Log($"Title submission failed ({SubmitTitleRequest.StatusCode}).");
            }
            Http.Destroy(SubmitTitleRequest);
            SubmitTitleRequest = null;
        }

        if (TotdRequest is not null && TotdRequest.IsCompleted)
        {
            if (TotdRequest.StatusCode == 200)
            {
                Log("TOTD received (200).");
                LayerCustomEvent(MainMenuLayer, "Totd", new[] { TotdRequest.Result });
            }
            else
            {
                Log($"TOTD request failed ({TotdRequest.StatusCode}).");
                // TODO: retry?
            }
            Http.Destroy(TotdRequest);
            TotdRequest = null;
        }

        if (StatsRequest is not null && StatsRequest.IsCompleted)
        {
            if (StatsRequest.StatusCode == 200)
            {
                Log("Stats received (200).");

                STitleStats stats = new();
                stats.FromJson(StatsRequest.Result);

                ProcessTitleStats(stats);
            }
            else
            {
                Log($"Stats request failed ({StatsRequest.StatusCode}).");
            }
            Http.Destroy(StatsRequest);
            StatsRequest = null;
        }

        ImmutableArray<string> mapUidsToRemove = new();

        foreach (var (mapUid, requests) in LeaderboardRequests)
        {
            ImmutableArray<string> lbRequestsToRemove = new();

            foreach (var (car, request) in requests)
            {
                if (!request.IsCompleted)
                {
                    continue;
                }

                if (request.StatusCode == 200)
                {
                    Log($"Leaderboard from map {mapUid} for car {car} received (200).");
                    LayerCustomEvent(SoloMenuLayer, "LeaderboardData", new[] { mapUid, car, request.Result });
                }
                else
                {
                    Log($"Leaderboard request from map {mapUid} for car {car} failed ({request.StatusCode}).");
                }

                Http.Destroy(request);
                lbRequestsToRemove.Add(car);
            }

            foreach (var car in lbRequestsToRemove)
            {
                LeaderboardRequests[mapUid].Remove(car);
            }

            if (LeaderboardRequests.Count == 0)
            {
                mapUidsToRemove.Add(mapUid);
            }
        }

        foreach (var mapUid in mapUidsToRemove)
        {
            LeaderboardRequests.Remove(mapUid);
        }
    }

    private void RequestUserToken()
    {
        SAuthenticateUserRequest userRequest = new()
        {
            Token = ManiaPlanetAuthenticationToken,
            User = CreateUserInfo(LocalUser)
        };

        Log("Requesting user token...");

        UserTokenRequest = Http.CreatePost($"{EnvimixWebAPI}/users", userRequest.ToJson(), "Content-Type: application/json");
    }

    private void ResetUserTokenState()
    {
        UserTokenRequestTimeout = -1;
        UserTokenFirstRequestTimeout = -1;
        EnvimixTurboUserToken = "";
        UserTokenReceived = -1;
    }

    private void CheckToken()
    {
        // Zpracování odpovědi na požadavek ManiaPlanet tokenu
        if (ManiaPlanetAuthenticationRequested && Authentication_GetTokenResponseReceived)
        {
            ManiaPlanetAuthenticationRequested = false;

            if (Authentication_ErrorCode == 0)
            {
                Log("ManiaPlanet authentication token received.");
                ManiaPlanetAuthenticationToken = Authentication_Token;
                ManiaPlanetAuthReceivedAt = Now;
                RequestUserToken();
            }
            else
            {
                Log($"ManiaPlanet authentication token not received (error {Authentication_ErrorCode}).");
            }
        }

        // Periodické obnovení ManiaPlanet tokenu (např. každých 30 min)
        if (ManiaPlanetAuthReceivedAt != -1 && Now - ManiaPlanetAuthReceivedAt >= 1800000 && !ManiaPlanetAuthenticationRequested)
        {
            Log("Refreshing ManiaPlanet authentication token...");
            ManiaPlanetAuthenticationRequested = true;
            Authentication_GetToken(null, "Envimix");
        }

        // Retry uživatelského tokenu po chybě (10 s)
        if (UserTokenRequestTimeout != -1 && Now - UserTokenRequestTimeout >= 10000)
        {
            // Pokud už 30 min bez úspěchu – start od nuly (včetně reauth)
            if (Now - UserTokenFirstRequestTimeout >= 1800000)
            {
                ResetUserTokenState();
                ManiaPlanetAuthenticationRequested = true;
                Authentication_GetToken(null, "Envimix");
                return;
            }

            RequestUserToken();
            UserTokenRequestTimeout = -1;
        }

        // HTTP odpověď
        if (UserTokenRequest is not null && UserTokenRequest.IsCompleted)
        {
            if (UserTokenRequest.StatusCode != 200)
            {
                if (UserTokenRequest.StatusCode == 429)
                {
                    // should retry much later lol, weird to implement here though
                    Log("User token request rate limited (429). Retry in 10 seconds.");
                }
                else
                {
                    Log($"User token request failed ({UserTokenRequest.StatusCode}). Retry in 10 seconds.");
                }
                Http.Destroy(UserTokenRequest);
                UserTokenRequest = null;
                UserTokenRequestTimeout = Now;

                if (UserTokenFirstRequestTimeout == -1)
                    UserTokenFirstRequestTimeout = Now;

                return;
            }

            if (EnvimixTurboUserToken == "")
            {
                Log("User token created (200).");
            }
            else
            {
                Log("User token refreshed (200).");
            }

            SAuthenticateUserReponse response = new();
            if (!response.FromJson(UserTokenRequest.Result))
            {
                Log("User token creation failed (JSON issue).");
                Log(UserTokenRequest.Result);
            }
            else if (response.Login != LocalUser.Login)
            {
                Log("User token creation failed (server login mismatch).");
            }
            else
            {
                EnvimixTurboUserIsAdmin = response.IsAdmin;
                if (response.IsAdmin)
                {
                    Log("Admin detected! Extra features have been enabled.");
                }

                EnvimixTurboUserToken = response.Token!;
                if (UserTokenReceived == -1)
                    UserTokenReceived = Now;

                // token can be used immediately
            }

            Http.Destroy(UserTokenRequest);
            UserTokenRequest = null;
        }

        // Periodické obnovení uživatelského tokenu (bez ztráty session)
        // (20 min - 30 min, po 30 min už se refreshuje celej maniaplanet token)
        if (UserTokenReceived != -1 && Now - UserTokenReceived >= 1200000 && Now - UserTokenReceived < 1800000 && UserTokenRequest is null)
        {
            Log("Running user token refresh...");
            ResetUserTokenState();
            RequestUserToken();
        }
    }

    private void SwitchToMainMenu()
    {
        Log("Switching to Main Menu...");
        LayerCustomEvent(SoloMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(MainMenuLayer, "AnimateOpen", new[] { "" });
    }

    private void PlayMap(int mapGroupNum, int mapInfoNum)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = DataFileMgr.Campaigns[0];
        var mapInfo = campaign.MapGroups[mapGroupNum].MapInfos[mapInfoNum];

        LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);

        Wait(() => TitleControl.IsReady);
        TitleControl.PlayCampaign(campaign, mapInfo, "Modes/TrackMania/EnvimixSolo.Script.txt", "");
    }

    private void ExploreMap(int mapGroupNum, int mapInfoNum)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = DataFileMgr.Campaigns[0];
        var mapInfo = campaign.MapGroups[mapGroupNum].MapInfos[mapInfoNum];

        LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);

        Wait(() => TitleControl.IsReady);
        Log("Exploring map: " + mapInfo.FileName);
        TitleControl.EditNewMapFromBaseMap(mapInfo.FileName, ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "Explore.Script.txt", $"<settings><setting name=\"S_NewMapName\" type=\"text\" value=\"{mapInfo.Name}\"/></settings>");
    }

    private void TryOpenRequestedMap()
    {
        if (EnvimixOpenMapUid == "")
        {
            return;
        }

        Log("Trying to open requested map UID: " + EnvimixOpenMapUid);

        foreach (var campaign in DataFileMgr.Campaigns)
        {
            foreach (var mapGroup in campaign.MapGroups)
            {
                for (var i = 0; i < mapGroup.MapInfos.Count; i++)
                {
                    var mapInfo = mapGroup.MapInfos[i];
                    if (mapInfo.MapUid == EnvimixOpenMapUid)
                    {
                        EnvimixOpenMapUid = "";
                        PlayMap(campaign.MapGroups.IndexOf(mapGroup), i);
                        return;
                    }
                }
            }
        }

        Log("Requested map UID not found: " + EnvimixOpenMapUid);
        EnvimixOpenMapUid = "";
    }

    private void SubmitTitle()
    {
        SSubmitTitleRequest request = new()
        {
            TitleId = LoadedTitle.TitleId,
            Name = LoadedTitle.Name,
            Version = LoadedTitle.TitleVersion
        };

        SubmitTitleRequest = Http.CreatePost($"{EnvimixWebAPI}/titles", request.ToJson(), $"Authorization: Bearer {EnvimixTurboUserToken}\nContent-Type: application/json");
    }

    private void SubmitCampaignMaps()
    {
        SSubmitMapsRequest request = new()
        {
            TitleId = LoadedTitle.TitleId,
        };

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var order = 0;
        foreach (var group in DataFileMgr.Campaigns[0].MapGroups)
        {
            foreach (var map in group.MapInfos)
            {
                SMapInfo mapInfo = new()
                {
                    Name = map.Name,
                    Uid = map.MapUid,
                    Order = order
                };
                request.Maps!.Add(mapInfo);
                order += 1;
            }
        }

        SubmitMapsRequest = Http.CreatePost($"{EnvimixWebAPI}/maps", request.ToJson(), $"Authorization: Bearer {EnvimixTurboUserToken}\nContent-Type: application/json");
    }

    private void RequestTotd()
    {
        TotdRequest = Http.CreateGet($"{EnvimixWebAPI}/totd/{LoadedTitle.TitleId}");
    }

    private void RequestStats()
    {
        StatsRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/stats");
    }

    private static int GetLaps(CMapInfo mapInfo)
    {
        if (!mapInfo.TMObjective_IsLapRace)
        {
            return 1;
        }

        return mapInfo.TMObjective_NbLaps;
    }

    private void ProcessTitleStats(STitleStats stats)
    {
        TitleRatings = stats.Ratings;
        TitleStars = stats.Stars;
        TitleValidations = stats.Validations;

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = DataFileMgr.Campaigns[0];
        var mapInfos = new Dictionary<string, CMapInfo>();

        foreach (var mapGroup in campaign.MapGroups)
        {
            foreach (var mapInfo in mapGroup.MapInfos)
            {
                mapInfos[mapInfo.MapUid] = mapInfo;
            }
        }

        var skillpointsTotal = 0;
        var activityPointsTotal = 0;

        foreach (var (mapUid, skillpointsByCombination) in stats.Skillpoints)
        {
            if (!mapInfos.ContainsKey(mapUid))
            {
                continue;
            }

            var mapInfo = mapInfos[mapUid];

            var hasValidation = stats.Validations.ContainsKey(mapUid);

            foreach (var car in Cars)
            {
                var pbTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, $"{ScoreContextPrefix}{car}");

                var skillpointsAndValidationKey = $"{car}_0_{GetLaps(mapInfo)}";

                if (pbTime == -1 || !skillpointsByCombination.ContainsKey(skillpointsAndValidationKey))
                {
                    continue;
                }

                var skillpoints = skillpointsByCombination[skillpointsAndValidationKey];

                var pbCounting = true;
                var pbRankCounter = 0;
                var pbSkillpointRankCounter = 0;
                var totalRecCount = 0;

                for (var i = 0; i < skillpoints.Count / 2; i++)
                {
                    var time = skillpoints[i * 2];
                    var count = skillpoints[i * 2 + 1];

                    totalRecCount += count;

                    if (pbCounting)
                    {
                        pbSkillpointRankCounter += count;
                    }

                    // should be just ==, however in cases where some offline recs are not synced with envimania, this works better
                    if (time >= pbTime)
                    {
                        pbCounting = false;
                        continue;
                    }

                    if (pbCounting)
                    {
                        pbRankCounter += count;
                    }
                }

                if (pbSkillpointRankCounter == 0)
                {
                    pbSkillpointRankCounter = 1; // avoid div by 0
                }
                var skillpointsReal = (totalRecCount - pbSkillpointRankCounter) * 100f / pbSkillpointRankCounter;

                int ceilingSkillpoints;
                if (skillpointsReal == MathLib.TruncInteger(skillpointsReal))
                {
                    ceilingSkillpoints = MathLib.TruncInteger(skillpointsReal);
                }
                else
                {
                    ceilingSkillpoints = MathLib.CeilingInteger(skillpointsReal);
                }

                skillpointsTotal += ceilingSkillpoints;

                var wr = pbTime;
                if (skillpoints.Count > 0)
                {
                    wr = skillpoints[0]; // first from time+count pair
                }
                var wrPb = wr * 1f / pbTime;
                var activityPointsReal = 1000 * MathLib.Exp(totalRecCount * (wrPb - 1));
                var activityPoints = MathLib.NearestInteger(activityPointsReal);

                if (hasValidation && stats.Validations[mapUid].ContainsKey(skillpointsAndValidationKey))
                {
                    var validation = stats.Validations[mapUid][skillpointsAndValidationKey];

                    if (validation.Login == LocalUser.Login && validation.DrivenAt != "" && TitleRelease != "")
                    {
                        var validationTimestampInSeconds = validation.DrivenAt;
                        var titlePackReleaseTimestampInSeconds = TitleRelease;
                        var validationAge = TimeLib.GetDelta(validationTimestampInSeconds, titlePackReleaseTimestampInSeconds);
                        var extraActivityPointsReal = 10 + validationAge / 86400f * 10;
                        var extraActivityPointsInt = MathLib.NearestInteger(extraActivityPointsReal);
                        activityPoints += extraActivityPointsInt;
                    }
                }

                activityPointsTotal += activityPoints;
            }
        }

        LayerCustomEvent(SoloMenuLayer, "SetPoints", new[] { skillpointsTotal.ToString(), activityPointsTotal.ToString() });
    }

    private void RequestLeaderboards(string mapUid, int laps, ImmutableArray<string> cars)
    {
        foreach (var car in cars)
        {
            if (car == "")
            {
                continue;
            }

            if (LeaderboardRequests.ContainsKey(mapUid) && LeaderboardRequests[mapUid].ContainsKey(car))
            {
                Http.Destroy(LeaderboardRequests[mapUid][car]);
            }

            if (!LeaderboardRequests.ContainsKey(mapUid))
            {
                LeaderboardRequests[mapUid] = new();
            }

            LeaderboardRequests[mapUid][car] = Http.CreateGet($"{EnvimixWebAPI}/envimania/records/{mapUid}/{car}?gravity=0&laps={laps}");
            Yield(); // requesting more than 2 at once creates some lag
        }
    }
}
