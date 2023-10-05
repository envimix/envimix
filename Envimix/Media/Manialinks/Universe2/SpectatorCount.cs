namespace Envimix.Media.Manialinks.Universe2;

public class SpectatorCount : CTmMlScriptIngame, IContext
{
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
        
    }
}
