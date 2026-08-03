namespace Envimix.Media.Manialinks;

public class Leaderboards : CManiaAppTitleLayer, IContext
{
    public struct SPlayerScore
    {
        public string L;
        public int S;
    }

    public struct SPlayerCompletion
    {
        public string L;
        public float S;
    }

    public struct SPlayerMedals
    {
        public string L;
        public int D;
        public int ST;
        public int SG;
        public int SS;
        public int SB;
        public int A;
        public int G;
        public int S;
        public int B;
    }

    public struct STitleUserInfo
    {
        public string N;
        public string Z;
    }

    public struct SCombinationRecordCount
    {
        public int E;
        public int D;
        public int G;
    }

    [ManialinkControl] public required CMlFrame FrameCompletion;
    [ManialinkControl] public required CMlFrame FrameMostSkillpoints;
    [ManialinkControl] public required CMlFrame FrameMostActivityPoints;
    [ManialinkControl] public required CMlFrame FrameOverallCompletion;
    [ManialinkControl] public required CMlFrame FrameQuit;
    [ManialinkControl] public required CMlFrame FrameCategory;
    [ManialinkControl] public required CMlQuad QuadQuit;
    [ManialinkControl] public required CMlLabel LabelOverallCompletion;
    [ManialinkControl] public required CMlLabel LabelOverallCompletionName;

    [ManialinkControl] public required CMlFrame FrameCompletionPlayers;
    [ManialinkControl] public required CMlFrame FrameMostSkillpointsPlayers;
    [ManialinkControl] public required CMlFrame FrameMostActivityPointsPlayers;

    [ManialinkControl] public required CMlFrame FramePersonalCompletion;
    [ManialinkControl] public required CMlFrame FramePersonalSkillpoints;
    [ManialinkControl] public required CMlFrame FramePersonalActivityPoints;

    [ManialinkControl] public required CMlQuad QuadEnvimixLeaderboards;
    [ManialinkControl] public required CMlQuad QuadDefaultCarLeaderboards;
    [ManialinkControl] public required CMlQuad QuadGlobalLeaderboards;

    [ManialinkControl] public required CMlFrame FrameCars;

    [ManialinkControl] public required CMlFrame FrameTooltip;

    [ManialinkControl] public required CMlQuad QuadDuck;
    [ManialinkControl] public required CMlQuad QuadSTM;
    [ManialinkControl] public required CMlQuad QuadSuperGold;
    [ManialinkControl] public required CMlQuad QuadSuperSilver;
    [ManialinkControl] public required CMlQuad QuadSuperBronze;
    [ManialinkControl] public required CMlQuad QuadNadeo;
    [ManialinkControl] public required CMlQuad QuadGold;
    [ManialinkControl] public required CMlQuad QuadSilver;
    [ManialinkControl] public required CMlQuad QuadBronze;
    [ManialinkControl] public required CMlLabel LabelDuck;
    [ManialinkControl] public required CMlLabel LabelSTM;
    [ManialinkControl] public required CMlLabel LabelSuperGold;
    [ManialinkControl] public required CMlLabel LabelSuperSilver;
    [ManialinkControl] public required CMlLabel LabelSuperBronze;
    [ManialinkControl] public required CMlLabel LabelNadeo;
    [ManialinkControl] public required CMlLabel LabelGold;
    [ManialinkControl] public required CMlLabel LabelSilver;
    [ManialinkControl] public required CMlLabel LabelBronze;

    public int OpenedAt = -1;
    public float EnvimixCompletionPercentage;
    public float DefaultCarCompletionPercentage;
    public float GlobalCompletionPercentage;
    public Dictionary<string, float> EnvimixCompletionPercentages;
    public Dictionary<string, float> DefaultCarCompletionPercentages;
    public Dictionary<string, float> GlobalCompletionPercentages;

    public CMlQuad SelectedLeaderboards;
    public string SelectedCar;

    public CAudioSource AudioClick;

    public int ZoneIndexCompletion;
    public int ZoneIndexSkillpoints;
    public int ZoneIndexActivityPoints;

    public CMlQuad? FocusedQuad;

    public bool ShowCarsRequested;

    public Leaderboards()
    {
        QuadQuit.MouseClick += () =>
        {
            SendCustomEvent("MainMenu", new[] { "" });
            AudioClick.Play();
        };

        QuadQuit.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        PluginCustomEvent += (type, data) =>
        {
            switch (type)
            {
                case "AnimateOpen":
                    EnableMenuNavigationInputs = true;
                    Show();
                    break;
                case "AnimateClose":
                    EnableMenuNavigationInputs = false;
                    Hide();
                    break;
                case "CompletionStats":
                    EnvimixCompletionPercentage = TextLib.ToReal(data[0]);
                    DefaultCarCompletionPercentage = TextLib.ToReal(data[1]);
                    GlobalCompletionPercentage = TextLib.ToReal(data[2]);
                    EnvimixCompletionPercentages.FromJson(data[3]);
                    DefaultCarCompletionPercentages.FromJson(data[4]);
                    GlobalCompletionPercentages.FromJson(data[5]);
                    UpdateCompletionLeaderboard();
                    UpdateCars();
                    break;
                case "SkillpointStats":
                    UpdateSkillpointsLeaderboard();
                    UpdateCars();
                    break;
                case "ActivityPointStats":
                    UpdateActivityPointsLeaderboard();
                    UpdateCars();
                    break;
                case "UserInfos":
                    UpdateAllLeaderboards();
                    break;
            }
        };

        MenuNavigation += (action) =>
        {
            switch (action)
            {
                case CMlScriptEvent.EMenuNavAction.Cancel:
                    SendCustomEvent("MainMenu", new[] { "" });
                    break;
                case CMlScriptEvent.EMenuNavAction.Left:
                    if (FocusedQuad is null)
                    {
                        FocusedQuad = SelectedLeaderboards;
                    }
                    FocusedQuad.StyleSelected = FocusedQuad == SelectedLeaderboards || (SelectedCar != "" && FocusedQuad.DataAttributeGet("CarName") == SelectedCar);
                    if (FocusedQuad == QuadEnvimixLeaderboards)
                    {
                        FocusedQuad = QuadGlobalLeaderboards;
                    }
                    else if (FocusedQuad == QuadDefaultCarLeaderboards)
                    {
                        FocusedQuad = QuadEnvimixLeaderboards;
                    }
                    else if (FocusedQuad == QuadGlobalLeaderboards)
                    {
                        FocusedQuad = QuadDefaultCarLeaderboards;
                    }
                    else if (FocusedQuad == (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameCars.GetFirstChild("QuadSelectCar") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad.ControlId == "QuadSelectCar")
                    {
                        FocusedQuad = (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadQuit)
                    {
                        FocusedQuad = (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    FocusedQuad.StyleSelected = true;
                    break;
                case CMlScriptEvent.EMenuNavAction.Right:
                    if (FocusedQuad is null)
                    {
                        FocusedQuad = SelectedLeaderboards;
                    }
                    FocusedQuad.StyleSelected = FocusedQuad == SelectedLeaderboards || (SelectedCar != "" && FocusedQuad.DataAttributeGet("CarName") == SelectedCar);
                    if (FocusedQuad == QuadEnvimixLeaderboards)
                    {
                        FocusedQuad = QuadDefaultCarLeaderboards;
                    }
                    else if (FocusedQuad == QuadDefaultCarLeaderboards)
                    {
                        FocusedQuad = QuadGlobalLeaderboards;
                    }
                    else if (FocusedQuad == QuadGlobalLeaderboards)
                    {
                        FocusedQuad = (FrameCars.GetFirstChild("QuadSelectCar") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = (FrameCars.GetFirstChild("QuadSelectCar") as CMlQuad);
                    }
                    else if (FocusedQuad.ControlId == "QuadSelectCar")
                    {
                        FocusedQuad = (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadQuit)
                    {
                        FocusedQuad = (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    FocusedQuad.StyleSelected = true;
                    break;
                case CMlScriptEvent.EMenuNavAction.Down:
                    if (FocusedQuad is null)
                    {
                        FocusedQuad = SelectedLeaderboards;
                    }
                    FocusedQuad.StyleSelected = FocusedQuad == SelectedLeaderboards || (SelectedCar != "" && FocusedQuad.DataAttributeGet("CarName") == SelectedCar);
                    if (FocusedQuad == QuadEnvimixLeaderboards)
                    {
                        FocusedQuad = (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadDefaultCarLeaderboards)
                    {
                        FocusedQuad = (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadGlobalLeaderboards)
                    {
                        FocusedQuad = (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadEnvimixLeaderboards;
                    }
                    else if (FocusedQuad == (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadDefaultCarLeaderboards;
                    }
                    else if (FocusedQuad == (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadGlobalLeaderboards;
                    }
                    else if (FocusedQuad.ControlId == "QuadSelectCar")
                    {
                        var currentFound = false;
                        var isLastControl = true;

                        foreach (var control in FocusedQuad.Parent.Parent.Controls)
                        {
                            if (currentFound)
                            {
                                FocusedQuad = ((control as CMlFrame)!.GetFirstChild("QuadSelectCar") as CMlQuad);
                                isLastControl = false;
                                break;
                            }

                            if (FocusedQuad.Parent == control)
                            {
                                currentFound = true;
                            }
                        }

                        if (isLastControl)
                        {
                            FocusedQuad = QuadQuit;
                        }
                    }
                    else if (FocusedQuad == QuadQuit)
                    {
                        FocusedQuad = (FrameCars.GetFirstChild("QuadSelectCar") as CMlQuad);
                    }
                    FocusedQuad.StyleSelected = true;
                    break;
                case CMlScriptEvent.EMenuNavAction.Up:
                    if (FocusedQuad is null)
                    {
                        FocusedQuad = SelectedLeaderboards;
                    }
                    FocusedQuad.StyleSelected = FocusedQuad == SelectedLeaderboards || (SelectedCar != "" && FocusedQuad.DataAttributeGet("CarName") == SelectedCar);
                    if (FocusedQuad == QuadEnvimixLeaderboards)
                    {
                        FocusedQuad = (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadDefaultCarLeaderboards)
                    {
                        FocusedQuad = (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == QuadGlobalLeaderboards)
                    {
                        FocusedQuad = (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad);
                    }
                    else if (FocusedQuad == (FrameCompletion.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadEnvimixLeaderboards;
                    }
                    else if (FocusedQuad == (FrameMostSkillpoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadDefaultCarLeaderboards;
                    }
                    else if (FocusedQuad == (FrameMostActivityPoints.GetFirstChild("QuadZone") as CMlQuad))
                    {
                        FocusedQuad = QuadGlobalLeaderboards;
                    }
                    else if (FocusedQuad.ControlId == "QuadSelectCar")
                    {
                        var currentFound = false;
                        var isFirstControl = true;

                        for (var i = 0; i < FocusedQuad.Parent.Parent.Controls.Count; i++)
                        {
                            var control = FocusedQuad.Parent.Parent.Controls[FocusedQuad.Parent.Parent.Controls.Count - i - 1];

                            if (currentFound)
                            {
                                FocusedQuad = ((control as CMlFrame)!.GetFirstChild("QuadSelectCar") as CMlQuad);
                                isFirstControl = false;
                                break;
                            }

                            if (FocusedQuad.Parent == control)
                            {
                                currentFound = true;
                            }
                        }

                        if (isFirstControl)
                        {
                            FocusedQuad = QuadQuit;
                        }
                    }
                    else if (FocusedQuad == QuadQuit)
                    {
                        var lastControl = (FrameCars.Controls[FrameCars.Controls.Count - 1] as CMlFrame)!;
                        FocusedQuad = (lastControl.GetFirstChild("QuadSelectCar") as CMlQuad);
                    }
                    FocusedQuad.StyleSelected = true;
                    break;
                case CMlScriptEvent.EMenuNavAction.Select:
                    if (FocusedQuad is not null)
                    {
                        if (FocusedQuad.ControlId == "QuadSelectCar")
                        {
                            SelectCar(FocusedQuad);
                        }
                        else if (FocusedQuad.ControlId == "QuadZone")
                        {
                            SelectZone(FocusedQuad);
                        }
                        else if (FocusedQuad == QuadEnvimixLeaderboards)
                        {
                            SelectedLeaderboards = QuadEnvimixLeaderboards;
                            UpdateAllLeaderboards();
                        }
                        else if (FocusedQuad == QuadDefaultCarLeaderboards)
                        {
                            SelectedLeaderboards = QuadDefaultCarLeaderboards;
                            UpdateAllLeaderboards();
                        }
                        else if (FocusedQuad == QuadGlobalLeaderboards)
                        {
                            SelectedLeaderboards = QuadGlobalLeaderboards;
                            UpdateAllLeaderboards();
                        }
                        else if (FocusedQuad == QuadQuit)
                        {
                            SendCustomEvent("MainMenu", new[] { "" });
                        }
                    }
                    break;
            }
        };

        QuadEnvimixLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadEnvimixLeaderboards;
            UpdateAllLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadEnvimixLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadDefaultCarLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadDefaultCarLeaderboards;
            UpdateAllLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadDefaultCarLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadGlobalLeaderboards.MouseClick += () =>
        {
            SelectedLeaderboards = QuadGlobalLeaderboards;
            UpdateAllLeaderboards();
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);
        };

        QuadGlobalLeaderboards.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        MouseClick += (control, controlId) =>
        {
            if (controlId == "QuadZone")
            {
                SelectZone(control);
            }

            if (controlId == "QuadSelectCar")
            {
                SelectCar(control);
            }
        };

        MouseOver += (control, controlId) =>
        {
            if (controlId == "QuadZone")
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
            if (controlId == "QuadSelectCar")
            {
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
            }
            if (controlId == "QuadTopmostMedal")
            {
                AnimMgr.Add(control, "<quad scale=\"1.15\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

                var medalsJson = control.Parent.DataAttributeGet("Medals");
                SPlayerMedals medals = new();
                medals.FromJson(medalsJson);
                UpdateTooltip(medals);
            }
        };

        MouseOut += (control, controlId) =>
        {
            if (controlId == "QuadTopmostMedal")
            {
                AnimMgr.Add(control, "<quad scale=\"1\"/>", 200, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            FrameTooltip.Hide();
        };
    }

    private static string FormatNumberSpace(int number)
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

    private static string GetFullZone(IList<string> zones, int index)
    {
        var zone = zones[0];

        for (var i = 0; i < zones.Count - 1; i++)
        {
            if (i == index)
            {
                break;
            }

            zone = $"{zone}|{zones[i + 1]}";
        }

        return zone;
    }

    private void UpdateCompletionLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos, IList<SPlayerCompletion> completionLeaderboard)
    {
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);

        var completionZone = "World";
        if (zones.Count > 0)
        {
            completionZone = GetFullZone(zones, ZoneIndexCompletion);
        }

        var completionOffsetX = 0f;
        var index = 0;
        var rank = 1;
        var prevCompletionScore = -1f;
        var rankOffset = 0;
        foreach (var control in FrameCompletionPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= completionLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerCompletion = completionLeaderboard[index];

            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                while (!TextLib.StartsWith(completionZone, leaderboardsUserInfos[playerCompletion.L].Z))
                {
                    index += 1;
                    if (index >= completionLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerCompletion = completionLeaderboard[index];
                }

                if (index >= completionLeaderboard.Count)
                {
                    continue;
                }
            }

            if (prevCompletionScore == playerCompletion.S)
            {
                rankOffset += 1;
            }
            else
            {
                prevCompletionScore = playerCompletion.S;
                rankOffset = 0;
            }

            frame.GetFirstChild("FrameMedals").Hide();
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank - rankOffset, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(1, 1, 1);
            labelRecord.SetText($"{TextLib.FormatReal(playerCompletion.S * 100, 2, false, false)}%");

            if (rank == 1)
            {
                completionOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

            var nickname = playerCompletion.L;
            if (leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                nickname = leaderboardsUserInfos[playerCompletion.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = completionOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerCompletion.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal completion stats
        var completionRank = 0;
        var completionScore = -1f;
        foreach (var player in completionLeaderboard)
        {
            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(completionZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                completionScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalCompletion.GetFirstChild("LabelRank") as CMlLabel)!;
        if (completionScore == -1f)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalCompletion.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(1, 1, 1);
        if (completionScore == -1f)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText($"{TextLib.FormatReal(completionScore * 100, 2, false, false)}%");
        }
        labelPersonalRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalCompletion.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = completionOffsetX + 5;

        FramePersonalCompletion.GetFirstChild("FrameMedals").Hide();
    }

    private void UpdateCompletionLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos, IList<SPlayerMedals> completionLeaderboard)
    {
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);

        var completionZone = "World";
        if (zones.Count > 0)
        {
            completionZone = GetFullZone(zones, ZoneIndexCompletion);
        }

        var completionOffsetX = 0f;
        var index = 0;
        var rank = 1;
        var prevCompletionScore = -1;
        var rankOffset = 0;
        foreach (var control in FrameCompletionPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= completionLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerCompletion = completionLeaderboard[index];

            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                while (!TextLib.StartsWith(completionZone, leaderboardsUserInfos[playerCompletion.L].Z))
                {
                    index += 1;
                    if (index >= completionLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerCompletion = completionLeaderboard[index];
                }

                if (index >= completionLeaderboard.Count)
                {
                    continue;
                }
            }

            var totalMedals = playerCompletion.D + playerCompletion.ST + playerCompletion.SG + playerCompletion.SS + playerCompletion.SB + playerCompletion.A + playerCompletion.G + playerCompletion.S + playerCompletion.B;

            if (prevCompletionScore == totalMedals)
            {
                rankOffset += 1;
            }
            else
            {
                prevCompletionScore = totalMedals;
                rankOffset = 0;
            }

            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank - rankOffset, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(1, 1, 1);
            labelRecord.SetText(totalMedals.ToString());

            if (rank == 1)
            {
                completionOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

            var nickname = playerCompletion.L;
            if (leaderboardsUserInfos.ContainsKey(playerCompletion.L))
            {
                nickname = leaderboardsUserInfos[playerCompletion.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = completionOffsetX + 11;

            var frameMedals = (frame.GetFirstChild("FrameMedals") as CMlFrame)!;
            frameMedals.DataAttributeSet("Medals", playerCompletion.ToJson());
            frameMedals.RelativePosition_V3.X = completionOffsetX + 6;
            frameMedals.Show();

            IList<int> medalArray = new[] { playerCompletion.D, playerCompletion.ST, playerCompletion.SG, playerCompletion.SS, playerCompletion.SB, playerCompletion.A, playerCompletion.G, playerCompletion.S, playerCompletion.B };
            var currentMedalIndex = 0;

            for (var i = 0; i < frameMedals.Controls.Count; i++)
            {
                var medalControl = frameMedals.Controls[frameMedals.Controls.Count - i - 1];
                var medalQuad = (medalControl as CMlQuad)!;

                while (medalArray[currentMedalIndex] == 0 && currentMedalIndex < medalArray.Count - 1)
                {
                    currentMedalIndex += 1;
                }

                medalQuad.Visible = medalArray[currentMedalIndex] > 0;
                medalQuad.ImageUrl = "";
                medalQuad.Substyle = "";

                switch (currentMedalIndex)
                {
                    case 0:
                        medalQuad.ImageUrl = "file://Media/Images/Medals/duck.png";
                        break;
                    case 1:
                        medalQuad.ImageUrl = "file://Media/Images/Medals/stm.png";
                        break;
                    case 2:
                        medalQuad.ImageUrl = "file://Media/Images/Medals/supergold.png";
                        break;
                    case 3:
                        medalQuad.ImageUrl = "file://Media/Images/Medals/supersilver.png";
                        break;
                    case 4:
                        medalQuad.ImageUrl = "file://Media/Images/Medals/superbronze.png";
                        break;
                    case 5:
                        medalQuad.Substyle = "MedalNadeo";
                        break;
                    case 6:
                        medalQuad.Substyle = "MedalGold";
                        break;
                    case 7:
                        medalQuad.Substyle = "MedalSilver";
                        break;
                    case 8:
                        medalQuad.Substyle = "MedalBronze";
                        break;
                }

                currentMedalIndex += 1;
            }

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerCompletion.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal completion stats
        var completionRank = 0;
        var completionScore = -1;
        var completionJson = "";
        foreach (var player in completionLeaderboard)
        {
            if (completionZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(completionZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                completionScore = player.D + player.ST + player.SG + player.SS + player.SB + player.A + player.G + player.S + player.B;
                completionJson = player.ToJson();
                break;
            }
        }

        var labelPersonalRank = (FramePersonalCompletion.GetFirstChild("LabelRank") as CMlLabel)!;
        if (completionScore == -1f)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalCompletion.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(1, 1, 1);
        if (completionScore == -1f)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText(completionScore.ToString());
        }
        labelPersonalRecord.RelativePosition_V3.X = completionOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalCompletion.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = completionOffsetX + 11;

        var framePersonalMedals = (FramePersonalCompletion.GetFirstChild("FrameMedals") as CMlFrame)!;
        framePersonalMedals.DataAttributeSet("Medals", completionJson);
        framePersonalMedals.RelativePosition_V3.X = completionOffsetX + 6;
        framePersonalMedals.Show();
    }

    private void UpdateCompletionLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos)
    {
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            if (SelectedCar == "")
            {
                var envimixCompletion = Local<IList<SPlayerCompletion>>.For(Page);
                UpdateCompletionLeaderboard(leaderboardsUserInfos, envimixCompletion.Get());
            }
            else
            {
                var envimixCombinationCompletion = Local<Dictionary<string, IList<SPlayerCompletion>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (envimixCombinationCompletion.Get().ContainsKey(carKey))
                {
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, envimixCombinationCompletion.Get()[carKey]);
                }
                else
                {
                    var envimixCompletion = Local<IList<SPlayerCompletion>>.For(Page);
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, envimixCompletion.Get());
                }
            }
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            if (SelectedCar == "")
            {
                var defaultCarCompletion = Local<IList<SPlayerMedals>>.For(Page);
                UpdateCompletionLeaderboard(leaderboardsUserInfos, defaultCarCompletion.Get());
            }
            else
            {
                var defaultCarCombinationCompletion = Local<Dictionary<string, IList<SPlayerMedals>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (defaultCarCombinationCompletion.Get().ContainsKey(carKey))
                {
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, defaultCarCombinationCompletion.Get()[carKey]);
                }
                else
                {
                    var defaultCarCompletion = Local<IList<SPlayerMedals>>.For(Page);
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, defaultCarCompletion.Get());
                }
            }
        }
        else
        {
            if (SelectedCar == "")
            {
                var globalCompletion = Local<IList<SPlayerCompletion>>.For(Page);
                UpdateCompletionLeaderboard(leaderboardsUserInfos, globalCompletion.Get());
            }
            else
            {
                var globalCombinationCompletion = Local<Dictionary<string, IList<SPlayerCompletion>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (globalCombinationCompletion.Get().ContainsKey(carKey))
                {
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, globalCombinationCompletion.Get()[carKey]);
                }
                else
                {
                    var globalCompletion = Local<IList<SPlayerCompletion>>.For(Page);
                    UpdateCompletionLeaderboard(leaderboardsUserInfos, globalCompletion.Get());
                }
            }
        }
    }

    private void UpdateCompletionLeaderboard()
    {
        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);
        UpdateCompletionLeaderboard(leaderboardsUserInfos.Get());
    }

    private void UpdateSkillpointsLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos)
    {
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);

        IList<SPlayerScore> skillpointsLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            if (SelectedCar == "")
            {
                var envimixMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                skillpointsLeaderboard = envimixMostSkillpoints.Get();
            }
            else
            {
                var envimixCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (envimixCombinationMostSkillpoints.Get().ContainsKey(carKey))
                {
                    skillpointsLeaderboard = envimixCombinationMostSkillpoints.Get()[carKey];
                }
                else
                {
                    var envimixMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                    skillpointsLeaderboard = envimixMostSkillpoints.Get();
                }
            }
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            if (SelectedCar == "")
            {
                var defaultCarMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                skillpointsLeaderboard = defaultCarMostSkillpoints.Get();
            }
            else
            {
                var defaultCarCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (defaultCarCombinationMostSkillpoints.Get().ContainsKey(carKey))
                {
                    skillpointsLeaderboard = defaultCarCombinationMostSkillpoints.Get()[carKey];
                }
                else
                {
                    var defaultCarMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                    skillpointsLeaderboard = defaultCarMostSkillpoints.Get();
                }
            }
        }
        else
        {
            if (SelectedCar == "")
            {
                var globalMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                skillpointsLeaderboard = globalMostSkillpoints.Get();
            }
            else
            {
                var globalCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (globalCombinationMostSkillpoints.Get().ContainsKey(carKey))
                {
                    skillpointsLeaderboard = globalCombinationMostSkillpoints.Get()[carKey];
                }
                else
                {
                    var globalMostSkillpoints = Local<IList<SPlayerScore>>.For(Page);
                    skillpointsLeaderboard = globalMostSkillpoints.Get();
                }
            }
        }

        var skillpointsZone = "World";
        if (zones.Count > 0)
        {
            skillpointsZone = GetFullZone(zones, ZoneIndexSkillpoints);
        }

        var skillpointsOffsetX = 0f;
        var index = 0;
        var rank = 1;
        var prevSkillpointsScore = -1;
        var rankOffset = 0;
        foreach (var control in FrameMostSkillpointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= skillpointsLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = skillpointsLeaderboard[index];

            if (skillpointsZone != "World" && leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                while (!TextLib.StartsWith(skillpointsZone, leaderboardsUserInfos[playerScore.L].Z))
                {
                    index += 1;
                    if (index >= skillpointsLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerScore = skillpointsLeaderboard[index];
                }

                if (index >= skillpointsLeaderboard.Count)
                {
                    continue;
                }
            }

            if (prevSkillpointsScore == playerScore.S)
            {
                rankOffset += 1;
            }
            else
            {
                prevSkillpointsScore = playerScore.S;
                rankOffset = 0;
            }

            frame.GetFirstChild("FrameMedals").Hide();
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank - rankOffset, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 0);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (rank == 1)
            {
                skillpointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal skillpoints stats
        var completionRank = 0;
        var pointsScore = -1;
        foreach (var player in skillpointsLeaderboard)
        {
            if (skillpointsZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(skillpointsZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalSkillpoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalSkillpoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 0);
        if (pointsScore == -1)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText(FormatNumberSpace(pointsScore));
        }
        labelPersonalRecord.RelativePosition_V3.X = skillpointsOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalSkillpoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = skillpointsOffsetX + 5;

        FramePersonalSkillpoints.GetFirstChild("FrameMedals").Hide();
    }

    private void UpdateSkillpointsLeaderboard()
    {
        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);
        UpdateSkillpointsLeaderboard(leaderboardsUserInfos.Get());
    }

    private void UpdateActivityPointsLeaderboard(Dictionary<string, STitleUserInfo> leaderboardsUserInfos)
    {
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);

        IList<SPlayerScore> activityPointsLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            if (SelectedCar == "")
            {
                var envimixMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                activityPointsLeaderboard = envimixMostActivityPoints.Get();
            }
            else
            {
                var envimixCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (envimixCombinationMostActivityPoints.Get().ContainsKey(carKey))
                {
                    activityPointsLeaderboard = envimixCombinationMostActivityPoints.Get()[carKey];
                }
                else
                {
                    var envimixMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                    activityPointsLeaderboard = envimixMostActivityPoints.Get();
                }
            }
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            if (SelectedCar == "")
            {
                var defaultCarMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                activityPointsLeaderboard = defaultCarMostActivityPoints.Get();
            }
            else
            {
                var defaultCarCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (defaultCarCombinationMostActivityPoints.Get().ContainsKey(carKey))
                {
                    activityPointsLeaderboard = defaultCarCombinationMostActivityPoints.Get()[carKey];
                }
                else
                {
                    var defaultCarMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                    activityPointsLeaderboard = defaultCarMostActivityPoints.Get();
                }
            }
        }
        else
        {
            if (SelectedCar == "")
            {
                var globalMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                activityPointsLeaderboard = globalMostActivityPoints.Get();
            }
            else
            {
                var globalCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
                var carKey = $"{SelectedCar}_0";
                if (globalCombinationMostActivityPoints.Get().ContainsKey(carKey))
                {
                    activityPointsLeaderboard = globalCombinationMostActivityPoints.Get()[carKey];
                }
                else
                {
                    var globalMostActivityPoints = Local<IList<SPlayerScore>>.For(Page);
                    activityPointsLeaderboard = globalMostActivityPoints.Get();
                }
            }
        }

        var activityPointsZone = "World";
        if (zones.Count > 0)
        {
            activityPointsZone = GetFullZone(zones, ZoneIndexActivityPoints);
        }

        var activityPointsOffsetX = 0f;
        var index = 0;
        var rank = 1;
        var prevActivityPointsScore = -1;
        var rankOffset = 0;
        foreach (var control in FrameMostActivityPointsPlayers.Controls)
        {
            if (control is not CMlFrame frame)
            {
                continue;
            }

            if (index >= activityPointsLeaderboard.Count)
            {
                frame.Hide();
                continue;
            }

            var playerScore = activityPointsLeaderboard[index];

            if (activityPointsZone != "World" && leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                while (!TextLib.StartsWith(activityPointsZone, leaderboardsUserInfos[playerScore.L].Z))
                {
                    index += 1;
                    if (index >= activityPointsLeaderboard.Count)
                    {
                        frame.Hide();
                        break;
                    }
                    playerScore = activityPointsLeaderboard[index];
                }

                if (index >= activityPointsLeaderboard.Count)
                {
                    continue;
                }
            }

            if (prevActivityPointsScore == playerScore.S)
            {
                rankOffset += 1;
            }
            else
            {
                prevActivityPointsScore = playerScore.S;
                rankOffset = 0;
            }

            frame.GetFirstChild("FrameMedals").Hide();
            (frame.GetFirstChild("LabelRank") as CMlLabel)!.SetText(TextLib.FormatInteger(rank - rankOffset, 2));

            var labelRecord = (frame.GetFirstChild("LabelRecord") as CMlLabel)!;
            labelRecord.TextColor = new Vec3(0, 1, 1);
            labelRecord.SetText(FormatNumberSpace(playerScore.S));

            if (rank == 1)
            {
                activityPointsOffsetX = labelRecord.ComputeWidth(labelRecord.Value);
            }

            labelRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

            var nickname = playerScore.L;
            if (leaderboardsUserInfos.ContainsKey(playerScore.L))
            {
                nickname = leaderboardsUserInfos[playerScore.L].N;
            }

            var labelNickname = (frame.GetFirstChild("LabelNickname") as CMlLabel)!;
            labelNickname.SetText(nickname);
            labelNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;

            frame.GetFirstChild("QuadHighlight")!.Visible = LocalUser.Login == playerScore.L;

            frame.Show();

            index += 1;
            rank += 1;
        }

        // Update personal activity points stats
        var completionRank = 0;
        var pointsScore = -1;
        foreach (var player in activityPointsLeaderboard)
        {
            if (activityPointsZone != "World" && leaderboardsUserInfos.ContainsKey(player.L) && !TextLib.StartsWith(activityPointsZone, leaderboardsUserInfos[player.L].Z))
            {
                continue;
            }

            completionRank += 1;
            if (player.L == LocalUser.Login)
            {
                pointsScore = player.S;
                break;
            }
        }

        var labelPersonalRank = (FramePersonalActivityPoints.GetFirstChild("LabelRank") as CMlLabel)!;
        if (pointsScore == -1)
        {
            labelPersonalRank.SetText("--");
        }
        else
        {
            labelPersonalRank.SetText(TextLib.FormatInteger(completionRank, 2));
        }

        var labelPersonalRecord = (FramePersonalActivityPoints.GetFirstChild("LabelRecord") as CMlLabel)!;
        labelPersonalRecord.TextColor = new Vec3(0, 1, 1);
        if (pointsScore == -1)
        {
            labelPersonalRecord.SetText("-");
        }
        else
        {
            labelPersonalRecord.SetText(FormatNumberSpace(pointsScore));
        }
        labelPersonalRecord.RelativePosition_V3.X = activityPointsOffsetX + 2.5;

        var labelPersonalNickname = (FramePersonalActivityPoints.GetFirstChild("LabelNickname") as CMlLabel)!;
        labelPersonalNickname.SetText(LocalUser.Name);
        labelPersonalNickname.RelativePosition_V3.X = activityPointsOffsetX + 5;

        FramePersonalActivityPoints.GetFirstChild("FrameMedals").Hide();
    }

    private void UpdateActivityPointsLeaderboard()
    {
        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);
        UpdateActivityPointsLeaderboard(leaderboardsUserInfos.Get());
    }

    private void ShowCars()
    {
        if (CombinationRecordCountValues.Count == 0)
        {
            ShowCarsRequested = true;
        }

        for (var i = 0; i < FrameCars.Controls.Count; i++)
        {
            var frame = (FrameCars.Controls[i] as CMlFrame)!;

            AnimMgr.Flush(frame);

            if (i >= CombinationRecordCountValues.Count)
            {
                frame.Hide();
                continue;
            }
            var recordCount = CombinationRecordCountValues[i];

            if ((SelectedLeaderboards == QuadEnvimixLeaderboards && recordCount.E == 0)
             || (SelectedLeaderboards == QuadDefaultCarLeaderboards && recordCount.D == 0)
             || (SelectedLeaderboards == QuadGlobalLeaderboards && recordCount.G == 0))
            {
                AnimMgr.Add(frame, $"<frame pos=\"0 {frame.RelativePosition_V3.Y}\"/>", Now + i * 100, 400, CAnimManager.EAnimManagerEasing.QuadOut);
                continue;
            }

            AnimMgr.Add(frame, $"<frame hidden=\"0\" pos=\"0 {frame.RelativePosition_V3.Y}\"/>", Now + i * 100, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    public IList<string> CombinationRecordCountKeys;
    public IList<SCombinationRecordCount> CombinationRecordCountValues;

    private void UpdateCars()
    {
        var combinationRecordCount = Local<Dictionary<string, SCombinationRecordCount>>.For(Page);

        CombinationRecordCountKeys.Clear();
        CombinationRecordCountValues.Clear();
        foreach (var (k, v) in combinationRecordCount.Get())
        {
            CombinationRecordCountKeys.Add(k);
            CombinationRecordCountValues.Add(v);
        }

        // Sort by values[i].E
        for (int i = 0; i < CombinationRecordCountValues.Count; i++)
        {
            for (int j = i + 1; j < CombinationRecordCountValues.Count; j++)
            {
                if ((SelectedLeaderboards == QuadEnvimixLeaderboards && CombinationRecordCountValues[j].E > CombinationRecordCountValues[i].E)
                 || (SelectedLeaderboards == QuadDefaultCarLeaderboards && CombinationRecordCountValues[j].D > CombinationRecordCountValues[i].D)
                 || (SelectedLeaderboards == QuadGlobalLeaderboards && CombinationRecordCountValues[j].G > CombinationRecordCountValues[i].G))
                {
                    // swap values
                    var tempVal = CombinationRecordCountValues[i];
                    CombinationRecordCountValues[i] = CombinationRecordCountValues[j];
                    CombinationRecordCountValues[j] = tempVal;

                    // swap corresponding keys
                    var tempKey = CombinationRecordCountKeys[i];
                    CombinationRecordCountKeys[i] = CombinationRecordCountKeys[j];
                    CombinationRecordCountKeys[j] = tempKey;
                }
            }
        }

        Dictionary<string, IList<SPlayerScore>> skillpointsLeaderboard;
        Dictionary<string, IList<SPlayerScore>> activityPointsLeaderboard;
        if (SelectedLeaderboards == QuadEnvimixLeaderboards)
        {
            var envimixCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            skillpointsLeaderboard = envimixCombinationMostSkillpoints.Get();
            var envimixCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            activityPointsLeaderboard = envimixCombinationMostActivityPoints.Get();
        }
        else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
        {
            var defaultCarCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            skillpointsLeaderboard = defaultCarCombinationMostSkillpoints.Get();
            var defaultCarCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            activityPointsLeaderboard = defaultCarCombinationMostActivityPoints.Get();
        }
        else
        {
            var globalCombinationMostSkillpoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            skillpointsLeaderboard = globalCombinationMostSkillpoints.Get();
            var globalCombinationMostActivityPoints = Local<Dictionary<string, IList<SPlayerScore>>>.For(Page);
            activityPointsLeaderboard = globalCombinationMostActivityPoints.Get();
        }

        var baseRecordCount = -1;

        for (var i = 0; i < FrameCars.Controls.Count; i++)
        {
            var frame = (FrameCars.Controls[i] as CMlFrame)!;
            if (i >= CombinationRecordCountValues.Count)
            {
                frame.Hide();
                continue;
            }
            var carKey = CombinationRecordCountKeys[i];
            var recordCount = CombinationRecordCountValues[i];

            if ((SelectedLeaderboards == QuadEnvimixLeaderboards && recordCount.E == 0)
             || (SelectedLeaderboards == QuadDefaultCarLeaderboards && recordCount.D == 0)
             || (SelectedLeaderboards == QuadGlobalLeaderboards && recordCount.G == 0))
            {
                AnimMgr.Flush(frame);
                frame.RelativePosition_V3.X = 0;
                frame.Hide();
                continue;
            }

            var carSplit = TextLib.Split("_", carKey);
            var carName = carSplit[0];

            var labelCarName = (frame.GetFirstChild("LabelCarName") as CMlLabel)!;
            labelCarName.SetText(carName);

            var quadCar = (frame.GetFirstChild("QuadCar") as CMlQuad)!;
            quadCar.ChangeImageUrl($"file://Media/Images/Cars/{carName}.png");

            var labelSkillpoints = (frame.GetFirstChild("LabelSkillpoints") as CMlLabel)!;
            var playerHasSkillpoints = false;

            if (skillpointsLeaderboard.ContainsKey(carKey))
            {
                foreach (var player in skillpointsLeaderboard[carKey])
                {
                    if (player.L == LocalUser.Login)
                    {
                        labelSkillpoints.SetText(FormatNumberSpace(player.S));
                        playerHasSkillpoints = true;
                        break;
                    }
                }
            }
            
            if (!playerHasSkillpoints)
            {
                labelSkillpoints.SetText("0");
            }

            var labelActivityPoints = (frame.GetFirstChild("LabelActivityPoints") as CMlLabel)!;
            var playerHasActivityPoints = false;

            if (activityPointsLeaderboard.ContainsKey(carKey))
            {
                foreach (var player in activityPointsLeaderboard[carKey])
                {
                    if (player.L == LocalUser.Login)
                    {
                        labelActivityPoints.SetText(FormatNumberSpace(player.S));
                        playerHasActivityPoints = true;
                        break;
                    }
                }
            }

            if (!playerHasActivityPoints)
            {
                labelActivityPoints.SetText("0");
            }

            var quadSelectCar = (frame.GetFirstChild("QuadSelectCar") as CMlQuad)!;
            quadSelectCar.StyleSelected = SelectedCar == carName;
            quadSelectCar.DataAttributeSet("CarName", carName);

            if (baseRecordCount == -1)
            {
                if (SelectedLeaderboards == QuadEnvimixLeaderboards)
                {
                    baseRecordCount = recordCount.E;
                }
                else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
                {
                    baseRecordCount = recordCount.D;
                }
                else
                {
                    baseRecordCount = recordCount.G;
                }
            }

            var gaugePopularity = (frame.GetFirstChild("GaugePopularity") as CMlGauge)!;
            if (SelectedLeaderboards == QuadEnvimixLeaderboards)
            {
                gaugePopularity.Ratio = recordCount.E * 0.98f / baseRecordCount;
            }
            else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
            {
                gaugePopularity.Ratio = recordCount.D * 0.98f / baseRecordCount;
            }
            else
            {
                gaugePopularity.Ratio = recordCount.G * 0.98f / baseRecordCount;
            }

            frame.Show();
        }

        if (ShowCarsRequested)
        {
            ShowCars();

            if (CombinationRecordCountValues.Count > 0)
            {
                ShowCarsRequested = false;
            }
        }
    }

    private void UpdateAllLeaderboardsWithoutCars()
    {
        QuadEnvimixLeaderboards.StyleSelected = SelectedLeaderboards == QuadEnvimixLeaderboards;
        QuadDefaultCarLeaderboards.StyleSelected = SelectedLeaderboards == QuadDefaultCarLeaderboards;
        QuadGlobalLeaderboards.StyleSelected = SelectedLeaderboards == QuadGlobalLeaderboards;

        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);

        UpdateCompletionLeaderboard(leaderboardsUserInfos.Get());
        UpdateSkillpointsLeaderboard(leaderboardsUserInfos.Get());
        UpdateActivityPointsLeaderboard(leaderboardsUserInfos.Get());
    }

    private void UpdateAllLeaderboards()
    {
        UpdateAllLeaderboardsWithoutCars();
        UpdateCars();
    }

    private void Show()
    {
        AnimMgr.Add(FrameCategory, "<frame hidden=\"0\" pos=\"0 0\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);

        AnimMgr.Add(FrameCompletion, "<frame hidden=\"0\" pos=\"-105 65\"/>", Now + 400, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostSkillpoints, "<frame hidden=\"0\" pos=\"-35 65\"/>", Now + 200, 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostActivityPoints, "<frame hidden=\"0\" pos=\"35 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);

        AnimMgr.Add(FrameOverallCompletion, "<frame hidden=\"0\" pos=\"105 65\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameQuit, "<frame hidden=\"0\" pos=\"130 -60\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
        
        ShowCars();

        OpenedAt = Now;
    }

    private void Hide()
    {
        FrameQuit.Visible = false;

        AnimMgr.Add(FrameCategory, "<frame hidden=\"1\" pos=\"0 30\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameCompletion, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostSkillpoints, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameMostActivityPoints, "<frame hidden=\"1\" pos=\"-210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameOverallCompletion, "<frame hidden=\"1\" pos=\"210 65\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        AnimMgr.Add(FrameQuit, "<frame hidden=\"0\" pos=\"130 -90\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);

        for (var i = 0; i < FrameCars.Controls.Count; i++)
        {
            var frame = (FrameCars.Controls[i] as CMlFrame)!;
            AnimMgr.Add(frame, $"<frame hidden=\"1\" pos=\"100 {frame.RelativePosition_V3.Y}\"/>", 400, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }

    private void SelectZone(CMlControl control)
    {
        IList<string> zones = TextLib.Split("|", LocalUser.ZonePath);
        var index = 0;

        switch (control.Parent.Parent.ControlId)
        {
            case "FrameCompletion":
                if (zones.Count == 0)
                {
                    ZoneIndexCompletion = 0;
                }
                else
                {
                    ZoneIndexCompletion = (ZoneIndexCompletion + 1) % zones.Count;
                }
                index = ZoneIndexCompletion;
                break;
            case "FrameMostSkillpoints":
                if (zones.Count == 0)
                {
                    ZoneIndexSkillpoints = 0;
                }
                else
                {
                    ZoneIndexSkillpoints = (ZoneIndexSkillpoints + 1) % zones.Count;
                }
                index = ZoneIndexSkillpoints;
                break;
            case "FrameMostActivityPoints":
                if (zones.Count == 0)
                {
                    ZoneIndexActivityPoints = 0;
                }
                else
                {
                    ZoneIndexActivityPoints = (ZoneIndexActivityPoints + 1) % zones.Count;
                }
                index = ZoneIndexActivityPoints;
                break;
        }

        var zone = "World";
        if (zones.Count > 0)
        {
            zone = zones[index];
        }

        var labelZone = (control.Parent.GetFirstChild("LabelZone") as CMlLabel)!;
        labelZone.SetText("|Zone|" + zone);

        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 1, 1);

        var leaderboardsUserInfos = Local<Dictionary<string, STitleUserInfo>>.For(Page);

        switch (control.Parent.Parent.ControlId)
        {
            case "FrameCompletion":
                UpdateCompletionLeaderboard(leaderboardsUserInfos.Get());
                break;
            case "FrameMostSkillpoints":
                UpdateSkillpointsLeaderboard(leaderboardsUserInfos.Get());
                break;
            case "FrameMostActivityPoints":
                UpdateActivityPointsLeaderboard(leaderboardsUserInfos.Get());
                break;
        }
    }

    private void SelectCar(CMlControl control)
    {
        var carName = control.DataAttributeGet("CarName");
        if (SelectedCar == carName)
        {
            SelectedCar = "";
        }
        else
        {
            SelectedCar = carName;
        }
        Audio.PlaySoundEvent(CAudioManager.ELibSound.Valid, 0, 1);

        UpdateAllLeaderboards();
    }

    public void Main()
    {
        FrameCategory.RelativePosition_V3.Y = 30;
        FrameCategory.Visible = false;

        FrameCompletion.RelativePosition_V3.X = -210;
        FrameCompletion.Visible = false;

        FrameMostSkillpoints.RelativePosition_V3.X = -210;
        FrameMostSkillpoints.Visible = false;

        FrameMostActivityPoints.RelativePosition_V3.X = -210;
        FrameMostActivityPoints.Visible = false;

        FrameOverallCompletion.RelativePosition_V3.X = 210;
        FrameOverallCompletion.Visible = false;

        FrameQuit.RelativePosition_V3.Y = -90;
        FrameQuit.Visible = false;

        foreach (var control in FrameCars.Controls)
        {
            var frame = (control as CMlFrame)!;
            frame.RelativePosition_V3.X = 100;
            frame.Visible = false;
        }

        SelectedLeaderboards = QuadEnvimixLeaderboards;

        AudioClick = Audio.CreateSound("file://Media/Sounds/Click.wav");

        UpdateAllLeaderboards();
    }

    public void Loop()
    {
        if (OpenedAt != -1)
        {
            float percentage;
            if (SelectedLeaderboards == QuadEnvimixLeaderboards)
            {
                if (SelectedCar == "")
                {
                    percentage = EnvimixCompletionPercentage;
                }
                else
                {
                    var carKey = $"{SelectedCar}_0";
                    if (EnvimixCompletionPercentages.ContainsKey(carKey))
                    {
                        percentage = EnvimixCompletionPercentages[carKey];
                    }
                    else
                    {
                        percentage = 0;
                    }
                }
            }
            else if (SelectedLeaderboards == QuadDefaultCarLeaderboards)
            {
                if (SelectedCar == "")
                {
                    percentage = DefaultCarCompletionPercentage;
                }
                else
                {
                    var carKey = $"{SelectedCar}_0";
                    if (DefaultCarCompletionPercentages.ContainsKey(carKey))
                    {
                        percentage = DefaultCarCompletionPercentages[carKey];
                    }
                    else
                    {
                        percentage = 0;
                    }
                }
            }
            else
            {
                if (SelectedCar == "")
                {
                    percentage = GlobalCompletionPercentage;
                }
                else
                {
                    var carKey = $"{SelectedCar}_0";
                    if (GlobalCompletionPercentages.ContainsKey(carKey))
                    {
                        percentage = GlobalCompletionPercentages[carKey];
                    }
                    else
                    {
                        percentage = 0;
                    }
                }
            }

            var animatedOverallCompletion = AnimLib.EaseOutQuad(Now - OpenedAt, 0, percentage * 100, 1000);
            LabelOverallCompletion.Value = $"{TextLib.FormatReal(animatedOverallCompletion, 2, false, false)}%";

            if (SelectedCar == "")
            {
                LabelOverallCompletionName.Value = "Overall completion";
            }
            else
            {
                LabelOverallCompletionName.Value = $"{SelectedCar} completion";
            }
        }

        if (FrameTooltip.Visible)
        {
            FrameTooltip.RelativePosition_V3 = new Vec2(MouseX, MouseY);
        }
    }

    private void UpdateTooltip(SPlayerMedals medals)
    {
        LabelDuck.Value = medals.D.ToString();

        var offset = 10f;

        offset += LabelDuck.ComputeWidth(LabelDuck.Value);
        QuadSTM.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelSTM.RelativePosition_V3.X = offset;
        LabelSTM.Value = medals.ST.ToString();

        offset += LabelSTM.ComputeWidth(LabelSTM.Value) + 4f;
        QuadSuperGold.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelSuperGold.RelativePosition_V3.X = offset;
        LabelSuperGold.Value = medals.SG.ToString();

        offset += LabelSuperGold.ComputeWidth(LabelSuperGold.Value) + 4f;
        QuadSuperSilver.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelSuperSilver.RelativePosition_V3.X = offset;
        LabelSuperSilver.Value = medals.SS.ToString();

        offset += LabelSuperSilver.ComputeWidth(LabelSuperSilver.Value) + 4f;
        QuadSuperBronze.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelSuperBronze.RelativePosition_V3.X = offset;
        LabelSuperBronze.Value = medals.SB.ToString();

        offset += LabelSuperBronze.ComputeWidth(LabelSuperBronze.Value) + 4f;
        QuadNadeo.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelNadeo.RelativePosition_V3.X = offset;
        LabelNadeo.Value = medals.A.ToString();

        offset += LabelNadeo.ComputeWidth(LabelNadeo.Value) + 4f;
        QuadGold.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelGold.RelativePosition_V3.X = offset;
        LabelGold.Value = medals.G.ToString();

        offset += LabelGold.ComputeWidth(LabelGold.Value) + 4f;
        QuadSilver.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelSilver.RelativePosition_V3.X = offset;
        LabelSilver.Value = medals.S.ToString();

        offset += LabelSilver.ComputeWidth(LabelSilver.Value) + 4f;
        QuadBronze.RelativePosition_V3.X = offset;
        offset += 3f;
        LabelBronze.RelativePosition_V3.X = offset;
        LabelBronze.Value = medals.B.ToString();

        offset += LabelBronze.ComputeWidth(LabelBronze.Value);

        var quadTooltip = (FrameTooltip.GetFirstChild("QuadTooltip") as CMlQuad)!;
        quadTooltip.Size.X = offset + 2f;

        FrameTooltip.Show();
    }
}
