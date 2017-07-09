using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Draft
{
    List<int>[] newPlayers;
	public static List<int> draftPlayers;
	GameObject manager;
	Transform draftList, header;
    RectTransform draftListRect, draftListParentRect;
	int currSortedStat = 0, currIndex = 0, initialPlayers;
	bool ascending;
	List<int> pickOrder;

    // Use this for initialization
	public Draft()
	{
		int index = 0;

        newPlayers = new List<int>[Manager.Instance.GetNumTeams()];
		draftPlayers = new List<int> ();

        for (int i = 0; i < newPlayers.Length; i++)
            newPlayers[i] = new List<int>();

		while (PlayerPrefs.HasKey("Draft" + index))
        {
			Player newPlayer = new Player();

			newPlayer.LoadPlayer (PlayerPrefs.GetInt("Draft" + index++));
			draftPlayers.Add (newPlayer.ID);
        }

		if (draftPlayers.Count == 0)
        {
			initialPlayers = (int)(Random.value * 5.0f) + 250;

			for (int i = 0; i < initialPlayers; i++)
            {
				Player newPlayer = new Player(Player.Positions[(int)(Random.value * Player.Positions.Length)], 16, 9, Manager.Instance.Players.Count);
				draftPlayers.Add (newPlayer.ID);
				Manager.Instance.Players.Add (newPlayer);
				newPlayer.SetDraftYear (Manager.Instance.Year);
				newPlayer.SavePlayer ();
				PlayerPrefs.SetInt ("Draft" + i.ToString(), newPlayer.ID);
            }
        }
        PlayerPrefs.Save();
    }

	public void StartDraft()
	{
		SetPickOrder();
		Sort(3);
		Display ();

		while (pickOrder [currIndex] != 0)
			DraftPlayer (GameObject.Find ("player" + currIndex), 0);
		
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	public void SetPickOrder()
	{
		pickOrder = new List<int> ();
		Team[] teams = new Team[Manager.Instance.teams.Count];

		Manager.Instance.teams.CopyTo (teams);
		teams = teams.OrderBy (teamX => teamX.Pick).ToArray();

		for (int i = 0; i < teams.Length; i++)
			pickOrder.Add (teams [i].id);
	}

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

		draftPlayers = Manager.Sort (headerNum, ascending, draftPlayers);
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
		currSortedStat = headerNum;
	}

	// Drafts a player for each team, or until no players are left
    public void PlayerDraft(GameObject player, string playerListing)
    {
        int count;
		int prevSortedStat = currSortedStat;
		bool prevAscending = ascending;

		DraftPlayer(player, int.Parse(player.name.Remove(0, 5)));

		if (currSortedStat != 3 || ascending)
		{
			Sort (3);
			if (Manager.Instance.teams.Count > draftPlayers.Count)
				count = draftPlayers.Count;
			else
				count = Manager.Instance.teams.Count - 1;

			for (int i = 0; i < count; i++)
				DraftPlayer (GameObject.Find ("draft" + i), 0);
		}
		else
		{
			if (Manager.Instance.teams.Count > draftPlayers.Count)
				count = draftPlayers.Count + 1;
			else
				count = Manager.Instance.teams.Count;

			for (int i = 1; i < count; i++)
				DraftPlayer (GameObject.Find ("draft" + i), 0);
		}

		do
		{
			Sort (prevSortedStat);
		}while (currSortedStat != prevSortedStat && ascending != prevAscending);
    }

	// Draft a player
    public void DraftPlayer(GameObject player, int playerNum)
    {
		newPlayers[pickOrder[currIndex]].Add(draftPlayers[playerNum]);
		draftPlayers.RemoveAt (playerNum);

		if (draftPlayers.Count == 0)
        {
            int numTeams = Manager.Instance.GetNumTeams();

            for (int i = 0; i < numTeams; i++)
            {
                while (newPlayers[i].Count > 0)
                {
					Manager.Instance.Players[newPlayers[i].First()].team = i;
					Manager.Instance.Players[newPlayers[i].First()].SavePlayer ();
					Manager.Instance.teams[i].DraftPicks.Add(newPlayers[i].First());
                    newPlayers[i].RemoveAt(0);
				}

				Manager.Instance.teams [i].AutomaticRoster ();
                PlayerPrefs.SetInt("NumPlayers" + i, Manager.Instance.teams[i].players.Count);
            }

			for (int i = 0; i < initialPlayers - draftPlayers.Count; i++)
                PlayerPrefs.DeleteKey("Draft" + i.ToString());

			for(int i = 0; i < draftPlayers.Count; i++)
				PlayerPrefs.SetInt ("Draft" + i.ToString(), Manager.Instance.Players[draftPlayers[i]].ID);

			PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
			PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

            Manager.Instance.needDraft = false;
            PlayerPrefs.SetString("NeedDraft", Manager.Instance.needDraft.ToString());
            PlayerPrefs.Save();

			Manager.ChangeToScene(4);
        }

		currIndex = (currIndex + 1) % Manager.Instance.teams.Count;
		Object.Destroy(player);
    }

	public void Display()
	{
		Manager.Instance.DisplayHeaders (header, draftListParentRect, DisplayType.Draft);
		Manager.Instance.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, DisplayType.Draft);
	}

	public void SetDraftPlayerObjects(Transform _draftList, Transform _header, RectTransform _draftListRect, RectTransform _draftListParentRect)
	{
		draftList = _draftList;
		header = _header;
		draftListRect = _draftListRect;
		draftListParentRect = _draftListParentRect;
	}

	public static void ReturnToDraft(int playerID)
	{
		draftPlayers.Add (playerID);
	}
}