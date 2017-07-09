using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiverPlayer
{
	int id, length, currentClaim;
	List<int> teamsToClaim;

	public int ID
	{
		get
		{
			return id;
		}
	}

	public WaiverPlayer(int _id)
	{
		id = _id;
		length = 10;

		for (int i = 0; i < Manager.Instance.teams.Count; i++)
			teamsToClaim.Add (i);

		teamsToClaim.Remove (Manager.Instance.Players [id].team);
		currentClaim = Manager.Instance.Players [id].team;
	}

	public void AdvanceDay()
	{
		for (int i = 0; i < teamsToClaim.Count; i++)
		{
			if (Manager.Instance.Players [id].overall > Manager.Instance.Players [Manager.Instance.teams [i].Batters [Manager.Instance.teams [i].Positions.IndexOf (Manager.Instance.Players [id].position)] [0]].overall || Manager.Instance.Players[Manager.Instance.teams [i].GetWorstOverall ()].overall > Manager.Instance.Players [Manager.Instance.teams [i].Batters [Manager.Instance.teams [i].Positions.IndexOf (Manager.Instance.Players [id].position)] [0]].overall)
				Claim (teamsToClaim [i]);
		}

		length--;

		if(length == 0)
		{
			TakeOffWaivers ();
			Manager.Instance.teams [Manager.Instance.Players [id].team].TransferPlayer (id);
		}
	}

	public void Claim(int team)
	{
		teamsToClaim.Remove (team);

		if(Manager.Instance.teams [currentClaim].League != Manager.Instance.teams [Manager.Instance.Players [id].team].League)
		{
			if (Manager.Instance.teams [team].League == Manager.Instance.teams [Manager.Instance.Players [id].team].League || Manager.Instance.teams [team].Wins / Manager.Instance.teams [team].Losses < Manager.Instance.teams [currentClaim].Wins / Manager.Instance.teams [currentClaim].Losses)
				currentClaim = team;
		}
		else if (Manager.Instance.teams [team].League == Manager.Instance.teams [Manager.Instance.Players [id].team].League && Manager.Instance.teams [team].Wins / Manager.Instance.teams [team].Losses < Manager.Instance.teams [currentClaim].Wins / Manager.Instance.teams [currentClaim].Losses)
			currentClaim = team;
	}

	public void TakeOffWaivers()
	{
		Manager.Instance.teams [Manager.Instance.Players [id].team].TakeOffWaivers (id);
	}
}
