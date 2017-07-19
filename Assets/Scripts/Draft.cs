using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Draft
{
	List<int> [] newPlayers;
	public static List<int> draftPlayers;
	GameObject manager;
	Transform draftList, header;
	RectTransform draftListRect, draftListParentRect;
	int currSortedStat = 0, currIndex = 0, initialPlayers;
	bool ascending;
	List<int> pickOrder;

	// Use this for initialization
	public Draft ()
	{
		string [] lines;

		newPlayers = new List<int>[Manager.Instance.GetNumTeams()];
		draftPlayers = new List<int> ();

		for (int i = 0; i < newPlayers.Length; i++)
			newPlayers [i] = new List<int>();

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
			StreamWriter sw = new StreamWriter(@"Save\DraftPlayers.txt");

			initialPlayers = (int)(Random.value * 5.0f) + 250;

			for (int i = 0; i < initialPlayers; i++)
			{
				Player newPlayer = new Player(Player.Positions [(int)(Random.value * Player.Positions.Length)], 16, 9, Manager.Instance.Players.Count);
				draftPlayers.Add (newPlayer.ID);
				Manager.Instance.NewPlayer (newPlayer);
				newPlayer.SavePlayer ();
				sw.WriteLine (newPlayer.ID);
			}

			sw.Close ();
		}
	}

	// Makes the first selections for the draft
	public void StartDraft ()
	{
		SetPickOrder();
		Sort(6);
		Display ();

		while (pickOrder [currIndex] != 0)
			DraftPlayer (GameObject.Find ("player" + currIndex), 0);
		
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	// Sets the pick order
	public void SetPickOrder()
	{
		pickOrder = new List<int> ();
		Team [] teams;

		teams = Manager.Instance.Teams [0].OrderBy (teamX => teamX.Pick).ToArray();

		for (int i = 0; i < teams.Length; i++)
			pickOrder.Add (teams [i].ID);
	}

	// Sorts the players by the selected stat
	public void Sort(int headerNum)
	{
		bool notString;

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

		draftPlayers = Manager.Instance.Sort (headerNum, ascending, draftPlayers);
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
		currSortedStat = headerNum;
	}

	// Drafts a player for each team, or until no players are left
	public void PlayerDraft(GameObject player, string playerListing)
	{
		int count;
		int prevSortedStat = currSortedStat;
		bool prevAscending = ascending;

		DraftPlayer(player, int.Parse (player.name.Remove (0, 5)));

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
	public void DraftPlayer(GameObject player, int playerNum)
	{
		newPlayers [pickOrder[currIndex]].Add(draftPlayers [playerNum]);
		draftPlayers.RemoveAt (playerNum);

		if (draftPlayers.Count == 0)
		{
			int numTeams = Manager.Instance.GetNumTeams();
			StreamWriter sw = new StreamWriter (@"Save\DraftPlayers.txt");

			for (int i = 0; i < numTeams; i++)
			{
				while (newPlayers [i].Count > 0)
				{
					Manager.Instance.Players [newPlayers [i].First ()].Team = i;
					Manager.Instance.Players [newPlayers [i].First ()].SavePlayer ();
					Manager.Instance.Teams [0] [i].DraftPicks.Add(newPlayers [i].First ());
					newPlayers [i].RemoveAt(0);
				}

				Manager.Instance.Teams [0] [i].AutomaticRoster ();
			}
				
			for (int i = 0; i < draftPlayers.Count; i++)
				sw.WriteLine (draftPlayers [i]);

			PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
			PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

			sw.Close();

			Manager.ChangeToScene (4);
		}

		currIndex = (currIndex + 1) % Manager.Instance.Teams [0].Count;
		Object.Destroy(player);
	}

	// Displays the draft players
	public void Display()
	{
		Manager.Instance.DisplayHeaders (header, draftListParentRect, DisplayType.Draft);
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	// Sets the objects to display the draft players
	public void SetDraftPlayerObjects(Transform _draftList, Transform _header, RectTransform _draftListRect, RectTransform _draftListParentRect)
	{
		draftList = _draftList;
		header = _header;
		draftListRect = _draftListRect;
		draftListParentRect = _draftListParentRect;
	}

	// Returns an unsigned player to the draft
	public static void ReturnToDraft(int playerID)
	{
		draftPlayers.Add (playerID);
	}
}