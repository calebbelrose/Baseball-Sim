using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Top8 : MonoBehaviour {

    Team[] top8 = new Team[8];
    Team[] temp;
    int[] schedule = new int[8];
    int[] newRound = new int[4];
    bool[] winners = new bool[4];
    int numWinners = 0, round;
    GameObject championPanel;
    AllTeams allTeams;

    void Start()
    {
        allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        temp = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(temp, 0);
        Sort(0, temp.Length - 1);
        championPanel = GameObject.Find("pnlChampion");
        championPanel.SetActive(false);
        allTeams.inFinals = true;
        PlayerPrefs.SetString("InFinals", allTeams.inFinals.ToString());
        PlayerPrefs.Save();

        if (PlayerPrefs.HasKey("Finals Round"))
        {
            round = PlayerPrefs.GetInt("Finals Round");
            int numTeams = temp.Length;
            for (int i = 0; i < round; i++)
            {
                for (int j = 0; j < numTeams; j++)
                    GameObject.Find("txtWins" + round.ToString() + numTeams).GetComponent<Text>().text = PlayerPrefs.GetInt("txtWins" + round.ToString() + numTeams).ToString();
                numTeams /= 2;
            }
        }
        else
            round = 1;

        for (int i = 0; i < allTeams.teams.Count; i++)
            allTeams.teams[i].pick = i;

        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = temp[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].cityName + " " + top8[i].teamName;
            schedule[i] = 7 - i;
            top8[i].wins = 0;
            top8[i].losses = 0;
        }
    }

    void Sort(int left, int right)
    {
        int i = left, j = right;
        string pivot = temp[(int)(left + (right - left) / 2)].wins.ToString();

        while (i <= j)
        {
            while (string.Compare(temp[i].wins.ToString(), pivot) > 0)
            {
                i++;
            }

            while (string.Compare(temp[j].wins.ToString(), pivot) < 0)
            {
                j--;
            }

            if (i <= j)
            {
                Team tempT = new Team();

                tempT = temp[i];
                temp[i] = temp[j];
                temp[j] = tempT;

                i++;
                j--;
            }
        }

        if (left < j)
        {
            Sort(left, j);
        }

        if (i < right)
        {
            Sort(i, right);
        }
    }

    public void PlayGame()
    {
        string result = "";
        bool player = false;
        int you = 0, them = 0;

        for (int i = 0; i < newRound.Length; i++)
        {
            int[] teams = new int[2];
            teams[0] = schedule[i];
            teams[1] = schedule[schedule.Length - i - 1];

            if (top8[teams[0]].losses < 4 && top8[teams[1]].losses < 4)
            {
                int inning = 1;
                int[] scores = new int[2] { 0, 0 };
                int[] batters = new int[2] { 0, 0 };
                int[] relievers = new int[2] { 0, 0 };
                int[] pitchers = new int[2] { 0, 0 };
                int[] staminas = new int[2];

                pitchers[0] = allTeams.teams[teams[0]].SP[allTeams.currStarter];
                pitchers[1] = allTeams.teams[teams[1]].SP[allTeams.currStarter];

				allTeams.teams[teams[0]].players[pitchers[0]].games++;
				allTeams.teams[teams[0]].players[pitchers[0]].gamesStarted++;

				allTeams.teams[teams[1]].players[pitchers[1]].games++;
				allTeams.teams[teams[1]].players[pitchers[1]].gamesStarted++;

                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < allTeams.teams[teams[j]].Batters.Count; k++)
                        allTeams.teams[teams[j]].players[allTeams.teams[teams[j]].Batters[k]].games++;

				staminas[0] = allTeams.teams[teams[0]].players[pitchers[0]].skills[7];
				staminas[1] = allTeams.teams[teams[1]].players[pitchers[1]].skills[7];

                while (inning <= 9 || scores[0] == scores[1])
                {
                    for (int j = 0; j < 2; j++)
                    {
                        /*if (i == 0)
                        {
                            if (j == 0)
                                Debug.Log("Top of the " + (inning + 1));
                            else
                                Debug.Log("Bottom of the " + (inning + 1));
                        }*/
                        int outs = 0, thisTeam = j, otherTeam = (j + 1) % 2, strikes = 0, balls = 0;
                        bool[] bases = new bool[4] { false, false, false, false };

                        if (inning == 9 && scores[thisTeam] > scores[otherTeam])
                        {
                            pitchers[thisTeam] = allTeams.teams[teams[thisTeam]].CP[0];
							staminas[thisTeam] = allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].skills[7];
                        }

                        while (outs < 3)
                        {
                            bool strike, swing;
                            float thisEye, thisContact, thisPower;
							float eye = allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]].skills[2] / 100.0f;
							float contact = allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]].skills[1] / 100.0f;

                            bases[0] = true;

                            if (staminas[thisTeam] <= 0 && strikes == 0 && balls == 0 && relievers[thisTeam] < 2)
                            {
                                pitchers[thisTeam] = allTeams.teams[teams[thisTeam]].RP[relievers[thisTeam]];
								staminas[thisTeam] = allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].skills[7];
                                relievers[thisTeam]++;
                            }

                            if (Random.value < 0.5f)
                                strike = true;
                            else
                                strike = false;

                            if (Random.value > allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].skills[5] / 100.0f)
                                strike = !strike;

                            thisEye = Random.value;
                            if (thisEye < eye)
                                swing = true;
                            else
                                swing = false;

                            if (swing)
                            {
                                thisContact = Random.value;
                                if (thisContact < contact)
                                {
                                    int numBases;
                                    bool fly = false;

                                    thisPower = Random.value;
									thisPower *= allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]].skills[0] / 100.0f;

                                    if (thisPower > 0.8f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Homerun");*/
										allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].homeruns++;
										allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].hitsAgainst++;
										allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].homerunsAgainst++;
                                        numBases = 4;
                                    }
                                    else
                                    {
										thisPower *= allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]].skills[3] / 100.0f;

                                        if (thisPower > 0.65f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Triple");*/
											allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].triples++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].hitsAgainst++;
                                            numBases = 4;
                                        }
                                        else if (thisPower > 0.55f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Flyout");*/
                                            fly = true;
                                            numBases = 1;
                                            bases[0] = false;
                                            outs++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched++;
                                        }
                                        else if (thisPower > 0.45f)
                                        {
                                            numBases = 2;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Double");*/
											allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].doubles++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].hitsAgainst++;
                                        }
                                        else if (thisPower > 0.35f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Popout");*/
                                            numBases = 0;
                                            outs++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched++;
                                        }
                                        else if (thisPower > 0.2f)
                                        {
                                            numBases = 1;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Single");*/
											allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].hits++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].hitsAgainst++;
                                        }
                                        else if (thisPower > 0.10f)
                                        {
                                            numBases = 0;
                                            outs++;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched++;
                                            /*if (i == 0)
                                                Debug.Log("Lineout");*/
                                        }
                                        else
                                        {
                                            numBases = 0;
                                            if (bases[1])
                                            {
                                                outs += 2;
												allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched += 2;
                                                bases[1] = false;
                                                /*if (i == 0)
                                                    Debug.Log("Double Play");*/
                                            }
                                            else
                                            {
                                                outs++;
												allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched++;
                                                /*if (i == 0)
                                                    Debug.Log("Groundout");*/
                                            }
                                        }
                                        allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].atBats++;
										allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].atBatsAgainst++;
                                    }

                                    if (outs < 3)
                                    {
                                        bool advanced = false;
                                        for (int k = 3; k >= 0; k--)
                                            if (bases[k])
                                            {
                                                advanced = true;
                                                bases[k] = false;
                                                if (k + numBases > 3)
                                                {
                                                    scores[otherTeam]++;
													allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].runsBattedIn++;
													allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].runsAgainst++;
													allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].earnedRuns++;
                                                }
                                                else
                                                    bases[k + numBases] = true;
                                            }
                                        if (fly && advanced)
                                        {
											allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].sacrifices++;
                                            allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].atBats--;
											allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].atBatsAgainst--;
                                        }
                                    }

                                    batters[otherTeam] = (batters[otherTeam] + 1) % 9;
                                    strikes = 0;
                                    balls = 0;
                                }
                                else
                                    strikes++;
                            }
                            else
                            {
                                if (strike)
                                    strikes++;
                                else
                                    balls++;
                            }

                            if (strikes == 3)
                            {
                                outs++;
								allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].inningsPitched++;
								allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].atBatsAgainst++;
								allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].atBats++;
                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Strikeout");*/
                                batters[otherTeam] = (batters[otherTeam] + 1) % 9;
								allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].strikeouts++;
								allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].strikeoutsAgainst++;
                                strikes = 0;
                                balls = 0;
                            }
                            else if (balls == 4)
                            {
                                int currBase = 0;

                                while (currBase < bases.Length && bases[currBase])
                                    currBase++;
                                if (currBase == 4)
                                {
                                    scores[otherTeam]++;
									allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].runsBattedIn++;
                                }
                                else
                                    for (int k = currBase; k > 0; k--)
                                    {
                                        bases[k] = true;
                                        bases[k - 1] = false;
                                    }

                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Walk");*/

								allTeams.teams[teams[otherTeam]].players[batters[otherTeam]].walks++;
								allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]].walksAgainst++;

                                batters[otherTeam] = (batters[otherTeam] + 1) % 9;
                                strikes = 0;
                                balls = 0;
                            }
                            staminas[thisTeam]--;
                        }
                    }

                    inning++;
                }

                if (scores[0] > scores[1])
                {
                    top8[teams[0]].wins++;
                    GameObject.Find("txtWins" + round.ToString() + teams[0]).GetComponent<Text>().text = top8[teams[0]].wins.ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[0]].wins);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].wins, top8[teams[1]].wins);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].wins, top8[teams[0]].wins);
                    }
                }
                else if (scores[1] > scores[0])
                {
                    top8[teams[1]].wins++;
                    GameObject.Find("txtWins" + round.ToString() + teams[1]).GetComponent<Text>().text = top8[teams[1]].wins.ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[1]].wins);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].wins, top8[teams[1]].wins);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].wins, top8[teams[0]].wins);
                    }
                }
                PlayerPrefs.Save();
            }
            else
            {
                if (!winners[i])
                {
                    if (top8[teams[0]].wins == 4)
                        newRound[i] = teams[0];
                    else
                        newRound[i] = teams[1];
                    winners[i] = true;
                    numWinners++;
                }
                if (numWinners == newRound.Length)
                {
                    if (round != 3)
                    {
                        schedule = newRound;
                        newRound = new int[newRound.Length / 2];
                        numWinners = 0;
                        winners = new bool[newRound.Length];
                        round++;

                        for (int k = 0; k < schedule.Length; k++)
                        {
                            top8[schedule[k]].wins = 0;
                            top8[schedule[k]].losses = 0;
                            GameObject.Find("txtTeam" + schedule[k]).GetComponent<Text>().name = "txtWinner";
                            Text text = GameObject.Find("txt" + round.ToString() + k.ToString()).GetComponent<Text>();
                            text.name = "txtTeam" + schedule[k];
                            text.text = top8[schedule[k]].cityName + " " + top8[schedule[k]].teamName;
                            GameObject.Find("txtWins" + round.ToString() + k.ToString()).GetComponent<Text>().name = "txtWins" + round.ToString() + schedule[k];
                        }
                    }
                    else
                    {
                        championPanel.SetActive(true);
                        DetermineMVP();
                        GameObject.Find("txtChampion").GetComponent<Text>().text = top8[newRound[i]].cityName + " " + top8[newRound[i]].teamName;
                        allTeams.needDraft = true;
                        PlayerPrefs.SetString("NeedDraft", allTeams.needDraft.ToString());
                        allTeams.inFinals = false;
                        PlayerPrefs.SetString("InFinals", allTeams.inFinals.ToString());
                        allTeams.year++;
                        PlayerPrefs.SetString("Year", allTeams.year.ToString());
                        allTeams.numPlays = 0;
                        PlayerPrefs.SetInt("NumPlays", 0);

                        for (int k = 0; k < allTeams.teams.Count; k++)
                        {
                            allTeams.teams[k].wins = 0;
                            allTeams.teams[k].losses = 0;
                            PlayerPrefs.SetString("WL" + top8[teams[i]].id.ToString(), "0,0");

							for (int l = 0; l < allTeams.teams [k].players.Count; l++) {
								int increase;

								if (allTeams.teams [k].players [l].potential <= 0)
									allTeams.teams [k].players [l].offense = allTeams.teams [k].players [l].potential - (int)(Random.value * 10);

								increase = (int)Mathf.Ceil (allTeams.teams [k].players [l].potential * 4 / 22f);
								allTeams.teams [k].players [l].potential -= increase;

								for (int m = 0; m < increase; m++) {
									int currStat = (int)(Random.value * allTeams.teams [k].players [l].skills.Length);
									allTeams.teams [k].players [l].skills[currStat]++;
								}

								allTeams.teams [k].players [l].SavePlayer(k, l);
							}
                        }

                        PlayerPrefs.Save();
                    }
                }
            }
        }
        if (!player)
            GameObject.Find("pnlScore").gameObject.SetActive(false);
    }

    void DisplayResult(string result, int you, int them, int yourWins, int theirWins)
    {
        GameObject.Find("txtYourScore").GetComponent<Text>().text = "You: " + you;
        Text txtResult = GameObject.Find("txtResult").GetComponent<Text>();

        txtResult.text = result;

        if (result == "Win")
            txtResult.color = Color.green;
        else if (result == "Loss")
            txtResult.color = Color.red;
        else
            txtResult.color = Color.white;

        GameObject.Find("txtWL").GetComponent<Text>().text = "W/L: " + allTeams.teams[0].wins + "/" + allTeams.teams[0].losses;
        GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
    }

    void DetermineMVP()
    {
        double mvpWorth, cyWorth, bestMVP = 0.0, bestCY = 0.0, mvpOPS = 0.0, cyERA = 0.0;
        int[] mvp = new int[2], cy = new int[2];
        for (int i = 0; i < allTeams.GetNumTeams(); i++)
            for(int j = 0; j < allTeams.teams[i].players.Count; j++)
            {
                
				if(allTeams.teams[i].players[j].position.Contains("P"))
                {
					double era = allTeams.teams[i].players[j].earnedRuns * 27 / (double)allTeams.teams[i].players[j].inningsPitched;
					cyWorth = (6.0 - era) * 5 + allTeams.teams[i].players[j].inningsPitched / (double)8;
                    if (cyWorth > bestCY)
                    {
                        bestCY = cyWorth;
                        cy[0] = i;
                        cy[1] = j;
                        cyERA = era;
                    }
                }
                else
                {
					double ops = (allTeams.teams[i].players[j].hits + allTeams.teams[i].players[j].walks) / (double)(allTeams.teams[i].players[j].atBats + allTeams.teams[i].players[j].walks + allTeams.teams[i].players[j].sacrifices) + (allTeams.teams[i].players[j].hits + allTeams.teams[i].players[j].doubles * 2 + allTeams.teams[i].players[j].triples * 3 + allTeams.teams[i].players[j].homeruns * 4) / (double)allTeams.teams[i].players[j].atBats;
					mvpWorth = allTeams.teams[i].players[j].homeruns / 40.0 + ops * 25;
                    if (mvpWorth > bestMVP)
                    {
                        bestMVP = mvpWorth;
                        mvp[0] = i;
                        mvp[1] = j;
                        mvpOPS = ops;
                    }
                }
            }
		GameObject.Find("txtMVP").GetComponent<Text>().text = allTeams.teams[mvp[0]].players[mvp[1]].firstName + " " + allTeams.teams[mvp[0]].players[mvp[1]].lastName + " (" + allTeams.teams[mvp[0]].shortform + ") OPS: " + mvpOPS.ToString("0.000") + " HR: " + allTeams.teams[mvp[0]].players[mvp[1]].homeruns;
		GameObject.Find("txtCY").GetComponent<Text>().text = allTeams.teams[cy[0]].players[cy[1]].firstName + " " + allTeams.teams[cy[0]].players[cy[1]].lastName + " (" + allTeams.teams[cy[0]].shortform + ") ERA: " + cyERA.ToString("0.00") + " SO: " + allTeams.teams[cy[0]].players[cy[1]].strikeoutsAgainst;
    }
}
