namespace Envimix.Media.Manialinks;

public class MainMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadSolo;
    [ManialinkControl] public required CMlQuad QuadLocal;
    [ManialinkControl] public required CMlQuad QuadInternet;
    [ManialinkControl] public required CMlQuad QuadEditor;
    [ManialinkControl] public required CMlQuad QuadQuit;
    [ManialinkControl] public required CMlFrame FrameMainMenu;
    [ManialinkControl] public required CMlLabel LabelBuild;

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
                    EnableMenuNavigationInputs = true;
                    ShowMenuFrame();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    HideMenuFrame();
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("Quit", new[] { "" });
                    break;
            }
        };
    }

    public void Main()
    {
        FrameMainMenu.RelativePosition_V3.X = 210;
        ShowMenuFrame();

        LabelBuild.SetText(TextLib.Split(" ", LoadedTitle.TitleVersion)[0]);
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
