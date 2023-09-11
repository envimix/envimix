namespace Envimix.Media.Manialinks.Universe2;

public class Rating : CTmMlScriptIngame, IContext
{
    public bool PreviousVisible;
    public int VisibleTime = -1;

    [ManialinkControl] public required CMlFrame FrameRating;
    [ManialinkControl] public required CMlGauge GaugeDifficulty;
    [ManialinkControl] public required CMlGauge GaugeQuality;
    [ManialinkControl] public required CMlLabel LabelDifficulty;
    [ManialinkControl] public required CMlLabel LabelQuality;

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    public void Main()
    {
        FrameRating.Visible = IsVisible();
        PreviousVisible = FrameRating.Visible;

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        FrameRating.Visible = IsVisible();

        if (FrameRating.Visible != PreviousVisible)
        {
            if (FrameRating.Visible)
            {
                (FrameRating.Controls[0] as CMlQuad)!.Size.X = 0;
                (FrameRating.Controls[1] as CMlQuad)!.Size.X = 0;
                AnimMgr.Add(FrameRating.Controls[0], "<quad size=\"42.5 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameRating.Controls[1], "<quad size=\"42.5 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                GaugeDifficulty.Size.X = 0;
                AnimMgr.Add(GaugeDifficulty, "<gauge size=\"0 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(GaugeDifficulty, "<gauge size=\"27 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                GaugeQuality.Size.X = 0;
                AnimMgr.Add(GaugeQuality, "<gauge size=\"0 7\"/>", 350, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(GaugeQuality, "<gauge size=\"27 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                LabelDifficulty.RelativePosition_V3.X = 40;
                LabelQuality.RelativePosition_V3.X = 40;
                AnimMgr.Add(LabelDifficulty, "<label pos=\"23.5 2\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(LabelQuality, "<label pos=\"23.5 -1.7\"/>", 350, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousVisible = FrameRating.Visible;
        }
    }
}
