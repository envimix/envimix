namespace Envimix.Media.Manialinks.Universe2;

public class Score : CTmMlScriptIngame, IContext
{
    public bool PreviousIsVisible;
    public int VisibleTime = -1;

    [ManialinkControl] public required CMlFrame FrameScore;
    [ManialinkControl] public required CMlFrame FrameInnerScore;
    [ManialinkControl] public required CMlLabel LabelBestTime;
    [ManialinkControl] public required CMlLabel LabelLastTime;

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

        return !IsInGameMenuDisplayed;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
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
        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                var frame = (FrameScore.Controls[0] as CMlFrame)!;
                frame.Controls[0].Size.X = 0;
                frame.Controls[1].Size.X = 0;

                AnimMgr.Add(frame.Controls[0], "<quad size=\"0 12.5\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[1], "<quad size=\"0 12.5\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[0], "<quad size=\"42.5 12.5\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[1], "<quad size=\"42.5 12.5\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

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
    }
}
