using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class PopulateDraftPlayers : MonoBehaviour {

    string[,] playerStats;
    int numStats;
    int numPlayers = 5;
    GameObject draftList;
    int longestFirstName = 0, longestLastName = 0;
    string[] stats;
    int currSortedStat = 3;
    char order = 'd';

    // Use this for initialization
    void Start () {
        string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
        string[] firstNames, lastNames;
        //players = File.ReadAllLines("players.csv");
        firstNames = File.ReadAllLines("FirstNames.txt");
        lastNames = File.ReadAllLines("LastNames.txt");
        stats = File.ReadAllLines("Stats.txt");
        numStats = stats.Length;
        //playerStats = new string [players.Length,numStats];
        int statHeaderLength = 0;

        //int numPlayers = (int)(Random.value * 5.0f) + 10;
        
        playerStats = new string[numPlayers, numStats];
        for (int i = 0; i < numPlayers; i++)
        {
            int totalStats = 0, age;
            playerStats[i,0] = firstNames[(int)(Random.value * firstNames.Length)];
            playerStats[i,1] = lastNames[(int)(Random.value * lastNames.Length)];

            if (playerStats[i, 0].Length > longestFirstName)
                longestFirstName = playerStats[i, 0].Length;

            if (playerStats[i, 1].Length > longestLastName)
                longestLastName = playerStats[i, 1].Length;

            playerStats[i, 2] = positions[(int)(Random.value * positions.Length)];

            age = (int)(Random.value * 27) + 18;
            playerStats[i, 5] = age.ToString();

            for (int j = 6; j < numStats; j++)
            {
                int currStat = (int)(Random.value * age) + 55;
                playerStats[i, j] = currStat.ToString();
                totalStats += currStat;
            }

            int potential = (int)(Random.value * 25 + (43 - age) * 3) ;
            if (potential < 0)
                potential = 0;
            playerStats[i, 4] = potential.ToString();

            playerStats[i, 3] = ((int)(totalStats / (numStats - 6))).ToString();
        }
        GameObject draftListHeader = GameObject.Find("DraftListHeader");

        for(int i = 0; i < stats.Length; i++)
        {
            statHeaderLength += stats[i].Length + 1;
        }

        Object header = Resources.Load("Header", typeof(GameObject));
        draftList = GameObject.Find("DraftList");
        float prevWidth = 5.0f, newWidth = 0.0f;
        float totalWidth = (8.04f * (statHeaderLength + 1.0f));
        draftList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * numPlayers - draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        draftList.GetComponent<RectTransform>().offsetMax = new Vector2(totalWidth - 160.0f, 0);
        totalWidth /= -2.0f;

        for (int i = 0; i < numStats; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(draftListHeader.transform);
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

        draftList.GetComponent<RectTransform>().offsetMax = new Vector2(newWidth - 160.0f, 0);

        if (longestFirstName < 10)
            longestFirstName = 10;

        if (longestLastName < 9)
            longestLastName = 9;

        Sort(3, 0, numPlayers - 1);
        DisplayPlayers();
    }

    public void StartSorting(GameObject other)
    {
        int statNum = int.Parse(other.name.Remove(0, 6));
        int left = 0, right = numPlayers - 1;
        string pivot = playerStats[(int)(left + (right - left) / 2), statNum];
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
        string pivot = playerStats[(int)(left + (right - left) / 2), statNum];

        Debug.Log(order);

        if(order == 'a')
            while (i <= j)
            {
                while (string.Compare(playerStats[i, statNum], pivot) < 0)
                {
                    i++;
                }

                while (string.Compare(playerStats[j, statNum], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp = new string[numStats];

                    for (int k = 0; k < numStats; k++)
                        temp[k] = playerStats[i, k];

                    for (int k = 0; k < numStats; k++)
                        playerStats[i, k] = playerStats[j, k];

                    for (int k = 0; k < numStats; k++)
                        playerStats[j, k] = temp[k];

                    i++;
                    j--;
                }
            }
        else
            while (i <= j)
            {
                while (string.Compare(playerStats[i, statNum], pivot) > 0)
                {
                    i++;
                }

                while (string.Compare(playerStats[j, statNum], pivot) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    string[] temp = new string[numStats];

                    for (int k = 0; k < numStats; k++)
                        temp[k] = playerStats[i, k];

                    for (int k = 0; k < numStats; k++)
                        playerStats[i, k] = playerStats[j, k];

                    for (int k = 0; k < numStats; k++)
                        playerStats[j, k] = temp[k];

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
            Object playerButton = Resources.Load("Button", typeof(GameObject));
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
            newPlayer.name = "player" + i.ToString();
            newPlayer.transform.SetParent(draftList.transform);
            string playerListing = playerStats[i, 0];

            for (int j = playerStats[i, 0].Length; j < longestFirstName; j++)
                playerListing += " ";

            playerListing += " " + playerStats[i, 1];

            for (int j = playerStats[i, 1].Length; j < longestLastName; j++)
                playerListing += " ";

            for (int j = 2; j < numStats - 1; j++)
            {
                playerListing += " " + playerStats[i, j];

                for (int k = playerStats[i, j].Length; k < stats[j].Length; k++)
                    playerListing += " ";
            }

            playerListing += " " + playerStats[i, numStats - 1];

            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
