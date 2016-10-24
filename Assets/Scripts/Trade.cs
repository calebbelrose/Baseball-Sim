using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trade : MonoBehaviour {

    public List<int> yourTrades, theirTrades;
    int theirTeam;

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
            if (theirTeam != teamName)
            {
                theirTrades = new List<int>();
                theirTeam = teamName;
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
            if (theirTeam != teamName)
            {
                theirTrades = new List<int>();
                theirTeam = teamName;
            }
            theirTrades.Remove(playerNum);
        }
    }

    public void Offer()
    {
        AllTeams allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        float yourValue = 0.0f, theirValue = 0.0f;

        for (int i = 0; i < yourTrades.Count; i++)
            yourValue += float.Parse(allTeams.teams[0].players[yourTrades[i]][3]) + (float.Parse(allTeams.teams[0].players[yourTrades[i]][6]) / 7);

        for (int i = 0; i < theirTrades.Count; i++)
            theirValue += float.Parse(allTeams.teams[theirTeam].players[theirTrades[i]][3]) + (float.Parse(allTeams.teams[theirTeam].players[theirTrades[i]][6]) / 7);

        if (yourValue >= theirValue)
        {
            int yourPlayers = allTeams.teams[0].players.Count, theirPlayers = allTeams.teams[theirTeam].players.Count;
            yourTrades.Sort((x1, x2) => x2.CompareTo(x1));
            theirTrades.Sort((x1, x2) => x2.CompareTo(x1));

            while (yourTrades.Count != 0)
            {
                allTeams.teams[theirTeam].players.Add(allTeams.teams[0].players[yourTrades[0]]);
                allTeams.teams[0].players.RemoveAt(yourTrades[0]);
                yourTrades.RemoveAt(0);
            }

            while (theirTrades.Count != 0)
            {
                allTeams.teams[0].players.Add(allTeams.teams[theirTeam].players[theirTrades[0]]);
                allTeams.teams[theirTeam].players.RemoveAt(theirTrades[0]);
                theirTrades.RemoveAt(0);
            }

            if (yourPlayers > allTeams.teams[0].players.Count)
            {
                for (int i = 0; i < allTeams.teams[0].players.Count; i++)
                {
                    string playerString = "";
                    for (int j = 0; j < allTeams.teams[0].players[i].Length - 1; j++)
                        playerString += allTeams.teams[0].players[i][j] + ",";
                    playerString += allTeams.teams[0].players[i][allTeams.teams[0].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + 0 + "-" + i, playerString);
                }
                for (int i = allTeams.teams[0].players.Count; i < yourPlayers; i++)
                    PlayerPrefs.DeleteKey("Player" + 0 + "-" + i);
            }
            else if (yourPlayers < allTeams.teams[0].players.Count)
            {
                for (int i = 0; i < yourPlayers; i++)
                {
                    string playerString = "";
                    for (int j = 0; j < allTeams.teams[0].players[i].Length - 1; j++)
                        playerString += allTeams.teams[0].players[i][j] + ",";
                    playerString += allTeams.teams[0].players[i][allTeams.teams[0].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + 0 + "-" + i, playerString);
                }
            }

            if (yourPlayers > allTeams.teams[0].players.Count)
            {
                for (int i = 0; i < allTeams.teams[0].players.Count; i++)
                {
                    string playerString = "";
                    for (int j = 0; j < allTeams.teams[0].players[i].Length - 1; j++)
                        playerString += allTeams.teams[0].players[i][j] + ",";
                    playerString += allTeams.teams[0].players[i][allTeams.teams[0].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + 0 + "-" + i, playerString);
                }
                for (int i = allTeams.teams[0].players.Count; i < yourPlayers; i++)
                    PlayerPrefs.DeleteKey("Player" + 0 + "-" + i);
            }
            else
            {
                for (int i = 0; i < yourPlayers; i++)
                {
                    string playerString = "";
                    for (int j = 0; j < allTeams.teams[0].players[i].Length - 1; j++)
                        playerString += allTeams.teams[0].players[i][j] + ",";
                    playerString += allTeams.teams[0].players[i][allTeams.teams[0].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + 0 + "-" + i, playerString);
                }
                PlayerPrefs.SetInt("NumPlayers" + 0, allTeams.teams[0].players.Count);
            }

            if (theirPlayers > allTeams.teams[theirTeam].players.Count)
            {
                for (int i = 0; i < allTeams.teams[theirTeam].players.Count; i++)
                {
                    string playerString = "";
                    for (int j = theirTeam; j < allTeams.teams[theirTeam].players[i].Length - 1; j++)
                        playerString += allTeams.teams[theirTeam].players[i][j] + ",";
                    playerString += allTeams.teams[theirTeam].players[i][allTeams.teams[theirTeam].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + theirTeam + "-" + i, playerString);
                }
                for (int i = allTeams.teams[theirTeam].players.Count; i < theirPlayers; i++)
                    PlayerPrefs.DeleteKey("Player" + theirTeam + "-" + i);
            }
            else
            {
                for (int i = 0; i < theirPlayers; i++)
                {
                    string playerString = "";
                    for (int j = theirTeam; j < allTeams.teams[theirTeam].players[i].Length - 1; j++)
                        playerString += allTeams.teams[theirTeam].players[i][j] + ",";
                    playerString += allTeams.teams[theirTeam].players[i][allTeams.teams[theirTeam].players[i].Length - 1];

                    PlayerPrefs.SetString("Player" + theirTeam + "-" + i, playerString);
                }
                PlayerPrefs.SetInt("NumPlayers" + theirTeam, allTeams.teams[theirTeam].players.Count);
            }

            PlayerPrefs.Save();
        }
    }
}
