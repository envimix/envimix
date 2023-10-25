namespace Envimix.Media.Manialinks.Universe2;

public class MultiplayerEvents : CTmMlScriptIngame, IContext
{
    public MultiplayerEvents()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.Respawn:
                    SendCustomEvent("Respawn", new[] { "" });
                    break;
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace)
                    {
                        SendCustomEvent("Finish", new[] { e.RaceTime.ToString() });
                    }
                    else if (e.IsEndLap)
                    {
                        SendCustomEvent("Lap", new[] { e.RaceTime.ToString() });
                    }
                    else
                    {
                        SendCustomEvent("Checkpoint", new[] { e.RaceTime.ToString() });
                    }
                    break;
            }
        };
    }

    public void Main()
    {

    }

    public void Loop()
    {

    }
}
