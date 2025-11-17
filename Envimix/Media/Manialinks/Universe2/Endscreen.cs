using System.Collections.Immutable;

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

    public struct SEnvimaniaRecord
    {
        public SUserInfo User;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public bool Verified;
        public bool Projected;
        public string GhostUrl;
        public string DrivenAt;
    }

    public struct SEnvimaniaRecordsFilter
    {
        public string Car;
        public int Gravity;
        public int Laps;
        public string Type;
    }

    public struct SEnvimaniaRecordsResponse
    {
        public SEnvimaniaRecordsFilter Filter;
        public string Zone;
        public ImmutableArray<SEnvimaniaRecord> Records;
        public ImmutableArray<SEnvimaniaRecord> Validation;
        public ImmutableArray<int> Skillpoints;
        public string TitlePackReleaseTimestamp;
    }

    [ManialinkControl] public required CMlFrame FrameEndscreenInfo;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlQuad QuadContinue;
    [ManialinkControl] public required CMlFrame FrameTime;
    [ManialinkControl] public required CMlFrame FrameScore;
    [ManialinkControl] public required CMlFrame FrameLeaderboardInfo;
    [ManialinkControl] public required CMlFrame FrameContinue;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelAuthor;
    [ManialinkControl] public required CMlLabel LabelEnvironment;
    [ManialinkControl] public required CMlQuad QuadEnvironment;
    [ManialinkControl] public required CMlLabel LabelTime;
    [ManialinkControl] public required CMlFrame FrameLeaderboard;
    [ManialinkControl] public required CMlFrame FrameLeaderboardContents;
    [ManialinkControl] public required CMlQuad QuadLoadingLeaderboard;
    [ManialinkControl] public required CMlFrame FramePersonalRecord;
    [ManialinkControl] public required CMlFrame FrameEvent;
    [ManialinkControl] public required CMlLabel LabelEvent;
    [ManialinkControl] public required CMlFrame FrameDelta;
    [ManialinkControl] public required CMlQuad QuadDelta;
    [ManialinkControl] public required CMlLabel LabelDelta;
    [ManialinkControl] public required CMlLabel LabelSkillpoints;
    [ManialinkControl] public required CMlLabel LabelActivityPoints;
    [ManialinkControl] public required CMlLabel LabelSkillpointsDelta;
    [ManialinkControl] public required CMlLabel LabelActivityPointsDelta;

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

    [Netread] public SEnvimaniaRecordsResponse EndscreenRecordsResponse { get; set; }
    [Netread] public int EndscreenRecordsResponseReceivedAt { get; set; }
    public int PreviousEndscreenRecordsResponseReceivedAt;
    public string EventText = "";
    public bool IsPb;
    public int ExpectedSkillpoints;
    public int ExpectedActivityPoints;
    public int CurrentSkillpoints;
    public int CurrentActivityPoints;
    public int StartPointChangeAt = -1;

    [Netread] public required Dictionary<string, int> Skillpoints { get; set; }
    [Netread] public required Dictionary<string, int> ActivityPoints { get; set; }

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

                        if (e.Player.Score.BestRace.Checkpoints.Count <= 0)
                        {
                            IsPb = true;
                            EventText = "Your first finish!";
                            FrameDelta.Hide();
                        }
                        else
                        {
                            var diff = e.RaceTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInRace];
                            var formattedDiff = TextLib.FormatReal(diff / 1000f, 3, false, false);

                            if (diff > 0)
                            {
                                IsPb = false;
                                EventText = "";
                                LabelDelta.SetText($"+{formattedDiff}");
                                QuadDelta.Colorize = new Vec3(1, 0.1, 0);
                            }
                            else if (diff < 0)
                            {
                                IsPb = true;
                                EventText = "New personal best!";
                                LabelDelta.SetText(formattedDiff);
                                QuadDelta.Colorize = new Vec3(0, 0.1, 1);
                            }
                            else
                            {
                                IsPb = false;
                                EventText = "Perfect tie!";
                                LabelDelta.SetText(formattedDiff);
                                QuadDelta.Colorize = new Vec3(1, 0, 1);
                            }

                            FrameDelta.Show();
                        }

                        LabelSkillpointsDelta.Hide();
                        LabelActivityPointsDelta.Hide();

                        ShowEndscreen();
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
        SetLeaderboardLoadingState();
        SetSlidingText(FrameLabelMapName, Map.MapInfo.Name);
        LabelAuthor.Value = Map.MapInfo.AuthorNickName;
        LabelEnvironment.Value = Map.CollectionName;
        QuadEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{Map.CollectionName}.png");

        FrameDiscordUsers = new[] { FrameDiscordUser1, FrameDiscordUser2 };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        Wait(() => GetPlayer() is not null);
    }

    private void SetLeaderboardLoadingState()
    {
        FrameLeaderboard.Hide();
        QuadLoadingLeaderboard.Show();
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
            SetLeaderboardLoadingState();
            SetPoints(car.Get());
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

        if (EndscreenRecordsResponseReceivedAt != PreviousEndscreenRecordsResponseReceivedAt)
        {
            UpdateLeaderboard();
            PreviousEndscreenRecordsResponseReceivedAt = EndscreenRecordsResponseReceivedAt;
        }

        AnimateWidget();
        AnimatePoints();
    }

    private string FormatNumberSpace(int number)
    {
        var txt = TextLib.ToText(number);
        if (number < 0)
        {
            txt = TextLib.SubText(txt, 1, TextLib.Length(txt) - 1);
        }
        var result = "";
        var len = TextLib.Length(txt);
        var count = 0;

        for (var i = 0; i < len; i++)
        {
            result = $"{TextLib.SubText(txt, len - 1 - i, 1)}{result}";
            count += 1;

            if (count == 3 && i < len - 1)
            {
                result = $" {result}";
                count = 0;
            }
        }

        if (number < 0)
        {
            result = $"-{result}";
        }

        return result;
    }

    private int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
        }

        if (NbLaps == -1)
        {
            return Map.TMObjective_NbLaps;
        }

        return NbLaps;
    }

    string ConstructValidationFilterKey(string car)
    {
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car}_{gravity.Get()}_{GetLaps()}";
    }

    private void SetPoints(string car)
    {
        var validationFilterKey = ConstructValidationFilterKey(car);

        if (Skillpoints.ContainsKey(validationFilterKey))
        {
            CurrentSkillpoints = Skillpoints[validationFilterKey];
        }
        else
        {
            CurrentSkillpoints = 0;
        }

        if (ActivityPoints.ContainsKey(validationFilterKey))
        {
            CurrentActivityPoints = ActivityPoints[validationFilterKey];
        }
        else
        {
            CurrentActivityPoints = 0;
        }

        ExpectedSkillpoints = CurrentSkillpoints;
        ExpectedActivityPoints = CurrentActivityPoints;
        LabelSkillpoints.SetText(FormatNumberSpace(CurrentSkillpoints));
        LabelActivityPoints.SetText(FormatNumberSpace(CurrentActivityPoints));
    }

    void ShowEndscreen()
    {
        FrameEndscreenInfo.RelativePosition_V3.X = -40;
        FrameEndscreenInfo.RelativeScale = 1;
        AnimMgr.Add(FrameEndscreenInfo, "<frame pos=\"0 0\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadBlur, "<quad scale=\"1\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameTime, "<frame pos=\"0 0\" />", Now + 200, 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameScore, "<frame pos=\"0 0\" />", Now + 200, 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameLeaderboardInfo, "<frame pos=\"0 0\" />", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
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
        AnimMgr.Add(FrameLeaderboardInfo, "<frame pos=\"-70 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 21\" scale=\"0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameEvent, "<frame pos=\"0 -8\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);

        EventText = "";
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

    private void UpdateLeaderboard()
    {
        QuadLoadingLeaderboard.Hide();
        FrameLeaderboard.Show();

        var rankIndex = 0;
        var prevTime = -1;
        var rankOffset = 0;

        var pbIsWorldRecord = false;

        foreach (var control in FrameLeaderboardContents.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (EndscreenRecordsResponse.Records.Length <= rankIndex)
            {
                frame.Hide();
                continue;
            }

            var record = EndscreenRecordsResponse.Records[rankIndex];

            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var quadHighlight = (frame.GetFirstChild("QuadHighlight") as CMlQuad)!;

            if (prevTime == record.Time)
            {
                rankOffset += 1;
            }
            else
            {
                prevTime = record.Time;
            }

            labelRank.SetText(TextLib.FormatInteger(rankIndex + 1 - rankOffset, 2));
            labelRecord.SetText(TimeToTextWithMilli(record.Time));
            labelNickname.SetText(record.User.Nickname);

            frame.Show();

            if (record.User.Login == GetPlayer().User.Login)
            {
                if (rankIndex == 0)
                {
                    pbIsWorldRecord = true;
                }

                quadHighlight.Show();
                if (IsPb)
                {
                    quadHighlight.Opacity = 0.6f;
                    AnimMgr.Add(quadHighlight, "<quad opacity=\"0.2\" />", 200, CAnimManager.EAnimManagerEasing.QuadInOut);
                }
            }
            else
            {
                quadHighlight.Hide();
            }

            rankIndex += 1;
        }

        var pbTime = GetPlayer().Score.BestRace.Time;

        var pbCounting = true;
        var pbRankCounter = 0;
        var pbSkillpointRankCounter = 0;
        var totalRecCount = 0;

        for (var i = 0; i < EndscreenRecordsResponse.Skillpoints.Length / 2; i++)
        {
            var time = EndscreenRecordsResponse.Skillpoints[i * 2];
            var count = EndscreenRecordsResponse.Skillpoints[i * 2 + 1];

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

        var labelPbRank = (FramePersonalRecord.GetFirstChild("LabelRank") as CMlLabel)!;
        var labelPbRecord = (FramePersonalRecord.GetFirstChild("LabelRecord") as CMlLabel)!;
        var labelPbNickname = (FramePersonalRecord.GetFirstChild("LabelNickname") as CMlLabel)!;
        var quadPbHighlight = (FramePersonalRecord.GetFirstChild("QuadHighlight") as CMlQuad)!;
        quadPbHighlight.Opacity = 0.2f;

        labelPbRank.SetText(TextLib.FormatInteger(pbRankCounter + 1, 2));
        labelPbRecord.SetText(TimeToTextWithMilli(pbTime));
        labelPbNickname.SetText(GetPlayer().User.Name);

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
        Log($"Skillpoints calculation: ({totalRecCount} - {pbSkillpointRankCounter}) * 100 / {pbSkillpointRankCounter} = {skillpointsReal} (ceiling: {ceilingSkillpoints})");

        var wr = pbTime;
        if (EndscreenRecordsResponse.Records.Length > 0)
        {
            wr = EndscreenRecordsResponse.Records[0].Time;
        }
        var wrPb = wr * 1f / pbTime;
        var activityPointsReal = 1000 * MathLib.Exp(totalRecCount * (wrPb - 1));
        var activityPoints = MathLib.NearestInteger(activityPointsReal);

        Log($"Activity points calculation: 1000 * exp({totalRecCount} * ({wr} / {pbTime} - 1)) = {activityPointsReal} (nearest: {activityPoints})");

        if (EndscreenRecordsResponse.Validation.Length > 0)
        {
            var validation = EndscreenRecordsResponse.Validation[0];
            if (validation.User.Login == GetPlayer().User.Login && validation.DrivenAt != "" && EndscreenRecordsResponse.TitlePackReleaseTimestamp != "")
            {
                var validationTimestampInSeconds = validation.DrivenAt;
                var titlePackReleaseTimestampInSeconds = EndscreenRecordsResponse.TitlePackReleaseTimestamp;
                var validationAge = TimeLib.GetDelta(validationTimestampInSeconds, titlePackReleaseTimestampInSeconds);
                var extraActivityPointsReal = 10 + validationAge / 86400f * 10;
                var extraActivityPointsInt = MathLib.NearestInteger(extraActivityPointsReal);
                Log($"Extra activity points calculation: 10 + truncate(({validationTimestampInSeconds} - {titlePackReleaseTimestampInSeconds}) / 86400) * 10 = {extraActivityPointsReal} (nearest: {extraActivityPointsInt})");
                activityPoints += extraActivityPointsInt;
            }
        }

        ExpectedSkillpoints = ceilingSkillpoints;
        ExpectedActivityPoints = activityPoints;

        StartPointChangeAt = Now;

        // somehow this function runs while endscreen is not visible
        // if less than 10 points difference, don't play sound, it will be played during loop
        if (FrameEndscreenInfo.Visible && (MathLib.Abs(ExpectedSkillpoints - CurrentSkillpoints) > 10 || MathLib.Abs(ExpectedActivityPoints - CurrentActivityPoints) > 10))
        {
            for (var i = 0; i < 10; i++)
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: i * 100);
            }
        }

        if (IsPb && pbIsWorldRecord)
        {
            EventText = "New world record!";

            if (EndscreenRecordsResponse.Validation.Length > 0)
            {
                var validation = EndscreenRecordsResponse.Validation[0];
                if (validation.User.Login == GetPlayer().User.Login && validation.Time == pbTime)
                {
                    EventText = "New validation!";
                }
            }
        }

        if (EventText != "")
        {
            LabelEvent.SetText(EventText);
            AnimMgr.Add(FrameEvent, "<frame pos=\"0 0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        }
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

    public int PrevSkillpointsInAnim;
    public int PrevActivityPointsInAnim;

    private void AnimatePoints()
    {
        var duration = 1000;

        if (StartPointChangeAt == -1)
        {
            return;
        }

        var time = Now - StartPointChangeAt;

        var skillpointsDiff = ExpectedSkillpoints - CurrentSkillpoints;
        var activityPointsDiff = ExpectedActivityPoints - CurrentActivityPoints;

        var skillpoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentSkillpoints), MathLib.ToReal(skillpointsDiff), duration));
        var activityPoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentActivityPoints), MathLib.ToReal(activityPointsDiff), duration));

        LabelSkillpoints.SetText(FormatNumberSpace(skillpoints));
        LabelActivityPoints.SetText(FormatNumberSpace(activityPoints));

        if (FrameEndscreenInfo.Visible)
        {
            if (MathLib.Abs(ExpectedSkillpoints - CurrentSkillpoints) > 10 || MathLib.Abs(ExpectedActivityPoints - CurrentActivityPoints) > 10)
            {
                var playSound = false;
                if (skillpoints != PrevSkillpointsInAnim)
                {
                    PrevSkillpointsInAnim = skillpoints;
                    playSound = true;
                }
                if (activityPoints != PrevActivityPointsInAnim)
                {
                    PrevActivityPointsInAnim = activityPoints;
                    playSound = true;
                }
                if (playSound)
                {
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f);
                }
            }
        }

        var noPointDiff = skillpointsDiff == 0 && activityPointsDiff == 0;

        if (time < duration && !noPointDiff)
        {
            return;
        }

        StartPointChangeAt = -1;
        skillpoints = ExpectedSkillpoints;
        activityPoints = ExpectedActivityPoints;
        CurrentSkillpoints = skillpoints;
        CurrentActivityPoints = activityPoints;

        if (skillpointsDiff < 0)
        {
            LabelSkillpointsDelta.SetText(FormatNumberSpace(skillpointsDiff));
        }
        else
        {
            LabelSkillpointsDelta.SetText($"+{FormatNumberSpace(skillpointsDiff)}");
        }

        if (activityPointsDiff < 0)
        {
            LabelActivityPointsDelta.SetText(FormatNumberSpace(activityPointsDiff));
        }
        else
        {
            LabelActivityPointsDelta.SetText($"+{FormatNumberSpace(activityPointsDiff)}");
        }

        LabelSkillpointsDelta.RelativePosition_V3.X = -LabelSkillpoints.ComputeWidth(LabelSkillpoints.Value) - 3;
        LabelActivityPointsDelta.RelativePosition_V3.X = -LabelActivityPoints.ComputeWidth(LabelActivityPoints.Value) - 3;

        LabelSkillpointsDelta.RelativeScale = 0.7f;
        LabelActivityPointsDelta.RelativeScale = 0.7f;
        AnimMgr.Add(LabelSkillpointsDelta, "<label hidden=\"0\" scale=\"1\" />", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(LabelActivityPointsDelta, "<label hidden=\"0\" scale=\"1\" />", 100, CAnimManager.EAnimManagerEasing.QuadOut);

        if (FrameEndscreenInfo.Visible && (skillpointsDiff > 0 || activityPointsDiff > 0))
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, 2, 1f);
        }
    }
}