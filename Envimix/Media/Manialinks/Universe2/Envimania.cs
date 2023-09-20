using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Envimania : CTmMlScriptIngame, IContext
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

    [ManialinkControl] public required CMlFrame FrameEnvimania;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaTitleBg;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaRecordsBg;
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
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    public void Main()
    {
        FrameEnvimania.Visible = IsVisible();
        PreviousVisible = FrameEnvimania.Visible;

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        FrameEnvimania.Visible = IsVisible();

        if (FrameEnvimania.Visible != PreviousVisible)
        {
            if (FrameEnvimania.Visible)
            {
                foreach (var control in FrameEnvimaniaTitleBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 8\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                foreach (var control in FrameEnvimaniaRecordsBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 82.5\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                VisibleTime = Now;
            }

            PreviousVisible = FrameEnvimania.Visible;
        }

        if (VisibleTime == -1)
        {
            FrameLabelCar.ClipWindowSize.X = 45;
            FrameRecords.ClipWindowSize.X = 45;
        }
        else
        {
            FrameLabelCar.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 400);
            FrameRecords.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 500);
        }
    }
}
