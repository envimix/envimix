using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Dashboard : CTmMlScriptIngame, IContext
{
    public int Start;
    public int PlayerTime;
    public int CPCounter;
    public int IndependantLapsOffset;
	public float DistanceOffset;
    public bool PreviousIsVisible;
    public CTmMlPlayer.ERaceState PreviousRaceState;
    public bool PreviousBrakeState;
    public int TimeResetRaceTime;
    public int TimeResetStamp;
    public int TimeResetLength = 500;
    public int LastGear;

    [ManialinkControl] public required CMlFrame FrameDashboard;
    [ManialinkControl] public required CMlFrame FrameTime;
    [ManialinkControl] public required CMlFrame FrameCheckpoint;
    [ManialinkControl] public required CMlFrame FrameCheckpointCounter;
    [ManialinkControl] public required CMlFrame FrameDistance;
    [ManialinkControl] public required CMlFrame FrameSpeed;
    [ManialinkControl] public required CMlFrame FrameGear;
    [ManialinkControl] public required CMlFrame FrameInfo;
    [ManialinkControl] public required CMlFrame FrameSteepness;
    [ManialinkControl] public required CMlLabel LabelGear;
    [ManialinkControl] public required CMlLabel LabelTime;
    [ManialinkControl] public required CMlQuad QuadBrake;
    [ManialinkControl] public required CMlLabel LabelCP;
    [ManialinkControl] public required CMlQuad QuadCP;
    [ManialinkControl] public required CMlLabel LabelSpeed;
    [ManialinkControl] public required CMlLabel LabelDistance;
    [ManialinkControl] public required CMlLabel LabelCheckpointCounter;
    [ManialinkControl] public required CMlGauge GaugeRPM;
    [ManialinkControl] public required CMlQuad QuadSteerLeft;
    [ManialinkControl] public required CMlQuad QuadSteerRight;
    [ManialinkControl] public required CMlQuad QuadSpeedFreewheeling;
    [ManialinkControl] public required CMlFrame FrameSteepnessZeroMinusOne;
    [ManialinkControl] public required CMlLabel LabelSteepnessZeroMinusOne;
    [ManialinkControl] public required CMlFrame FrameSteepnessZeroOne;
    [ManialinkControl] public required CMlLabel LabelSteepnessZeroOne;

    [Netread(NetFor.Teams0)] public int FinishedAt { get; set; }
    [Netread(NetFor.Teams0)] public bool Outro { get; set; }

    public Dashboard()
    {
        PluginCustomEvent += (string type, ImmutableArray<string> data) =>
        {
            switch (type)
            {
                case "Show":
                    Start = Now;
                    LabelGear.Opacity = 1;
                    QuadBrake.Opacity = 0;
                    break;
            }
        };
        
        RaceEvent += (CTmRaceClientEvent e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    WaypointEvent(e);
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    RespawnEvent(e);
                    break;
            }
        };

        PluginCustomEvent += (eventName, eventParams) =>
        {
            switch (eventName)
            {
                case "MenuOpen":
                    MenuOpen = eventParams.Length > 0 && eventParams[0] == "True";
                    break;
            }
        };
    }

    CTmMlPlayer GetOwner()
    {
        Wait(() => GUIPlayer is not null || InputPlayer is not null);
        if (GUIPlayer is not null) return GUIPlayer;
        return InputPlayer;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    void WaypointEvent(CTmRaceClientEvent e)
    {
        if (e.Player.User.Login != LocalUser.Login)
        {
            return;
        }

        if (e.IsEndRace)
        {
            PlayerTime = e.RaceTime;
        }
        else
        {
            CPCounter += 1;

            if (e.IsEndLap && IndependantLaps)
            {
                IndependantLapsOffset = e.RaceTime;
                CPCounter = 0;
                DistanceOffset = GetOwner().Distance;
            }
        }
        //PlayAudio(Sound_TimeChange);

        if (e.Player.Score is null)
        {
            return;
        }

        if (e.Player.Score.BestRace.Checkpoints.Count == 0)
        {
            LabelCP.Opacity = 0;
            AnimMgr.Add(LabelCP, "<label opacity=\"1\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);
            QuadCP.Opacity = 0;

            if (IndependantLaps)
            {
                LabelCP.Value = TimeToTextWithMilli(e.LapTime);
            }
            else
            {
                LabelCP.Value = TimeToTextWithMilli(e.RaceTime);
            }

            return;
        }

        int difference;
        if (IndependantLaps)
        {
            difference = e.LapTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInLap];
        }
        else
        {
            difference = e.RaceTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInRace];
        }

        if (difference > 0)
        {
            LabelCP.SetText($"+{TimeToTextWithMilli(difference)}");
            QuadCP.Colorize = new Vec3(1, 0.1, 0);
        }
        else if (difference < 0)
        {
            LabelCP.SetText(TimeToTextWithMilli(difference));
            QuadCP.Colorize = new Vec3(0, 0.1, 1);
        }
        else
        {
            LabelCP.SetText(TimeToTextWithMilli(difference));
            QuadCP.Colorize = new Vec3(1, 0, 1);
        }

        LabelCP.Opacity = 0;
        AnimMgr.Add(LabelCP, "<label opacity=\"1\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);

        QuadCP.Opacity = 0;
        AnimMgr.Add(QuadCP, "<quad opacity=\"0.5\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    void RespawnEvent(CTmRaceClientEvent e)
    {
        if (e.Player.User.Login != LocalUser.Login || e.Player.CurRace.Time != -1)
        {
            return;
        }

        // If fully respawned to start
        if (PlayerTime > 0)
        {
            for (var i = 0; i < 6; i++)
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.5f, Delay: i * 80);
            }
        }

        if (LabelCP.Value is not "-:--.---")
        {
            LabelCP.Opacity = 0;
            AnimMgr.Add(LabelCP, "<label opacity=\"1\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        LabelCP.Value = "-:--.---";
        QuadCP.Opacity = 0;

        CPCounter = 0;
        IndependantLapsOffset = 0;
        DistanceOffset = 0;
    }

    public bool MenuOpen;

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    private bool IsVisible()
    {
        if (IsExplore())
        {
            return !MenuOpen;
        }

        return !IsInGameMenuDisplayed && FinishedAt == -1 && !Outro;
    }

    public void Main()
    {
        Start = Now + 500;

        FrameTime.ClipWindowRelativePosition.Y = -20;
        FrameCheckpoint.ClipWindowRelativePosition.X = 32;
        //Frame_Distance.ClipWindowRelativePosition.X = -32.;

        FrameCheckpointCounter.ClipWindowRelativePosition.Y = -6;
        FrameDistance.ClipWindowRelativePosition.Y = -6;

        LastGear = GetOwner().EngineCurGear;

        FrameDashboard.Visible = IsVisible();
        PreviousIsVisible = IsVisible();
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            Start = Now;
            FrameDashboard.Visible = IsVisible();
            PreviousIsVisible = IsVisible();
        }

        FrameTime.ClipWindowRelativePosition.Y = AnimLib.EaseOutQuad(Now - Start, -20, 20, 500);
        FrameCheckpoint.ClipWindowRelativePosition.X = AnimLib.EaseOutQuad(Now - Start - 100, 40, -40, 500);
        FrameSpeed.ClipWindowRelativePosition.X = AnimLib.EaseOutQuad(Now - Start - 100, -40, 40, 500);
        FrameGear.ClipWindowRelativePosition.X = AnimLib.EaseOutQuad(Now - Start - 100, -30, 30, 1000);
        FrameInfo.ClipWindowRelativePosition.X = AnimLib.EaseOutQuad(Now - Start - 100, 30, -30, 1000);
        FrameCheckpointCounter.ClipWindowRelativePosition.Y = AnimLib.EaseOutQuad(Now - Start - 400, -6, 6, 500);
        FrameDistance.ClipWindowRelativePosition.Y = AnimLib.EaseOutQuad(Now - Start - 400, -6, 6, 500);

        foreach (var control in FrameSteepness.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            foreach (var control2 in frame.Controls)
            {
                if (control2 is CMlQuad quad)
                {
                    quad.Opacity = AnimLib.EaseOutQuad(_T: Now - Start - 800, _Base: 0, _Change: 1, _Duration: 200);
                }
                else if (control2 is CMlLabel label)
                {
                    label.Opacity = AnimLib.EaseOutQuad(_T: Now - Start - 800, _Base: 0, _Change: 1, _Duration: 200);
                }
            }
        }

        var rpmRatio = MathLib.Clamp((GetOwner().EngineRpm - 1000) / 9000, _Min: 0, _Max: 1);

        switch (GetOwner().RaceState)
        {
            case CTmMlPlayer.ERaceState.BeforeStart:
                if (PreviousRaceState == CTmMlPlayer.ERaceState.Running)
                {
                    TimeResetRaceTime = PlayerTime - IndependantLapsOffset;
                    TimeResetStamp = Now;

                    PreviousRaceState = CTmMlPlayer.ERaceState.BeforeStart;
                }

                PlayerTime = MathLib.NearestInteger(AnimLib.EaseOutQuad(Now - TimeResetStamp, TimeResetRaceTime + 0f, -TimeResetRaceTime + 0f, TimeResetLength));
                break;
            case CTmMlPlayer.ERaceState.Running:
                if (PreviousRaceState == CTmMlPlayer.ERaceState.BeforeStart)
                {
                    LabelTime.RelativeScale = 1.25f;
                    AnimMgr.Add(LabelTime, "<label scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);

                    PreviousRaceState = CTmMlPlayer.ERaceState.Running;
                }

                PlayerTime = MathLib.Max(0, GameTime - GetOwner().RaceStartTime);
                break;
            case CTmMlPlayer.ERaceState.Finished:
                break;
        }

        LabelTime.SetText(TimeToTextWithMilli(PlayerTime - IndependantLapsOffset));
        LabelSpeed.SetText(GetOwner().DisplaySpeed.ToString());
        LabelSpeed.RelativeScale = rpmRatio * 0.2f + 0.9f;
        LabelDistance.Value = $"{TextLib.GetTranslatedText("Distance")}: $o{TextLib.FormatReal(GetOwner().Distance - DistanceOffset, 2, false, false)}$tm";

        int checkpointCount;
        if (IndependantLaps)
        {
            checkpointCount = MapCheckpointPos.Count;
        }
        else
        {
            checkpointCount = MapCheckpointPos.Count * NbLaps + NbLaps - 1;
        }

        if (checkpointCount == 0)
        {
            LabelCheckpointCounter.Value = "No checkpoints";
        }
        else
        {
            LabelCheckpointCounter.Value = $"Checkpoint $o{CPCounter}/{checkpointCount}";
        }

        if (checkpointCount > 0 && checkpointCount == CPCounter)
        {
            LabelCheckpointCounter.TextColor = new Vec3(1, 1, 0);
        }
        else
        {
            LabelCheckpointCounter.TextColor = new Vec3(1, 1, 1);
        }

        GaugeRPM.Ratio = rpmRatio;
        GaugeRPM.RelativeScale = GetOwner().EngineTurboRatio * 0.15f + 1;

        if (GetOwner().EngineCurGear != LastGear)
        {
            LabelGear.Opacity = 0;
            AnimMgr.Add(LabelGear, "<label opacity=\"1\"/>", Duration: 200, EasingFunc: CAnimManager.EAnimManagerEasing.QuadOut);

            if (GetOwner().EngineCurGear > 0)
            {
                LabelGear.SetText(GetOwner().EngineCurGear.ToString());
            }
            else
            {
                LabelGear.SetText("R");
            }

            LastGear = GetOwner().EngineCurGear;
        }

        QuadSteerLeft.Size.X = MathLib.Clamp(-GetOwner().InputSteer, _Min: 0, _Max: 1) * 30;
        QuadSteerRight.Size.X = MathLib.Clamp(GetOwner().InputSteer, _Min: 0, _Max: 1) * 30;

        // If player started or stopped braking
        if (GetOwner().InputIsBraking != PreviousBrakeState)
        {
            if (GetOwner().InputIsBraking)
            {
                AnimMgr.Add(QuadBrake, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Add(QuadBrake, "<quad opacity=\"0\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            PreviousBrakeState = GetOwner().InputIsBraking;
        }

        if (GetOwner().FreeWheelingDuration > 0)
        {
            QuadSpeedFreewheeling.Opacity = (MathLib.Sin(GetOwner().FreeWheelingDuration / 100f + 180) + 1) / 2 * 0.25f;
        }
        else
        {
            QuadSpeedFreewheeling.Opacity = 0;
        }

        if (GetOwner().AimPitch <= 0 && GetOwner().AimPitch >= -1)
        {
            FrameSteepnessZeroMinusOne.RelativeRotation = GetOwner().AimPitch * 90;
            LabelSteepnessZeroMinusOne.Value = $"{MathLib.NearestInteger(-GetOwner().AimPitch * 90)}°";

            FrameSteepnessZeroMinusOne.Visible = true;
            FrameSteepnessZeroOne.Visible = false;
        }
        if (GetOwner().AimPitch >= 0 && GetOwner().AimPitch <= 1)
        {
            FrameSteepnessZeroOne.RelativeRotation = GetOwner().AimPitch * 90;
            LabelSteepnessZeroOne.Value = $"{MathLib.NearestInteger(-GetOwner().AimPitch * 90)}°";

            FrameSteepnessZeroOne.Visible = true;
            FrameSteepnessZeroMinusOne.Visible = false;
        }
    }
}
