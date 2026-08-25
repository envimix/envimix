namespace Envimix.Media.Manialinks.Universe2;

public class Score : CTmMlScriptIngame, IContext
{
    public bool PreviousIsVisible;
    public int VisibleTime = -1;

    [ManialinkControl] public required CMlFrame FrameScore;
    [ManialinkControl] public required CMlFrame FrameInnerScore;
    [ManialinkControl] public required CMlLabel LabelBestTime;
    [ManialinkControl] public required CMlLabel LabelLastTime;
    [ManialinkControl] public required CMlLabel LabelSessionTime;
    [ManialinkControl] public required CMlLabel LabelTotalTime;

    [Netread(NetFor.Teams0)] public int FinishedAt { get; set; }

    public string SessionStartedAt = "";
    public int PrevGameTime;
    public string PrevCar;
    public int TotalTimeAtStart;
    public bool TimerPaused;
    public int SessionTimeAtPause;
    public int LastInputActiveAt = -1;
    public bool EnqueueAttempt;

    public Score()
    {
        PluginCustomEvent += (eventName, eventParams) =>
        {
            switch (eventName)
            {
                case "MenuOpen":
                    MenuOpen = eventParams.Length > 0 && eventParams[0] == "True";
                    break;
            }
        };

        RaceEvent += (e) =>
        {
            if (e.Type == CTmRaceClientEvent.EType.WayPoint && e.IsEndRace)
            {
                // no input presses within 5 seconds do good enough job
                // PauseTimer();

                IncrementFinish();
            }

            if (e.Type == CTmRaceClientEvent.EType.Respawn)
            {
                UnpauseTimer();
                EnqueueAttempt = true;
            }
        };
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public bool MenuOpen;

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    bool IsVisible()
    {
        if (IsExplore())
        {
            return !MenuOpen;
        }

        return !IsInGameMenuDisplayed && FinishedAt == -1 && GUIPlayer is not null;
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private Dictionary<string, int> GetLegacyFinishes()
    {
        var persistent_EnvimixFinishes = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixFinishes.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            return persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid];
        }

        return new();
    }

    private Dictionary<string, int> GetLegacyAttempts()
    {
        var persistent_EnvimixAttempts = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixAttempts.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            return persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid];
        }

        return new();
    }

    private void ResetLegacyFinishes()
    {
        var persistent_EnvimixFinishes = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixFinishes.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixFinishes.Get().Remove(Map.MapInfo.MapUid);
        }
    }

    private void ResetLegacyAttempts()
    {
        var persistent_EnvimixAttempts = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixAttempts.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixAttempts.Get().Remove(Map.MapInfo.MapUid);
        }
    }

    private Dictionary<string, int> GetLegacyTotalTime()
    {
        var persistent_EnvimixTotalTime = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixTotalTime.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            return persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid];
        }

        return new();
    }

    private Dictionary<string, string> GetLegacyTotalTimeTrackingAt()
    {
        var persistent_EnvimixTotalTimeTrackingAt = Persistent<Dictionary<string, Dictionary<string, string>>>.For(LocalUser);
        if (persistent_EnvimixTotalTimeTrackingAt.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            return persistent_EnvimixTotalTimeTrackingAt.Get()[Map.MapInfo.MapUid];
        }

        return new();
    }

    private void ResetLegacyTotalTime()
    {
        var persistent_EnvimixTotalTime = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (persistent_EnvimixTotalTime.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixTotalTime.Get().Remove(Map.MapInfo.MapUid);
        }
    }

    private void ResetLegacyTotalTimeTrackingAt()
    {
        var persistent_EnvimixTotalTimeTrackingAt = Persistent<Dictionary<string, Dictionary<string, string>>>.For(LocalUser);
        if (persistent_EnvimixTotalTimeTrackingAt.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixTotalTimeTrackingAt.Get().Remove(Map.MapInfo.MapUid);
        }
    }

    private void MigratePersistentValues()
    {
        var persistent_EnvimixFinishes = Persistent<Dictionary<string, int>>.For(Map);
        if (persistent_EnvimixFinishes.Get().Count == 0)
        {
            persistent_EnvimixFinishes.Set(GetLegacyFinishes());
            ResetLegacyFinishes();
        }

        var persistent_EnvimixAttempts = Persistent<Dictionary<string, int>>.For(Map);
        if (persistent_EnvimixAttempts.Get().Count == 0)
        {
            persistent_EnvimixAttempts.Set(GetLegacyAttempts());
            ResetLegacyAttempts();
        }

        var persistent_EnvimixTotalTime = Persistent<Dictionary<string, int>>.For(Map);
        if (persistent_EnvimixTotalTime.Get().Count == 0)
        {
            persistent_EnvimixTotalTime.Set(GetLegacyTotalTime());
            ResetLegacyTotalTime();
        }

        var persistent_EnvimixTotalTimeTrackingAt = Persistent<Dictionary<string, string>>.For(Map);
        if (persistent_EnvimixTotalTimeTrackingAt.Get().Count == 0)
        {
            persistent_EnvimixTotalTimeTrackingAt.Set(GetLegacyTotalTimeTrackingAt());
            ResetLegacyTotalTimeTrackingAt();
        }
    }

    private void IncrementFinish()
    {
        if (InputPlayer is null)
        {
            return;
        }

        var car = Netread<string>.For(InputPlayer);
        var persistent_EnvimixFinishes = Persistent<Dictionary<string, int>>.For(Map);
        if (!persistent_EnvimixFinishes.Get().ContainsKey(car.Get()))
        {
            persistent_EnvimixFinishes.Get()[car.Get()] = 0;
        }
        persistent_EnvimixFinishes.Get()[car.Get()] = persistent_EnvimixFinishes.Get()[car.Get()] + 1;
    }

    private void IncrementAttempt()
    {
        if (InputPlayer is null)
        {
            return;
        }

        var car = Netread<string>.For(InputPlayer);
        var persistent_EnvimixAttempts = Persistent<Dictionary<string, int>>.For(Map);
        if (!persistent_EnvimixAttempts.Get().ContainsKey(car.Get()))
        {
            persistent_EnvimixAttempts.Get()[car.Get()] = 0;
        }
        persistent_EnvimixAttempts.Get()[car.Get()] = persistent_EnvimixAttempts.Get()[car.Get()] + 1;
    }

    private void PauseTimer()
    {
        if (TimerPaused)
        {
            return;
        }

        if (SessionStartedAt != "")
        {
            var delta = TimeLib.GetDelta((GameTime / 1000).ToString(), SessionStartedAt);

            SessionTimeAtPause += delta;
            TotalTimeAtStart += delta;
            SessionStartedAt = "";

            var car = Netread<string>.For(GetPlayer());
            var persistent_EnvimixTotalTime = Persistent<Dictionary<string, int>>.For(Map);
            persistent_EnvimixTotalTime.Get()[car.Get()] = TotalTimeAtStart;
        }

        TimerPaused = true;
    }

    private void UnpauseTimer()
    {
        TimerPaused = false;
    }

    public void Main()
    {
        FrameScore.Visible = IsVisible();
        PreviousIsVisible = IsVisible();

        VisibleTime = -1;

        Wait(() => GetPlayer() is not null);
        MigratePersistentValues();
    }

    public void Loop()
    {
        var car = Netread<string>.For(GetPlayer());
        if (car.Get() != PrevCar)
        {
            var persistent_EnvimixTotalTime = Persistent<Dictionary<string, int>>.For(Map);
            if (persistent_EnvimixTotalTime.Get().ContainsKey(car.Get()))
            {
                TotalTimeAtStart = persistent_EnvimixTotalTime.Get()[car.Get()];
            }
            else
            {
                TotalTimeAtStart = 0;
            }
            SendCustomEvent("SessionTime", new[] { "0" });
            SendCustomEvent("TotalTime", new[] { TotalTimeAtStart.ToString() });

            SessionStartedAt = "";
            SessionTimeAtPause = 0;

            if (InputPlayer is not null)
            {
                var persistent_EnvimixTotalTimeTrackingAt = Persistent<Dictionary<string, string>>.For(Map);
                if (!persistent_EnvimixTotalTimeTrackingAt.Get().ContainsKey(car.Get()))
                {
                    persistent_EnvimixTotalTimeTrackingAt.Get()[car.Get()] = TimeLib.GetCurrent();
                }
            }

            PrevCar = car.Get();
        }

        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                var frame = (FrameScore.Controls[0] as CMlFrame)!;
                frame.Controls[0].Size.X = 0;
                frame.Controls[1].Size.X = 0;

                AnimMgr.Add(frame.Controls[0], "<quad size=\"0 22\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[1], "<quad size=\"0 22\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[0], "<quad size=\"42.5 22\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[1], "<quad size=\"42.5 22\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                VisibleTime = Now;
            }

            PreviousIsVisible = IsVisible();
        }

        if (VisibleTime == -1)
        {
            FrameInnerScore.ClipWindowSize.X = 40;
        }
        else
        {
            FrameInnerScore.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime - 200, 0, 40, 300);
        }

        if (GetPlayer().Score is null)
        {
            LabelBestTime.Value = "-:--.---";
            LabelLastTime.Value = "-:--.---";
        }
        else
        {
            if (GetPlayer().Score.BestRace.Time < 0)
            {
                LabelBestTime.Value = "-:--.---";
            }
            else
            {
                LabelBestTime.Value = TimeToTextWithMilli(GetPlayer().Score.BestRace.Time);
            }

            if (GetPlayer().Score.PrevRace.Time < 0)
            {
                LabelLastTime.Value = "-:--.---";
            }
            else
            {
                LabelLastTime.Value = TimeToTextWithMilli(GetPlayer().Score.PrevRace.Time);
            }
        }

        FrameScore.Visible = IsVisible();

        if (InputPlayer != GUIPlayer)
        {
            // update every half second
            if ((GameTime / 500) != (PrevGameTime / 500))
            {
                var sessionTime = Netread<int>.For(GetPlayer());
                var totalTime = Netread<int>.For(GetPlayer());

                FormatDelta(LabelSessionTime, sessionTime.Get());
                FormatDelta(LabelTotalTime, totalTime.Get());
            }
        }
        else if (InputPlayer is not null)
        {
            // detect active input
            if (InputPlayer.InputGasPedal != 0 || InputPlayer.InputSteer != 0)
            {
                LastInputActiveAt = GameTime;
            }

            // pause timer if no input for 5 seconds
            if (!TimerPaused && LastInputActiveAt != -1 && GameTime - LastInputActiveAt >= 5000)
            {
                PauseTimer();
            }

            // unpause timer if input detected
            if (TimerPaused && InputPlayer.RaceStartTime < GameTime && (InputPlayer.InputGasPedal != 0 || InputPlayer.InputSteer != 0))
            {
                UnpauseTimer();
            }

            // increment attempt if respawned after race
            if (EnqueueAttempt && InputPlayer.RaceStartTime != 0 && InputPlayer.RaceStartTime < GameTime)
            {
                IncrementAttempt();
                EnqueueAttempt = false;
            }

            // start session timer if not started and input detected
            if (!TimerPaused && SessionStartedAt == "" && InputPlayer.RaceStartTime < GameTime && InputPlayer.InputGasPedal != 0)
            {
                SessionStartedAt = (GameTime / 1000).ToString();
            }

            // update every tenth of a second
            if ((GameTime / 100) != (PrevGameTime / 100))
            {
                if (SessionStartedAt == "")
                {
                    FormatDelta(LabelSessionTime, SessionTimeAtPause);
                    FormatDelta(LabelTotalTime, TotalTimeAtStart);
                }
                else
                {
                    var delta = TimeLib.GetDelta((GameTime / 1000).ToString(), SessionStartedAt);

                    var sessionTime = SessionTimeAtPause + delta;
                    FormatDelta(LabelSessionTime, sessionTime);

                    var totalTime = TotalTimeAtStart + delta;
                    FormatDelta(LabelTotalTime, totalTime);

                    // update total time every second
                    if ((GameTime / 1000) != (PrevGameTime / 1000))
                    {
                        var persistent_EnvimixTotalTime = Persistent<Dictionary<string, int>>.For(Map);
                        persistent_EnvimixTotalTime.Get()[car.Get()] = totalTime;
                    }

                    // update session time and total time every half second
                    if ((GameTime / 500) != (PrevGameTime / 500))
                    {
                        SendCustomEvent("SessionTime", new[] { sessionTime.ToString() });
                        SendCustomEvent("TotalTime", new[] { totalTime.ToString() });
                    }
                }

                PrevGameTime = GameTime;
            }
        }
    }

    private static void FormatDelta(CMlLabel label, int delta)
    {
        label.Value = TimeLib.FormatDelta("0", delta.ToString(), TimeLib.EDurationFormats.Abbreviated);

        if (label.Value == "")
        {
            label.Value = "0s";
        }
    }
}
