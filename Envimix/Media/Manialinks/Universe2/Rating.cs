namespace Envimix.Media.Manialinks.Universe2;

public class Rating : CTmMlScriptIngame, IContext
{
    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    public struct SUserInfo
    {
        public string Login;
        public string Nickname;
        public string Zone;
        public string AvatarUrl;
        public string Language;
        public string Description;
        public Vec3 Color;
        public string SteamUserId;
        public int FameStars;
        public float LadderPoints;
    }

    public struct SEnvimaniaRecord
    {
        public SUserInfo User;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public bool Verified;
        public bool Projected;
        public string GhostUrl;
        public string DrivenAt;
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
    [ManialinkControl] public required CMlQuad QuadRating;
    [ManialinkControl] public required CMlQuad QuadStar;

    [Netread] public required Dictionary<string, SRating> Ratings { get; set; }
    [Netread] public required Dictionary<string, SStar> Stars { get; set; }
    [Netread] public required int RatingsUpdatedAt { get; set; }

    [Netread] public int FinishedAt { get; set; }
    [Netread] public bool Outro { get; set; }
    [Netread] public string MapPlayerModelName { get; set; }

    public bool MenuOpen;

    public Rating()
    {
        QuadRating.MouseClick += () =>
        {
            SendCustomEvent("OpenRating", new[] {""});
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

    bool IsExplore()
    {
        return CurrentServerModeName is "";
    }

    bool IsSolo()
    {
        return CurrentServerLogin is "";
    }

    private bool IsVisible()
    {
        if (IsExplore())
        {
            return !MenuOpen;
        }

        return !IsInGameMenuDisplayed && FinishedAt == -1 && !Outro;
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

    int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
        }

        if (NbLaps == -1)
        {
            return Map.TMObjective_NbLaps;
        }

        return NbLaps;
    }

    string ConstructValidationFilterKey()
    {
        var car = Netread<string>.For(GetPlayer());
        var gravity = Netread<int>.For(GetPlayer());

        return $"{car.Get()}_{gravity.Get()}_{GetLaps()}";
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
                AnimMgr.AddChain(GaugeDifficulty, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                GaugeDifficulty.Ratio = 0;
            }
            else
            {
                AnimMgr.AddChain(GaugeDifficulty, $"<gauge ratio=\"{rating.Difficulty * .84f + .16f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            if (rating.Quality < 0)
            {
                AnimMgr.AddChain(GaugeQuality, "<gauge ratio=\"0\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
                GaugeQuality.Ratio = 0;
            }
            else
            {
                AnimMgr.AddChain(GaugeQuality, $"<gauge ratio=\"{rating.Quality * .84f + .16f}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
        }
        else
        {
            GaugeDifficulty.Ratio = 0;
            GaugeQuality.Ratio = 0;
        }

        if (Stars.ContainsKey(filterKey))
        {
            var star = Stars[filterKey];
            QuadStar.Visible = true;
        }
        else
        {
            QuadStar.Visible = false;
        }

        var validations = Netread<Dictionary<string, SEnvimaniaRecord>>.For(Teams[0]);


        var validationKey = ConstructValidationFilterKey();

        var car = Netread<string>.For(GetPlayer());

        // if validated or is the default car
        if (validations.Get().ContainsKey(validationKey) || MapPlayerModelName == car.Get())
        {
            GaugeDifficulty.Color = new Vec3(1, 1, 1);
            GaugeQuality.Color = new Vec3(1, 1, 1);
        }
        else
        {
            // otherwise use the impossible red color
            GaugeDifficulty.Color = new Vec3(1, 0, 0);
            GaugeQuality.Color = new Vec3(1, 0, 0);
        }
    }

    public void Loop()
    {
        FrameRating.Visible = IsVisible();

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

                QuadStar.RelativePosition_V3.X = -50;
                AnimMgr.Add(QuadStar, "<quad pos=\"-25 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousVisible = FrameRating.Visible;
        }

        QuadRating.Visible = IsSolo();
    }
}
