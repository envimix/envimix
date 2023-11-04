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
    [ManialinkControl] public required CMlQuad QuadEchelonPercent;
    [ManialinkControl] public required CMlQuad QuadEchelonCurrent;
    [ManialinkControl] public required CMlQuad QuadEchelonNext;
    [ManialinkControl] public required CMlLabel LabelEchelonCurrent;
    [ManialinkControl] public required CMlLabel LabelEchelonNext;
    [ManialinkControl] public required CMlFrame FrameDifficulty;
    [ManialinkControl] public required CMlFrame FrameQuality;
    [ManialinkControl] public required CMlQuad QuadMyCar;
    [ManialinkControl] public required CMlLabel LabelMyCar;

    public required CMlLabel LabelDifficulty;
    public required CMlLabel LabelQuality;
    public required CMlQuad QuadDifficultyBlink;
    public required CMlQuad QuadQualityBlink;
    public required CMlFrame? Hold;

    public float CurrentLadderPoints;
    public required Dictionary<string, int> PlayerPoints;
    public required Dictionary<string, int> PlayerTeams;
    public required Dictionary<string, CUser.EEchelon> PlayerEchelons;
    public required Dictionary<string, string> PlayerCars;
    public required Dictionary<string, int> LastUpdated;
    public required Dictionary<string, Dictionary<string, int>> Ranks;

    public float Difficulty;
    public float Quality;

    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }

    public Scoreboard()
    {
        RaceEvent += Scoreboard_RaceEvent;
        MouseClick += Scoreboard_MouseClick;
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

    private void UpdatePlayer(CMlFrame frame, CTmScore score, int rank)
    {
        var quadTeam = (frame.GetFirstChild("QuadTeam") as CMlQuad)!;
        quadTeam.BgColor = Teams[score.TeamNum - 1].ColorPrimary;

        var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;

        if (rank == 0)
        {
            labelRank.SetText("--");
        }
        else
        {
        labelRank.SetText(TextLib.FormatInteger(rank, 2));
        }

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
            var currentCarUrl = $"https://envimix.bigbang1112.cz/img/cars/{PlayerCars[score.User.Login]}.png";

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

        QuadEchelonPercent.Size.X = LocalUser.NextEchelonPercent / 100f * 72;
        QuadEchelonCurrent.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{EchelonToInteger(LocalUser.Echelon)}.dds");
        LabelEchelonCurrent.Value = EchelonToInteger(LocalUser.Echelon).ToString();
        if (EchelonToInteger(LocalUser.Echelon) + 1 < 10)
        {
            QuadEchelonNext.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{EchelonToInteger(LocalUser.Echelon) + 1}.dds");
            LabelEchelonNext.Value = (EchelonToInteger(LocalUser.Echelon) + 1).ToString();
        }
        else
        {
            QuadEchelonNext.ChangeImageUrl("");
            LabelEchelonNext.Value = "";
        }

        if (PlayerCars.ContainsKey(LocalUser.Login))
        {
            var currentCarUrl = $"https://envimix.bigbang1112.cz/img/cars/{PlayerCars[LocalUser.Login]}.png";

            if (QuadMyCar.ImageUrl != currentCarUrl)
            {
                QuadMyCar.ChangeImageUrl(currentCarUrl);
            }

            LabelMyCar.Value = PlayerCars[LocalUser.Login];

            QuadMyCar.Show();
            LabelMyCar.Show();
        }
        else
        {
            QuadMyCar.Hide();
            LabelMyCar.Hide();
        }

        Ranks = new();

        if (InputPlayer is not null)
        {
            if (InputPlayer.Score is not null)
            {
                UpdatePlayer(FrameYourScore, InputPlayer.Score, rank: 0);
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

            UpdatePlayer(frame, Scores[i], rank: i + 1);

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

    private void Scoreboard_MouseClick(CMlControl control, string controlId)
    {
        if (controlId == "QuadBox" || controlId == "QuadDraggable")
        {
            CMlFrame frame;

            if (controlId == "QuadBox")
            {
                frame = control.Parent.Parent;
            }
            else
            {
                frame = control.Parent.Parent.Parent;
            }
            
            frame.GetFirstChild("LabelRateName").Hide();

            if (frame.ControlId == "FrameDifficulty")
            {
                QuadDifficultyBlink.Hide();
            }
            else if (frame.ControlId == "FrameQuality")
            {
                QuadQualityBlink.Hide();
            }
        }

        if (controlId == "QuadBox")
        {
            var frameDraggable = (control.Parent.Parent.GetFirstChild("FrameDraggable") as CMlFrame)!;
            frameDraggable.Show();

            Hold = frameDraggable;
        }
        else if (controlId == "QuadDraggable")
        {
            Hold = control.Parent;
        }
    }

    public void Main()
    {
        Difficulty = -1;
        Quality = -1;

        CurrentLadderPoints = -2;

        LabelDifficulty = (FrameDifficulty.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelDifficulty.SetText("Difficulty");
        QuadDifficultyBlink = (FrameDifficulty.GetFirstChild("QuadBlink") as CMlQuad)!;
        LabelQuality = (FrameQuality.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelQuality.SetText("Quality");
        QuadQualityBlink = (FrameQuality.GetFirstChild("QuadBlink") as CMlQuad)!;

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

        QuadDifficultyBlink.Opacity = (MathLib.Sin(Now / 100f) + 1) / 2f * .1f;
        QuadQualityBlink.Opacity = (MathLib.Sin(Now / 100f + 180) + 1) / 2f * .1f;

        if (Hold is not null)
        {
            if (MouseLeftButton)
            {
                var frame = Hold.Parent.Parent;

                var visualValue = MathLib.Clamp(MouseX - (float)frame.RelativePosition_V3.X, -28, 28);
                var realValue = (visualValue + 28) / 56;

                Hold.RelativePosition_V3.X = visualValue;

                (Hold.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = true;

                if (frame.ControlId == "FrameDifficulty")
                {
                    Difficulty = realValue;
                }
                else if (frame.ControlId == "FrameQuality")
                {
                    Quality = realValue;
                }

                var gauge = (frame.GetFirstChild("GaugeRating") as CMlGauge)!;
                gauge.SetRatio(realValue);
            }
            else
            {
                (Hold.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = false;

                var frame = Hold.Parent.Parent;

                if (frame.ControlId == "FrameDifficulty")
                {
                    SendCustomEvent("Rate", new[] { "Difficulty", Difficulty.ToString() });
                }
                else if (frame.ControlId == "FrameQuality")
                {
                    SendCustomEvent("Rate", new[] { "Quality", Quality.ToString() });
                }

                Hold = null;
            }
        }
    }
}
