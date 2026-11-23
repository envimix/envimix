namespace Envimix.Media.Manialinks;

public class Secret : CMlScriptIngame, IContext
{
    [Netread] public string EnvimixWebAPI { get; set; }
    [Local(LocalFor.LocalUser)] public string EnvimixTurboUserToken { get; set; } = "";

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

        // the netwrite may need a moment to sync, don't wait forever outside of Envimix modes
        var waitStart = Now;
        Wait(() => EnvimixWebAPI != "" || Now - waitStart > 5000);

        if (EnvimixWebAPI == "" || EnvimixTurboUserToken == "")
        {
            Log("Secret found, but it couldn't be reported (missing web API or user token).");
            return;
        }

        // reported on every discovery, the webapi ignores duplicates, so a failed report is retried by finding it again
        var request = Http.CreatePost($"{EnvimixWebAPI}/maps/{mapUid}/secret", "", $"Authorization: Bearer {EnvimixTurboUserToken}");
        Wait(() => request.IsCompleted);

        if (request.StatusCode == 200)
        {
            Log("Secret reported.");
        }
        else
        {
            Log($"Failed to report the secret (status code: {request.StatusCode}).");
        }

        Http.Destroy(request);
    }

    public void Loop()
    {
    }
}
