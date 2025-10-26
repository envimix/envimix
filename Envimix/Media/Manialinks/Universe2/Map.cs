namespace Envimix.Media.Manialinks.Universe2;

public class Map : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FrameMap;
    [ManialinkControl] public required CMlFrame FrameMapName;
    [ManialinkControl] public required CMlFrame FrameMapNameBg;
    [ManialinkControl] public required CMlFrame FrameLabelMapName;
    [ManialinkControl] public required CMlLabel LabelMapName;
    [ManialinkControl] public required CMlLabel LabelMapName2;
    [ManialinkControl] public required CMlFrame FrameMapAuthor;
    [ManialinkControl] public required CMlFrame FrameMapAuthorBg;
    [ManialinkControl] public required CMlFrame FrameLabelMapAuthor;
    [ManialinkControl] public required CMlLabel LabelMapAuthor;
    [ManialinkControl] public required CMlLabel LabelMapAuthor2;
    [ManialinkControl] public required CMlFrame FrameCar;
    [ManialinkControl] public required CMlFrame FrameCarBg;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlLabel LabelCar;
    [ManialinkControl] public required CMlLabel LabelCar2;
    [ManialinkControl] public required CMlQuad QuadMapName;

    public string PreviousMapName = "";
    public string PreviousMapAuthor = " ";
    public string PreviousCar = "";
    public bool PreviousIsVisible;

    public string MapNameInExplore;

    [Netread(NetFor.Teams0)] public int FinishedAt { get; set; }
    [Netread(NetFor.Teams0)] public bool Outro { get; set; }

    public Map()
    {
        QuadMapName.MouseClick += () =>
        {
            ShowCurChallengeCard();
        };

        QuadMapName.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        PluginCustomEvent += (eventName, eventParams) =>
        {
            switch (eventName)
            {
                case "MenuOpen":
                    MenuOpen = eventParams.Length > 0 && eventParams[0] == "True";
                    break;
            }
        };
    }

    private CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public bool MenuOpen;

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    private bool IsVisible()
    {
        if (IsExplore())
        {
            return !MenuOpen;
        }

        return !IsInGameMenuDisplayed && FinishedAt == -1 && !Outro;
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

    public void Main()
    {
        if (IsExplore())
        {
            var exploreMapName = Metadata<string>.For(Map);
            MapNameInExplore = exploreMapName.Get();
        }

        FrameMapName.Hide();
        FrameCar.Hide();

        Wait(() => GetPlayer() is not null);

        PreviousIsVisible = IsVisible();
    }

    public void Loop()
    {
        FrameMap.Visible = IsVisible();

        if (Map is null)
        {
            HideFrames();
        }
        else
        {
            AdjustMapNameFrame();
            AdjustMapAuthorFrame();
            AdjustCarFrame();
        }

        TryAnimateVisibility();

        MoveSlidingText(FrameLabelMapName, 10, 0.01f);
    }

    private void AnimateVisiblity()
    {
        foreach (var control in FrameMapNameBg.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, "<quad size=\"" + FrameMapNameBg.DataAttributeGet("size") + " 15\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameLabelMapName.Controls)
        {
            if (control is not CMlLabel label)
            {
                continue;
            }

            label.Opacity = 0;
            AnimMgr.Add(label, "<label opacity=\"0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.AddChain(label, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameMapAuthorBg.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, $"<quad size=\"{FrameMapAuthorBg.DataAttributeGet("size")} 8\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameLabelMapAuthor.Controls)
        {
            if (control is not CMlLabel label)
            {
                continue;
            }

            label.Opacity = 0;
            AnimMgr.Add(label, "<label opacity=\"0\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.AddChain(label, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameCarBg.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, $"<quad size=\"{FrameCarBg.DataAttributeGet("size")} 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameLabelCar.Controls)
        {
            if (control is not CMlLabel label)
            {
                continue;
            }

            label.Opacity = 0;
            AnimMgr.Add(label, "<label opacity=\"0\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.AddChain(label, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void TryAnimateVisibility()
    {
        if (FrameMap.Visible == PreviousIsVisible)
        {
            return;
        }

        if (FrameMap.Visible)
        {
            AnimateVisiblity();
        }

        PreviousIsVisible = FrameMap.Visible;
    }

    private void HideFrames()
    {
        SetSlidingText(FrameLabelMapName, "");
        FrameMapName.Visible = false;
        FrameCar.Visible = false;
    }

    private string GetMapName()
    {
        if (IsExplore())
        {
            return MapNameInExplore;
        }
        return Map.MapInfo.Name;
    }

    private void AdjustCarFrame()
    {
        var car = Netread<string>.For(GetPlayer());
        LabelCar.Value = car.Get();

        if (car.Get() == PreviousCar)
        {
            return;
        }

        Map.MapName = $"{GetMapName()}.{car.Get()}";

        foreach (var control in FrameCarBg.Controls)
        {
            if (control is CMlQuad quad)
            {
                quad.Size.X = LabelCar.ComputeWidth(LabelCar.Value) + 6;
            }
        }

        FrameCarBg.DataAttributeSet("size", (LabelCar.ComputeWidth(LabelCar.Value) + 6).ToString());

        PreviousCar = car.Get();

        if (PreviousCar == "")
        {
            FrameCar.Visible = false;
        }
        else
        {
            FrameCar.Visible = true;
        }

        foreach (var control in FrameCarBg.Controls)
        {
            control.Size.X = 0;
            AnimMgr.Add(control, $"<quad size=\"{FrameCarBg.DataAttributeGet("size")} 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        foreach (var control in FrameLabelCar.Controls)
        {
            if (control is not CMlLabel label)
            {
                continue;
            }

            label.Opacity = 0;
            AnimMgr.Add(label, "<label opacity=\"0\"/>", Duration: 300, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.AddChain(label, "<label opacity=\"1\"/>", Duration: 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void AdjustMapNameFrame()
    {
        if (GetMapName() == PreviousMapName)
        {
            return;
        }

        foreach (var control in FrameMapNameBg.Controls)
        {
            if (control is not CMlQuad quad)
            {
                continue;
            }

            quad.Size.X = LabelMapName.ComputeWidth(GetMapName()) + 5;

            if (quad.Size.X > 80)
            {
                quad.Size.X = 80;
            }

            FrameMapNameBg.DataAttributeSet("size", quad.Size.X.ToString());
        }

        SetSlidingText(FrameLabelMapName, GetMapName());
        PreviousMapName = GetMapName();

        if (PreviousMapName is "")
        {
            FrameMapName.Visible = false;
        }
        else
        {
            FrameMapName.Visible = true;
        }
    }

    private string GetAuthorName()
    {
        if (IsExplore())
        {
            // This should probably move to separate location with other map types
            return "EXPLORE MODE";
        }
        return Map.MapInfo.AuthorNickName;
    }

    private void AdjustMapAuthorFrame()
    {
        if (GetAuthorName() == PreviousMapAuthor)
        {
            return;
        }

        foreach (var control in FrameMapAuthorBg.Controls)
        {
            if (control is not CMlQuad quad)
            {
                continue;
            }

            quad.Size.X = LabelMapAuthor.ComputeWidth(GetAuthorName()) + 5;

            if (quad.Size.X > 80)
            {
                quad.Size.X = 80;
            }

            FrameMapAuthorBg.DataAttributeSet("size", quad.Size.X.ToString());
        }

        SetSlidingText(FrameLabelMapAuthor, GetAuthorName());
        PreviousMapAuthor = GetAuthorName();

        if (PreviousMapAuthor is "")
        {
            FrameMapAuthor.Visible = false;
        }
        else
        {
            FrameMapAuthor.Visible = true;
        }
    }
}
