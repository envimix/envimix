using System.Collections.Immutable;

namespace Envimix.Scripts.Modes.TrackMania;

public class EnvimixSolo : Envimix
{
    public struct SGhostMetadata
    {
        public string FileName;
        public int Index;
        public string Nickname;
        public int Time;
    }

    [Setting] public new bool EnableStadiumEnvimix = true;
    [Setting] public new bool EnableUnitedCars = true;
    [Setting] public new bool EnableTrafficCar = true;
    [Setting] public new bool EnableTrafficCarInStadium = true;
    [Setting] public new bool UseUnitedModels = true;
    [Setting] public new bool AlwaysUseVehicleItems = true;
    [Setting] public new string EnvimixWebAPI = "https://envimix.bigbang1112.cz/api";
    [Setting] public new string SkinsFile = "Skins_Turbo.json";

    public Dictionary<Ident, string> LocalGhostsTaskFiles;
    public IList<CTaskResult_GhostList> LocalGhostsTasks;
    public Dictionary<string, IList<CGhost>> LocalGhosts;
    public Dictionary<CGhost, Ident> SpawnedGhosts;

    [Netwrite] public IList<SGhostMetadata> LocalGhostMetadata { get; set; }
    [Netwrite] public int LocalGhostMetadataUpdatedAt { get; set; }
    [Netwrite] public bool CanListenToUIEvents { get; set; }

    public override void OnServerInit()
    {
        ClientManiaAppUrl = "file://Media/ManiaApps/EnvimixSingleplayerClient.Script.txt";

        UIManager.UIAll.ScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.SmallScoreTableVisibility = CUIConfig.EVisibility.ForcedHidden;
        UIManager.UIAll.ScoreTableOnlyManialink = true;
    }

    public override void OnMapLoad()
    {
        Wait(() => Players.Count > 0); // Sync the player, as it's not available right after map load

        CanListenToUIEvents = true;

        PrespawnEnvimixPlayers();
    }

    public override void OnUIEvent(CUIConfigEvent e)
    {
        switch (e.Type)
        {
            case CUIConfigEvent.EType.OnLayerCustomEvent:
                ProcessGeneralEnvimixEvents(e);
                ProcessUpdateSkinEvent(e);
                ProcessUpdateCarEvent(e);
                ProcessReplayEvent(e);
                // ProcessUpdateGravityEvent(e); unexpected behavior due to GravityCoef being applied once after spawning
                break;
        }
    }

    public override void OnPlayerAdded(CTmModeEvent e)
    {
        PrespawnPlayer(e.Player);
    }

    public override void OnGameLoop()
    {
        foreach (var player in PlayersWaiting)
        {
            TrySpawnEnvimixPlayer(player, frozen: false);
        }

        CheckForLocalGhosts();
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

    private void ProcessReplayEvent(CUIConfigEvent e)
    {
        switch (e.CustomEventType)
        {
            case "ResetReplays":
                LocalGhostMetadata.Clear();
                break;
            case "Replay":
                var fileName = e.CustomEventData[0];
                var task = DataFileMgr.Replay_Load(fileName);
                LocalGhostsTaskFiles[task.Id] = fileName;
                LocalGhostsTasks.Add(task);
                break;
            case "AddGhost":
                var replayFileNameAdd = e.CustomEventData[0];
                var ghostIndexAdd = e.CustomEventData[1];
                var ghostToAdd = LocalGhosts[replayFileNameAdd][TextLib.ToInteger(ghostIndexAdd)];
                SpawnedGhosts[ghostToAdd] = RaceGhost_Add(ghostToAdd, DisplayAsPlayerBest: false);

                Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameAdd}' (#{ghostIndexAdd}) added.");

                break;
            case "RemoveGhost":
                var replayFileNameRemove = e.CustomEventData[0];
                var ghostIndexRemove = e.CustomEventData[1];
                var ghostToRemove = LocalGhosts[replayFileNameRemove][TextLib.ToInteger(ghostIndexRemove)];

                if (SpawnedGhosts.ContainsKey(ghostToRemove))
                {
                    RaceGhost_Remove(SpawnedGhosts[ghostToRemove]);
                    SpawnedGhosts.Remove(ghostToRemove);

                    Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameRemove}' (#{ghostIndexRemove}) removed.");
                }
                else
                {
                    Log(nameof(EnvimixSolo), $"Ghost '{replayFileNameRemove}' (#{ghostIndexRemove}) is missing but shouldn't be.");
                }

                break;
        }
    }

    private void CheckForLocalGhosts()
    {
        ImmutableArray<CTaskResult_GhostList> completedGhostTasks = new();

        foreach (var ghostTask in LocalGhostsTasks)
        {
            if (ghostTask.IsProcessing)
            {
                continue;
            }

            if (ghostTask.HasSucceeded && ghostTask.Ghosts.Count > 0)
            {
                ImmutableArray<CGhost> ghosts = new();

                for (int i = 0; i < ghostTask.Ghosts.Count; i++)
                {
                    var ghost = ghostTask.Ghosts[i];

                    ghosts.Add(ghost);

                    SGhostMetadata metadata = new()
                    {
                        FileName = LocalGhostsTaskFiles[ghostTask.Id],
                        Index = i,
                        Nickname = ghost.Nickname,
                        Time = ghost.Result.Time
                    };

                    LocalGhostMetadata.Add(metadata);
                }

                LocalGhosts[LocalGhostsTaskFiles[ghostTask.Id]] = ghosts;
            }

            completedGhostTasks.Add(ghostTask);

            LocalGhostsTaskFiles.Remove(ghostTask.Id);

            LocalGhostMetadataUpdatedAt = Now;
        }

        foreach (var ghostTask in completedGhostTasks)
        {
            LocalGhostsTasks.Remove(ghostTask);
        }

        if (completedGhostTasks.Length > 0)
        {
            completedGhostTasks.Clear();
        }
    }
}
