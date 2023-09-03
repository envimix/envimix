namespace Envimix.Media.Manialinks.Universe2;

public class PrePostLoading : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameTop;
    [ManialinkControl] public required CMlFrame FrameBottom;
    [ManialinkControl] public required CMlQuad QuadTopShadow;
    [ManialinkControl] public required CMlQuad QuadBottomShadow;

    public int PrevPrepareLoading;
    public int CloseAnimation = -1;
    public int OpenAnimation;

    [Netread(NetFor.UI)] public required int PrepareLoading { get; set; }

    public void Main()
    {
        PrevPrepareLoading = PrepareLoading;
        OpenAnimation = GameTime;
    }

    public void Loop()
    {
        if (PrepareLoading != PrevPrepareLoading)
        {
            if (PrepareLoading == -1)
            {
                CloseAnimation = -1;
                OpenAnimation = GameTime;
            }
            else
            {
                CloseAnimation = GameTime;
                OpenAnimation = -1;
            }

            PrevPrepareLoading = PrepareLoading;
        }

        /*if(Net_UnprepareLoading != PrevUnprepareLoading) {
            if(Net_UnprepareLoading == -1)
                OpenAnimation = -1;
            else
                OpenAnimation = Now;
            PrevUnprepareLoading = Net_UnprepareLoading;
        }*/

        if (FrameTop.RelativePosition_V3.Y == 0 && FrameBottom.RelativePosition_V3.Y == 0)
        {
            QuadTopShadow.Hide();
            QuadBottomShadow.Hide();
        }
        else
        {
            QuadTopShadow.Show();
            QuadBottomShadow.Show();
        }

        if (CloseAnimation == -1)
        {
            if (OpenAnimation != -1 && GameTime - OpenAnimation > 2000)
            {
                FrameTop.RelativePosition_V3.Y = 100;
                FrameBottom.RelativePosition_V3.Y = -100;
            }
            else if (OpenAnimation != -1)
            {
                FrameTop.RelativePosition_V3.Y = AnimLib.EaseInQuad(GameTime - OpenAnimation - 1000, _Base: 0, _Change: 100, _Duration: 1000);
                FrameBottom.RelativePosition_V3.Y = AnimLib.EaseInQuad(GameTime - OpenAnimation - 1000, _Base: 0, _Change: -100, _Duration: 1000);
            }
        }
        else
        {
            FrameTop.RelativePosition_V3.Y = AnimLib.EaseInQuad(GameTime - CloseAnimation, _Base: 100, _Change: -100, _Duration: 1000);
            FrameBottom.RelativePosition_V3.Y = AnimLib.EaseInQuad(GameTime - CloseAnimation, _Base: -100, _Change: 100, _Duration: 1000);
        }
    }
}
