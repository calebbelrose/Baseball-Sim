using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetStandings : MonoBehaviour {

    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    string[] headers = new string[] { "Team Name", "Points", "Wins", "Losses" };
    int currSortedStat = 3;
    char order = 'd';
    int longestTeamName = 0;
    Team[] teams;

    void Start()
    {
        teamList = GameObject.Find("TeamList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        for (int i = 0; i < allTeams.teams.Length; i++)
        {
            if (allTeams.teams[i].teamName.Length > longestTeamName)
                longestTeamName = allTeams.teams[i].teamName.Length;
        }
        teams = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(teams, 0);
        DisplayHeader();
        DisplayTeams();
    }

    void DisplayHeader()
    {
        int standingsHeaderLength = longestTeamName;
        GameObject teamListHeader = GameObject.Find("StandingsHeader");

        for (int i = 1; i < headers.Length; i++)
        {
            standingsHeaderLength += headers[i].Length + 1;
        }

        int[] headerLengths = new int[headers.Length];
        headerLengths[0] = longestTeamName + 1;

        for (int i = 1; i < headers.Length; i++)
            headerLengths[i] = headers[i].Length + 1;

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = -10.0f, newWidth = 0.0f;
        float totalWidth = (8.04f * (standingsHeaderLength));
        teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (teams.Length) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        totalWidth /= -2.0f;

        for (int i = 0; i < headers.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(teamListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = headers[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader.name));

            float currWidth = (8.04f * (headerLengths[i]));
            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        teamList.GetComponent<RectTransform>().offsetMax = new Vector2(newWidth - 160.0f, 0);
    }

    public void DisplayTeams()
    {
        GameObject[] currTeams = GameObject.FindGameObjectsWithTag("Team");

        for (int i = 0; i < currTeams.Length; i++)
            Destroy(currTeams[i]);

        for (int i = 0; i < teams.Length; i++)
        {
            Object teamButton = Resources.Load("Team", typeof(GameObject));
            GameObject newTeam = Instantiate(teamButton) as GameObject;
            newTeam.name = "team" + i.ToString();
            newTeam.transform.SetParent(teamList.transform);
            teams[i].SetStats();
            string teamListing = teams[i].teamName;

            for (int j = teams[i].teamName.Length; j < longestTeamName; j++)
                teamListing += " ";

            for (int j = 1; j < headers.Length - 1; j++)
            {
                teamListing += " " + teams[i].GetStats()[j];

                for (int k = teams[i].GetStats()[j].Length; k < headers[j].Length; k++)
                    teamListing += " ";
            }

            teamListing += " " + teams[i].GetStats()[headers.Length - 1];

            newTeam.transform.GetChild(0).gameObject.GetComponent<Text>().text = teamListing;
            newTeam.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newTeam.GetComponent<Button>().interactable = false;
            if (teams[i].teamName == allTeams.teams[0].teamName)
            {
                Button b = newTeam.GetComponent<Button>();
                ColorBlock c = b.colors;
                c.disabledColor = new Color(1.0f, 1.0f, 0.0f);
                b.colors = c;
            }
        }
    }

    public void StartSorting(string name)
    {
        int left = 0, right = teams.Length - 1, statNum = int.Parse(name.Remove(0, 6));
        string pivot = teams[(int)(left + (right - left) / 2)].GetStats()[statNum];
        int test;
        bool notString = int.TryParse(pivot, out test);

        if (currSortedStat == statNum)
            if (order == 'a')
                order = 'd';
            else
                order = 'a';
        else
            if (notString)
            order = 'd';
        else
            order = 'a';
        Sort(statNum, left, right);
        currSortedStat = statNum;
        DisplayTeams();
    }

    void Sort(int statNum, int left, int right)
    {
        int i = left, j = right;
        string pivot = teams[(int)(left + (right - left) / 2)].GetStats()[statNum];

        if (order == 'a')
            while (i <= j)
            {
                while (string.Compare(teams[i].GetStats()[statNum], pivot) < 0)
                {
                    i++;
                }

                while (string.Compare(teams[j].GetStats()[statNum], pivot) > 0)
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
        else
            while (i <= j)
            {
                while (string.Compare(teams[i].GetStats()[statNum], pivot) > 0)
                {
                    i++;
                }

                while (string.Compare(teams[j].GetStats()[statNum], pivot) < 0)
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
            Sort(statNum, left, j);
        }

        if (i < right)
        {
            Sort(statNum, i, right);
        }
    }
}
