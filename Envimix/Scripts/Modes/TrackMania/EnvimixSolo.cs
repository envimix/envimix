namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixSolo : Envimix
{
    [Setting] public new bool EnableStadiumEnvimix = true;
    [Setting] public new bool EnableUnitedCars = true;
    [Setting] public new bool EnableTrafficCar = false; // TODO: Fix
    [Setting] public new bool EnableTrafficCarInStadium = false; // TODO: Fix
    [Setting] public new bool UseUnitedModels = true;
    [Setting] public new bool AlwaysUseVehicleItems = true;
    [Setting] public new string EnvimixWebAPI = "";
    [Setting] public new string SkinsFile = "Skins_Turbo.json";

    public override void OnServerInit()
    {
        ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixSingleplayerClient.Script.txt";
    }

    public override void OnMapLoad()
    {
        Wait(() => Players.Count > 0); // Sync the player, as it's not available right after map load

        PrespawnEnvimixPlayers();
    }

    public override void OnUIEvent(CUIConfigEvent e)
    {
        switch (e.Type)
        {
            case CUIConfigEvent.EType.OnLayerCustomEvent:
                ProcessUpdateSkinEvent(e);
                ProcessUpdateCarEvent(e);
                break;
        }
    }

    public override void OnPlayerAdded(CTmModeEvent e)
    {
        PrepareJoinedPlayer(e.Player);
    }

    public override void OnGameLoop()
    {
        foreach (var player in PlayersWaiting)
        {
            TrySpawnEnvimixPlayer(player, frozen: false);
        }
    }

    private void ProcessUpdateCarEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "Car":
                if (e.CustomEventData.Count > 0)
                {
                    var carName = e.CustomEventData[0];
                    var player = GetPlayer(e.UI);
                    SetValidClientCar(player, carName);

                    var car = Netwrite<string>.For(player);

                    if (e.CustomEventData.Count > 1)
                    {
                        var respawn = true; // always respawn on Car event

                        if (respawn)
                        {
                            var frozen = e.CustomEventData.Count > 2 && e.CustomEventData[2] == "True";
                            var spawned = SpawnEnvimixPlayer(player, car.Get(), frozen);
                        }
                    }
                }
                break;
        }
    }
}
