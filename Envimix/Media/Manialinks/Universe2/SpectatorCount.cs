namespace Envimix.Media.Manialinks.Universe2;

public class SpectatorCount : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameSpectatorCount;

    public bool PreviousIsVisible;

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

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    public void Main()
    {
        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        var specCount = 0;

        foreach (var player in Players)
        {
            if (player == GUIPlayer)
            {
                specCount += 1;
            }
        }

        if (IsVisible() != PreviousIsVisible)
        {
            FrameSpectatorCount.Visible = IsVisible();

            if (IsVisible())
            {
                FrameSpectatorCount.RelativePosition_V3.Y = -102;

                AnimMgr.Add(FrameSpectatorCount, "<frame pos=\"92.5 -88\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousIsVisible = IsVisible();
        }
    }
}
