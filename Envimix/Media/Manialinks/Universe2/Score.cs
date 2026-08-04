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

    bool IsSolo()
    {
        return CurrentServerLogin is "";
    }

    bool IsVisible()
    {
        if (IsExplore())
        {
            return !MenuOpen;
        }

        return !IsInGameMenuDisplayed && FinishedAt == -1;
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private void IncrementFinish()
    {
        var car = Netread<string>.For(GetPlayer());
        var persistent_EnvimixFinishes = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (!persistent_EnvimixFinishes.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid] = new();
        }
        if (!persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid].ContainsKey(car.Get()))
        {
            persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid][car.Get()] = 0;
        }
        persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid][car.Get()] = persistent_EnvimixFinishes.Get()[Map.MapInfo.MapUid][car.Get()] + 1;
    }

    private void IncrementAttempt()
    {
        var car = Netread<string>.For(GetPlayer());
        var persistent_EnvimixAttempts = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
        if (!persistent_EnvimixAttempts.Get().ContainsKey(Map.MapInfo.MapUid))
        {
            persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid] = new();
        }
        if (!persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid].ContainsKey(car.Get()))
        {
            persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid][car.Get()] = 0;
        }
        persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid][car.Get()] = persistent_EnvimixAttempts.Get()[Map.MapInfo.MapUid][car.Get()] + 1;
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
            var persistent_EnvimixTotalTime = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
            if (!persistent_EnvimixTotalTime.Get().ContainsKey(Map.MapInfo.MapUid))
            {
                persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid] = new();
            }
            persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid][car.Get()] = TotalTimeAtStart;
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
    }

    public void Loop()
    {
        var car = Netread<string>.For(GetPlayer());
        if (car.Get() != PrevCar)
        {
            var persistent_EnvimixTotalTime = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
            if (persistent_EnvimixTotalTime.Get().ContainsKey(Map.MapInfo.MapUid) && persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid].ContainsKey(car.Get()))
            {
                TotalTimeAtStart = persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid][car.Get()];
            }
            else
            {
                TotalTimeAtStart = 0;
            }

            SessionStartedAt = "";
            SessionTimeAtPause = 0;

            if (IsSolo())
            {
                var persistent_EnvimixTotalTimeTrackingAt = Persistent<Dictionary<string, Dictionary<string, string>>>.For(LocalUser);
                if (!persistent_EnvimixTotalTimeTrackingAt.Get().ContainsKey(Map.MapInfo.MapUid))
                {
                    persistent_EnvimixTotalTimeTrackingAt.Get()[Map.MapInfo.MapUid] = new();
                }
                if (!persistent_EnvimixTotalTimeTrackingAt.Get()[Map.MapInfo.MapUid].ContainsKey(car.Get()))
                {
                    persistent_EnvimixTotalTimeTrackingAt.Get()[Map.MapInfo.MapUid][car.Get()] = TimeLib.GetCurrent();
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

        // detect active input
        if (GetPlayer().InputGasPedal != 0 || GetPlayer().InputSteer != 0)
        {
            LastInputActiveAt = GameTime;
        }

        // pause timer if no input for 5 seconds
        if (!TimerPaused && LastInputActiveAt != -1 && GameTime - LastInputActiveAt >= 5000)
        {
            PauseTimer();
        }

        // unpause timer if input detected
        if (TimerPaused && GetPlayer().RaceStartTime < GameTime && (GetPlayer().InputGasPedal != 0 || GetPlayer().InputSteer != 0))
        {
            UnpauseTimer();
        }

        // increment attempt if respawned after race
        if (EnqueueAttempt && GetPlayer().RaceStartTime != 0 && GetPlayer().RaceStartTime < GameTime)
        {
            IncrementAttempt();
            EnqueueAttempt = false;
        }

        // start session timer if not started and input detected
        if (!TimerPaused && SessionStartedAt == "" && GetPlayer().RaceStartTime < GameTime && GetPlayer().InputGasPedal != 0)
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
                FormatDelta(LabelSessionTime, SessionTimeAtPause + delta);
                FormatDelta(LabelTotalTime, TotalTimeAtStart + delta);

                // update total time every second
                if (IsSolo() && (GameTime / 1000) != (PrevGameTime / 1000))
                {
                    var persistent_EnvimixTotalTime = Persistent<Dictionary<string, Dictionary<string, int>>>.For(LocalUser);
                    if (!persistent_EnvimixTotalTime.Get().ContainsKey(Map.MapInfo.MapUid))
                    {
                        persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid] = new();
                    }
                    persistent_EnvimixTotalTime.Get()[Map.MapInfo.MapUid][car.Get()] = TotalTimeAtStart + delta;
                }
            }

            PrevGameTime = GameTime;
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
