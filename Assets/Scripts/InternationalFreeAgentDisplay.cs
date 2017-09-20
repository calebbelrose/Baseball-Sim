using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InternationalFreeAgentDisplay : MonoBehaviour
{
	public UnityEngine.Transform playerList;				// Holds the header and player objects
	public UnityEngine.Transform header;					// Header object
	public UnityEngine.RectTransform playerListRect;		// RectTransform of the playerList
	public UnityEngine.RectTransform playerListParentRect;	// RectTransform of the parent of the player list
	public UnityEngine.GameObject panel;					// Panel used for displaying a selected player

	private int currSortedStat = 6;							// Current sorted stat
	private bool ascending = true;							// Whether it's sorted ascending or descending
	private List<int> freeAgents;							// International free agents
	Object playerButton;									// Player button

	void Start ()
	{
		playerButton = Resources.Load ("Player", typeof(GameObject));
		freeAgents = new List<int> ();
		playerListRect.sizeDelta = new Vector2 (Manager.DisplayHeaders ((GameObject) => StartSorting(GameObject), header), 20 * (Manager.Instance.InternationalFreeAgents.Count + 1) - playerListParentRect.rect.height);
		freeAgents = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.InternationalFreeAgents);
		DisplayPlayers ();
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < freeAgents.Count; i++)
			Manager.DisplayPlayer (playerButton, transform, freeAgents [i]).GetComponent<Button> ().onClick.AddListener (() => ShowInternationalFreeAgent (freeAgents [i]));
	}

	// Starts sorting players
	public void StartSorting (GameObject other)
	{
		bool notString;
		int headerNum = int.Parse (other.name.Remove (0, 6));

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

		currSortedStat = headerNum;
		freeAgents = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.InternationalFreeAgents);
		DisplayPlayers ();
	}

	// Shows an International free agent's stats
	public void ShowInternationalFreeAgent (int id)
	{
		panel.SetActive (true);
		panel.GetComponent<DisplayPlayer> ().SetPlayerID (id);
	}
}
