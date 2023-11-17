namespace Envimix.Media.Manialinks.Universe2;

public class SpectatorEffect : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameSpectatorEffect;

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed && IsSpectatorClient;
    }

    public void Main()
    {
        FrameSpectatorEffect.Visible = IsVisible();
    }

    public void Loop()
    {
        FrameSpectatorEffect.Visible = IsVisible();
    }
}
