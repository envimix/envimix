namespace Envimix.Media.Manialinks.Universe2;

public class MenuFake : CMlScriptIngame, IContext
{
    public void Main()
    {

    }

    public void Loop()
    {
        CloseInGameMenu(EInGameMenuResult.Resume);
    }
}
