namespace Envimix.Scripts;

public class MainMenu : CManiaAppTitle, IContext
{
    public void Main()
    {
        var layer = UILayerCreate();
        layer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";
    }

    public void Loop()
    {

    }
}
