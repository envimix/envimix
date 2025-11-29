using System.Collections.Immutable;

namespace Envimix.Scripts.Libs.Envimix;

public static class Loading
{
    private static IList<float> GetTimeOpacities(string currentLocalDateText)
    {
        var Time = TextLib.Split(" ", currentLocalDateText)[1];
        var Hour = TextLib.ToReal(TextLib.Split(":", Time)[0]);
        var Minute = TextLib.ToReal(TextLib.Split(":", Time)[1]);
        var RealHour = Hour + Minute / 60;

        var Opacities = new[] { 0f, 0f, 0f, 0f };
        if (RealHour >= 4 && RealHour <= 8)
        {
            Opacities[0] = (RealHour - 4) / 4f;
            Opacities[1] = 0f;
            Opacities[2] = 0f;
            Opacities[3] = 1 - (RealHour - 4) / 4f;
        }
        else if (RealHour >= 8 && RealHour <= 12)
        {
            Opacities[0] = 1 - (RealHour - 8) / 4f;
            Opacities[1] = (RealHour - 8) / 4f;
            Opacities[2] = 0f;
            Opacities[3] = 0f;
        }
        else if (RealHour >= 12 && RealHour <= 14)
        {
            Opacities[0] = 0f;
            Opacities[1] = 1f;
            Opacities[2] = 0f;
            Opacities[3] = 0f;
        }
        else if (RealHour >= 14 && RealHour <= 18)
        {
            Opacities[0] = 0f;
            Opacities[1] = 1 - (RealHour - 14) / 4f;
            Opacities[2] = (RealHour - 14) / 4f;
            Opacities[3] = 0f;
        }
        else if (RealHour >= 18 && RealHour <= 22)
        {
            Opacities[0] = 0f;
            Opacities[1] = 0f;
            Opacities[2] = 1 - (RealHour - 18) / 4f;
            Opacities[3] = (RealHour - 18) / 4f;
        }
        else if (RealHour >= 22 || RealHour <= 4)
        {
            Opacities[0] = 0f;
            Opacities[1] = 0f;
            Opacities[2] = 0f;
            Opacities[3] = 1f;
        }
        return Opacities;
    }

    public static string GetTip()
    {
        ImmutableArray<string> tips = new()
        {
            "TIP: Donating to BigBang1112 helps covering the server costs to keep the title pack running: ~20€/month (including domains)",
            "FUN FACT: Activity points have a base limit of 1000. If you received over 1000 activity points on a single combination, then it was given via extra points from a validation bonus.",
            "TIP: TrafficCar is a very fast car which has a lower gravity similar to ValleyCar. If a combination is possible with ValleyCar, then it is certainly possible with TrafficCar.",
            "FUN FACT: TrafficCar doesn't have an official car model. The one commonly used is based off Sommer SportCar, designed by Stéphane Sommer for the Valley environment and ported in by puriPictures.",
            "FUN FACT: Activity points are based on the WR/PB scoring formula invented by Poutrel.",
            "TIP: Activity points are tolerant to subtle differences on the leaderboard. If you're close to WR, you will get similar amount of activity points as the WR.",
            "TIP: Activity points are valuable on combinations with less records, while skillpoints are more valuable on combinations with many records.",
            "TIP: By validating maps, you receive extra activity points. The bonus becomes bigger with the age of the title pack. But be careful, someone who doesn't care about the points can grab your desired validation if you wait for too long.",
            "TIP: Think of the difficulty/quality rating bar as more filled => more difficult/superior.",
            "TIP: Skillpoints are rank-based. Just move up the leaderboard to receive exponentially more skillpoints. Improvement without a new rank doesn't count!",
            "FUN FACT: Trackmania Turbo envimix is directly possible without this ManiaPlanet port, however, the leaderboard features are limited and TMUF cars are not possible without the GameData exploit.",
            "FUN FACT: Envimix is a slightly incorrect naming of the project. It's a signature name taken from TMUF environment mixing map bases that were a lot popular outside of TMUnlimiter. Carmix would be the correct challenge name, especially with the latest TM2020 updates.",
            "TIP: All combinations are playable! The maps are validated using the default car and the gamemode just changes the car on the fly. Validate them yourself!",
            "FUN FACT: All envimix replays driven in Envimix title packs are invalid! The validation test is trying to use the default car instead of the car specified in the replay file.",
            "FUN FACT: Nadeo disallowed changing the default car of a map in Summer 2024 update of TM2020. It was restored by the Editor++ plugin 2 months later. The official restriction is there to this day.",
            "TIP: Reaching 100% envimix completion is impossible. Start with lower goals and set the final one to 90%-95% to keep the motivation going.",
            "TIP: Completion percentage and Track of the Day finishes don't count towards the activity points.",
            "FUN FACT: Item set includes two non-functioning helicopter rotors, one of which represents a simulation of a humongous tornado hitting the Stadium buildings.",
            "FUN FACT: Several ingame UI pieces are taken straight from the Challenge title pack project.",
            "FUN FACT: This is the first title pack fully written in C# (translated to ManiaScript using ManiaScriptSharp)."
        };

        var randomIndex = MathLib.Rand(0, tips.Length - 1);
        return tips[randomIndex];
    }

    public static string GetLoadingManialink(CMapInfo mapInfo, string currentLocalDateText)
	{
		var isEnvimix = 0;
		var isExplore = 0;

		if (mapInfo.MapType == "TrackMania\\Envimix" || mapInfo.MapType == "Envimix")
		{
            isEnvimix = 1;
        }

        if (mapInfo.MapType == "TrackMania\\EnvimixExplore" || mapInfo.MapType == "EnvimixExplore")
        {
            isEnvimix = 1;
        }

        var opacities = GetTimeOpacities(currentLocalDateText);

        var loadingManialink = "<manialink version=\"3\" name=\"Loading\">";
        loadingManialink = $"{loadingManialink}<frame z-index=\"0\">";
        loadingManialink = $"{loadingManialink}    <quad pos=\"0 0\" z-index=\"-6\" size=\"320 180\" bgcolor=\"000D\" halign=\"center\" valign=\"center\" image=\"file://Media/Images/Loading/{mapInfo.CollectionName}_Sunrise.jpg\" opacity=\"{opacities[0]*.9}\" hidden=\"1\"/>";
        loadingManialink = $"{loadingManialink}    <quad pos=\"0 0\" z-index=\"-5\" size=\"320 180\" bgcolor=\"000D\" halign=\"center\" valign=\"center\" image=\"file://Media/Images/Loading/{mapInfo.CollectionName}_Day.jpg\" opacity=\"{opacities[1]*.9}\" hidden=\"1\"/>";
        loadingManialink = $"{loadingManialink}    <quad pos=\"0 0\" z-index=\"-4\" size=\"320 180\" bgcolor=\"000D\" halign=\"center\" valign=\"center\" image=\"file://Media/Images/Loading/{mapInfo.CollectionName}_Sunset.jpg\" opacity=\"{opacities[2]*.9}\" hidden=\"1\"/>";
        loadingManialink = $"{loadingManialink}    <quad pos=\"0 0\" z-index=\"-3\" size=\"320 180\" bgcolor=\"000D\" halign=\"center\" valign=\"center\" image=\"file://Media/Images/Loading/{mapInfo.CollectionName}_Night.jpg\" opacity=\"{opacities[3]*.9}\" hidden=\"1\"/>";
        loadingManialink = $"{loadingManialink}    <quad pos=\"0 0\" z-index=\"-2\" size=\"320 180\" bgcolor=\"111E\" halign=\"center\" valign=\"center\"/>";
        loadingManialink = $"{loadingManialink}    <frame pos=\"60\">";
        loadingManialink = $"{loadingManialink}        <frame pos=\"0 5\">";
        loadingManialink = $"{loadingManialink}            <quad pos=\"0 0\" z-index=\"0\" size=\"88 12.675\" image=\"file://Media/Images/EnvimixSmall.png\" halign=\"center\" valign=\"center\"/>";
        loadingManialink = $"{loadingManialink}            <quad pos=\"0.5 -0.5\" z-index=\"0\" size=\"88 12.675\" image=\"file://Media/Images/EnvimixSmall.png\" halign=\"center\" modulatecolor=\"222\" valign=\"center\"/>";
        loadingManialink = $"{loadingManialink}        </frame>";
        loadingManialink = $"{loadingManialink}        <label pos=\"25 -5\" z-index=\"0\" size=\"30 5\" text=\"LOADING...\" halign=\"center\" textemboss=\"1\" textfont=\"BiryaniDemiBold\"/>";
        loadingManialink = $"{loadingManialink}    </frame>";
        loadingManialink = $"{loadingManialink}    <frame>";
        loadingManialink = $"{loadingManialink}        <quad pos=\"-80 0\" z-index=\"-1\" size=\"55 55\" halign=\"center\" valign=\"center\" style=\"Bgs1\" substyle=\"BgButtonGlow\" opacity=\".5\"/>";
        loadingManialink = $"{loadingManialink}        <quad pos=\"-80 0\" z-index=\"0\" size=\"50 50\" bgcolor=\"000\" halign=\"center\" valign=\"center\" image=\"file://Thumbnails/MapUid/{mapInfo.MapUid}\"/>";
        loadingManialink = $"{loadingManialink}        <quad pos=\"-80 0\" z-index=\"1\" size=\"51 51\" halign=\"center\" valign=\"center\" style=\"Bgs1\" substyle=\"BgColorContour\"/>";
        loadingManialink = $"{loadingManialink}        <label pos=\"-50 20\" z-index=\"0\" size=\"60 5\" textprefix=\"$t\" text=\"Heading over to...\" textemboss=\"1\" textfont=\"BiryaniDemiBold\" textsize=\"2\"/>";
        loadingManialink = $"{loadingManialink}        <frame pos=\"-50 15\" clip=\"True\" clipsizen=\"60 6\" clipposn=\"30 -3\">";
        loadingManialink = $"{loadingManialink}            <label z-index=\"0\" size=\"200 6\" text=\"{mapInfo.Name}\" textemboss=\"1\" textfont=\"Oswald\" textsize=\"5\"/>";
        loadingManialink = $"{loadingManialink}        </frame>";
        loadingManialink = $"{loadingManialink}        <frame pos=\"-50 8\" clip=\"True\" clipsizen=\"60 6\" clipposn=\"30 -3\" hidden=\"0\">";
        loadingManialink = $"{loadingManialink}            <label z-index=\"0\" size=\"60 6\" text=\"by {mapInfo.AuthorNickName}\" textemboss=\"1\" textfont=\"Oswald\" textsize=\"2\"/>";
        loadingManialink = $"{loadingManialink}        </frame>";
        loadingManialink = $"{loadingManialink}    </frame>";
        loadingManialink = $"{loadingManialink}    <label pos=\"0 -50\" z-index=\"0\" size=\"210 20\" text=\"{GetTip()}\" halign=\"center\" autonewline=\"1\" textsize=\"2\" textfont=\"BiryaniDemiBold\"/>";
        loadingManialink = $"{loadingManialink}</frame>";
        loadingManialink = $"{loadingManialink}</manialink>";

        return loadingManialink;
	}
}