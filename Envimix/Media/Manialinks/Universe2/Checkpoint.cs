namespace Envimix.Media.Manialinks.Universe2;

public class Checkpoint : CTmMlScriptIngame, IContext
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

    public Checkpoint()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    Waypoint(e);
                    break;
            }
        };
    }

    public void Main()
    {
        FrameCheckpoint.Hide();

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (CheckpointShowTime != -1)
        {
            var time = Now - CheckpointShowTime;

            if (time > 2700)
            {
                FrameInnerCheckpointTime.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2700, 40, -40, 300);

                foreach (var control in (FrameCheckpointTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                {
                    control.Size.X = AnimLib.EaseOutQuad(time - 2700, 40, -40, 300);
                }

                foreach (var control in (FrameLap.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
                {
                    control.Size.X = AnimLib.EaseOutQuad(time - 2800, 20, -20, 300);
                }

                FrameLap.ClipWindowSize.X = AnimLib.EaseOutQuad(time - 2800, 20, -20, 300);

                for (var i = 0; i < FrameDifferences.Controls.Count; i++)
                {
                    var frame = (FrameDifferences.Controls[i] as CMlFrame)!;

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

                if (time > 3600)
                {
                    FrameCheckpoint.Hide();
                    CheckpointShowTime = -1;
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

    private static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    private void Waypoint(CTmRaceClientEvent e)
    {
        if (e.Player.User.Login != GetPlayer().User.Login)
        {
            return;
        }

        var rank = TextLib.GetTranslatedText("1st");

        if (IndependantLaps)
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(e.LapTime));
        }
        else
        {
            LabelCheckpointTime.SetText(TimeToTextWithMilli(e.RaceTime));
        }

        if (e.IsEndLap)
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 1, 1);
        }
        else
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Checkpoint, 0, 1);
        }

        FrameCheckpoint.Show();

        foreach (var control in (FrameCheckpointTime.GetFirstChild("FrameBackground") as CMlFrame)!.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, "<quad size=\"40 9\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        FrameInnerCheckpointTime.ClipWindowSize.X = 0;
        CheckpointShowTime = Now;

        if (e.RaceTime != e.LapTime && !IndependantLaps)
        {
            LabelLapTime.SetText(TimeToTextWithMilli(e.LapTime));
            FrameLap.Show();
        }
        else
        {
            FrameLap.Hide();
        }

        if (e.Player.Score.BestRace.Checkpoints.Count <= 0)
        {
            FrameDifferences.Hide();
            return;
        }

        var frameTime = ((FrameDifferences.Controls[0] as CMlFrame)!.GetFirstChild("FrameTime") as CMlFrame)!;
        var labelTime = (frameTime.GetFirstChild("LabelTime") as CMlLabel)!;
        var quadColor = ((frameTime.GetFirstChild("FrameBackground") as CMlFrame)!.GetFirstChild("QuadColor") as CMlQuad)!;

        int difference;
        if (IndependantLaps)
        {
            difference = e.LapTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInLap]; // Time difference
        }
        else
        {
            difference = e.RaceTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInRace]; // Time difference
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

        FrameDifferences.Show();
    }
}
