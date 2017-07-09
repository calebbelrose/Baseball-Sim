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

    void Start()
    {
		int pick = 29;

        temp = new Team[Manager.Instance.GetNumTeams()];
        Manager.Instance.teams.CopyTo(temp, 0);
        Sort(0, temp.Length - 1);
        championPanel = GameObject.Find("pnlChampion");
        championPanel.SetActive(false);
        Manager.Instance.inFinals = true;
        PlayerPrefs.SetString("InFinals", Manager.Instance.inFinals.ToString());
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

        for (int i = 0; i < temp.Length; i++)
			Manager.Instance.teams[temp[i].id].Pick = pick--;

        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = temp[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].CityName + " " + top8[i].TeamName;
            schedule[i] = 7 - i;
			top8[i].ResetWins ();
			top8[i].ResetLosses ();
        }
    }

	// Sorts players to determine who's in the finals
    void Sort(int left, int right)
    {
        int i = left, j = right;
        string pivot = temp[(int)(left + (right - left) / 2)].Wins.ToString();

        while (i <= j)
        {
            while (string.Compare(temp[i].Wins.ToString(), pivot) > 0)
            {
                i++;
            }

            while (string.Compare(temp[j].Wins.ToString(), pivot) < 0)
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

	// Play a game
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

            if (top8[teams[0]].Losses < 4 && top8[teams[1]].Losses < 4)
            {
                int inning = 1;
                int[] scores = new int[2] { 0, 0 };
                int[] batters = new int[2] { 0, 0 };
                int[] relievers = new int[2] { 0, 0 };
                int[] pitchers = new int[2] { 0, 0 };
                int[] staminas = new int[2];

				pitchers[0] = Manager.Instance.teams[teams[0]].SP[Manager.Instance.teams[teams[0]].CurrStarter];
				pitchers[1] = Manager.Instance.teams[teams[1]].SP[Manager.Instance.teams[teams[1]].CurrStarter];

				Manager.Instance.Players[Manager.Instance.teams[teams[0]].players[pitchers[0]]].stats[0][0]++;
				Manager.Instance.Players[Manager.Instance.teams[teams[0]].players[pitchers[0]]].stats[0][17]++;

				Manager.Instance.Players[Manager.Instance.teams[teams[1]].players[pitchers[1]]].stats[0][0]++;
				Manager.Instance.Players[Manager.Instance.teams[teams[1]].players[pitchers[1]]].stats[0][17]++;

                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < Manager.Instance.teams[teams[j]].Batters.Count; k++)
						Manager.Instance.Players[Manager.Instance.teams[teams[j]].players[Manager.Instance.teams[teams[j]].Batters[k][0]]].stats[0][0]++;

				staminas[0] = Manager.Instance.Players[Manager.Instance.teams[teams[0]].players[pitchers[0]]].skills[7];
				staminas[1] = Manager.Instance.Players[Manager.Instance.teams[teams[1]].players[pitchers[1]]].skills[7];

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
                            pitchers[thisTeam] = Manager.Instance.teams[teams[thisTeam]].CP[0];
							staminas[thisTeam] = Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].skills[7];
                        }

                        while (outs < 3)
                        {
                            bool strike, swing;
                            float thisEye, thisContact, thisPower;
							float eye = Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[Manager.Instance.teams[teams[otherTeam]].Batters[batters[otherTeam]][0]]].skills[2] / 100.0f;
							float contact = Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[Manager.Instance.teams[teams[otherTeam]].Batters[batters[otherTeam]][0]]].skills[1] / 100.0f;

                            bases[0] = true;

                            if (staminas[thisTeam] <= 0 && strikes == 0 && balls == 0 && relievers[thisTeam] < 2)
                            {
                                pitchers[thisTeam] = Manager.Instance.teams[teams[thisTeam]].RP[relievers[thisTeam]];
								staminas[thisTeam] = Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].skills[7];
                                relievers[thisTeam]++;
                            }

                            if (Random.value < 0.5f)
                                strike = true;
                            else
                                strike = false;

                            if (Random.value > Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].skills[5] / 100.0f)
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
									thisPower *= Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[Manager.Instance.teams[teams[otherTeam]].Batters[batters[otherTeam]][0]]].skills[0] / 100.0f;

                                    if (thisPower > 0.8f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Homerun");*/
										Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][7]++;
										Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][22]++;
										Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][25]++;
                                        numBases = 4;
                                    }
                                    else
                                    {
										thisPower *= Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[Manager.Instance.teams[teams[otherTeam]].Batters[batters[otherTeam]][0]]].skills[3] / 100.0f;

                                        if (thisPower > 0.65f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Triple");*/
											Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][6]++;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][22]++;
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
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20]++;
                                        }
                                        else if (thisPower > 0.45f)
                                        {
                                            numBases = 2;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Double");*/
											Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][5]++;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][22]++;
                                        }
                                        else if (thisPower > 0.35f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Popout");*/
                                            numBases = 0;
                                            outs++;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20]++;
                                        }
                                        else if (thisPower > 0.2f)
                                        {
                                            numBases = 1;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Single");*/
											Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][3]++;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][22]++;
                                        }
                                        else if (thisPower > 0.10f)
                                        {
                                            numBases = 0;
                                            outs++;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20]++;
                                            /*if (i == 0)
                                                Debug.Log("Lineout");*/
                                        }
                                        else
                                        {
                                            numBases = 0;
                                            if (bases[1])
                                            {
                                                outs += 2;
												Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20] += 2;
                                                bases[1] = false;
                                                /*if (i == 0)
                                                    Debug.Log("Double Play");*/
                                            }
                                            else
                                            {
                                                outs++;
												Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20]++;
                                                /*if (i == 0)
                                                    Debug.Log("Groundout");*/
                                            }
                                        }
                                        Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][1]++;
										Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][21]++;
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
													Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][9]++;
													Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][23]++;
													Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][24]++;
                                                }
                                                else
                                                    bases[k + numBases] = true;
                                            }
                                        if (fly && advanced)
                                        {
											Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][14]++;
                                            Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][1]--;
											Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][21]--;
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
								Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][20]++;
								Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][21]++;
								Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][1]++;
                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Strikeout");*/
                                batters[otherTeam] = (batters[otherTeam] + 1) % 9;
								Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][11]++;
								Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][27]++;
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
									Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][9]++;
                                }
                                else
                                    for (int k = currBase; k > 0; k--)
                                    {
                                        bases[k] = true;
                                        bases[k - 1] = false;
                                    }

                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Walk");*/

								Manager.Instance.Players[Manager.Instance.teams[teams[otherTeam]].players[batters[otherTeam]]].stats[0][10]++;
								Manager.Instance.Players[Manager.Instance.teams[teams[thisTeam]].players[pitchers[thisTeam]]].stats[0][26]++;

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
					top8[teams[0]].Win ();
                    GameObject.Find("txtWins" + round.ToString() + teams[0]).GetComponent<Text>().text = top8[teams[0]].Wins.ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[0]].Wins);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].Wins, top8[teams[1]].Wins);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].Wins, top8[teams[0]].Wins);
                    }
                }
                else if (scores[1] > scores[0])
                {
					top8[teams[1]].Win ();
                    GameObject.Find("txtWins" + round.ToString() + teams[1]).GetComponent<Text>().text = top8[teams[1]].Wins.ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[1]].Wins);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].Wins, top8[teams[1]].Wins);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].Wins, top8[teams[0]].Wins);
                    }
                }
                PlayerPrefs.Save();
            }
            else
            {
                if (!winners[i])
                {
                    if (top8[teams[0]].Wins == 4)
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
							top8[schedule[k]].ResetWins ();
							top8[schedule[k]].ResetLosses ();
                            GameObject.Find("txtTeam" + schedule[k]).GetComponent<Text>().name = "txtWinner";
                            Text text = GameObject.Find("txt" + round.ToString() + k.ToString()).GetComponent<Text>();
                            text.name = "txtTeam" + schedule[k];
                            text.text = top8[schedule[k]].CityName + " " + top8[schedule[k]].TeamName;
                            GameObject.Find("txtWins" + round.ToString() + k.ToString()).GetComponent<Text>().name = "txtWins" + round.ToString() + schedule[k];
                        }
                    }
                    else
                    {
                        championPanel.SetActive(true);
                        DetermineMVP();
                        GameObject.Find("txtChampion").GetComponent<Text>().text = top8[newRound[i]].CityName + " " + top8[newRound[i]].TeamName;
                        Manager.Instance.needDraft = true;
                        PlayerPrefs.SetString("NeedDraft", Manager.Instance.needDraft.ToString());
                        Manager.Instance.inFinals = false;
                        PlayerPrefs.SetString("InFinals", Manager.Instance.inFinals.ToString());
						Manager.Instance.NewYear();
                        PlayerPrefs.SetString("Year", Manager.Instance.Year.ToString());

                        for (int k = 0; k < Manager.Instance.teams.Count; k++)
                        {
							Manager.Instance.teams[k].ResetWins ();
							Manager.Instance.teams[k].ResetLosses ();
                            PlayerPrefs.SetString("WLH" + top8[teams[i]].id.ToString(), "0,0,0.5");

							for (int l = 0; l < Manager.Instance.teams [k].players.Count; l++) {
								int increase;

								if (Manager.Instance.Players[Manager.Instance.teams [k].players [l]].potential <= 0)
									Manager.Instance.Players[Manager.Instance.teams [k].players [l]].offense = Manager.Instance.Players[Manager.Instance.teams [k].players [l]].potential - (int)(Random.value * 10);

								increase = (int)Mathf.Ceil (Manager.Instance.Players[Manager.Instance.teams [k].players [l]].potential * 4 / 22f);
								Manager.Instance.Players[Manager.Instance.teams [k].players [l]].potential -= increase;

								for (int m = 0; m < increase; m++) {
									int currStat = (int)(Random.value * Manager.Instance.Players[Manager.Instance.teams [k].players [l]].skills.Length);
									Manager.Instance.Players[Manager.Instance.teams [k].players [l]].skills[currStat]++;
								}

								Manager.Instance.Players[Manager.Instance.teams [k].players [l]].SavePlayer();
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

	// Displays the result of the player's game (if they have one)
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

        GameObject.Find("txtWL").GetComponent<Text>().text = "W/L: " + Manager.Instance.teams[0].Wins + "/" + Manager.Instance.teams[0].Losses;
        GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
    }

	// Determines who the MVP and Cy Young award winners are
    void DetermineMVP()
    {
        double mvpWorth, cyWorth, bestMVP = 0.0, bestCY = 0.0, mvpOPS = 0.0, cyERA = 0.0;
        int[] mvp = new int[2], cy = new int[2];
        for (int i = 0; i < Manager.Instance.GetNumTeams(); i++)
            for(int j = 0; j < Manager.Instance.teams[i].players.Count; j++)
            {
                
				if(Manager.Instance.Players[Manager.Instance.teams[i].players[j]].position.Contains("P"))
                {
					double era = Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][24] * 27 / (double)Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][20];
					cyWorth = (6.0 - era) * 5 + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][20] / (double)8;
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
					double ops = (Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][3] + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][10]) / (double)(Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][1] + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][10] + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][14]) + (Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][3] + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][5] * 2 + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][6] * 3 + Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][7] * 4) / (double)Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][1];
					mvpWorth = Manager.Instance.Players[Manager.Instance.teams[i].players[j]].stats[0][7] / 40.0 + ops * 25;
                    if (mvpWorth > bestMVP)
                    {
                        bestMVP = mvpWorth;
                        mvp[0] = i;
                        mvp[1] = j;
                        mvpOPS = ops;
                    }
                }
            }
		GameObject.Find("txtMVP").GetComponent<Text>().text = Manager.Instance.Players[Manager.Instance.teams[mvp[0]].players[mvp[1]]].firstName + " " + Manager.Instance.Players[Manager.Instance.teams[mvp[0]].players[mvp[1]]].lastName + " (" + Manager.Instance.teams[mvp[0]].Shortform + ") OPS: " + mvpOPS.ToString("0.000") + " HR: " + Manager.Instance.Players[Manager.Instance.teams[mvp[0]].players[mvp[1]]].stats[0][7];
		GameObject.Find("txtCY").GetComponent<Text>().text = Manager.Instance.Players[Manager.Instance.teams[cy[0]].players[cy[1]]].firstName + " " + Manager.Instance.Players[Manager.Instance.teams[cy[0]].players[cy[1]]].lastName + " (" + Manager.Instance.teams[cy[0]].Shortform + ") ERA: " + cyERA.ToString("0.00") + " SO: " + Manager.Instance.Players[Manager.Instance.teams[cy[0]].players[cy[1]]].stats[0][27];
    }
}