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
        public string BanReason;
    }

    public struct SMapInfo
    {
        public string Name;
        public string Uid;
        public string Collection;
        public int Order;
        public string Campaign;
        public int AuthorTime;
        public int GoldTime;
        public int SilverTime;
        public int BronzeTime;
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

    public struct SPlayerScore
    {
        public string L;
        public int S;
    }

    public struct SPlayerCompletion
    {
        public string L;
        public float S;
    }

    public struct SPlayerMedals
    {
        public string L;
        public int D;
        public int ST;
        public int SG;
        public int SS;
        public int SB;
        public int A;
        public int G;
        public int S;
        public int B;
    }

    public struct SCombinationStat
    {
        public string VL;
        public string VD;
        public float D;
        public float Q;
        public IList<int> S;
    }

    public struct STitleUserInfo
    {
        public string N;
        public string Z;
    }

    public struct SCombinationRecordCount
    {
        public int E;
        public int D;
        public int G;
    }

    public struct STitleGeneralStats
    {
        public Dictionary<string, STitleUserInfo> Players;
        public Dictionary<string, Dictionary<string, SCombinationStat>> Combinations;
        public Dictionary<string, Dictionary<string, string>> Stars;
    }

    public struct STitleSkillpointStats
    {
        public IList<SPlayerScore> EnvimixMostSkillpoints;
        public IList<SPlayerScore> DefaultCarMostSkillpoints;
        public IList<SPlayerScore> GlobalMostSkillpoints;
        public Dictionary<string, IList<SPlayerScore>> EnvimixCombinationMostSkillpoints;
        public Dictionary<string, IList<SPlayerScore>> DefaultCarCombinationMostSkillpoints;
        public Dictionary<string, IList<SPlayerScore>> GlobalCombinationMostSkillpoints;
    }

    public struct STitleCompletionStats
    {
        public float EnvimixCompletionPercentage;
        public float DefaultCarCompletionPercentage;
        public float GlobalCompletionPercentage;
        public Dictionary<string, float> EnvimixCompletionPercentages;
        public Dictionary<string, float> DefaultCarCompletionPercentages;
        public Dictionary<string, float> GlobalCompletionPercentages;
        public IList<SPlayerCompletion> EnvimixCompletion;
        public IList<SPlayerMedals> DefaultCarCompletion;
        public IList<SPlayerCompletion> GlobalCompletion;
        public Dictionary<string, SCombinationRecordCount> CombinationRecordCount;
        public Dictionary<string, IList<SPlayerCompletion>> EnvimixCombinationCompletion;
        public Dictionary<string, IList<SPlayerMedals>> DefaultCarCombinationCompletion;
        public Dictionary<string, IList<SPlayerCompletion>> GlobalCombinationCompletion;
    }

    public struct STitleActivityPointStats
    {
        public IList<SPlayerScore> EnvimixMostActivityPoints;
        public IList<SPlayerScore> DefaultCarMostActivityPoints;
        public IList<SPlayerScore> GlobalMostActivityPoints;
        public Dictionary<string, IList<SPlayerScore>> EnvimixCombinationMostActivityPoints;
        public Dictionary<string, IList<SPlayerScore>> DefaultCarCombinationMostActivityPoints;
        public Dictionary<string, IList<SPlayerScore>> GlobalCombinationMostActivityPoints;
    }

    public bool ManiaPlanetAuthenticationRequested;
    public required string ManiaPlanetAuthenticationToken;
    public CHttpRequest? UserTokenRequest;
    public int UserTokenRequestTimeout = -1;
    public int UserTokenFirstRequestTimeout = -1;
    public int UserTokenReceived = -1;
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";
    [Local(LocalFor.LocalUser)] public bool EnvimixTurboUserIsAdmin { get; set; }
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserBanReason { get; set; } = "";
    public int ManiaPlanetAuthReceivedAt = -1;

    [Local(LocalFor.LocalUser)] public Dictionary<string, string> CampaignsReleasedAt { get; set; }

    [Local(LocalFor.LocalUser)] public string EnvimixOpenMapUid { get; set; } = "";

    /*[Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SRating>> TitleRatings { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SValidationInfo>> TitleValidations { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, IList<int>>> TitleSkillpoints { get; set; }

    [Local(LocalFor.LocalUser)] public IList<SPlayerCompletion> EnvimixCompletion { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostSkillpoints { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostActivityPoints { get; set; }*/

    [Local(LocalFor.LocalUser)] public bool IntroEnded { get; set; }

    public CUILayer MainMenuLayer;
    public CUILayer SoloMenuLayer;
    public CUILayer LoadingLayer;
    public CUILayer LeaderboardsLayer;
    public CUILayer LocalPlayMenuLayer;
    public CUILayer OnlinePlayMenuLayer;
    public CUILayer EditorsMenuLayer;

    public CHttpRequest? SubmitMapsRequest;
    public CHttpRequest? SubmitTitleRequest;
    public CHttpRequest? TotdRequest;
    public CHttpRequest? StatsGeneralRequest;
    public CHttpRequest? StatsSkillpointsRequest;
    public CHttpRequest? StatsActivityPointsRequest;
    public CHttpRequest? StatsCompletionRequest;
    public Dictionary<string, Dictionary<string, CHttpRequest>> LeaderboardRequests;
    public CHttpRequest? RestoreRecordsRequest;

    public string ScoreContextPrefix = "";

    public ImmutableArray<string> Cars;

    public const string EnvimixWebAPI = "https://api.envimix.gbx.tools";

    public const int OfflineHttpCode = 10006;

    public void Main()
    {        
        Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

        ManiaPlanetAuthenticationRequested = true;
        Authentication_GetToken(null, "Envimix");

        var preloadLayer = UILayerCreate();
        preloadLayer.ManialinkPage = "file://Media/Manialinks/PreloadThumbnails.xml";

        Yield();
        while (preloadLayer.IsLocalPageScriptRunning)
        {
            Yield();
        }

        UILayerDestroy(preloadLayer);

        var introLayer = UILayerCreate();
        introLayer.ManialinkPage = "file://Media/Manialinks/Intro.xml";

        MainMenuLayer = UILayerCreate();
        MainMenuLayer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";

        SoloMenuLayer = UILayerCreate();
        SoloMenuLayer.ManialinkPage = "file://Media/Manialinks/SoloMenu.xml";

        LeaderboardsLayer = UILayerCreate();
        LeaderboardsLayer.ManialinkPage = "file://Media/Manialinks/Leaderboards.xml";

        LoadingLayer = UILayerCreate();
        LoadingLayer.Type = CUILayer.EUILayerType.LoadingScreen;

        LocalPlayMenuLayer = UILayerCreate();
        LocalPlayMenuLayer.ManialinkPage = "file://Media/Manialinks/LocalPlayMenu.xml";

        OnlinePlayMenuLayer = UILayerCreate();
        OnlinePlayMenuLayer.ManialinkPage = "file://Media/Manialinks/OnlinePlayMenu.xml";

        EditorsMenuLayer = UILayerCreate();
        EditorsMenuLayer.ManialinkPage = "file://Media/Manialinks/EditorsMenu.xml";

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
                            LayerCustomEvent(MainMenuLayer, "PlayMenuChanged", new[] { "" });
                            LayerCustomEvent(LocalPlayMenuLayer, "AnimateClose", new[] { "" });
                            LayerCustomEvent(OnlinePlayMenuLayer, "AnimateClose", new[] { "" });
                            LayerCustomEvent(EditorsMenuLayer, "AnimateClose", new[] { "" });
                            break;
                        case "MenuLocal":
                            SwitchToPlayMenu("Local");
                            break;
                        case "MenuLocalLegacy":
                            LoadingLayer.IsVisible = false;
                            Menu_Local();
                            break;
                        case "MenuInternet":
                            SwitchToPlayMenu("Online");
                            break;
                        case "MenuInternetLegacy":
                            LoadingLayer.IsVisible = false;
                            Menu_Internet();
                            break;
                        case "MenuEditor":
                            SwitchToPlayMenu("Editors");
                            break;
                        case "MenuEditorLegacy":
                            LoadingLayer.IsVisible = false;
                            Menu_Editor();
                            break;
                        case "MainMenu":
                            SwitchToMainMenu();
                            break;
                        case "Leaderboards":
                            SwitchToLeaderboards();
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
                        case "RestoreRecords":
                            RestoreRecords();
                            break;
                        case "LoadLeaderboards":
                            lbRequestMapUid = e.CustomEventData[0];
                            lbRequestLaps = TextLib.ToInteger(e.CustomEventData[1]);
                            lbRequestCars.FromJson(e.CustomEventData[2]);
                            break;
                        case "Quickplay":
                            Quickplay();
                            break;
                        case "PlayLocalMap":
                            PlayLocalMap(e.CustomEventData[0]);
                            break;
                        case "ViewGhost":
                            var viewGhostGroupNum = TextLib.ToInteger(e.CustomEventData[0]);
                            var viewGhostInfoNum = TextLib.ToInteger(e.CustomEventData[1]);
                            var car = e.CustomEventData[2];
                            var ghostUrl = e.CustomEventData[3];
                            ViewGhost(viewGhostGroupNum, viewGhostInfoNum, car, ghostUrl);
                            break;
                        case "EditReplay":
                            ImmutableArray<string> replayPaths = new();
                            foreach (var path in e.CustomEventData)
                            {
                                replayPaths.Add(path);
                            }
                            EditReplay(replayPaths);
                            break;
                        case "EditMap":
                            EditMap(e.CustomEventData[0]);
                            break;
                        case "NewMap":
                            if (TitleControl.IsReady)
                            {
                                TitleControl.EditNewMap(e.CustomEventData[0], "", "", "", "", "", "");
                            }
                            break;
                        case "InterfaceDesigner":
                            if (TitleControl.IsReady)
                            {
                                TitleControl.OpenEditor(CTitleControl.EEditorType.InterfaceDesigner);
                            }
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
                if (TotdRequest.StatusCode == OfflineHttpCode)
                {
                    LayerCustomEvent(MainMenuLayer, "TotdError", new[] { "in offline mode" });
                }
                else
                {
                    LayerCustomEvent(MainMenuLayer, "TotdError", new[] { $"(error code: {TotdRequest.StatusCode})" });
                }
                // TODO: retry?
            }
            Http.Destroy(TotdRequest);
            TotdRequest = null;
        }

        if (StatsGeneralRequest is not null && StatsGeneralRequest.IsCompleted)
        {
            if (StatsGeneralRequest.StatusCode == 200)
            {
                Log("General stats received (200).");

                STitleGeneralStats stats = new();
                stats.FromJson(StatsGeneralRequest.Result);
                Yield();
                ProcessTitleGeneralStats(stats);
            }
            else
            {
                Log($"General stats request failed ({StatsGeneralRequest.StatusCode}).");
            }
            Http.Destroy(StatsGeneralRequest);
            StatsGeneralRequest = null;
        }

        if (StatsSkillpointsRequest is not null && StatsSkillpointsRequest.IsCompleted)
        {
            if (StatsSkillpointsRequest.StatusCode == 200)
            {
                Log("Skillpoint stats received (200).");
                STitleSkillpointStats stats = new();
                stats.FromJson(StatsSkillpointsRequest.Result);
                Yield();
                ProcessTitleSkillpointStats(stats);
            }
            else
            {
                Log($"Skillpoint stats request failed ({StatsSkillpointsRequest.StatusCode}).");
            }
            Http.Destroy(StatsSkillpointsRequest);
            StatsSkillpointsRequest = null;
        }

        if (StatsActivityPointsRequest is not null && StatsActivityPointsRequest.IsCompleted)
        {
            if (StatsActivityPointsRequest.StatusCode == 200)
            {
                Log("Activity point stats received (200).");
                STitleActivityPointStats stats = new();
                stats.FromJson(StatsActivityPointsRequest.Result);
                Yield();
                ProcessTitleActivityPointStats(stats);
            }
            else
            {
                Log($"Activity point stats request failed ({StatsActivityPointsRequest.StatusCode}).");
            }
            Http.Destroy(StatsActivityPointsRequest);
            StatsActivityPointsRequest = null;
        }

        if (StatsCompletionRequest is not null && StatsCompletionRequest.IsCompleted)
        {
            if (StatsCompletionRequest.StatusCode == 200)
            {
                Log("Completion stats received (200).");
                STitleCompletionStats stats = new();
                stats.FromJson(StatsCompletionRequest.Result);
                Yield();
                ProcessTitleCompletionStats(stats);
            }
            else
            {
                Log($"Completion stats request failed ({StatsCompletionRequest.StatusCode}).");
            }
            Http.Destroy(StatsCompletionRequest);
            StatsCompletionRequest = null;
        }

        if (RestoreRecordsRequest is not null && RestoreRecordsRequest.IsCompleted)
        {
            if (RestoreRecordsRequest.StatusCode == 200)
            {
                Log("Restoring records began (200).");
            }
            else
            {
                Log($"Restoring records request failed ({RestoreRecordsRequest.StatusCode}).");
            }
            Http.Destroy(RestoreRecordsRequest);
            RestoreRecordsRequest = null;
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
                    LayerCustomEvent(SoloMenuLayer, "LeaderboardError", new[] { mapUid, car, request.StatusCode.ToString() });
                }

                Http.Destroy(request);
                lbRequestsToRemove.Add(car);
            }

            foreach (var car in lbRequestsToRemove)
            {
                LeaderboardRequests[mapUid].Remove(car);
            }

            if (LeaderboardRequests[mapUid].Count == 0)
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
                LayerCustomEvent(OnlinePlayMenuLayer, "Authenticate", new[] { ManiaPlanetAuthenticationToken });
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
                Log($"User token creation has a JSON issue.");
            }
            
            if (response.Login != LocalUser.Login)
            {
                Log($"User token creation failed (login mismatch, local: {LocalUser.Login} != server: {response.Login}).");
            }
            else
            {
                EnvimixTurboUserIsAdmin = response.IsAdmin;
                if (response.IsAdmin)
                {
                    Log("Admin detected! Extra features have been enabled.");
                }

                EnvimixTurboUserBanReason = response.BanReason!;

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
        LayerCustomEvent(LeaderboardsLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(MainMenuLayer, "AnimateOpen", new[] { "" });
        LayerCustomEvent(MainMenuLayer, "PlayMenuChanged", new[] { "" });
        LayerCustomEvent(LocalPlayMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(OnlinePlayMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(EditorsMenuLayer, "AnimateClose", new[] { "" });
    }

    private void SwitchToPlayMenu(string menu)
    {
        if (menu == "Local")
            LayerCustomEvent(LocalPlayMenuLayer, "AnimateOpen", new[] { "" });
        else
            LayerCustomEvent(LocalPlayMenuLayer, "AnimateClose", new[] { "" });

        if (menu == "Online")
        {
            LayerCustomEvent(OnlinePlayMenuLayer, "Authenticate", new[] { ManiaPlanetAuthenticationToken });
            LayerCustomEvent(OnlinePlayMenuLayer, "AnimateOpen", new[] { "" });
        }
        else
        {
            LayerCustomEvent(OnlinePlayMenuLayer, "AnimateClose", new[] { "" });
        }

        if (menu == "Editors")
            LayerCustomEvent(EditorsMenuLayer, "AnimateOpen", new[] { "" });
        else
            LayerCustomEvent(EditorsMenuLayer, "AnimateClose", new[] { "" });

        LayerCustomEvent(MainMenuLayer, "PlayMenuChanged", new[] { menu });
    }

    private void SwitchToLeaderboards()
    {
        Log("Switching to Leaderboards...");
        LayerCustomEvent(MainMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(MainMenuLayer, "PlayMenuChanged", new[] { "" });
        LayerCustomEvent(LeaderboardsLayer, "AnimateOpen", new[] { "" });
        LayerCustomEvent(LocalPlayMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(OnlinePlayMenuLayer, "AnimateClose", new[] { "" });
        LayerCustomEvent(EditorsMenuLayer, "AnimateClose", new[] { "" });
    }

    public bool PlayMapInProgress;

    private void PlayMap(CCampaign campaign, CMapInfo mapInfo)
    {
        if (PlayMapInProgress)
        {
            return;
        }

        PlayMapInProgress = true;

        LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);
        LoadingLayer.IsVisible = true;

        if (TitleControl.IsReady)
        {
            TitleControl.PlayCampaign(campaign, mapInfo, "", "");
        }

        PlayMapInProgress = false;
    }

    private CCampaign GetCampaignForMaps()
    {
        var selectedCampaign = Local<string>.For(SoloMenuLayer.LocalPage);

        if (selectedCampaign.Get() == "")
        {
            return DataFileMgr.Campaigns[0];
        }

        if (selectedCampaign.Get() == "VR")
        {
            return DataFileMgr.Campaigns[12];
        }

        if (selectedCampaign.Get() == "VROffzone")
        {
            return DataFileMgr.Campaigns[24];
        }

        return DataFileMgr.Campaigns[0];
    }

    private void PlayMap(int mapGroupNum, int mapInfoNum)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = GetCampaignForMaps();
        var mapInfo = campaign.MapGroups[mapGroupNum].MapInfos[mapInfoNum];

        PlayMap(campaign, mapInfo);
    }

    private void PlayLocalMap(string filePath)
    {
        if (PlayMapInProgress)
        {
            return;
        }

        PlayMapInProgress = true;

        IList<string> splitPath = TextLib.Split("\\", filePath);
        splitPath.RemoveAt(splitPath.Count - 1);
        var joinedPath = TextLib.Join("\\", (string[])splitPath);

        var mapInfoInList = DataFileMgr.Map_GetGameList(joinedPath, false);
        Wait(() => !mapInfoInList.IsProcessing);

        foreach (var mapInfo in mapInfoInList.MapInfos)
        {
            if (mapInfo.FileName == filePath)
            {
                LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);
                LoadingLayer.IsVisible = true;

                DataFileMgr.TaskResult_Release(mapInfoInList.Id);
                break;
            }
        }

        if (TitleControl.IsReady)
        {
            TitleControl.PlayMap(filePath, "", "");
        }

        PlayMapInProgress = false;
    }

    private void EditReplay(IList<string> filePaths)
    {
        if (filePaths.Count == 0 || PlayMapInProgress)
        {
            return;
        }

        PlayMapInProgress = true;

        var filePath = filePaths[0];

        IList<string> splitPath = TextLib.Split("\\", filePath);
        splitPath.RemoveAt(splitPath.Count - 1);
        var joinedPath = TextLib.Join("\\", (string[])splitPath);

        var replayInfoInList = DataFileMgr.Replay_GetGameList(joinedPath, false);
        Wait(() => !replayInfoInList.IsProcessing);

        var mapFound = false;
        foreach (var replayInfo in replayInfoInList.ReplayInfos)
        {
            if (replayInfo.FileName == filePath)
            {
                foreach (var mapInfo in DataFileMgr.Campaigns)
                {
                    foreach (var mapGroup in mapInfo.MapGroups)
                    {
                        foreach (var map in mapGroup.MapInfos)
                        {
                            if (map.MapUid == replayInfo.MapUid)
                            {
                                LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(map, System.CurrentLocalDateText);
                                LoadingLayer.IsVisible = true;
                                mapFound = true;
                                break;
                            }
                        }

                        if (mapFound)
                        {
                            break;
                        }
                    }

                    if (mapFound)
                    {
                        break;
                    }
                }

                if (!mapFound)
                {
                    LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(replayInfo.MapUid, System.CurrentLocalDateText);
                    LoadingLayer.IsVisible = true;
                }

                DataFileMgr.TaskResult_Release(replayInfoInList.Id);
                break;
            }
        }

        if (TitleControl.IsReady)
        {
            TitleControl.EditReplay((string[])filePaths);
        }

        PlayMapInProgress = false;
    }

    private void EditMap(string filePath)
    {
        if (PlayMapInProgress)
        {
            return;
        }

        PlayMapInProgress = true;

        IList<string> splitPath = TextLib.Split("\\", filePath);
        splitPath.RemoveAt(splitPath.Count - 1);
        var joinedPath = TextLib.Join("\\", (string[])splitPath);

        var mapInfoInList = DataFileMgr.Map_GetGameList(joinedPath, false);
        Wait(() => !mapInfoInList.IsProcessing);

        foreach (var mapInfo in mapInfoInList.MapInfos)
        {
            if (mapInfo.FileName == filePath)
            {
                LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);
                LoadingLayer.IsVisible = true;

                DataFileMgr.TaskResult_Release(mapInfoInList.Id);
                break;
            }
        }

        if (TitleControl.IsReady)
        {
            TitleControl.EditMap(filePath, "", "");
        }

        PlayMapInProgress = false;
    }

    private void ExploreMap(int mapGroupNum, int mapInfoNum)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = GetCampaignForMaps();
        var mapInfo = campaign.MapGroups[mapGroupNum].MapInfos[mapInfoNum];

        LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);
        LoadingLayer.IsVisible = true;

        Wait(() => TitleControl.IsReady);
        Log("Exploring map: " + mapInfo.FileName);
        TitleControl.EditNewMapFromBaseMap(mapInfo.FileName, ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "Explore.Script.txt", $"<settings><setting name=\"S_OriginalMapName\" type=\"text\" value=\"{mapInfo.Name}\"/><setting name=\"S_OriginalMapUid\" type=\"text\" value=\"{mapInfo.MapUid}\"/><setting name=\"S_OriginalAuthorNickName\" type=\"text\" value=\"{mapInfo.AuthorNickName}\"/></settings>");
    }

    private void ViewGhost(int mapGroupNum, int mapInfoNum, string car, string ghostUrl)
    {
        if (ghostUrl == "" || DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = GetCampaignForMaps();
        var mapInfo = campaign.MapGroups[mapGroupNum].MapInfos[mapInfoNum];

        LoadingLayer.ManialinkPage = Loading.GetLoadingManialink(mapInfo, System.CurrentLocalDateText);
        LoadingLayer.IsVisible = true;

        Wait(() => TitleControl.IsReady);
        TitleControl.PlayMap(mapInfo.FileName, "Modes/TrackMania/ViewGhost.Script.txt", $"<mode_script_settings><setting name=\"S_Car\" type=\"text\" value=\"{car}\"/><setting name=\"S_GhostUrl\" type=\"text\" value=\"{ghostUrl}\"/></mode_script_settings>");
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

        ImmutableArray<CCampaign> campaigns = new();
        campaigns.Add(DataFileMgr.Campaigns[0]);
        campaigns.Add(DataFileMgr.Campaigns[12]);
        campaigns.Add(DataFileMgr.Campaigns[24]);

        foreach (var campaign in campaigns)
        {
            var order = 0;
            foreach (var group in campaign.MapGroups)
            {
                foreach (var map in group.MapInfos)
                {
                    var campaignName = "";
                    if (campaign == DataFileMgr.Campaigns[12])
                    {
                        campaignName = "VR";
                    }

                    if (campaign == DataFileMgr.Campaigns[24])
                    {
                        campaignName = "VROffzone";
                    }

                    SMapInfo mapInfo = new()
                    {
                        Name = map.Name,
                        Uid = map.MapUid,
                        Collection = map.CollectionName,
                        Order = order,
                        Campaign = campaignName,
                        AuthorTime = map.TMObjective_AuthorTime,
                        GoldTime = map.TMObjective_GoldTime,
                        SilverTime = map.TMObjective_SilverTime,
                        BronzeTime = map.TMObjective_BronzeTime
                    };
                    request.Maps!.Add(mapInfo);
                    order += 1;
                }
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
        StatsGeneralRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/stats/general");
        StatsSkillpointsRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/stats/skillpoints");
        StatsActivityPointsRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/stats/activity-points");
        StatsCompletionRequest = Http.CreateGet($"{EnvimixWebAPI}/titles/{LoadedTitle.TitleId}/stats/completion");
    }

    private void RestoreRecords()
    {
        RestoreRecordsRequest = Http.CreatePost($"{EnvimixWebAPI}/envimania/restore-records", "", $"Authorization: Bearer {EnvimixTurboUserToken}");
    }

    private void EvaluateOfflinePoints(STitleGeneralStats stats)
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var skillpointsTotal = 0;
        var activityPointsTotal = 0;

        for (var i = 0; i < DataFileMgr.Campaigns.Count / 12; i++)
        {
            var campaign = DataFileMgr.Campaigns[i * 12];

            var campaignReleaseAt = "";
            if (i == 0)
            {
                campaignReleaseAt = "1764349200";
            }
            else if (i == 1 && CampaignsReleasedAt.ContainsKey("VR"))
            {
                campaignReleaseAt = CampaignsReleasedAt["VR"];
            }

            foreach (var mapGroup in campaign.MapGroups)
            {
                Yield();
                foreach (var mapInfo in mapGroup.MapInfos)
                {
                    foreach (var car in Cars)
                    {
                        var scoreContext = $"{ScoreContextPrefix}{car}";
                        var isDefaultCar = false;

                        // hacky but it works for TMT
                        if ((mapInfo.CollectionName == "Canyon" && car == "CanyonCar")
                            || (mapInfo.CollectionName == "Stadium" && car == "StadiumCar")
                            || (mapInfo.CollectionName == "Valley" && car == "ValleyCar")
                            || (mapInfo.CollectionName == "Lagoon" && car == "LagoonCar"))
                        {
                            scoreContext = ScoreContextPrefix;
                            isDefaultCar = true;
                        }

                        var pbTime = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, scoreContext);

                        if (pbTime == -1)
                        {
                            continue;
                        }

                        var combinationKey = $"{car}_0";

                        if (!stats.Combinations.ContainsKey(mapInfo.MapUid) || !stats.Combinations[mapInfo.MapUid].ContainsKey(combinationKey))
                        {
                            continue;
                        }

                        var combination = stats.Combinations[mapInfo.MapUid][combinationKey];
                        var skillpoints = combination.S;

                        var pbCounting = true;
                        var pbRankCounter = 0;
                        var pbSkillpointRankCounter = 0;
                        var totalRecCount = 0;

                        for (var j = 0; j < skillpoints.Count / 2; j++)
                        {
                            var time = skillpoints[j * 2];
                            var count = skillpoints[j * 2 + 1];

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

                        var validationLogin = combination.VL;
                        var validationTimestampInSeconds = combination.VD;

                        if (!isDefaultCar && validationLogin == LocalUser.Login && validationTimestampInSeconds != "" && campaignReleaseAt != "")
                        {
                            var campaignReleaseTimestampInSeconds = campaignReleaseAt;
                            var validationAge = TimeLib.GetDelta(validationTimestampInSeconds, campaignReleaseTimestampInSeconds);
                            var extraActivityPointsReal = 100 + validationAge / 86400f * 10;
                            var extraActivityPointsInt = MathLib.NearestInteger(extraActivityPointsReal);
                            activityPoints += extraActivityPointsInt;
                        }

                        activityPointsTotal += activityPoints;
                    }
                }
            }
        }

        LayerCustomEvent(SoloMenuLayer, "SetPoints", new[] { skillpointsTotal.ToString(), activityPointsTotal.ToString() });
    }

    private void ProcessTitleGeneralStats(STitleGeneralStats stats)
    {
        var titleStars = Local<Dictionary<string, Dictionary<string, string>>>.For(SoloMenuLayer.LocalPage);
        titleStars.Set(stats.Stars);

        var titleCombinations = Local<Dictionary<string, Dictionary<string, SCombinationStat>>>.For(SoloMenuLayer.LocalPage);
        titleCombinations.Set(stats.Combinations);

        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(LeaderboardsLayer.LocalPage);
        leaderboardsUserInfos.Set(stats.Players);

        var soloUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(SoloMenuLayer.LocalPage);
        soloUserInfos.Set(stats.Players);

        LayerCustomEvent(SoloMenuLayer, "GeneralStats", new[] { "" });
        LayerCustomEvent(LeaderboardsLayer, "UserInfos", new[] { "" });
    }

    private void ProcessTitleCompletionStats(STitleCompletionStats stats)
    {
        var envimixCompletion = Local<IList<SPlayerCompletion>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarCompletion = Local<IList<SPlayerMedals>>.For(LeaderboardsLayer.LocalPage);
        var globalCompletion = Local<IList<SPlayerCompletion>>.For(LeaderboardsLayer.LocalPage);

        envimixCompletion.Set(stats.EnvimixCompletion);
        defaultCarCompletion.Set(stats.DefaultCarCompletion);
        globalCompletion.Set(stats.GlobalCompletion);

        var combinationRecordCount = Local<Dictionary<string, SCombinationRecordCount>>.For(LeaderboardsLayer.LocalPage);
        combinationRecordCount.Set(stats.CombinationRecordCount);

        var envimixCombinationCompletion = Local<Dictionary<string, IList<SPlayerCompletion>>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarCombinationCompletion = Local<Dictionary<string, IList<SPlayerMedals>>>.For(LeaderboardsLayer.LocalPage);
        var globalCombinationCompletion = Local<Dictionary<string, IList<SPlayerCompletion>>>.For(LeaderboardsLayer.LocalPage);

        envimixCombinationCompletion.Set(stats.EnvimixCombinationCompletion);
        defaultCarCombinationCompletion.Set(stats.DefaultCarCombinationCompletion);
        globalCombinationCompletion.Set(stats.GlobalCombinationCompletion);

        LayerCustomEvent(LeaderboardsLayer, "CompletionStats", new[] { stats.EnvimixCompletionPercentage.ToString(), stats.DefaultCarCompletionPercentage.ToString(), stats.GlobalCompletionPercentage.ToString(), stats.EnvimixCompletionPercentages.ToJson(), stats.DefaultCarCompletionPercentages.ToJson(), stats.GlobalCompletionPercentages.ToJson() });
    }

    private void ProcessTitleSkillpointStats(STitleSkillpointStats stats)
    {
        var envimixMostSkillpoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarMostSkillpoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);
        var globalMostSkillpoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);

        envimixMostSkillpoints.Set(stats.EnvimixMostSkillpoints);
        defaultCarMostSkillpoints.Set(stats.DefaultCarMostSkillpoints);
        globalMostSkillpoints.Set(stats.GlobalMostSkillpoints);

        var envimixCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);
        var globalCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);

        envimixCombinationMostSkillpoints.Set(stats.EnvimixCombinationMostSkillpoints);
        defaultCarCombinationMostSkillpoints.Set(stats.DefaultCarCombinationMostSkillpoints);
        globalCombinationMostSkillpoints.Set(stats.GlobalCombinationMostSkillpoints);

        LayerCustomEvent(LeaderboardsLayer, "SkillpointStats", new[] { "" });

        foreach (var playerScore in stats.GlobalMostSkillpoints)
        {
            if (playerScore.L == LocalUser.Login)
            {
                LayerCustomEvent(SoloMenuLayer, "Skillpoints", new[] { playerScore.S.ToString() });
                break;
            }
        }
    }

    private void ProcessTitleActivityPointStats(STitleActivityPointStats stats)
    {
        var envimixMostActivityPoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarMostActivityPoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);
        var globalMostActivityPoints = Local<IList<SPlayerScore>>.For(LeaderboardsLayer.LocalPage);

        envimixMostActivityPoints.Set(stats.EnvimixMostActivityPoints);
        defaultCarMostActivityPoints.Set(stats.DefaultCarMostActivityPoints);
        globalMostActivityPoints.Set(stats.GlobalMostActivityPoints);

        var envimixCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);
        var defaultCarCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);
        var globalCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(LeaderboardsLayer.LocalPage);

        envimixCombinationMostActivityPoints.Set(stats.EnvimixCombinationMostActivityPoints);
        defaultCarCombinationMostActivityPoints.Set(stats.DefaultCarCombinationMostActivityPoints);
        globalCombinationMostActivityPoints.Set(stats.GlobalCombinationMostActivityPoints);

        LayerCustomEvent(LeaderboardsLayer, "ActivityPointStats", new[] { "" });

        foreach (var playerScore in stats.GlobalMostActivityPoints)
        {
            if (playerScore.L == LocalUser.Login)
            {
                LayerCustomEvent(SoloMenuLayer, "ActivityPoints", new[] { playerScore.S.ToString() });
                break;
            }
        }
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

    private void Quickplay()
    {
        if (MathLib.Rand(0, 50) == 0)
        {
            Log("Going to a mysterious place...");
            var xml = "<?xml version='1.0' encoding='utf-8' ?>";
            xml = $"{xml}<maniacode noconfirmation=\"1\">";
            xml = $"{xml}<play_map><name>0</name><url>https://api.envimix.gbx.tools/maps/qlLsZ8gthEsvOcfugdadY4DqWU8/download</url></play_map>";
            xml = $"{xml}</maniacode>";
            LoadingLayer.ManialinkPage = Loading.GetLoadingManialink("qlLsZ8gthEsvOcfugdadY4DqWU8", System.CurrentLocalDateText);
            LoadingLayer.IsVisible = true;
            Audio.PlaySoundEvent("file://Media/Sounds/Voices/CleanSweep.wav", 1);
            if (TitleControl.IsReady)
            {
                TitleControl.ProcessManiaCodeXml(xml);
            }
            return;
        }

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        var campaign = GetCampaignForMaps();
        ImmutableArray<CMapInfo> mapInfos = new();

        foreach (var mapGroup in campaign.MapGroups)
        {
            foreach (var mapInfo in mapGroup.MapInfos)
            {
                mapInfos.Add(mapInfo);
            }
        }

        if (mapInfos.Length == 0)
        {
            return;
        }

        var randomIndex = MathLib.Rand(0, mapInfos.Length - 1);
        var mapToPlay = mapInfos[randomIndex];

        PlayMap(campaign, mapToPlay);
    }
}
