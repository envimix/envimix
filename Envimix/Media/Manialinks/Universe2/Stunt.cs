namespace Envimix.Media.Manialinks.Universe2;

public class Stunt : CTmMlScriptIngame, IContext
{
    public int PrevStuntLastTime;
    public Dictionary<CTmMlPlayer.ESceneVehiclePhyStuntFigure, string> StuntMappings;

    [ManialinkControl] public required CMlLabel LabelStunt;

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public void Main()
    {
        StuntMappings = new()
        {
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Aerial, "Aerial" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.AlleyOop, "Alley Oop" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.BackFlip, "Backflip" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Corkscrew, "Corkscrew" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Flip, "Flip" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.FlipFlap, "Flip Flap" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.FlippingChaos, "Flipping Chaos" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.FreeStyle, "Free Style" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Grind, "Grind" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.None, "None" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Reset, "Reset" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.RespawnPenalty, "Respawn Penalty" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Rodeo, "Rodeo" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Roll, "Roll" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.RollingMadness, "Rolling Madness" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Spin, "Spin" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.SpinningMix, "Spinning Mix" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.SpinOff, "Spin Off" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.StraightJump, "Straight Jump" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.TimePenalty, "Time Penalty" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.Twister, "Twister" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckAerial, "Wreck Aerial" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckAlleyOop, "Wreck Alley Oop" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckBackFlip, "Wreck Backflip" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckCorkscrew, "Wreck Backflip" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckFlip, "Wreck Flip" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckFlipFlap, "Wreck Flip Flap" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckFlippingChaos, "Wreck Flipping Chaos" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckFreeStyle, "Wreck Free Style" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckNone, "Wreck None" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckRodeo, "Wreck Rodeo" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckRoll, "Wreck Roll" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckRollingMadness, "Wreck Rolling Madness" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckSpin, "Wreck Spin" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckSpinningMix, "Wreck Spinning Mix" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckSpinOff, "Wreck Spin Off" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckStraightJump, "Wreck Straight Jump" },
            { CTmMlPlayer.ESceneVehiclePhyStuntFigure.WreckTwister, "Wreck Twister" },
        };

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        var stuntTime = GetPlayer().StuntLastTime;

        if (stuntTime != PrevStuntLastTime)
        {
            if (stuntTime == -1)
            {
                LabelStunt.Hide();
                return;
            }

            if (GetPlayer().StuntAngle == 0)
            {
                return;
            }

            LabelStunt.SetText($"{StuntMappings[GetPlayer().StuntLast]} {GetPlayer().StuntAngle}!");
            LabelStunt.Show();

            PrevStuntLastTime = stuntTime;
        }

        if (GetPlayer().TimeElapsedSinceLastStunt > 2000)
        {
            LabelStunt.Hide();
        }
    }
}
