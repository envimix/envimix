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
    }

    public struct STitleReleaseInfo
    {
        public string ReleasedAt;
    }

    public bool ManiaPlanetAuthenticationRequested;
    public required string ManiaPlanetAuthenticationToken;
    public CHttpRequest? UserTokenRequest;
    public int UserTokenRequestTimeout = -1;
    public int UserTokenFirstRequestTimeout = -1;
    public int UserTokenReceived = -1;
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";
    public int ManiaPlanetAuthReceivedAt = -1;

    [Local(LocalFor.LocalUser)] public string TitleRelease { get; set; } = "";

    public CUILayer MainMenuLayer;
    public CUILayer SoloMenuLayer;
    public CUILayer LoadingLayer;
    public CUILayer ReleaseLayer;

    public string MenuLocation = "MainMenu";

    public const string EnvimixWebAPI = "https://api.envimix.gbx.tools";

    public void Main()
    {        
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

        while (TitleRelease == "" || TimeLib.Compare(TimeLib.GetCurrent(), TitleRelease) <= 0)
        {
            // this is some REAL maniascript bullshit bug right here
            if (TitleRelease != "" && TimeLib.Compare(TimeLib.GetCurrent(), TitleRelease) > 0)
            {
                break;
            }

            // if we received release info recently, wait a bit before requesting again
            if (releaseReceivedAt != -1 && Now - releaseReceivedAt < 30000)
            {
                Yield();
                foreach (var e in PendingEvents)
                {
                    if (e.Type == CManiaAppEvent.EType.LayerCustomEvent)
                    {
                        if (e.CustomEventLayer == introLayer && e.CustomEventType == "IntroEnded")
                        {
                            LayerCustomEvent(ReleaseLayer, "Show", new[] { "" });
                            break;
                        }
                    }
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
                    Log("Title release info received.");
                    TitleRelease = releaseInfo.ReleasedAt!;
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
                Log("No title release info found. This assumes the title pack is playable.");
                releaseReceivedAt = Now;
            }
            else
            {
                Log($"Failed to fetch title release info ({titleReleaseRequest.StatusCode}). You can press ESC to escape.");
                Sleep(10000);
            }
            Http.Destroy(titleReleaseRequest);
        }

        LayerCustomEvent(ReleaseLayer, "Hide", new[] { "" });

        EnableMenuNavigationInputs = true;

        MainMenuLayer = UILayerCreate();
        MainMenuLayer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";

        SoloMenuLayer = UILayerCreate();
        SoloMenuLayer.ManialinkPage = "file://Media/Manialinks/SoloMenu.xml";

        LoadingLayer = UILayerCreate();
        LoadingLayer.Type = CUILayer.EUILayerType.LoadingScreen;
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
        foreach (var e in PendingEvents)
        {
            switch (e.Type)
            {
                case CManiaAppEvent.EType.LayerCustomEvent:
                    if (e.CustomEventType == "MenuSolo")
                    {
                        Log("Switching to Solo Menu...");
                        LayerCustomEvent(SoloMenuLayer, "AnimateOpen", new[] { "" });
                        LayerCustomEvent(MainMenuLayer, "AnimateClose", new[] { "" });
                        MenuLocation = "MenuSolo";
                    }
                    if (e.CustomEventType == "MainMenu")
                    {
                        SwitchToMainMenu();
                    }
                    if (e.CustomEventType == "PlayMap")
                    {
                        var mapGroupNum = TextLib.ToInteger(e.CustomEventData[0]);
                        var mapInfoNum = TextLib.ToInteger(e.CustomEventData[1]);
                        PlayMap(mapGroupNum, mapInfoNum);
                    }
                    if (e.CustomEventType == "ExploreMap")
                    {
                        var mapGroupNum = TextLib.ToInteger(e.CustomEventData[0]);
                        var mapInfoNum = TextLib.ToInteger(e.CustomEventData[1]);
                        ExploreMap(mapGroupNum, mapInfoNum);
                    }
                    break;
                case CManiaAppEvent.EType.MenuNavigation:
                    if (e.MenuNavAction == CManiaAppEvent.EMenuNavAction.Cancel)
                    {
                        if (MenuLocation == "MainMenu")
                        {
                            Menu_Quit();
                        }
                        else if (MenuLocation == "MenuSolo")
                        {
                            SwitchToMainMenu();
                        }
                    }
                    break;
            }
        }

        CheckToken();
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
                EnvimixTurboUserToken = response.Token!;
                if (UserTokenReceived == -1)
                    UserTokenReceived = Now;

                // token can be used immediately
            }

            Http.Destroy(UserTokenRequest);
            UserTokenRequest = null;
        }

        // Periodické obnovení uživatelského tokenu (bez ztráty session)
        if (UserTokenReceived != -1 && Now - UserTokenReceived >= 1200000)
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
        MenuLocation = "MainMenu";
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
}
