namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixSolo : Envimix
{
    [Setting] public new bool EnableStadiumEnvimix = true;
    [Setting] public new bool EnableUnitedCars = true;
    [Setting] public new bool EnableTrafficCar = false; // TODO: Fix
    [Setting] public new bool EnableTrafficCarInStadium = false; // TODO: Fix
    [Setting] public new string EnvimixWebAPI = "";

    public override void OnMapLoad()
    {
        Wait(() => Players.Count > 0); // Sync the player, as it's not available right after map load

        PrespawnEnvimixPlayers();
    }
}
