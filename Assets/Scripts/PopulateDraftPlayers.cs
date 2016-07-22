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

        //int numPlayers = (int)(Random.value * 5.0f) + 10;
        int numPlayers = 1;
        playerStats = new string[numPlayers, numStats];

        for (int i = 0; i < numPlayers; i++)
        {
            playerStats[i,0] = firstNames[(int)(Random.value * firstNames.Length)];
            playerStats[i,1] = lastNames[(int)(Random.value * lastNames.Length)];
            for(int j = 2; j < numStats; j++)
            {
                playerStats[i, j] = ((int)(Random.value * 75) + 25).ToString();
            }
        }

        GameObject draftList = GameObject.Find("DraftList");
        Object playerButton = Resources.Load("Button", typeof(GameObject));

        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
            newPlayer.transform.SetParent(draftList.transform);
            string playerListing = string.Format("{0:-15}{1:-15}", playerStats[i,0], playerStats[i, 1]);

            for(int j = 2; j < numStats; j++)
            {
                playerListing += string.Format("{0:-5}", playerStats[i, j]);
            }
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
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
