namespace Envimix.Media.Manialinks.Universe2;

public class Multilap : CTmMlScriptIngame, IContext
{
    public int Start;
    public bool PreviousIsVisible;

    [ManialinkControl] public required CMlFrame FrameMultilap;
    [ManialinkControl] public required CMlQuad QuadMode;
    [ManialinkControl] public required CMlFrame FrameMultilapLabel;
    [ManialinkControl] public required CMlLabel LabelMultilap;

    [Netread] public int FinishedAt { get; set; }
    [Netread] public bool Outro { get; set; }

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    bool IsVisible()
    {
        if (IsExplore())
        {
            return false;
        }

        return !IsInGameMenuDisplayed && MapIsLapRace && FinishedAt == -1 && !Outro;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public void Main()
    {
        Start = Now;

        FrameMultilap.Visible = IsVisible();
        PreviousIsVisible = IsVisible();

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                var frame = (FrameMultilap.Controls[0] as CMlFrame)!;
                frame.Controls[0].Size.X = 0;
                frame.Controls[1].Size.X = 0;
                frame.Controls[2].Size.X = 0;
                LabelMultilap.Opacity = 0;

                AnimMgr.Add(frame.Controls[0], "<quad size=\"0 10\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[1], "<quad size=\"0 10\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(frame.Controls[2], "<quad size=\"0 8.5\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(LabelMultilap, "<label opacity=\"0\"/>", Duration: 100, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[0], "<quad size=\"42.5 10\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(frame.Controls[1], "<quad size=\"42 9.75\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(LabelMultilap, "<label opacity=\"1\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadIn);
            }

            PreviousIsVisible = IsVisible();
        }

        FrameMultilap.Visible = IsVisible();

        FrameMultilapLabel.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - Start - 100, _Base: 0, _Change: 40, _Duration: 300);

        if (GetPlayer().CurrentNbLaps >= NbLaps)
        {
            LabelMultilap.Value = $"🔁  {GetPlayer().CurrentNbLaps} / {NbLaps}";
        }
        else
        {
            LabelMultilap.Value = $"🔁  {GetPlayer().CurrentNbLaps + 1} / {NbLaps}";
        }
    }
}
