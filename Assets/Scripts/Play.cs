using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour
{

    int maxPlays = 163;
    AllTeams allTeams;

    public void PlayGame()
    {
        GameObject manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        Text txtResult;
        int you = 0, them = 0, numRows;
        int[,] temp;
        string result = "", strSchedule = "";
        int totalInnings = 0, totalScore = 0;

        for (int q = 0; q < 162; q++)
        {
            for (int i = 0; i < allTeams.GetNumTeams() / 2; i++)
            {
                int inning = 1, points = 1;
                int[] scores = new int[2] { 0, 0 };
                int[] batters = new int[2] { 0, 0 };
                int[] relievers = new int[2] { 0, 0 };
                int[] teams = new int[2] { 0, 0 };
                int[] pitchers = new int[2] { 0, 0 };
                int[] staminas = new int[2];

                teams[0] = allTeams.schedule[i, 0];
                teams[1] = allTeams.schedule[i, 1];

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
                            {
                                if (strike)
                                    swing = true;
                                else
                                    swing = false;
                            }
                            else
                            {
                                if (strike)
                                    swing = false;
                                else
                                    swing = true;
                            }

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

                                        if (thisPower > 0.85f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Triple");*/
                                            allTeams.teams[teams[otherTeam]].pStats[batters[otherTeam]][5]++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][26]++;
                                            numBases = 4;
                                        }
                                        else if (thisPower > 0.65f)
                                        {
                                            /*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Flyout");*/
                                            fly = true;
                                            numBases = 1;
                                            bases[0] = false;
                                            outs++;
                                            allTeams.teams[teams[thisTeam]].pStats[pitchers[thisTeam]][24]++;
                                        }
                                        else if (thisPower > 0.6f)
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
                                        else if (thisPower > 0.25f)
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

                if (inning <= 9)
                    points++;

                if (i == 0)
                {
                    you = scores[0];
                    them = scores[1];
                    if (scores[0] > scores[1])
                    {
                        allTeams.teams[teams[0]].pwl[1]++;
                        allTeams.teams[teams[0]].pwl[0] += points;
                        allTeams.teams[teams[1]].pwl[2]++;
                        result = "Win";
                    }
                    else
                    {
                        allTeams.teams[teams[0]].pwl[2]++;
                        allTeams.teams[teams[1]].pwl[1]++;
                        allTeams.teams[teams[1]].pwl[0] += points;
                        result = "Loss";
                    }
                }
                else
                {
                    if (scores[0] > scores[1])
                    {
                        allTeams.teams[teams[0]].pwl[1]++;
                        allTeams.teams[teams[0]].pwl[0] += points;
                        allTeams.teams[teams[1]].pwl[2]++;
                    }
                    else
                    {
                        allTeams.teams[teams[0]].pwl[2]++;
                        allTeams.teams[teams[1]].pwl[1]++;
                        allTeams.teams[teams[1]].pwl[0] += points;
                    }
                }
                PlayerPrefs.SetString("PWL" + allTeams.teams[teams[0]].id.ToString(), allTeams.teams[teams[0]].pwl[0] + "," + allTeams.teams[teams[0]].pwl[1] + "," + allTeams.teams[teams[0]].pwl[2]);
                PlayerPrefs.SetString("PWL" + allTeams.teams[teams[1]].id.ToString(), allTeams.teams[teams[1]].pwl[0] + "," + allTeams.teams[teams[1]].pwl[1] + "," + allTeams.teams[teams[1]].pwl[2]);

                for (int j = 0; j < teams.Length; j++)
                {
                    for (int k = 0; k < allTeams.teams[teams[j]].pStats.Count; k++)
                    {
                        string currStats = "";

                        for (int l = 0; l < allTeams.teams[teams[j]].pStats[k].Length - 1; l++)
                            currStats += allTeams.teams[teams[j]].pStats[k][l] + ",";

                        currStats += allTeams.teams[teams[j]].pStats[k][allTeams.teams[teams[j]].pStats[k].Length - 1];

                        PlayerPrefs.SetString("PlayerStats" + teams[j] + "-" + k, currStats);
                    }
                }
                totalInnings += inning;
                totalScore += scores[0] + scores[1];
            }

            allTeams.currStarter = (allTeams.currStarter + 1) % 5;
            PlayerPrefs.SetInt("CurrStarter", allTeams.currStarter);

            GameObject.Find("txtYourScore").GetComponent<Text>().text = "You: " + you;
            txtResult = GameObject.Find("txtResult").GetComponent<Text>();
            txtResult.text = result;

            if (result == "Win")
                txtResult.color = Color.green;
            else if (result == "Loss")
                txtResult.color = Color.red;
            else
                txtResult.color = Color.white;

            GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
            double bottom = allTeams.teams[0].pwl[1] + allTeams.teams[0].pwl[2];
            double ratio = System.Math.Round(allTeams.teams[0].pwl[1] / bottom, 3);
            GameObject.Find("txtWL").GetComponent<Text>().text = "W/L: " + allTeams.teams[0].pwl[1] + "/" + allTeams.teams[0].pwl[2] + " (" + ratio + ")";
            temp = new int[allTeams.GetNumTeams() / 2, 2];
            System.Array.Copy(allTeams.schedule, 0, temp, 0, allTeams.schedule.Length);
            numRows = ((temp.Length + 1) / 2) - 1;

            for (int i = 0; i < allTeams.GetNumTeams(); i++)
            {
                int x = i % 2, y = i / 2;
                if (y == 0)
                {
                    if (x == 1)
                        allTeams.schedule[0, 1] = temp[1, 0];
                }
                else if (x == 1)
                    allTeams.schedule[y, 1] = temp[y - 1, 1];
                else if (y < numRows)
                    allTeams.schedule[y, 0] = temp[y + 1, 0];
                else
                    allTeams.schedule[y, 0] = temp[y, 1];
                strSchedule += allTeams.schedule[y, x] + ",";
            }

            strSchedule = strSchedule.Remove(strSchedule.Length - 1, 1);
        }
        PlayerPrefs.SetString("Schedule", strSchedule);
        allTeams.numPlays++;
        /*float numGames = (allTeams.numPlays + allTeams.GetNumTeams()) * 1000;

        System.IO.StreamWriter file =
            new System.IO.StreamWriter("test.csv");
        file.Write(abs/numGames + "," + totalScore/numGames + "," + singles / numGames + "," + doubles / numGames + "," + triples / numGames + "," + homeruns / numGames + "," + (singles + doubles * 2 + triples * 3 + homeruns * 4) / numGames + "," + rbis / numGames + "," + (singles+doubles+triples+homeruns) / abs + "," + (singles + doubles+ triples+homeruns + walks) / (abs + walks + sacs));
        file.Close();
        Debug.Log(totalScore / numGames);*/

        if (allTeams.numPlays >= maxPlays)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);

        PlayerPrefs.SetInt("NumPlays", allTeams.numPlays);
        PlayerPrefs.Save();
        Debug.Log((double)totalInnings / 162 / 15 + " " + (double)totalScore / 162 / 30);
    }
}