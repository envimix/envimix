namespace Envimix.Media.Manialinks;

public class Secret : CMlScriptIngame, IContext
{
    public void Main()
    {
        Wait(() => Map is not null && Map.MapInfo.MapUid != "");

        var persistent_SecretPlace = Persistent<Dictionary<string, int>>.For(LocalUser);
        var mapUid = Map.MapInfo.MapUid;
        if (!persistent_SecretPlace.Get().ContainsKey(mapUid))
        {
            persistent_SecretPlace.Get()[mapUid] = 0;
        }
        persistent_SecretPlace.Get()[mapUid] += 1;
        return;
    }

    public void Loop()
    {
    }
}
