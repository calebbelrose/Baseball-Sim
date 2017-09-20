using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftedPlayerDisplay : MonoBehaviour
{
	public Transform playerList;				// Holds the header and player objects
	public Transform header;					// Header object
	public RectTransform playerListRect;		// RectTransform of the playerList
	public RectTransform playerListParentRect;	// RectTransform of the parent of the player list
	public GameObject panel;					// Panel used for displaying a selected player

	private int currSortedStat = 6;				// Current sorted stat
	private bool ascending = true;				// Whether it's sorted ascending or descending
	private List <int> yourPlayers;				// Your draft players
	Object playerButton;						// Player button

	void Start ()
	{
		playerButton = Resources.Load ("Player", typeof(GameObject));
		yourPlayers = new List<int> ();
		playerListRect.sizeDelta = new Vector2 (Manager.DisplayHeaders ((GameObject) => StartSorting(GameObject), header), 20 * (Manager.Instance.Teams [0] [0].DraftPicks.Count + 1) - playerListParentRect.rect.height);
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].DraftPicks);
		DisplayPlayers ();
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < yourPlayers.Count; i++)
			Manager.DisplayPlayer (playerButton, transform, yourPlayers [i]).GetComponent<Button> ().onClick.AddListener (() => ShowDraftedPlayer (yourPlayers [i]));
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
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].DraftPicks);
		DisplayPlayers ();
	}

	// Shows a drafted player's stats
	public void ShowDraftedPlayer (int id)
	{
		panel.SetActive (true);
		panel.GetComponent<DisplayPlayer> ().SetPlayerID (id);
	}
}