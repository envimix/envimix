namespace Envimix.Media.Manialinks.Universe2;

public class Rating : CTmMlScriptIngame, IContext
{
    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public bool PreviousVisible;
    public int VisibleTime = -1;
    public string PreviousCar = "";
    public int PrevRatingsUpdatedAt = -1;

    [ManialinkControl] public required CMlFrame FrameRating;
    [ManialinkControl] public required CMlGauge GaugeDifficulty;
    [ManialinkControl] public required CMlGauge GaugeQuality;
    [ManialinkControl] public required CMlLabel LabelDifficulty;
    [ManialinkControl] public required CMlLabel LabelQuality;

    [Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    [Netread] public required int RatingsUpdatedAt { get; set; }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    string ConstructRatingFilterKey()
    {
        var car = Netread<string>.For(GetPlayer());
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car.Get()}_{gravity.Get()}_Time";
    }

    string GetCar()
    {
        var car = Netread<string>.For(GetPlayer());
        return car.Get();
    }

    public void Main()
    {
        FrameRating.Visible = IsVisible();
        PreviousVisible = FrameRating.Visible;
        
        Wait(() => GetPlayer() is not null);
    }

    public void UpdateRatings()
    {
        var filterKey = ConstructRatingFilterKey();

        if (Ratings.ContainsKey(filterKey))
        {
            var rating = Ratings[filterKey];

            if (rating.Difficulty < 0)
            {
                AnimMgr.Add(GaugeDifficulty, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                GaugeDifficulty.Ratio = 0;
            }
            else
            {
                AnimMgr.Add(GaugeDifficulty, $"<gauge ratio=\"{rating.Difficulty * .84f + .16f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            if (rating.Quality < 0)
            {
                AnimMgr.Add(GaugeQuality, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                GaugeQuality.Ratio = 0;
            }
            else
            {
                AnimMgr.Add(GaugeQuality, $"<gauge ratio=\"{rating.Quality * .84f + .16f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        }
        else
        {
            GaugeDifficulty.Ratio = 0;
            GaugeQuality.Ratio = 0;
        }
    }

    public void Loop()
    {
        FrameRating.Visible = IsVisible();

        if (FrameRating.Visible != PreviousVisible)
        {
            if (FrameRating.Visible)
            {
                (FrameRating.Controls[0] as CMlQuad)!.Size.X = 0;
                (FrameRating.Controls[1] as CMlQuad)!.Size.X = 0;
                AnimMgr.Add(FrameRating.Controls[0], "<quad size=\"42.5 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(FrameRating.Controls[1], "<quad size=\"42.5 10\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                GaugeDifficulty.Size.X = 0;
                AnimMgr.Add(GaugeDifficulty, "<gauge size=\"0 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(GaugeDifficulty, "<gauge size=\"27 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                GaugeQuality.Size.X = 0;
                AnimMgr.Add(GaugeQuality, "<gauge size=\"0 7\"/>", 350, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.AddChain(GaugeQuality, "<gauge size=\"27 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                LabelDifficulty.RelativePosition_V3.X = 40;
                LabelQuality.RelativePosition_V3.X = 40;
                AnimMgr.Add(LabelDifficulty, "<label pos=\"23.5 2\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(LabelQuality, "<label pos=\"23.5 -1.7\"/>", 350, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousVisible = FrameRating.Visible;
        }

        if (GetCar() != PreviousCar)
        {
            UpdateRatings();

            PreviousCar = GetCar();
        }

        if (RatingsUpdatedAt != PrevRatingsUpdatedAt)
        {
            UpdateRatings();

            PrevRatingsUpdatedAt = RatingsUpdatedAt;
        }
    }
}
