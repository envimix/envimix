namespace Envimix.Media.Manialinks.Universe2;

public class Scoreboard : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameGlobalScores;
    [ManialinkControl] public required CMlFrame FrameYourScore;
    [ManialinkControl] public required CMlLabel LabelYourName;

    public required Dictionary<string, int> PlayerPoints { get; set; }
    public required Dictionary<string, int> PlayerTeams { get; set; }
    public required Dictionary<string, CUser.EEchelon> PlayerEchelons { get; set; }

    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }

    public Scoreboard()
    {
        RaceEvent += Scoreboard_RaceEvent;
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
        return !IsInGameMenuDisplayed;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    static int EchelonToInteger(CUser.EEchelon echelon)
    {
        switch (echelon)
        {
            case CUser.EEchelon.Bronze1: return 1;
            case CUser.EEchelon.Bronze2: return 2;
            case CUser.EEchelon.Bronze3: return 3;
            case CUser.EEchelon.Silver1: return 4;
            case CUser.EEchelon.Silver2: return 5;
            case CUser.EEchelon.Silver3: return 6;
            case CUser.EEchelon.Gold1: return 7;
            case CUser.EEchelon.Gold2: return 8;
            case CUser.EEchelon.Gold3: return 9;
        }

        return 0;
    }

    private void UpdatePlayer(CMlFrame frame, CTmScore score)
    {
        var quadTeam = (frame.GetFirstChild("QuadTeam") as CMlQuad)!;
        quadTeam.BgColor = Teams[score.TeamNum - 1].ColorPrimary;

        var quadEchelon = (frame.GetFirstChild("QuadEchelon") as CMlQuad)!;
        quadEchelon.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{EchelonToInteger(score.User.Echelon)}.dds");

        var quadZone = (frame.GetFirstChild("QuadZone") as CMlQuad)!;
        quadZone.ChangeImageUrl($"file://ZoneFlags/Path/{score.User.ZonePath}");

        if (score.User.Echelon == CUser.EEchelon.None)
        {
            quadZone.RelativePosition_V3.Y = 0;
        }
        else
        {
            quadZone.RelativePosition_V3.Y = -0.4;
        }

        var labelPlayerName = (frame.GetFirstChild("LabelPlayerName") as CMlLabel)!;
        labelPlayerName.SetText(score.User.Name);

        var labelScore = (frame.GetFirstChild("LabelScore") as CMlLabel)!;
        labelScore.SetText(score.Points.ToString());
    }

    private void UpdateScoreboard()
    {
        if (InputPlayer is not null)
        {
            LabelYourName.SetText(InputPlayer.User.Name);
            UpdatePlayer(FrameYourScore, InputPlayer.Score);
        }

        for (int i = 0; i < FrameGlobalScores.Controls.Count; i++)
        {
            var frame = (FrameGlobalScores.Controls[i] as CMlFrame)!;

            if (Scores.Count <= i)
            {
                frame.Visible = false;
                continue;
            }

            UpdatePlayer(frame, Scores[i]);

            frame.Visible = true;
        }
    }

    private void Scoreboard_RaceEvent(CTmRaceClientEvent e)
    {
        switch (e.Type)
        {
            case CTmRaceClientEvent.EType.WayPoint:
                if (e.IsEndRace)
                {
                    UpdateScoreboard();
                }
                break;
        }

        if (e.Player != InputPlayer)
        {
            return;
        }

        switch (e.Type)
        {
            case CTmRaceClientEvent.EType.WayPoint:
                if (e.IsEndRace)
                {
                    ClientUI.ScoreTableVisibility = CUIConfig.EVisibility.ForcedVisible;
                }
                break;
            case CTmRaceClientEvent.EType.Respawn:
                ClientUI.ScoreTableVisibility = CUIConfig.EVisibility.Normal;
                break;
        }
    }

    public void Main()
    {
        Wait(() => GetPlayer() is not null);

        UpdateScoreboard();
    }

    private bool DetectChange()
    {
        foreach (var score in Scores)
        {
            if (!PlayerPoints.ContainsKey(score.User.Login))
            {
                PlayerPoints[score.User.Login] = score.Points;
                return true;
            }
            else if (PlayerPoints[score.User.Login] != score.Points)
            {
                PlayerPoints[score.User.Login] = score.Points;
                return true;
            }

            if (!PlayerTeams.ContainsKey(score.User.Login))
            {
                PlayerTeams[score.User.Login] = score.TeamNum;
                return true;
            }
            else if (PlayerTeams[score.User.Login] != score.TeamNum)
            {
                PlayerTeams[score.User.Login] = score.TeamNum;
                return true;
            }

            if (!PlayerEchelons.ContainsKey(score.User.Login))
            {
                PlayerEchelons[score.User.Login] = score.User.Echelon;
                return true;
            }
            else if (PlayerEchelons[score.User.Login] != score.User.Echelon)
            {
                PlayerEchelons[score.User.Login] = score.User.Echelon;
                return true;
            }
        }

        return false;
    }

    public void Loop()
    {
        ScoreTableIsVisible = PageIsVisible;

        if (DetectChange())
        {
            UpdateScoreboard();
        }
    }
}
