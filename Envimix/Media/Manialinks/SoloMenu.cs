using System.Collections.Immutable;

namespace Envimix.Media.Manialinks;

public class SoloMenu : CManiaAppTitleLayer, IContext
{
    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }

    public struct SValidationInfo
    {
        public string Login;
        public string Nickname;
        public string DrivenAt;
    }

    public struct SEnvimaniaRecordsFilter
    {
        public string Car;
        public int Gravity;
        public int Laps;
        public string Type;
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
        public bool Projected;
        public string GhostUrl;
        public string DrivenAt;
    }

    public struct SEnvimaniaRecordsResponse
    {
        public SEnvimaniaRecordsFilter Filter;
        public string Zone;
        public ImmutableArray<SEnvimaniaRecord> Records;
        public ImmutableArray<SEnvimaniaRecord> Validation;
        public ImmutableArray<int> Skillpoints;
        public string TitlePackReleaseTimestamp;
    }

    [ManialinkControl] public required CMlQuad QuadBack;
    [ManialinkControl] public required CMlFrame FrameCars;
    [ManialinkControl] public required CMlFrame FrameValidations;
    [ManialinkControl] public required CMlFrame FrameLeaderboards;
    [ManialinkControl] public required CMlQuad QuadTM2Cars;
    [ManialinkControl] public required CMlQuad QuadTMUFCars;
    [ManialinkControl] public required CMlFrame FrameCampaignOverview;
    [ManialinkControl] public required CMlFrame FrameMapOverview;
    [ManialinkControl] public required CMlFrame FrameCampaign;
    [ManialinkControl] public required CMlLabel LabelSelectedMapName;
    [ManialinkControl] public required CMlQuad QuadPlay;
    [ManialinkControl] public required CMlQuad QuadExplore;
    [ManialinkControl] public required CMlLabel LabelSkillpoints;
    [ManialinkControl] public required CMlLabel LabelActivityPoints;
    [ManialinkControl] public required CMlLabel LabelCompletionPercentage;
    [ManialinkControl] public required CMlFrame FrameDifficultyRatings;
    [ManialinkControl] public required CMlFrame FrameQualityRatings;
    [ManialinkControl] public required CMlFrame FrameStars;
    [ManialinkControl] public required CMlFrame FrameRatingsCars;

    public ImmutableArray<string> TM2Cars;
    public ImmutableArray<string> TMUFCars;
    public ImmutableArray<string> FunnyCars;

    public CCampaign? Campaign;
    public int MapGroupNum = -1;
    public int MapInfoNum = -1;
    public int MapSelectedAt = -1;
    public int StatsAt = -1;
    public int StatsLocalAt = -1;
    public int StatsReceivedAt = -1;

    public int CurrentSkillpoints;
    public int CurrentActivityPoints;
    public float CurrentCompletionPercentage;
    public int ExpectedSkillpoints;
    public int ExpectedActivityPoints;
    public float ExpectedCompletionPercentage;

    public string ScoreContextPrefix = "Test";

    public bool IsTMUF;

    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SRating>> TitleRatings { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SStar>> TitleStars { get; set; }
    [Local(LocalFor.LocalUser)] public Dictionary<string, Dictionary<string, SValidationInfo>> TitleValidations { get; set; }

    public Dictionary<string, Dictionary<string, SEnvimaniaRecordsResponse>> Leaderboards;
    public Dictionary<string, int> LeaderboardRequestTimestamps;

    public SoloMenu()
    {
        QuadBack.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
        };

        QuadTM2Cars.MouseClick += () =>
        {
            SwitchCars(false);
        };

        QuadTMUFCars.MouseClick += () =>
        {
            SwitchCars(true);
        };

        QuadPlay.MouseClick += () =>
        {
            if (MapGroupNum != -1 && MapInfoNum != -1)
            {
                PlaySelectedMap();
            }
        };

        QuadExplore.MouseClick += () =>
        {
            if (MapGroupNum != -1 && MapInfoNum != -1)
            {
                ExploreSelectedMap();
            }
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    ShowCampaignOverviewFrame();
                    ShowMapOverviewFrame();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    HideCampaignOverviewFrame();
                    HideMapOverviewFrame();
                    break;
                case "SetPoints":
                    ExpectedSkillpoints = TextLib.ToInteger(data[0]);
                    ExpectedActivityPoints = TextLib.ToInteger(data[1]);
                    StatsReceivedAt = Now;

                    if (EnableMenuNavigationInputs && (ExpectedSkillpoints > CurrentSkillpoints || ExpectedActivityPoints > CurrentActivityPoints))
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            Audio.PlaySoundEvent(CAudioManager.ELibSound.ScoreIncrease, SoundVariant: 0, VolumedB: 0.8f, Delay: i * 100);
                        }
                    }

                    // weird to call it at SetPoints, but it allows updating the solo menu stats when nothing is clicked
                    if (DataFileMgr.Campaigns.Count > 0 && MapGroupNum != -1 && MapInfoNum != -1)
                    {
                        UpdateStats(DataFileMgr.Campaigns[0].MapGroups[MapGroupNum].MapInfos[MapInfoNum]);
                    }
                    break;
                case "LeaderboardData":
                    var mapUid = data[0];
                    var car = data[1];
                    var recordsJson = data[2];
                    SEnvimaniaRecordsResponse response = new();
                    response.FromJson(recordsJson);
                    ProcessLeaderboardData(mapUid, car, response);
                    break;
            }
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapClick(control);
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (controlId == "QuadMapButton")
            {
                MapSelect(control);
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("MainMenu", new[] { "" });
                    break;
            }
        };
    }

    public void Main()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        FrameMapOverview.RelativePosition_V3.Y = 10;

        TM2Cars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "" };
        TMUFCars = new() { "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        FunnyCars = new() { "HighlandsCar", "DumpsterCar", "ToasterCar", "FunnyCar" };

        Page.GetClassChildren("LOADING", Page.MainFrame, true);

        SwitchCars(false);
        SetupCampaign();
    }

    public void Loop()
    {
        foreach (var control in Page.GetClassChildren_Result)
        {
            if (control.Visible)
            {
                control.RelativeRotation += Period * 0.2f;
            }
        }

        // Update stats every minute
        if (StatsAt == -1 || (Now - StatsAt) > 60000)
        {
            Log("Updating stats...");

            var finishedEnvimixCount = 0;
            var totalEnvimixCount = 0;

            foreach (var campaign in DataFileMgr.Campaigns)
            {
                foreach (var mapGroup in campaign.MapGroups)
                {
                    foreach (var mapInfo in mapGroup.MapInfos)
                    {
                        // a bit of a hack but clientside on turbo maps it works - if current car context is environment car, skip
                        if (mapInfo.CollectionName == "Canyon" && campaign.ScoreContext == ScoreContextPrefix + "CanyonCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Stadium" && campaign.ScoreContext == ScoreContextPrefix + "StadiumCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Valley" && campaign.ScoreContext == ScoreContextPrefix + "ValleyCar")
                        {
                            continue;
                        }
                        if (mapInfo.CollectionName == "Lagoon" && campaign.ScoreContext == ScoreContextPrefix + "LagoonCar")
                        {
                            continue;
                        }

                        if (ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, campaign.ScoreContext) != -1)
                        {
                            finishedEnvimixCount += 1;
                        }

                        totalEnvimixCount += 1;
                    }
                }
            }

            LoadLeaderboards(false);

            SendCustomEvent("Stats", new[] { "" });

            ExpectedCompletionPercentage = finishedEnvimixCount * 1f / totalEnvimixCount * 100;
            StatsAt = Now;
            StatsLocalAt = Now;
        }

        if (StatsLocalAt != -1)
        {
            var duration = 1000;
            var time = Now - StatsLocalAt;

            var percentageDiff = ExpectedCompletionPercentage - CurrentCompletionPercentage;
            var percentage = AnimLib.EaseOutQuad(time, CurrentCompletionPercentage, percentageDiff, duration);

            var percentageText = TextLib.FormatReal(percentage, 4, false, true);

            LabelCompletionPercentage.Value = $"Envimix $ff0Turbo$g completion: $o{percentageText}%";

            if (time >= duration)
            {
                StatsLocalAt = -1;
                CurrentCompletionPercentage = ExpectedCompletionPercentage;
            }
        }

        if (StatsReceivedAt != -1)
        {
            var duration = 1000;
            var time = Now - StatsReceivedAt;

            var skillpointsDiff = ExpectedSkillpoints - CurrentSkillpoints;
            var activityPointsDiff = ExpectedActivityPoints - CurrentActivityPoints;

            var skillpoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentSkillpoints), MathLib.ToReal(skillpointsDiff), duration));
            var activityPoints = MathLib.FloorInteger(AnimLib.EaseOutQuad(time, MathLib.ToReal(CurrentActivityPoints), MathLib.ToReal(activityPointsDiff), duration));

            LabelSkillpoints.Value = FormatNumberSpace(skillpoints);
            LabelActivityPoints.Value = FormatNumberSpace(activityPoints);

            var noPointDiff = skillpointsDiff == 0 && activityPointsDiff == 0;

            if (time >= duration || noPointDiff)
            {
                StatsReceivedAt = -1;
                CurrentSkillpoints = ExpectedSkillpoints;
                CurrentActivityPoints = ExpectedActivityPoints;
            }
        }
    }

    static string TimeToTextWithMilli(int time)
    {
        var formatted = $"{TextLib.TimeToText(time, true)}{MathLib.Abs(time % 10)}";
        if (TextLib.Length(TextLib.Split(".", formatted)[1]) > 3)
            return TextLib.SubString(formatted, 0, TextLib.Length(formatted) - 1);
        return formatted;
    }

    private string FormatNumberSpace(int number)
    {
        var txt = TextLib.ToText(number);
        if (number < 0)
        {
            txt = TextLib.SubText(txt, 1, TextLib.Length(txt) - 1);
        }
        var result = "";
        var len = TextLib.Length(txt);
        var count = 0;

        for (var i = 0; i < len; i++)
        {
            result = $"{TextLib.SubText(txt, len - 1 - i, 1)}{result}";
            count += 1;

            if (count == 3 && i < len - 1)
            {
                result = $" {result}";
                count = 0;
            }
        }

        if (number < 0)
        {
            result = $"-{result}";
        }

        return result;
    }

    private void ShowCampaignOverviewFrame()
    {
        FrameCampaignOverview.RelativePosition_V3.Y = -50;
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 80\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideCampaignOverviewFrame()
    {
        AnimMgr.Add(FrameCampaignOverview, "<frame pos=\"0 -50\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void ShowMapOverviewFrame()
    {
        FrameMapOverview.RelativePosition_V3.Y = 10;
        AnimMgr.Add(FrameMapOverview, "<frame pos=\"-155 -41\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private void HideMapOverviewFrame()
    {
        AnimMgr.Add(FrameMapOverview, "<frame pos=\"-155 10\"/>", 600, CAnimManager.EAnimManagerEasing.QuadOut);
    }

    private static int GetLaps(CMapInfo mapInfo)
    {
        if (!mapInfo.TMObjective_IsLapRace)
        {
            return 1;
        }

        return mapInfo.TMObjective_NbLaps;
    }

    private void ResetPBs()
    {
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }
            var labelPersonalBest = (frameLeaderboard.GetFirstChild("LabelPersonalBest") as CMlLabel)!;
            labelPersonalBest.SetText("--:--.---");
        }
    }

    private void ResetValidators()
    {
        foreach (var control in FrameValidations.Controls)
        {
            control.Hide();
        }
    }

    private void ResetRatings()
    {
        foreach (var control in FrameDifficultyRatings.Controls)
        {
            if (control is not CMlGauge gaugeDifficulty)
            {
                continue;
            }
            gaugeDifficulty.Ratio = 0;
            gaugeDifficulty.Color = new Vec3(1, 0, 0);
        }
        foreach (var control in FrameQualityRatings.Controls)
        {
            if (control is not CMlGauge gaugeQuality)
            {
                continue;
            }
            gaugeQuality.Ratio = 0;
            gaugeQuality.Color = new Vec3(1, 0, 0);
        }
    }

    private void ResetStars()
    {
        foreach (var control in FrameStars.Controls)
        {
            control.Hide();
        }
    }

    private void UpdatePBs(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        var carIndex = 0;
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            var labelPersonalBest = (frameLeaderboard.GetFirstChild("LabelPersonalBest") as CMlLabel)!;

            var time = ScoreMgr.Map_GetRecord(null, mapInfo.MapUid, $"{ScoreContextPrefix}{carName}");

            if (time == -1)
            {
                labelPersonalBest.SetText("--:--.---");
            }
            else
            {
                labelPersonalBest.SetText(TimeToTextWithMilli(time));
            }

            carIndex += 1;
        }
    }

    private void UpdateValidators(CMapInfo mapInfo, ImmutableArray<string> cars)
    {
        var carIndex = 0;
        foreach (var control in FrameValidations.Controls)
        {
            if (control is not CMlLabel labelValidation)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            labelValidation.Show();

            if (carName == "")
            {
                labelValidation.SetText("$888...send us suggestions!");
                carIndex += 1;
                continue;
            }

            // hacky but it works for TMT
            if ((mapInfo.CollectionName == "Canyon" && carName == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && carName == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && carName == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && carName == "LagoonCar"))
            {
                labelValidation.SetText($"validated by {mapInfo.AuthorNickName}");
                carIndex += 1;
                continue;
            }

            var validationFilterKey = $"{carName}_0_{GetLaps(mapInfo)}";

            if (!TitleValidations.ContainsKey(mapInfo.MapUid) || !TitleValidations[mapInfo.MapUid].ContainsKey(validationFilterKey))
            {
                labelValidation.SetText("$888you can validate this!");
                carIndex += 1;
                continue;
            }

            var validation = TitleValidations[mapInfo.MapUid][validationFilterKey];
            labelValidation.SetText($"validated by {validation.Nickname}");
            carIndex += 1;
        }
    }

    private void UpdateRatings(CMapInfo mapInfo)
    {
        ImmutableArray<string> allCars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };

        var carIndex = 0;
        foreach (var car in allCars)
        {
            var difficultyGauge = (FrameDifficultyRatings.Controls[carIndex] as CMlGauge)!;
            var qualityGauge = (FrameQualityRatings.Controls[carIndex] as CMlGauge)!;
            var carLabel = (FrameRatingsCars.Controls[carIndex] as CMlLabel)!;

            var filterKey = $"{car}_0_Time";

            if (TitleRatings.ContainsKey(mapInfo.MapUid) && TitleRatings[mapInfo.MapUid].ContainsKey(filterKey))
            {
                var rating = TitleRatings[mapInfo.MapUid][filterKey];
                difficultyGauge.Ratio = rating.Difficulty;
                qualityGauge.Ratio = rating.Quality;
            }
            else
            {
                difficultyGauge.Ratio = 0;
                qualityGauge.Ratio = 0;
            }

            // hacky but it works for TMT
            if ((mapInfo.CollectionName == "Canyon" && car == "CanyonCar")
                || (mapInfo.CollectionName == "Stadium" && car == "StadiumCar")
                || (mapInfo.CollectionName == "Valley" && car == "ValleyCar")
                || (mapInfo.CollectionName == "Lagoon" && car == "LagoonCar"))
            {
                difficultyGauge.Color = new Vec3(1, 1, 1);
                qualityGauge.Color = new Vec3(1, 1, 1);
                carLabel.TextColor = new Vec3(1, 1, 1);
                carIndex += 1;
                continue;
            }

            var validationFilterKey = $"{car}_0_{GetLaps(mapInfo)}";

            if (!TitleValidations.ContainsKey(mapInfo.MapUid) || !TitleValidations[mapInfo.MapUid].ContainsKey(validationFilterKey))
            {
                difficultyGauge.Color = new Vec3(1, 0, 0);
                qualityGauge.Color = new Vec3(1, 0, 0);
                carLabel.TextColor = new Vec3(1, 0, 0);
            }
            else
            {
                difficultyGauge.Color = new Vec3(1, 1, 1);
                qualityGauge.Color = new Vec3(1, 1, 1);
                carLabel.TextColor = new Vec3(1, 1, 1);
            }

            carIndex += 1;
        }
    }

    private void UpdateStars(CMapInfo mapInfo)
    {
        ImmutableArray<string> allCars = new() { "CanyonCar", "StadiumCar", "ValleyCar", "LagoonCar", "TrafficCar", "DesertCar", "SnowCar", "RallyCar", "IslandCar", "BayCar", "CoastCar" };
        
        var carIndex = 0;
        
        foreach (var car in allCars)
        {
            var controlStar = FrameStars.Controls[carIndex];

            var filterKey = $"{car}_0_Time";

            if (TitleStars.ContainsKey(mapInfo.MapUid) && TitleStars[mapInfo.MapUid].ContainsKey(filterKey))
            {
                var star = TitleStars[mapInfo.MapUid][filterKey];
                controlStar.Show();
            }
            else
            {
                controlStar.Hide();
            }

            carIndex += 1;
        }
    }

    private void UpdateStats(CMapInfo selectedMapInfo)
    {
        if (IsTMUF)
        {
            UpdatePBs(selectedMapInfo, TMUFCars);
            UpdateValidators(selectedMapInfo, TMUFCars);
        }
        else
        {
            UpdatePBs(selectedMapInfo, TM2Cars);
            UpdateValidators(selectedMapInfo, TM2Cars);
        }

        UpdateRatings(selectedMapInfo);
        UpdateStars(selectedMapInfo);
    }

    private void UpdateLeaderboards()
    {
        if (DataFileMgr.Campaigns.Count == 0 || MapGroupNum == -1 || MapInfoNum == -1)
        {
            return;
        }

        Campaign = DataFileMgr.Campaigns[0];
        var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];

        ImmutableArray<string> cars = new();
        if (IsTMUF)
        {
            cars = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
        }

        var carIndex = 0;
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            // if the car is not supposed to be there, the carName is empty string, so this is safe
            var carName = cars[carIndex];

            var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
            var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;
            var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;

            labelConfirm.Hide();

            carIndex += 1;

            if (!Leaderboards.ContainsKey(selectedMapInfo.MapUid) || !Leaderboards[selectedMapInfo.MapUid].ContainsKey(carName))
            {
                // check for any error
                continue;
            }

            var lb = Leaderboards[selectedMapInfo.MapUid][carName];

            quadLoadingLeaderboard.Hide();

            if (lb.Records.Length == 0)
            {
                var labelYouCouldBeHere = (frameRecords.Controls[0] as CMlLabel)!;
                labelYouCouldBeHere.SetText("01 -:--.---  $i$888you could be here!");
                labelYouCouldBeHere.Show();

                for (var i = 1; i < frameRecords.Controls.Count; i++)
                {
                    frameRecords.Controls[i].Hide();
                }
                continue;
            }

            var rankIndex = 0;
            var prevTime = -1;
            var rankOffset = 0;

            foreach (var controlRec in frameRecords.Controls)
            {
                if (lb.Records.Length <= rankIndex)
                {
                    controlRec.Hide();
                    continue;
                }

                var record = lb.Records[rankIndex];

                if (prevTime == record.Time)
                {
                    rankOffset += 1;
                }
                else
                {
                    prevTime = record.Time;
                }

                var labelRec = (controlRec as CMlLabel)!;
                labelRec.SetText($"{TextLib.FormatInteger(rankIndex + 1 - rankOffset, 2)} {TimeToTextWithMilli(record.Time)}  {record.User.Nickname}");
                labelRec.Show();

                rankIndex += 1;
            }
        }
    }

    private void LoadLeaderboards(bool showLoader)
    {
        if (DataFileMgr.Campaigns.Count == 0 || MapGroupNum == -1 || MapInfoNum == -1)
        {
            return;
        }

        Campaign = DataFileMgr.Campaigns[0];
        var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];

        var mapUid = selectedMapInfo.MapUid;
        var laps = GetLaps(selectedMapInfo);

        ImmutableArray<string> cars = new();
        ImmutableArray<string> carsToRequest = new();

        if (IsTMUF)
        {
            cars = TMUFCars;
            carsToRequest = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
            carsToRequest = TM2Cars;
        }

        ImmutableArray<string> carsToNotRequest = new();

        foreach (var car in carsToRequest)
        {
            var timestampKey = $"{mapUid}_{car}_{laps}";

            if (LeaderboardRequestTimestamps.ContainsKey(timestampKey) && Now - LeaderboardRequestTimestamps[timestampKey] < 50000)
            {
                carsToNotRequest.Add(car);
            }
            else
            {
                LeaderboardRequestTimestamps[timestampKey] = Now;
            }
        }

        foreach (var car in carsToNotRequest)
        {
            carsToRequest.Remove(car);
        }

        if (carsToRequest.Length == 0)
        {
            UpdateLeaderboards();
            return;
        }

        if (showLoader)
        {
            var carIndex = 0;
            foreach (var control in FrameLeaderboards.Controls)
            {
                if (control is not CMlFrame frameLeaderboard)
                {
                    continue;
                }

                var carName = cars[carIndex];
                if (!carsToRequest.Contains(carName))
                {
                    continue;
                }

                var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
                var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;
                var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

                quadLoadingLeaderboard.Show();
                labelConfirm.Hide();

                foreach (var controlRec in frameRecords.Controls)
                {
                    controlRec.Hide();
                }

                carIndex += 1;
            }
        }

        SendCustomEvent("LoadLeaderboards", new[] { mapUid, laps.ToString(), carsToRequest.ToJson() });
    }

    private void ProcessLeaderboardData(string mapUid, string car, SEnvimaniaRecordsResponse response)
    {
        if (!Leaderboards.ContainsKey(mapUid))
        {
            Leaderboards[mapUid] = new();
        }

        Leaderboards[mapUid][car] = response;
        UpdateLeaderboards();
    }

    private void SwitchCars(bool isTMUF)
    {
        ImmutableArray<string> cars;
        if (isTMUF)
        {
            cars = TMUFCars;
        }
        else
        {
            cars = TM2Cars;
        }

        QuadTMUFCars.StyleSelected = isTMUF;
        QuadTM2Cars.StyleSelected = !isTMUF;

        IsTMUF = isTMUF;

        for (int i = 0; i < FrameCars.Controls.Count; i++)
        {
            var controlCar = FrameCars.Controls[i];

            if (controlCar is not CMlLabel labelCar)
            {
                continue;
            }

            if (i >= cars.Length)
            {
                labelCar.Hide();
                continue;
            }

            var carName = cars[i];
            var missingCar = carName == "";
            if (missingCar)
            {
                var randomIndex = MathLib.Rand(0, FunnyCars.Length - 1);
                carName = FunnyCars[randomIndex] + "?";
            }

            labelCar.SetText(carName);

            var labelValidation = (FrameValidations.Controls[i] as CMlLabel)!;
            var frameLeaderboard = (FrameLeaderboards.Controls[i] as CMlFrame)!;

            if (missingCar)
            {
                labelValidation.SetText("$888...send us suggestions!");
                labelValidation.Show();
                frameLeaderboard.Hide();
            }
            else
            {
                labelValidation.Hide();
                frameLeaderboard.Show();
            }

            labelCar.Show();
        }

        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        Campaign = DataFileMgr.Campaigns[0];

        if (MapGroupNum != -1 && MapInfoNum != -1)
        {
            UpdateStats(Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum]);
        }

        if (MapSelectedAt != -1)
        {
            LoadLeaderboards(true);
        }
    }

    private void SetupCampaign()
    {
        if (DataFileMgr.Campaigns.Count == 0)
        {
            return;
        }

        Campaign = DataFileMgr.Campaigns[0];

        var selectedNum = -1;

        for (int i = 0; i < FrameCampaign.Controls.Count; i++)
        {
            var controlDifficulty = FrameCampaign.Controls[i];

            if (controlDifficulty is not CMlFrame frameDifficulty)
            {
                continue;
            }

            CMapGroup? mapGroup = null;
            if (i < Campaign.MapGroups.Count)
            {
                mapGroup = Campaign.MapGroups[i];
            }

            var mapCounter = 0;
            foreach (var controlGroup in frameDifficulty.Controls)
            {
                if (controlGroup is not CMlFrame frameGroup)
                {
                    continue;
                }

                foreach (var controlMap in frameGroup.Controls)
                {
                    if (controlMap is not CMlFrame frameMap)
                    {
                        continue;
                    }

                    var quadMapThumbnail = (frameMap.GetFirstChild("QuadMapThumbnail") as CMlQuad)!;
                    var quadMapButton = (frameMap.GetFirstChild("QuadMapButton") as CMlQuad)!;
                    var quadMapName = (frameMap.GetFirstChild("QuadMapName") as CMlQuad)!;
                    var labelMapName = (frameMap.GetFirstChild("LabelMapName") as CMlLabel)!;
                    var labelSkillpoints = (frameMap.GetFirstChild("LabelSkillpoints") as CMlLabel)!;

                    if (mapGroup is null || mapCounter >= mapGroup.MapInfos.Count)
                    {
                        quadMapThumbnail.Hide();
                        quadMapButton.Hide();
                        quadMapName.Hide();
                        labelMapName.Hide();
                        labelSkillpoints.Hide();
                        continue;
                    }

                    var mapInfo = mapGroup.MapInfos[mapCounter];

                    quadMapThumbnail.ChangeImageUrl($"file://Thumbnails/MapUid/{mapInfo.MapUid}");
                    quadMapThumbnail.Show();

                    labelMapName.SetText(TextLib.FormatInteger(mapCounter + 1, 3));
                    labelMapName.Show();

                    quadMapButton.DataAttributeSet("MapGroupNum", i.ToString());
                    quadMapButton.DataAttributeSet("MapInfoNum", mapCounter.ToString());
                    quadMapButton.Show();

                    var hovered = MapGroupNum == i && MapInfoNum == mapCounter;
                    quadMapButton.StyleSelected = MapSelectedAt != -1 && hovered;

                    if (hovered)
                    {
                        selectedNum = mapCounter;
                    }

                    quadMapName.Show();
                    labelSkillpoints.Hide();

                    mapCounter += 1;
                }
            }
        }

        if (MapGroupNum != -1 && MapInfoNum != -1)
        {
            var selectedMapInfo = Campaign.MapGroups[MapGroupNum].MapInfos[MapInfoNum];
            LabelSelectedMapName.SetText(TextLib.FormatInteger(selectedNum + 1, 3));
            QuadPlay.Visible = true;

            UpdateStats(selectedMapInfo);
        }
        else
        {
            LabelSelectedMapName.SetText("...");
            QuadPlay.Visible = false;

            ResetPBs();
            ResetValidators();
            ResetRatings();
            ResetStars();
        }
    }

    private void UnloadLeaderboards()
    {
        foreach (var control in FrameLeaderboards.Controls)
        {
            if (control is not CMlFrame frameLeaderboard)
            {
                continue;
            }

            var quadLoadingLeaderboard = (frameLeaderboard.GetFirstChild("QuadLoadingLeaderboard") as CMlQuad)!;
            var labelConfirm = (frameLeaderboard.GetFirstChild("LabelConfirm") as CMlLabel)!;
            var frameRecords = (frameLeaderboard.GetFirstChild("FrameRecords") as CMlFrame)!;

            quadLoadingLeaderboard.Hide();
            labelConfirm.Show();

            foreach (var controlRec in frameRecords.Controls)
            {
                controlRec.Hide();
            }
        }
    }

    private void PlaySelectedMap()
    {
        SendCustomEvent("PlayMap", new[] { MapGroupNum.ToString(), MapInfoNum.ToString() });
    }

    private void ExploreSelectedMap()
    {
        SendCustomEvent("ExploreMap", new[] { MapGroupNum.ToString(), MapInfoNum.ToString() });
    }

    private void MapClick(CMlControl control)
    {
        var mapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        var mapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        if (MapSelectedAt != -1 && MapGroupNum == mapGroupNum && MapInfoNum == mapInfoNum)
        {
            if ((Now - MapSelectedAt) < 500)
            {
                PlaySelectedMap();
            }
            else
            {
                MapSelectedAt = -1;
                SetupCampaign();
                UnloadLeaderboards();
            }
            return;
        }

        MapGroupNum = mapGroupNum;
        MapInfoNum = mapInfoNum;
        MapSelectedAt = Now;

        SetupCampaign();
        LoadLeaderboards(true);
    }

    private void MapSelect(CMlControl control)
    {
        if (MapSelectedAt != -1)
        {
            return;
        }

        MapGroupNum = TextLib.ToInteger(control.DataAttributeGet("MapGroupNum"));
        MapInfoNum = TextLib.ToInteger(control.DataAttributeGet("MapInfoNum"));

        SetupCampaign();
    }
}
