using System.Collections.Immutable;

namespace Envimix.Scripts.Libs.BigBang1112;

public static class Record
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
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public ImmutableArray<SCheckpoint> Checkpoints;
    }

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
        e.Distance = 0; // nefunguje v independent
    }

    public static void CheckpointTempResult(CTmModeEvent e, bool independentLaps)
    {
        var tempRace = Netwrite<SRecord>.For(e.Player.Score);
        var t = tempRace.Get();

        SCheckpoint checkpoint = new()
        {
            Score = e.StuntsScore,
            NbRespawns = e.NbRespawns,
            Distance = e.Distance,
            Speed = e.Speed
        };

        if (independentLaps)
        {
            checkpoint.Time = e.LapTime;
        }
        else
        {
            checkpoint.Time = e.RaceTime;
        }

        t.Checkpoints.Add(checkpoint);
        tempRace.Set(t);
    }

    public static void FinishTempResult(CTmModeEvent e, bool independentLaps)
    {
        var tempRace = Netwrite<SRecord>.For(e.Player.Score);
        var t = tempRace.Get();

        if (independentLaps)
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

        CheckpointTempResult(e, independentLaps);
    }
}
