namespace Envimix.Media.Manialinks.Universe2;

public class SpectatorInfo : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameSpectatorInfo;
    [ManialinkControl] public required CMlQuad QuadSpectatorAvatar;
    [ManialinkControl] public required CMlLabel LabelSpectatorName;

    public bool PreviousIsVisible = true;
    public string PrevPlayerLogin;

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed && IsSpectatorClient && GetPlayer().User.Login != LocalUser.Login;
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    public void Main()
    {
        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (GetPlayer().User.Login != PrevPlayerLogin)
        {
            QuadSpectatorAvatar.ChangeImageUrl(GetPlayer().User.AvatarUrl);
            LabelSpectatorName.SetText(GetPlayer().User.Name);
            PrevPlayerLogin = GetPlayer().User.Login;
        }

        if (IsVisible() != PreviousIsVisible)
        {
            FrameSpectatorInfo.Visible = IsVisible();

            if (IsVisible())
            {
                FrameSpectatorInfo.RelativePosition_V3.Y = -102;

                AnimMgr.Flush(FrameSpectatorInfo);
                AnimMgr.Add(FrameSpectatorInfo, "<frame pos=\"85 -88\"/>", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousIsVisible = IsVisible();
        }
    }
}
