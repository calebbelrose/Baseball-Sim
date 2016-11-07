using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetStats : MonoBehaviour {

    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    string[] headers = new string[] { "G", "AB", "R", "H", "2B", "3B", "HR", "TB", "RBI", "BB", "SO", "SB", "CS", "SAC", "AVG", "OBP", "SLG", "OPS", "W", "L", "ERA", "GS", "SV", "SVO", "IP", "AB", "H", "R", "ER", "HR", "BB", "SO", "AVG", "WHIP" };
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

        teams = new Team[allTeams.GetNumTeams()];
        allTeams.teams.CopyTo(teams, 0);

        for (int i = 0; i < teams.Length; i++)
            for (int j = 0; j < teams[i].pStats.Count; j++)
            {
                string[] copy = new string[playerInfoHeaders.Length + teams[i].pStats[j].Length];
                int currStat = 0, temp, tb;
                double obp, slug;

                for (int k = 0; k < playerInfoHeaders.Length - 1; k++)
                    copy[currStat++] = teams[i].players[j][k];

                copy[currStat++] = teams[i].shortform;

                for (int k = 0; k < 7; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                tb = teams[i].pStats[j][3] + teams[i].pStats[j][4] * 2 + teams[i].pStats[j][5] * 3 + teams[i].pStats[j][6] * 4;
                copy[currStat++] = tb.ToString();

                for (int k = 8; k < 14; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                if(teams[i].pStats[j][1] != 0)
                    copy[currStat++] = (teams[i].pStats[j][3] / (double)teams[i].pStats[j][1]).ToString("0.000");
                else 
                    copy[currStat++] = (teams[i].pStats[j][3] / (double)1).ToString("0.000");

                temp = (teams[i].pStats[j][1] + teams[i].pStats[j][9] + teams[i].pStats[j][13]);
                if (temp != 0)
                    obp = (teams[i].pStats[j][3] + teams[i].pStats[j][9]) / (double)temp;
                else
                    obp = (teams[i].pStats[j][3] + teams[i].pStats[j][9]) / (double)1;
                copy[currStat++] = obp.ToString("0.000");

                if(teams[i].pStats[j][1] != 0)
                    slug = tb / (double)teams[i].pStats[j][1];
                else
                    slug = tb / (double)1;
                copy[currStat++] = (slug).ToString("0.000");

                copy[currStat++] = (obp + slug).ToString("0.000");

                for (int k = 18; k < 20; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                if (teams[i].pStats[j][24] != 0)
                    copy[currStat++] = (teams[i].pStats[j][28] * 27 / (double)teams[i].pStats[j][24]).ToString("0.00");
                else
                    copy[currStat++] = (teams[i].pStats[j][28] * 27 / (double)1 / 3).ToString("0.00");

                for (int k = 21; k < 24; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                if (teams[i].pStats[j][24] != 0)
                    copy[currStat++] = (teams[i].pStats[j][24] / 3).ToString() + "." + (teams[i].pStats[j][24] % 3).ToString();
                else
                    copy[currStat++] = "0.0";

                for (int k = 25; k < 32; k++)
                    copy[currStat++] = teams[i].pStats[j][k].ToString();

                if (teams[i].pStats[j][24] != 0)
                    copy[currStat++] = (teams[i].pStats[j][26]  / (double)teams[i].pStats[j][25]).ToString("0.000");
                else
                    copy[currStat++] = (teams[i].pStats[j][26] / (double)1).ToString("0.000");

                if (teams[i].pStats[j][24] != 0)
                    copy[currStat++] = ((teams[i].pStats[j][26] + teams[i].pStats[j][29]) / (double)teams[i].pStats[j][24] / 3).ToString("0.000");
                else
                    copy[currStat++] = ((teams[i].pStats[j][26] + teams[i].pStats[j][29]) / (double)1 / 3).ToString("0.000");

                tempStats.Add(copy);
            }

        for (int i = 0; i < tempStats.Count; i++)
        {
            for (int j = 0; j < playerInfoLengths.Length - 1; j++)
                if (tempStats[i][j].Length > playerInfoLengths[j])
                    playerInfoLengths[j] = tempStats[i][j].Length;

            for (int j = 0; j < headers.Length; j++)
                if (tempStats[i][j + playerInfoLengths.Length].Length > headerLengths[j])
                    headerLengths[j] = tempStats[i][j + playerInfoLengths.Length].Length;
        }

        for (int i = 0; i < allTeams.teams.Length; i++)
            if (allTeams.teams[i].shortform.Length > playerInfoLengths[playerInfoLengths.Length - 1])
                playerInfoLengths[playerInfoLengths.Length - 1] = allTeams.teams[i].shortform.Length;

        for (int i = 0; i < playerInfoLengths.Length; i++)
            playerInfoLengths[i]++;

        for (int i = 0; i < headerLengths.Length; i++)
            headerLengths[i]++;

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
        int left = 0, right = tempStats.Count - 1, statNum = int.Parse(name.Remove(0, 6));
        bool isInt = false, isFloat = false;
        int[] intStats = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 22, 23, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35 };
        isInt = intStats.Contains(statNum);

        if (!isInt)
        {
            int[] floatStats = { 18, 19, 20, 21, 24, 28, 35, 36 };
            isFloat = floatStats.Contains(statNum);
        }

        if (currSortedStat == statNum)
        {
            if (order == 'a')
                order = 'd';
            else
                order = 'a';
        }
        else
        {
            if (isInt || isFloat)
                order = 'd';
            else
                order = 'a';
        }

        if (isInt)
            IntSort(statNum, left, right);
        else if (isFloat)
            FloatSort(statNum, left, right);
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

    void FloatSort(int statNum, int left, int right)
    {
        int i = left, j = right;
        float pivot = float.Parse(tempStats[(int)(left + (right - left) / 2)][statNum]);

        if (order == 'a')
            while (i <= j)
            {
                while (float.Parse(tempStats[i][statNum]) < pivot)
                    i++;

                while (float.Parse(tempStats[j][statNum]) > pivot)
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
                while (float.Parse(tempStats[i][statNum]) > pivot)
                    i++;

                while (float.Parse(tempStats[j][statNum]) < pivot)
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
            FloatSort(statNum, left, j);
        }

        if (i < right)
        {
            FloatSort(statNum, i, right);
        }
    }
}
