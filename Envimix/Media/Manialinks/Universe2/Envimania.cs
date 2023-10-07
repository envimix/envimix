using System.Collections.Immutable;

namespace Envimix.Media.Manialinks.Universe2;

public class Envimania : CTmMlScriptIngame, IContext
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

    public struct SUserInfo
    {
        public string Login;
        public string Nickname;
        public string Zone;
        public string AvatarUrl;
        public string Language;
        public string Description;
        public Vec3 Color;
        public string SteamUserId;
        public int FameStars;
        public float LadderPoints;
    }

    public struct SEnvimaniaRecord
    {
        public SUserInfo User;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public bool Verified;
    }

    public struct SEnvimaniaRecordsFilter
    {
        public string Car;
        public int Gravity;
        public int Laps;
        public string Type;
    }

    public struct SEnvimaniaRecordsResponse
    {
        public SEnvimaniaRecordsFilter Filter;
        public ImmutableArray<SEnvimaniaRecord> Records;
    }

    [ManialinkControl] public required CMlFrame FrameEnvimania;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaTitleBg;
    [ManialinkControl] public required CMlFrame FrameLabelCar;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaRecordsBg;
    [ManialinkControl] public required CMlFrame FrameRecords;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaYourRecordBg;
    [ManialinkControl] public required CMlFrame FrameYourRecord;
    [ManialinkControl] public required CMlFrame FrameEnvimaniaStatus;
    [ManialinkControl] public required CMlLabel LabelEnvimaniaStatus;

    [Netread] public required Dictionary<SEnvimaniaRecordsFilter, SEnvimaniaRecordsResponse> EnvimaniaRecords { get; init; }
    [Netread] public int EnvimaniaRecordsUpdatedAt { get; init; }
    [Netread] public required string EnvimaniaStatusMessage { get; init; }

    public bool PreviousVisible;
    public int VisibleTime = -1;

    public int PreviousEnvimaniaRecordsUpdatedAt;
    public string PreviousCar = "";
    public string PreviousEnvimaniaStatusMessage = "";

    public required CMlLabel LabelYourRecordNickname;

    CTmMlPlayer GetPlayer()
    {
        if (GUIPlayer is not null)
        {
            return GUIPlayer;
        }

        return InputPlayer;
    }

    static string TimeToTextWithMilli(int time)
    {
        return $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
    }

    bool IsVisible()
    {
        return !IsInGameMenuDisplayed;
    }

    string GetCar()
    {
        var car = Netread<string>.For(GetPlayer());
        return car.Get();
    }

    public void Main()
    {
        FrameEnvimania.Visible = IsVisible();
        PreviousVisible = FrameEnvimania.Visible;

        Wait(() => GetPlayer() is not null);

        PreviousEnvimaniaRecordsUpdatedAt = EnvimaniaRecordsUpdatedAt;
        PreviousCar = GetCar();
        PreviousEnvimaniaStatusMessage = EnvimaniaStatusMessage;

        LabelYourRecordNickname = (FrameYourRecord.GetFirstChild("LabelNickname") as CMlLabel)!;
        (FrameYourRecord.GetFirstChild("LabelRank") as CMlLabel)!.SetText("00");
    }

    public void Loop()
    {
        FrameEnvimania.Visible = IsVisible();

        if (FrameEnvimania.Visible != PreviousVisible)
        {
            if (FrameEnvimania.Visible)
            {
                foreach (var control in FrameEnvimaniaTitleBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 8\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                foreach (var control in FrameEnvimaniaRecordsBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 82.5\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                foreach (var control in FrameEnvimaniaYourRecordBg.Controls)
                {
                    control.Size.X = 0;
                    AnimMgr.Add(control, "<quad size=\"42.5 6\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                }

                VisibleTime = Now;
            }

            PreviousVisible = FrameEnvimania.Visible;
        }

        if (VisibleTime == -1)
        {
            FrameLabelCar.ClipWindowSize.X = 45;
            FrameRecords.ClipWindowSize.X = 45;
            FrameYourRecord.ClipWindowSize.X = 45;
            FrameEnvimaniaStatus.ClipWindowSize.X = 45;
        }
        else
        {
            FrameLabelCar.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 400);
            FrameRecords.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 500);
            FrameYourRecord.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 400);
            FrameEnvimaniaStatus.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - VisibleTime, 0, 45, 500);
        }

        if (EnvimaniaRecordsUpdatedAt != PreviousEnvimaniaRecordsUpdatedAt)
        {
            LabelEnvimaniaStatus.SetText(EnvimaniaStatusMessage);
            UpdateRecords();
            PreviousEnvimaniaRecordsUpdatedAt = EnvimaniaRecordsUpdatedAt;
        }

        if (GetCar() != PreviousCar)
        {
            UpdateRecords();
            PreviousCar = GetCar();
        }

        if (EnvimaniaStatusMessage != PreviousEnvimaniaStatusMessage)
        {
            LabelEnvimaniaStatus.SetText(EnvimaniaStatusMessage);
            PreviousEnvimaniaStatusMessage = EnvimaniaStatusMessage;
        }

        LabelYourRecordNickname.SetText(GetPlayer().User.Name);
    }

    public int GetLaps()
    {
        if (!MapIsLapRace)
        {
            return 1;
        }

        if (NbLaps == -1)
        {
            return Map.TMObjective_NbLaps;
        }

        return NbLaps;
    }

    private SEnvimaniaRecordsFilter GetFilter()
    {
        var gravity = Netread<int>.For(GetPlayer());

        SEnvimaniaRecordsFilter filter = new()
        {
            Car = GetCar(),
            Gravity = gravity.Get(),
            Laps = GetLaps(),
            Type = "Time" // TODO: Get type
        };

        return filter;
    }

    private void SetYouCouldBeHere()
    {
        foreach (var control in FrameRecords.Controls)
        {
            control.Visible = false;
        }

        var firstFrame = (FrameRecords.Controls[0] as CMlFrame)!;
        firstFrame.Visible = true;

        var labelRank = (firstFrame.GetFirstChild("LabelRank") as CMlLabel)!;
        var labelNickname = (firstFrame.GetFirstChild("LabelNickname") as CMlLabel)!;
        var labelTime = (firstFrame.GetFirstChild("LabelTime") as CMlLabel)!;

        AnimMgr.Add(labelRank, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(labelNickname, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(labelTime, "<label opacity=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);

        labelNickname.SetText("$i$888you could be here!");
        labelTime.SetText("-:--.---");

    }

    private void UpdateRecords()
    {
        if (EnvimaniaStatusMessage is not "")
        {
            foreach (var control in FrameRecords.Controls)
            {
                control.Visible = false;
            }

            return;
        }

        var filter = GetFilter();

        if (!EnvimaniaRecords.ContainsKey(filter))
        {
            SetYouCouldBeHere();
            return;
        }

        var recResponse = EnvimaniaRecords[filter];

        if (recResponse.Records.Length == 0)
        {
            SetYouCouldBeHere();
            return;
        }

        SEnvimaniaRecord prevRecord = new();
        var rankOffset = 0;

        for (int i = 0; i < FrameRecords.Controls.Count; i++)
        {
            var frame = (FrameRecords.Controls[i] as CMlFrame)!;

            if (i >= recResponse.Records.Length)
            {
                frame.Visible = false;
                continue;
            }

            var record = recResponse.Records[i];

            var rank = i + 1 - rankOffset;

            if (record.Time == prevRecord.Time && record.Score == prevRecord.Score && record.NbRespawns == prevRecord.NbRespawns && record.Distance == prevRecord.Distance && record.Speed == prevRecord.Speed)
            {
                rankOffset += 1;
            }

            var labelRank = (frame.GetFirstChild("LabelRank") as CMlLabel)!;
            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            var labelTime = (frame.GetFirstChild("LabelTime") as CMlLabel)!;

            labelRank.SetText(TextLib.FormatInteger(rank, 2));
            labelNickname.SetText(record.User.Nickname);
            labelTime.SetText(TimeToTextWithMilli(record.Time));

            var opacity = 0.5f;

            if (record.Verified)
            {
                opacity = 1;
            }

            if (frame.Visible)
            {
                // TODO: Apply only when different
                //frame.RelativeScale = 1.02f;
                //AnimMgr.Add(frame, "<frame scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                frame.Visible = true;

                labelRank.Opacity = 0;
                labelNickname.Opacity = 0;
                labelTime.Opacity = 0;
            }

            AnimMgr.Add(labelRank, $"<label opacity=\"{opacity}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.Add(labelNickname, $"<label opacity=\"{opacity}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            AnimMgr.Add(labelTime, $"<label opacity=\"{opacity}\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);

            prevRecord = record;
        }
    }
}
