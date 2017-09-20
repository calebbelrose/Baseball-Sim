using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOffer
{
	public List<int> yourTrades = new List<int> ();		// List of players from your team to trade
	public List<int> theirTrades = new List<int> ();	// List of players from their team to trade

	private int yourTeam;								// ID of your team
	private int theirTeam;								// ID of their team
	private float yourValue = 0.0f;						// Value of your players in the trade
	private float theirValue = 0.0f;					// Value of their players in the trade

	private static float lookingForValue = 1 / 0.9f;	// Bonus multiplier for a player whose position is wanted by the other team

	public TradeOffer (int _yourTeam, int _theirTeam)
	{
		yourTeam = _yourTeam;
		theirTeam = _theirTeam;
	}

	public void Clear ()
	{
		yourTrades.Clear ();
		theirTrades.Clear ();
	}

	// Adds a player to the trade
	public void AddPlayer (int playerNum, int teamID)
	{
		if (teamID == yourTeam)
			yourTrades.Add (playerNum);
		else
			theirTrades.Add (playerNum);
	}

	// Removes a player from the trade
	public void RemovePlayer (int playerNum, int teamID)
	{
		if (teamID == yourTeam)
			yourTrades.Remove (playerNum);
		else
			theirTrades.Remove (playerNum);
	}

	// Considers the offer
	public bool Consider ()
	{
		if (Manager.Instance.TradeDeadline == TradeDeadline.NonWaiver)
			return false;
		else
		{
			if (!CalculateValues ())
				return false;
			else
			{
				// If your players' values is higher than theirs, they will trade
				if (yourValue >= theirValue)
				{
					if (Manager.Instance.TradeDeadline == TradeDeadline.None)
						Accept ();
					else if(Manager.Instance.TradeDeadline == TradeDeadline.NonWaiver)
					{
						bool clearedWaivers = true;

						for (int i = 0; i < yourTrades.Count; i++)
							if (!Manager.Instance.Players [yourTrades [i]].ClearedWaivers)
							{
								if (!Manager.Instance.Players [yourTrades [i]].OnWaivers)
									Manager.Instance.Teams [0] [yourTeam].PutOnWaivers (yourTrades [i], true);
								
								clearedWaivers = false;
							}

						for (int i = 0; i < theirTrades.Count; i++)
							if (!Manager.Instance.Players [theirTrades [i]].ClearedWaivers)
							{
								if (!Manager.Instance.Players [theirTrades [i]].OnWaivers)
									Manager.Instance.Teams [0] [theirTeam].PutOnWaivers (theirTrades [i], true);
								
								clearedWaivers = false;
							}

						if (clearedWaivers)
							Accept ();
					}

					return true;
				}
				else
					return false;
			}
		}
	}

	// Accepts the offer
	void Accept ()
	{
		string trade = Manager.Instance.Teams [0] [yourTeam].CityName + " " + Manager.Instance.Teams [0] [yourTeam].TeamName + " has traded ";					// String format of the trade

		// Adds your new players to your team
		while (yourTrades.Count != 0)
		{
			trade += Manager.Instance.Teams [0] [yourTeam].Transfer (yourTrades [0], theirTeam);
			yourTrades.RemoveAt (0);
		}

		trade = trade.Remove (trade.Length - 2) + " to" + Manager.Instance.Teams [0] [theirTeam].CityName + " " + Manager.Instance.Teams [0] [theirTeam].TeamName + " for ";

		// Adds their new players to their team
		while (theirTrades.Count != 0)
		{
			trade += Manager.Instance.Teams [0] [theirTeam].Transfer (theirTrades [0], 0);
			theirTrades.RemoveAt (0);
		}

		trade = trade.Remove (trade.Length - 2) + ".";
		Manager.Instance.TradeList.Add (trade);
		Manager.Instance.Teams [0] [yourTeam].SavePlayers ();
		Manager.Instance.Teams [0] [theirTeam].SavePlayers ();

		Debug.Log (true);
	}

	public bool CalculateValues ()
	{
		return CalculateYourValue () && CalculateTheirValue ();
	}

	// Calculates the value of your players in the trade
	public bool CalculateYourValue ()
	{
		yourValue = 0;

		for (int i = 0; i < yourTrades.Count; i++)
		{
			if (Manager.Instance.Players [yourTrades [i]].Team == yourTeam)
			{
				if (Manager.Instance.Teams [0] [theirTeam].LookingFor.Contains (Manager.Instance.Players [yourTrades [i]].Position))
					yourValue += (Manager.Instance.Players [yourTrades [i]].TradeValue * lookingForValue);
				else
					yourValue += Manager.Instance.Players [yourTrades [i]].TradeValue;
			}
			else
				return false;
		}

		return true;
	}

	// Calculates the value of their players in the trade
	public bool CalculateTheirValue ()
	{
		theirValue = 0;

		for (int i = 0; i < theirTrades.Count; i++)
		{
			if (Manager.Instance.Players [theirTrades [i]].Team == theirTeam)
				theirValue += Manager.Instance.Players [theirTrades [i]].TradeValue;
			else
				return false;
		}

		return true;
	}

	public bool HaveOffer
	{
		get
		{
			return yourTrades.Count > 0;
		}
	}

	public float YourValue
	{
		get
		{
			return yourValue;
		}
	}

	public float TheirValue
	{
		get
		{
			return theirValue;
		}
	}

	public int YourTeam
	{
		get
		{
			return yourTeam;
		}
	}

	public int TheirTeam
	{
		get
		{
			return theirTeam;
		}
	}
}
