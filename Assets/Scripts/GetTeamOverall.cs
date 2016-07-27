using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetTeamOverall : MonoBehaviour
{

    string[][] players;
    GameObject manager;

    // Use this for initialization
    void Start()
    {
        
    }

    public void GetOverall()
    {
        float totalBestPlayers = 0.0f;
        manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        List<string[]> playerList = allTeams.teams[0].players;
        string currPos;
        int currPlayer = 0, numSP = 0, numRP = 0;
        players = new string[playerList.Count][];
        playerList.CopyTo(players);
        var result = playerList.OrderBy(player => player[2]).ThenByDescending(player => player[3]).ToArray<string[]>();
        currPos = "";
        while (currPlayer < (result.Length - 1))
        {
            if (result[currPlayer][2] == "SP" && numSP < 5)
            {
                totalBestPlayers += float.Parse(result[currPlayer][3]);
                numSP++;
            }
            else if (result[currPlayer][2] == "RP" && numRP < 3)
            {
                totalBestPlayers += float.Parse(result[currPlayer][3]);
                numRP++;
            }
            else if (result[currPlayer][2] != currPos)
            {
                totalBestPlayers += float.Parse(result[currPlayer][3]);
            }

            currPos = result[currPlayer][2];
            currPlayer++;
        }
        GetComponent<Text>().text = (totalBestPlayers / 18.0f).ToString();
    }
}
