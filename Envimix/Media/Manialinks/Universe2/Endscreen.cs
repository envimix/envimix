namespace Envimix.Media.Manialinks.Universe2;

public class Endscreen : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameEndscreenInfo;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlQuad QuadContinue;
    [ManialinkControl] public required CMlFrame FrameTime;
    [ManialinkControl] public required CMlFrame FrameScore;
    [ManialinkControl] public required CMlFrame FrameLeaderboard;
    [ManialinkControl] public required CMlFrame FrameContinue;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelAuthor;
    [ManialinkControl] public required CMlLabel LabelEnvironment;
    [ManialinkControl] public required CMlQuad QuadEnvironment;
    [ManialinkControl] public required CMlLabel LabelTime;

    public int FinishedAt;
    public string PreviousCar = "";

    public Endscreen()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace && !IsExplore())
                    {
                        FinishedAt = Now;
                        LabelTime.SetText(TimeToTextWithMilli(e.RaceTime));
                        ShowEndscreen();
                        
                        // show full endscreen in 2 seconds, hide the other UI after the 2 seconds, move blur background from bottom to top for interesting effect
                    }
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    break;
            }
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

        QuadContinue.MouseClick += () =>
        {
            Continue();
        };
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
        return true;
    }

    bool IsExplore()
    {
        return CurrentServerModeName is "";
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
        HideEndscreen();
        FrameEndscreenInfo.Hide();
        SetSlidingText(FrameLabelMapName, Map.MapInfo.Name);
        LabelAuthor.Value = Map.MapInfo.AuthorNickName;
        LabelEnvironment.Value = Map.CollectionName;
        QuadEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{Map.CollectionName}.png");

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (FinishedAt != -1)
        {
            MoveSlidingText(FrameLabelMapName, 10, 0.01f);
            MoveSlidingText(FrameLabelCar, 10, -0.01f);

            if (Input.IsKeyPressed(109) || Input.IsKeyPressed(119))
            {
                Continue();
            }
        }

        var car = Netread<string>.For(GetPlayer());

        if (car.Get() != PreviousCar)
        {
            PreviousCar = car.Get();
            SetSlidingText(FrameLabelCar, car.Get());
        }

        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }
    }

    void ShowEndscreen()
    {
        FrameEndscreenInfo.RelativePosition_V3.X = -40;
        FrameEndscreenInfo.RelativeScale = 1;
        AnimMgr.Add(FrameEndscreenInfo, "<frame pos=\"0 0\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(QuadBlur, "<quad scale=\"1\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameTime, "<frame pos=\"0 0\" />", Now + 200, 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameScore, "<frame pos=\"0 0\" />", Now + 200, 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameLeaderboard, "<frame pos=\"0 0\" />", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        FrameContinue.RelativeScale = 1;
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 0\" />", Now + 600, 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    void HideEndscreen()
    {
        FinishedAt = -1;
        AnimMgr.Add(QuadBlur, "<quad scale=\"0\" hidden=\"1\" />", 500, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameEndscreenInfo, "<frame pos=\"-10 0\" scale=\"0.2\" hidden=\"1\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameTime, "<frame pos=\"140 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameScore, "<frame pos=\"140 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameLeaderboard, "<frame pos=\"-60 0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 21\" scale=\"0\" />", 250, CAnimManager.EAnimManagerEasing.QuadIn);
    }

    private void Continue()
    {
        if (FinishedAt == -1 || Now - FinishedAt < 500)
        {
            return;
        }

        FinishedAt = -1;
        SendCustomEvent("EndscreenContinue", new[] { "" });
        HideEndscreen();
    }

    private void SetSlidingText(CMlFrame frame, string value)
    {
        var l1 = (frame.Controls[0] as CMlLabel)!;
        l1.Value = value;
        l1.Size.X = l1.ComputeWidth(value);

        var l2 = (frame.Controls[1] as CMlLabel)!;
        l2.Value = value;
        l2.Size.X = l2.ComputeWidth(value);
    }

    private void MoveSlidingText(CMlFrame frame, int distance, float speed)
    {
        var l1 = (frame.Controls[0] as CMlLabel)!;
        var l2 = (frame.Controls[1] as CMlLabel)!;

        if (frame.ClipWindowSize.X >= l1.Size.X)
        {
            l2.Hide();
            l1.RelativePosition_V3.X = 0;
            return;
        }

        l1.RelativePosition_V3.X -= Period * speed;
        l2.RelativePosition_V3.X -= Period * speed;
        l2.Show();

        if (speed > 0)
        {
            if (l1.RelativePosition_V3.X + l1.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X > l2.RelativePosition_V3.X)
            {
                l1.RelativePosition_V3.X = l2.RelativePosition_V3.X + l2.Size.X + distance;
            }

            if (l2.RelativePosition_V3.X + l2.Size.X < 0 || l1.RelativePosition_V3.X + l1.Size.X < l2.RelativePosition_V3.X)
            {
                l2.RelativePosition_V3.X = l1.RelativePosition_V3.X + l1.Size.X + distance;
            }
        }
        else if (speed < 0)
        {
            if (l1.RelativePosition_V3.X - l1.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X < l2.RelativePosition_V3.X)
            {
                l1.RelativePosition_V3.X = l2.RelativePosition_V3.X - l2.Size.X - distance;
            }

            if (l2.RelativePosition_V3.X - l2.Size.X > 0 || l1.RelativePosition_V3.X - l1.Size.X > l2.RelativePosition_V3.X)
            {
                l2.RelativePosition_V3.X = l1.RelativePosition_V3.X - l1.Size.X - distance;
            }
        }
    }
}