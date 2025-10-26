using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class LiveRankingsCar2 : CTmMlScriptIngame, IContext
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

    public Dictionary<string, bool> GetPlayersOf(string carName)
    {
        Dictionary<string, bool> playerLogins = new();

        foreach (var player in Players)
        {
            var car = Netread<string>.For(player);

            if (carName == car.Get())
            {
                playerLogins[player.User.Login] = true;
            }
        }

        return playerLogins;
    }

    string ConstructFilterKey(string car)
    {
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car}_{gravity.Get()}_Time";
    }

    public void Loop()
    {
        var car = Netread<string>.For(GetPlayer());

        var playersOfThisCar = GetPlayersOf(car.Get());

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
            FrameRecords.ClipWindowSize.X = 53.5;
            FrameLabelCar.ClipWindowSize.X = 53.5;
        }
        else
        {
            FrameRecords.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 53.5f, 500);
            FrameLabelCar.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 53.5f, 500);
        }

        LabelCar.Value = car.Get();

        Dictionary<string, int> playerTimes = new();
        ImmutableArray<string> playerLogins = new();
        Dictionary<string, int> scoreIndices = new();

        var key = ConstructFilterKey(car.Get());

        for (var i = 0; i < Scores.Count; i++)
        {
            var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(Scores[i]);

            var login = Scores[i].User.Login;

            if (envimixBestRace.Get().ContainsKey(key))
            {
                playerTimes[login] = envimixBestRace.Get()[key].Time;
            }
            else
            {
                playerTimes[login] = 2147483647;
            }

            scoreIndices[login] = i;
        }

        playerTimes.Sort();

        foreach (var (login, time) in playerTimes)
        {
            playerLogins.Add(login);
        }

        var offset = 0;
        var previousTime = 0;

        for (int i = 0; i < FrameRecords.Controls.Count; i++)
        {
            var frame = (FrameRecords.Controls[i] as CMlFrame)!;

            if (playerLogins.Length <= i)
            {
                frame.Hide();
                continue;
            }

            frame.Show();

            var login = playerLogins[i];
            var time = playerTimes[login];
            var scoreIndex = scoreIndices[login];

            var quadTeam = (frame.GetFirstChild("QuadTeam") as CMlQuad)!;
            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;
            var quadCar = (frame.GetFirstChild("QuadCar") as CMlQuad)!;

            labelNickname.Value = Scores[scoreIndex].User.Name;

            quadTeam.BgColor = Teams[Scores[scoreIndex].TeamNum - 1].ColorPrimary;

            if (car.Get() != "" && playersOfThisCar.ContainsKey(login))
            {
                quadCar.ChangeImageUrl($"https://envimix.gbx.tools/img/cars/{car.Get()}.png");
                quadCar.Show();
            }
            else
            {
                quadCar.Hide();
            }

            if (time == 2147483647)
            {
                labelRank.Visible = false;
                labelNickname.Parent.RelativePosition_V3.X = 0;
                labelTime.Value = "-:--.---";
                continue;
            }

            if (time == previousTime)
            {
                offset += 1;
            }
            else
            {
                offset = 0;
            }

            previousTime = time;

            labelRank.Value = TextLib.FormatInteger(i - offset + 1, 2);

            labelRank.Size.X = labelRank.ComputeWidth(labelRank.Value);
            labelRank.Visible = true;

            labelNickname.Parent.RelativePosition_V3.X = labelRank.Size.X + 1;

            labelTime.Value = TimeToTextWithMilli(time);
        }
    }
}
