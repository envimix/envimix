namespace Envimix.Media.Manialinks.Universe2;

public class TimeLimit : CTmMlScriptIngame, IContext
{
    public int Start;
    public bool PreviousIsVisible;
    public int PreviousCutOffTimeLimit;

    [ManialinkControl] public required CMlFrame FrameTimeLimit;
    [ManialinkControl] public required CMlQuad QuadMode;
    [ManialinkControl] public required CMlFrame FrameTimeLimitLabel;
    [ManialinkControl] public required CMlLabel LabelTimeLimit;

    [Netread] public bool CarSelectionMode { get; }
    [Netread] public int CurrentWarmUpNb { get; }
    [Netread] public int CutOffTimeLimit { get; }

    private CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    private bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    public void Main()
    {
        Start = Now;

        FrameTimeLimit.Visible = IsVisible();
        PreviousIsVisible = IsVisible();
        PreviousCutOffTimeLimit = CutOffTimeLimit;

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                var frame = (FrameTimeLimit.Controls[0] as CMlFrame)!;
                frame.Controls[0].Size.X = 0;
                frame.Controls[1].Size.X = 0;
                frame.Controls[2].Size.X = 0;
                LabelTimeLimit.Opacity = 0;

                AnimMgr.Add(frame.Controls[0], "<quad size=\"0 10\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[1], "<quad size=\"0 10\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[2], "<quad size=\"0 8.5\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(LabelTimeLimit, "<label opacity=\"0\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[0], "<quad size=\"52.5 10\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[1], "<quad size=\"52.5 10\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[2], "<quad size=\"50 8.5\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(LabelTimeLimit, "<label opacity=\"1\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousIsVisible = IsVisible();
        }

        if (CurrentWarmUpNb > 0)
        {
            QuadMode.Colorize = ColorLib.HexToRgb("FC6");
            QuadMode.Show();
        }
        else if (CarSelectionMode)
        {
            QuadMode.Colorize = ColorLib.HexToRgb("6F6");
            QuadMode.Show();
        }
        else
        {
            QuadMode.Hide();
        }

        if (CutOffTimeLimit != PreviousCutOffTimeLimit)
        {
            PreviousCutOffTimeLimit = CutOffTimeLimit;
            LabelTimeLimit.Opacity = 0;
            AnimMgr.Add(LabelTimeLimit, "<label opacity=\"1\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        FrameTimeLimit.Visible = IsVisible();

        FrameTimeLimitLabel.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - Start - 100, _Base: 0, _Change: 40, _Duration: 300);

        var timeLeft = CutOffTimeLimit - GameTime;
        var lastMinute = (60000 - timeLeft) / 60000f;

        if (60000 - timeLeft < 0)
        {
            lastMinute = 1;
        }

        if (CutOffTimeLimit <= 0)
        {
            LabelTimeLimit.Value = "-:--";
        }
        else if (timeLeft + 1 < 0)
        {
            LabelTimeLimit.Value = "0:00";
        }
        else
        {
            LabelTimeLimit.Value = TextLib.TimeToText(timeLeft + 1);
        }

        if (timeLeft >= 0 && timeLeft < 60000)
        {
            LabelTimeLimit.RelativeScale = (MathLib.Sin(Now / 1000f * MathLib.PI() * 2 * 1.5f - MathLib.PI() / 2) + 1) / 2 * lastMinute * 0.1f + 1;
        }
        else
        {
            LabelTimeLimit.RelativeScale = 1;
        }
    }
}
