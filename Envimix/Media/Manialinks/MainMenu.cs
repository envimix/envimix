namespace Envimix.Media.Manialinks;

public class MainMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadSolo;
    [ManialinkControl] public required CMlQuad QuadLocal;
    [ManialinkControl] public required CMlQuad QuadInternet;
    [ManialinkControl] public required CMlQuad QuadEditor;
    [ManialinkControl] public required CMlQuad QuadQuit;
    [ManialinkControl] public required CMlFrame FrameMainMenu;

    public MainMenu()
    {
        QuadSolo.MouseClick += () =>
        {
            SendCustomEvent("MenuSolo", new[] {""});
        };

        QuadLocal.MouseClick += () =>
        {
            ParentApp.Menu_Local();
        };

        QuadInternet.MouseClick += () =>
        {
            ParentApp.Menu_Internet();
        };

        QuadEditor.MouseClick += () =>
        {
            ParentApp.Menu_Editor();
        };

        QuadQuit.MouseClick += () =>
        {
            ParentApp.Menu_Quit();
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    ShowMenuFrame();
                    break;
                case "AnimateClose":
                    HideMenuFrame();
                    break;
            }
        };
    }

    public void Main()
    {
        
    }

    public void Loop()
    {

    }

    private void ShowMenuFrame()
    {
        AnimMgr.Add(FrameMainMenu, "<frame pos=\"90 75\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideMenuFrame()
    {
        AnimMgr.Add(FrameMainMenu, "<frame pos=\"210 75\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }
}
