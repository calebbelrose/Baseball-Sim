using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class PopulateDraftPlayers : MonoBehaviour {

    string[,] playerStats;
    int numStats;

    // Use this for initialization
    void Start () {
        string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
        string[] firstNames, lastNames, stats;
        //players = File.ReadAllLines("players.csv");
        firstNames = File.ReadAllLines("FirstNames.txt");
        lastNames = File.ReadAllLines("LastNames.txt");
        stats = File.ReadAllLines("Stats.txt");
        numStats = stats.Length;
        //playerStats = new string [players.Length,numStats];
        int longestFirstName = 0, longestLastName = 0, statHeaderLength = 0;

        //int numPlayers = (int)(Random.value * 5.0f) + 10;
        int numPlayers = 250;
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

        for(int i = 2; i < stats.Length; i++)
        {
            statHeaderLength += stats[i].Length + 1;
        }


        Object playerButton = Resources.Load("Button", typeof(GameObject));
        float totalWidth = 0.0f;

        for (int i = 0; i < numStats; i++)
        {
            GameObject statHeader = Instantiate(playerButton) as GameObject;
            statHeader.name = "header" + stats[i];
            statHeader.transform.SetParent(draftListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = stats[i];
            statHeader.GetComponent<Button>().interactable = false;
            //statHeader.GetComponent<RectTransform>().rect.height = 20;
            //statHeader.GetComponent<RectTransform>().rect.width = 8.04 * (stats[i].Length + 1);

            Rect rect = statHeader.GetComponent<RectTransform>().rect;
            if (rect != null)
            {
                float currWidth = 8.04f * (stats[i].Length + 1);
                rect.size = new Vector2(currWidth, 20);
                rect.x = totalWidth;
                totalWidth += currWidth;
            }
        }

        if (longestFirstName < 10)
            longestFirstName = 10;

        if (longestLastName < 9)
            longestLastName = 9;

        

        Sort(3, 0, numPlayers - 1);

        GameObject draftList = GameObject.Find("DraftList");
        draftList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * numPlayers - draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        draftList.GetComponent<RectTransform>().offsetMax = new Vector2((int)(8.04 * (statHeaderLength + 1)), 0);

        for (int i = 0; i < numPlayers; i++)
        {
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

    void Sort(int statNum, int left, int right)
    {
        int i = left, j = right;
        int pivot = int.Parse(playerStats[(int)(left + (right - left) / 2), statNum]);

        while(i <= j)
        {
            while(int.Parse(playerStats[i, statNum]) > pivot)
            {
                i++;
            }

            while (int.Parse(playerStats[j, statNum]) < pivot)
            {
                j--;
            }

            if(i <= j)
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

        if(left < j)
        {
            Sort(statNum, left, j);
        }

        if (i < right)
        {
            Sort(statNum, i, right);
        }
    }
}
