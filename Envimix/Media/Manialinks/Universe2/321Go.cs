namespace Envimix.Media.Manialinks.Universe2;

public class _321Go : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlLabel LabelCountdown;
    [ManialinkControl] public required CMlLabel LabelCountdownEffect;

    public int PreviousSecond;
    public bool PreviousIsVisible;

    [Netread] public required int CutOffTimeLimit { get; set; }
    
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
        if (GetPlayer() is null)
        {
            return false;
        }

        return GetPlayer().RaceStartTime > 0
            && (GetPlayer().RaceStartTime < CutOffTimeLimit
                || (CutOffTimeLimit == -1 && GameTime - GetPlayer().RaceStartTime > -3000)
            );
    }

    public void Main()
    {
        PreviousSecond = MathLib.FloorInteger(Now / 1000f);

        Wait(() => GetPlayer() is not null);

        LabelCountdown.Visible = IsVisible();
        LabelCountdownEffect.Visible = IsVisible();
        PreviousIsVisible = IsVisible();
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            LabelCountdown.Visible = IsVisible();
            LabelCountdownEffect.Visible = IsVisible();
            PreviousIsVisible = IsVisible();
        }

        if (!IsVisible())
        {
            return; // generated as return, should act as continue, or just migrated to a method
        }

        var time = GameTime - GetPlayer().RaceStartTime;
        var animTime = time % 1000;

        if (animTime < 0)
        {
            animTime = 1000 - animTime * -1;
        }

        if (MathLib.FloorInteger(time / 1000f) != PreviousSecond && animTime < 500)
        {
            PreviousSecond = MathLib.FloorInteger(time / 1000f);

            switch (PreviousSecond)
            {
                case < 0:
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 1, 1);
                    SendCustomEvent("Countdown", new[] { "" });
                    break;
                case 0:
                    Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 0, 1);
                    SendCustomEvent("Countdown", new[] { "Start" });
                    break;
            }
        }

        if (animTime <= 100)
        {
            LabelCountdown.Opacity = AnimLib.EaseLinear(animTime, 0, 1, 100);
            LabelCountdown.RelativeScale = AnimLib.EaseLinear(animTime, .5f, .5f, 100);
            LabelCountdownEffect.Opacity = AnimLib.EaseLinear(animTime, 0, 1, 100);
            LabelCountdownEffect.RelativeScale = AnimLib.EaseLinear(animTime, .5f, .5f, 100);
        }
        else if (animTime >= 900 && time <= 0)
        {
            LabelCountdown.Opacity = AnimLib.EaseLinear(animTime - 900, _Base: 1, _Change: -1, _Duration: 100);
            LabelCountdown.RelativeScale = AnimLib.EaseLinear(animTime - 900, 1, -.5f, 100);
            LabelCountdownEffect.Opacity = AnimLib.EaseLinear(animTime - 900, 1, -1, 100);
            LabelCountdownEffect.RelativeScale = AnimLib.EaseLinear(animTime - 900, 1, -.5f, 100);
        }
        else if (time >= 0)
        {
            LabelCountdown.Opacity = AnimLib.EaseLinear(animTime - 250, 1, -1, 500);
            LabelCountdownEffect.RelativeScale = AnimLib.EaseLinear(animTime, 1, 1, 750);
            LabelCountdownEffect.Opacity = AnimLib.EaseLinear(animTime, .5f, -.5f, 750);
        }
        else
        {
            LabelCountdown.Opacity = 1;
            LabelCountdown.RelativeScale = 1;
            LabelCountdownEffect.Opacity = AnimLib.EaseOutQuad(animTime, .75f, -.75f, 1000);
            LabelCountdownEffect.RelativeScale = AnimLib.EaseOutQuad(animTime, 1, 1, 1000);
        }

        if (time < 0)
        {
            LabelCountdown.Value = MathLib.CeilingInteger(time / -1000f) + "";
            LabelCountdownEffect.Value = MathLib.CeilingInteger(time / -1000f) + "";
        }
        else if (time < 1000)
        {
            LabelCountdown.Value = "Go!";
            LabelCountdownEffect.Value = "Go!";
        }
        else
        {
            LabelCountdown.Value = "";
            LabelCountdownEffect.Value = "";
        }
    }
}