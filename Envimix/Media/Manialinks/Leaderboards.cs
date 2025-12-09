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

    /*[Local(LocalFor.LocalUser)] public IList<SPlayerCompletion> EnvimixCompletion { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostSkillpoints { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostActivityPoints { get; set; }*/

    public Leaderboards()
    {
        QuadQuit.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
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
        };

        QuadDefaultCarLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadDefaultCarLeaderboards;
            UpdateLeaderboards();
        };

        QuadGlobalLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadGlobalLeaderboards;
            UpdateLeaderboards();
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

    private void UpdateLeaderboards()
    {
        QuadEnvimixLeaderboards.StyleSelected = SelectedLeaderboards == QuadEnvimixLeaderboards;
        QuadDefaultCarLeaderboards.StyleSelected = SelectedLeaderboards == QuadDefaultCarLeaderboards;
        QuadGlobalLeaderboards.StyleSelected = SelectedLeaderboards == QuadGlobalLeaderboards;

        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);

        var envimixCompletion = Local<IList<SPlayerCompletion>>.For(Page);
        var envimixMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
        var envimixMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);

        var defaultCarCompletion = Local<IList<SPlayerCompletion>>.For(Page);
        var defaultCarMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
        var defaultCarMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);

        var globalCompletion = Local<IList<SPlayerCompletion>>.For(Page);
        var globalMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
        var globalMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);

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
        var completionOffsetX = 0f;
        var index = 0;
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
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(1, 1, 1);
            labelRecord.SetText($"{TextLib.FormatReal(playerCompletion.S * 100, 2, false, false)}%");
            
            if (index == 0)
            {
                completionOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

            var nickname = playerCompletion.L;
            if (leaderboardsUserInfos.Get().ContainsKey(playerCompletion.L))
            {
                nickname = leaderboardsUserInfos.Get()[playerCompletion.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = completionOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerCompletion.L;

            frame.Show();

            index += 1;
        }


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
        var skillpointsOffsetX = 0f;
        index = 0;
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
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 0);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (index == 0)
            {
                skillpointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.Get().ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos.Get()[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
        }

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
        var activityPointsOffsetX = 0f;
        index = 0;
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
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 1);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (index == 0)
            {
                activityPointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.Get().ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos.Get()[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
        }

        var completionRank = 0;
        var completionScore = -1f;
        foreach (var player in completionLeaderboard)
        {
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

        completionRank = 0;
        var pointsScore = -1;
        foreach (var player in skillpointsLeaderboard)
        {
            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }
        labelPersonalRank = (FramePersonalSkillpoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }
        labelPersonalRecord = (FramePersonalSkillpoints.GetFirstChild("LabelRecord") as CMlLabel)!;
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
        labelPersonalNickname = (FramePersonalSkillpoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

        completionRank = 0;
        pointsScore = -1;
        foreach (var player in activityPointsLeaderboard)
        {
            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }
        labelPersonalRank = (FramePersonalActivityPoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }
        labelPersonalRecord = (FramePersonalActivityPoints.GetFirstChild("LabelRecord") as CMlLabel)!;
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
        labelPersonalNickname = (FramePersonalActivityPoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;
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
