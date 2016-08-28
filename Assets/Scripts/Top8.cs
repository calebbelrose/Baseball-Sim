using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Top8 : MonoBehaviour {

    Team[] top8 = new Team[8];
    Team[] teams;
    int[] schedule = new int[8];
    int[] newRound = new int[4];
    bool[] winners = new bool[4];
    int numWinners = 0, round;
    GameObject championPanel;
    AllTeams allTeams;

    void Start()
    {
        allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        teams = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(teams, 0);
        Sort(0, allTeams.GetNumTeams() - 1);
        championPanel = GameObject.Find("pnlChampion");
        championPanel.SetActive(false);
        allTeams.inFinals = true;
        PlayerPrefs.SetString("InFinals", allTeams.inFinals.ToString());
        PlayerPrefs.Save();

        if (PlayerPrefs.HasKey("Finals Round"))
        {
            round = PlayerPrefs.GetInt("Finals Round");
            int numTeams = top8.Length;
            for (int i = 0; i < round; i++)
            {
                for (int j = 0; j < numTeams; j++)
                    GameObject.Find("txtWins" + round.ToString() + numTeams).GetComponent<Text>().text = PlayerPrefs.GetInt("txtWins" + round.ToString() + numTeams).ToString();
                numTeams /= 2;
            }
        }
        else
            round = 1;

        for (int i = 0; i < allTeams.teams.Length; i++)
            teams[i].pick = i;

        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = teams[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].cityName + " " + top8[i].teamName;
            schedule[i] = 7 - i;
            top8[i].pwl[1] = 0;
        }
    }

    void Sort(int left, int right)
    {
        int i = left, j = right;
        string pivot = teams[(int)(left + (right - left) / 2)].pwl[0].ToString();

        while (i <= j)
        {
            while (string.Compare(teams[i].pwl[0].ToString(), pivot) > 0)
            {
                i++;
            }

            while (string.Compare(teams[j].pwl[0].ToString(), pivot) < 0)
            {
                j--;
            }

            if (i <= j)
            {
                Team temp = new Team();

                temp = teams[i];
                teams[i] = teams[j];
                teams[j] = temp;

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

        for (int i = 0; i < newRound.Length; i++)
        {
            int otherTeam, thisTeam, j = 0;
            otherTeam = schedule[i];
            thisTeam = schedule[schedule.Length - i - 1];

            if (top8[thisTeam].pwl[1] < 4 && top8[otherTeam].pwl[1] < 4)
            {
                int score1 = 0, score2 = 0, you = 0, them = 0; ;
                float goal1, goal2;

                goal1 = top8[thisTeam].overalls[1] / (top8[thisTeam].overalls[1] + top8[otherTeam].overalls[2]);
                goal2 = top8[otherTeam].overalls[1] / (top8[thisTeam].overalls[2] + top8[otherTeam].overalls[1]);

                while (j < 9 || score1 == score2)
                {
                    float team1 = Random.value, team2 = Random.value;
                    if (team1 < goal1)
                        score1++;
                    if (team2 < goal2)
                        score2++;
                    j++;
                }

                Debug.Log(thisTeam + " " + top8[thisTeam].teamName + " " + otherTeam + " " + top8[otherTeam].teamName);

                if (score1 > score2)
                {
                    top8[thisTeam].pwl[1]++;
                    GameObject.Find("txtWins" + round.ToString() + thisTeam).GetComponent<Text>().text = top8[thisTeam].pwl[1].ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + otherTeam, top8[thisTeam].pwl[1]);

                    if (top8[thisTeam].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = score1;
                        them = score2;
                        DisplayResult(result, you, them, top8[thisTeam].pwl[1], top8[otherTeam].pwl[1]);
                    }
                    else if (top8[otherTeam].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = score2;
                        them = score1;
                        DisplayResult(result, you, them, top8[otherTeam].pwl[1], top8[thisTeam].pwl[1]);
                    }
                }
                else if (score2 > score1)
                {
                    top8[otherTeam].pwl[1]++;
                    GameObject.Find("txtWins" + round.ToString() + otherTeam).GetComponent<Text>().text = top8[otherTeam].pwl[1].ToString();
                    PlayerPrefs.SetInt("txtWins" + round.ToString() + otherTeam, top8[otherTeam].pwl[1]);

                    if (top8[thisTeam].id == 0)
                    {
                        result = "Loss";
                        player = true;
                        you = score1;
                        them = score2;
                        DisplayResult(result, you, them, top8[thisTeam].pwl[1], top8[otherTeam].pwl[1]);
                    }
                    else if (top8[otherTeam].id == 0)
                    {
                        result = "Win";
                        player = true;
                        you = score2;
                        them = score1;
                        DisplayResult(result, you, them, top8[otherTeam].pwl[1], top8[thisTeam].pwl[1]);
                    }
                }
                PlayerPrefs.Save();
            }
            else
            {
                if (!winners[i])
                {
                    if (top8[thisTeam].pwl[1] == 4)
                        newRound[i] = thisTeam;
                    else
                        newRound[i] = otherTeam;
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
                        GameObject.Find("txtChampion").GetComponent<Text>().text = top8[newRound[i]].teamName;
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
                            PlayerPrefs.SetString("PWL" + teams[i].id.ToString(), "0,0,0");

                            for(int l = 0; l < allTeams.teams[k].players.Count; l++)
                            {
                                int increase;
                                string playerString ="";

                                if (int.Parse(allTeams.teams[k].players[l][4]) <= 0)
                                    allTeams.teams[k].players[l][4] = (int.Parse(allTeams.teams[k].players[l][4]) - (int)(Random.value * 10)).ToString();

                                increase = (int)Mathf.Ceil(int.Parse(allTeams.teams[k].players[l][4]) * 4 / 27 * 27 / 22);                                    
                                allTeams.teams[k].players[l][4] = (int.Parse(allTeams.teams[k].players[l][4]) - increase).ToString();

                                for (int m = 0; m < increase; m++)
                                {
                                    int currStat = (int)(Random.value * (allTeams.numStats - 6) + 6);
                                    allTeams.teams[k].players[l][currStat] = (int.Parse(allTeams.teams[k].players[l][currStat]) + 1).ToString();
                                }

                                for (int m = 0; m < allTeams.teams[k].players[l].Length - 1; m++)
                                    playerString += allTeams.teams[k].players[l][m] + ",";
                                playerString += allTeams.teams[k].players[l][allTeams.teams[k].players[l].Length - 1];

                                PlayerPrefs.SetString("Player" + k + "-" + l, playerString);
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
}
