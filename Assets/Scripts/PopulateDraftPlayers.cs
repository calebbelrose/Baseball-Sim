using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class PopulateDraftPlayers : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
        int numStats;
        string[,] playerStats;
        string[] firstNames, lastNames, stats;
        //players = File.ReadAllLines("players.csv");
        firstNames = File.ReadAllLines("FirstNames.txt");
        lastNames = File.ReadAllLines("LastNames.txt");
        stats = File.ReadAllLines("Stats.txt");
        numStats = stats.Length;
        //playerStats = new string [players.Length,numStats];
        int longestFirstName = 0, longestLastName = 0, statHeaderLength = 0;

        //int numPlayers = (int)(Random.value * 5.0f) + 10;
        int numPlayers = 5;
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

        GameObject draftList = GameObject.Find("DraftList");
        draftList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * numPlayers - draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));

        for(int i = 2; i < stats.Length; i++)
        {
            statHeaderLength += stats[i].Length + 1;
        }

        draftList.GetComponent<RectTransform>().offsetMax = new Vector2((int)(8.04 * (statHeaderLength + 1)), 0);
        Object playerButton = Resources.Load("Button", typeof(GameObject));

        GameObject statHeaders = Instantiate(playerButton) as GameObject;
        statHeaders.name = "statHeaders";
        statHeaders.transform.SetParent(draftList.transform);
        string statHeaderText = "";

        statHeaderText = stats[0];

        if (longestFirstName < 10)
            longestFirstName = 10;

        for (int i = stats[0].Length; i < longestFirstName; i++)
            statHeaderText += " ";

        statHeaderText += " " + stats[1];

        if (longestLastName < 9)
            longestLastName = 9;

        for (int i = stats[1].Length; i < longestLastName; i++)
            statHeaderText += " ";

        for (int i = 2; i < numStats; i++)
        {
            statHeaderText += " " + stats[i];
        }

        statHeaders.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        statHeaders.transform.GetChild(0).gameObject.GetComponent<Text>().text = statHeaderText;
        statHeaders.GetComponent<Button>().interactable = false;

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

        /*GameObject[] players = new GameObject[numPlayers];
        //Object player = Resources.Load("txtPlayer", typeof(GameObject));
        for (int i = 0; i < numPlayers; i++)
        {
            player = players[i].Split(',');

            for (int j = 0; j < player.Length; j++)
                playerStats[i, j] = player[j];
        }*/
	}
}
