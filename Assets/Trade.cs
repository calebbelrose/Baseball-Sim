using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trade : MonoBehaviour {

    public List<int> yourTrades, theirTrades;
    int otherTeam;

    void Start()
    {
        yourTrades = new List<int>();
        theirTrades = new List<int>();
    }

	public void AddPlayer(int playerNum, int teamName)
    {
        if (teamName == 0)
            yourTrades.Add(playerNum);
        else
        {
            if (otherTeam != teamName)
            {
                theirTrades = new List<int>();
                otherTeam = teamName;
            }
            theirTrades.Add(playerNum);
        }
    }

    public void RemovePlayer(int playerNum, int teamName)
    {
        if (teamName == 0)
            yourTrades.Remove(playerNum);
        else
        {
            if (otherTeam != teamName)
            {
                theirTrades = new List<int>();
                otherTeam = teamName;
            }
            theirTrades.Remove(playerNum);
        }
    }
}
