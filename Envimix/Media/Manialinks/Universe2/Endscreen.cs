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
    [ManialinkControl] public required CMlLabel LabelAuthor;
    [ManialinkControl] public required CMlLabel LabelEnvironment;
    [ManialinkControl] public required CMlQuad QuadEnvironment;

    public int FinishedAt;

    public Endscreen()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace)
                    {
                        FinishedAt = Now;
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

    public void Main()
    {
        HideEndscreen();
        SetSlidingText(FrameLabelMapName, Map.MapInfo.Name);
        LabelAuthor.Value = Map.MapInfo.AuthorNickName;
        LabelEnvironment.Value = Map.CollectionName;
        QuadEnvironment.ChangeImageUrl($"file://Media/Images/Environments/{Map.CollectionName}.png");

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (FinishedAt != -1)
        {
            MoveSlidingText(FrameLabelMapName, 10, 0.01f);

            if (Input.IsKeyPressed(109) || Input.IsKeyPressed(119))
            {
                Continue();
            }
        }
    }

    void ShowEndscreen()
    {
        FrameEndscreenInfo.Show();
        AnimMgr.Add(QuadBlur, "<quad scale=\"1\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameTime, "<frame pos=\"0 0\" />", 600, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameScore, "<frame pos=\"0 0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameLeaderboard, "<frame pos=\"0 0\" />", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameContinue, "<frame pos=\"0 0\" />", Now + 400, 500, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    void HideEndscreen()
    {
        FinishedAt = -1;
        FrameEndscreenInfo.Hide();
        AnimMgr.Add(QuadBlur, "<quad scale=\"0\" hidden=\"1\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        FrameTime.RelativePosition_V3.X = 140;
        FrameScore.RelativePosition_V3.X = 140;
        FrameLeaderboard.RelativePosition_V3.X = -60;
        FrameContinue.RelativePosition_V3.Y = 21;
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