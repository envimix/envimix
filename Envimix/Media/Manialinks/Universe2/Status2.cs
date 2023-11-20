namespace Envimix.Media.Manialinks.Universe2;

public class Status2 : CTmMlScriptIngame, IContext
{
    public int Start;
    public bool PrevVisible;
    public bool PrevIsInMenu;
    public bool PrevTeamSelectionMode;
    public IList<int> PrevClanScores;
    public IList<int> PrevClanScoresForAnim;
    public IList<int> PrevClanScoresTime;

    [ManialinkControl] public required CMlFrame FrameStatus;
    [ManialinkControl] public required CMlQuad QuadRedPoints;
    [ManialinkControl] public required CMlQuad QuadBluePoints;
    [ManialinkControl] public required CMlQuad QuadJoinRed;
    [ManialinkControl] public required CMlQuad QuadJoinBlue;
    [ManialinkControl] public required CMlLabel LabelRedPoints;
    [ManialinkControl] public required CMlLabel LabelBluePoints;
    [ManialinkControl] public required CMlQuad QuadTeam;
    [ManialinkControl] public required CMlLabel LabelTeam;
    [ManialinkControl] public required CMlLabel LabelRedPlayerCount;
    [ManialinkControl] public required CMlLabel LabelBluePlayerCount;

    [Netread] public bool TeamSelectionMode { get; }

    public Status2()
    {
        QuadJoinRed.MouseClick += () =>
        {
            if (TeamSelectionMode)
            {
                Log("Joining Team Red...");
                SendCustomEvent("JoinTeam", new[] { "1" });
                LabelRedPoints.RelativeScale = 1.2f;
                AnimMgr.Add(LabelRedPoints, "<label scale=\"1\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
        };
        QuadJoinRed.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
        };

        QuadJoinBlue.MouseClick += () =>
        {
            if (TeamSelectionMode)
            {
                Log("Joining Team Blue...");
                SendCustomEvent("JoinTeam", new[] { "2" });
                LabelBluePoints.RelativeScale = 1.2f;
                AnimMgr.Add(LabelBluePoints, "<label scale=\"1\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 1, 1);
            }
        };
        QuadJoinBlue.MouseOver += () =>
        {
            Audio.PlaySoundEvent(CAudioManager.ELibSound.Focus, 2, 1);
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
        return UseClans;//!IsInGameMenuDisplayed;
    }

    static string ToNicerNumber(int num)
    {
        var numText = num.ToString();
        var newText = "";

        var numTextLength = TextLib.Length(numText);
        var numLengthReal = numTextLength / 3f;
        var numLength = MathLib.FloorInteger(numLengthReal);

        if (numLengthReal <= 1)
        {
            return numText;
        }

        for (var i = 0; i < numLength + 1; i++)
        {
            var length = MathLib.Min(3, numTextLength - i * 3);

            var numPart = TextLib.SubText(numText, numTextLength - 3 - i * 3, length);

            if (i == 0)
            {
                newText = numPart;
                continue;
            }

            newText = $"{numPart} {newText}";
        }

        return newText;
    }

    public void Main()
    {
        Start = Now;
        PrevVisible = IsVisible();
        PrevIsInMenu = IsInGameMenuDisplayed;
        PrevClanScores = new[] { ClanScores[0], ClanScores[1], ClanScores[2] };
        PrevClanScoresForAnim = new[] { ClanScores[0], ClanScores[1], ClanScores[2] };
        PrevClanScoresTime = new[] { 0, 0, 0 };

        ChangeStatusPosition();

        QuadRedPoints.ModulateColor = Teams[0].ColorPrimary;
        QuadBluePoints.ModulateColor = Teams[1].ColorPrimary;
        QuadJoinRed.ModulateColor = Teams[0].ColorPrimary;
        QuadJoinBlue.ModulateColor = Teams[1].ColorPrimary;

        FrameStatus.Visible = IsVisible();

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (IsInGameMenuDisplayed != PrevIsInMenu)
        {
            ChangeStatusPosition();

            PrevIsInMenu = IsInGameMenuDisplayed;
        }

        if (TeamSelectionMode)
        {
            if (IsSpectatorClient)
            {
                LabelRedPoints.SetText(ToNicerNumber(ClanScores[0]));
                LabelBluePoints.SetText(ToNicerNumber(ClanScores[1]));
                QuadJoinRed.Visible = false;
                QuadJoinBlue.Visible = false;
            }
            else
            {
                Dictionary<int, int> teamPlayerCounts = new()
                {
                    { 1, 0 },
                    { 2, 0 }
                };

                foreach (var score in Scores)
                {
                    if (teamPlayerCounts.ContainsKey(score.TeamNum))
                    {
                        teamPlayerCounts[score.TeamNum] += 1;
                    }
                }

                foreach (var (t1, count1) in teamPlayerCounts)
                {
                    foreach (var (t2, count2) in teamPlayerCounts)
                    {
                        if (t1 == t2)
                        {
                            continue;
                        }

                        if (count1 < count2)
                        {
                            if (t1 == 1)
                            {
                                LabelRedPoints.SetText("Join");
                                LabelBluePoints.SetText("🔒");
                                QuadJoinRed.Visible = true;
                                QuadJoinBlue.Visible = false;
                            }
                            else if (t1 == 2)
                            {
                                LabelRedPoints.SetText("🔒");
                                LabelBluePoints.SetText("Join");
                                QuadJoinRed.Visible = false;
                                QuadJoinBlue.Visible = true;
                            }
                        }
                        else if (count1 == count2)
                        {
                            LabelRedPoints.SetText("🔒");
                            LabelBluePoints.SetText("🔒");
                            QuadJoinRed.Visible = false;
                            QuadJoinBlue.Visible = false;
                        }
                    }
                }

                LabelRedPlayerCount.SetText(teamPlayerCounts[1].ToString());
                LabelBluePlayerCount.SetText(teamPlayerCounts[2].ToString());
            }
        }

        if (TeamSelectionMode != PrevTeamSelectionMode)
        {
            QuadJoinRed.Visible = TeamSelectionMode;
            QuadJoinBlue.Visible = TeamSelectionMode;
            LabelRedPlayerCount.Visible = TeamSelectionMode;
            LabelBluePlayerCount.Visible = TeamSelectionMode;

            if (TeamSelectionMode)
            {

            }
            else
            {
                LabelRedPoints.SetText(ToNicerNumber(ClanScores[0]));
                LabelBluePoints.SetText(ToNicerNumber(ClanScores[1]));
            }

            PrevTeamSelectionMode = TeamSelectionMode;
        }

        if (GetPlayer().Score is null)
        {
            QuadTeam.Colorize = new Vec3(0.2, 0.2, 0.2);
        }
        else
        {
            QuadTeam.Colorize = Teams[GetPlayer().Score.TeamNum - 1].ColorPrimary;
            LabelTeam.SetText(Teams[GetPlayer().Score.TeamNum - 1].Name);
        }

        for (var i = 0; i < ClanScores.Count; i++)
        {
            if (ClanScores[i] != PrevClanScores[i])
            {
                PrevClanScoresTime[i] = Now;
                PrevClanScoresForAnim[i] = PrevClanScores[i];
                PrevClanScores[i] = ClanScores[i];
            }
        }

        var animatedScore1 = 0f;
        var animatedScore2 = 0f;
        var isAnimating = false;

        for (int i = 0; i < PrevClanScoresTime.Count; i++)
        {
            var time = PrevClanScoresTime[i];

            if (Now - time < 1200)
            {
                var score = ClanScores[i];
                var prevScore = PrevClanScoresForAnim[i];

                var animatedScore = AnimLib.EaseOutQuad(Now - time, prevScore * 1f, (score - prevScore) * 1f, 1000);

                if (i == 1)
                {
                    animatedScore1 = animatedScore;
                    LabelRedPoints.SetText(ToNicerNumber(MathLib.NearestInteger(animatedScore)));
                }
                else if (i == 2)
                {
                    animatedScore2 = animatedScore;
                    LabelBluePoints.SetText(ToNicerNumber(MathLib.NearestInteger(animatedScore)));
                }

                isAnimating = true;
            }
        }

        if (isAnimating)
        {
            var redRatio = .5f;
            var blueRatio = .5f;

            if (animatedScore1 + animatedScore2 > 0)
            {
                redRatio = animatedScore1 / (animatedScore1 + animatedScore2);
                blueRatio = animatedScore2 / (animatedScore1 + animatedScore2);
            }

            redRatio = MathLib.Clamp(redRatio, .2f, .8f);
            blueRatio = MathLib.Clamp(blueRatio, .2f, .8f);

            QuadRedPoints.Size.X = 47f * redRatio - (QuadRedPoints.RelativePosition_V3.X + 23.4);
            QuadBluePoints.Size.X = 47f * blueRatio - (-QuadBluePoints.RelativePosition_V3.X + 23.4);
            LabelRedPoints.RelativePosition_V3.X = QuadRedPoints.RelativePosition_V3.X + QuadRedPoints.Size.X / 2;
            LabelBluePoints.RelativePosition_V3.X = -QuadRedPoints.RelativePosition_V3.X - QuadBluePoints.Size.X / 2;
        }

        if (IsVisible() != PrevVisible)
        {
            FrameStatus.Visible = IsVisible();

            PrevVisible = IsVisible();
        }
    }

    private void ChangeStatusPosition()
    {
        if (IsInGameMenuDisplayed)
        {
            AnimMgr.Add(FrameStatus, "<frame pos=\"0 80\" scale=\"1.25\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        }
        else
        {
            AnimMgr.Add(FrameStatus, "<frame pos=\"0 88\" scale=\"0.85\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
        }
    }
}