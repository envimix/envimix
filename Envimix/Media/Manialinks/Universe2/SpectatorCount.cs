namespace Envimix.Media.Manialinks.Universe2;

public class SpectatorCount : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameSpectatorCount;
    [ManialinkControl] public required CMlLabel LabelSpectatorCount;
    [ManialinkControl] public required CMlFrame FrameSpectatorList;
    [ManialinkControl] public required CMlQuad QuadSpectatorList;
    [ManialinkControl] public required CMlLabel LabelSpectatorList;

    public bool PreviousIsVisible = true; // Hack
    public string PrevSpectatedPlayerLogin;
    public int CurrentSpectatorCount;
    public bool ListShown = false;

    [Netwrite(NetFor.UI)] public string SpectatorTarget { get; set; }

    [Netread] public Dictionary<string, Dictionary<string, string>> SpectatorLists { get; }

    public SpectatorCount()
    {
        LabelSpectatorCount.MouseOver += LabelSpectatorCount_MouseOver;
        LabelSpectatorCount.MouseOut += LabelSpectatorCount_MouseOut;
    }

    private void LabelSpectatorCount_MouseOver()
    {
        AnimMgr.Add(LabelSpectatorCount, "<label scale=\"1.1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        //AnimMgr.Add(FrameSpectatorList, "<frame pos=\"0 10.5\" hidden=\"0\">", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        FrameSpectatorList.RelativePosition_V3.Y = 10.5f;
        FrameSpectatorList.Show();
        ListShown = true;
    }

    private void LabelSpectatorCount_MouseOut()
    {
        AnimMgr.Add(LabelSpectatorCount, "<label scale=\"1\"/>", 100, CAnimManager.EAnimManagerEasing.QuadOut);
        //AnimMgr.Add(FrameSpectatorList, "<frame pos=\"0 -20\" hidden=\"1\">", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        FrameSpectatorList.RelativePosition_V3.Y = -20;
        FrameSpectatorList.Hide();
        ListShown = false;
    }

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
        return !IsInGameMenuDisplayed && CurrentSpectatorCount > 0;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    public void Main()
    {
        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        SpectatorTarget = "";

        if (GUIPlayer is not null)
        {
            SpectatorTarget = GUIPlayer.User.Login;
        }

        if (SpectatorLists.ContainsKey(GetPlayer().User.Login))
        {
            CurrentSpectatorCount = SpectatorLists[GetPlayer().User.Login].Count;

            if (ListShown)
            {
                LabelSpectatorList.Value = "";

                var first = true;

                foreach (var (spectator, nickname) in SpectatorLists[GetPlayer().User.Login])
                {
                    if (first)
                    {
                        first = false;
                        LabelSpectatorList.Value = $"$<{nickname}$>";
                    }
                    else
                    {
                        LabelSpectatorList.Value = $"{LabelSpectatorList.Value}\n$<{nickname}$>";
                    }
                }

                QuadSpectatorList.Size.Y = CurrentSpectatorCount * 5f + 2.5f;
            }
        }
        else
        {
            CurrentSpectatorCount = 0;
        }

        LabelSpectatorCount.SetText(CurrentSpectatorCount.ToString());

        if (IsVisible() != PreviousIsVisible)
        {
            FrameSpectatorCount.Visible = IsVisible();

            if (IsVisible())
            {
                FrameSpectatorCount.RelativePosition_V3.Y = -102;

                AnimMgr.Add(FrameSpectatorCount, "<frame pos=\"75 -88\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousIsVisible = IsVisible();
        }
    }
}
