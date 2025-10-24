namespace Envimix.Media.Manialinks;

public class Release : CManiaAppTitleLayer, IContext
{
    [ManialinkControl] public required CMlLabel LabelDelta;
    [ManialinkControl] public required CMlLabel LabelDate;
    [ManialinkControl] public required CMlQuad QuadBg;

    [Local(LocalFor.LocalUser)] public required string TitleRelease { get; set; }

    public bool Begins;

    public Release()
    {
        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "Show":
                    AnimMgr.Add(LabelDelta, "<label opacity=\"1\" hidden=\"0\"/>", Now, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(LabelDate, "<label opacity=\"1\" hidden=\"0\"/>", Now + 250, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(QuadBg, "<quad opacity=\"0.9\" hidden=\"0\"/>", Now, 1000, CAnimManager.EAnimManagerEasing.QuadOut);
                    break;
                case "Hide":
                    AnimMgr.Add(LabelDelta, "<label opacity=\"0\" hidden=\"1\"/>", Now, 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(LabelDate, "<label opacity=\"0\" hidden=\"1\"/>", Now + 100, 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    AnimMgr.Add(QuadBg, "<quad opacity=\"0\" hidden=\"1\"/>", Now, 500, CAnimManager.EAnimManagerEasing.QuadOut);
                    break;
            }
        };
    }

    public void Main()
    {
        LabelDelta.Opacity = 0;
        LabelDelta.Hide();
        LabelDate.Opacity = 0;
        LabelDate.Hide();
        QuadBg.Opacity = 0;
        QuadBg.Hide();
    }

    public void Loop()
    {
        if (Begins)
        {
            return;
        }

        if (TitleRelease == "")
        {
            LabelDelta.SetText("COMING SOON");
            LabelDate.SetText("EXACT DATE TO BE YET ANNOUNCED");
            return;
        }

        var delta = TimeLib.GetDelta(TimeLib.GetCurrent(), TitleRelease);

        if (delta >= 0)
        {
            LabelDelta.SetText("LET'S BEGIN!");
            Begins = true;
        }
        else
        {
            LabelDelta.SetText(TimeLib.FormatDelta(TimeLib.GetCurrent(), TitleRelease, TimeLib.EDurationFormats.Full));
        }

        LabelDate.SetText(TimeLib.FormatDate(TitleRelease, TimeLib.EDateFormats.Full));
    }
}
