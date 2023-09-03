using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace Envimix.Scripts.Modes.TrackMania;

public class UniverseModeBase : CTmMode, IContext
{
    public struct SCheckpoint
    {
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
    }

    public struct SRecord
    {
        public string Login;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public ImmutableArray<SCheckpoint> Checkpoints;
    }

    [Setting(As = "Number of warm-ups")]
    public int WarmUpNb = 0;

    [Setting(As = "Warm-up duration")]
    public int WarmUpDuration = 0;

    [Setting(As = "Force number of laps")]
    public int ForceLapsNb = 0;

    [Setting(As = "Chat time")]
    public int ChatTime = 20;

    [Setting(As = "Enable map intro")]
    public bool EnableMapIntro = false;

    public bool Reload = true;
    public bool Terminate = false;
    public bool ReloadMap = false;
    public ImmutableArray<int> MapQueue;
    public int WarmUpStartTime = -1;
    public bool IsWarmUp = false;
    public int PodiumStartTime = -1;

    public required Dictionary<string, CUILayer> Layers;

    [Netwrite] public new int CutOffTimeLimit { get; set; }
    [Netwrite] public new string MapPlayerModelName { get; set; } = "";

    [Netwrite] public int CurrentWarmUpNb { get; set; }

    public virtual void BeforeServerInit() { }
    public virtual void Settings() { }

    public virtual void OnServerInit()
    {
        Log("UniverseModeBase", "Initializing server...");
    }

    public virtual void BeforeServerStart() { }
    public virtual void OnServerStart() { }

    public virtual void BeforeMapInit()
    {
        WarmUpStartTime = -1;
        IsWarmUp = CurrentWarmUpNb > 0;
        PodiumStartTime = -1;

        MatchEndRequested = false;

        UIManager.UIAll.UISequence = CUIConfig.EUISequence.None;
    }

    public virtual void OnMapInit() { }
    public virtual void BeforeMapLoad() { }
    public virtual void WhileMapLoad() { }

    public virtual void OnMapLoad()
    {
        MapPlayerModelName = base.MapPlayerModelName;
    }

    public virtual void BeforeMapIntroStart() { }

    public virtual void OnMapIntroStart()
    {
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.Intro;
    }

    public virtual void WhileMapIntro() { }

    public virtual void OnMapIntroEnd()
    {
        UIManager.UIAll.UISequence = CUIConfig.EUISequence.Playing;
    }

    public virtual void BeforeMapStart()
    {
        var barrier = Synchro_AddBarrier();
        Wait(() => Synchro_BarrierReached(barrier) || ServerShutdownRequested);
    }

    public virtual void OnMapStart() { }
    public virtual void OnWarmUpStart() { }
    public virtual void OnGameStart() { }

    public virtual void BeforeEvent()
    {
        IsWarmUp = CurrentWarmUpNb > 0;
    }

    public virtual void OnEvent(CTmModeEvent e)
    {
        switch (e.Type)
        {
            case CTmModeEvent.EType.StartLine:
                OnPlayerStart(e);
                break;
            case CTmModeEvent.EType.WayPoint:

                if (e.IsEndRace || (e.IsEndLap && IndependantLaps)) // Finish
                {
                    OnPlayerFinish(e);
                }
                else if (e.IsEndLap) // Lap
                {
                    OnPlayerLap(e);
                }
                else // Checkpoint
                {
                    OnPlayerCheckpoint(e);
                }

                break;
            case CTmModeEvent.EType.GiveUp:
                OnPlayerGiveUp(e);
                break;
            case CTmModeEvent.EType.OnPlayerAdded:
                OnPlayerAdded(e);
                break;
            case CTmModeEvent.EType.OnPlayerRemoved:
                OnPlayerRemoved(e);
                break;
            case CTmModeEvent.EType.Stunt:
                OnStunt(e);
                break;
        }
    }

    public virtual void OnPlayerStart(CTmModeEvent e)
    {
        ResetTempResult(e);
    }

    public virtual void OnPlayerFinish(CTmModeEvent e)
    {
        FinishTempResult(e);
    }

    public virtual void OnPlayerCheckpoint(CTmModeEvent e)
    {
        CheckpointTempResult(e);
    }

    public virtual void OnPlayerLap(CTmModeEvent e)
    {
        CheckpointTempResult(e);
    }

    public virtual void OnPlayerGiveUp(CTmModeEvent e) { }
    public virtual void OnPlayerAdded(CTmModeEvent e) { }
    public virtual void OnPlayerRemoved(CTmModeEvent e) { }
    public virtual void OnStunt(CTmModeEvent e) { }

    public virtual void OnUIEvent(CUIConfigEvent e) { }
    public virtual void OnXmlRpcEvent(CXmlRpcEvent e) { }
    public virtual void OnHttpEvent(CHttpEvent e) { }
    public virtual void OnLoop() { }
    public virtual void OnWarmUpLoop() { }
    public virtual void OnWarmUpEnd() { }
    public virtual void OnWarmUpNext() { }
    public virtual void OnGameLoop() { }
    public virtual void UpdateSettings() { }
    public virtual void OnGameEnd() { }
    public virtual void OnPodiumStart() { }
    public virtual void OnPodiumLoop() { }
    public virtual void OnPodiumEnd() { }

    public virtual void OnMapEnd()
    {
        if (ReloadMap) NextMapIndex -= 1;
        if (Reload) NextMapIndex = 0;
    }

    public virtual void BeforeMapEnd() { }

    public static void ToResult(CTmResult result, SRecord record)
    {
        result.Time = record.Time;
        result.Score = record.Score;
        result.NbRespawns = record.NbRespawns;

        result.Checkpoints.Clear();

        foreach (var checkpoint in record.Checkpoints)
        {
            result.Checkpoints.Add(checkpoint.Time);
        }
    }

    public static SRecord ToRecord(CTmResult result, float distance, CTmPlayer player)
    {
        SRecord record = new()
        {
            Login = player.User.Login,
            Time = result.Time,
            Score = result.Score,
            NbRespawns = result.NbRespawns,
            Distance = distance
        };

        foreach (var checkpoint in result.Checkpoints)
        {
            SCheckpoint c = new()
            {
                Time = checkpoint
            };

            record.Checkpoints.Add(c);
        }

        return record;
    }

    public void OpenNewLadder()
    {
        Ladder_CancelMatchRequest();
        Wait(() => !Ladder_RequestInProgress);
        Ladder_OpenMatch_Request();

        foreach (var score in Scores)
        {
            Ladder_AddPlayer(score);
        }

        Wait(() => !Ladder_RequestInProgress);
    }

    public void CloseLadder()
    {
        Ladder_CloseMatchRequest();
        Wait(() => !Ladder_RequestInProgress);
    }

    public void CancelLadder()
    {
        Ladder_CancelMatchRequest();
        Wait(() => !Ladder_RequestInProgress);
    }

    public int GetMapCountByEnvironment(string environment)
    {
        return MapList.Count(map => map.CollectionName == environment);
    }

    public void SpawnAllWaitingPlayers(int team, bool frozen)
    {
        foreach (var player in PlayersWaiting)
        {
            SpawnPlayer(player, team, -1);

            if (frozen)
            {
                player.RaceStartTime = -1;
            }
        }
    }

    public void SpawnAllWaitingPlayers(int team, int raceStartTime)
    {
        foreach (var player in PlayersWaiting)
        {
            SpawnPlayer(player, team, raceStartTime);
        }
    }

    public void SpawnAllWaitingPlayers(int team)
    {
        SpawnAllWaitingPlayers(team, frozen: false);
    }

    public void SpawnAllWaitingPlayers()
    {
        SpawnAllWaitingPlayers(team: 0, frozen: false);
    }

    public void UnspawnAllPlayers()
    {
        foreach (var player in Players)
        {
            UnspawnPlayer(player);
        }
    }

    protected static void Log(string scriptName, string text)
    {
        ManiaScript.Log($"[{scriptName}] {text}");
    }

    public string ReadFile(string fileName)
    {
        var request = Http.CreateGet("file://Media/" + fileName);
        Wait(() => request.IsCompleted);

        var result = request.Result;
        if (result == "")
        {
            Log(nameof(UniverseModeBase), "Warning: File located in file://Media/" + fileName + " does not exist or is empty.");
        }

        Http.Destroy(request);
        return result;
    }

    public static void ResetTempResult(CTmModeEvent e)
    {
        var tempRace = Netwrite<SRecord>.For(e.Player.Score);
        var t = tempRace.Get();
        t.Time = -1;
        t.Score = -1;
        t.NbRespawns = -1;
        t.Distance = -1;
        t.Speed = -1;
        t.Checkpoints.Clear();
        tempRace.Set(t);

        e.Player.CurRace.Score = 0; // nefunguje v independent
        e.Distance = 0.0f; // nefunguje v independent
    }

    public void CheckpointTempResult(CTmModeEvent e)
    {
        var tempRace = Netwrite<SRecord>.For(e.Player.Score);
        var t = tempRace.Get();

        SCheckpoint checkpoint;

        if (IndependantLaps)
        {
            checkpoint.Time = e.LapTime;
        }
        else
        {
            checkpoint.Time = e.RaceTime;
        }

        checkpoint.Score = e.StuntsScore;
        checkpoint.NbRespawns = e.NbRespawns;
        checkpoint.Distance = e.Distance;
        checkpoint.Speed = e.Speed;

        t.Checkpoints.Add(checkpoint);
        tempRace.Set(t);
    }

    public void FinishTempResult(CTmModeEvent e)
    {
        var tempRace = Netwrite<SRecord>.For(e.Player.Score);
        var t = tempRace.Get();

        if (IndependantLaps)
        {
            t.Time = e.LapTime;
        }
        else
        {
            t.Time = e.RaceTime;
        }

        t.Score = e.StuntsScore;
        t.NbRespawns = e.NbRespawns;
        t.Distance = e.Distance;
        t.Speed = e.Speed;
        tempRace.Set(t);

        CheckpointTempResult(e);
    }

    public void DestroyLayer(string layerName)
    {
        UIManager.UILayerDestroy(Layers[layerName]);
    }

    public void CreateLayer(string layerName, string universeXml)
    {
        var layer = UIManager.UILayerCreate();
        layer.ManialinkPage = ReadFile("Manialinks/Universe2/" + universeXml);
        Layers[layerName] = layer;
        UIManager.UIAll.UILayers.Add(layer);
    }

    public void Main()
    {
        // nothing
    }

    public void Loop()
    {
        Reload = false;

        BeforeServerInit();
        Settings();
        OnServerInit();

        BeforeServerStart();
        OnServerStart();

        while (!Reload && !Terminate && !ServerShutdownRequested)
        {
            BeforeMapInit();
            OnMapInit();

            BeforeMapLoad();
            RequestLoadMap();

            while (!MapLoaded)
            {
                WhileMapLoad();
                Yield();
            }

            OnMapLoad();

            if (EnableMapIntro)
            {
                BeforeMapIntroStart();
                OnMapIntroStart();

                while (!UIManager.UIAll.UISequenceIsCompleted && !ServerShutdownRequested)
                {
                    WhileMapIntro();
                    Yield();
                }

                OnMapIntroEnd();
            }

            BeforeMapStart();
            OnMapStart();

            // If warmups are set
            if (WarmUpNb > 0)
            {
                WarmUpStartTime = Now; // Start the first warmup now
                CutOffTimeLimit = WarmUpStartTime + WarmUpDuration * 1000 + 3000; // Set the time limit to be the warmup length
                OnWarmUpStart();
            }
            else
            {
                OnGameStart();
            }

            ReloadMap = false;

            while (!Reload && !ReloadMap && !Terminate && !ServerShutdownRequested && !MatchEndRequested)
            {
                BeforeEvent();

                foreach (var _E in PendingEvents) OnEvent(_E);
                foreach (var _E in UIManager.PendingEvents) OnUIEvent(_E);
                foreach (var _E in XmlRpc.PendingEvents) OnXmlRpcEvent(_E);
                foreach (var _E in Http.PendingEvents) OnHttpEvent(_E);

                OnLoop();

                if (WarmUpNb > 0 && CurrentWarmUpNb > 0)
                {
                    OnWarmUpLoop();

                    if (Now >= CutOffTimeLimit)
                    {
                        UnspawnAllPlayers();

                        Sleep(2000);

                        var barrier = Synchro_AddBarrier();
                        Wait(() => Synchro_BarrierReached(barrier) || ServerShutdownRequested);

                        CurrentWarmUpNb -= 1;

                        // One-run action after warmup ended
                        if (CurrentWarmUpNb == 0)
                        {
                            OnWarmUpEnd();
                            OnGameStart();
                        }
                        else
                        {
                            WarmUpStartTime = Now; // Start the first warmup now
                            CutOffTimeLimit = WarmUpStartTime + WarmUpDuration * 1000 + 3000; // Set the time limit to be the warmup length
                            OnWarmUpNext();
                        }
                    }
                }
                else
                {
                    OnGameLoop();
                }

                UpdateSettings();

                Yield();
            }

            OnGameEnd();


            if (!Reload && !ReloadMap && !Terminate && !ServerShutdownRequested)
            {
                PodiumStartTime = Now;
                CutOffTimeLimit = PodiumStartTime + ChatTime * 1000;

                UnspawnAllPlayers();
                UIManager.UIAll.UISequence = CUIConfig.EUISequence.Podium;

                OnPodiumStart();

                while (!Reload && !ReloadMap && !Terminate && !ServerShutdownRequested && Now - PodiumStartTime < ChatTime * 1000)
                {
                    OnPodiumLoop();
                    Yield();
                }

                OnPodiumEnd();
            }

            BeforeMapEnd();
            OnMapEnd();

            RequestUnloadMap();
            Wait(() => !MapLoaded);
        }
    }
}
