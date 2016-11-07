using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Top8 : MonoBehaviour {

    Team[] top8 = new Team[2];
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
            round = 3;

        for (int i = 0; i < allTeams.teams.Length; i++)
            allTeams.teams[i].pick = i;

        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = temp[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].cityName + " " + top8[i].teamName;
            schedule[i] = 1 - i;
            top8[i].pwl[1] = 0;
            top8[i].pwl[2] = 0;
        }
    }

    void Sort(int left, int right)
    {
        int i = left, j = right;
        string pivot = temp[(int)(left + (right - left) / 2)].pwl[0].ToString();

        while (i <= j)
        {
            while (string.Compare(temp[i].pwl[0].ToString(), pivot) > 0)
            {
                i++;
            }

            while (string.Compare(temp[j].pwl[0].ToString(), pivot) < 0)
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
            int[] teams = new int[2] { 0, 0 };
            teams[0] = schedule[i];
            teams[1] = schedule[schedule.Length - i - 1];

            if (top8[teams[0]].pwl[1] < 4 && top8[teams[1]].pwl[1] < 4)
            {
                int inning = 1;
                int[] scores = new int[2] { 0, 0 };
                int[] batters = new int[2] { 0, 0 };
                int[] relievers = new int[2] { 0, 0 };
                int[] pitchers = new int[2] { 0, 0 };
                int[] staminas = new int[2];

                pitchers[0] = allTeams.teams[teams[0]].SP[allTeams.currStarter];
                pitchers[1] = allTeams.teams[teams[1]].SP[allTeams.currStarter];

                allTeams.teams[teams[0]].pStats[pitchers[0]][0]++;
                allTeams.teams[teams[0]].pStats[pitchers[0]][21]++;

                allTeams.teams[teams[1]].pStats[pitchers[1]][0]++;
                allTeams.teams[teams[1]].pStats[pitchers[1]][21]++;

                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < allTeams.teams[teams[j]].Batters.Count; k++)
                        allTeams.teams[teams[j]].pStats[allTeams.teams[teams[j]].Batters[k]][0]++;

                staminas[0] = int.Parse(allTeams.teams[teams[0]].players[pitchers[0]][14]);
                staminas[1] = int.Parse(allTeams.teams[teams[1]].players[pitchers[1]][14]);

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
                            staminas[thisTeam] = int.Parse(allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]][14]);
                        }

                        while (outs < 3)
                        {
                            bool strike, swing;
                            float thisEye, thisContact, thisPower;
                            float eye = float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][10]) / 100;
                            float contact = float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][9]) / 100;

                            bases[0] = true;

                            if (staminas[thisTeam] <= 0 && strikes == 0 && balls == 0 && relievers[thisTeam] < 2)
                            {
                                pitchers[thisTeam] = allTeams.teams[teams[thisTeam]].RP[relievers[thisTeam]];
                                staminas[thisTeam] = int.Parse(allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]][14]);
                                relievers[thisTeam]++;
                            }

                            if (Random.value < 0.5f)
                                strike = true;
                            else
                                strike = false;

                            if (Random.value > float.Parse(allTeams.teams[teams[thisTeam]].players[pitchers[thisTeam]][13]))
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
                                    thisPower *= float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][8]) / 100;

                                    if (thisPower > 0.8f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Homerun");*/
                                        allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][6]++;
                                        allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][26]++;
                                        allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][29]++;
                                        numBases = 4;
                                    }
                                    else
                                    {
                                        thisPower *= float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][11]) / 100;

                                        if (thisPower > 0.65f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Triple");*/
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][5]++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][26]++;
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
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                        }
                                        else if (thisPower > 0.45f)
                                        {
                                            numBases = 2;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Double");*/
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][4]++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][26]++;
                                        }
                                        else if (thisPower > 0.35f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Popout");*/
                                            numBases = 0;
                                            outs++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                        }
                                        else if (thisPower > 0.2f)
                                        {
                                            numBases = 1;
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Single");*/
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][3]++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][26]++;
                                        }
                                        else if (thisPower > 0.10f)
                                        {
                                            numBases = 0;
                                            outs++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                            /*if (i == 0)
                                                Debug.Log("Lineout");*/
                                        }
                                        else
                                        {
                                            numBases = 0;
                                            if (bases[1])
                                            {
                                                outs += 2;
                                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24] += 2;
                                                bases[1] = false;
                                                /*if (i == 0)
                                                    Debug.Log("Double Play");*/
                                            }
                                            else
                                            {
                                                outs++;
                                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                                /*if (i == 0)
                                                    Debug.Log("Groundout");*/
                                            }
                                        }
                                        allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][1]++;
                                        allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][25]++;
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
                                                    allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][8]++;
                                                    allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][27]++;
                                                    allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][28]++;
                                                }
                                                else
                                                    bases[k + numBases] = true;
                                            }
                                        if (fly && advanced)
                                        {
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][13]++;
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][1]--;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][25]--;
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
                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][25]++;
                                allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][1]++;
                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Strikeout");*/
                                batters[otherTeam] = (batters[otherTeam] + 1) % 9;
                                allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][10]++;
                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][31]++;
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
                                    allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][8]++;
                                }
                                else
                                    for (int k = currBase; k > 0; k--)
                                    {
                                        bases[k] = true;
                                        bases[k - 1] = false;
                                    }

                                /*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Walk");*/

                                allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][9]++;
                                allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][30]++;

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
                    top8[teams[0]].pwl[1]++;
                    GameObject.Find("txtWins" + round.ToString() + teams[0]).GetComponent<Text>().text = top8[teams[0]].pwl[1].ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[0]].pwl[1]);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].pwl[1], top8[teams[1]].pwl[1]);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].pwl[1], top8[teams[0]].pwl[1]);
                    }
                }
                else if (scores[1] > scores[0])
                {
                    top8[teams[1]].pwl[1]++;
                    GameObject.Find("txtWins" + round.ToString() + teams[1]).GetComponent<Text>().text = top8[teams[1]].pwl[1].ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + teams[1], top8[teams[1]].pwl[1]);

                    if (top8[teams[0]].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = scores[0];
                        them = scores[1];
                        DisplayResult(result, you, them, top8[teams[0]].pwl[1], top8[teams[1]].pwl[1]);
                    }
                    else if (top8[teams[1]].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = scores[1];
                        them = scores[0];
                        DisplayResult(result, you, them, top8[teams[1]].pwl[1], top8[teams[0]].pwl[1]);
                    }
                }
                PlayerPrefs.Save();
            }
            else
            {
                if (!winners[i])
                {
                    if (top8[teams[0]].pwl[1] == 4)
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
                            top8[schedule[k]].pwl[1] = 0;
                            top8[schedule[k]].pwl[2] = 0;
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

                        for (int k = 0; k < allTeams.teams.Length; k++)
                        {
                            allTeams.teams[k].pwl[0] = 0;
                            allTeams.teams[k].pwl[1] = 0;
                            allTeams.teams[k].pwl[2] = 0;
                            PlayerPrefs.SetString("PWL" + top8[teams[i]].id.ToString(), "0,0,0");

                            for (int l = 0; l < allTeams.teams[k].players.Count; l++)
                            {
                                int increase;
                                string playerString = "", currStats = "";

                                if (int.Parse(allTeams.teams[k].players[l][6]) <= 0)
                                    allTeams.teams[k].players[l][4] = (int.Parse(allTeams.teams[k].players[l][6]) - (int)(Random.value * 10)).ToString();

                                increase = (int)Mathf.Ceil(int.Parse(allTeams.teams[k].players[l][6]) * 4 / 27 * 27 / 22);
                                allTeams.teams[k].players[l][6] = (int.Parse(allTeams.teams[k].players[l][6]) - increase).ToString();

                                for (int m = 0; m < increase; m++)
                                {
                                    int currStat = (int)(Random.value * (allTeams.numStats - 6) + 6);
                                    allTeams.teams[k].players[l][currStat] = (int.Parse(allTeams.teams[k].players[l][currStat]) + 1).ToString();
                                }

                                for (int m = 0; m < allTeams.teams[k].players[l].Length - 1; m++)
                                    playerString += allTeams.teams[k].players[l][m] + ",";
                                playerString += allTeams.teams[k].players[l][allTeams.teams[k].players[l].Length - 1];

                                PlayerPrefs.SetString("Player" + k + "-" + l, playerString);

                                for (int m = 0; m < allTeams.teams[k].pStats[l].Length - 1; m++)
                                {
                                    allTeams.teams[k].pStats[l][m] = 0;
                                    currStats += "0,";
                                }

                                allTeams.teams[k].pStats[l][allTeams.teams[k].pStats[l].Length - 1] = 0;
                                currStats += "0";

                                PlayerPrefs.SetString("PlayerStats" + k + "-" + l, currStats);
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

        GameObject.Find("txtWL").GetComponent<Text>().text = "W/L: " + allTeams.teams[0].pwl[1] + "/" + allTeams.teams[0].pwl[2];
        GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
    }

    void DetermineMVP()
    {
        double mvpWorth, cyWorth, bestMVP = 0.0, bestCY = 0.0, mvpOPS = 0.0, cyERA = 0.0;
        int[] mvp = new int[2], cy = new int[2];
        for (int i = 0; i < allTeams.GetNumTeams(); i++)
            for(int j = 0; j < allTeams.teams[i].pStats.Count; j++)
            {
                
                if(allTeams.teams[i].players[j][2].Contains("P"))
                {
                    double era = allTeams.teams[i].pStats[j][28] * 27 / (double)allTeams.teams[i].pStats[j][24];
                    cyWorth = (6.0 - era) * 5 + allTeams.teams[i].pStats[j][24] / (double)8;
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
                    double ops = (allTeams.teams[i].pStats[j][3] + allTeams.teams[i].pStats[j][9]) / (double)(allTeams.teams[i].pStats[j][1] + allTeams.teams[i].pStats[j][9] + allTeams.teams[i].pStats[j][13]) + (allTeams.teams[i].pStats[j][3] + allTeams.teams[i].pStats[j][4] * 2 + allTeams.teams[i].pStats[j][5] * 3 + allTeams.teams[i].pStats[j][6] * 4) / (double)allTeams.teams[i].pStats[j][1];
                    mvpWorth = allTeams.teams[i].pStats[j][6] / 40.0 + ops * 25;
                    if (mvpWorth > bestMVP)
                    {
                        bestMVP = mvpWorth;
                        mvp[0] = i;
                        mvp[1] = j;
                        mvpOPS = ops;
                    }
                }
            }
        GameObject.Find("txtMVP").GetComponent<Text>().text = allTeams.teams[mvp[0]].players[mvp[1]][0] + " " + allTeams.teams[mvp[0]].players[mvp[1]][1] + " (" + allTeams.teams[mvp[0]].shortform + ") OPS: " + mvpOPS.ToString("0.000") + " HR: " + allTeams.teams[mvp[0]].pStats[mvp[1]][6];
        GameObject.Find("txtCY").GetComponent<Text>().text = allTeams.teams[cy[0]].players[cy[1]][0] + " " + allTeams.teams[cy[0]].players[cy[1]][1] + " (" + allTeams.teams[cy[0]].shortform + ") ERA: " + cyERA.ToString("0.00") + " SO: " + allTeams.teams[cy[0]].pStats[cy[1]][24];
    }
}
