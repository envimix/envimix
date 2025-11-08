using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class RatingSolo : CTmMlScriptIngame, IContext
{
    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SRatingFilter
    {
        public string Car;
        public int Gravity;
        public string Type;
    }

    public struct SFilteredRating
    {
        public SRatingFilter Filter;
        public SRating Rating;
    }

    [ManialinkControl] public required CMlFrame FrameRatingSolo;
    [ManialinkControl] public required CMlFrame FrameRatingSoloWrap;
    [ManialinkControl] public required CMlQuad QuadBlur;
    [ManialinkControl] public required CMlQuad QuadMyCar;
    [ManialinkControl] public required CMlLabel LabelMyCar;
    [ManialinkControl] public required CMlQuad QuadRatingBg;
    [ManialinkControl] public required CMlQuad QuadStar;
    [ManialinkControl] public required CMlFrame FrameDifficulty;
    [ManialinkControl] public required CMlFrame FrameQuality;

    [Netread] public int FinishedAt { get; set; }
    [Netread] public bool RatingOpen { get; set; }
    [Netread] public bool Outro { get; set; }

    [Netread] public bool RatingEnabled { get; }
    [Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    [Netread] public required int RatingsUpdatedAt { get; set; }
    [Netread(NetFor.UI)] public required IList<SFilteredRating> MyRatings { get; set; }
    [Netread] public required Dictionary<string, bool> Stars { get; set; }

    public bool MenuOpen;
    public bool PreviousIsVisible;

    public required ImmutableArray<CMlFrame> RatingFrames;
    public required CMlLabel LabelDifficulty;
    public required CMlLabel LabelQuality;
    public required CMlQuad QuadDifficultyBlink;
    public required CMlQuad QuadQualityBlink;
    public required CMlQuad QuadDifficultyGlow;
    public required CMlQuad QuadQualityGlow;
    public required CMlFrame? Hold;

    public float Difficulty;
    public float Quality;
    public bool PrevRatingEnabled;
    public int PrevRatingsUpdatedAt;
    public string PrevCar;
    public float PrevScrollOffsetY;
    public bool HoldsScrollbar;
    public float HoldsScrollbarMouseY;

    public RatingSolo()
    {
        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("CloseRating", new[] { "" });
                    break;
            }
        };

        QuadStar.MouseOver += () =>
        {
            QuadStar.Opacity = 0.7f;
        };

        QuadStar.MouseOut += () =>
        {
            QuadStar.Opacity = 0.1f;
        };

        MouseClick += RatingSolo_MouseClick;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

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

        return !IsInGameMenuDisplayed && FinishedAt == -1 && (RatingOpen || Outro);
    }

    string GetCar()
    {
        var car = Netread<string>.For(GetPlayer());
        return car.Get();
    }

    public void Main()
    {
        PreviousIsVisible = IsVisible();
        FrameRatingSolo.Visible = IsVisible();
        FrameRatingSolo.RelativePosition_V3.Y = -13;
        QuadBlur.RelativeScale = 0;

        Difficulty = -1;
        Quality = -1;
        LabelDifficulty = (FrameDifficulty.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelDifficulty.SetText("Difficulty");
        QuadDifficultyBlink = (FrameDifficulty.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadDifficultyGlow = (FrameDifficulty.GetFirstChild("QuadGlow") as CMlQuad)!;
        LabelQuality = (FrameQuality.GetFirstChild("LabelRating") as CMlLabel)!;
        LabelQuality.SetText("Quality");
        QuadQualityBlink = (FrameQuality.GetFirstChild("QuadBlink") as CMlQuad)!;
        QuadQualityGlow = (FrameDifficulty.GetFirstChild("QuadGlow") as CMlQuad)!;
        RatingFrames.Add(FrameDifficulty);
        RatingFrames.Add(FrameQuality);

        var envimixTurboUserIsAdmin = Local<bool>.For(LocalUser);
        QuadStar.Visible = envimixTurboUserIsAdmin.Get();
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                // do animation
                if (!Outro)
                {
                    AnimMgr.Add(QuadBlur, "<quad scale=\"1\" hidden=\"0\" />", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                }
                AnimMgr.Add(FrameRatingSolo, "<frame pos=\"0 0\" />", 300, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Flush(QuadBlur);
                AnimMgr.Flush(FrameRatingSolo);
                QuadBlur.RelativeScale = 0;
                FrameRatingSolo.RelativePosition_V3.Y = -13;
            }

            PreviousIsVisible = IsVisible();
        }

        QuadBlur.Visible = IsVisible();
        FrameRatingSolo.Visible = IsVisible();
        EnableMenuNavigationInputs = IsVisible() && !Outro;

        var car = Netread<string>.For(GetPlayer());
        var currentCarUrl = $"https://envimix.gbx.tools/img/cars/{car.Get()}.png";

        if (QuadMyCar.ImageUrl != currentCarUrl)
        {
            QuadMyCar.ChangeImageUrl(currentCarUrl);
        }

        LabelMyCar.Value = car.Get();

        if (Outro)
        {
            FrameRatingSoloWrap.RelativePosition_V3.X = -55;
            FrameRatingSoloWrap.RelativePosition_V3.Y = 81;
            QuadRatingBg.Visible = false;
        }
        else
        {
            FrameRatingSoloWrap.RelativePosition_V3.X = 0;
            FrameRatingSoloWrap.RelativePosition_V3.Y = 0;
            QuadRatingBg.Visible = true;
        }

        if (RatingEnabled != PrevRatingEnabled)
        {
            QuadDifficultyBlink.Visible = RatingEnabled;
            QuadQualityBlink.Visible = RatingEnabled;

            if (RatingEnabled)
            {
                UpdatePersonalRatings();

                QuadDifficultyGlow.Opacity = 0.25f;
                QuadQualityGlow.Opacity = 0.25f;
            }
            else
            {
                QuadDifficultyGlow.Opacity = 0.1f;
                QuadQualityGlow.Opacity = 0.1f;
            }

            UpdateRatings();

            foreach (var frame in RatingFrames)
            {
                if (RatingEnabled)
                {
                    (frame.GetFirstChild("LabelRateName") as CMlLabel)!.SetText("Click to rate");
                }
                else
                {
                    (frame.GetFirstChild("LabelRateName") as CMlLabel)!.SetText("Rating is currently disabled");
                }
            }

            PrevRatingEnabled = RatingEnabled;
        }

        if (RatingEnabled)
        {
            if (QuadDifficultyBlink.Visible)
            {
                QuadDifficultyBlink.Opacity = (MathLib.Sin(Now / 100f) + 1) / 2f * .1f;
            }

            if (QuadQualityBlink.Visible)
            {
                QuadQualityBlink.Opacity = (MathLib.Sin(Now / 100f + 180) + 1) / 2f * .1f;
            }

            if (Hold is not null)
            {
                if (MouseLeftButton)
                {
                    var frame = Hold.Parent.Parent;

                    var visualValue = MathLib.Clamp(MouseX - (float)frame.RelativePosition_V3.X - (float)FrameRatingSoloWrap.RelativePosition_V3.X, -28, 28);
                    var realValue = (visualValue + 28) / 56;

                    Hold.RelativePosition_V3.X = visualValue;

                    (Hold.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = true;

                    if (frame.ControlId == "FrameDifficulty")
                    {
                        Difficulty = realValue;
                    }
                    else if (frame.ControlId == "FrameQuality")
                    {
                        Quality = realValue;
                    }

                    //var gauge = (frame.GetFirstChild("GaugeRating") as CMlGauge)!;
                    //gauge.SetRatio(realValue);
                }
                else
                {
                    (Hold.GetFirstChild("QuadDraggable") as CMlQuad)!.StyleSelected = false;

                    var frame = Hold.Parent.Parent;

                    if (frame.ControlId == "FrameDifficulty")
                    {
                        SendCustomEvent("Rate", new[] { "Difficulty", Difficulty.ToString() });
                    }
                    else if (frame.ControlId == "FrameQuality")
                    {
                        SendCustomEvent("Rate", new[] { "Quality", Quality.ToString() });
                    }

                    Hold = null;
                }
            }
        }

        if (RatingsUpdatedAt != PrevRatingsUpdatedAt)
        {
            UpdateRatings();

            PrevRatingsUpdatedAt = RatingsUpdatedAt;
        }

        if (GetCar() != PrevCar)
        {
            UpdateRatings();
            UpdatePersonalRatings();

            PrevCar = GetCar();
        }
    }

    static string ConstructFilterKey(string car, int gravity)
    {
        return $"{car}_{gravity}_Time";
    }

    static string ConstructRatingFilterKey(SRatingFilter filter)
    {
        return ConstructFilterKey(filter.Car, filter.Gravity);
    }

    string ConstructRatingFilterKey()
    {
        var car = Netread<string>.For(GetPlayer());
        var gravity = Netread<int>.For(GetPlayer());

        return ConstructFilterKey(car.Get(), gravity.Get());
    }

    private void UpdateRatings()
    {
        var filterKey = ConstructRatingFilterKey();

        if (!Ratings.ContainsKey(filterKey))
        {
            (FrameDifficulty.GetFirstChild("GaugeRating") as CMlGauge)!.Ratio = 0;
            (FrameQuality.GetFirstChild("GaugeRating") as CMlGauge)!.Ratio = 0;
            return;
        }

        var rating = Ratings[filterKey];

        if (rating.Difficulty < 0)
        {
            AnimMgr.Add(FrameDifficulty.GetFirstChild("GaugeRating"), "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
        else
        {
            AnimMgr.Add(FrameDifficulty.GetFirstChild("GaugeRating"), $"<gauge ratio=\"{rating.Difficulty * .9f + .1f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }

        if (rating.Quality < 0)
        {
            AnimMgr.Add(FrameQuality.GetFirstChild("GaugeRating"), "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
        else
        {
            AnimMgr.Add(FrameQuality.GetFirstChild("GaugeRating"), $"<gauge ratio=\"{rating.Quality * .9f + .1f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void ClearPersonalRating(CMlFrame frame)
    {
        frame.GetFirstChild("LabelRateName").Hide();

        if (frame.ControlId == "FrameDifficulty")
        {
            QuadDifficultyBlink.Hide();
        }
        else if (frame.ControlId == "FrameQuality")
        {
            QuadQualityBlink.Hide();
        }
    }

    private void SetPersonalRating(CMlFrame frame, float value)
    {
        var draggable = frame.GetFirstChild("FrameDraggable")!;

        if (value < 0)
        {
            frame.GetFirstChild("LabelRateName").Show();

            if (frame.ControlId == "FrameDifficulty")
            {
                QuadDifficultyBlink.Show();
            }
            else if (frame.ControlId == "FrameQuality")
            {
                QuadQualityBlink.Show();
            }

            draggable.Hide();
        }
        else
        {
            ClearPersonalRating(frame);

            draggable.RelativePosition_V3.X = value * 56 - 28;
            draggable.Show();
        }
    }

    private void UpdatePersonalRatings()
    {
        var ratingsUpdated = false;

        foreach (var r in MyRatings)
        {
            if (ConstructRatingFilterKey(r.Filter) == ConstructRatingFilterKey())
            {
                SetPersonalRating(FrameDifficulty, r.Rating.Difficulty);
                SetPersonalRating(FrameQuality, r.Rating.Quality);

                ratingsUpdated = true;
                break;
            }
        }

        if (ratingsUpdated)
        {
            return;
        }

        SetPersonalRating(FrameDifficulty, -1);
        SetPersonalRating(FrameQuality, -1);
    }

    private void RatingSolo_MouseClick(CMlControl control, string controlId)
    {
        if (RatingEnabled)
        {
            if (controlId == "QuadBox" || controlId == "QuadDraggable")
            {
                CMlFrame frame;

                if (controlId == "QuadBox")
                {
                    frame = control.Parent.Parent;
                }
                else
                {
                    frame = control.Parent.Parent.Parent;
                }

                ClearPersonalRating(frame);
            }

            if (controlId == "QuadBox")
            {
                var frameDraggable = (control.Parent.Parent.GetFirstChild("FrameDraggable") as CMlFrame)!;
                frameDraggable.Show();

                Hold = frameDraggable;
            }
            else if (controlId == "QuadDraggable")
            {
                Hold = control.Parent;
            }
        }
    }
}