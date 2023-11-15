namespace Envimix.Media.Manialinks.Universe2;

public class Status2 : CTmMlScriptIngame, IContext
{
    public int Start;
    public bool PrevIsInMenu;
    public bool PrevTeamSelectionMode;

    [ManialinkControl] public required CMlFrame FrameStatus;
    [ManialinkControl] public required CMlQuad QuadRedPoints;
    [ManialinkControl] public required CMlQuad QuadBluePoints;
    [ManialinkControl] public required CMlQuad QuadJoinRed;
    [ManialinkControl] public required CMlQuad QuadJoinBlue;
    [ManialinkControl] public required CMlLabel LabelRedPoints;
    [ManialinkControl] public required CMlLabel LabelBluePoints;
    [ManialinkControl] public required CMlQuad QuadTeam;
    [ManialinkControl] public required CMlLabel LabelTeam;

    [Netread] public bool TeamSelectionMode { get; }

    public Status2()
    {
        QuadJoinRed.MouseClick += () =>
        {
            if (TeamSelectionMode)
            {
                Log("Joining Team Red...");
                SendCustomEvent("JoinTeam", new[] { "1" });
            }
        };

        QuadJoinBlue.MouseClick += () =>
        {
            if (TeamSelectionMode)
            {
                Log("Joining Team Blue...");
                SendCustomEvent("JoinTeam", new[] { "2" });
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
        return UseClans;//!IsInGameMenuDisplayed;
    }

    static string ToNicerNumber(int num)
    {
        var newText = "";

        for (var i = 0; i < TextLib.Length(num.ToString()); i++)
        {
            newText = $"{newText}{TextLib.SubText(num.ToString(), i, 1)}";

            if (((TextLib.Length(num.ToString()) - i - 1) % 3) == 0)
            {
                newText = newText + " ";
            }
        }

        return TextLib.SubText(newText, 0, TextLib.Length(newText) - 1);
    }

    public void Main()
    {
        Start = Now;
        PrevIsInMenu = IsInGameMenuDisplayed;

        ChangeStatusPosition();

        QuadRedPoints.ModulateColor = Teams[0].ColorPrimary;
        QuadBluePoints.ModulateColor = Teams[1].ColorPrimary;
        QuadJoinRed.ModulateColor = Teams[0].ColorPrimary;
        QuadJoinBlue.ModulateColor = Teams[1].ColorPrimary;

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
                                LabelBluePoints.SetText("");
                                QuadJoinRed.Visible = true;
                                QuadJoinBlue.Visible = false;
                            }
                            else if (t1 == 2)
                            {
                                LabelRedPoints.SetText("");
                                LabelBluePoints.SetText("Join");
                                QuadJoinRed.Visible = false;
                                QuadJoinBlue.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        if (TeamSelectionMode != PrevTeamSelectionMode)
        {
            QuadJoinRed.Visible = TeamSelectionMode;
            QuadJoinBlue.Visible = TeamSelectionMode;

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