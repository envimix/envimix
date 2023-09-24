namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixSolo : Envimix
{
    [Setting] public new bool EnableStadiumEnvimix = true;
    [Setting] public new bool EnableUnitedCars = true;
    [Setting] public new bool EnableTrafficCar = false; // TODO: Fix
    [Setting] public new bool EnableTrafficCarInStadium = false; // TODO: Fix
    [Setting] public new bool UseUnitedModels = true;
    [Setting] public new string EnvimixWebAPI = "";

    public override void OnMapLoad()
    {
        Wait(() => Players.Count > 0); // Sync the player, as it's not available right after map load

        PrespawnEnvimixPlayers();
    }

    public override void OnGameStart()
    {
        while (true)
        {
            foreach (var e in UIManager.PendingEvents)
            {
                switch (e.Type)
                {
                    case CUIConfigEvent.EType.OnLayerCustomEvent:
                        ProcessUpdateSkinEvent(e);
                        break;
                }
            }

            foreach (var e in PendingEvents)
            {
                switch (e.Type)
                {
                    case CTmModeEvent.EType.OnPlayerAdded:
                        PrepareJoinedPlayer(e.Player);
                        break;
                }
            }

            foreach (var player in Players)
            {
                TrySpawnEnvimixPlayer(player, frozen: true);
            }

            Yield();
        }
    }
}
