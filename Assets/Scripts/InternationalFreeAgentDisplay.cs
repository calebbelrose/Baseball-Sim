﻿using System.Collections.Generic;

public class InternationalFreeAgentDisplay
{
	UnityEngine.Transform playerList, header;
	UnityEngine.RectTransform playerListRect, playerListParentRect;
	UnityEngine.GameObject panel;
	int currSortedStat = 6;
	bool ascending;

	// Displays the International free agents
	public void Display ()
	{
		Manager.Instance.DisplayHeaders (header, playerListParentRect, DisplayType.InternationalFreeAgent);
		Sort (6);
	}

	// Sorts the International free agents
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

		players = Manager.Instance.Sort (headerNum, ascending, Manager.Instance.FreeAgents);
		Manager.Instance.DisplayPlayers (players, playerList, playerListRect, playerListParentRect, DisplayType.InternationalFreeAgent);
	}

	// Shows an International free agent's stats
	public void ShowInternationalFreeAgent (int id)
	{
		panel.SetActive (true);
		panel.GetComponent<DisplayPlayer> ().SetPlayerID (id);
	}

	// Sets the objects to display the International free agents
	public void SetPlayerDisplayObjects (UnityEngine.Transform _playerList, UnityEngine.Transform _header, UnityEngine.RectTransform _playerListRect, UnityEngine.RectTransform _playerListParentRect, UnityEngine.GameObject _panel)
	{
		playerList = _playerList;
		header = _header;
		playerListRect = _playerListRect;
		playerListParentRect = _playerListParentRect;
		panel = _panel;
	}
}
