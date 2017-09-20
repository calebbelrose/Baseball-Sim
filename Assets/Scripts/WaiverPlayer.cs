using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiverPlayer
{
	private int id;						// PlayerID
	private int length;					// How many days are left in waivers
	private int currentClaim;			// Team with the current claim
	private List<int> teamsToClaim;		// Teams still able to claim
	private bool trade;			// Trade that put the player on waivers (if there is one)

	// 2-Arg Constructor
	public WaiverPlayer (int _id, bool _trade)
	{
		id = _id;
		trade = _trade;
		length = 10;

		Manager.Instance.Players [id].PutOnWaivers ();

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			teamsToClaim.Add (i);

		teamsToClaim.Remove (Manager.Instance.Players [id].Team);
		currentClaim = Manager.Instance.Players [id].Team;
	}

	// 4-arg Constructor
	public WaiverPlayer (int _id, int _length, int _currentClaim, bool _trade)
	{
		id = _id;
		length = _length;
		currentClaim = _currentClaim;
		trade = _trade;
	}

	// Advances the day
	public void AdvanceDay ()
	{
		for (int i = 0; i < teamsToClaim.Count; i++)
		{
			if (Manager.Instance.Players [id].Overall > Manager.Instance.Players [Manager.Instance.Teams [0] [i].Batters [Manager.Instance.Teams [0] [i].Positions.IndexOf (Manager.Instance.Players [id].Position)] [0]].Overall || Manager.Instance.Players [Manager.Instance.Teams [0] [i].GetWorstOverall ()].Overall > Manager.Instance.Players [Manager.Instance.Teams [0] [i].Batters [Manager.Instance.Teams [0] [i].Positions.IndexOf (Manager.Instance.Players [id].Position)] [0]].Overall)
				Claim (teamsToClaim [i]);
		}

		length--;

		if (length == 0)
		{
			TakeOffWaivers ();

			if (Manager.Instance.Players [id].Team == currentClaim)
				Manager.Instance.Players [id].ClearWaivers ();
			else if (!trade)
				Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].Transfer (id, currentClaim);
		}
	}

	// Claims the player
	public void Claim (int team)
	{
		int index = teamsToClaim.IndexOf (team);

		if(index != -1)
		{
			teamsToClaim.RemoveAt (index);

			if(trade)
			{
				if (team == Manager.Instance.Players [id].Team)
					currentClaim = team;
				else if (Manager.Instance.Players [id].FirstTimeOnWaivers)
					Manager.Instance.TakeOffWaivers (id);
			}
			else
			{
				if (Manager.Instance.Teams [0] [currentClaim].League != Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League)
				{
					if (Manager.Instance.Teams [0] [team].League == Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League || Manager.Instance.Teams [0] [team].Wins / Manager.Instance.Teams [0] [team].Losses < Manager.Instance.Teams [0] [currentClaim].Wins / Manager.Instance.Teams [0] [currentClaim].Losses)
						currentClaim = team;
				}
				else if (Manager.Instance.Teams [0] [team].League == Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League && Manager.Instance.Teams [0] [team].Wins / Manager.Instance.Teams [0] [team].Losses < Manager.Instance.Teams [0] [currentClaim].Wins / Manager.Instance.Teams [0] [currentClaim].Losses)
					currentClaim = team;
			}
		}
	}

	// Takes player off waivers
	public void TakeOffWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].TakeOffWaivers (id);
	}

	// Getter
	public int ID
	{
		get
		{
			return id;
		}
	}

	public override string ToString ()
	{
		return id + "," + length + "," + currentClaim + "," + trade;
	}
}
