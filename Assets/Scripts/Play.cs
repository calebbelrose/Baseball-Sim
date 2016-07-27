using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Play : MonoBehaviour {

    int numPlays = 0;
    public void PlayGame()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        int you = 0, them = 0;
        string result = "";
        for (int i = 0; i < allTeams.GetNumTeams() / 2; i++)
        {
            int score1 = 0, score2 = 0, otherTeam, thisTeam;
            thisTeam = allTeams.schedule[i, 0];
            otherTeam = allTeams.schedule[i, 1];
            float goal1 = allTeams.teams[thisTeam].overalls[1] / (allTeams.teams[thisTeam].overalls[1] + allTeams.teams[otherTeam].overalls[2]),
                    goal2 = allTeams.teams[otherTeam].overalls[1] / (allTeams.teams[otherTeam].overalls[1] + allTeams.teams[thisTeam].overalls[2]);
            if(thisTeam == 0)
                //Debug.Log((allTeams.teams[thisTeam].overalls[1] / (allTeams.teams[thisTeam].overalls[1] + allTeams.teams[otherTeam].overalls[2]))/
                //(allTeams.teams[otherTeam].overalls[1] / (allTeams.teams[otherTeam].overalls[1] + allTeams.teams[thisTeam].overalls[2]) + (allTeams.teams[thisTeam].overalls[1] / (allTeams.teams[thisTeam].overalls[1] + allTeams.teams[otherTeam].overalls[2]))));
            for (int k = 0; k < 9; k++)
            {
                float team1 = Random.value, team2 = Random.value;
                if (team1 < goal1)
                    score1++;
                if (team2 < goal2)
                    score2++;
            }

            if (i == 0)
            {
                you = score1;
                them = score2;
                if (score1 > score2)
                {
                    allTeams.teams[thisTeam].pwlt[1]++;
                    allTeams.teams[thisTeam].pwlt[0] += 2;
                    allTeams.teams[otherTeam].pwlt[2]++;
                    result = "Win";
                }
                else if (score2 > score1)
                {
                    allTeams.teams[thisTeam].pwlt[2]++;
                    allTeams.teams[otherTeam].pwlt[1]++;
                    allTeams.teams[otherTeam].pwlt[0] += 2;
                    result = "Loss";
                }
                else
                {
                    allTeams.teams[thisTeam].pwlt[3]++;
                    allTeams.teams[thisTeam].pwlt[0]++;
                    allTeams.teams[otherTeam].pwlt[3]++;
                    allTeams.teams[otherTeam].pwlt[0]++;
                    result = "Tie";
                }
            }
            else
                if (score1 > score2)
            {
                allTeams.teams[thisTeam].pwlt[1]++;
                allTeams.teams[thisTeam].pwlt[0] += 2;
                allTeams.teams[otherTeam].pwlt[2]++;
            }
            else if (score2 > score1)
            {
                allTeams.teams[thisTeam].pwlt[2]++;
                allTeams.teams[otherTeam].pwlt[1]++;
                allTeams.teams[otherTeam].pwlt[0] += 2;
            }
            else
            {
                allTeams.teams[thisTeam].pwlt[3]++;
                allTeams.teams[thisTeam].pwlt[0]++;
                allTeams.teams[otherTeam].pwlt[3]++;
                allTeams.teams[otherTeam].pwlt[0]++;
            }
        }
        GameObject.Find("txtYourScore").GetComponent<Text>().text = "You: " + you;
        Text txtResult = GameObject.Find("txtResult").GetComponent<Text>();
        txtResult.text = result;

        if (result == "Win")
            txtResult.color = Color.green;
        else if (result == "Loss")
            txtResult.color = Color.red;
        else
            txtResult.color = Color.white;

        GameObject.Find("txtTheirScore").GetComponent<Text>().text = "Them: " + them;
        int[,] temp = new int[allTeams.GetNumTeams() / 2, 2];
        System.Array.Copy(allTeams.schedule, 0, temp, 0, allTeams.schedule.Length);
        int numRows = ((temp.Length + 1)/ 2) - 1;
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

        }
        
        numPlays++;
        Debug.Log(numPlays.ToString() + " " + (allTeams.GetNumTeams() - 1).ToString());
        if (numPlays == 1)
        {
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);
        }
    }
}