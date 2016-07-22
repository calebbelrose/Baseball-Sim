using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class PopulateDraftPlayers : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int numStats = 8;
        string[,] playerStats;
        string[] players, player, firstNames, lastNames;
        //players = File.ReadAllLines("players.csv");
        firstNames = File.ReadAllLines("firstNames.txt");
        lastNames = File.ReadAllLines("lastNames.txt");
        //playerStats = new string [players.Length,numStats];
        int longestFirstName = 0, longestLastName = 0;

        //int numPlayers = (int)(Random.value * 5.0f) + 10;
        int numPlayers = 20;
        playerStats = new string[numPlayers, numStats];

        for (int i = 0; i < numPlayers; i++)
        {
            playerStats[i,0] = firstNames[(int)(Random.value * firstNames.Length)];
            playerStats[i,1] = lastNames[(int)(Random.value * lastNames.Length)];

            if (playerStats[i, 0].Length > longestFirstName)
                longestFirstName = playerStats[i, 0].Length;

            if (playerStats[i, 1].Length > longestLastName)
                longestLastName = playerStats[i, 1].Length;

            for (int j = 2; j < numStats; j++)
            {
                playerStats[i, j] = ((int)(Random.value * 75) + 25).ToString();
            }
        }

        GameObject draftList = GameObject.Find("DraftList");
        draftList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * numPlayers - draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        draftList.GetComponent<RectTransform>().offsetMax = new Vector2((int)(48.25 * (numStats - 2)), 0);
        Object playerButton = Resources.Load("Button", typeof(GameObject));

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

                for (int k = playerStats[i, j].Length; k < 5; k++)
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
