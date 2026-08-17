using System.Diagnostics.Metrics;

namespace Envimix.Media.Manialinks.Universe2;

public class Dashboard2 : CTmMlScriptIngame, IContext
{
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
    [ManialinkControl] public required CMlQuad QuadUnderwater;
    [ManialinkControl] public required CMlLabel LabelUnderwaterTime;

    [Netread(NetFor.Teams0)] public int FinishedAt { get; set; }
    [Netread(NetFor.Teams0)] public bool Outro { get; set; }

    public bool MenuOpen;
    public bool PrevIsVisible;
    public int StartTime;
    public int PrevGear;
    public bool PrevInputIsBraking;
    public int RaceTime;
    public int RaceTimeSnapshot;
    public int RespawnAt;

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    public Dashboard2()
    {
        RaceEvent += (CTmRaceClientEvent e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.Player == GetPlayer())
                    {
                        LabelCP.Opacity = 0;
                        AnimMgr.Add(LabelCP, "<label opacity=\"1\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    if (e.Player == GetPlayer() && RaceTime > 0)
                    {
                        for (var i = 0; i < 6; i++)
                        {
                            Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.5f, Delay: i * 80);
                        }

                        RaceTimeSnapshot = RaceTime;
                        RaceTime = 0;
                        RespawnAt = GameTime;
                    }
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

        return !IsInGameMenuDisplayed && FinishedAt == -1 && !Outro && GUIPlayer is not null;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public void Main()
    {
        FrameDashboard.Visible = false;
        PrevIsVisible = IsVisible();

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        var isVisible = IsVisible();

        if (isVisible != PrevIsVisible)
        {
            StartTime = Now;
            FrameDashboard.Visible = isVisible;
            PrevIsVisible = isVisible;
        }

        if (!isVisible)
        {
            return;
        }

        FrameTime.RelativePosition_V3.Y = AnimLib.EaseOutQuad(Now - StartTime, -15, 15, 500);
        FrameCheckpoint.RelativePosition_V3.X = AnimLib.EaseOutQuad(Now - StartTime - 100, 40, -40, 500);
        FrameSpeed.RelativePosition_V3.X = AnimLib.EaseOutQuad(Now - StartTime - 100, -40, 40, 500);
        FrameGear.RelativePosition_V3.X = AnimLib.EaseOutQuad(Now - StartTime - 150, -30, 30, 1000);
        FrameInfo.RelativePosition_V3.X = AnimLib.EaseOutQuad(Now - StartTime - 150, 30, -30, 1000);
        FrameCheckpointCounter.RelativePosition_V3.Y = AnimLib.EaseOutQuad(Now - StartTime - 400, -6, 6, 500);
        FrameDistance.RelativePosition_V3.Y = AnimLib.EaseOutQuad(Now - StartTime - 400, -6, 6, 500);

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
                    quad.Opacity = AnimLib.EaseOutQuad(_T: Now - StartTime - 800, _Base: 0, _Change: 1, _Duration: 200);
                }
                else if (control2 is CMlLabel label)
                {
                    label.Opacity = AnimLib.EaseOutQuad(_T: Now - StartTime - 800, _Base: 0, _Change: 1, _Duration: 200);
                }
            }
        }

        int raceStartTime;
        if (IndependantLaps)
        {
            raceStartTime = GetPlayer().LapStartTime;
        }
        else
        {
            raceStartTime = GetPlayer().RaceStartTime;
        }

        if (raceStartTime <= 0)
        {
            LabelTime.SetText("0:00.000");
        }
        else
        {
            if (GameTime - raceStartTime >= 0)
            {
                RaceTime = MathLib.Max(0, GameTime - raceStartTime);
                LabelTime.SetText(TimeToTextWithMilli(RaceTime));
            }
            else
            {
                var visualRaceTime = MathLib.NearestInteger(AnimLib.EaseOutQuad(GameTime - RespawnAt, RaceTimeSnapshot * 1f, -RaceTimeSnapshot * 1f, 500));
                LabelTime.SetText(TimeToTextWithMilli(visualRaceTime));
            }
        }

        if (GameTime - raceStartTime < 0)
        {
            LabelTime.RelativeScale = 1;
        }
        else
        {
            LabelTime.RelativeScale = AnimLib.EaseOutQuad(MathLib.Max(0, GameTime - raceStartTime), 1.2f, -0.2f, 200);
        }

        LabelSpeed.SetText(GetPlayer().DisplaySpeed.ToString());

        var rpmRatio = MathLib.Clamp((GetPlayer().EngineRpm - 1000) / 9000, _Min: 0, _Max: 1);
        LabelSpeed.RelativeScale = rpmRatio * 0.2f + 0.9f;
        GaugeRPM.Ratio = rpmRatio;
        GaugeRPM.RelativeScale = GetPlayer().EngineTurboRatio * 0.15f + 1;

        UpdateDistance();
        UpdateGear();
        UpdateSteer();
        UpdateBrake();
        UpdateFreewheel();
        UpdateUnderwater();
        UpdateCheckpointCounter();
        UpdateCheckpoint();
        UpdateSteepness();
    }

    private void UpdateBrake()
    {
        if (GetPlayer().InputIsBraking == PrevInputIsBraking)
        {
            return;
        }

        if (GetPlayer().InputIsBraking)
        {
            AnimMgr.Add(QuadBrake, "<quad opacity=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        }
        else
        {
            AnimMgr.Add(QuadBrake, "<quad opacity=\"0\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        PrevInputIsBraking = GetPlayer().InputIsBraking;
    }

    void UpdateDistance()
    {
        if (IsSpectator)
        {
            LabelDistance.Value = $"{TextLib.GetTranslatedText("Distance")}: $o?";
        }
        else if (GetPlayer().Distance >= 1000)
        {
            LabelDistance.Value = $"{TextLib.GetTranslatedText("Distance")}: $o{TextLib.FormatReal(GetPlayer().Distance / 1000f, 2, false, false)}$tkm";
        }
        else
        {
            LabelDistance.Value = $"{TextLib.GetTranslatedText("Distance")}: $o{TextLib.FormatReal(GetPlayer().Distance, 2, false, false)}$tm";
        }
    }

    private void UpdateGear()
    {
        if (IsSpectator)
        {
            LabelGear.SetText("?");
            return;
        }

        if (GetPlayer().EngineCurGear == PrevGear)
        {
            return;
        }

        LabelGear.Opacity = 0;
        AnimMgr.Add(LabelGear, "<label opacity=\"1\"/>", Duration: 200, EasingFunc: CAnimManager.EAnimManagerEasing.QuadOut);

        if (GetPlayer().EngineCurGear > 0)
        {
            LabelGear.SetText(GetPlayer().EngineCurGear.ToString());
        }
        else
        {
            LabelGear.SetText("R");
        }

        PrevGear = GetPlayer().EngineCurGear;
    }

    private void UpdateSteer()
    {
        QuadSteerLeft.Size.X = MathLib.Clamp(-GetPlayer().InputSteer, _Min: 0, _Max: 1) * 30;
        QuadSteerRight.Size.X = MathLib.Clamp(GetPlayer().InputSteer, _Min: 0, _Max: 1) * 30;
    }

    private void UpdateFreewheel()
    {
        if (GetPlayer().FreeWheelingDuration > 0)
        {
            QuadSpeedFreewheeling.Opacity = (MathLib.Sin(GetPlayer().FreeWheelingDuration / 100f + 180) + 1) / 2 * 0.25f;
        }
        else
        {
            QuadSpeedFreewheeling.Opacity = 0;
        }
    }

    private void UpdateUnderwater()
    {
        if (GetPlayer().InWaterDuration > 0)
        {
            QuadUnderwater.Opacity = (MathLib.Sin(GetPlayer().InWaterDuration / 100f + 180) + 1) / 2 * 0.2f + 0.1f;
            LabelUnderwaterTime.Value = TimeToTextWithMilli(GetPlayer().InWaterDuration);
            LabelUnderwaterTime.Visible = true;
        }
        else
        {
            QuadUnderwater.Opacity = 0;
            LabelUnderwaterTime.Visible = false;
        }
    }

    private void UpdateCheckpointCounter()
    {
        int mapCheckpointCount;
        if (IndependantLaps)
        {
            mapCheckpointCount = MapCheckpointPos.Count;
        }
        else
        {
            mapCheckpointCount = MapCheckpointPos.Count * NbLaps + NbLaps - 1;
        }

        var playerCheckpointCount = GetPlayer().CurRace.Checkpoints.Count;

        if (IndependantLaps)
        {
            playerCheckpointCount %= mapCheckpointCount + 1;
        }

        playerCheckpointCount = MathLib.Min(playerCheckpointCount, mapCheckpointCount);

        if (mapCheckpointCount == 0)
        {
            LabelCheckpointCounter.Value = "No checkpoints";
        }
        else
        {
            LabelCheckpointCounter.Value = $"Checkpoint $o{playerCheckpointCount}/{mapCheckpointCount}";
        }

        if (mapCheckpointCount > 0 && playerCheckpointCount >= mapCheckpointCount)
        {
            LabelCheckpointCounter.TextColor = new Vec3(1, 1, 0);
        }
        else
        {
            LabelCheckpointCounter.TextColor = new Vec3(1, 1, 1);
        }
    }

    private void UpdateSteepness()
    {
        var aimPitch = MathLib.Clamp(GetPlayer().AimPitch, _Min: -1, _Max: 1);

        if (aimPitch >= 0)
        {
            FrameSteepnessZeroOne.RelativeRotation = aimPitch * 90;
            LabelSteepnessZeroOne.Value = $"{MathLib.NearestInteger(-aimPitch * 90)}°";

            FrameSteepnessZeroOne.Visible = true;
            FrameSteepnessZeroMinusOne.Visible = false;
        }
        else
        {
            FrameSteepnessZeroMinusOne.RelativeRotation = aimPitch * 90;
            LabelSteepnessZeroMinusOne.Value = $"{MathLib.NearestInteger(-aimPitch * 90)}°";

            FrameSteepnessZeroMinusOne.Visible = true;
            FrameSteepnessZeroOne.Visible = false;
        }
    }

    private void UpdateCheckpoint()
    {
        if (IndependantLaps)
        {
            if (GetPlayer().CurLap.Checkpoints.Count == 0)
            {
                LabelCP.Value = "-:--.---";
                QuadCP.Visible = false;
            }
            else if (GetPlayer().Score.BestRace.Checkpoints.Count == 0)
            {
                LabelCP.Value = TimeToTextWithMilli(GetPlayer().CurLap.Checkpoints[GetPlayer().CurLap.Checkpoints.Count - 1]);
                QuadCP.Visible = false;
            }
            else
            {
                var latestCheckpoint = GetPlayer().CurLap.Checkpoints[GetPlayer().CurLap.Checkpoints.Count - 1];
                var difference = latestCheckpoint - GetPlayer().Score.BestRace.Checkpoints[GetPlayer().CurLap.Checkpoints.Count - 1];

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

                QuadCP.Visible = true;
            }
        }
        else
        {
            if (GetPlayer().CurRace.Checkpoints.Count == 0)
            {
                LabelCP.Value = "-:--.---";
                QuadCP.Visible = false;
            }
            else if (GetPlayer().Score.BestRace.Checkpoints.Count == 0)
            {
                LabelCP.Value = TimeToTextWithMilli(GetPlayer().CurRace.Checkpoints[GetPlayer().CurRace.Checkpoints.Count - 1]);
                QuadCP.Visible = false;
            }
            else
            {
                var latestCheckpoint = GetPlayer().CurRace.Checkpoints[GetPlayer().CurRace.Checkpoints.Count - 1];
                var difference = latestCheckpoint - GetPlayer().Score.BestRace.Checkpoints[GetPlayer().CurRace.Checkpoints.Count - 1];

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

                QuadCP.Visible = true;
            }
        }
    }
}
