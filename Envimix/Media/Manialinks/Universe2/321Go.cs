namespace Envimix.Media.Manialinks.Universe2;

public class _321Go : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlLabel LabelCountdown;
    [ManialinkControl] public required CMlLabel LabelCountdownEffect;

    public int PreviousSecond;
    public bool PreviousIsVisible;

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
            return; // generated as return, should act as continue
        }

        var time = GameTime - GetPlayer().RaceStartTime;
        var animTime = time % 1000;
        if (animTime < 0) animTime = 1000 - animTime * -1;

        if (MathLib.FloorInteger(time / 1000) != PreviousSecond && animTime < 500)
        {
            PreviousSecond = MathLib.FloorInteger(time / 1000f);
            if (PreviousSecond < 0)
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 1, 1);
            else if (PreviousSecond == 0)
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Countdown, 0, 1);
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
            LabelCountdown.Opacity = AnimLib.EaseLinear(animTime - 900, 1, -1, 100);
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

    bool IsVisible()
    {
        var Net_CutOffTimeLimit = Netread<int>.For(Teams[0]);
        return GetPlayer().RaceStartTime > 0 && (GetPlayer().RaceStartTime < Net_CutOffTimeLimit.Get() || (Net_CutOffTimeLimit.Get() == -1 && GameTime - GetPlayer().RaceStartTime > -3000));
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null) return GUIPlayer;
        return InputPlayer;
    }
}