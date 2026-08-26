using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class OnlinePlayMenu : CManiaAppTitleLayer, IContext
{
    public struct SOnlineServer
    {
        public string Name;
        public string Description;
        public string Login;
        public string Title;
        public int LadderLimitMin;
        public int LadderLimitMax;
        public int AverageLadderPoints;
        public int PlayerCount;
        public int PlayerMax;
        public int SpectatorCount;
        public string Zone;
        public bool IsPrivate;
        public bool IsSpectatorPrivate;
        public bool IsLobby;
        public int LevelClass1;
        public int LevelClass2;
        public int LevelClass3;
        public int LevelClass4;
        public int LevelClass5;
        public string ScriptName;
        public int GameMode;
        public string RelayOf;
        public string Environment;
    }

    [ManialinkControl] public required CMlFrame FrameOnlinePlayMenu;
    [ManialinkControl] public required CMlFrame FrameServerList;
    [ManialinkControl] public required CMlFrame FrameServerListScrollable;
    [ManialinkControl] public required CMlFrame FrameServerPanel;
    [ManialinkControl] public required CMlLabel LabelStatus;
    [ManialinkControl] public required CMlLabel LabelPanelName;
    [ManialinkControl] public required CMlLabel LabelPanelLogin;
    [ManialinkControl] public required CMlLabel LabelPanelPlayers;
    [ManialinkControl] public required CMlLabel LabelPanelSpectators;
    [ManialinkControl] public required CMlLabel LabelPanelZone;
    [ManialinkControl] public required CMlLabel LabelPanelMode;
    [ManialinkControl] public required CMlLabel LabelPanelLadder;
    [ManialinkControl] public required CMlLabel LabelPanelDescription;
    [ManialinkControl] public required CMlQuad QuadJoinServer;
    [ManialinkControl] public required CMlQuad QuadJoinServerBase;
    [ManialinkControl] public required CMlQuad QuadRefresh;
    [ManialinkControl] public required CMlQuad QuadScroller;
    [ManialinkControl] public required CMlQuad QuadFavorites;
    [ManialinkControl] public required CMlQuad QuadAccess;
    [ManialinkControl] public required CMlQuad QuadOrder;
    [ManialinkControl] public required CMlLabel LabelAccess;
    [ManialinkControl] public required CMlLabel LabelOrder;
    [ManialinkControl] public required CMlQuad QuadLegacyMenu;

    public const string ServersEndpoint = "https://prod.live.maniaplanet.com/ingame/servers/online";
    public const int PageLength = 20;
    public const int VisibleServerCount = 13;
    public const float ServerRowHeight = 10;

    public string Token = "";
    public string SelectedServerLogin = "";
    public bool IsOpen;
    public bool OnlyFavorites;
    public int AccessFilter;
    public bool OrderByLadder;
    public int Offset;
    public bool AppendRequest;
    public bool HasMoreServers;
    public float PrevScrollY;
    public bool HoldScrollbar;
    public float HoldScrollbarPos;
    public bool ScrollbarMouseOut;
    public CHttpRequest? ServersRequest;
    public ImmutableArray<SOnlineServer> Servers;

    public OnlinePlayMenu()
    {
        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    IsOpen = true;
                    Show();
                    ResetAndRequestServers();
                    break;
                case "AnimateClose":
                    IsOpen = false;
                    Hide();
                    break;
                case "Authenticate":
                    if (data.Length < 1)
                        break;
                    Token = data[0];
                    if (IsOpen)
                        ResetAndRequestServers();
                    break;
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadServer")
            {
                var serverLogin = control.DataAttributeGet("login");
                if (serverLogin != "")
                {
                    if (SelectedServerLogin == serverLogin)
                        JoinSelectedServer();
                    else
                        SelectedServerLogin = serverLogin;

                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
                    UpdateServerList();
                    UpdateServerPanel();
                }
            }
        };

        QuadJoinServer.MouseClick += () =>
        {
            JoinSelectedServer();
        };
        QuadRefresh.MouseClick += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
            ResetAndRequestServers();
        };
        QuadFavorites.MouseClick += () =>
        {
            OnlyFavorites = !OnlyFavorites;
            UpdateFilters();
            ResetAndRequestServers();
        };
        QuadAccess.MouseClick += () =>
        {
            AccessFilter = (AccessFilter + 1) % 3;
            UpdateFilters();
            ResetAndRequestServers();
        };
        QuadOrder.MouseClick += () =>
        {
            OrderByLadder = !OrderByLadder;
            UpdateFilters();
            ResetAndRequestServers();
        };
        QuadScroller.MouseClick += () =>
        {
            HoldScrollbar = true;
            HoldScrollbarPos = MouseY - (float)QuadScroller.RelativePosition_V3.Y;
        };
        QuadScroller.MouseOver += () =>
        {
            AnimMgr.Add(QuadScroller, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };
        QuadScroller.MouseOut += () =>
        {
            if (HoldScrollbar)
                ScrollbarMouseOut = true;
            else
                AnimMgr.Add(QuadScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        };
        QuadLegacyMenu.MouseClick += () =>
        {
            SendCustomEvent("MenuInternetLegacy", new[] { "" });
        };

        QuadLegacyMenu.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };
    }

    private string RegexReplace(string _Pattern, string _Text, string _Flags, string _Replacement)
    {
        var Final = _Text;

        var MatchFlags = TextLib.Replace(_Flags, "g", "");

        IList<string> Finds = TextLib.RegexFind(_Pattern, _Text, _Flags);
        for (var i = 0; i < Finds.Count; i++)
        {
            IList<string> Matches = TextLib.RegexMatch(_Pattern, Finds[i], MatchFlags);
            var Replacement = _Replacement;
            for (var j = 0; j < Matches.Count; j++)
                Replacement = TextLib.Replace(Replacement, $"\\{j}", Matches[j]);
            Final = TextLib.Replace(Final, Finds[i], Replacement);
        }

        return Final;
    }

    private string ToCleanerJson(string _Json)
    {
        var Final = _Json;
        var AllKeys = TextLib.RegexFind("\"(.*?)\":(.*?)(,|})", _Json, "g");
        foreach (var Key in AllKeys)
        {
            var PatternPrecise = "\"([a-z])(.*?)\":(.*?)(,|})";

            var MatchFirstLetter = TextLib.RegexMatch(PatternPrecise, Key, "");
            var FirstLetter = TextLib.ToUpperCase(MatchFirstLetter[1]);

            var FixedRest = MatchFirstLetter[2];
            var FindInnerFirstLetters = TextLib.RegexFind("_([a-z])(.*?)", FixedRest, "g");
            foreach (var InnerFirstLetter in FindInnerFirstLetters)
            {
                var MatchInnerFirstLetters = TextLib.RegexMatch("_([a-z])(.*?)", InnerFirstLetter, "");
                var FirstInnerLetter = TextLib.ToUpperCase(MatchInnerFirstLetters[1]);
                FixedRest = TextLib.Replace(FixedRest, InnerFirstLetter, RegexReplace("_([a-z])(.*?)", InnerFirstLetter, "g", $"{FirstInnerLetter}\\2"));
            }

            var Fixed = RegexReplace(PatternPrecise, Key, "g", $"\"{FirstLetter}{FixedRest}\":\\3\\4");
            Final = TextLib.Replace(Final, Key, Fixed);
        }

        return Final;
    }

    private void RequestServers(bool append)
    {
        if (ServersRequest is not null || Token == "")
        {
            if (Token == "")
                LabelStatus.SetText("Waiting for authentication...");
            return;
        }

        AppendRequest = append;

        var url = $"{ServersEndpoint}?titleUids[]={LoadedTitle.TitleId}&length={PageLength}";
        if (OrderByLadder)
            url = $"{url}&orderBy=levelClass1";
        if (OnlyFavorites)
            url = $"{url}&onlyFavorite=1";
        if (AccessFilter == 1)
            url = $"{url}&onlyPublic=1";
        else if (AccessFilter == 2)
            url = $"{url}&onlyPrivate=1";
        if (Offset > 0)
            url = $"{url}&offset={Offset}";

        LabelStatus.SetText("Loading servers...");
        ServersRequest = Http.CreateGet(url, false, $"Maniaplanet-Auth: Login=\"{LocalUser.Login}\", Token=\"{Token}\"\nAccept: application/json");
    }

    private void UpdateFilters()
    {
        QuadFavorites.StyleSelected = OnlyFavorites;
        if (AccessFilter == 0)
            LabelAccess.SetText("ALL");
        else if (AccessFilter == 1)
            LabelAccess.SetText("PUBLIC");
        else
            LabelAccess.SetText("PRIVATE");

        if (OrderByLadder)
            LabelOrder.SetText("LADDER");
        else
            LabelOrder.SetText("PLAYERS");
    }

    private void UpdateServerList()
    {
        FrameServerListScrollable.ScrollMax.Y = 1f * MathLib.Max(0, Servers.Length - VisibleServerCount) * ServerRowHeight;
        var scrollOffset = MathLib.NearestInteger((float)FrameServerListScrollable.ScrollOffset.Y / ServerRowHeight);

        if (Servers.Length > VisibleServerCount)
        {
            QuadScroller.Show();
            QuadScroller.Size.Y = VisibleServerCount * 1f / Servers.Length * 130f;
            QuadScroller.RelativePosition_V3.Y = -(float)FrameServerListScrollable.ScrollOffset.Y
                / FrameServerListScrollable.ScrollMax.Y * (130 - QuadScroller.Size.Y);
        }
        else
        {
            QuadScroller.Hide();
        }

        var index = 0;
        foreach (var control in FrameServerList.Controls)
        {
            var frame = (control as CMlFrame)!;
            var serverIndex = index + scrollOffset;

            if (serverIndex >= Servers.Length)
            {
                frame.Hide();
                index += 1;
                continue;
            }

            var server = Servers[serverIndex];
            var quadServer = (frame.GetFirstChild("QuadServer") as CMlQuad)!;
            var labelName = (frame.GetFirstChild("LabelServerName") as CMlLabel)!;
            var labelPlayers = (frame.GetFirstChild("LabelPlayers") as CMlLabel)!;
            var labelMode = (frame.GetFirstChild("LabelMode") as CMlLabel)!;

            quadServer.DataAttributeSet("login", server.Login);
            quadServer.StyleSelected = server.Login == SelectedServerLogin;
            labelName.SetText(server.Name);
            labelPlayers.SetText($"{server.PlayerCount}$888/{server.PlayerMax}");
            labelMode.SetText(server.ScriptName);
            if (server.IsPrivate)
                labelPlayers.SetText($"$ff0🔒 $fff{server.PlayerCount}$888/{server.PlayerMax}");
            frame.Show();
            index += 1;
        }
    }

    private void UpdateServerPanel()
    {
        foreach (var server in Servers)
        {
            if (server.Login != SelectedServerLogin)
                continue;

            LabelPanelName.SetText(server.Name);
            LabelPanelLogin.SetText(server.Login);
            LabelPanelPlayers.SetText($"{server.PlayerCount}/{server.PlayerMax}");
            if (server.IsPrivate)
            {
                LabelPanelPlayers.SetText($"{server.PlayerCount}/{server.PlayerMax}$ff0🔒");
            }
            LabelPanelSpectators.SetText($"{server.SpectatorCount}/{server.PlayerMax}");
            if (server.IsSpectatorPrivate)
            {
                LabelPanelSpectators.SetText($"{server.SpectatorCount}/{server.PlayerMax}$ff0🔒");
            }
            LabelPanelZone.SetText(server.Zone);
            LabelPanelMode.SetText(server.ScriptName);
            LabelPanelLadder.SetText($"Ladder {server.LadderLimitMin}-{server.LadderLimitMax}");
            LabelPanelDescription.SetText(server.Description);
            QuadJoinServerBase.Colorize = new Vec3(0, 1, 0);
            QuadJoinServer.Show();
            FrameServerPanel.Show();
            return;
        }

        SelectedServerLogin = "";
        QuadJoinServerBase.Colorize = new Vec3(0.1, 0.1, 0.1);
        QuadJoinServer.Hide();
        FrameServerPanel.Hide();
    }

    private void JoinSelectedServer()
    {
        if (SelectedServerLogin == "")
            return;
        OpenLink($"#join={SelectedServerLogin}", CMlScript.LinkType.Goto);
    }

    private void Show()
    {
        AnimMgr.Add(FrameOnlinePlayMenu, "<frame pos=\"0 0\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void Hide()
    {
        AnimMgr.Add(FrameOnlinePlayMenu, "<frame pos=\"180 0\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void UpdateScrolling()
    {
        if (HoldScrollbar && MouseLeftButton)
        {
            var newY = MathLib.Clamp(MouseY - HoldScrollbarPos, (float)QuadScroller.Size.Y - 130f, 0);
            var targetScrollOffset = newY / ((float)QuadScroller.Size.Y - 130) * (float)FrameServerListScrollable.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger(targetScrollOffset / ServerRowHeight);
            var steppedScrollOffset = MathLib.Clamp(stepIndex * ServerRowHeight * 1f, 0, (float)FrameServerListScrollable.ScrollMax.Y);
            if (FrameServerListScrollable.ScrollMax.Y > 0)
                QuadScroller.RelativePosition_V3.Y = -(steppedScrollOffset / FrameServerListScrollable.ScrollMax.Y) * (130 - QuadScroller.Size.Y);
            FrameServerListScrollable.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldScrollbar)
        {
            if (ScrollbarMouseOut)
            {
                AnimMgr.Add(QuadScroller, "<quad opacity=\"0.8\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
                ScrollbarMouseOut = false;
            }
            HoldScrollbar = false;
        }

        if (FrameServerListScrollable.ScrollOffset.Y != PrevScrollY)
        {
            PrevScrollY = (float)FrameServerListScrollable.ScrollOffset.Y;
            FrameServerList.RelativePosition_V3.Y = -PrevScrollY;
            UpdateServerList();
        }

        if (ServersRequest is null && HasMoreServers && Servers.Length > 0
            && FrameServerListScrollable.ScrollAnimOffset.Y > FrameServerListScrollable.ScrollMax.Y)
        {
            Offset += PageLength;
            RequestServers(true);
        }
    }

    private void ResetAndRequestServers()
    {
        if (ServersRequest is not null)
        {
            Http.Destroy(ServersRequest);
            ServersRequest = null;
        }
        Offset = 0;
        Servers = new();
        HasMoreServers = true;
        SelectedServerLogin = "";
        FrameServerListScrollable.ScrollOffset.Y = 0;
        PrevScrollY = 0;
        FrameServerList.RelativePosition_V3.Y = 0;
        UpdateServerList();
        UpdateServerPanel();
        RequestServers(false);
    }

    public void Main()
    {
        FrameOnlinePlayMenu.RelativePosition_V3.X = 180;
        UpdateFilters();
        UpdateServerList();
        UpdateServerPanel();
    }

    public void Loop()
    {
        UpdateScrolling();

        if (ServersRequest is null || !ServersRequest.IsCompleted)
            return;

        if (ServersRequest.StatusCode == 200)
        {
            ImmutableArray<SOnlineServer> response = new();
            var cleanerResult = ToCleanerJson(ServersRequest.Result);
            if (response.FromJson(cleanerResult))
            {
            }
            if (AppendRequest)
            {
                foreach (var server in response)
                {
                    var alreadyLoaded = false;
                    foreach (var loadedServer in Servers)
                    {
                        if (loadedServer.Login == server.Login)
                        {
                            alreadyLoaded = true;
                            break;
                        }
                    }
                    if (!alreadyLoaded)
                        Servers.Add(server);
                }
            }
            else
            {
                Servers = response;
            }
            HasMoreServers = response.Length == PageLength;
            if (Servers.Length == 0)
                LabelStatus.SetText("No servers found");
            else
                LabelStatus.SetText("");
        }
        else
        {
            if (!AppendRequest)
                Servers = new();
            else
                Offset = MathLib.Max(0, Offset - PageLength);
            LabelStatus.SetText($"Server list unavailable ({ServersRequest.StatusCode})");
        }

        Http.Destroy(ServersRequest);
        ServersRequest = null;
        UpdateServerList();
        UpdateServerPanel();
    }
}
