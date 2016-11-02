using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetStats : MonoBehaviour {

    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    string[] headers = new string[] { "G", "AB", "R", "H", "2B", "3B", "HR", "RBI", "BB", "SO", "SB", "CS", "AVG", "OBP", "SLG", "OPS", "W", "L", "ERA", "GS", "SV", "SVO", "IP", "H", "R", "ER", "HR", "BB", "SO", "AVG", "WHIP" };
    string[] playerInfoHeaders = new string[] { "First Name", "Last Name", "Position", "Team" };
    int currSortedStat = 10;
    char order = 'd';
    Team[] teams;
    int[] playerInfoLengths, headerLengths;
    List<string[]> tempStats = new List<string[]>();

    void Start()
    {
        teamList = GameObject.Find("TeamList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        playerInfoLengths = new int[playerInfoHeaders.Length];
        headerLengths = new int[headers.Length];
        

        for (int i = 0; i < playerInfoLengths.Length; i++)
            playerInfoLengths[i] = playerInfoHeaders[i].Length;

        for (int i = 0; i < headerLengths.Length; i++)
            headerLengths[i] = headers[i].Length;

        for (int i = 0; i < allTeams.teams.Length; i++)
        {
            if (allTeams.teams[i].shortform.Length > playerInfoLengths[playerInfoLengths.Length - 1])
                playerInfoLengths[playerInfoLengths.Length - 1] = allTeams.teams[i].shortform.Length;

            for (int j = 0; j < allTeams.teams[i].players.Count; j++)
            {
                for(int k = 0; k < playerInfoLengths.Length - 1; k++)
                    if (allTeams.teams[i].players[j][k].Length > playerInfoLengths[k])
                        playerInfoLengths[k] = allTeams.teams[i].players[j][k].Length;

                for (int k = 0; k < headers.Length; k++)
                    if (allTeams.teams[i].pStats[j][k].ToString().Length > headerLengths[k])
                        headerLengths[k] = allTeams.teams[i].pStats[j][k].ToString().Length;
            }
        }

        for (int i = 0; i < playerInfoLengths.Length; i++)
            playerInfoLengths[i]++;

        for (int i = 0; i < headerLengths.Length; i++)
            headerLengths[i]++;

        teams = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(teams, 0);

        for (int i = 0; i < teams.Length; i++)
            for (int j = 0; j < teams[i].pStats.Count; j++)
            {
                string[] copy = new string[playerInfoHeaders.Length + teams[i].pStats[j].Length];
                int currStat = 0;

                for (int k = 0; k < playerInfoHeaders.Length - 1; k++)
                    copy[currStat++] = teams[i].players[j][k];

                copy[currStat++] = teams[i].shortform;

                for (int k = 0; k < headers.Length; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                tempStats.Add(copy);
            }

        IntSort(10, 0, tempStats.Count - 1);
        DisplayHeader();
        DisplayTeams();
    }

    void DisplayHeader()
    {
        int standingsHeaderLength = -1;
        GameObject teamListHeader = GameObject.Find("StandingsHeader");
        int totalPlayers = 0;

        for (int i = 0; i < playerInfoLengths.Length; i++)
            standingsHeaderLength += playerInfoLengths[i];

        for (int i = 0; i < headerLengths.Length; i++)
            standingsHeaderLength += headerLengths[i];

        for (int i = 0; i < teams.Length; i++)
            totalPlayers += teams[i].players.Count;

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = -10.0f, newWidth = 0.0f;
        float totalWidth = (8.04f * (standingsHeaderLength));
        teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (totalPlayers) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        totalWidth /= -2.0f;

        for (int i = 0; i < playerInfoHeaders.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(teamListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerInfoHeaders[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader.name));

            float currWidth = (8.04f * (playerInfoLengths[i]));
            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        for (int i = 0; i < headers.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + (i + playerInfoHeaders.Length).ToString();
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
        GameObject[] currTeams = GameObject.FindGameObjectsWithTag("Player");
        Object teamButton = Resources.Load("Player", typeof(GameObject));

        for (int i = 0; i < currTeams.Length; i++)
            Destroy(currTeams[i]);

        for (int i = 0; i < tempStats.Count; i++)
        {
            string playerListing = "";
            GameObject newTeam = Instantiate(teamButton) as GameObject;

            newTeam.name = "player" + i.ToString();
            newTeam.transform.SetParent(teamList.transform);

            for (int j = 0; j < playerInfoHeaders.Length; j++)
            {
                playerListing += tempStats[i][j];

                for (int l = tempStats[i][j].Length; l < playerInfoLengths[j]; l++)
                    playerListing += " ";
            }

            for (int j = 0; j < headers.Length - 1; j++)
            {
                int temp = j + playerInfoHeaders.Length;

                playerListing += tempStats[i][temp];

                for (int l = tempStats[i][temp].Length; l < headerLengths[j]; l++)
                    playerListing += " ";
            }

            playerListing += tempStats[i][tempStats[i].Length - 1];

            newTeam.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newTeam.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newTeam.GetComponent<Button>().interactable = false;
            if (tempStats[i][playerInfoHeaders.Length - 1] == allTeams.teams[0].shortform)
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
        int left = 0, right = tempStats.Count- 1, statNum = int.Parse(name.Remove(0, 6));
        string pivot = tempStats[(int)(left + (right - left) / 2)][statNum];
        int test;
        bool isInt = int.TryParse(pivot, out test);

        if (currSortedStat == statNum)
        {
            if (order == 'a')
                order = 'd';
            else
                order = 'a';
        }
        else
        {
            if (isInt)
                order = 'd';
            else
                order = 'a';
        }

        if (isInt)
            IntSort(statNum, left, right);
        else
            Sort(statNum, left, right);

        currSortedStat = statNum;
        DisplayTeams();
    }

    void Sort(int statNum, int left, int right)
    {
        int i = left, j = right;
        string pivot = tempStats[(int)(left + (right - left) / 2)][statNum];

        if (order == 'a')
            while (i <= j)
            {
                while (string.Compare(tempStats[i][statNum], pivot) < 0)
                {
                    i++;
                }

                while (string.Compare(tempStats[j][statNum], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp;

                    temp = tempStats[i];
                    tempStats[i] = tempStats[j];
                    tempStats[j] = temp;

                    i++;
                    j--;
                }
            }
        else
            while (i <= j)
            {
                while (string.Compare(tempStats[i][statNum], pivot) > 0)
                {
                    i++;
                }

                while (string.Compare(tempStats[j][statNum], pivot) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp;

                    temp = tempStats[i];
                    tempStats[i] = tempStats[j];
                    tempStats[j] = temp;

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

    void IntSort(int statNum, int left, int right)
    {
        int i = left, j = right;
        int pivot = int.Parse(tempStats[(int)(left + (right - left) / 2)][statNum]);

        if (order == 'a')
            while (i <= j)
            {
                while (int.Parse(tempStats[i][statNum]) < pivot)
                    i++;

                while (int.Parse(tempStats[j][statNum]) > pivot)
                    j--;

                if (i <= j)
                {
                    string[] temp;

                    temp = tempStats[i];
                    tempStats[i] = tempStats[j];
                    tempStats[j] = temp;

                    i++;
                    j--;
                }
            }
        else
            while (i <= j)
            {
                while (int.Parse(tempStats[i][statNum]) > pivot)
                    i++;

                while (int.Parse(tempStats[j][statNum]) < pivot)
                    j--;

                if (i <= j)
                {
                    string[] temp;

                    temp = tempStats[i];
                    tempStats[i] = tempStats[j];
                    tempStats[j] = temp;

                    i++;
                    j--;
                }
            }

        if (left < j)
        {
            IntSort(statNum, left, j);
        }

        if (i < right)
        {
            IntSort(statNum, i, right);
        }
    }
}
