using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PopulateDraftPlayers : MonoBehaviour {

    List<string[]> playerStats = new List<string[]>();
    List<string[]>[] newPlayers;
    int numPlayers = 0, initialPlayers;
    GameObject draftList, manager;
    RectTransform draftListRect, draftListParentRect;
    AllTeams allTeams;
    int longestFirstName = 10, longestLastName = 9;
    string[] stats;
    int currSortedStat = 3;
    char order = 'd';
    int currTeam = 0;
    float newWidth = 0.0f;

    void Awake()
    {
        if(allTeams != null && allTeams.needDraft)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(4);
    }

    // Use this for initialization
    void Start() {
        string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
        string[] firstNames, lastNames;

        int statHeaderLength = 0;
        manager = GameObject.Find("_Manager");
        draftList = GameObject.Find("DraftList");
        allTeams = manager.GetComponent<AllTeams>();
        newPlayers = new List<string[]>[allTeams.GetNumTeams()];
        draftListRect = draftList.GetComponent<RectTransform>();
        draftListParentRect = draftList.transform.parent.gameObject.GetComponent<RectTransform>();
        stats = manager.GetComponent<Stats>().statList;
        firstNames = manager.GetComponent<Stats>().firstNames;
        lastNames = manager.GetComponent<Stats>().lastNames;

        for (int i = 0; i < newPlayers.Length; i++)
            newPlayers[i] = new List<string[]>();

        while (PlayerPrefs.HasKey("Draft" + numPlayers.ToString()))
        {
            numPlayers++;
        }

        if (numPlayers > 0)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                string[] newPlayer = new string[stats.Length];
                string playerString = PlayerPrefs.GetString("Draft" + i.ToString());
                newPlayer = playerString.Split(',');
                playerStats.Add(newPlayer);
                if (newPlayer[0].Length > longestFirstName)
                    longestFirstName = newPlayer[0].Length;

                if (newPlayer[1].Length > longestLastName)
                    longestLastName = newPlayer[1].Length;
            }
        }
        else
        {
            numPlayers = (int)(Random.value * 5.0f) + 250;
            numPlayers = 1;

            for (int i = 0; i < numPlayers; i++)
            {
                string[] newPlayer = new string[stats.Length];
                float totalStats = 0, totalOffense = 0, totalDefense = 0;
                int age;
                string playerString = "";

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];

                if (newPlayer[0].Length > longestFirstName)
                    longestFirstName = newPlayer[0].Length;

                if (newPlayer[1].Length > longestLastName)
                    longestLastName = newPlayer[1].Length;

                newPlayer[2] = positions[(int)(Random.value * positions.Length)];

                age = (int)(Random.value * 27) + 18;
                newPlayer[7] = age.ToString();

                for (int j = 8; j < stats.Length; j++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[j] = currStat.ToString();
                    totalStats += currStat;
                    if (j < 11)
                        totalOffense += currStat;
                    else if (j > 11)
                        totalDefense += currStat;
                    else
                    {
                        totalOffense += currStat;
                        totalDefense += currStat;
                    }
                }

                int potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[6] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 8))).ToString("0.00");
                newPlayer[4] = ((totalOffense / 4)).ToString("0.00");
                newPlayer[5] = ((totalDefense / 4)).ToString("0.00");

                playerStats.Add(newPlayer);

                for (int j = 0; j < stats.Length - 1; j++)
                    playerString += newPlayer[j] + ",";

                playerString += newPlayer[stats.Length - 1];

                PlayerPrefs.SetString("Draft" + i.ToString(), playerString);
            }
        }
        PlayerPrefs.Save();

        initialPlayers = numPlayers;
        GameObject draftListHeader = GameObject.Find("DraftListHeader");

        statHeaderLength += longestFirstName + longestLastName + 2;

        for (int i = 2; i < stats.Length; i++)
        {
            statHeaderLength += stats[i].Length + 1;
        }

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = 5.0f;
        float totalWidth = (8.04f * (statHeaderLength + 1.0f));
        totalWidth /= -2.0f;

        for (int i = 0; i < stats.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(draftListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = stats[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader));

            float currWidth;
            if (i > 1)
                currWidth = (8.04f * (stats[i].Length + 1));
            else if (i == 1)
                currWidth = (8.04f * (longestLastName + 1));
            else
                currWidth = (8.04f * (longestFirstName + 1));

            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        newWidth -= draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;

        Sort(3, 0, numPlayers - 1);
        DisplayPlayers();
    }

    public void StartSorting(GameObject other)
    {
        int statNum = int.Parse(other.name.Remove(0, 6));
        int left = 0, right = numPlayers - 1;
        string pivot = playerStats[(int)(left + (right - left) / 2)][statNum];
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
        string pivot = playerStats[(int)(left + (right - left) / 2)][statNum];

        if(order == 'a')
            while (i <= j)
            {
                while (string.Compare(playerStats[i][statNum], pivot) < 0)
                {
                    i++;
                }

                while (string.Compare(playerStats[j][statNum], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp = new string[stats.Length];

                    for (int k = 0; k < stats.Length; k++)
                        temp[k] = playerStats[i][k];

                    for (int k = 0; k < stats.Length; k++)
                        playerStats[i][k] = playerStats[j][k];

                    for (int k = 0; k < stats.Length; k++)
                        playerStats[j][k] = temp[k];

                    i++;
                    j--;
                }
            }
        else
            while (i <= j)
            {
                while (string.Compare(playerStats[i][statNum], pivot) > 0)
                {
                    i++;
                }

                while (string.Compare(playerStats[j][statNum], pivot) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp = new string[stats.Length];

                    for (int k = 0; k < stats.Length; k++)
                        temp[k] = playerStats[i][k];

                    for (int k = 0; k < stats.Length; k++)
                        playerStats[i][k] = playerStats[j][k];

                    for (int k = 0; k < stats.Length; k++)
                        playerStats[j][k] = temp[k];

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

    void DisplayPlayers()
    {
        GameObject[] currPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < currPlayers.Length; i++)
            Destroy(currPlayers[i]);

        for (int i = 0; i < numPlayers; i++)
        {
            Object playerButton = Resources.Load("Player", typeof(GameObject));
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
            string playerListing = playerStats[i][0];

            newPlayer.name = "player" + i.ToString();
            newPlayer.transform.SetParent(draftList.transform);
            
            for (int j = playerStats[i][0].Length; j < longestFirstName; j++)
                playerListing += " ";

            playerListing += " " + playerStats[i][1];

            for (int j = playerStats[i][1].Length; j < longestLastName; j++)
                playerListing += " ";

            for (int j = 2; j < stats.Length - 1; j++)
            {
                playerListing += " " + playerStats[i][j];

                for (int k = playerStats[i][j].Length; k < stats[j].Length; k++)
                    playerListing += " ";
            }

            playerListing += " " + playerStats[i][stats.Length - 1];
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newPlayer.GetComponent<Button>().onClick.AddListener(() => PlayerDraft(newPlayer, playerListing));
        }

        draftListRect.offsetMin = new Vector2(0, -(20 * (numPlayers + 1) - draftListParentRect.rect.height));
        draftListRect.offsetMax = new Vector2(newWidth, 0);
    }

    public void PlayerDraft(GameObject player, string playerListing)
    {
        int count = numPlayers;

        Draft(player, player.name.Remove(0, 6));

        if (numPlayers > 30)
            count = 30;

        for(int i = 1; i < count; i++)
        {
            Draft(GameObject.Find("player" + i), "0");
        }
        DisplayPlayers();
    }

    public void Draft(GameObject player, string name)
    {
        int playerNum = int.Parse(name);

        newPlayers[currTeam].Add(playerStats[playerNum]);
        numPlayers--;

        if (numPlayers == 0)
        {
            int numTeams = allTeams.GetNumTeams();
            List<string> starters = new List<string>();

            starters.Add("SP");
            starters.Add("SP");
            starters.Add("SP");
            starters.Add("SP");
            starters.Add("SP");
            starters.Add("RP");
            starters.Add("RP");
            starters.Add("RP");
            starters.Add("CP");
            starters.Add("C");
            starters.Add("1B");
            starters.Add("2B");
            starters.Add("3B");
            starters.Add("SS");
            starters.Add("LF");
            starters.Add("CF");
            starters.Add("RF");
            starters.Add("DH");

            for (int i = 0; i < numTeams; i++)
            {
                float totalBestPlayers = 0.0f, offenseBestPlayers = 0.0f, defenseBestPlayers = 0.0f;

                allTeams.teams[i].Batters.Clear();
                allTeams.teams[i].SP.Clear();
                allTeams.teams[i].RP.Clear();
                allTeams.teams[i].CP.Clear();

                while (newPlayers[i].Count > 0)
                {
                    string playerString = "";
                    string[] newPlayer = newPlayers[i].First();
                    string currStats = "";

                    allTeams.teams[i].players.Add(newPlayer);

                    allTeams.teams[i].pStats.Add(allTeams.teams[i].emptyStats);

                    for (int k = 0; k < allTeams.teams[i].pStats[allTeams.teams[i].pStats.Count - 1].Length - 1; k++)
                        currStats += allTeams.teams[i].pStats[allTeams.teams[i].pStats.Count - 1][k] + ",";

                    currStats += allTeams.teams[i].pStats[allTeams.teams[i].pStats.Count - 1][allTeams.teams[i].pStats[allTeams.teams[i].pStats.Count - 1].Length - 1];

                    PlayerPrefs.SetString("PlayerStats" + i + "-" + (allTeams.teams[i].pStats.Count - 1), currStats);

                    for (int k = 0; k < newPlayer.Length - 1; k++)
                        playerString += newPlayer[k] + ",";

                    playerString += newPlayer[newPlayer.Length - 1];
                    newPlayers[i].RemoveAt(0);
                    PlayerPrefs.SetString("Player" + i + "-" + (allTeams.teams[i].players.Count - 1), playerString);
                }

                var result = allTeams.teams[i].players.OrderBy(playerX => playerX[2]).ThenByDescending(playerX => playerX[3]).ToArray<string[]>();

                PlayerPrefs.SetInt("NumPlayers" + i, allTeams.teams[i].players.Count);

                for(int j = 0; j < result.Length; j++)
                {
                    if (starters.Contains(result[j][2]))
                    {
                        totalBestPlayers += float.Parse(result[j][3]);
                        offenseBestPlayers += float.Parse(result[j][8]) + float.Parse(result[j][9]) + float.Parse(result[j][10]) + float.Parse(result[j][11]);
                        defenseBestPlayers += float.Parse(result[j][11]) + float.Parse(result[j][12]) + float.Parse(result[j][13]) + float.Parse(result[j][14]);

                        if (result[j][2] == "SP")
                        {
                            PlayerPrefs.SetInt("SP" + i + "-" + allTeams.teams[i].SP.Count, j);
                            allTeams.teams[i].SP.Add(j);
                        }
                        else if (result[j][2] == "RP")
                        {
                            PlayerPrefs.SetInt("RP" + i + "-" + allTeams.teams[i].RP.Count, j);
                            allTeams.teams[i].RP.Add(j);
                        }
                        else if (result[j][2] == "CP")
                        {
                            PlayerPrefs.SetInt("CP" + i + "-" + allTeams.teams[i].CP.Count, j);
                            allTeams.teams[i].CP.Add(j);
                        }
                        else
                        {
                            PlayerPrefs.SetInt("Batters" + i + "-" + allTeams.teams[i].Batters.Count, j);
                            allTeams.teams[i].Batters.Add(j);
                        }
                    }
                }

                Order(allTeams.teams[i].Batters, i, 4, 0, allTeams.teams[i].Batters.Count - 1);
                Order(allTeams.teams[i].SP, i, 13, 0, allTeams.teams[i].SP.Count - 1);
                Order(allTeams.teams[i].RP, i, 13, 0, allTeams.teams[i].RP.Count - 1);

                allTeams.teams[i].overalls[0] = totalBestPlayers / 18.0f;
                allTeams.teams[i].overalls[1] = offenseBestPlayers / 18.0f;
                allTeams.teams[i].overalls[2] = defenseBestPlayers / 18.0f;
                PlayerPrefs.SetString("Overalls" + allTeams.teams[i].id, allTeams.teams[i].overalls[0] + "," + allTeams.teams[i].overalls[1] + "," + allTeams.teams[i].overalls[2]);
                PlayerPrefs.Save();
            }

            for (int i = 0; i < initialPlayers; i++)
                PlayerPrefs.DeleteKey("Draft" + i.ToString());

            allTeams.needDraft = false;
            PlayerPrefs.SetString("NeedDraft", allTeams.needDraft.ToString());

            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(4);
        }
        currTeam = (currTeam + 1) % 30;
        Destroy(player);
        playerStats.RemoveAt(playerNum);
    }

    void Order(List<int> list, int team, int stat, int left, int right)
    {
        int i = left, j = right;
        string pivot = allTeams.teams[team].players[list[(int)(left + (right - left) / 2)]][stat].ToString();

        while (i <= j)
        {
            while (string.Compare(allTeams.teams[team].players[list[i]][stat].ToString(), pivot) > 0)
            {
                i++;
            }

            while (string.Compare(allTeams.teams[team].players[list[j]][stat].ToString(), pivot) < 0)
            {
                j--;
            }

            if (i <= j)
            {
                int temp = list[i];
                list[i] = list[j];
                list[j] = temp;

                i++;
                j--;
            }
        }

        if (left < j)
        {
            Order(list, team, stat, left, j);
        }

        if (i < right)
        {
            Order(list, team, stat, i, right);
        }
    }
}
