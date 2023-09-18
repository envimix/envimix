using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class RankingsCar : CTmMlScriptIngame, IContext
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

    [ManialinkControl] public required CMlFrame FrameRankingsCar;
    [ManialinkControl] public required CMlFrame FrameRankingsCarPlayerBg;
    [ManialinkControl] public required CMlFrame FrameLabelPlayer;
    [ManialinkControl] public required CMlLabel LabelPlayer;
    [ManialinkControl] public required CMlFrame FrameCars;

    public bool PreviousVisible;
    public int VisibleTime = -1;

    [Netread] public ImmutableArray<string> DisplayedCars { get; }
    [Netread] public bool EnableDefaultCar { get; set; }
    [Netread] public string MapPlayerModelName { get; set; }

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

    public void Main()
    {
        FrameRankingsCar.Visible = IsVisible();
        PreviousVisible = FrameRankingsCar.Visible;

        VisibleTime = -1;

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        LabelPlayer.Value = GetPlayer().User.Name;

        var ranker = new Dictionary<string, Dictionary<string, int>>();

        if (GetPlayer().Score is not null)
        {
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
        }

        var ranks = new Dictionary<string, int>();

        foreach (var (car, times) in ranker)
        {
            var offset = 0;
            var prevTime = 0;
            var index = 1;

            foreach (var (login, time) in times.Sort())
            {
                if (time == prevTime)
                {
                    offset += 1;
                }

                if (login == GetPlayer().User.Login)
                {
                    ranks[car] = index - offset;
                }

                prevTime = time;
                index += 1;
            }
        }

        var iOffset = 0;

        for (int i = 0; i < FrameCars.Controls.Count; i++)
        {
            var frame = (FrameCars.Controls[i] as CMlFrame)!;

            if (DisplayedCars.Length <= i + iOffset)
            {
                frame.Hide();
                continue;
            }

            var labelCar = (frame.GetFirstChild("LabelCar") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;
            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;

            var car = DisplayedCars[i + iOffset];

            if (!EnableDefaultCar && car == MapPlayerModelName)
            {
                iOffset += 1;
                car = DisplayedCars[i + iOffset];
            }

            labelCar.Value = car;

            if (GetPlayer().Score is not null)
            {
                var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(GetPlayer().Score);

                if (envimixBestRace.Get().ContainsKey(car) && envimixBestRace.Get()[car].Time != -1)
                {
                    labelTime.Value = TimeToTextWithMilli(envimixBestRace.Get()[car].Time);

                    if (ranks.ContainsKey(car))
                    {
                        var rank = ranks[car];
                        labelRank.Value = TextLib.FormatInteger(rank, 2);
                    }
                    else
                    {
                        labelRank.Value = "";
                    }
                }
                else
                {
                    labelTime.Value = "-:--.---";
                    labelRank.Value = "";
                }
            }

            frame.Show();
        }

        FrameRankingsCar.Visible = IsVisible();

        if (FrameRankingsCar.Visible != PreviousVisible)
        {
            if (FrameRankingsCar.Visible)
            {
                foreach (var control in FrameRankingsCarPlayerBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"52.5 6\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                for (int i = 0; i < FrameCars.Controls.Count; i++)
                {
                    var frame = ((FrameCars.Controls[i] as CMlFrame)!.GetFirstChild("FrameInner") as CMlFrame)!;
                    frame.RelativePosition_V3.Y = 6;
                    AnimMgr.Add(frame, "<frame pos=\"0 6\"/>", 100 * i + 100, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.AddChain(frame, "<frame pos=\"0 0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                VisibleTime = Now;
            }

            PreviousVisible = FrameRankingsCar.Visible;
        }

        if (VisibleTime == -1)
        {
            FrameLabelPlayer.ClipWindowSize.X = 50;
        }
        else
        {
            FrameLabelPlayer.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 50, 300);
        }
    }
}
