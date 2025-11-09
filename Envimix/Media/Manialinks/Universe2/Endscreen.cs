namespace Envimix.Media.Manialinks.Universe2;

public class Endscreen : CTmMlScriptIngame, IContext
{
    public struct SGame
    {
        public string name;
    }

    public struct SChannel
    {
        public string Id;
        public string Name;
        public int Position;
    }

    public struct SMember
    {
        //public string id;
        public string Username;
        public string Discriminator;
        public string Avatar;
        public string Status;
        public string AvatarUrl;
        public SGame Game;
    }

    public struct SWidget
    {
        public string Id;
        public string Name;
        public IList<SChannel> Channels;
        public IList<SMember> Members;
        public int PresenceCount;

        public string Message;
        public int Code;
    }

    [ManialinkControl] public required CMlFrame FrameEndscreenInfo;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlQuad QuadContinue;
    [ManialinkControl] public required CMlFrame FrameTime;
    [ManialinkControl] public required CMlFrame FrameScore;
    [ManialinkControl] public required CMlFrame FrameLeaderboard;
    [ManialinkControl] public required CMlFrame FrameContinue;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelAuthor;
    [ManialinkControl] public required CMlLabel LabelEnvironment;
    [ManialinkControl] public required CMlQuad QuadEnvironment;
    [ManialinkControl] public required CMlLabel LabelTime;

    [ManialinkControl] public required CMlLabel LabelDiscordName;
    [ManialinkControl] public required CMlFrame FrameDiscord;
    [ManialinkControl] public required CMlLabel LabelDiscordMemberCount;
    [ManialinkControl] public required CMlFrame FrameDiscordUser1;
    [ManialinkControl] public required CMlFrame FrameDiscordUser2;
    [ManialinkControl] public required CMlQuad QuadButtonDiscord;

    public int FinishedAt;
    public string PreviousCar = "";
    public CHttpRequest? WidgetRequest;
    public SWidget Widget;
    public int WidgetAt = -1;
    public bool WidgetHover;
    public int WidgetCurrentMember;
    public IList<CMlFrame> FrameDiscordUsers;

    [Netread] public bool GhostToUpload { get; set; }

    public Endscreen()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace && !IsExplore())
                    {
                        FinishedAt = Now;
                        LabelTime.SetText(TimeToTextWithMilli(e.RaceTime));
                        ShowEndscreen();
                        
                        // show full endscreen in 2 seconds, hide the other UI after the 2 seconds, move blur background from bottom to top for interesting effect
                    }
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Select:
                    Continue();
                    break;
            }
        };

        QuadContinue.MouseClick += () =>
        {
            Continue();
        };

        QuadButtonDiscord.MouseOver += () =>
        {
            WidgetHover = true;
        };

        QuadButtonDiscord.MouseOut += () =>
        {
            WidgetHover = false;
        };

        QuadButtonDiscord.MouseClick += () =>
        {
            OpenLink("https://discord.gg/Rh23k9jcch", CMlScript.LinkType.ExternalBrowser);
        };

        Input.PadButtonPress += (pad, button, isAutoRepeat, keyCode, keyName) =>
        {
            if (button == CInputManager.EButton.A)
            {
                Continue();
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

    bool IsVisible()
    {
        return true;
    }

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    public void Main()
    {
        HideEndscreen();
        FrameEndscreenInfo.Hide();
        SetSlidingText(FrameLabelMapName, Map.MapInfo.Name);
        LabelAuthor.Value = Map.MapInfo.AuthorNickName;
        LabelEnvironment.Value = Map.CollectionName;
        QuadEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{Map.CollectionName}.png");

        FrameDiscordUsers = new[] { FrameDiscordUser1, FrameDiscordUser2 };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (FinishedAt != -1)
        {
            MoveSlidingText(FrameLabelMapName, 10, 0.01f);
            MoveSlidingText(FrameLabelCar, 10, -0.01f);

            if (Input.IsKeyPressed(109) || Input.IsKeyPressed(119))
            {
                Continue();
            }
        }

        var car = Netread<string>.For(GetPlayer());

        if (car.Get() != PreviousCar)
        {
            PreviousCar = car.Get();
            SetSlidingText(FrameLabelCar, car.Get());
        }

        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }

        if (WidgetRequest is not null && WidgetRequest.IsCompleted)
        {
            if (WidgetRequest.StatusCode == 200)
            {
                var cleanerResult = ToCleanerJson(WidgetRequest.Result);
                if (Widget.FromJson(cleanerResult))
                {
                }
                else
                {
                    // it's fairly common for the json to not parse entirely lol
                }
                WidgetAt = Now;
            }
            Http.Destroy(WidgetRequest);
            WidgetRequest = null;
        }

        AnimateWidget();
    }

    void ShowEndscreen()
    {
        FrameEndscreenInfo.RelativePosition_V3.X = -40;
        FrameEndscreenInfo.RelativeScale = 1;
        AnimMgr.Add(FrameEndscreenInfo, "<frame pos=\"0 0\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadBlur, "<quad scale=\"1\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameTime, "<frame pos=\"0 0\" />", Now + 200, 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameScore, "<frame pos=\"0 0\" />", Now + 200, 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameLeaderboard, "<frame pos=\"0 0\" />", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        FrameContinue.RelativeScale = 1;
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 0\" />", Now + 600, 500, CAnimManager.EAnimManagerEasing.QuadOut);

        WidgetRequest = Http.CreateGet($"https://discord.com/api/guilds/1324043936204980234/widget.json");
    }

    void HideEndscreen()
    {
        FinishedAt = -1;
        AnimMgr.Add(QuadBlur, "<quad scale=\"0\" hidden=\"1\" />", 500, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameEndscreenInfo, "<frame pos=\"-10 0\" scale=\"0.2\" hidden=\"1\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameTime, "<frame pos=\"140 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameScore, "<frame pos=\"140 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameLeaderboard, "<frame pos=\"-60 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 21\" scale=\"0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
    }

    private void Continue()
    {
        if (FinishedAt == -1 || Now - FinishedAt < 500 || GhostToUpload)
        {
            return;
        }

        FinishedAt = -1;
        SendCustomEvent("EndscreenContinue", new[] { "" });
        HideEndscreen();
    }

    private void SetSlidingText(CMlFrame frame, string value)
    {
        var l1 = (frame.Controls[0] as CMlLabel)!;
        l1.Value = value;
        l1.Size.X = l1.ComputeWidth(value);

        var l2 = (frame.Controls[1] as CMlLabel)!;
        l2.Value = value;
        l2.Size.X = l2.ComputeWidth(value);
    }

    private void MoveSlidingText(CMlFrame frame, int distance, float speed)
    {
        var l1 = (frame.Controls[0] as CMlLabel)!;
        var l2 = (frame.Controls[1] as CMlLabel)!;

        if (frame.ClipWindowSize.X >= l1.Size.X)
        {
            l2.Hide();
            l1.RelativePosition_V3.X = 0;
            return;
        }

        l1.RelativePosition_V3.X -= Period * speed;
        l2.RelativePosition_V3.X -= Period * speed;
        l2.Show();

        if (speed > 0)
        {
            if (l1.RelativePosition_V3.X + l1.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X > l2.RelativePosition_V3.X)
            {
                l1.RelativePosition_V3.X = l2.RelativePosition_V3.X + l2.Size.X + distance;
            }

            if (l2.RelativePosition_V3.X + l2.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X < l2.RelativePosition_V3.X)
            {
                l2.RelativePosition_V3.X = l1.RelativePosition_V3.X + l1.Size.X + distance;
            }
        }
        else if (speed < 0)
        {
            if (l1.RelativePosition_V3.X - l1.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X < l2.RelativePosition_V3.X)
            {
                l1.RelativePosition_V3.X = l2.RelativePosition_V3.X - l2.Size.X - distance;
            }

            if (l2.RelativePosition_V3.X - l2.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X > l2.RelativePosition_V3.X)
            {
                l2.RelativePosition_V3.X = l1.RelativePosition_V3.X - l1.Size.X - distance;
            }
        }
    }

    private void AnimateWidget()
    {
        if (Widget.Code == 50004)
        {
            LabelDiscordName.SetText("$f00WIDGET NOT ENABLED");
            return;
        }

        if (Widget.Id == "")
        {
            return;
        }

        LabelDiscordName.SetText(Widget.Name);

        if (WidgetAt == -1)
        {
            return;
        }

        var time = Now - WidgetAt;

        if (WidgetHover && Widget.Members.Count != 0)
        {
            LabelDiscordName.Hide();
            LabelDiscordMemberCount.Hide();

            foreach (var frame in FrameDiscordUsers)
            {
                frame.Show();
            }

            if (Widget.Members.Count > 1)
            {
                var offset = 0;
                if (time > 3000)
                {
                    offset = 1;
                }

                for (var i = 0; i < FrameDiscordUsers.Count; i++)
                {
                    var frame = FrameDiscordUsers[i];

                    var pos = i;
                    if (pos == 0) pos = i + offset * 2;
                    if (WidgetCurrentMember + pos > Widget.Members.Count - 1) pos = -WidgetCurrentMember;

                    var quadAvatar = (frame.GetFirstChild("QUAD_AVATAR") as CMlQuad)!;
                    var labelUsername = (frame.GetFirstChild("LABEL_USERNAME") as CMlLabel)!;

                    var member = Widget.Members[WidgetCurrentMember + pos];

                    //quadAvatar.ChangeImageUrl(member.AvatarUrl);
                    //quadAvatar.RefreshImages();

                    var username = member.Username;
                    if (username == "") username = "$BBB[unknown]";

                    var StatusColor = "$888";
                    if (member.Status == "idle") StatusColor = "$ff0";
                    else if (member.Status == "online") StatusColor = "$0f0";
                    else if (member.Status == "dnd") StatusColor = "$f00";

                    labelUsername.SetText($"{username} {StatusColor}•");
                }

                FrameDiscordUsers[0].RelativePosition_V3.Y = AnimLib.EaseInOutQuad(time - 2000, 0, 10, 1000) + AnimLib.EaseInOutQuad(time - 2000 - 2000 - 1000, 0, 10, 1000) + offset * -20;
                FrameDiscordUsers[1].RelativePosition_V3.Y = AnimLib.EaseInOutQuad(time - 2000, -10, 10, 1000) + AnimLib.EaseInOutQuad(time - 2000 - 2000 - 1000, 0, 10, 1000);

                if (time > 6000)
                {
                    WidgetAt = Now;

                    WidgetCurrentMember += 2;
                    if (WidgetCurrentMember >= Widget.Members.Count) WidgetCurrentMember = 0;
                }
            }
            else
            {
                // only one member online
            }
        }
        else
        {
            foreach (var frame in FrameDiscordUsers) frame.Hide();

            LabelDiscordName.Show();
            LabelDiscordMemberCount.Show();

            LabelDiscordName.Opacity = AnimLib.EaseOutQuad(time, 0, 1, 300) + AnimLib.EaseOutQuad(time - 2000, 0, -1, 300);
            LabelDiscordMemberCount.Opacity = AnimLib.EaseOutQuad(time - 2000 - 300, 0, 1, 300) + AnimLib.EaseOutQuad(time - 2000 - 2000 - 300, 0, -1, 300);

            if (Widget.Members.Count > 1) LabelDiscordMemberCount.SetText($"$0f0• $fff{Widget.Members.Count} members online");
            else if (Widget.Members.Count == 1) LabelDiscordMemberCount.SetText($"$0f0• $fff{Widget.Members.Count} member online");
            else LabelDiscordMemberCount.SetText("$888• $fff0 members online");

            if (time > 4600) WidgetAt = Now;
        }
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
}