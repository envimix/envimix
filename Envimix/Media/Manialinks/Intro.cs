namespace Envimix.Media.Manialinks;

public class Intro : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlQuad QuadTitleLogo;
    [ManialinkControl] public required CMlQuad QuadTitleLogoShadow;

    public int StartedAt;

    [Local(LocalFor.LocalUser)] public bool IntroEnded { get; set; }

    public void Main()
    {
        IntroEnded = false;
        StartedAt = Now;

        QuadTitleLogo.Opacity = 0;
        QuadTitleLogo.RelativeScale = 0;
        QuadTitleLogoShadow.Opacity = 0;
        QuadTitleLogoShadow.RelativeScale = 0;
        AnimMgr.Add(QuadTitleLogo, "<quad scale=\"0.9\" hidden=\"0\" opacity=\"1\"/>", Now, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadTitleLogo, "<quad scale=\"0.97\"/>", Now + 1000, 1000, CAnimManager.EAnimManagerEasing.Linear);
        AnimMgr.Add(QuadTitleLogo, "<quad scale=\"1\" hidden=\"1\" opacity=\"0\"/>", Now + 2000, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadTitleLogoShadow, "<quad scale=\"0.9\" hidden=\"0\" opacity=\"1\"/>", Now, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadTitleLogoShadow, "<quad scale=\"0.97\"/>", Now + 1000, 1000, CAnimManager.EAnimManagerEasing.Linear);
        AnimMgr.Add(QuadTitleLogoShadow, "<quad scale=\"1\" hidden=\"1\" opacity=\"0\"/>", Now + 2000, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    public void Loop()
    {
        if (StartedAt != -1 && Now - StartedAt > 2000)
        {
            IntroEnded = true;
            StartedAt = -1;
        }
    }
}
