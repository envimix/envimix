using Envimix.Scripts.Libs.BigBang1112;
using System.Collections.Immutable;

namespace Envimix.Scripts.Modes.TrackMania;

public class UniverseModeBase : CTmMode, IContext
{
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

    [Setting(As = "Is channel server")]
    public bool IsChannelServer = false;

    [Setting(As = "Use script callbacks")]
    public bool UseScriptCallbacks = false;

    public bool Reload = true;
    public bool Terminate = false;
    public ImmutableArray<int> MapQueue;
    public int WarmUpStartTime = -1;
    public bool IsWarmUp = false;
    public int MatchCount;
    public int MapCount;
    public int PlayLoopCount;
    [Netwrite] public int PodiumStartTime { get; set; }

    public required Dictionary<string, CUILayer> Layers;

    [Netwrite] public new int CutOffTimeLimit { get; set; }
    [Netwrite] public new string MapPlayerModelName { get; set; } = "";

    [Netwrite] public int CurrentWarmUpNb { get; set; }

    public virtual void BeforeServerInit() { }
    public virtual void Settings() { }

    public virtual void OnServerInit()
    {
        Log(nameof(UniverseModeBase), "Initializing server...");
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

        while (!Synchro_BarrierReached(barrier) && !ServerShutdownRequested)
        {
            WhileSynchro();
            Yield();
        }
    }

    public virtual void WhileSynchro() { }

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
        Record.ResetTempResult(e);
    }

    public virtual void OnPlayerFinish(CTmModeEvent e)
    {
        Record.FinishTempResult(e, IndependantLaps);
    }

    public virtual void OnPlayerCheckpoint(CTmModeEvent e)
    {
        Record.CheckpointTempResult(e, IndependantLaps);
    }

    public virtual void OnPlayerLap(CTmModeEvent e)
    {
        Record.CheckpointTempResult(e, IndependantLaps);
    }

    public virtual void OnPlayerGiveUp(CTmModeEvent e) { }
    public virtual void OnPlayerAdded(CTmModeEvent e) { }
    public virtual void OnPlayerRemoved(CTmModeEvent e) { }
    public virtual void OnStunt(CTmModeEvent e) { }

    public virtual void OnUIEvent(CUIConfigEvent e) { }
    public virtual void OnXmlRpcEvent(CXmlRpcEvent e) { }
    public virtual void OnHttpEvent(CHttpEvent e) { }

    public virtual void OnLoop()
    {
        UpdateSpectatorLists();
    }

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
        //if (Reload) NextMapIndex = 0;
    }

    public virtual void BeforeMapEnd() { }

    public bool IsSolo()
    {
        return ScoreMgr is not null;
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

    public void DestroyLayer(string layerName)
    {
        UIManager.UILayerDestroy(Layers[layerName]);
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml, string toReplace, string replaceWith)
    {
        if (Layers.ContainsKey(layerName))
        {
            DestroyLayer(layerName);
        }

        var layer = UIManager.UILayerCreate();
        layer.Type = layerType;
        layer.ManialinkPage = TextLib.Replace(manialinkXml, toReplace, replaceWith);
        Layers[layerName] = layer;
        UIManager.UIAll.UILayers.Add(layer);
    }

    public void CreateLayer(string layerName, CUILayer.EUILayerType layerType, string manialinkXml)
    {
        CreateLayer(layerName, layerType, manialinkXml, "", "");
    }

    public bool Terminated()
    {
        return Reload || Terminate || ServerShutdownRequested;
    }

    public bool TerminatedMatch()
    {
        return Reload || Terminate || ServerShutdownRequested || MatchEndRequested;
    }

    public void SetLaps()
    {
        IndependantLaps = ForceLapsNb == 0 && MapIsLapRace;

        if (ForceLapsNb > 0)
        {
            NbLaps = ForceLapsNb;
        }
        else
        {
            NbLaps = -1;
        }
    }

    public int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
        }

        if (IndependantLaps)
        {
            return 0;
        }

        if (NbLaps == -1)
        {
            return Map.TMObjective_NbLaps;
        }

        return NbLaps;
    }

    public static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private string GetSpectatorTargetFromLogin(string login)
    {
        var spectatorTarget = Netread<string>.For(UIManager.GetUI(GetPlayer(login)));
        return spectatorTarget.Get();
    }

    public void UpdateSpectatorLists()
    {
        var spectatorLists = Netwrite<Dictionary<string, Dictionary<string, string>>>.For(Teams[0]);
        var spectators = Netwrite<Dictionary<string, bool>>.For(Teams[0]);

        foreach (var (spectatorTarget, unused) in spectatorLists.Get())
        {
            if (GetPlayer(spectatorTarget) is null)
            {
                spectatorLists.Get().Remove(spectatorTarget);
            }
        }

        foreach (var player in AllPlayers)
        {
            if (UIManager.GetUI(player) is null)
            {
                continue;
            }

            var spectatorTarget = Netread<string>.For(UIManager.GetUI(player));

            // Remove spectators that are no longer spectating this player
            foreach (var (spectator, unused) in spectatorLists.Get())
            {
                if (spectator == player.User.Login)
                {
                    continue;
                }

                if (spectatorLists.Get()[spectator].ContainsKey(player.User.Login))
                {
                    spectatorLists.Get()[spectator].Remove(player.User.Login);
                }

                if (spectatorLists.Get()[spectator].Count == 0)
                {
                    spectatorLists.Get().Remove(spectator);
                }
            }

            // Add spectators that are spectating this player
            if (spectatorTarget.Get() != "" && player.User.Login != spectatorTarget.Get())
            {
                if (!spectatorLists.Get().ContainsKey(spectatorTarget.Get()))
                {
                    spectatorLists.Get()[spectatorTarget.Get()] = new();
                }

                spectatorLists.Get()[spectatorTarget.Get()][player.User.Login] = player.User.Name;
            }

            spectators.Get()[player.User.Login] = false;
        }

        foreach (var spectator in Spectators)
        {
            spectators.Get()[spectator.User.Login] = true;
        }
    }

    private void SendXmlRpcCallback(string callbackName, string payload)
    {
        if (UseScriptCallbacks)
        {
            XmlRpc.SendCallback(callbackName, payload);
        }
    }

    private void SendXmlRpcCallbackArray(string callbackName, IList<string> data)
    {
        if (UseScriptCallbacks)
        {
            XmlRpc.SendCallbackArray(callbackName, (string[])data);
        }
    }

    private string GetXmlRpcTimePayload()
    {
        return $"{{\"time\":{Now}}}";
    }

    private string GetXmlRpcCountPayload(int count)
    {
        return $"{{\"count\":{count},\"time\":{Now}}}";
    }

    private string GetXmlRpcMapPayload(bool includeCount, bool includeRestarted)
    {
        var payload = "{";

        if (includeCount)
        {
            payload = $"{payload}\"count\":{MapCount},";
        }

        if (includeRestarted)
        {
            payload = $"{payload}\"restarted\":false,";
        }

        string isLapRace;
        if (Map.MapInfo.TMObjective_IsLapRace)
        {
            isLapRace = "true";
        }
        else
        {
            isLapRace = "false";
        }

        payload = $"{payload}\"time\":{Now},\"map\":{{\"uid\":\"{Map.MapInfo.MapUid}\",\"name\":\"{Map.MapInfo.Name}\",\"filename\":\"{Map.MapInfo.FileName}\",\"author\":\"{Map.MapInfo.AuthorLogin}\",\"environment\":\"{Map.MapInfo.CollectionName}\",\"mood\":\"{Map.DecorationName}\",\"bronzetime\":{Map.MapInfo.TMObjective_BronzeTime},\"silvertime\":{Map.MapInfo.TMObjective_SilverTime},\"goldtime\":{Map.MapInfo.TMObjective_GoldTime},\"authortime\":{Map.MapInfo.TMObjective_AuthorTime},\"copperprice\":{Map.MapInfo.CopperPrice},\"laprace\":{isLapRace},\"nblaps\":{Map.TMObjective_NbLaps},\"maptype\":\"{Map.MapInfo.MapType}\",\"mapstyle\":\"{Map.MapInfo.MapStyle}\"}}}}";
        return payload;
    }

    private void SendXmlRpcEventCallbacks(CTmModeEvent e)
    {
        switch (e.Type)
        {
            case CTmModeEvent.EType.StartLine:
                SendXmlRpcCallbackArray("LibXmlRpc_OnStartLine", new[] { e.Player.User.Login });
                break;
            case CTmModeEvent.EType.WayPoint:
                SendXmlRpcCallbackArray("LibXmlRpc_OnWayPoint", new[]
                {
                    e.Player.User.Login,
                    e.BlockId.ToString(),
                    e.RaceTime.ToString(),
                    e.CheckpointInRace.ToString(),
                    e.IsEndRace.ToString(),
                    e.LapTime.ToString(),
                    e.CheckpointInLap.ToString(),
                    e.IsEndLap.ToString()
                });

                if (e.IsEndRace || (e.IsEndLap && IndependantLaps))
                {
                    var finishTime = e.RaceTime;
                    if (IndependantLaps)
                    {
                        finishTime = e.LapTime;
                    }

                    SendXmlRpcCallbackArray("LibXmlRpc_OnPlayerFinish", new[]
                    {
                        e.Player.User.Login,
                        e.BlockId.ToString(),
                        finishTime.ToString()
                    });
                }

                break;
            case CTmModeEvent.EType.GiveUp:
                SendXmlRpcCallbackArray("LibXmlRpc_OnGiveUp", new[] { e.Player.User.Login });
                break;
            case CTmModeEvent.EType.Respawn:
                SendXmlRpcCallbackArray("LibXmlRpc_OnRespawn", new[]
                {
                    e.Player.User.Login,
                    e.BlockId.ToString(),
                    e.CheckpointInRace.ToString(),
                    e.CheckpointInLap.ToString(),
                    e.NbRespawns.ToString()
                });
                break;
            case CTmModeEvent.EType.Stunt:
                SendXmlRpcCallbackArray("LibXmlRpc_OnStunt", new[]
                {
                    e.Player.User.Login,
                    e.Points.ToString(),
                    e.Combo.ToString(),
                    e.StuntsScore.ToString(),
                    e.Factor.ToString(),
                    e.StuntFigure.ToString(),
                    e.Angle.ToString(),
                    e.IsStraight.ToString(),
                    e.IsReverse.ToString(),
                    e.IsMasterJump.ToString()
                });
                break;
        }
    }

    public void Main()
    {
        // nothing
    }

    public void Loop()
    {
        Reload = false;

        SendXmlRpcCallback("Maniaplanet.StartServer_Start", $"{{\"restarted\":false,\"mode\":{{\"updated\":false,\"name\":\"{nameof(UniverseModeBase)}\"}},\"time\":{Now}}}");
        BeforeServerInit();
        Settings();
        OnServerInit();

        BeforeServerStart();
        OnServerStart();
        SendXmlRpcCallback("Maniaplanet.StartServer_End", $"{{\"restarted\":false,\"mode\":{{\"updated\":false,\"name\":\"{nameof(UniverseModeBase)}\"}},\"time\":{Now}}}");

        while (!Terminated())
        {
            MatchCount += 1;
            SendXmlRpcCallback("Maniaplanet.StartMatch_Start", GetXmlRpcCountPayload(MatchCount));
            MapCount += 1;
            BeforeMapInit();
            OnMapInit();

            BeforeMapLoad();
            SendXmlRpcCallback("Maniaplanet.LoadingMap_Start", $"{{\"restarted\":false,\"time\":{Now}}}");
            RequestLoadMap();

            while (!MapLoaded)
            {
                WhileMapLoad();
                Yield();
            }

            OnMapLoad();
            SendXmlRpcCallback("Maniaplanet.LoadingMap_End", GetXmlRpcMapPayload(false, true));
            SendXmlRpcCallback("Maniaplanet.StartMap_Start", GetXmlRpcMapPayload(true, true));

            if (EnableMapIntro)
            {
                BeforeMapIntroStart();
                OnMapIntroStart();

                while (!UIManager.UIAll.UISequenceIsCompleted && !Terminated())
                {
                    WhileMapIntro();
                    Yield();
                }

                OnMapIntroEnd();
            }

            BeforeMapStart();
            OnMapStart();
            SendXmlRpcCallback("Maniaplanet.StartMap_End", GetXmlRpcMapPayload(true, true));
            PlayLoopCount += 1;
            SendXmlRpcCallback("Maniaplanet.StartPlayLoop", GetXmlRpcCountPayload(PlayLoopCount));

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

            while (!TerminatedMatch())
            {
                BeforeEvent();

                foreach (var _E in PendingEvents)
                {
                    SendXmlRpcEventCallbacks(_E);
                    OnEvent(_E);
                }
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

            SendXmlRpcCallback("Maniaplanet.EndPlayLoop", GetXmlRpcCountPayload(PlayLoopCount));
            OnGameEnd();
            SendXmlRpcCallback("Maniaplanet.EndMatch_Start", GetXmlRpcCountPayload(MatchCount));

            if (!Terminated())
            {
                PodiumStartTime = Now;
                CutOffTimeLimit = PodiumStartTime + ChatTime * 1000;

                UnspawnAllPlayers();
                UIManager.UIAll.UISequence = CUIConfig.EUISequence.Podium;

                OnPodiumStart();
                SendXmlRpcCallback("Maniaplanet.Podium_Start", GetXmlRpcTimePayload());

                while (!Terminated() && Now - PodiumStartTime < ChatTime * 1000)
                {
                    OnPodiumLoop();
                    Yield();
                }

                OnPodiumEnd();
                SendXmlRpcCallback("Maniaplanet.Podium_End", GetXmlRpcTimePayload());
            }

            SendXmlRpcCallback("Maniaplanet.EndMap_Start", GetXmlRpcMapPayload(true, false));
            BeforeMapEnd();
            OnMapEnd();
            SendXmlRpcCallback("Maniaplanet.EndMap_End", GetXmlRpcMapPayload(true, false));

            SendXmlRpcCallback("Maniaplanet.UnloadingMap_Start", GetXmlRpcMapPayload(false, false));
            RequestUnloadMap();
            Wait(() => !MapLoaded);
            SendXmlRpcCallback("Maniaplanet.UnloadingMap_End", GetXmlRpcTimePayload());
            SendXmlRpcCallback("Maniaplanet.EndMatch_End", GetXmlRpcCountPayload(MatchCount));
        }

        SendXmlRpcCallback("Maniaplanet.EndServer_Start", GetXmlRpcTimePayload());
        SendXmlRpcCallback("Maniaplanet.EndServer_End", GetXmlRpcTimePayload());
    }
}
