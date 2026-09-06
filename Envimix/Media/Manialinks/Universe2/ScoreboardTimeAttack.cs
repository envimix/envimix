using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class ScoreboardTimeAttack : CTmMlScriptIngame, IContext
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

    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SRatingFilter
    {
        public string Car;
        public int Gravity;
        public string Type;
    }

    public struct SFilteredRating
    {
        public SRatingFilter Filter;
        public SRating Rating;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    public struct SRatingStarRequest
    {
        public string MapUid;
        public SRatingFilter Filter;
    }

    [ManialinkControl] public required CMlFrame FrameLeaderboard0;
    [ManialinkControl] public required CMlFrame FrameLeaderboard1;
    [ManialinkControl] public required CMlFrame FrameLeaderboard2;
    [ManialinkControl] public required CMlFrame FrameLeaderboard3;
    [ManialinkControl] public required CMlFrame FrameLeaderboard4;
    [ManialinkControl] public required CMlFrame FrameLeaderboard5;
    [ManialinkControl] public required CMlFrame FrameLeaderboard6;
    [ManialinkControl] public required CMlFrame FrameLeaderboard7;
    [ManialinkControl] public required CMlFrame FrameLeaderboard8;
    [ManialinkControl] public required CMlFrame FrameLeaderboard9;
    [ManialinkControl] public required CMlFrame FrameLeaderboard10;
    [ManialinkControl] public required CMlLabel LabelYourName;
    [ManialinkControl] public required CMlLabel LabelLadderPoints;
    [ManialinkControl] public required CMlLabel LabelLadderZone;
    [ManialinkControl] public required CMlQuad QuadEchelonPercent;
    [ManialinkControl] public required CMlQuad QuadEchelonCurrent;
    [ManialinkControl] public required CMlQuad QuadEchelonNext;
    [ManialinkControl] public required CMlLabel LabelEchelonCurrent;
    [ManialinkControl] public required CMlLabel LabelEchelonNext;
    [ManialinkControl] public required CMlFrame FrameMyCar;
    [ManialinkControl] public required CMlFrame FrameDifficulty;
    [ManialinkControl] public required CMlFrame FrameQuality;
    [ManialinkControl] public required CMlQuad QuadMyCar;
    [ManialinkControl] public required CMlLabel LabelMyCar;
    [ManialinkControl] public required CMlQuad QuadStar;
    [ManialinkControl] public required CMlFrame FrameTooltip;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlFrame FrameScoreboard;

    public required ImmutableArray<CMlFrame> Leaderboards;
    public required ImmutableArray<string> LeaderboardCars;
    public required ImmutableArray<CMlFrame> RatingFrames;
    public required CMlLabel LabelDifficulty;
    public required CMlLabel LabelQuality;
    public required CMlQuad QuadDifficultyBlink;
    public required CMlQuad QuadQualityBlink;
    public required CMlQuad QuadDifficultyGlow;
    public required CMlQuad QuadQualityGlow;
    public required Dictionary<string, bool> ConnectedPlayers;
    public required Dictionary<string, float> PreviousUserLadderPoints;
    public CMlFrame? HeldRating;
    public CMlFrame? HeldLeaderboard;
    public CMlQuad? HeldScrollbar;
    public float HeldScrollbarMouseY;
    public bool HeldScrollbarMouseOut;
    public CHttpRequest? StarRequest;

    public float Difficulty;
    public float Quality;
    public string PrevCar = "";
    public bool PrevRatingEnabled;
    public int PrevRatingsUpdatedAt;
    public bool PrevScoreTableIsVisible;
    public int PrevUpdateAt;
    public int PrevPodiumStartTime;
    public int LocalPodiumStartTime = -1;
    public float PreviousLadderPoints;
    public int PreviousLadderRank;
    public int PreviousLadderTotal;
    public int PreviousNextEchelonPercent;
    public bool IsLadderPointsAnimating;
    public bool IsLadderPointsAnimationDone;

    [Netread] public bool RatingEnabled { get; }
    [Netread] public bool ShowIndividualLadderPointsDiffEnabled { get; }
    [Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    [Netread] public required Dictionary<string, SStar> Stars { get; set; }
    [Netread] public required int RatingsUpdatedAt { get; set; }
    [Netread(NetFor.UI)] public required IList<SFilteredRating> MyRatings { get; set; }
    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }
    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }
    [Netread] public string EnvimixWebAPI { get; set; }
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";
    [Netread] public int PodiumStartTime { get; }

    public ScoreboardTimeAttack()
    {
        MouseClick += Scoreboard_MouseClick;
        MouseOver += Scoreboard_MouseOver;
        MouseOut += Scoreboard_MouseOut;

        QuadStar.MouseOver += () =>
        {
            var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);

            if (envimixTurboUserIsAdmin.Get() && StarRequest is null)
            {
                if (HasStar())
                {
                    QuadStar.Opacity = 1;
                }
                else
                {
                    QuadStar.Opacity = 0.7f;
                }
            }

            var filterKey = ConstructRatingFilterKey();
            if (Stars.ContainsKey(filterKey))
            {
                UpdateTooltip(Stars[filterKey].Nickname);
            }
        };

        QuadStar.MouseOut += () =>
        {
            var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);

            if (envimixTurboUserIsAdmin.Get() && StarRequest is null)
            {
                if (HasStar())
                {
                    QuadStar.Opacity = 0.9f;
                }
                else
                {
                    QuadStar.Opacity = 0.1f;
                }
            }
        };

        QuadStar.MouseClick += ToggleStar;
    }

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

    string GetCar()
    {
        var car = Netread<string>.For(GetPlayer());
        return car.Get();
    }

    static string ConstructFilterKey(string car, int gravity)
    {
        return $"{car}_{gravity}_Time";
    }

    static string ConstructRatingFilterKey(SRatingFilter filter)
    {
        return ConstructFilterKey(filter.Car, filter.Gravity);
    }

    string ConstructRatingFilterKey()
    {
        var gravity = Netread<int>.For(GetPlayer());
        return ConstructFilterKey(GetCar(), gravity.Get());
    }

    bool HasStar()
    {
        return Stars.ContainsKey(ConstructRatingFilterKey());
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

    private void UpdateTooltip(string text)
    {
        var labelText = (FrameTooltip.GetFirstChild("LabelText") as CMlLabel)!;
        var quadTooltip = (FrameTooltip.GetFirstChild("QuadTooltip") as CMlQuad)!;

        labelText.Size.X = labelText.ComputeWidth(text);
        quadTooltip.Size.X = labelText.ComputeWidth(text) + 4;
        labelText.SetText(text);
        FrameTooltip.Show();
    }

    private void ToggleStar()
    {
        if (StarRequest is not null)
        {
            return;
        }

        var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);
        if (!envimixTurboUserIsAdmin.Get())
        {
            return;
        }

        var gravity = Netread<int>.For(GetPlayer());
        SRatingFilter filter = new()
        {
            Car = GetCar(),
            Gravity = gravity.Get(),
            Type = "Time"
        };
        SRatingStarRequest starRequest = new()
        {
            MapUid = Map.MapInfo.MapUid,
            Filter = filter
        };

        if (HasStar())
        {
            QuadStar.Opacity = 0.7f;
            StarRequest = Http.CreatePost($"{EnvimixWebAPI}/rate/unstar", starRequest.ToJson(), $"Authorization: Bearer {EnvimixTurboUserToken}\nContent-Type: application/json");
        }
        else
        {
            QuadStar.Opacity = 0.9f;
            StarRequest = Http.CreatePost($"{EnvimixWebAPI}/rate/star", starRequest.ToJson(), $"Authorization: Bearer {EnvimixTurboUserToken}\nContent-Type: application/json");
        }
    }

    private void Scoreboard_MouseOver(CMlControl control, string controlId)
    {
        var car = control.DataAttributeGet("car");
        if (car != "")
        {
            UpdateTooltip(car);
        }

        if (controlId == "QuadRecordsScrollbar")
        {
            AnimMgr.Add(control, "<quad opacity=\"0.9\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void Scoreboard_MouseOut(CMlControl control, string controlId)
    {
        FrameTooltip.Hide();

        if (controlId != "QuadRecordsScrollbar")
        {
            return;
        }

        if (HeldScrollbar == control)
        {
            HeldScrollbarMouseOut = true;
        }
        else
        {
            AnimMgr.Add(control, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void ClearPersonalRating(CMlFrame frame)
    {
        frame.GetFirstChild("LabelRateName").Hide();
        if (frame.ControlId == "FrameDifficulty") QuadDifficultyBlink.Hide();
        if (frame.ControlId == "FrameQuality") QuadQualityBlink.Hide();
    }

    private void Scoreboard_MouseClick(CMlControl control, string controlId)
    {
        if (controlId == "LabelPlayerName")
        {
            ShowProfile(control.DataAttributeGet("login"));
        }

        if (controlId == "QuadRecordsScrollbar")
        {
            HeldScrollbar = (control as CMlQuad)!;
            HeldLeaderboard = control.Parent.Parent;
            HeldScrollbarMouseY = MouseY - (float)HeldScrollbar.RelativePosition_V3.Y;
        }

        if (!RatingEnabled || (controlId != "QuadBox" && controlId != "QuadDraggable"))
        {
            return;
        }

        CMlFrame frame;
        if (controlId == "QuadBox")
        {
            frame = control.Parent.Parent;
            HeldRating = (frame.GetFirstChild("FrameDraggable") as CMlFrame)!;
            HeldRating.Show();
        }
        else
        {
            HeldRating = control.Parent;
            frame = HeldRating.Parent.Parent;
        }

        ClearPersonalRating(frame);
    }

    private void SetLeaderboardRating(CMlFrame leaderboard, string car)
    {
        var gaugeDifficulty = (leaderboard.GetFirstChild("GaugeDifficulty") as CMlGauge)!;
        var gaugeQuality = (leaderboard.GetFirstChild("GaugeQuality") as CMlGauge)!;
        var key = ConstructFilterKey(car, 0);

        if (!Ratings.ContainsKey(key))
        {
            gaugeDifficulty.Ratio = 0;
            gaugeQuality.Ratio = 0;
            return;
        }

        var rating = Ratings[key];
        gaugeDifficulty.Ratio = 0;
        gaugeQuality.Ratio = 0;
        if (rating.Difficulty >= 0)
        {
            gaugeDifficulty.Ratio = rating.Difficulty * 0.9f + 0.1f;
        }
        if (rating.Quality >= 0)
        {
            gaugeQuality.Ratio = rating.Quality * 0.9f + 0.1f;
        }
    }

    private void UpdateRatings()
    {
        var filterKey = ConstructRatingFilterKey();
        var gaugeDifficulty = (FrameDifficulty.GetFirstChild("GaugeRating") as CMlGauge)!;
        var gaugeQuality = (FrameQuality.GetFirstChild("GaugeRating") as CMlGauge)!;

        if (!Ratings.ContainsKey(filterKey))
        {
            gaugeDifficulty.Ratio = 0;
            gaugeQuality.Ratio = 0;
        }
        else
        {
            var rating = Ratings[filterKey];
            var difficultyRatio = 0f;
            var qualityRatio = 0f;
            if (rating.Difficulty >= 0)
            {
                difficultyRatio = rating.Difficulty * 0.9f + 0.1f;
            }
            if (rating.Quality >= 0)
            {
                qualityRatio = rating.Quality * 0.9f + 0.1f;
            }
            AnimMgr.Add(gaugeDifficulty, $"<gauge ratio=\"{difficultyRatio}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.Add(gaugeQuality, $"<gauge ratio=\"{qualityRatio}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);
        if (Stars.ContainsKey(filterKey))
        {
            QuadStar.Show();
            if (envimixTurboUserIsAdmin.Get()) QuadStar.Opacity = 0.9f;
        }
        else if (envimixTurboUserIsAdmin.Get())
        {
            QuadStar.Show();
            QuadStar.Opacity = 0.1f;
        }
        else
        {
            QuadStar.Hide();
        }
    }

    private void SetPersonalRating(CMlFrame frame, float value)
    {
        var draggable = frame.GetFirstChild("FrameDraggable")!;
        if (value < 0)
        {
            frame.GetFirstChild("LabelRateName").Show();
            if (frame.ControlId == "FrameDifficulty") QuadDifficultyBlink.Show();
            if (frame.ControlId == "FrameQuality") QuadQualityBlink.Show();
            draggable.Hide();
        }
        else
        {
            ClearPersonalRating(frame);
            draggable.RelativePosition_V3.X = value * 56 - 28;
            draggable.Show();
        }
    }

    private void UpdatePersonalRatings()
    {
        foreach (var rating in MyRatings)
        {
            if (ConstructRatingFilterKey(rating.Filter) == ConstructRatingFilterKey())
            {
                SetPersonalRating(FrameDifficulty, rating.Rating.Difficulty);
                SetPersonalRating(FrameQuality, rating.Rating.Quality);
                return;
            }
        }

        SetPersonalRating(FrameDifficulty, -1);
        SetPersonalRating(FrameQuality, -1);
    }

    private void SetRecordRow(CMlFrame row, string login, int time, int rank, int scoreIndex)
    {
        var labelRank = (row.GetFirstChild("LabelRank") as CMlLabel)!;
        var labelPlayerName = (row.GetFirstChild("LabelPlayerName") as CMlLabel)!;
        var labelScore = (row.GetFirstChild("LabelScore") as CMlLabel)!;
        var isConnected = ConnectedPlayers.ContainsKey(login) && ConnectedPlayers[login];

        if (scoreIndex >= 0)
        {
            labelPlayerName.SetText(Scores[scoreIndex].User.Name);
        }
        else
        {
            labelPlayerName.SetText(LocalUser.Name);
        }
        labelPlayerName.DataAttributeSet("login", login);

        if (time == 2147483647)
        {
            labelRank.SetText("--");
            labelScore.SetText("-:--.---");
        }
        else
        {
            labelRank.SetText(TextLib.FormatInteger(rank, 2));
            labelScore.SetText(TimeToTextWithMilli(time));
        }

        if (ShowIndividualLadderPointsDiffEnabled && LocalPodiumStartTime != -1 && Now - LocalPodiumStartTime >= 10000 && scoreIndex >= 0)
        {
            var ladderPointsDiff = 0f;
            if (PreviousUserLadderPoints.ContainsKey(login))
            {
                ladderPointsDiff = Scores[scoreIndex].User.LadderPoints - PreviousUserLadderPoints[login];
            }

            var diffText = TextLib.FormatReal(ladderPointsDiff, 1, _HideZeroes: false, _HideDot: false);
            if (ladderPointsDiff >= 0)
            {
                diffText = $"+{diffText}";
            }

            labelScore.SetText($"{diffText} LP");
            labelScore.TextColor = new Vec3(0.8f, 0.733f, 0.4f);
        }
        else
        {
            labelScore.TextColor = new Vec3(1, 1, 1);
        }

        if (isConnected)
        {
            labelRank.Opacity = 1;
            labelPlayerName.Opacity = 1;
            labelScore.Opacity = 1;
        }
        else
        {
            labelRank.Opacity = 0.3f;
            labelPlayerName.Opacity = 0.3f;
            labelScore.Opacity = 0.3f;
        }

        labelPlayerName.RelativePosition_V3.X = 12 + labelScore.ComputeWidth(labelScore.Value);
    }

    private void UpdateLeaderboard(CMlFrame leaderboard, string car)
    {
        leaderboard.Show();

        var quadCar = (leaderboard.GetFirstChild("QuadCar") as CMlQuad)!;
        var labelCar = (leaderboard.GetFirstChild("LabelCar") as CMlLabel)!;
        var carOpacity = 0.5f;
        if (DisplayedCars.Contains(car))
        {
            carOpacity = 1;
        }
        quadCar.ChangeImageUrl($"file://Media/Images/Cars/{car}.png");
        quadCar.DataAttributeSet("car", car);
        quadCar.Opacity = carOpacity;
        labelCar.SetText(car);
        labelCar.Opacity = carOpacity;
        SetLeaderboardRating(leaderboard, car);

        var outerRecords = (leaderboard.GetFirstChild("FrameOuterRecords") as CMlFrame)!;
        var records = (leaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;
        var yourRecord = (leaderboard.GetFirstChild("FrameYourRecord") as CMlFrame)!;
        var scrollbar = (leaderboard.GetFirstChild("QuadRecordsScrollbar") as CMlQuad)!;
        var scrollable = (leaderboard.GetFirstChild("QuadRecordsScrollable") as CMlQuad)!;
        var key = ConstructFilterKey(car, 0);

        Dictionary<string, int> playerTimes = new();
        Dictionary<string, int> scoreIndices = new();
        ImmutableArray<string> playerLogins = new();

        for (var i = 0; i < Scores.Count; i++)
        {
            var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(Scores[i]);
            var login = Scores[i].User.Login;
            playerTimes[login] = 2147483647;
            if (envimixBestRace.Get().ContainsKey(key))
            {
                playerTimes[login] = envimixBestRace.Get()[key].Time;
            }
            scoreIndices[login] = i;
        }

        playerTimes = playerTimes.Sort();
        foreach (var (login, time) in playerTimes)
        {
            playerLogins.Add(login);
        }

        Dictionary<string, int> ranks = new();
        var previousTime = -1;
        var currentRank = 0;
        for (var i = 0; i < playerLogins.Length; i++)
        {
            var login = playerLogins[i];
            var time = playerTimes[login];
            if (time == 2147483647)
            {
                ranks[login] = 0;
                continue;
            }

            if (time != previousTime) currentRank = i + 1;
            ranks[login] = currentRank;
            previousTime = time;
        }

        var visibleRows = records.Controls.Count;
        if (playerLogins.Length > visibleRows)
        {
            outerRecords.ScrollMax = new Vec2(0, (playerLogins.Length - visibleRows) * 5f);
            scrollbar.Size.Y = visibleRows * 1f / playerLogins.Length * 60;
            scrollbar.RelativePosition_V3.Y = -(outerRecords.ScrollOffset.Y / outerRecords.ScrollMax.Y) * (scrollable.Size.Y - scrollbar.Size.Y);
            scrollbar.Show();
        }
        else
        {
            outerRecords.ScrollOffset = new Vec2(0, 0);
            outerRecords.ScrollMax = new Vec2(0, 0);
            scrollbar.Hide();
        }

        for (var i = 0; i < records.Controls.Count; i++)
        {
            var row = (records.Controls[i] as CMlFrame)!;
            var index = i + MathLib.FloorInteger((float)outerRecords.ScrollOffset.Y / 5);
            if (index >= playerLogins.Length)
            {
                row.Hide();
                continue;
            }

            row.Show();
            SetRecordRow(row, playerLogins[index], playerTimes[playerLogins[index]], ranks[playerLogins[index]], scoreIndices[playerLogins[index]]);
        }

        records.RelativePosition_V3 = new Vec2(-outerRecords.ScrollOffset.X, -outerRecords.ScrollOffset.Y);

        if (InputPlayer is not null && InputPlayer.Score is not null)
        {
            var login = InputPlayer.User.Login;
            var time = 2147483647;
            var rank = 0;
            var scoreIndex = -1;
            if (playerTimes.ContainsKey(login))
            {
                time = playerTimes[login];
            }
            if (ranks.ContainsKey(login))
            {
                rank = ranks[login];
            }
            if (scoreIndices.ContainsKey(login))
            {
                scoreIndex = scoreIndices[login];
            }
            SetRecordRow(yourRecord, login, time, rank, scoreIndex);
            yourRecord.Show();
        }
        else
        {
            yourRecord.Hide();
        }
    }

    private void UpdateScoreboard()
    {
        LabelYourName.SetText(LocalUser.Name);

        foreach (var (login, isConnected) in ConnectedPlayers)
        {
            ConnectedPlayers[login] = false;
        }

        foreach (var player in Players)
        {
            ConnectedPlayers[player.User.Login] = true;
        }

        var car = GetCar();
        FrameMyCar.Visible = car != "" && LocalPodiumStartTime == -1;

        if (car != "")
        {
            QuadMyCar.ChangeImageUrl($"file://Media/Images/Cars/{car}.png");
            QuadMyCar.DataAttributeSet("car", car);
            LabelMyCar.SetText(car);
            QuadMyCar.Show();
            LabelMyCar.Show();
        }
        else
        {
            QuadMyCar.Hide();
            LabelMyCar.Hide();
        }

        for (var i = 0; i < Leaderboards.Length; i++)
        {
            UpdateLeaderboard(Leaderboards[i], LeaderboardCars[i]);
        }
    }

    private void SetEchelon()
    {
        var echelon = EchelonToInteger(LocalUser.Echelon);
        QuadEchelonCurrent.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{echelon}.dds");
        LabelEchelonCurrent.Value = echelon.ToString();

        if (echelon + 1 < 10)
        {
            QuadEchelonNext.ChangeImageUrl($"file://Media/Manialinks/Common/Echelons/echelon{echelon + 1}.dds");
            LabelEchelonNext.Value = (echelon + 1).ToString();
        }
        else
        {
            QuadEchelonNext.ChangeImageUrl("");
            LabelEchelonNext.Value = "";
        }
    }

    private void SetLadderDisplay(float points, int rank, int total, float nextEchelonPercent)
    {
        LabelLadderPoints.SetText(TextLib.FormatReal(points, 1, _HideZeroes: false, _HideDot: false));
        if (rank == -1)
        {
            LabelLadderZone.Value = "Not ranked";
        }
        else
        {
            LabelLadderZone.Value = $"{TextLib.GetTranslatedText(LocalUser.LadderZoneName)}: $ff0{rank}$aaa / {total}";
        }
        QuadEchelonPercent.Size.X = nextEchelonPercent / 100f * 50;
    }

    private void UpdateLadderPoints()
    {
        if (LocalPodiumStartTime == -1 || Now - LocalPodiumStartTime <= 5000 || IsLadderPointsAnimationDone)
        {
            return;
        }

        if (!IsLadderPointsAnimating)
        {
            IsLadderPointsAnimating = true;
            if (LocalUser.LadderPoints != PreviousLadderPoints)
            {
                for (var i = 0; i < 10; i++)
                {
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: i * 100);
                }
            }
        }

        var time = Now - LocalPodiumStartTime - 5000;
        var points = AnimLib.EaseOutQuad(time, PreviousLadderPoints, LocalUser.LadderPoints - PreviousLadderPoints, 1000);
        var rank = MathLib.NearestInteger(AnimLib.EaseOutQuad(time, PreviousLadderRank * 1f, LocalUser.LadderRank - PreviousLadderRank * 1f, 1000));
        var total = MathLib.NearestInteger(AnimLib.EaseOutQuad(time, PreviousLadderTotal * 1f, LocalUser.LadderTotal - PreviousLadderTotal * 1f, 1000));
        var percent = AnimLib.EaseOutQuad(time, PreviousNextEchelonPercent * 1f, LocalUser.NextEchelonPercent - PreviousNextEchelonPercent * 1f, 1000);
        SetLadderDisplay(points, rank, total, percent);

        if (time >= 1000)
        {
            PreviousLadderPoints = LocalUser.LadderPoints;
            PreviousLadderRank = LocalUser.LadderRank;
            PreviousLadderTotal = LocalUser.LadderTotal;
            PreviousNextEchelonPercent = LocalUser.NextEchelonPercent;
            IsLadderPointsAnimating = false;
            IsLadderPointsAnimationDone = true;
            SetEchelon();
        }
    }

    public void Main()
    {
        Difficulty = -1;
        Quality = -1;

        Leaderboards.Add(FrameLeaderboard0);
        Leaderboards.Add(FrameLeaderboard1);
        Leaderboards.Add(FrameLeaderboard2);
        Leaderboards.Add(FrameLeaderboard3);
        Leaderboards.Add(FrameLeaderboard4);
        Leaderboards.Add(FrameLeaderboard5);
        Leaderboards.Add(FrameLeaderboard6);
        Leaderboards.Add(FrameLeaderboard7);
        Leaderboards.Add(FrameLeaderboard8);
        Leaderboards.Add(FrameLeaderboard9);
        Leaderboards.Add(FrameLeaderboard10);

        LeaderboardCars.Add("CanyonCar");
        LeaderboardCars.Add("StadiumCar");
        LeaderboardCars.Add("ValleyCar");
        LeaderboardCars.Add("LagoonCar");
        LeaderboardCars.Add("TrafficCar");
        LeaderboardCars.Add("DesertCar");
        LeaderboardCars.Add("SnowCar");
        LeaderboardCars.Add("RallyCar");
        LeaderboardCars.Add("IslandCar");
        LeaderboardCars.Add("BayCar");
        LeaderboardCars.Add("CoastCar");

        LabelDifficulty = (FrameDifficulty.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelDifficulty.SetText("Difficulty");
        QuadDifficultyBlink = (FrameDifficulty.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadDifficultyGlow = (FrameDifficulty.GetFirstChild("QuadGlow") as CMlQuad)!;
        LabelQuality = (FrameQuality.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelQuality.SetText("Quality");
        QuadQualityBlink = (FrameQuality.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadQualityGlow = (FrameQuality.GetFirstChild("QuadGlow") as CMlQuad)!;
        RatingFrames.Add(FrameDifficulty);
        RatingFrames.Add(FrameQuality);

        Wait(() => GetPlayer() is not null);

        SetLadderDisplay(LocalUser.LadderPoints, LocalUser.LadderRank, LocalUser.LadderTotal, LocalUser.NextEchelonPercent * 1f);
        SetEchelon();
        PreviousLadderPoints = LocalUser.LadderPoints;
        PreviousLadderRank = LocalUser.LadderRank;
        PreviousLadderTotal = LocalUser.LadderTotal;
        PreviousNextEchelonPercent = LocalUser.NextEchelonPercent;

        foreach (var score in Scores)
        {
            PreviousUserLadderPoints[score.User.Login] = score.User.LadderPoints;
        }

        PrevCar = GetCar();
        UpdateRatings();
        UpdatePersonalRatings();
        UpdateScoreboard();
    }

    public void Loop()
    {
        ScoreTableIsVisible = PageIsVisible;
        FrameScoreboard.Visible = !IsInGameMenuDisplayed;

        if (PageIsVisible != PrevScoreTableIsVisible)
        {
            if (PageIsVisible && (ClientUI.ScoreTableVisibility == CUIConfig.EVisibility.ForcedVisible || UI.ScoreTableVisibility == CUIConfig.EVisibility.ForcedVisible))
            {
                QuadBlur.RelativeScale = 0;
                AnimMgr.Add(QuadBlur, "<quad scale=\"1\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                UpdateScoreboard();
            }
            else
            {
                QuadBlur.RelativeScale = 1;
            }
            PrevScoreTableIsVisible = PageIsVisible;
        }

        if (PodiumStartTime != PrevPodiumStartTime && LocalPodiumStartTime == -1 && UI.ScoreTableVisibility == CUIConfig.EVisibility.ForcedVisible)
        {
            LocalPodiumStartTime = Now;
            PrevPodiumStartTime = PodiumStartTime;
            IsLadderPointsAnimationDone = false;
        }

        if (LocalPodiumStartTime != -1 && UI.ScoreTableVisibility != CUIConfig.EVisibility.ForcedVisible)
        {
            foreach (var score in Scores)
            {
                PreviousUserLadderPoints[score.User.Login] = score.User.LadderPoints;
            }

            LocalPodiumStartTime = -1;
        }

        foreach (var score in Scores)
        {
            if (!PreviousUserLadderPoints.ContainsKey(score.User.Login))
            {
                PreviousUserLadderPoints[score.User.Login] = score.User.LadderPoints;
            }
        }

        UpdateLadderPoints();

        if (PageIsVisible && Now - PrevUpdateAt >= 250)
        {
            UpdateScoreboard();
            PrevUpdateAt = Now;
        }

        if (RatingEnabled != PrevRatingEnabled)
        {
            QuadDifficultyBlink.Visible = RatingEnabled;
            QuadQualityBlink.Visible = RatingEnabled;
            if (RatingEnabled)
            {
                QuadDifficultyGlow.Opacity = 0.25f;
                QuadQualityGlow.Opacity = 0.25f;
            }
            else
            {
                QuadDifficultyGlow.Opacity = 0.1f;
                QuadQualityGlow.Opacity = 0.1f;
            }

            if (RatingEnabled) UpdatePersonalRatings();
            foreach (var frame in RatingFrames)
            {
                var labelRateName = (frame.GetFirstChild("LabelRateName") as CMlLabel)!;
                if (RatingEnabled)
                {
                    labelRateName.SetText("Click to rate");
                }
                else
                {
                    labelRateName.SetText("Rating is currently disabled");
                }
            }

            UpdateRatings();
            PrevRatingEnabled = RatingEnabled;
        }

        if (RatingEnabled)
        {
            if (QuadDifficultyBlink.Visible) QuadDifficultyBlink.Opacity = (MathLib.Sin(Now / 100f) + 1) / 2f * 0.1f;
            if (QuadQualityBlink.Visible) QuadQualityBlink.Opacity = (MathLib.Sin(Now / 100f + 180) + 1) / 2f * 0.1f;

            if (HeldRating is not null)
            {
                var frame = HeldRating.Parent.Parent;
                if (MouseLeftButton)
                {
                    var visualValue = MathLib.Clamp(MouseX - (float)frame.RelativePosition_V3.X, -28, 28);
                    var realValue = (visualValue + 28) / 56;
                    HeldRating.RelativePosition_V3.X = visualValue;
                    (HeldRating.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = true;
                    if (frame.ControlId == "FrameDifficulty") Difficulty = realValue;
                    if (frame.ControlId == "FrameQuality") Quality = realValue;
                }
                else
                {
                    (HeldRating.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = false;
                    if (frame.ControlId == "FrameDifficulty") SendCustomEvent("Rate", new[] { "Difficulty", Difficulty.ToString() });
                    if (frame.ControlId == "FrameQuality") SendCustomEvent("Rate", new[] { "Quality", Quality.ToString() });
                    HeldRating = null;
                }
            }
        }

        if (RatingsUpdatedAt != PrevRatingsUpdatedAt)
        {
            UpdateRatings();
            UpdatePersonalRatings();
            PrevRatingsUpdatedAt = RatingsUpdatedAt;
        }

        if (GetCar() != PrevCar)
        {
            PrevCar = GetCar();
            UpdateRatings();
            UpdatePersonalRatings();
            UpdateScoreboard();
        }

        if (HeldScrollbar is not null && HeldLeaderboard is not null)
        {
            if (MouseLeftButton)
            {
                var outerRecords = (HeldLeaderboard.GetFirstChild("FrameOuterRecords") as CMlFrame)!;
                var scrollable = (HeldLeaderboard.GetFirstChild("QuadRecordsScrollable") as CMlQuad)!;
                var trackHeight = (float)scrollable.Size.Y;
                var newY = MathLib.Clamp(MouseY - HeldScrollbarMouseY, (float)HeldScrollbar.Size.Y - trackHeight, 0);
                var targetOffset = newY / ((float)HeldScrollbar.Size.Y - trackHeight) * (float)outerRecords.ScrollMax.Y;
                var steppedOffset = MathLib.NearestInteger(targetOffset / 5f) * 5f;
                steppedOffset = MathLib.Clamp(steppedOffset, 0f, (float)outerRecords.ScrollMax.Y);

                if (outerRecords.ScrollMax.Y > 0)
                {
                    HeldScrollbar.RelativePosition_V3.Y = -(steppedOffset / outerRecords.ScrollMax.Y) * (trackHeight - HeldScrollbar.Size.Y);
                }
                outerRecords.ScrollOffset.Y = steppedOffset;
            }
            else
            {
                if (HeldScrollbarMouseOut) AnimMgr.Add(HeldScrollbar, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                HeldScrollbar = null;
                HeldLeaderboard = null;
                HeldScrollbarMouseOut = false;
            }
        }

        if (StarRequest is not null && StarRequest.IsCompleted)
        {
            if (StarRequest.StatusCode == 200)
            {
                Log("Star/unstar request succeeded.");
                SendCustomEvent("Star", new[] { "" });
            }
            else
            {
                Log($"Star/unstar request failed with status code {StarRequest.StatusCode}.");
            }
            Http.Destroy(StarRequest);
            StarRequest = null;
        }

        if (FrameTooltip.Visible)
        {
            FrameTooltip.RelativePosition_V3 = new Vec2(MouseX, MouseY);
        }
    }
}
