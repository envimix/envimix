namespace Envimix.Media.Manialinks.Universe2;

public class Endscreen : CTmMlScriptIngame, IContext
{
    [ManialinkControl] public required CMlFrame FramePopup;

    public Endscreen()
    {
        RaceEvent += (e) =>
        {
            switch (e.Type)
            {
                case CTmRaceClientEvent.EType.WayPoint:
                    if (e.IsEndRace)
                    {
                        FramePopup.Show();

                        // show full endscreen in 2 seconds, hide the other UI after the 2 seconds, move blur background from bottom to top for interesting effect
                    }
                    break;
                case CTmRaceClientEvent.EType.Respawn:
                    FramePopup.Hide();
                    break;
            }
        };
    }

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    bool IsVisible()
    {
        return true;
    }

    public void Main()
    {
        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {

    }
}