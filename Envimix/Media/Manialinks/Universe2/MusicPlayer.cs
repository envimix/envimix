namespace Envimix.Media.Manialinks.Universe2;

public class MusicPlayer : CTmMlScriptIngame, IContext
{
    public CAudioSourceMusic Music;
    public int LatestCheckpointForPlayers = -1;

    public MusicPlayer()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.Respawn:
                    // switch track
                    break;
                case CTmRaceClientEvent.EType.StopEngine:
                    Music.NextVariant();
                    break;
                case CTmRaceClientEvent.EType.StartEngine:
                    Music.NextVariant();
                    break;
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace)
                    {
                        Music.NextVariant();
                    }
                    else if (e.IsEndLap)
                    {
                        //M_LapTrackNeeded = True;
                    }
                    else if (e.CheckpointInRace > LatestCheckpointForPlayers)
                    {
                        LatestCheckpointForPlayers = e.CheckpointInRace;
                        Music.NextVariant();
                    }
                    break;
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
        Music = Audio.CreateMusic("file://Media/Sounds/87 Bustre Combine.zip");
        Music.EnableSegment("loop");
        Music.FadeDuration = .35f;
        Music.FadeTracksDuration = 1;
        Music.UpdateMode = CAudioSourceMusic.EUpdateMode.OnNextBeat;
        Music.Volume = 1f;
        Log(Music);
        
        Music.Play();
    }

    public void Loop()
    {
        var ratio = GetPlayer().DisplaySpeed / 200f;
        if (ratio > 1) ratio = 1;
        if (ratio < 0.2f) ratio = 0.2f;
        Music.LPF_CutoffRatio = ratio;
        Music.LPF_Q = 1;
    }
}
