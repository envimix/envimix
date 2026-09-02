namespace Envimix.Media.Manialinks.Universe2;

public class TimeLimit : CMlScriptIngame, IContext
{
    public int Start;
    public int StartVote;
    public bool PreviousIsVisible;
    public int PreviousCutOffTimeLimit;
    public string PreviousVoteType;

    [ManialinkControl] public required CMlFrame FrameTimeLimit;
    [ManialinkControl] public required CMlQuad QuadMode;
    [ManialinkControl] public required CMlFrame FrameTimeLimitLabel;
    [ManialinkControl] public required CMlLabel LabelTimeLimit;
    [ManialinkControl] public required CMlFrame FrameVote;
    [ManialinkControl] public required CMlLabel LabelVote;
    [ManialinkControl] public required CMlLabel LabelVoteYes;
    [ManialinkControl] public required CMlLabel LabelVoteNo;
    [ManialinkControl] public required CMlQuad QuadYes;
    [ManialinkControl] public required CMlQuad QuadNo;
    [ManialinkControl] public required CMlQuad QuadExtend;

    [Netread] public bool CarSelectionMode { get; }
    [Netread] public int CurrentWarmUpNb { get; }
    [Netread] public int CutOffTimeLimit { get; }
    [Netread] public string VoteType { get; }
    [Netread] public int VoteYes { get; }
    [Netread] public int VoteNo { get; }

    private bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    public TimeLimit()
    {
        QuadExtend.MouseClick += () =>
        {
            SendCustomEvent("Extend", new[]{ "" });
        };
        QuadYes.MouseClick += () =>
        {
            SendCustomEvent("Extend", new[]{ "Yes" });
        };
        QuadNo.MouseClick += () =>
        {
            SendCustomEvent("Extend", new[]{ "No" });
        };
    }

    private void ShowVote()
    {
        FrameVote.Show();
        FrameVote.RelativePosition_V3.X = 25;
        AnimMgr.Add(FrameVote, "<frame pos=\"0 0\"/>", Duration: 800, CAnimManager.EAnimManagerEasing.QuadOut);
        LabelVote.Opacity = 0;
        LabelVoteYes.Opacity = 0;
        LabelVoteNo.Opacity = 0;

        LabelVote.Show();
        LabelVoteYes.Show();
        LabelVoteNo.Show();

        StartVote = Now;

        if (VoteType == "Extend")
        {
            LabelVote.Value = "Extend?";
        }
    }

    private void HideVote()
    {
        FrameVote.Hide();
        LabelVote.Hide();
        LabelVoteYes.Hide();
        LabelVoteNo.Hide();
    }

    public void Main()
    {
        Start = Now;
        StartVote = Now;

        FrameTimeLimit.Visible = IsVisible();
        PreviousIsVisible = IsVisible();
        PreviousCutOffTimeLimit = CutOffTimeLimit;
        PreviousVoteType = VoteType;

        if (VoteType == "")
        {
            HideVote();
        }
        else
        {
            ShowVote();
        }
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
                AnimMgr.AddChain(frame.Controls[0], "<quad size=\"42.5 10\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[1], "<quad size=\"42 9.75\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(LabelTimeLimit, "<label opacity=\"1\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadIn);
                Start = Now;

                if (VoteType != "")
                {
                    ShowVote();
                }
            }

            PreviousIsVisible = IsVisible();
        }

        if (VoteType != PreviousVoteType)
        {
            if (VoteType == "")
            {
                HideVote();
            }
            else
            {
                ShowVote();
            }
            PreviousVoteType = VoteType;
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
        if (LabelVote.Visible || LabelVoteYes.Visible || LabelVoteNo.Visible)
        {
            var voteLabelOpacity = AnimLib.EaseOutQuad(Now - StartVote - 700, _Base: 0, _Change: 1, _Duration: 200);
            var flash = (MathLib.Sin(Now / 1000f * MathLib.PI() * 2) + 1) / 4 + 0.5f;
            LabelVote.Opacity = voteLabelOpacity * flash;
            LabelVote.RelativeScale = flash * 0.25f + 0.75f;
            LabelVoteYes.Opacity = voteLabelOpacity;
            LabelVoteNo.Opacity = voteLabelOpacity;
            LabelVoteYes.Value = VoteYes.ToString();
            LabelVoteNo.Value = VoteNo.ToString();
        }

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
