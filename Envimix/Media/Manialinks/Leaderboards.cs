namespace Envimix.Media.Manialinks;

public class Leaderboards : CManiaAppTitleLayer, IContext
{
    public struct SPlayerScore
    {
        public string PlayerLogin;
        public string PlayerNickname;
        public int Score;
    }

    public struct SPlayerCompletion
    {
        public string PlayerLogin;
        public string PlayerNickname;
        public float Score;
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

    public int OpenedAt = -1;
    public float EnvimixOverallCompletion;

    [Local(LocalFor.LocalUser)] public IList<SPlayerCompletion> EnvimixCompletion { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostSkillpoints { get; set; }
    [Local(LocalFor.LocalUser)] public IList<SPlayerScore> EnvimixMostActivityPoints { get; set; }

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
                    EnvimixOverallCompletion = TextLib.ToReal(data[0]);
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

    private void UpdateLeaderboards()
    {
        var completionOffsetX = 0f;
        var index = 0;
        foreach (var control in FrameCompletionPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= EnvimixCompletion.Count)
            {
                frame.Hide();
                continue;
            }

            var playerCompletion = EnvimixCompletion[index];
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(1, 1, 1);
            labelRecord.SetText($"{TextLib.FormatReal(playerCompletion.Score * 100, 2, false, false)}%");
            
            if (index == 0)
            {
                completionOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(playerCompletion.PlayerNickname);
            labelNickname.RelativePosition_V3.X = completionOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerCompletion.PlayerLogin;

            frame.Show();

            index += 1;
        }

        var skillpointsOffsetX = 0f;
        index = 0;
        foreach (var control in FrameMostSkillpointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= EnvimixMostSkillpoints.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = EnvimixMostSkillpoints[index];
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 0);
            labelRecord.SetText(FormatNumberSpace(playerScore.Score));

            if (index == 0)
            {
                skillpointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(playerScore.PlayerNickname);
            labelNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.PlayerLogin;

            frame.Show();

            index += 1;
        }

        var activityPointsOffsetX = 0f;
        index = 0;
        foreach (var control in FrameMostActivityPointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= EnvimixMostSkillpoints.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = EnvimixMostActivityPoints[index];
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(index + 1, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 1);
            labelRecord.SetText(FormatNumberSpace(playerScore.Score));

            if (index == 0)
            {
                activityPointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(playerScore.PlayerNickname);
            labelNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.PlayerLogin;

            frame.Show();

            index += 1;
        }

        var labelPersonalRank = (FramePersonalCompletion.GetFirstChild("LabelRank") as CMlLabel)!;
        labelPersonalRank.SetText("--");
        var labelPersonalRecord = (FramePersonalCompletion.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(1, 1, 1);
        labelPersonalRecord.SetText("TBD");
        labelPersonalRecord.RelativePosition_V3.X = completionOffsetX + 2.5;
        var labelPersonalNickname = (FramePersonalCompletion.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = completionOffsetX + 5;

        labelPersonalRank = (FramePersonalSkillpoints.GetFirstChild("LabelRank") as CMlLabel)!;
        labelPersonalRank.SetText("--");
        labelPersonalRecord = (FramePersonalSkillpoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 0);
        labelPersonalRecord.SetText("TBD");
        labelPersonalRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;
        labelPersonalNickname = (FramePersonalSkillpoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

        labelPersonalRank = (FramePersonalActivityPoints.GetFirstChild("LabelRank") as CMlLabel)!;
        labelPersonalRank.SetText("--");
        labelPersonalRecord = (FramePersonalActivityPoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 1);
        labelPersonalRecord.SetText("TBD");
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

        UpdateLeaderboards();
    }

    public void Loop()
    {
        if (OpenedAt != -1)
        {
            var animatedOverallCompletion = AnimLib.EaseOutQuad(Now - OpenedAt, 0, EnvimixOverallCompletion * 100, 1000);
            LabelOverallCompletion.Value = $"{TextLib.FormatReal(animatedOverallCompletion, 2, false, false)}%";
        }
    }
}
