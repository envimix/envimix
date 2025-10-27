using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Checkpoint2 : CTmMlScriptIngame, IContext
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

    public struct SUniverseWaypoint
    {
        public string Login;
        public bool IsEndLap;
        public int LapTime;
        public int RaceTime;
        public int CheckpointInLap;
        public int CheckpointInRace;
    }

    [ManialinkControl] public required CMlLabel LabelCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameCheckpoint;
    [ManialinkControl] public required CMlFrame FrameCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameInnerCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameLap;
    [ManialinkControl] public required CMlLabel LabelLapTime;
    [ManialinkControl] public required CMlFrame FrameDifferences;

    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }

    public int CheckpointShowTime = -1;

    public Checkpoint2()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (!e.IsEndRace || IsExplore())
                    {
                        RaceEventWaypoint(e);
                    }
                    break;
            }
        };
    }

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    static string ConstructFilterKey(CPlayer player)
    {
        var car = Netread<string>.For(player);
        var gravity = Netread<int>.For(player);

        return $"{car.Get()}_{gravity.Get()}_Time";
    }

    public void Main()
    {
        FrameCheckpoint.Hide();

        foreach (var control in FrameDifferences.Controls)
        {
            control.Hide();
        }

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (CheckpointShowTime != -1)
        {
            FrameCheckpoint.Visible = !ScoreTableIsVisible;

            var time = Now - CheckpointShowTime;

            if (time > 3600)
            {
                FrameCheckpoint.Hide();
                CheckpointShowTime = -1;
            }
            else if (time > 2700)
            {
                FrameInnerCheckpointTime.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2700, 40, -40, 300);

                foreach (var control in (FrameCheckpointTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                {
                    control.Size.X = AnimLib.EaseOutQuad(time - 2700, 40, -40, 300);
                }

                if (FrameLap.Visible)
                {
                    foreach (var control in (FrameLap.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                    {
                        control.Size.X = AnimLib.EaseOutQuad(time - 2800, 20, -20, 300);
                    }

                    FrameLap.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2800, 20, -20, 300);
                }

                for (var i = 0; i < FrameDifferences.Controls.Count; i++)
                {
                    var frame = (FrameDifferences.Controls[i] as CMlFrame)!;

                    if (!frame.Visible)
                    {
                        continue;
                    }

                    var frameRecordType = (frame.GetFirstChild("FrameRecordType") as CMlFrame)!;

                    foreach (var control in (frameRecordType.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                    {
                        control.Size.X = AnimLib.EaseOutQuad(time - 2800 - i * 100, 30, -30, 300);
                    }

                    frameRecordType.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2800 - i * 100, 60, -60, 300);

                    var frameTime = (frame.GetFirstChild("FrameTime") as CMlFrame)!;

                    foreach (var control in (frameTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                    {
                        control.Size.X = AnimLib.EaseOutQuad(time - 2800 - i * 100, 20, -20, 300);
                    }

                    frameTime.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2800 - i * 100, 40, -40, 300);
                }
            }
            else
            {
                FrameInnerCheckpointTime.ClipWindowSize.X = AnimLib.EaseOutQuad(time, 0, 40, 300);

                foreach (var control in (FrameLap.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                {
                    control.Size.X = AnimLib.EaseOutQuad(time - 100, 0, 20, 300);
                }

                FrameLap.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 100, 0, 20, 300);

                for (var i = 0; i < FrameDifferences.Controls.Count; i++)
                {
                    var frame = (FrameDifferences.Controls[i] as CMlFrame)!;

                    if (!frame.Visible)
                    {
                        continue;
                    }

                    var frameRecordType = (frame.GetFirstChild("FrameRecordType") as CMlFrame)!;
                    
                    foreach (var control in (frameRecordType.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                    {
                        control.Size.X = AnimLib.EaseOutQuad(time - 100 - i * 100, 0, 30, 300);
                    }

                    frameRecordType.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 100 - i * 100, 0, 60, 300);

                    var frameTime = (frame.GetFirstChild("FrameTime") as CMlFrame)!;
                    
                    foreach (var control in (frameTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                    {
                        control.Size.X = AnimLib.EaseOutQuad(time - 100 - i * 100, 0, 20, 300);
                    }

                    frameTime.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 100 - i * 100, 0, 40, 300);
                }
            }
        }
    }

    void Waypoint(SUniverseWaypoint waypoint)
    {
        if (waypoint.Login != GetPlayer().User.Login)
        {
            return;
        }

        FrameInnerCheckpointTime.ClipWindowSize.X = 0;

        foreach (var control in (FrameCheckpointTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, "<quad size=\"40 9\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        FrameCheckpoint.Show();
        CheckpointShowTime = Now;

        if (waypoint.IsEndLap)
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 1, 1);
        }
        else
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 0, 1);
        }

        if (IndependantLaps)
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(waypoint.LapTime));
        }
        else
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(waypoint.RaceTime));
        }

        if (waypoint.RaceTime != waypoint.LapTime && !IndependantLaps)
        {
            LabelLapTime.SetText(TimeToTextWithMilli(waypoint.LapTime));
            FrameLap.Show();
        }
        else
        {
            FrameLap.Hide();
        }

        var key = ConstructFilterKey(GetPlayer());

        var envimixBestRace = Netread<Dictionary<string, SRecord>>.For(GetPlayer().Score);

        var framePb = (FrameDifferences.Controls[0] as CMlFrame)!;

        if (envimixBestRace.Get().ContainsKey(key) && envimixBestRace.Get()[key].Time != -1)
        {
            framePb.Show();

            var pb = envimixBestRace.Get()[key];

            var frameTime = (framePb.GetFirstChild("FrameTime") as CMlFrame)!;
            var labelTime = (frameTime.GetFirstChild("LabelTime") as CMlLabel)!;
            var quadColor = ((frameTime.GetFirstChild("FrameBackground") as CMlFrame)!.GetFirstChild("QuadColor") as CMlQuad)!;

            int difference;
            if (IndependantLaps)
            {
                difference = waypoint.LapTime - pb.Checkpoints[waypoint.CheckpointInLap].Time; // Time difference
            }
            else
            {
                difference = waypoint.RaceTime - pb.Checkpoints[waypoint.CheckpointInRace].Time; // Time difference
            }

            if (difference > 0)
            {
                labelTime.SetText("+" + TimeToTextWithMilli(difference));
                quadColor.ModulateColor = new Vec3(1, 0.1f, 0);
            }
            else if (difference == 0)
            {
                labelTime.SetText(TimeToTextWithMilli(difference));
                quadColor.ModulateColor = new Vec3(1, 0, 1);
            }
            else
            {
                labelTime.SetText(TimeToTextWithMilli(difference));
                quadColor.ModulateColor = new Vec3(0, 0.1f, 1);
            }
        }
        else
        {
            framePb.Hide();
        }
    }

    void RaceEventWaypoint(CTmRaceClientEvent e)
    {
        SUniverseWaypoint waypoint = new()
        {
            Login = e.Player.User.Login,
            IsEndLap = e.IsEndLap,
            LapTime = e.LapTime,
            RaceTime = e.RaceTime,
            CheckpointInLap = e.CheckpointInLap,
            CheckpointInRace = e.CheckpointInRace
        };

        Waypoint(waypoint);
    }
}
