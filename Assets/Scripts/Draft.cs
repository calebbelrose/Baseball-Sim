using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Draft
{
	public Transform draftList;					// Holds the header and player objects
	public Transform header;					// Header object
	public RectTransform draftListRect;			// RectTransform of the playerList
	public RectTransform draftListParentRect;	// RectTransform of the parent of the player list

	private int currSortedStat = 6;				// Current sorted stat
	private int currIndex = 0;					// Index of the current picking team
	private int initialPlayers;					// Number of players at the start of the draft
	private bool ascending = true;				// Whether it's sorted ascending or descending
	private List<int> pickAscending;			// List of team IDs sorted by pick order
	private List<int> [] newPlayers;			// New players for each team

	public static List<int> draftPlayers;		// All draft players

	// Use this for initialization
	public Draft ()
	{
		string [] lines;

		newPlayers = new List<int>[Manager.Instance.GetNumTeams ()];
		draftPlayers = new List<int> ();

		for (int i = 0; i < newPlayers.Length; i++)
			newPlayers [i] = new List<int> ();

		if (File.Exists (@"Save\DraftPlayers.txt"))
		{
			lines = File.ReadAllLines (@"Save\DraftPlayers.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				Player newPlayer = new Player ();

				newPlayer.LoadPlayer (int.Parse (lines [i]));
				draftPlayers.Add (newPlayer.ID);
				Manager.Instance.NewPlayer (newPlayer);
			}
		}
		else
		{
			StreamWriter sw = new StreamWriter (@"Save\DraftPlayers.txt");

			initialPlayers = (int) (Random.value * 5.0f) + 250;

			for (int i = 0; i < initialPlayers; i++)
			{
				Player newPlayer = new Player (Player.Positions [ (int) (Random.value * Player.Positions.Length)], 16, 9, Manager.Instance.Players.Count);
				newPlayer.DraftCountry ();
				draftPlayers.Add (newPlayer.ID);
				Manager.Instance.NewPlayer (newPlayer);
				newPlayer.SavePlayer ();
				newPlayer.SaveContract ();
				newPlayer.SaveStats ();
				sw.WriteLine (newPlayer.ID);
			}

			sw.Close ();
		}
	}

	// Makes the first selections for the draft
	public void StartDraft ()
	{
		SetPickAscending ();
		Sort (6);
		Display ();

		while (pickAscending [currIndex] != 0)
			DraftPlayer (GameObject.Find ("player" + currIndex), 0);
		
		Manager.Instance.DisplayPlayers (draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	// Sets the pick ascending
	public void SetPickAscending ()
	{
		pickAscending = new List<int> ();
		Team [] teams;

		teams = Manager.Instance.Teams [0].OrderBy (teamX => teamX.Pick).ToArray ();

		for (int i = 0; i < teams.Length; i++)
			pickAscending.Add (teams [i].ID);
	}

	// Sorts the players by the selected stat
	public void Sort (int headerNum)
	{
		bool notString;

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

		draftPlayers = Manager.Instance.Sort (headerNum, ascending, draftPlayers);
		Manager.Instance.DisplayPlayers (draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
		currSortedStat = headerNum;
	}

	// Drafts a player for each team, or until no players are left
	public void PlayerDraft (GameObject player, string playerListing)
	{
		int count;
		int prevSortedStat = currSortedStat;
		bool prevAscending = ascending;

		DraftPlayer (player, int.Parse (player.name.Remove (0, 5)));

		if (currSortedStat != 6 || ascending)
		{
			Sort (6);
			if (Manager.Instance.Teams [0].Count > draftPlayers.Count)
				count = draftPlayers.Count;
			else
				count = Manager.Instance.Teams [0].Count - 1;

			for (int i = 0; i < count; i++)
				DraftPlayer (GameObject.Find ("draft" + i), 0);
		}
		else
		{
			if (Manager.Instance.Teams [0].Count > draftPlayers.Count)
				count = draftPlayers.Count + 1;
			else
				count = Manager.Instance.Teams [0].Count;

			for (int i = 1; i < count; i++)
				DraftPlayer (GameObject.Find ("draft" + i), 0);
		}

		do
		{
			Sort (prevSortedStat);
		}while (currSortedStat != prevSortedStat && ascending != prevAscending);
	}

	// Drafts a player
	public void DraftPlayer (GameObject player, int playerNum)
	{
		newPlayers [pickAscending[currIndex]].Add (draftPlayers [playerNum]);
		draftPlayers.RemoveAt (playerNum);

		if (draftPlayers.Count == 0)
		{
			int numTeams = Manager.Instance.GetNumTeams ();
			StreamWriter sw = new StreamWriter (@"Save\DraftPlayers.txt");

			for (int i = 0; i < numTeams; i++)
			{
				while (newPlayers [i].Count > 0)
				{
					Manager.Instance.Players [newPlayers [i].First ()].Team = i;
					Manager.Instance.Players [newPlayers [i].First ()].SavePlayer ();
					Manager.Instance.Teams [0] [i].DraftPicks.Add (newPlayers [i].First ());
					newPlayers [i].RemoveAt (0);
				}

				Manager.Instance.Teams [0] [i].SetRoster ();
			}
				
			for (int i = 0; i < draftPlayers.Count; i++)
				sw.WriteLine (draftPlayers [i]);


			sw.Close ();
			PlayerPrefs.SetInt ("LongestFirstName", Player.longestFirstName);
			PlayerPrefs.SetInt ("LongestLastName", Player.longestLastName);
			PlayerPrefs.SetString ("NeedDraft", false.ToString ());
			PlayerPrefs.Save ();

			Manager.ChangeToScene (4);
		}

		currIndex = (currIndex + 1) % Manager.Instance.Teams [0].Count;
		Object.Destroy (player);
	}

	// Displays the draft players
	public void Display ()
	{
		Manager.Instance.DisplayHeaders (header, draftListRect, draftListParentRect, DisplayType.Draft);
		Manager.Instance.DisplayPlayers (draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	// Sets the objects to display the draft players
	public void SetDraftPlayerObjects (Transform _draftList, Transform _header, RectTransform _draftListRect, RectTransform _draftListParentRect)
	{
		draftList = _draftList;
		header = _header;
		draftListRect = _draftListRect;
		draftListParentRect = _draftListParentRect;
	}

	// Returns an unsigned player to the draft
	public static void ReturnToDraft (int playerID)
	{
		draftPlayers.Add (playerID);
	}
}