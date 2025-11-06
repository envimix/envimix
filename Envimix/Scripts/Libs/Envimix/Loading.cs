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
        loadingManialink = $"{loadingManialink}            <quad pos=\"0.5 -0.5\" z-index=\"0\" size=\"88 12.675\" image=\"file://Media/Images/EnvimixSmall.png\" halign=\"center\" modulatecolor=\"640\" valign=\"center\"/>";
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
        loadingManialink = $"{loadingManialink}</frame>";
        loadingManialink = $"{loadingManialink}</manialink>";

        return loadingManialink;
	}
}