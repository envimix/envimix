using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class ScoreboardTeamAttack : CTmMlScriptIngame, IContext
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

    [ManialinkControl] public required CMlFrame FrameOuterGlobalScores;
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
    [ManialinkControl] public required CMlQuad QuadStar;
    [ManialinkControl] public required CMlFrame FrameTooltip;
    [ManialinkControl] public required CMlQuad QuadScoreboardScrollable;
    [ManialinkControl] public required CMlQuad QuadScoreboardScrollbar;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlFrame FrameCars;
    [ManialinkControl] public required CMlLabel LabelLeaderboardCar;
    [ManialinkControl] public required CMlQuad QuadLeaderboardPrevCar;
    [ManialinkControl] public required CMlQuad QuadLeaderboardNextCar;
    [ManialinkControl] public required CMlFrame FrameOuterRecords;
    [ManialinkControl] public required CMlFrame FrameRecords;
    [ManialinkControl] public required CMlFrame FrameYourRecord;
    [ManialinkControl] public required CMlQuad QuadRecordsScrollable;
    [ManialinkControl] public required CMlQuad QuadRecordsScrollbar;
    [ManialinkControl] public required CMlFrame FrameScoreboard;

    public required ImmutableArray<CMlFrame> RatingFrames;
    public required CMlLabel LabelDifficulty;
    public required CMlLabel LabelQuality;
    public required CMlQuad QuadDifficultyBlink;
    public required CMlQuad QuadQualityBlink;
    public required CMlQuad QuadDifficultyGlow;
    public required CMlQuad QuadQualityGlow;
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
    public bool PrevRatingEnabled;
    public int PrevRatingsUpdatedAt;
    public string PrevCar;
    public float PrevScrollOffsetY;
    public bool HoldsScrollbar;
    public float HoldsScrollbarMouseY;
    public bool ScrollbarMouseOut;
    public float PrevRecordsScrollOffsetY;
    public bool HoldsRecordsScrollbar;
    public float HoldsRecordsScrollbarMouseY;
    public bool RecordsScrollbarMouseOut;
    public CHttpRequest? StarRequest;
    public bool PrevScoreTableIsVisible;
    public bool IsLadderPointsAnimating;
    public bool IsLadderPointsAnimationDone;
    public int LocalPodiumStartTime = -1;
    public float PreviousLadderPoints;
    public int PreviousLadderRank;
    public int PreviousLadderTotal;
    public int PreviousNextEchelonPercent;
    public int PrevPodiumStartTime;
    public bool PrevShowLadderPointsDiff;

    public Dictionary<string, bool> ConnectedPlayers;
    public Dictionary<string, int> PreviousPoints;
    public Dictionary<string, int> PreviousPointsTime;
    public Dictionary<string, bool> PreviousSpectators;
    public Dictionary<string, float> PreviousUserLadderPoints;

    [Netread] public bool RatingEnabled { get; }
    [Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    [Netread] public required Dictionary<string, SStar> Stars { get; set; }
    [Netread] public required int RatingsUpdatedAt { get; set; }
    [Netread(NetFor.UI)] public required IList<SFilteredRating> MyRatings { get; set; }
    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }
    [Netread] public ImmutableArray<string> DisplayedCars { get; set; }

    [Netread] public string EnvimixWebAPI { get; set; }
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";

    [Netread] public Dictionary<string, bool> Spectators { get; }
    [Netread] public int PodiumStartTime { get; }

    public ScoreboardTeamAttack()
    {
        RaceEvent += Scoreboard_RaceEvent;
        MouseClick += Scoreboard_MouseClick;
        MouseOver += Scoreboard_MouseOver;

        QuadScoreboardScrollbar.MouseOver += () =>
        {
            AnimMgr.Add(QuadScoreboardScrollbar, "<quad opacity=\"0.9\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadScoreboardScrollbar.MouseOut += () =>
        {
            if (HoldsScrollbar)
            {
                ScrollbarMouseOut = true;
            }
            else
            {
                AnimMgr.Add(QuadScoreboardScrollbar, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        };

        QuadScoreboardScrollbar.MouseClick += () =>
        {
            HoldsScrollbar = true;
            HoldsScrollbarMouseY = MouseY - (float)QuadScoreboardScrollbar.RelativePosition_V3.Y;
        };

        QuadRecordsScrollbar.MouseOver += () =>
        {
            AnimMgr.Add(QuadRecordsScrollbar, "<quad opacity=\"0.9\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        };

        QuadRecordsScrollbar.MouseOut += () =>
        {
            if (HoldsRecordsScrollbar)
            {
                RecordsScrollbarMouseOut = true;
            }
            else
            {
                AnimMgr.Add(QuadRecordsScrollbar, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        };

        QuadRecordsScrollbar.MouseClick += () =>
        {
            HoldsRecordsScrollbar = true;
            HoldsRecordsScrollbarMouseY = MouseY - (float)QuadRecordsScrollbar.RelativePosition_V3.Y;
        };

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
                var star = Stars[filterKey];
                UpdateTooltip(star.Nickname);
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

        QuadStar.MouseClick += () =>
        {
            if (StarRequest is null)
            {
                var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);
                if (envimixTurboUserIsAdmin.Get())
                {
                    var car = Netread<string>.For(GetPlayer());
                    var gravity = Netread<int>.For(GetPlayer());

                    SRatingFilter filter = new()
                    {
                        Car = car.Get(),
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
            }
        };

        MouseOut += (control, controlId) =>
        {
            FrameTooltip.Hide();
        };

        QuadLeaderboardNextCar.MouseClick += () =>
        {
            var currentIndex = DisplayedCars.IndexOf(LabelLeaderboardCar.Value);
            var nextIndex = (currentIndex + 1) % DisplayedCars.Length;
            var nextCar = DisplayedCars[nextIndex];
            LabelLeaderboardCar.Value = nextCar;
            UpdateRecords();
        };

        QuadLeaderboardPrevCar.MouseClick += () =>
        {
            var currentIndex = DisplayedCars.IndexOf(LabelLeaderboardCar.Value);
            var prevIndex = (currentIndex - 1 + DisplayedCars.Length) % DisplayedCars.Length;
            var prevCar = DisplayedCars[prevIndex];
            LabelLeaderboardCar.Value = prevCar;
            UpdateRecords();
        };
    }

    private int GetInterpolatedScore(int previousPoints, int targetPoints, int startTime)
    {
        if (Now - startTime >= 1000) return targetPoints;
        var t = (Now - startTime) * 1f;
        var c = (targetPoints - previousPoints) * 1f;
        t = t / 1000f;
        return MathLib.NearestInteger(-c * t * (t - 2) + previousPoints);
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
        var car = Netread<string>.For(GetPlayer());
        var gravity = Netread<int>.For(GetPlayer());

        return ConstructFilterKey(car.Get(), gravity.Get());
    }

    bool HasStar()
    {
        return Stars.ContainsKey(ConstructRatingFilterKey());
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

    static string ToNicerNumber(int num)
    {
        var numText = num.ToString();
        var newText = "";

        var numTextLength = TextLib.Length(numText);
        var numLengthReal = numTextLength / 3f;
        var numLength = MathLib.FloorInteger(numLengthReal);

        if (numLengthReal <= 1)
        {
            return numText;
        }

        for (var i = 0; i < numLength + 1; i++)
        {
            var length = MathLib.Min(3, numTextLength - i * 3);

            var numPart = TextLib.SubText(numText, numTextLength - 3 - i * 3, length);

            if (i == 0)
            {
                newText = numPart;
                continue;
            }

            newText = $"{numPart} {newText}";
        }

        return newText;
    }

    private void UpdatePlayer(CMlFrame frame, CTmScore score, int rank)
    {
        var isConnected = ConnectedPlayers.ContainsKey(score.User.Login) && ConnectedPlayers[score.User.Login];
        var isSpectator = Spectators.ContainsKey(score.User.Login) && Spectators[score.User.Login];

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
        quadEchelon.Visible = isConnected;

        var quadZone = (frame.GetFirstChild("QuadZone") as CMlQuad)!;
        quadZone.ChangeImageUrl($"file://ZoneFlags/Path/{score.User.ZonePath}");
        quadZone.Visible = isConnected;

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
        labelPlayerName.DataAttributeSet("login", score.User.Login);
        if (isConnected)
        {
            labelPlayerName.Opacity = 1f;
        }
        else
        {
            labelPlayerName.Opacity = 0.3f;
        }

        var labelScore = (frame.GetFirstChild("LabelScore") as CMlLabel)!;

        if (LocalPodiumStartTime != -1 && Now - LocalPodiumStartTime >= 10000)
        {
            float ladderPointsDiff;
            if (PreviousUserLadderPoints.ContainsKey(score.User.Login))
            {
                ladderPointsDiff = score.User.LadderPoints - PreviousUserLadderPoints[score.User.Login];
            }
            else
            {
                ladderPointsDiff = score.User.LadderPoints - score.User.LadderPoints;
            }

            var diffText = TextLib.FormatReal(ladderPointsDiff, 1, _HideZeroes: false, _HideDot: false);

            if (ladderPointsDiff >= 0)
            {
                diffText = $"+{diffText}";
            }

            labelScore.SetText($"{diffText} LP");
            labelScore.TextColor = new Vec3(0.8, 0.733, 0.4); // Same color as LabelLadderPoints
        }
        else
        {
            int currentVisualPoints = score.Points;

            if (PreviousPointsTime.ContainsKey(score.User.Login) && PreviousPoints.ContainsKey(score.User.Login))
            {
                currentVisualPoints = GetInterpolatedScore(PreviousPoints[score.User.Login], score.Points, PreviousPointsTime[score.User.Login]);
            }

            labelScore.SetText(ToNicerNumber(currentVisualPoints));
            labelScore.TextColor = new Vec3(1, 1, 1);
        }

        if (isConnected)
        {
            labelScore.Opacity = 1f;
        }
        else
        {
            labelScore.Opacity = 0.3f;
        }

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

            if (isConnected && DisplayedCars.Contains(car))
            {
                label.Opacity = 1f;
            }
            else
            {
                label.Opacity = 0.3f;
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

            quadCurrentCar.DataAttributeSet("car", PlayerCars[score.User.Login]);
        }

        quadCurrentCar.Visible = isConnected && !isSpectator;

        var quadStatus = (frame.GetFirstChild("QuadStatus") as CMlQuad)!;
        quadStatus.Visible = !isConnected;

        var quadSpectator = (frame.GetFirstChild("QuadSpectator") as CMlQuad)!;
        quadSpectator.Visible = isSpectator;
    }

    private void UpdateRatings()
    {
        var filterKey = ConstructRatingFilterKey();

        if (!Ratings.ContainsKey(filterKey))
        {
            (FrameDifficulty.GetFirstChild("GaugeRating") as CMlGauge)!.Ratio = 0;
            (FrameQuality.GetFirstChild("GaugeRating") as CMlGauge)!.Ratio = 0;
        }
        else
        {
            var rating = Ratings[filterKey];

            if (rating.Difficulty < 0)
            {
                AnimMgr.Add(FrameDifficulty.GetFirstChild("GaugeRating"), "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Add(FrameDifficulty.GetFirstChild("GaugeRating"), $"<gauge ratio=\"{rating.Difficulty * .9f + .1f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            if (rating.Quality < 0)
            {
                AnimMgr.Add(FrameQuality.GetFirstChild("GaugeRating"), "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Add(FrameQuality.GetFirstChild("GaugeRating"), $"<gauge ratio=\"{rating.Quality * .9f + .1f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        }

        var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);

        if (Stars.ContainsKey(filterKey))
        {
            var star = Stars[filterKey];
            QuadStar.Visible = true;

            if (envimixTurboUserIsAdmin.Get())
            {
                QuadStar.Opacity = 0.9f;
            }
        }
        else
        {
            if (envimixTurboUserIsAdmin.Get())
            {
                QuadStar.Visible = true;
                QuadStar.Opacity = 0.1f;
            }
            else
            {
                QuadStar.Visible = false;
            }
        }
    }

    private void UpdateRecords()
    {
        var car = LabelLeaderboardCar.Value;
        var key = $"{car}_0_Time";

        Dictionary<string, int> playerTimes = new();
        ImmutableArray<string> playerLogins = new();
        Dictionary<string, int> scoreIndices = new();

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

        playerTimes = playerTimes.Sort();

        foreach (var (login, time) in playerTimes)
        {
            playerLogins.Add(login);
        }

        var offset = 0;
        var previousTime = 0;
        Dictionary<string, int> calculatedRanks = new();

        // Precalculate all ranks so we have the local player's rank even if they aren't on the first page
        for (int i = 0; i < playerLogins.Length; i++)
        {
            var login = playerLogins[i];
            var time = playerTimes[login];

            if (time == 2147483647)
            {
                calculatedRanks[login] = 0;
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

            calculatedRanks[login] = i - offset + 1;
            previousTime = time;
        }

        var visibleRecordRows = FrameRecords.Controls.Count;

        if (playerLogins.Length > visibleRecordRows)
        {
            FrameOuterRecords.ScrollMax = new Vec2(0, (playerLogins.Length - visibleRecordRows) * 5f);
            QuadRecordsScrollbar.Size.Y = visibleRecordRows * 1f / playerLogins.Length * 60;
            QuadRecordsScrollbar.RelativePosition_V3.Y = -(FrameOuterRecords.ScrollOffset.Y / FrameOuterRecords.ScrollMax.Y) * (QuadRecordsScrollable.Size.Y - QuadRecordsScrollbar.Size.Y);
            QuadRecordsScrollbar.Show();
        }
        else
        {
            FrameOuterRecords.ScrollOffset = new Vec2(0, 0);
            FrameOuterRecords.ScrollMax = new Vec2(0, 0);
            QuadRecordsScrollbar.Hide();
        }

        for (int i = 0; i < FrameRecords.Controls.Count; i++)
        {
            var frame = (FrameRecords.Controls[i] as CMlFrame)!;

            var newI = i + MathLib.FloorInteger((float)FrameOuterRecords.ScrollOffset.Y / 5);

            if (playerLogins.Length <= newI)
            {
                frame.Hide();
                continue;
            }

            frame.Show();

            var login = playerLogins[newI];
            var time = playerTimes[login];
            var scoreIndex = scoreIndices[login];
            var isConnected = ConnectedPlayers.ContainsKey(login) && ConnectedPlayers[login];

            var quadTeam = (frame.GetFirstChild("QuadTeam") as CMlQuad)!;
            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelPlayerName = (frame.GetFirstChild("LabelPlayerName") as CMlLabel)!;
            var labelScore = (frame.GetFirstChild("LabelScore") as CMlLabel)!;

            labelPlayerName.Value = Scores[scoreIndex].User.Name;
            labelPlayerName.DataAttributeSet("login", Scores[scoreIndex].User.Login);

            quadTeam.BgColor = Teams[Scores[scoreIndex].TeamNum - 1].ColorPrimary;

            if (time == 2147483647)
            {
                labelRank.Value = "--";
                labelScore.Value = "-:--.---";
                labelPlayerName.RelativePosition_V3.X = 12 + labelScore.ComputeWidth(labelScore.Value);
                continue;
            }

            labelRank.Value = TextLib.FormatInteger(calculatedRanks[login], 2);
            labelScore.Value = TimeToTextWithMilli(time);
            labelPlayerName.RelativePosition_V3.X = 12 + labelScore.ComputeWidth(labelScore.Value);
        }

        if (InputPlayer is not null && InputPlayer.Score is not null)
        {
            var myLogin = InputPlayer.User.Login;
            var quadTeam = (FrameYourRecord.GetFirstChild("QuadTeam") as CMlQuad)!;
            var labelRank = (FrameYourRecord.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelPlayerName = (FrameYourRecord.GetFirstChild("LabelPlayerName") as CMlLabel)!;
            var labelScore = (FrameYourRecord.GetFirstChild("LabelScore") as CMlLabel)!;

            labelPlayerName.Value = InputPlayer.User.Name;
            labelPlayerName.DataAttributeSet("login", myLogin);

            quadTeam.BgColor = Teams[InputPlayer.Score.TeamNum - 1].ColorPrimary;

            if (playerTimes.ContainsKey(myLogin) && playerTimes[myLogin] != 2147483647)
            {
                labelRank.Value = TextLib.FormatInteger(calculatedRanks[myLogin], 2);
                labelScore.Value = TimeToTextWithMilli(playerTimes[myLogin]);
            }
            else
            {
                labelRank.Value = "--";
                labelScore.Value = "-:--.---";
            }

            labelPlayerName.RelativePosition_V3.X = 12 + labelScore.ComputeWidth(labelScore.Value);
        }
    }

    private void UpdateScoreboard()
    {
        LabelYourName.SetText(LocalUser.Name);

        if (PlayerCars.ContainsKey(LocalUser.Login))
        {
            var currentCarUrl = $"file://Media/Images/Cars/{PlayerCars[LocalUser.Login]}.png";

            if (QuadMyCar.ImageUrl != currentCarUrl)
            {
                QuadMyCar.ChangeImageUrl(currentCarUrl);
            }

            QuadMyCar.DataAttributeSet("car", PlayerCars[LocalUser.Login]);

            LabelMyCar.Value = PlayerCars[LocalUser.Login];

            QuadMyCar.Show();
            LabelMyCar.Show();
        }
        else
        {
            QuadMyCar.Hide();
            LabelMyCar.Hide();
        }

        foreach (var (connectedPlayer, isConnected) in ConnectedPlayers)
        {
            ConnectedPlayers[connectedPlayer] = false;
        }

        foreach (var player in Players)
        {
            ConnectedPlayers[player.User.Login] = true;
        }

        Ranks = new();
        var ranker = new Dictionary<string, Dictionary<string, int>>();

        foreach (var score in Scores)
        {
            var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(score);

            foreach (var car in DisplayedCars)
            {
                if (!ranker.ContainsKey(car))
                {
                    ranker[car] = new();
                }
                var key = $"{car}_0_Time";
                if (envimixBestRace.Get().ContainsKey(key))
                {
                    ranker[car][score.User.Login] = envimixBestRace.Get()[key].Time;
                }
            }
        }

        foreach (var control in FrameCars.Controls)
        {
            var quadCar = (control as CMlQuad)!;
            var car = quadCar.DataAttributeGet("car");
            if (!DisplayedCars.Contains(car))
            {
                quadCar.Opacity = 0.3f;
            }
            else
            {
                quadCar.Opacity = 1f;
            }
        }

        foreach (var (car, times) in ranker)
        {
            Ranks[car] = new();

            var index = 1;
            var currentRank = 1;
            var prevTime = -1;

            foreach (var (login, time) in times.Sort()) // Assumes times.Sort() sorts ascending
            {
                // Only update the assigned rank if the time is actually slower
                if (time != prevTime)
                {
                    currentRank = index;
                }

                Ranks[car][login] = currentRank;

                prevTime = time;
                index += 1;
            }
        }

        // Players with equal points share the same rank, like on the records list
        var pointsOffset = 0;
        var previousPoints = -1;
        Dictionary<string, int> calculatedScoreRanks = new();

        for (int i = 0; i < Scores.Count; i++)
        {
            if (Scores[i].Points == previousPoints)
            {
                pointsOffset += 1;
            }
            else
            {
                pointsOffset = 0;
            }

            calculatedScoreRanks[Scores[i].User.Login] = i - pointsOffset + 1;
            previousPoints = Scores[i].Points;
        }

        if (InputPlayer is not null)
        {
            if (InputPlayer.Score is not null)
            {
                var myOverallRank = 0;

                if (calculatedScoreRanks.ContainsKey(InputPlayer.User.Login))
                {
                    myOverallRank = calculatedScoreRanks[InputPlayer.User.Login];
                }

                UpdatePlayer(FrameYourScore, InputPlayer.Score, rank: myOverallRank);
            }
        }

        if (Scores.Count > 10)
        {
            FrameOuterGlobalScores.ScrollMax = new Vec2(0, (Scores.Count - 10) * 6f);
            QuadScoreboardScrollbar.Size.Y = 10f / Scores.Count * 60f;
            QuadScoreboardScrollbar.RelativePosition_V3.Y = -FrameOuterGlobalScores.ScrollOffset.Y * 10f / (Scores.Count - 10) * ((QuadScoreboardScrollable.Size.Y - QuadScoreboardScrollbar.Size.Y) / QuadScoreboardScrollable.Size.Y);
            QuadScoreboardScrollbar.Show();
        }
        else
        {
            FrameOuterGlobalScores.ScrollOffset = new Vec2(0, 0);
            FrameOuterGlobalScores.ScrollMax = new Vec2(0, 0);
            QuadScoreboardScrollbar.Hide();
        }

        for (int i = 0; i < FrameGlobalScores.Controls.Count; i++)
        {
            var frame = (FrameGlobalScores.Controls[i] as CMlFrame)!;

            var newI = i + MathLib.FloorInteger((float)FrameOuterGlobalScores.ScrollOffset.Y / 6);

            if (Scores.Count <= newI)
            {
                frame.Visible = false;
                continue;
            }

            UpdatePlayer(frame, Scores[newI], rank: calculatedScoreRanks[Scores[newI].User.Login]);

            frame.Visible = true;
        }

        UpdateRecords();
    }

    private void ClearPersonalRating(CMlFrame frame)
    {
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

    private void SetPersonalRating(CMlFrame frame, float value)
    {
        var draggable = frame.GetFirstChild("FrameDraggable")!;

        if (value < 0)
        {
            frame.GetFirstChild("LabelRateName").Show();

            if (frame.ControlId == "FrameDifficulty")
            {
                QuadDifficultyBlink.Show();
            }
            else if (frame.ControlId == "FrameQuality")
            {
                QuadQualityBlink.Show();
            }

            draggable.Hide();
        }
        else
        {
            ClearPersonalRating(frame);

            draggable.RelativePosition_V3.X = value * 56 - 28;
            draggable.Show();
        }
    }

    private void Scoreboard_RaceEvent(CTmRaceClientEvent e)
    {
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
                ClientUI.ScoreTableVisibility = CUIConfig.EVisibility.None;
                break;
        }
    }

    private void Scoreboard_MouseOver(CMlControl control, string controlId)
    {
        var car = control.DataAttributeGet("car");

        if (car != "")
        {
            UpdateTooltip(car);
        }
    }

    private void Scoreboard_MouseClick(CMlControl control, string controlId)
    {
        if (controlId == "LabelPlayerName")
        {
            ShowProfile(control.DataAttributeGet("login"));
        }

        if (RatingEnabled)
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

                ClearPersonalRating(frame);
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
    }

    private void SetEchelon()
    {
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
    }

    public void Main()
    {
        Difficulty = -1;
        Quality = -1;

        CurrentLadderPoints = -2;

        LabelDifficulty = (FrameDifficulty.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelDifficulty.SetText("Difficulty");
        QuadDifficultyBlink = (FrameDifficulty.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadDifficultyGlow = (FrameDifficulty.GetFirstChild("QuadGlow") as CMlQuad)!;
        LabelQuality = (FrameQuality.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelQuality.SetText("Quality");
        QuadQualityBlink = (FrameQuality.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadQualityGlow = (FrameDifficulty.GetFirstChild("QuadGlow") as CMlQuad)!;
        RatingFrames.Add(FrameDifficulty);
        RatingFrames.Add(FrameQuality);

        Wait(() => GetPlayer() is not null);

        LabelLadderPoints.SetText(TextLib.FormatReal(LocalUser.LadderPoints, 1, _HideZeroes: false, _HideDot: false));

        if (LocalUser.LadderRank == -1)
        {
            LabelLadderZone.Value = "Not ranked";
        }
        else
        {
            LabelLadderZone.Value = $"{TextLib.GetTranslatedText(LocalUser.LadderZoneName)}: $ff0{LocalUser.LadderRank}$aaa / {LocalUser.LadderTotal}";
        }

        QuadEchelonPercent.Size.X = LocalUser.NextEchelonPercent / 100f * 50;
        SetEchelon();

        PreviousLadderPoints = LocalUser.LadderPoints;
        PreviousLadderRank = LocalUser.LadderRank;
        PreviousLadderTotal = LocalUser.LadderTotal;
        PreviousNextEchelonPercent = LocalUser.NextEchelonPercent;

        foreach (var score in Scores)
        {
            PreviousUserLadderPoints[score.User.Login] = score.User.LadderPoints;
        }

        UpdateScoreboard();
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

        if (!IsLadderPointsAnimating)
        {
            return;
        }

        var time = Now - LocalPodiumStartTime - 5000;
        var animatedPoints = AnimLib.EaseOutQuad(time, PreviousLadderPoints, LocalUser.LadderPoints - PreviousLadderPoints, 1000);
        var animatedRank = MathLib.NearestInteger(AnimLib.EaseOutQuad(time, PreviousLadderRank * 1f, LocalUser.LadderRank - PreviousLadderRank * 1f, 1000));
        var animatedTotal = MathLib.NearestInteger(AnimLib.EaseOutQuad(time, PreviousLadderTotal * 1f, LocalUser.LadderTotal - PreviousLadderTotal * 1f, 1000));

        float animatedNextEchelonPercent;
        if (LocalUser.NextEchelonPercent < PreviousNextEchelonPercent)
        {
            animatedNextEchelonPercent = AnimLib.EaseOutQuad(time, PreviousNextEchelonPercent * 1f, LocalUser.NextEchelonPercent + PreviousNextEchelonPercent * 1f, 1000);
            if (animatedNextEchelonPercent > 100f)
            {
                animatedNextEchelonPercent -= 100f;
            }
        }
        else
        {
            animatedNextEchelonPercent = AnimLib.EaseOutQuad(time, PreviousNextEchelonPercent * 1f, LocalUser.NextEchelonPercent - PreviousNextEchelonPercent * 1f, 1000);
        }

        LabelLadderPoints.SetText(TextLib.FormatReal(animatedPoints, 1, _HideZeroes: false, _HideDot: false));

        if (animatedRank == -1)
        {
            LabelLadderZone.Value = "Not ranked";
        }
        else
        {
            LabelLadderZone.Value = $"{TextLib.GetTranslatedText(LocalUser.LadderZoneName)}: $ff0{animatedRank}$aaa / {animatedTotal}";
        }

        QuadEchelonPercent.Size.X = animatedNextEchelonPercent / 100f * 50;

        if (Now - LocalPodiumStartTime - 5000 >= 1000)
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

    private bool DetectChange()
    {
        bool changed = false;

        var showLadderPointsDiff = LocalPodiumStartTime != -1 && Now - LocalPodiumStartTime >= 10000;
        if (showLadderPointsDiff != PrevShowLadderPointsDiff)
        {
            PrevShowLadderPointsDiff = showLadderPointsDiff;
            changed = true;
        }

        if (InputPlayer is not null && InputPlayer.User.LadderPoints != CurrentLadderPoints)
        {
            CurrentLadderPoints = InputPlayer.User.LadderPoints;
            changed = true;
        }

        if (FrameOuterGlobalScores.ScrollOffset.Y != PrevScrollOffsetY)
        {
            PrevScrollOffsetY = (float)FrameOuterGlobalScores.ScrollOffset.Y;
            changed = true;
        }

        if (FrameOuterRecords.ScrollOffset.Y != PrevRecordsScrollOffsetY)
        {
            PrevRecordsScrollOffsetY = (float)FrameOuterRecords.ScrollOffset.Y;
            changed = true;
        }

        foreach (var score in Scores)
        {
            if (!PlayerPoints.ContainsKey(score.User.Login) || PlayerPoints[score.User.Login] != score.Points)
            {
                if (PlayerPoints.ContainsKey(score.User.Login))
                {
                    PreviousPoints[score.User.Login] = PlayerPoints[score.User.Login];
                    PreviousPointsTime[score.User.Login] = Now;
                }
                else
                {
                    PreviousPoints[score.User.Login] = score.Points;
                    PreviousPointsTime[score.User.Login] = 0; // Don't animate when first discovered
                }

                PlayerPoints[score.User.Login] = score.Points;
                changed = true;
            }

            if (!PlayerTeams.ContainsKey(score.User.Login) || PlayerTeams[score.User.Login] != score.TeamNum)
            {
                PlayerTeams[score.User.Login] = score.TeamNum;
                changed = true;
            }

            if (!PlayerEchelons.ContainsKey(score.User.Login) || PlayerEchelons[score.User.Login] != score.User.Echelon)
            {
                PlayerEchelons[score.User.Login] = score.User.Echelon;
                changed = true;
            }

            var envimixRecordUpdated = Netread<int>.For(score);
            if (!LastUpdated.ContainsKey(score.User.Login) || LastUpdated[score.User.Login] != envimixRecordUpdated.Get())
            {
                LastUpdated[score.User.Login] = envimixRecordUpdated.Get();
                changed = true;
            }
        }

        foreach (var player in Players)
        {
            var car = Netread<string>.For(player);
            if (!PlayerCars.ContainsKey(player.User.Login) || PlayerCars[player.User.Login] != car.Get())
            {
                PlayerCars[player.User.Login] = car.Get();
                changed = true;
            }

            if (!ConnectedPlayers.ContainsKey(player.User.Login) || !ConnectedPlayers[player.User.Login])
            {
                ConnectedPlayers[player.User.Login] = true;
                changed = true;
            }
        }

        foreach (var (login, isConnected) in ConnectedPlayers)
        {
            if (!isConnected)
            {
                continue;
            }

            var isStillHere = false;

            // Search the Players array for this login
            foreach (var player in Players)
            {
                if (player.User.Login == login)
                {
                    isStillHere = true;
                    break;
                }
            }

            // If we didn't find them in the Players array, they left
            if (!isStillHere)
            {
                ConnectedPlayers[login] = false;
                changed = true;
            }
        }

        // check for spectator changes
        if (PreviousSpectators.Count != Spectators.Count)
        {
            changed = true;
        }
        else
        {
            foreach (var (login, isSpectator) in Spectators)
            {
                if (!PreviousSpectators.ContainsKey(login) || PreviousSpectators[login] != isSpectator)
                {
                    changed = true;
                    break;
                }
            }
        }

        if (changed)
        {
            foreach (var (login, isSpectator) in Spectators)
            {
                PreviousSpectators[login] = isSpectator;
            }
        }

        return changed;
    }

    private void UpdatePersonalRatings()
    {
        var ratingsUpdated = false;

        foreach (var r in MyRatings)
        {
            if (ConstructRatingFilterKey(r.Filter) == ConstructRatingFilterKey())
            {
                SetPersonalRating(FrameDifficulty, r.Rating.Difficulty);
                SetPersonalRating(FrameQuality, r.Rating.Quality);

                ratingsUpdated = true;
                break;
            }
        }

        if (ratingsUpdated)
        {
            return;
        }

        SetPersonalRating(FrameDifficulty, -1);
        SetPersonalRating(FrameQuality, -1);
    }

    public void Loop()
    {
        ScoreTableIsVisible = PageIsVisible;

        FrameScoreboard.Visible = !IsInGameMenuDisplayed;

        if (PageIsVisible != PrevScoreTableIsVisible)
        {
            if (PageIsVisible)
            {
                if (ClientUI.ScoreTableVisibility == CUIConfig.EVisibility.ForcedVisible || UI.ScoreTableVisibility == CUIConfig.EVisibility.ForcedVisible)
                {
                    QuadBlur.RelativeScale = 0;
                    AnimMgr.Add(QuadBlur, "<quad scale=\"1\" />", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                else
                {
                    QuadBlur.RelativeScale = 1;
                }
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
            LocalPodiumStartTime = -1;
        }

        UpdateLadderPoints();

        if (DetectChange())
        {
            UpdateScoreboard();
        }

        // While the ladder-points diff is shown, this must not overwrite LabelScore back to the points number
        var showLadderPointsDiff = LocalPodiumStartTime != -1 && Now - LocalPodiumStartTime >= 10000;

        // Point animation start
        if (PageIsVisible && !showLadderPointsDiff)
        {
            for (int i = 0; i < FrameGlobalScores.Controls.Count; i++)
            {
                var frame = (FrameGlobalScores.Controls[i] as CMlFrame)!;
                if (!frame.Visible) continue;

                var newI = i + MathLib.FloorInteger((float)FrameOuterGlobalScores.ScrollOffset.Y / 6);
                if (Scores.Count <= newI) continue;

                if (PreviousPointsTime.ContainsKey(Scores[newI].User.Login) && PreviousPoints.ContainsKey(Scores[newI].User.Login))
                {
                    var labelScore = (frame.GetFirstChild("LabelScore") as CMlLabel)!;
                    int visualPoints = GetInterpolatedScore(PreviousPoints[Scores[newI].User.Login], Scores[newI].Points, PreviousPointsTime[Scores[newI].User.Login]);
                    string targetText = ToNicerNumber(visualPoints);

                    if (labelScore.Value != targetText)
                    {
                        labelScore.SetText(targetText);
                    }
                }
            }

            if (InputPlayer is not null && InputPlayer.Score is not null)
            {
                string login = InputPlayer.User.Login;
                if (PreviousPointsTime.ContainsKey(login) && PreviousPoints.ContainsKey(login))
                {
                    var labelScore = (FrameYourScore.GetFirstChild("LabelScore") as CMlLabel)!;
                    int visualPoints = GetInterpolatedScore(PreviousPoints[login], InputPlayer.Score.Points, PreviousPointsTime[login]);
                    string targetText = ToNicerNumber(visualPoints);

                    if (labelScore.Value != targetText)
                    {
                        labelScore.SetText(targetText);
                    }
                }
            }
        }
        // Point animation end

        // Score label shrinks to 0 right as it swaps to the ladder-points diff at 10s, then grows back in
        var scoreLabelScale = 1f;

        if (LocalPodiumStartTime != -1)
        {
            var elapsed = Now - LocalPodiumStartTime;

            if (elapsed >= 9800 && elapsed < 10000)
            {
                scoreLabelScale = AnimLib.EaseOutQuad(elapsed - 9800, 1f, -1f, 200);
            }
            else if (elapsed >= 10000 && elapsed < 10200)
            {
                scoreLabelScale = AnimLib.EaseOutQuad(elapsed - 10000, 0f, 0.8f, 200);
            }
            else if (elapsed >= 10200)
            {
                scoreLabelScale = 0.8f;
            }
        }

        if (PageIsVisible)
        {
            for (int i = 0; i < FrameGlobalScores.Controls.Count; i++)
            {
                var frame = (FrameGlobalScores.Controls[i] as CMlFrame)!;
                if (!frame.Visible) continue;

                (frame.GetFirstChild("LabelScore") as CMlLabel)!.RelativeScale = scoreLabelScale;
            }

            (FrameYourScore.GetFirstChild("LabelScore") as CMlLabel)!.RelativeScale = scoreLabelScale;
        }

        if (RatingEnabled != PrevRatingEnabled)
        {
            QuadDifficultyBlink.Visible = RatingEnabled;
            QuadQualityBlink.Visible = RatingEnabled;

            if (RatingEnabled)
            {
                UpdatePersonalRatings();

                QuadDifficultyGlow.Opacity = 0.25f;
                QuadQualityGlow.Opacity = 0.25f;
            }
            else
            {
                QuadDifficultyGlow.Opacity = 0.1f;
                QuadQualityGlow.Opacity = 0.1f;
            }

            UpdateRatings();

            foreach (var frame in RatingFrames)
            {
                if (RatingEnabled)
                {
                    (frame.GetFirstChild("LabelRateName") as CMlLabel)!.SetText("Click to rate");
                }
                else
                {
                    (frame.GetFirstChild("LabelRateName") as CMlLabel)!.SetText("Rating is currently disabled");
                }
            }

            PrevRatingEnabled = RatingEnabled;
        }

        if (RatingEnabled)
        {
            if (QuadDifficultyBlink.Visible)
            {
                QuadDifficultyBlink.Opacity = (MathLib.Sin(Now / 100f) + 1) / 2f * .1f;
            }

            if (QuadQualityBlink.Visible)
            {
                QuadQualityBlink.Opacity = (MathLib.Sin(Now / 100f + 180) + 1) / 2f * .1f;
            }

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

                    //var gauge = (frame.GetFirstChild("GaugeRating") as CMlGauge)!;
                    //gauge.SetRatio(realValue);
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

        if (RatingsUpdatedAt != PrevRatingsUpdatedAt)
        {
            UpdateRatings();

            PrevRatingsUpdatedAt = RatingsUpdatedAt;
        }

        if (GetCar() != PrevCar)
        {
            UpdateRatings();
            UpdatePersonalRatings();

            LabelLeaderboardCar.Value = GetCar();
            UpdateRecords();

            PrevCar = GetCar();
        }

        if (HoldsScrollbar && MouseLeftButton)
        {
            var trackHeight = (float)QuadScoreboardScrollable.Size.Y;
            var newY = MathLib.Clamp(MouseY - HoldsScrollbarMouseY, (float)QuadScoreboardScrollbar.Size.Y - trackHeight, 0);

            var targetScrollOffset = newY / ((float)QuadScoreboardScrollbar.Size.Y - trackHeight) * (float)FrameOuterGlobalScores.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger(targetScrollOffset / 6f);
            var steppedScrollOffset = stepIndex * 6f;

            steppedScrollOffset = MathLib.Clamp(steppedScrollOffset, 0f, (float)FrameOuterGlobalScores.ScrollMax.Y);

            if (FrameOuterGlobalScores.ScrollMax.Y > 0)
            {
                QuadScoreboardScrollbar.RelativePosition_V3.Y = -(steppedScrollOffset / FrameOuterGlobalScores.ScrollMax.Y) * (trackHeight - QuadScoreboardScrollbar.Size.Y);
            }

            FrameOuterGlobalScores.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldsScrollbar)
        {
            if (ScrollbarMouseOut)
            {
                AnimMgr.Add(QuadScoreboardScrollbar, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                ScrollbarMouseOut = false;
            }

            HoldsScrollbar = false;
        }

        FrameGlobalScores.RelativePosition_V3 = new Vec2(-FrameOuterGlobalScores.ScrollOffset.X, -FrameOuterGlobalScores.ScrollOffset.Y);

        if (HoldsRecordsScrollbar && MouseLeftButton)
        {
            var trackHeight = (float)QuadRecordsScrollable.Size.Y;
            var newY = MathLib.Clamp(MouseY - HoldsRecordsScrollbarMouseY, (float)QuadRecordsScrollbar.Size.Y - trackHeight, 0);

            var targetScrollOffset = newY / ((float)QuadRecordsScrollbar.Size.Y - trackHeight) * (float)FrameOuterRecords.ScrollMax.Y;
            var stepIndex = MathLib.NearestInteger(targetScrollOffset / 5f);
            var steppedScrollOffset = stepIndex * 5f;

            steppedScrollOffset = MathLib.Clamp(steppedScrollOffset, 0f, (float)FrameOuterRecords.ScrollMax.Y);

            if (FrameOuterRecords.ScrollMax.Y > 0)
            {
                QuadRecordsScrollbar.RelativePosition_V3.Y = -(steppedScrollOffset / FrameOuterRecords.ScrollMax.Y) * (trackHeight - QuadRecordsScrollbar.Size.Y);
            }

            FrameOuterRecords.ScrollOffset.Y = steppedScrollOffset;
        }
        else if (HoldsRecordsScrollbar)
        {
            if (RecordsScrollbarMouseOut)
            {
                AnimMgr.Add(QuadRecordsScrollbar, "<quad opacity=\"0.75\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                RecordsScrollbarMouseOut = false;
            }

            HoldsRecordsScrollbar = false;
        }

        FrameRecords.RelativePosition_V3 = new Vec2(-FrameOuterRecords.ScrollOffset.X, -FrameOuterRecords.ScrollOffset.Y);

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
