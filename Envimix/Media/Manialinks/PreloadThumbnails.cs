namespace Envimix.Media.Manialinks;

public class PreloadThumbnails : CMlScript, IContext
{
    public void Main()
    {
        foreach (var campaign in DataFileMgr.Campaigns)
        {
            foreach (var groups in campaign.MapGroups)
            {
                foreach (var mapInfo in groups.MapInfos)
                {
                    PreloadImage($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                }
            }
        }

        return;
    }

    public void Loop()
    {

    }
}
