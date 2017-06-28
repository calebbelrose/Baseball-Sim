using System.Collections.Generic;

public class PlayerDisplay
{
	UnityEngine.Transform teamList, header;
    AllTeams allTeams;
	UnityEngine.RectTransform teamListRect, teamListParentRect;
    int currSortedStat = 3;
	bool ascending;

	public PlayerDisplay(AllTeams _allTeams)
    {
		allTeams = _allTeams;
    }

	public void Display()
	{
		allTeams.DisplayHeaders(header, teamListParentRect, false);
		Sort (3);
	}

	public void Sort(int headerNum)
	{
		bool notString;
		List<Player> players;

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

		players = allTeams.Sort (headerNum, ascending, allTeams.teams[0].players);
		allTeams.DisplayPlayers (players, teamList, teamListRect, teamListParentRect, false);
	}

	public void SetPlayerDisplayObjects(UnityEngine.Transform _teamList, UnityEngine.Transform _header, UnityEngine.RectTransform _teamListRect, UnityEngine.RectTransform _teamListParentRect)
	{
		teamList = _teamList;
		header = _header;
		teamListRect = _teamListRect;
		teamListParentRect = _teamListParentRect;
	}
}