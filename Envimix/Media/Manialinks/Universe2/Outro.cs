namespace Envimix.Media.Manialinks.Universe2;

public class Outro : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlQuad QuadContinue;

    public Outro()
    {
        QuadContinue.MouseClick += () =>
        {
            Continue();
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Select:
                    Continue();
                    break;
            }
        };
    }

    public void Main()
    {
        EnableMenuNavigationInputs = true;
    }

    public void Loop()
    {
        
    }

    private void Continue()
    {
        SendCustomEvent("OutroContinue", new[] { "" });
    }
}
