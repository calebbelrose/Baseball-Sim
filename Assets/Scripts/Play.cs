using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour
{

    int maxPlays = 29;
    AllTeams allTeams;
    int totalScore = 0;

    public void PlayGame()
    {
        GameObject manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        Text txtResult;
        int you = 0, them = 0, numRows;
        int[,] temp;
        string result = "", strSchedule = "";
        int singles = 0, doubles = 0, triples = 0, homeruns = 0, walks = 0, rbis = 0, sacs = 0, strikeouts = 0;
        float abs = 0.0f;

        for (int i = 0; i < allTeams.GetNumTeams() / 2; i++)
        {
            int inning = 0, points = 1;
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

            staminas[0] = int.Parse(allTeams.teams[teams[0]].players[pitchers[0]][14]);
            staminas[1] = int.Parse(allTeams.teams[teams[1]].players[pitchers[1]][14]);

            Debug.Log(allTeams.teams[teams[0]].Batters.Count);
            Debug.Log(allTeams.teams[teams[1]].Batters.Count);
            for (int q = 0; q < allTeams.teams[teams[0]].Batters.Count; q++)
                Debug.Log(allTeams.teams[teams[0]].players[allTeams.teams[teams[0]].Batters[q]][2]);

            for (int q = 0; q < allTeams.teams[teams[1]].Batters.Count; q++)
                Debug.Log(allTeams.teams[teams[1]].players[allTeams.teams[teams[1]].Batters[q]][2]);

            while (inning < 9 || scores[0] == scores[1])
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
                        pitchers[thisTeam] = allTeams.teams[teams[thisTeam]].CP[0];



                    while (outs < 3)
                    {
                        bool strike, swing;

                        float thisEye, thisContact, thisPower;
                        float eye = float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][10]) / 100;
                        float contact = float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][9]) / 100;

                        bases[0] = true;

                        if (staminas[thisTeam] <= 0 && relievers[thisTeam] <= 3)
                        {
                            pitchers[thisTeam] = allTeams.teams[teams[thisTeam]].RP[relievers[thisTeam]];
                            relievers[thisTeam]++;
                        }

                        if (Random.value < 0.5f)
                            strike = true;
                        else
                            strike = false;

                        if (Random.value > float.Parse(allTeams.teams[teams[otherTeam]].players[pitchers[thisTeam]][13]))
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
                                    homeruns++;
                                    abs++;
                                    numBases = 4;
                                }
                                else
                                {
                                    thisPower *= float.Parse(allTeams.teams[teams[otherTeam]].players[allTeams.teams[teams[otherTeam]].Batters[batters[otherTeam]]][11]) / 100;

                                    if (thisPower > 0.65f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Triple");*/
                                        triples++;
                                        abs++;
                                        numBases = 4;
                                    }
                                    else if (thisPower > 0.55f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Flyout");*/
                                        fly = true;
                                        numBases = 1;
                                        bases[0] = false;
                                        abs++;
                                        outs++;
                                    }
                                    else if (thisPower > 0.45f)
                                    {
                                        numBases = 2;
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Double");*/
                                        doubles++;
                                        abs++;
                                    }
                                    else if (thisPower > 0.35f)
                                    {
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Popout");*/
                                        numBases = 0;
                                        abs++;
                                        outs++;
                                    }
                                    else if (thisPower > 0.2f)
                                    {
                                        numBases = 1;
                                        /*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Single");*/
                                        singles++;
                                        abs++;
                                    }
                                    else if (thisPower > 0.10f)
                                    {
                                        numBases = 0;
                                        outs++;
                                        abs++;
                                        /*if (i == 0)
                                            Debug.Log("Lineout");*/
                                    }
                                    else
                                    {
                                        numBases = 0;
                                        if (bases[1])
                                        {
                                            outs += 2;
                                            bases[1] = false;
                                            /*if (i == 0)
                                                Debug.Log("Double Play");*/
                                        }
                                        else
                                        {
                                            outs++;
                                            abs++;
                                            /*if (i == 0)
                                                Debug.Log("Groundout");*/
                                        }

                                    }
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
                                                rbis++;
                                            }
                                            else
                                                bases[k + numBases] = true;
                                        }
                                    if (fly && advanced)
                                    {
                                        sacs++;
                                        abs--;
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
                            abs++;
                            /*if (i == 0)
                                Debug.Log(strikes + "-" + balls + " Strikeout");*/
                            batters[otherTeam] = (batters[otherTeam] + 1) % 9;
                            strikeouts++;
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
                                rbis++;
                            }
                            else
                                for (int k = currBase; k > 0; k--)
                                {
                                    bases[k] = true;
                                    bases[k - 1] = false;
                                }

                            /*if (i == 0)
                                Debug.Log(strikes + "-" + balls + " Walk");*/

                            walks++;

                            batters[otherTeam] = (batters[otherTeam] + 1) % 9;
                            strikes = 0;
                            balls = 0;
                        }
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

            totalScore += scores[0] + scores[1];
        }

        allTeams.currStarter = (allTeams.currStarter + 1) % 5;
        PlayerPrefs.SetInt("CurrStarter", allTeams.currStarter);

        PlayerPrefs.Save();
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
    }
}