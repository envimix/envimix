using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class LiveRankingsCar : CTmMlScriptIngame, IContext
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

    [ManialinkControl] public required CMlFrame FrameLiveRankingsCar;
    [ManialinkControl] public required CMlFrame FrameLiveRankingsCarBg;
    [ManialinkControl] public required CMlFrame FrameLiveRankingsRecords;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelCar;
    [ManialinkControl] public required CMlFrame FrameRecords;

    public bool PreviousVisible;
    public int VisibleTime = -1;

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    public void Main()
    {
        FrameLiveRankingsCar.Visible = IsVisible();
        PreviousVisible = FrameLiveRankingsCar.Visible;

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        var car = Netread<string>.For(GetPlayer());

        FrameLiveRankingsCar.Visible = IsVisible();

        if (FrameLiveRankingsCar.Visible != PreviousVisible)
        {
            if (FrameLiveRankingsCar.Visible)
            {
                foreach (var control in FrameLiveRankingsCarBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 6\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                FrameLiveRankingsRecords.Controls[0].Size.X = 0;
                AnimMgr.Add(FrameLiveRankingsRecords.Controls[0], "<quad size=\"42.5 52.5\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                VisibleTime = Now;
            }

            PreviousVisible = FrameLiveRankingsCar.Visible;
        }

        if (VisibleTime == -1)
        {
            FrameRecords.ClipWindowSize.X = 52.5;
            FrameLabelCar.ClipWindowSize.X = 52.5;
        }
        else
        {
            FrameRecords.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 52.5f, 500);
            FrameLabelCar.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 52.5f, 500);
        }

        LabelCar.Value = car.Get();

        Dictionary<string, int> loginSort = new();

        for (var i = 0; i < Scores.Count; i++)
        {
            var envimixPoints = Netread<Dictionary<string, int>>.For(Scores[i]);

            if (envimixPoints.Get().ContainsKey(car.Get()))
            {
                loginSort[Scores[i].User.Login] = envimixPoints.Get()[car.Get()];
            }
            else
            {
                loginSort[Scores[i].User.Login] = 0;
            }
        }

        loginSort = loginSort.SortReverse();

        ImmutableArray<int> carScores = new();

        foreach (var (login, s) in loginSort)
        {
            for (int i = 0; i < Scores.Count; i++)
            {
                if (login == Scores[i].User.Login)
                {
                    carScores.Add(i);
                }
            }
        }

        var offset = 0;
        var previousScore = 0;

        for (int i = 0; i < FrameRecords.Controls.Count; i++)
        {
            var frame = (FrameRecords.Controls[i] as CMlFrame)!;

            if (carScores.Length <= i)
            {
                frame.Hide();
                continue;
            }

            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;

            var envimixPoints = Netread<Dictionary<string, int>>.For(Scores[carScores[i]]);
            var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(Scores[carScores[i]]);

            if (envimixPoints.Get().ContainsKey(car.Get()))
            {
                if (envimixPoints.Get()[car.Get()] > 0)
                {
                    if (envimixPoints.Get()[car.Get()] == previousScore)
                    {
                        offset += 1;
                    }
                    else
                    {
                        offset = 0;
                    }

                    previousScore = envimixPoints.Get()[car.Get()];
                    labelRank.Value = TextLib.FormatInteger(i - offset + 1, 2);

                    labelRank.Size.X = labelRank.ComputeWidth(labelRank.Value);
                    labelRank.Visible = true;

                    labelNickname.Parent.RelativePosition_V3.X = labelRank.Size.X + 1;
                }
                else
                {
                    labelRank.Visible = false;
                    labelNickname.Parent.RelativePosition_V3.X = 0;
                }

                if (envimixBestRace.Get().ContainsKey(car.Get()))
                {
                    labelTime.Value = TimeToTextWithMilli(envimixBestRace.Get()[car.Get()].Time);
                }
                else
                {
                    labelTime.Value = "-:--.---";
                }
            }
            else
            {
                labelRank.Visible = false;
                labelNickname.Parent.RelativePosition_V3.X = 0;
                labelTime.Value = "-:--.---";
            }

            labelNickname.Value = Scores[carScores[i]].User.Name;

            frame.Show();
        }
    }
}
