using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trade : MonoBehaviour {

	public List<int> yourTrades, theirTrades;	// List of players from your team and their team to trade
    int theirTeam;								// the other team you are trading with

    void Start()
    {
        yourTrades = new List<int> ();
        theirTrades = new List<int> ();
    }

	// Adds a player to the trade
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

	// Removes a player from the trade
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

	// Offers your players for their players
    public void Offer()
	{
		AllTeams allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();	// All of the teams
		float yourValue = 0.0f, theirValue = 0.0f;									// The value of your and their players in the trade

		// Calculates the value of your players in the trade
		for (int i = 0; i < yourTrades.Count; i++)
			yourValue += allTeams.teams [0].players [yourTrades [i]].overall + allTeams.teams [0].players [yourTrades [i]].potential / 7;

		// Calculates the value of their players in the trade
		for (int i = 0; i < theirTrades.Count; i++)
			theirValue += allTeams.teams [theirTeam].players [theirTrades [i]].overall + allTeams.teams [theirTeam].players [theirTrades [i]].potential / 7;

		// If your players' values is higher than theirs, they will trade
		if (yourValue >= theirValue) {
			int yourPlayers = allTeams.teams [0].players.Count, theirPlayers = allTeams.teams [theirTeam].players.Count;	// Number of players on your and their teams
			string trade = allTeams.teams[0].cityName + " " + allTeams.teams[0].teamName + " has traded ";					// String format of the trade

			// Sorts trades
			yourTrades.Sort ((x1, x2) => x2.CompareTo (x1));
			theirTrades.Sort ((x1, x2) => x2.CompareTo (x1));

			//  Adds your new players to your team
			while (yourTrades.Count != 0) {
				allTeams.teams [theirTeam].players.Add (allTeams.teams [0].players [yourTrades [0]]);
				allTeams.teams [theirTeam].currentSalary += allTeams.teams [0].players [yourTrades [0]].salary;
				allTeams.teams [0].currentSalary -= allTeams.teams [0].players [yourTrades [0]].salary;
				trade += allTeams.teams[0].players[yourTrades [0]].firstName + " " + allTeams.teams[0].players[yourTrades [0]].lastName + ", ";
				allTeams.teams [0].players.RemoveAt (yourTrades [0]);
				yourTrades.RemoveAt (0);
			}

			trade = trade.Remove (trade.Length - 2) + " to" + allTeams.teams[theirTeam].cityName + " " + allTeams.teams[theirTeam].teamName + " for ";

			//  Adds their new players to their team
			while (theirTrades.Count != 0) {
				allTeams.teams [0].players.Add (allTeams.teams [theirTeam].players [theirTrades [0]]);
				allTeams.teams [0].currentSalary += allTeams.teams [theirTeam].players [theirTrades [0]].salary;
				allTeams.teams [theirTeam].currentSalary -= allTeams.teams [theirTeam].players [theirTrades [0]].salary;
				trade += allTeams.teams[theirTeam].players[theirTrades [0]].firstName + " " + allTeams.teams[theirTeam].players[theirTrades [0]].lastName + ", ";
				allTeams.teams [theirTeam].players.RemoveAt (theirTrades [0]);
				theirTrades.RemoveAt (0);
			}

			trade = trade.Remove (trade.Length - 2) + ".";
			allTeams.tradeList.Add (trade);

			// Saves the new players to your team
			for (int i = 0; i < allTeams.teams [0].players.Count; i++) {
				allTeams.teams [0].players [i].SavePlayer (0);
				allTeams.teams [0].players [i].PlayerIndex = i;
			}

			// Removes the old players from your team if you have less players
			for (int i = allTeams.teams [0].players.Count; i < yourPlayers; i++) {
				PlayerPrefs.DeleteKey ("Player0-" + i);
				PlayerPrefs.DeleteKey ("PlayerStats0-" + i);
			}

			// Saves the new players to their team
			for (int i = 0; i < allTeams.teams [theirTeam].players.Count; i++) {
				allTeams.teams [theirTeam].players [i].SavePlayer (theirTeam);
				allTeams.teams [theirTeam].players [i].PlayerIndex = i;
			}

			// Removes the old players from their team if they have less players
			for (int i = allTeams.teams [theirTeam].players.Count; i < theirPlayers; i++) {
				PlayerPrefs.DeleteKey ("Player" + theirTeam + "-" + i);
				PlayerPrefs.DeleteKey ("PlayerStats" + theirTeam + "-" + i);
			}

			PlayerPrefs.Save ();
		}
	}
}
