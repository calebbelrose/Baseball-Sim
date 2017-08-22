using System.Collections.Generic;

public class InternationalFreeAgentDisplay
{
	public UnityEngine.Transform playerList;				// Holds the header and player objects
	public UnityEngine.Transform header;					// Header object
	public UnityEngine.RectTransform playerListRect;		// RectTransform of the playerList
	public UnityEngine.RectTransform playerListParentRect;	// RectTransform of the parent of the player list
	public UnityEngine.GameObject panel;					// Panel used for displaying a selected player

	private int currSortedStat = 6;							// Current sorted stat
	private bool ascending = true;							// Whether it's sorted ascending or descending

	// Displays the International free agents
	public void Display ()
	{
		Manager.Instance.DisplayHeaders (header, playerListRect, playerListParentRect, DisplayType.InternationalFreeAgent);
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
			ascending = !ascending;
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
