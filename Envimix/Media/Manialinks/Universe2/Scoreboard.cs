namespace Envimix.Media.Manialinks.Universe2;

public class Scoreboard : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameGlobalScores;
    [ManialinkControl] public required CMlLabel LabelYourName;

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

    private void UpdateScoreboard()
    {
        if (InputPlayer is not null)
        {
            LabelYourName.SetText(InputPlayer.User.Name);
        }

        for (int i = 0; i < FrameGlobalScores.Controls.Count; i++)
        {
            var frame = (FrameGlobalScores.Controls[i] as CMlFrame)!;

            if (Scores.Count <= i)
            {
                frame.Visible = false;
                continue;
            }

            var labelPlayerName = (frame.GetFirstChild("LabelPlayerName") as CMlLabel)!;
            labelPlayerName.SetText(Scores[i].User.Name);

            var quadEchelon = (frame.GetFirstChild("QuadEchelon") as CMlQuad)!;
            quadEchelon.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{EchelonToInteger(Scores[i].User.Echelon)}.dds");

            //var score = Scores[i];
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

    public void Loop()
    {
        ScoreTableIsVisible = PageIsVisible;
    }
}
