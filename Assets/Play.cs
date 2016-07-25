using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Play : MonoBehaviour {

    public void PlayGame()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams teams = manager.GetComponent<AllTeams>();
        int you = 0, them = 0;
        string result = "";
        for (int i = 0; i < teams.GetNumTeams(); i += 2)
        {
            int score1 = 0, score2 = 0, otherTeam = teams.nextTeam[i];
            Debug.Log(teams.nextTeam[i]);
            teams.nextTeam[i] += (teams.nextTeam[i] + 1) % teams.GetNumTeams();
            teams.nextTeam[otherTeam] = (teams.nextTeam[otherTeam] + 1) % teams.GetNumTeams();
            float goal1 = teams.overalls[i][1] / (teams.overalls[i][1] + teams.overalls[otherTeam][2]),
                    goal2 = teams.overalls[otherTeam][1] / (teams.overalls[otherTeam][1] + teams.overalls[i][2]);
            for (int j = 0; j < 9; j++)
            {
                float team1 = Random.value, team2 = Random.value;
                if (team1 > goal1)
                    score1++;
                if (team2 > goal2)
                    score2++;
            }
            
            if(i == 0)
            {
                you = score1;
                them = score2;
                if (score1 > score2)
                {
                    teams.wlt[i][0]++;
                    teams.wlt[otherTeam][1]++;
                    result = "Win";
                }
                else if (score2 > score1)
                {
                    teams.wlt[i][1]++;
                    teams.wlt[otherTeam][0]++;
                    result = "Loss";
                }
                else
                {
                    teams.wlt[i][2]++;
                    teams.wlt[otherTeam][2]++;
                    result = "Tie";
                }
            }
            else
                if (score1 > score2)
                {
                    teams.wlt[i][0]++;
                    teams.wlt[otherTeam][1]++;
                }
                else if (score2 > score1)
                {
                    teams.wlt[i][1]++;
                    teams.wlt[otherTeam][0]++;
                }
                else
                {
                    teams.wlt[i][2]++;
                    teams.wlt[otherTeam][2]++;
                }
        }
        GameObject.Find("txtScore").GetComponent<Text>().text = "You: " + you + " " + result + " Them: " + them;

    }
}