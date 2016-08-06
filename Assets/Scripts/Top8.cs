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
    int numWinners = 0, round = 1;
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

        teams[0].id = 0;

        for(int i = 0; i < allTeams.teams.Length; i++)
            teams[i].pick = i;

        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = teams[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].teamName;
            schedule[i] = 7 - i;
            top8[i].pwl[1] = 0;
        }
    }

    void Sort(int left, int right)
    {
        int i = left, j = right;
        string pivot = teams[(int)(left + (right - left) / 2)].GetStats()[0];

        while (i <= j)
        {
            while (string.Compare(teams[i].GetStats()[0], pivot) > 0)
            {
                i++;
            }

            while (string.Compare(teams[j].GetStats()[0], pivot) < 0)
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

                if (score1 > score2)
                {
                    top8[thisTeam].pwl[1]++;
                    GameObject.Find("txtWins" + thisTeam).GetComponent<Text>().text = top8[thisTeam].pwl[1].ToString();

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
                    GameObject.Find("txtWins" + otherTeam).GetComponent<Text>().text = top8[otherTeam].pwl[1].ToString();

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
                            text.text = top8[schedule[k]].teamName;
                            GameObject.Find("txtWins" + schedule[k]).GetComponent<Text>().name = "txtWinsWinner";
                            GameObject.Find("txtWins" + round.ToString() + k.ToString()).GetComponent<Text>().name = "txtWins" + schedule[k];
                        }
                    }
                    else
                    {
                        championPanel.SetActive(true);
                        GameObject.Find("txtChampion").GetComponent<Text>().text = top8[newRound[i]].teamName;
                        allTeams.noDraft = true;
                        allTeams.year++;
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

        GameObject.Find("txtWL").GetComponent<Text>().text = "(" + yourWins + "-" + theirWins + ")";
        GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
    }
}
