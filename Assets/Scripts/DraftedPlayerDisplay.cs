using System.Collections.Generic;

public class DraftedPlayerDisplay
{
	UnityEngine.Transform playerList, header;
	UnityEngine.RectTransform playerListRect, playerListParentRect;
	UnityEngine.GameObject panel;
	int currSortedStat = 3;
	bool ascending;

	public void Display()
	{
		Manager.Instance.DisplayHeaders(header, playerListParentRect, DisplayType.Signed);
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

		players = Manager.Sort (headerNum, ascending, Manager.Instance.teams[0].DraftPicks);
		Manager.Instance.DisplayPlayers (players, playerList, playerListRect, playerListParentRect, DisplayType.Signed);
	}

	public void ShowDraftedPlayer(int id)
	{
		panel.SetActive (true);
		panel.GetComponent<DisplayDraftPlayer> ().SetPlayerID (id);
	}

	public void SetPlayerDisplayObjects(UnityEngine.Transform _playerList, UnityEngine.Transform _header, UnityEngine.RectTransform _playerListRect, UnityEngine.RectTransform _playerListParentRect, UnityEngine.GameObject _panel)
	{
		playerList = _playerList;
		header = _header;
		playerListRect = _playerListRect;
		playerListParentRect = _playerListParentRect;
		panel = _panel;
	}
}