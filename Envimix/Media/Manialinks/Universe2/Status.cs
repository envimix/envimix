namespace Envimix.Media.Manialinks.Universe2;

public class Status : CTmMlScriptIngame, IContext
{
    public int Start;
    public bool PreviousIsVisible;
    public bool PreviousIsInMenu;
    public IList<int> PrevClanScores;
    public IList<int> PreviousClanScores;
    public IList<int> PreviousClanScoresTime;

    [ManialinkControl] public required CMlFrame FrameAllTimeLimit;
    [ManialinkControl] public required CMlFrame FrameTeam;
    [ManialinkControl] public required CMlQuad QuadTeam;
    [ManialinkControl] public required CMlFrame FrameTeamLabel;
    [ManialinkControl] public required CMlLabel LabelTeam;
    [ManialinkControl] public required CMlFrame FramePointStatus;
    [ManialinkControl] public required CMlLabel LabelRedPoints;
    [ManialinkControl] public required CMlLabel LabelBluePoints;
    [ManialinkControl] public required CMlQuad QuadRedPoints;
    [ManialinkControl] public required CMlQuad QuadBluePoints;

    [Netread] public bool CarSelectionMode { get; }
    [Netread] public int CurrentWarmUpNb { get; }

    public Status()
    {
        QuadTeam.MouseClick += () =>
        {
            var manialinkUrl = Teams[GetPlayer().CurrentClan].PresentationManialinkUrl;

            if (manialinkUrl != "")
            {
                OpenLink(manialinkUrl, CMlScript.LinkType.ManialinkBrowser);
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

    static bool IsVisible()
    {
        return true;//!IsInGameMenuDisplayed;
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

        FrameTeam.Visible = IsVisible();
        PreviousIsVisible = IsVisible();
        PreviousIsInMenu = IsInGameMenuDisplayed;

        PrevClanScores = new[] { ClanScores[0], ClanScores[1], ClanScores[2] };
        PreviousClanScores = new[] { ClanScores[0], ClanScores[1], ClanScores[2] };
        PreviousClanScoresTime = new[] { 0, 0, 0 };

        Wait(() => GetPlayer() is not null);
    }

    public void Loop()
    {
        if (IsVisible() != PreviousIsVisible)
        {
            if (IsVisible())
            {
                foreach (var control in FrameTeam.Controls)
                {
                    if (control is not CMlQuad quad)
                    {
                        continue;
                    }

                    quad.Size.X = 0;

                    if (quad.ControlId == "QuadTeam")
                    {
                        AnimMgr.Add(quad, "<quad size=\"39 6.25\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                    else
                    {
                        AnimMgr.Add(quad, "<quad size=\"40 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                }
            }

            Start = Now;
            FrameTeam.Visible = IsVisible();
            PreviousIsVisible = IsVisible();
        }

        if (IsInGameMenuDisplayed != PreviousIsInMenu)
        {
            if (IsInGameMenuDisplayed)
            {
                AnimMgr.Add(FrameAllTimeLimit, "<frame pos=\"0 80\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                foreach (var control in FrameTeam.Controls)
                {
                    if (control is not CMlQuad quad)
                    {
                        continue;
                    }

                    if (quad.ControlId == "QuadTeam")
                    {
                        AnimMgr.Add(quad, "<quad size=\"39 6.25\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                    else
                    {
                        AnimMgr.Add(quad, "<quad size=\"40 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                    }
                }

                AnimMgr.Add(FramePointStatus.Controls[0], "<quad size=\"50 10\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(QuadRedPoints, "<quad pos=\"-23.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(QuadBluePoints, "<quad pos=\"23.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
            }
            else
            {
                AnimMgr.Add(FrameAllTimeLimit, "<frame pos=\"0 88\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);

                foreach (var control in FrameTeam.Controls)
                {
                    if (control is CMlQuad quad)
                    {
                        if (quad.ControlId == "QuadTeam")
                        {
                            AnimMgr.Add(quad, "<quad size=\"49 6.25\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                        }
                        else
                        {
                            AnimMgr.Add(quad, "<quad size=\"50 7\"/>", 300, CAnimManager.EAnimManagerEasing.QuadOut);
                        }
                    }
                }

                AnimMgr.Add(FramePointStatus.Controls[0], "<quad size=\"70 10\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(QuadRedPoints, "<quad pos=\"-33.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
                AnimMgr.Add(QuadBluePoints, "<quad pos=\"33.5 0\"/>", 500, CAnimManager.EAnimManagerEasing.QuadOut);
            }

            PreviousIsInMenu = IsInGameMenuDisplayed;
        }

        FrameTeamLabel.ClipWindowSize.X = AnimLib.EaseOutQuad(Now - Start, 0, 40, 300);

        if (UseClans)
        {
            if (GetPlayer().Score is null)
            {
                QuadTeam.Hide();
            }
            else
            {
                LabelTeam.Value = $"$t{Teams[GetPlayer().Score.TeamNum - 1].Name}";
                QuadTeam.Colorize = Teams[GetPlayer().Score.TeamNum - 1].ColorPrimary;
            }

            QuadTeam.Show();
            FramePointStatus.Show();
        }
        else
        {
            LabelTeam.Value = GetPlayer().User.Name;
            QuadTeam.Hide();
            FramePointStatus.Hide();
        }

        for (var i = 0; i < ClanScores.Count; i++)
        {
            if (ClanScores[i] != PrevClanScores[i])
            {
                PreviousClanScores[i] = PrevClanScores[i];
                PreviousClanScoresTime[i] = Now;
                PrevClanScores[i] = ClanScores[i];
            }
        }

        var animatedClanScore1 = AnimLib.EaseOutQuad(Now - PreviousClanScoresTime[1], PreviousClanScores[1] * 1f, (ClanScores[1] - PreviousClanScores[1]) * 1f, 1000);
        var animatedClanScore2 = AnimLib.EaseOutQuad(Now - PreviousClanScoresTime[2], PreviousClanScores[2] * 1f, (ClanScores[2] - PreviousClanScores[2]) * 1f, 1000);

        LabelRedPoints.Value = ToNicerNumber(MathLib.NearestInteger(animatedClanScore1));
        LabelBluePoints.Value = ToNicerNumber(MathLib.NearestInteger(animatedClanScore2));

        var redRatio = .5f;
        var blueRatio = .5f;

        if (animatedClanScore1 + animatedClanScore2 > 0)
        {
            redRatio = 1f * animatedClanScore1 / (animatedClanScore1 + animatedClanScore2);
            blueRatio = 1f * animatedClanScore2 / (animatedClanScore1 + animatedClanScore2);
        }

        redRatio = MathLib.Clamp(redRatio, .1f, .9f);
        blueRatio = MathLib.Clamp(blueRatio, .1f, .9f);

        QuadRedPoints.Size.X = 47f * redRatio - (QuadRedPoints.RelativePosition_V3.X + 23.5);
        QuadBluePoints.Size.X = 47f * blueRatio - (-QuadBluePoints.RelativePosition_V3.X + 23.5);

        //if(BlueRatio >= .15) Label_Red_Points.Show();
        //else Label_Red_Points.Hide();
        LabelRedPoints.RelativePosition_V3.X = QuadRedPoints.RelativePosition_V3.X + QuadRedPoints.Size.X / 2;

        //if(RedRatio >= .15) Label_Blue_Points.Show();
        //else Label_Blue_Points.Hide();
        LabelBluePoints.RelativePosition_V3.X = -QuadRedPoints.RelativePosition_V3.X - QuadBluePoints.Size.X / 2;
    }
}