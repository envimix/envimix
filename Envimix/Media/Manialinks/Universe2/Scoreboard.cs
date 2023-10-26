using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Scoreboard : CTmMlScriptIngame, IContext
{
    public struct SCheckpoint
    {
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
    }

    public struct SRecord
    {
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public ImmutableArray<SCheckpoint> Checkpoints;
    }

    [ManialinkControl] public required CMlFrame FrameGlobalScores;
    [ManialinkControl] public required CMlFrame FrameYourScore;
    [ManialinkControl] public required CMlLabel LabelYourName;
    [ManialinkControl] public required CMlLabel LabelLadderPoints;
    [ManialinkControl] public required CMlLabel LabelLadderZone;

    public float CurrentLadderPoints;
    public required Dictionary<string, int> PlayerPoints;
    public required Dictionary<string, int> PlayerTeams;
    public required Dictionary<string, CUser.EEchelon> PlayerEchelons;
    public required Dictionary<string, string> PlayerCars;
    public required Dictionary<string, int> LastUpdated;
    public required Dictionary<string, Dictionary<string, int>> Ranks;

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

        var frameCarRanks = (frame.GetFirstChild("FrameCarRanks") as CMlFrame)!;

        foreach (var control in frameCarRanks.Controls)
        {
            var label = (control as CMlLabel)!;
            var car = label.DataAttributeGet("car");

            if (Ranks.ContainsKey(car) && Ranks[car].ContainsKey(score.User.Login))
            {
                label.SetText(TextLib.FormatInteger(Ranks[car][score.User.Login], 2));
            }
            else
            {
                label.SetText("--");
            }
        }

        var quadCurrentCar = (frame.GetFirstChild("QuadCurrentCar") as CMlQuad)!;
        
        if (PlayerCars.ContainsKey(score.User.Login))
        {
            var currentCarUrl = $"file://Media/Images/Cars/{PlayerCars[score.User.Login]}.png";

            if (quadCurrentCar.ImageUrl != currentCarUrl)
            {
                quadCurrentCar.ChangeImageUrl(currentCarUrl);
            }
        }
    }

    private void UpdateScoreboard()
    {
        LabelYourName.SetText(LocalUser.Name);
        LabelLadderPoints.SetText(TextLib.FormatReal(LocalUser.LadderPoints, 1, _HideZeroes: false, _HideDot: false));
        
        if (LocalUser.LadderRank == -1)
        {
            LabelLadderZone.Value = "Not ranked";
        }
        else
        {
            LabelLadderZone.Value = $"{TextLib.GetTranslatedText(LocalUser.LadderZoneName)}: $ff0{LocalUser.LadderRank}$aaa / {LocalUser.LadderTotal}";
        }

        Ranks = new();

        if (InputPlayer is not null)
        {
            if (InputPlayer.Score is not null)
            {
                UpdatePlayer(FrameYourScore, InputPlayer.Score);
            }

            var ranker = new Dictionary<string, Dictionary<string, int>>();

            foreach (var score in Scores)
            {
                var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(score);

                foreach (var (car, time) in envimixBestRace.Get())
                {
                    if (!ranker.ContainsKey(car))
                    {
                        ranker[car] = new();
                    }

                    ranker[car][score.User.Login] = envimixBestRace.Get()[car].Time;
                }
            }

            foreach (var (car, times) in ranker)
            {
                Ranks[car] = new();

                var offset = 0;
                var prevTime = 0;
                var index = 1;

                foreach (var (login, time) in times.Sort())
                {
                    if (time == prevTime)
                    {
                        offset += 1;
                    }

                    Ranks[car][login] = index - offset;

                    prevTime = time;
                    index += 1;
                }
            }
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
        CurrentLadderPoints = -2;

        Wait(() => GetPlayer() is not null);

        UpdateScoreboard();
    }

    private bool DetectChange()
    {
        if (InputPlayer is not null && InputPlayer.User.LadderPoints != CurrentLadderPoints)
        {
            CurrentLadderPoints = InputPlayer.User.LadderPoints;
            return true;
        }

        foreach (var score in Scores)
        {
            if (!PlayerPoints.ContainsKey(score.User.Login) || PlayerPoints[score.User.Login] != score.Points)
            {
                PlayerPoints[score.User.Login] = score.Points;
                return true;
            }

            if (!PlayerTeams.ContainsKey(score.User.Login) || PlayerTeams[score.User.Login] != score.TeamNum)
            {
                PlayerTeams[score.User.Login] = score.TeamNum;
                return true;
            }

            if (!PlayerEchelons.ContainsKey(score.User.Login) || PlayerEchelons[score.User.Login] != score.User.Echelon)
            {
                PlayerEchelons[score.User.Login] = score.User.Echelon;
                return true;
            }

            var envimixRecordUpdated = Netread<int>.For(score);

            if (!LastUpdated.ContainsKey(score.User.Login) || LastUpdated[score.User.Login] != envimixRecordUpdated.Get())
            {
                LastUpdated[score.User.Login] = envimixRecordUpdated.Get();
                return true;
            }
        }

        foreach (var player in Players)
        {
            var car = Netread<string>.For(player);

            if (!PlayerCars.ContainsKey(player.User.Login) || PlayerCars[player.User.Login] != car.Get())
            {
                PlayerCars[player.User.Login] = car.Get();
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
