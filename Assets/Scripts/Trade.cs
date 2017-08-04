using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trade : MonoBehaviour
{
	public List<int> yourTrades, theirTrades;	// List of players from your team and their team to trade
	int theirTeam;								// the other team you are trading with

	void Start ()
	{
		yourTrades = new List<int> ();
		theirTrades = new List<int> ();
	}

	// Adds a player to the trade
	public void AddPlayer (int playerNum, int teamName)
	{
		if (teamName == 0)
			yourTrades.Add (playerNum);
		else
		{
			if (theirTeam != teamName)
			{
				theirTrades = new List<int> ();
				theirTeam = teamName;
			}
			theirTrades.Add (playerNum);
		}
	}

	// Removes a player from the trade
	public void RemovePlayer (int playerNum, int teamName)
	{
		if (teamName == 0)
			yourTrades.Remove (playerNum);
		else
		{
			if (theirTeam != teamName)
			{
				theirTrades = new List<int> ();
				theirTeam = teamName;
			}
			theirTrades.Remove (playerNum);
		}
	}

	// Offers your players for their players
	public void Offer ()
	{
		float yourValue = 0.0f, theirValue = 0.0f;									// The value of your and their players in the trade

		// Calculates the value of your players in the trade
		for (int i = 0; i < yourTrades.Count; i++)
			yourValue += Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [yourTrades [i]]].Overall + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [yourTrades [i]]].Potential / 7;

		// Calculates the value of their players in the trade
		for (int i = 0; i < theirTrades.Count; i++)
			theirValue += Manager.Instance.Players [Manager.Instance.Teams [0] [theirTeam].Players [theirTrades [i]]].Overall + Manager.Instance.Players [Manager.Instance.Teams [0] [theirTeam].Players [theirTrades [i]]].Potential / 7;

		// If your players' values is higher than theirs, they will trade
		if (yourValue >= theirValue)
		{
			int yourPlayers = Manager.Instance.Teams [0] [0].Players.Count, theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.Count;	// Number of players on your and their teams
			string trade = Manager.Instance.Teams [0] [0].CityName + " " + Manager.Instance.Teams [0] [0].TeamName + " has traded ";					// String format of the trade

			// Sorts trades
			yourTrades.Sort ((x1, x2) => x2.CompareTo (x1));
			theirTrades.Sort ((x1, x2) => x2.CompareTo (x1));

			// Adds your new players to your team
			while (yourTrades.Count != 0)
			{
				trade += Manager.Instance.Teams [0] [0].Transfer (yourTrades [0], theirTeam);
				yourTrades.RemoveAt (0);
			}

			trade = trade.Remove (trade.Length - 2) + " to" + Manager.Instance.Teams [0] [theirTeam].CityName + " " + Manager.Instance.Teams [0] [theirTeam].TeamName + " for ";

			// Adds their new players to their team
			while (theirTrades.Count != 0)
			{
				trade += Manager.Instance.Teams [0] [0].Transfer (theirTrades [0], 0);
				theirTrades.RemoveAt (0);
			}

			trade = trade.Remove (trade.Length - 2) + ".";
			Manager.Instance.tradeList.Add (trade);

			// Saves the new players to your team
			for (int i = 0; i < Manager.Instance.Teams [0] [0].Players.Count; i++)
				Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [i]].SavePlayer ();

			// Saves the new players to their team
			for (int i = 0; i < Manager.Instance.Teams [0] [theirTeam].Players.Count; i++)
				Manager.Instance.Players [Manager.Instance.Teams [0] [theirTeam].Players [i]].SavePlayer ();
		}
	}
}
