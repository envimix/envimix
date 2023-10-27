namespace Envimix.Media.Manialinks;

public class MainMenu : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlLabel LabelPlay4;
    [ManialinkControl] public required CMlLabel LabelExplore4;
    [ManialinkControl] public required CMlLabel LabelPlay7;
    [ManialinkControl] public required CMlLabel LabelExplore7;
    [ManialinkControl] public required CMlLabel LabelPlay10;
    [ManialinkControl] public required CMlLabel LabelExplore10;
    [ManialinkControl] public required CMlLabel LabelSolo;
    [ManialinkControl] public required CMlLabel LabelLocal;
    [ManialinkControl] public required CMlLabel LabelInternet;
    [ManialinkControl] public required CMlLabel LabelEditor;
    [ManialinkControl] public required CMlLabel LabelProfile;
    [ManialinkControl] public required CMlLabel LabelBuildVersion;

    public MainMenu()
    {
        LabelPlay4.MouseClick += () =>
        {
            ParentApp.TitleControl.PlayMap("4-Converted.Map.Gbx", "", "");
        };

        LabelExplore4.MouseClick += () =>
        {
            ParentApp.TitleControl.EditNewMapFromBaseMap("4-Converted.Map.Gbx", ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "", "");
        };

        LabelPlay7.MouseClick += () =>
        {
            ParentApp.TitleControl.PlayMap("7-Converted.Map.Gbx", "", "");
        };

        LabelExplore7.MouseClick += () =>
        {
            ParentApp.TitleControl.EditNewMapFromBaseMap("7-Converted.Map.Gbx", ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "", "");
        };

        LabelPlay10.MouseClick += () =>
        {
            ParentApp.TitleControl.PlayMap("10-Converted.Map.Gbx", "", "");
        };

        LabelExplore10.MouseClick += () =>
        {
            ParentApp.TitleControl.EditNewMapFromBaseMap("10-Converted.Map.Gbx", ModNameOrUrl: "", PlayerModel: "", "EnvimixExplore.Script.txt", "", "");
        };


        LabelSolo.MouseClick += () =>
        {
            ParentApp.Menu_Solo();
        };

        LabelLocal.MouseClick += () =>
        {
            ParentApp.Menu_Local();
        };

        LabelInternet.MouseClick += () =>
        {
            ParentApp.Menu_Internet();
        };

        LabelEditor.MouseClick += () =>
        {
            ParentApp.Menu_Editor();
        };

        LabelProfile.MouseClick += () =>
        {
            ParentApp.Menu_Profile();
        };
    }

    public void Main()
    {
        LabelBuildVersion.SetText(LoadedTitle.TitleVersion);
    }

    public void Loop()
    {

    }
}
