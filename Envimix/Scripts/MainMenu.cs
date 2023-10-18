using System.Collections.Immutable;

namespace Envimix.Scripts;

public class MainMenu : CManiaAppTitle, IContext
{
    public void Main()
    {        
        ImmutableArray<string> allowedLogins = new()
        {
            "bigbang1112",
            "linuxcat",
            "pekatour",
            "adamkooo",
            "poutrel",
            "riolu",
            "eyohna",
            "dragontm",
            "mystixor",
            "spookykoala2113",
            "ajsasflaym",
            "arkes910",
            "ydoowoody",
            "hot_wheeler",
            "totokill13",
            "tushy444trackmaniagamer",
            "naruto42",
            "burpman176",
            "marosis99"
        };

        if (!allowedLogins.Contains(LocalUser.Login))
        {
            while (true)
            {
                Menu_Quit();
                Yield();
            }
        }

        var layer = UILayerCreate();
        layer.ManialinkPage = "file://Media/Manialinks/MainMenu.xml";
    }

    public void Loop()
    {

    }
}
