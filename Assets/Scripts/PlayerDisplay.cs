using System.Collections.Generic;

public class PlayerDisplay
{
	public UnityEngine.Transform teamList;					// Holds the header and player objects
	public UnityEngine.Transform header;					// Header object
	public UnityEngine.RectTransform teamListRect;			// RectTransform of the playerList
	public UnityEngine.RectTransform teamListParentRect;	// RectTransform of the parent of the player list

	private int currSortedStat = 6;							// Current sorted stat
	private bool ascending = true;							// Whether it's sorted ascending or descending

	// Displays players
	public void Display ()
	{
		Manager.Instance.DisplayHeaders (header, teamListRect, teamListParentRect, DisplayType.Team);
		Sort (6);
	}

	// Sorts players
	public void Sort (int headerNum)
	{
		bool notString;
		List<int> players;

		if (headerNum <= 1)
			notString = false;
		else
			notString = true;

		if (currSortedStat == headerNum)
			ascending = !ascending;
		else if (notString)
			ascending = false;
		else
			ascending = true;

		players = Manager.Instance.Sort (headerNum, ascending, Manager.Instance.Teams [0] [0].Players);
		Manager.Instance.DisplayPlayers (players, teamList, teamListRect, teamListParentRect, DisplayType.Team);
		currSortedStat = headerNum;
	}

	// Sets objects for displaying players
	public void SetPlayerDisplayObjects (UnityEngine.Transform _teamList, UnityEngine.Transform _header, UnityEngine.RectTransform _teamListRect, UnityEngine.RectTransform _teamListParentRect)
	{
		teamList = _teamList;
		header = _header;
		teamListRect = _teamListRect;
		teamListParentRect = _teamListParentRect;
	}
}