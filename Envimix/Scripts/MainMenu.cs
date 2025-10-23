using System.Collections.Immutable;

namespace Envimix.Scripts;

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

    public bool ManiaPlanetAuthenticationRequested;
    public required string ManiaPlanetAuthenticationToken;
    public CHttpRequest? UserTokenRequest;
    public int UserTokenRequestTimeout;
    public int UserTokenFirstRequestTimeout;
    public int UserTokenReceived = -1;
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";
    public int ManiaPlanetAuthReceivedAt = -1;

    public CUILayer MainMenuLayer;
    public CUILayer SoloMenuLayer;

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

        MainMenuLayer = UILayerCreate();
        MainMenuLayer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";

        SoloMenuLayer = UILayerCreate();
        SoloMenuLayer.ManialinkPage = "file://Media/Manialinks/SoloMenu.xml";

        ManiaPlanetAuthenticationRequested = true;
        Authentication_GetToken(null, "Envimix");
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
                    }
                    if (e.CustomEventType == "MainMenu")
                    {
                        Log("Switching to Main Menu...");
                        LayerCustomEvent(SoloMenuLayer, "AnimateClose", new[] { "" });
                        LayerCustomEvent(MainMenuLayer, "AnimateOpen", new[] { "" });
                    }
                    break;
            }
        }

        // Zpracování odpovědi na požadavek ManiaPlanet tokenu
        if (ManiaPlanetAuthenticationRequested && Authentication_GetTokenResponseReceived)
        {
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

            ManiaPlanetAuthenticationRequested = false;
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
                Log($"User token request failed ({UserTokenRequest.StatusCode}). Retry in 10 seconds.");
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
            Log("Running Envimania session refresh...");
            ResetUserTokenState();
            RequestUserToken();
        }
    }

    private void ResetUserTokenState()
    {
        UserTokenRequestTimeout = -1;
        UserTokenFirstRequestTimeout = -1;
        EnvimixTurboUserToken = "";
        UserTokenReceived = -1;
    }
}
