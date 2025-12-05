namespace Envimix.Media.Manialinks.Universe2;

public class MusicPlayer : CTmMlScriptIngame, IContext
{
    public int LatestCheckpointForPlayers = -1;
    public int LastVoicePlayedAt = -1;

    public MusicPlayer()
    {
        RaceEvent += (e) =>
        {
            if (e.Player.User.Login == GetPlayer().User.Login)
            {
                switch (e.Type)
                {
                    case CTmRaceClientEvent.EType.Respawn:
                        // switch track
                        break;
                    case CTmRaceClientEvent.EType.StopEngine:
                        //Music.NextVariant();
                        break;
                    case CTmRaceClientEvent.EType.StartEngine:
                        //Music.NextVariant();
                        break;
                    case CTmRaceClientEvent.EType.WayPoint:
                        if (e.IsEndRace)
                        {
                            //Music.NextVariant();
                        }
                        else if (e.IsEndLap)
                        {
                            if (LoadedTitle.TitleId == "Envimix_Turbo@bigbang1112" && (LastVoicePlayedAt == -1 || Now - LastVoicePlayedAt > 3000))
                            {
                                var laps = e.Player.CurrentNbLaps + 1;
                                var lapsToGo = NbLaps - e.Player.CurrentNbLaps;
                                if (laps == NbLaps)
                                {
                                    Audio.PlaySoundEvent($"file://Media/Sounds/Voices/voice-lap-final.wav", 1);
                                    LastVoicePlayedAt = Now;
                                }
                                else if (lapsToGo > 0 && lapsToGo <= 5)
                                {
                                    Audio.PlaySoundEvent($"file://Media/Sounds/Voices/voice-lap-{lapsToGo}.wav", 1);
                                    LastVoicePlayedAt = Now;
                                }
                            }
                            //M_LapTrackNeeded = True;
                        }
                        else
                        {
                            int difference;

                            if (e.Player.Score.BestRace is null || e.Player.Score.BestRace.Checkpoints.Count == 0)
                            {
                                difference = 0;
                            }
                            else if (IndependantLaps)
                            {
                                difference = e.LapTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInLap];
                            }
                            else
                            {
                                difference = e.RaceTime - e.Player.Score.BestRace.Checkpoints[e.CheckpointInRace];
                            }

                            if (LoadedTitle.TitleId == "Envimix_Turbo@bigbang1112")
                            {
                                if (MathLib.Rand(0, 5) == 0 && (LastVoicePlayedAt == -1 || Now - LastVoicePlayedAt > 3000))
                                {
                                    if (difference > 0)
                                    {
                                        Audio.PlaySoundEvent($"file://Media/Sounds/Voices/voice-checkpoint-no-{MathLib.Rand(1, 38)}.wav", 1);
                                    }
                                    else
                                    {
                                        Audio.PlaySoundEvent($"file://Media/Sounds/Voices/voice-checkpoint-yes-{MathLib.Rand(1, 23)}.wav", 1);
                                    }
                                    LastVoicePlayedAt = Now;
                                }
                            }

                            /*if (e.CheckpointInRace > LatestCheckpointForPlayers)
                            {
                                LatestCheckpointForPlayers = e.CheckpointInRace;
                                Music.NextVariant();
                            }*/
                        }
                        break;
                    case CTmRaceClientEvent.EType.Impact:
                        if (LoadedTitle.TitleId == "Envimix_Turbo@bigbang1112")
                        {
                            if (MathLib.Rand(0, 3) == 0 && (LastVoicePlayedAt == -1 || Now - LastVoicePlayedAt > 3000))
                            {
                                Audio.PlaySoundEvent($"file://Media/Sounds/Voices/voice-carhit-{MathLib.Rand(1, 16)}.wav", 1);
                                LastVoicePlayedAt = Now;
                            }
                        }
                        break;
                }
            }
        };
    }

    private CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    public void Main()
    {
        
    }

    public void Loop()
    {
        /*var ratio = GetPlayer().DisplaySpeed / 200f;
        if (ratio > 1) ratio = 1;
        if (ratio < 0.2f) ratio = 0.2f;
        Music.LPF_CutoffRatio = ratio;
        Music.LPF_Q = 2;*/
    }
}
