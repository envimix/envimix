namespace Envimix.Media.Manialinks.Universe2;

public class Checkpoint3 : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlLabel LabelCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameCheckpoint;
    [ManialinkControl] public required CMlFrame FrameCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameInnerCheckpointTime;
    [ManialinkControl] public required CMlFrame FrameLap;
    [ManialinkControl] public required CMlLabel LabelLapTime;
    [ManialinkControl] public required CMlFrame FrameDifferences;

    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }

    public int CheckpointShowTime = -1;
    public int PrevBestTime = -1;
    public bool IsFirstFinish;

    public Checkpoint3()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    Waypoint(e);
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    IsFirstFinish = false;
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

    void Waypoint(CTmRaceClientEvent e)
    {
        if (e.Player != GetPlayer())
        {
            return;
        }

        if (e.IsEndRace && !IsExplore())
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

        if (e.IsEndLap)
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 1, 1);
        }
        else
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 0, 1);
        }

        if (IndependantLaps)
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(e.LapTime));
        }
        else
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(e.RaceTime));
        }

        if (!IndependantLaps && e.CheckpointInRace > e.CheckpointInLap)
        {
            LabelLapTime.SetText(TimeToTextWithMilli(e.LapTime));
            FrameLap.Show();
        }
        else
        {
            FrameLap.Hide();
        }

        var framePb = (FrameDifferences.Controls[0] as CMlFrame)!;

        if (IsFirstFinish || e.Player.Score.BestRace.Time == -1)
        {
            framePb.Hide();
        }
        else
        {
            var frameTime = (framePb.GetFirstChild("FrameTime") as CMlFrame)!;
            var labelTime = (frameTime.GetFirstChild("LabelTime") as CMlLabel)!;
            var quadColor = ((frameTime.GetFirstChild("FrameBackground") as CMlFrame)!.GetFirstChild("QuadColor") as CMlQuad)!;

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

            framePb.Show();
        }
    }

    public void Main()
    {
        FrameCheckpoint.Hide();

        foreach (var control in FrameDifferences.Controls)
        {
            control.Hide();
        }

        Wait(() => GetPlayer() is not null && GetPlayer().Score is not null);
    }

    public void Loop()
    {
        if (GetPlayer().Score.BestRace.Time != PrevBestTime)
        {
            if (PrevBestTime == -1)
            {
                IsFirstFinish = !IndependantLaps;
            }
            PrevBestTime = GetPlayer().Score.BestRace.Time;
        }

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
}
