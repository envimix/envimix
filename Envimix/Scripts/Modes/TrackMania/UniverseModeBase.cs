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

    public bool Reload = true;
    public bool Terminate = false;
    public bool ReloadMap = false;
    public IList<int> MapQueue;
    public int WarmUpStartTime = -1;
    public bool IsWarmUp = false;
    public int PodiumStartTime = -1;

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

    public void SetCutOffTimeLimit(int value)
    {
        CutOffTimeLimit = value;
        var netCutOffTimeLimit = Netwrite<int>.For(Teams[0]);
        netCutOffTimeLimit.Set(value);
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

    protected static void Log(string scriptName, string text)
    {
        ManiaScript.Log($"[{scriptName}] {text}");
    }

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
        var netCurrentWarmUpNb = Netwrite<int>.For(Teams[0]);
        netCurrentWarmUpNb.Set(WarmUpNb);

        WarmUpStartTime = -1;
        IsWarmUp = netCurrentWarmUpNb > 0;
        PodiumStartTime = -1;

        MatchEndRequested = false;

        UIManager.UIAll.UISequence = CUIConfig.EUISequence.None;
    }

    public virtual void OnMapInit() { }
    public virtual void BeforeMapLoad() { }
    public virtual void WhileMapLoad() { }
    public virtual void OnMapLoad() { }
    public virtual void BeforeMapIntroStart() { }
    public virtual void OnMapIntroStart() { }
    public virtual void WhileMapIntro() { }
    public virtual void OnMapIntroEnd() { }
    public virtual void BeforeMapStart() { }
    public virtual void OnMapStart() { }
    public virtual void OnWarmUpStart() { }
    public virtual void OnGameStart() { }
    public virtual void BeforeEvent() { }
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
    public virtual void OnMapEnd() { }
    public virtual void BeforeMapEnd() { }

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
                SetCutOffTimeLimit(WarmUpStartTime + WarmUpDuration * 1000 + 3000); // Set the time limit to be the warmup length
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

                //foreach (Event in PendingEvents) +++OnEvent+++

                //foreach (Event in UIManager.PendingEvents) +++OnUIEvent+++

                //foreach (Event in XmlRpc.PendingEvents) +++OnXmlRpcEvent+++

                //foreach (Event in Http.PendingEvents) +++OnHttpEvent+++

                OnLoop();

                var netCurrentWarmUpNb = Netwrite<int>.For(Teams[0]);

                if (WarmUpNb > 0 && netCurrentWarmUpNb > 0)
                {
                    OnWarmUpLoop();

                    if (Now >= CutOffTimeLimit)
                    {
                        UnspawnAllPlayers();

                        Sleep(2000);

                        var barrier = Synchro_AddBarrier();
                        Wait(() => Synchro_BarrierReached(barrier) || ServerShutdownRequested);

                        netCurrentWarmUpNb -= 1;

                        // One-run action after warmup ended
                        if (netCurrentWarmUpNb == 0)
                        {
                            OnWarmUpEnd();
                            OnGameStart();
                        }
                        else
                        {
                            WarmUpStartTime = Now; // Start the first warmup now
                            SetCutOffTimeLimit(WarmUpStartTime + WarmUpDuration * 1000 + 3000); // Set the time limit to be the warmup length
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
                SetCutOffTimeLimit(PodiumStartTime + ChatTime * 1000);

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
