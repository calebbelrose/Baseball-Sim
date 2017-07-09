using System.Collections.Generic;

public class PlayerDisplay
{
	UnityEngine.Transform teamList, header;
	UnityEngine.RectTransform teamListRect, teamListParentRect;
    int currSortedStat = 3;
	bool ascending;

	public void Display()
	{
		Manager.Instance.DisplayHeaders(header, teamListParentRect, DisplayType.Team);
		Sort (3);
	}

	public void Sort(int headerNum)
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

		players = Manager.Sort (headerNum, ascending, Manager.Instance.teams[0].players);
		Manager.Instance.DisplayPlayers (players, teamList, teamListRect, teamListParentRect, DisplayType.Team);
		currSortedStat = headerNum;
	}

	public void SetPlayerDisplayObjects(UnityEngine.Transform _teamList, UnityEngine.Transform _header, UnityEngine.RectTransform _teamListRect, UnityEngine.RectTransform _teamListParentRect)
	{
		teamList = _teamList;
		header = _header;
		teamListRect = _teamListRect;
		teamListParentRect = _teamListParentRect;
	}
}