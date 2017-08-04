using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiverPlayer
{
	private int id, length, currentClaim;
	private List<int> teamsToClaim;

	// 1-Arg Constructor
	public WaiverPlayer (int _id)
	{
		id = _id;
		length = 10;

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			teamsToClaim.Add (i);

		teamsToClaim.Remove (Manager.Instance.Players [id].Team);
		currentClaim = Manager.Instance.Players [id].Team;
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
			Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].Transfer (id, currentClaim);
		}
	}

	// Claims the player
	public void Claim (int team)
	{
		teamsToClaim.Remove (team);

		if (Manager.Instance.Teams [0] [currentClaim].League != Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League)
		{
			if (Manager.Instance.Teams [0] [team].League == Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League || Manager.Instance.Teams [0] [team].Wins / Manager.Instance.Teams [0] [team].Losses < Manager.Instance.Teams [0] [currentClaim].Wins / Manager.Instance.Teams [0] [currentClaim].Losses)
				currentClaim = team;
		}
		else if (Manager.Instance.Teams [0] [team].League == Manager.Instance.Teams [0] [Manager.Instance.Players [id].Team].League && Manager.Instance.Teams [0] [team].Wins / Manager.Instance.Teams [0] [team].Losses < Manager.Instance.Teams [0] [currentClaim].Wins / Manager.Instance.Teams [0] [currentClaim].Losses)
			currentClaim = team;
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
}
