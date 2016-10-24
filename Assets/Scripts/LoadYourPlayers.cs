using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadYourPlayers : MonoBehaviour
{
    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    string[] stats;
    int currSortedStat = 3;
    char order = 'd';
    Trade trade;

    void Start()
    {
        teamList = GameObject.Find("YourList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        stats = manager.GetComponent<Stats>().statList;
        trade = GameObject.Find("btnOffer").GetComponent<Trade>();
        DisplayHeader();
        DisplayPlayers();
    }

    void DisplayHeader()
    {
        int statHeaderLength = 0;
        GameObject teamListHeader = GameObject.Find("YourListHeader");

        for (int i = 0; i < stats.Length; i++)
        {
            statHeaderLength += stats[i].Length + 1;
        }

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = 5.0f, newWidth = 0.0f;
        float totalWidth = (8.04f * (statHeaderLength + 1.0f));
        teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (allTeams.teams[0].players.Count + 1) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        teamList.GetComponent<RectTransform>().offsetMax = new Vector2(totalWidth - 160.0f, 0);
        totalWidth /= -2.0f;

        for (int i = 0; i < stats.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(teamListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = stats[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader));

            float currWidth = (8.04f * (stats[i].Length + 1));
            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        teamList.GetComponent<RectTransform>().offsetMax = new Vector2(newWidth - 160.0f, 0);
    }

    public void DisplayPlayers()
    {
        GameObject[] currPlayers = GameObject.FindGameObjectsWithTag("YourPlayer");
        int longestFirstName = 10, longestLastName = 9;
        for (int i = 0; i < currPlayers.Length; i++)
            Destroy(currPlayers[i]);

        for (int i = 0; i < allTeams.teams[0].players.Count; i++)
        {
            if (allTeams.teams[0].players[i][0].Length > longestFirstName)
                longestFirstName = allTeams.teams[0].players[i][0].Length;

            if (allTeams.teams[0].players[i][1].Length > longestLastName)
                longestLastName = allTeams.teams[0].players[i][1].Length;
        }

        for (int i = 0; i < allTeams.teams[0].players.Count; i++)
        {
            Object playerButton = Resources.Load("YourPlayer", typeof(GameObject));
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
            newPlayer.name = "player" + i.ToString();
            newPlayer.transform.SetParent(teamList.transform);
            string allTeamsing = allTeams.teams[0].players[i][0];

            for (int j = allTeams.teams[0].players[i][0].Length; j < longestFirstName; j++)
                allTeamsing += " ";

            allTeamsing += " " + allTeams.teams[0].players[i][1];

            for (int j = allTeams.teams[0].players[i][1].Length; j < longestLastName; j++)
                allTeamsing += " ";

            for (int j = 2; j < stats.Length - 1; j++)
            {
                allTeamsing += " " + allTeams.teams[0].players[i][j];

                for (int k = allTeams.teams[0].players[i][j].Length; k < stats[j].Length; k++)
                    allTeamsing += " ";
            }

            allTeamsing += " " + allTeams.teams[0].players[i][stats.Length - 1];
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = allTeamsing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            TradePlayerInfo tradeInfo = newPlayer.GetComponent<TradePlayerInfo>();
            tradeInfo.teamNum = 0;
            tradeInfo.playerNum = i;
            if (trade.yourTrades.Contains(i))
                tradeInfo.ChangeButtonColour();
        }
    }

    public void StartSorting(GameObject other)
    {
        int statNum = int.Parse(other.name.Remove(0, 6));
        int left = 0, right = allTeams.teams[0].players.Count - 1;
        string pivot = allTeams.teams[0].players[(int)(left + (right - left) / 2)][statNum];
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
        DisplayPlayers();
    }

    void Sort(int statNum, int left, int right)
    {
        int i = left, j = right;
        string pivot = allTeams.teams[0].players[(int)(left + (right - left) / 2)][statNum];

        if (order == 'a')
            while (i <= j)
            {
                while (string.Compare(allTeams.teams[0].players[i][statNum], pivot) < 0)
                {
                    i++;
                }

                while (string.Compare(allTeams.teams[0].players[j][statNum], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    int indexI = trade.yourTrades.IndexOf(i), indexJ = trade.yourTrades.IndexOf(j);
                    string[] temp = new string[stats.Length];

                    for (int k = 0; k < stats.Length; k++)
                        temp[k] = allTeams.teams[0].players[i][k];

                    for (int k = 0; k < stats.Length; k++)
                        allTeams.teams[0].players[i][k] = allTeams.teams[0].players[j][k];

                    for (int k = 0; k < stats.Length; k++)
                        allTeams.teams[0].players[j][k] = temp[k];

                    if (indexI != -1)
                        trade.yourTrades[indexI] = j;

                    if (indexJ != -1)
                        trade.yourTrades[indexJ] = i;

                    i++;
                    j--;
                }
            }
        else
            while (i <= j)
            {
                while (string.Compare(allTeams.teams[0].players[i][statNum], pivot) > 0)
                {
                    i++;
                }

                while (string.Compare(allTeams.teams[0].players[j][statNum], pivot) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    int indexI = trade.yourTrades.IndexOf(i), indexJ = trade.yourTrades.IndexOf(j);
                    string[] temp = new string[stats.Length];

                    for (int k = 0; k < stats.Length; k++)
                        temp[k] = allTeams.teams[0].players[i][k];

                    for (int k = 0; k < stats.Length; k++)
                        allTeams.teams[0].players[i][k] = allTeams.teams[0].players[j][k];

                    for (int k = 0; k < stats.Length; k++)
                        allTeams.teams[0].players[j][k] = temp[k];

                    if (indexI != -1)
                        trade.yourTrades[indexI] = j;

                    if (indexJ != -1)
                        trade.yourTrades[indexJ] = i;

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
