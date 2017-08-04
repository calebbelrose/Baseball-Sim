using System.Collections.Generic;

public class PlayerDisplay
{
	UnityEngine.Transform teamList, header;
	UnityEngine.RectTransform teamListRect, teamListParentRect;
	int currSortedStat = 6;
	bool ascending;

	// Displays players
	public void Display ()
	{
		Manager.Instance.DisplayHeaders (header, teamListParentRect, DisplayType.Team);
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
		{
			if (ascending)
				ascending = false;
			else
				ascending = true;
		}
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