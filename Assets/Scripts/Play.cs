using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Play : MonoBehaviour {

    int numPlays, maxPlays = 29;

    void Start()
    {
        if (!int.TryParse(PlayerPrefs.GetString("NumPlays"), out numPlays))
            numPlays = 0;
    }

    void Awake()
    {
        if (numPlays == maxPlays)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);
    }

    public void PlayGame()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        Text txtResult;
        int you = 0, them = 0, points = 1, numRows;
        int[,] temp;
        string result = "", strSchedule = "";

        for (int i = 0; i < allTeams.GetNumTeams() / 2; i++)
        {
            int score1 = 0, score2 = 0, otherTeam, thisTeam, j = 0;
            thisTeam = allTeams.schedule[i, 0];
            otherTeam = allTeams.schedule[i, 1];
            float goal1, goal2;

            goal1 = allTeams.teams[thisTeam].overalls[1] / (allTeams.teams[thisTeam].overalls[1] + allTeams.teams[otherTeam].overalls[2]);
            goal2 = allTeams.teams[otherTeam].overalls[1] / (allTeams.teams[thisTeam].overalls[2] + allTeams.teams[otherTeam].overalls[1]);

            while (j < 9 || score1 == score2)
            {
                float team1 = Random.value, team2 = Random.value;
                if (team1 < goal1)
                    score1++;
                if (team2 < goal2)
                    score2++;
                j++;
            }

            if (j > 9)
                points++;

            if (i == 0)
            {
                you = score1;
                them = score2;
                if (score1 > score2)
                {
                    allTeams.teams[thisTeam].pwl[1]++;
                    allTeams.teams[thisTeam].pwl[0] += points;
                    allTeams.teams[otherTeam].pwl[2]++;
                    result = "Win";
                }
                else
                {
                    allTeams.teams[thisTeam].pwl[2]++;
                    allTeams.teams[otherTeam].pwl[1]++;
                    allTeams.teams[otherTeam].pwl[0] += points;
                    result = "Loss";
                }
            }
            else
            {
                if (score1 > score2)
                {
                    allTeams.teams[thisTeam].pwl[1]++;
                    allTeams.teams[thisTeam].pwl[0] += 2;
                    allTeams.teams[otherTeam].pwl[2]++;
                }
                else
                {
                    allTeams.teams[thisTeam].pwl[2]++;
                    allTeams.teams[otherTeam].pwl[1]++;
                    allTeams.teams[otherTeam].pwl[0] += 2;
                }
            }
            PlayerPrefs.SetString("PWL" + allTeams.teams[thisTeam].id.ToString(), allTeams.teams[thisTeam].pwl[0] + "," + allTeams.teams[thisTeam].pwl[1] + "," + allTeams.teams[thisTeam].pwl[2]);
            PlayerPrefs.SetString("PWL" + allTeams.teams[otherTeam].id.ToString(), allTeams.teams[otherTeam].pwl[0] + "," + allTeams.teams[otherTeam].pwl[1] + "," + allTeams.teams[otherTeam].pwl[2]);
        }

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
        temp = new int[allTeams.GetNumTeams() / 2, 2];
        System.Array.Copy(allTeams.schedule, 0, temp, 0, allTeams.schedule.Length);
        numRows = ((temp.Length + 1)/ 2) - 1;

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
            else if(y < numRows)
                allTeams.schedule[y, 0] = temp[y + 1, 0];
            else
                allTeams.schedule[y, 0] = temp[y, 1];
            strSchedule += allTeams.schedule[y, x] + ",";
        }

        strSchedule = strSchedule.Remove(strSchedule.Length - 1, 1);
        PlayerPrefs.SetString("Schedule", strSchedule);
        numPlays++;

        if (numPlays == maxPlays)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);
    }
}