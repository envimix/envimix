namespace Envimix.Media.Manialinks.Universe2;

public class Notice : CTmMlScriptIngame, IContext
{
    public string PreviousMessage = "";

    [ManialinkControl] public required CMlFrame FrameNotice;
    [ManialinkControl] public required CMlLabel LabelNotice;

    [Netwrite(NetFor.UI)] public required bool ScoreTableIsVisible { get; set; }

    [Netread(NetFor.UI)] public required string NoticeMessage { get; init; }

    bool IsVisible()
    {
        return !ScoreTableIsVisible;
    }

    public void Main()
    {
        FrameNotice.Visible = IsVisible();
        PreviousMessage = NoticeMessage;
    }

    public void Loop()
    {
        if (NoticeMessage != PreviousMessage)
        {
            if (NoticeMessage != "")
            {
                FrameNotice.Controls[0].Size.X = 0;
                FrameNotice.Controls[1].Size.X = 0;
                LabelNotice.Opacity = 0;
                AnimMgr.Add(FrameNotice.Controls[0], "<quad size=\"110 12\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameNotice.Controls[1], "<quad size=\"110 12\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(LabelNotice, "<label opacity=\"1\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousMessage = NoticeMessage;
        }

        if (NoticeMessage == "")
        {
            FrameNotice.Hide();
        }
        else
        {
            LabelNotice.Value = NoticeMessage;
            FrameNotice.Visible = IsVisible();
        }
    }
}
