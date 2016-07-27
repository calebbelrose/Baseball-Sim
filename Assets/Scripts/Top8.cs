using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Top8 : MonoBehaviour {

    Team[] top8 = new Team[8];
    Team[] teams;

    void Start()
    {
        AllTeams allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        teams = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(teams, 0);
        Sort(0, allTeams.GetNumTeams() - 1);
        for (int i = 0; i < top8.Length; i++)
        {
            top8[i] = teams[i];
            GameObject.Find("txtTeam" + i).GetComponent<Text>().text = top8[i].teamName;
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
}
