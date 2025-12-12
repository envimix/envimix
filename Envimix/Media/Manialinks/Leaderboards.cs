namespace Envimix.Media.Manialinks;

public class Leaderboards : CManiaAppTitleLayer, IContext
{
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

    public struct STitleUserInfo
    {
        public string N;
        public string Z;
    }

    [ManialinkControl] public required CMlFrame FrameCompletion;
    [ManialinkControl] public required CMlFrame FrameMostSkillpoints;
    [ManialinkControl] public required CMlFrame FrameMostActivityPoints;
    [ManialinkControl] public required CMlFrame FrameOverallCompletion;
    [ManialinkControl] public required CMlFrame FrameQuit;
    [ManialinkControl] public required CMlFrame FrameCategory;
    [ManialinkControl] public required CMlQuad QuadQuit;
    [ManialinkControl] public required CMlLabel LabelOverallCompletion;

    [ManialinkControl] public required CMlFrame FrameCompletionPlayers;
    [ManialinkControl] public required CMlFrame FrameMostSkillpointsPlayers;
    [ManialinkControl] public required CMlFrame FrameMostActivityPointsPlayers;

    [ManialinkControl] public required CMlFrame FramePersonalCompletion;
    [ManialinkControl] public required CMlFrame FramePersonalSkillpoints;
    [ManialinkControl] public required CMlFrame FramePersonalActivityPoints;

    [ManialinkControl] public required CMlQuad QuadEnvimixLeaderboards;
    [ManialinkControl] public required CMlQuad QuadDefaultCarLeaderboards;
    [ManialinkControl] public required CMlQuad QuadGlobalLeaderboards;

    public int OpenedAt = -1;
    public float EnvimixCompletionPercentage;
    public float DefaultCarCompletionPercentage;
    public float GlobalCompletionPercentage;

    public CMlQuad SelectedLeaderboards;

    public CAudioSource AudioClick;

    public int ZoneIndexCompletion;
    public int ZoneIndexSkillpoints;
    public int ZoneIndexActivityPoints;

    public Leaderboards()
    {
        QuadQuit.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
            AudioClick.Play();
        };

        QuadQuit.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    Show();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    Hide();
                    break;
                case "SetLeaderboards":
                    EnvimixCompletionPercentage = TextLib.ToReal(data[0]);
                    DefaultCarCompletionPercentage = TextLib.ToReal(data[1]);
                    GlobalCompletionPercentage = TextLib.ToReal(data[2]);
                    UpdateLeaderboards();
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("MainMenu", new[] { "" });
                    break;
            }
        };

        QuadEnvimixLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadEnvimixLeaderboards;
            UpdateLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadEnvimixLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadDefaultCarLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadDefaultCarLeaderboards;
            UpdateLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadDefaultCarLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadGlobalLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadGlobalLeaderboards;
            UpdateLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadGlobalLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadZone")
            {
                IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);
                var index = 0;

                switch (control.Parent.Parent.ControlId)
                {
                    case "FrameCompletion":
                        if (zones.Count == 0)
                        {
                            ZoneIndexCompletion = 0;
                        }
                        else
                        {
                            ZoneIndexCompletion = (ZoneIndexCompletion + 1) % zones.Count;
                        }
                        index = ZoneIndexCompletion;
                        break;
                    case "FrameMostSkillpoints":
                        if (zones.Count == 0)
                        {
                            ZoneIndexSkillpoints = 0;
                        }
                        else
                        {
                            ZoneIndexSkillpoints = (ZoneIndexSkillpoints + 1) % zones.Count;
                        }
                        index = ZoneIndexSkillpoints;
                        break;
                    case "FrameMostActivityPoints":
                        if (zones.Count == 0)
                        {
                            ZoneIndexActivityPoints = 0;
                        }
                        else
                        {
                            ZoneIndexActivityPoints = (ZoneIndexActivityPoints + 1) % zones.Count;
                        }
                        index = ZoneIndexActivityPoints;
                        break;
                }

                var zone = "World";
                if (zones.Count > 0)
                {
                    zone = zones[index];
                }

                var labelZone = (control.Parent.GetFirstChild("LabelZone") as CMlLabel)!;
                labelZone.SetText("|Zone|" + zone);

                Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 1, 1);

                var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);

                switch (control.Parent.Parent.ControlId)
                {
                    case "FrameCompletion":
                        UpdateCompletionLeaderboard(leaderboardsUserInfos.Get(), zones);
                        break;
                    case "FrameMostSkillpoints":
                        UpdateSkillpointsLeaderboard(leaderboardsUserInfos.Get(), zones);
                        break;
                    case "FrameMostActivityPoints":
                        UpdateActivityPointsLeaderboard(leaderboardsUserInfos.Get(), zones);
                        break;
                }
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (controlId == "QuadZone")
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
        };
    }

    private static string FormatNumberSpace(int number)
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

    private static string GetFullZone(IList<string> zones, int index)
    {
        var zone = zones[0];

        for (var i = 0; i < zones.Count - 1; i++)
        {
            if (i == index)
            {
                break;
            }

            zone = $"{zone}|{zones[i + 1]}";
        }

        return zone;
    }

    private void UpdateCompletionLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos, IList<string> zones)
    {
        var envimixCompletion = Local<IList<SPlayerCompletion>>.For(Page);
        var defaultCarCompletion = Local<IList<SPlayerCompletion>>.For(Page);
        var globalCompletion = Local<IList<SPlayerCompletion>>.For(Page);

        IList<SPlayerCompletion> completionLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            completionLeaderboard = envimixCompletion.Get();
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            completionLeaderboard = defaultCarCompletion.Get();
        }
        else
        {
            completionLeaderboard = globalCompletion.Get();
        }

        var completionZone = "World";
        if (zones.Count > 0)
        {
            completionZone = GetFullZone(zones, ZoneIndexCompletion);
        }

        var completionOffsetX = 0f;
        var index = 0;
        var rank = 1;
        foreach (var control in FrameCompletionPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= completionLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerCompletion = completionLeaderboard[index];

            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                while (!TextLib.StartsWith(completionZone, leaderboardsUserInfos[playerCompletion.L].Z))
                {
                    index += 1;
                    if (index >= completionLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerCompletion = completionLeaderboard[index];
                }

                if (index >= completionLeaderboard.Count)
                {
                    continue;
                }
            }

            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(1, 1, 1);
            labelRecord.SetText($"{TextLib.FormatReal(playerCompletion.S * 100, 2, false, false)}%");
            
            if (rank == 1)
            {
                completionOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

            var nickname = playerCompletion.L;
            if (leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                nickname = leaderboardsUserInfos[playerCompletion.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = completionOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerCompletion.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal completion stats
        var completionRank = 0;
        var completionScore = -1f;
        foreach (var player in completionLeaderboard)
        {
            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(completionZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                completionScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalCompletion.GetFirstChild("LabelRank") as CMlLabel)!;
        if (completionScore == -1f)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalCompletion.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(1, 1, 1);
        if (completionScore == -1f)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText($"{TextLib.FormatReal(completionScore * 100, 2, false, false)}%");
        }
        labelPersonalRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalCompletion.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = completionOffsetX + 5;
    }

    private void UpdateSkillpointsLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos, IList<string> zones)
    {
        var envimixMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
        var defaultCarMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
        var globalMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);

        IList<SPlayerScore> skillpointsLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            skillpointsLeaderboard = envimixMostSkillpoints.Get();
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            skillpointsLeaderboard = defaultCarMostSkillpoints.Get();
        }
        else
        {
            skillpointsLeaderboard = globalMostSkillpoints.Get();
        }

        var skillpointsZone = "World";
        if (zones.Count > 0)
        {
            skillpointsZone = GetFullZone(zones, ZoneIndexSkillpoints);
        }

        var skillpointsOffsetX = 0f;
        var index = 0;
        var rank = 1;
        foreach (var control in FrameMostSkillpointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= skillpointsLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = skillpointsLeaderboard[index];

            if (skillpointsZone != "World" && leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                while (!TextLib.StartsWith(skillpointsZone, leaderboardsUserInfos[playerScore.L].Z))
                {
                    index += 1;
                    if (index >= skillpointsLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerScore = skillpointsLeaderboard[index];
                }

                if (index >= skillpointsLeaderboard.Count)
                {
                    continue;
                }
            }

            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 0);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (rank == 1)
            {
                skillpointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal skillpoints stats
        var completionRank = 0;
        var pointsScore = -1;
        foreach (var player in skillpointsLeaderboard)
        {
            if (skillpointsZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(skillpointsZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalSkillpoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalSkillpoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 0);
        if (pointsScore == -1)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText(FormatNumberSpace(pointsScore));
        }
        labelPersonalRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalSkillpoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;
    }

    private void UpdateActivityPointsLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos, IList<string> zones)
    {
        var envimixMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
        var defaultCarMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
        var globalMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);

        IList<SPlayerScore> activityPointsLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            activityPointsLeaderboard = envimixMostActivityPoints.Get();
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            activityPointsLeaderboard = defaultCarMostActivityPoints.Get();
        }
        else
        {
            activityPointsLeaderboard = globalMostActivityPoints.Get();
        }

        var activityPointsZone = "World";
        if (zones.Count > 0)
        {
            activityPointsZone = GetFullZone(zones, ZoneIndexActivityPoints);
        }

        var activityPointsOffsetX = 0f;
        var index = 0;
        var rank = 1;
        foreach (var control in FrameMostActivityPointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= activityPointsLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = activityPointsLeaderboard[index];

            if (activityPointsZone != "World" && leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                while (!TextLib.StartsWith(activityPointsZone, leaderboardsUserInfos[playerScore.L].Z))
                {
                    index += 1;
                    if (index >= activityPointsLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerScore = activityPointsLeaderboard[index];
                }

                if (index >= activityPointsLeaderboard.Count)
                {
                    continue;
                }
            }

            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 1);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (rank == 1)
            {
                activityPointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal activity points stats
        var completionRank = 0;
        var pointsScore = -1;
        foreach (var player in activityPointsLeaderboard)
        {
            if (activityPointsZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(activityPointsZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalActivityPoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalActivityPoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 1);
        if (pointsScore == -1)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText(FormatNumberSpace(pointsScore));
        }
        labelPersonalRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalActivityPoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;
    }

    private void UpdateLeaderboards()
    {
        QuadEnvimixLeaderboards.StyleSelected = SelectedLeaderboards == QuadEnvimixLeaderboards;
        QuadDefaultCarLeaderboards.StyleSelected = SelectedLeaderboards == QuadDefaultCarLeaderboards;
        QuadGlobalLeaderboards.StyleSelected = SelectedLeaderboards == QuadGlobalLeaderboards;

        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);

        UpdateCompletionLeaderboard(leaderboardsUserInfos.Get(), zones);
        UpdateSkillpointsLeaderboard(leaderboardsUserInfos.Get(), zones);
        UpdateActivityPointsLeaderboard(leaderboardsUserInfos.Get(), zones);
    }

    private void Show()
    {
        AnimMgr.Add(FrameCategory, "<frame hidden=\"0\" pos=\"0 0\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);

        AnimMgr.Add(FrameCompletion, "<frame hidden=\"0\" pos=\"-105 65\"/>", Now + 400, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostSkillpoints, "<frame hidden=\"0\" pos=\"-35 65\"/>", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostActivityPoints, "<frame hidden=\"0\" pos=\"35 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);

        AnimMgr.Add(FrameOverallCompletion, "<frame hidden=\"0\" pos=\"105 65\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameQuit, "<frame hidden=\"0\" pos=\"130 -50\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

        OpenedAt = Now;
    }

    private void Hide()
    {
        FrameQuit.Visible = false;

        AnimMgr.Add(FrameCategory, "<frame hidden=\"1\" pos=\"0 30\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameCompletion, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostSkillpoints, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostActivityPoints, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameOverallCompletion, "<frame hidden=\"1\" pos=\"210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameQuit, "<frame hidden=\"0\" pos=\"130 -90\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    public void Main()
    {
        FrameCategory.RelativePosition_V3.Y = 30;
        FrameCategory.Visible = false;

        FrameCompletion.RelativePosition_V3.X = -210;
        FrameCompletion.Visible = false;

        FrameMostSkillpoints.RelativePosition_V3.X = -210;
        FrameMostSkillpoints.Visible = false;

        FrameMostActivityPoints.RelativePosition_V3.X = -210;
        FrameMostActivityPoints.Visible = false;

        FrameOverallCompletion.RelativePosition_V3.X = 210;
        FrameOverallCompletion.Visible = false;

        FrameQuit.RelativePosition_V3.Y = -90;
        FrameQuit.Visible = false;

        SelectedLeaderboards = QuadEnvimixLeaderboards;

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        UpdateLeaderboards();
    }

    public void Loop()
    {
        if (OpenedAt != -1)
        {
            float percentage;
            if (SelectedLeaderboards == QuadEnvimixLeaderboards)
            {
                percentage = EnvimixCompletionPercentage;
            }
            else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
            {
                percentage = DefaultCarCompletionPercentage;
            }
            else
            {
                percentage = GlobalCompletionPercentage;
            }

            var animatedOverallCompletion = AnimLib.EaseOutQuad(Now - OpenedAt, 0, percentage * 100, 1000);
            LabelOverallCompletion.Value = $"{TextLib.FormatReal(animatedOverallCompletion, 2, false, false)}%";
        }
    }
}
