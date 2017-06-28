using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Draft
{
    List<Player>[] newPlayers;
	public static List<Player> draftPlayers;
	GameObject manager;
	Transform draftList, header;
    RectTransform draftListRect, draftListParentRect;
    AllTeams allTeams;
	int currSortedStat = 0, currIndex = 0, initialPlayers;
	bool ascending;
	string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
	List<int> pickOrder;

    // Use this for initialization
	public Draft(AllTeams _allTeams)
	{
		allTeams = _allTeams;
        newPlayers = new List<Player>[allTeams.GetNumTeams()];
		draftPlayers = new List<Player> ();

        for (int i = 0; i < newPlayers.Length; i++)
            newPlayers[i] = new List<Player>();

		while (PlayerPrefs.HasKey("Draft" + draftPlayers.Count.ToString()))
        {
			string playerString = PlayerPrefs.GetString("Draft" + draftPlayers.Count);
			Player newPlayer = new Player();
			string[] splitPlayer;
			int current;

			splitPlayer = playerString.Split(',');

			newPlayer.firstName  = splitPlayer [0];
			newPlayer.lastName  = splitPlayer [1];
			newPlayer.position = splitPlayer [2];
			newPlayer.potential = int.Parse(splitPlayer [3]);
			newPlayer.age  = int.Parse(splitPlayer [4]);
			newPlayer.potential = int.Parse(splitPlayer [5]);
			current = 6;
			for(int i = 0; i < newPlayer.skills.Length; i++)
				newPlayer.skills [i] = int.Parse(splitPlayer [current++]);
			newPlayer.offense = float.Parse(splitPlayer[current++]);
			newPlayer.defense = float.Parse(splitPlayer[current++]);
			newPlayer.overall= float.Parse(splitPlayer[current++]);
			newPlayer.salary = double.Parse(splitPlayer[current++]);
			newPlayer.contractLength = int.Parse(splitPlayer[current++]);
			newPlayer.injuryLength = int.Parse(splitPlayer[current++]);
			newPlayer.country = splitPlayer[current];
			draftPlayers.Add (newPlayer);
        }

		if (draftPlayers.Count == 0)
        {
			initialPlayers = (int)(Random.value * 5.0f) + 250;

			for (int i = 0; i < initialPlayers; i++)
            {
				Player newPlayer = new Player(positions[(int)(Random.value * positions.Length)], 18, 7);
				draftPlayers.Add (newPlayer);
				PlayerPrefs.SetString ("Draft" + i.ToString(), newPlayer.firstName + "," + newPlayer.lastName + "," + newPlayer.position + "," + newPlayer.potential + "," + newPlayer.age + "," + newPlayer.potential + "," + newPlayer.skills [0] + "," + newPlayer.skills [1] + "," + newPlayer.skills [2] + "," + newPlayer.skills [3] + "," + newPlayer.skills [4] + "," + newPlayer.skills [5] + "," + newPlayer.skills [6] + "," + newPlayer.skills [7] + "," + newPlayer.skills [8] + "," + newPlayer.skills [9] + "," + newPlayer.offense + "," + newPlayer.defense + "," + newPlayer.overall + "," + newPlayer.salary + "," + newPlayer.contractLength + "," + newPlayer.injuryLength + "," + newPlayer.country);
            }
        }
        PlayerPrefs.Save();
    }

	public void StartDraft()
	{
		Sort(3);
		SetPickOrder();
		Display ();

		while (pickOrder [currIndex] != 0)
			DraftPlayer (GameObject.Find ("player" + currIndex), 0);
	}

	public void SetPickOrder()
	{
		pickOrder = new List<int> ();
		Team[] teams = new Team[allTeams.teams.Count];

		allTeams.teams.CopyTo (teams);
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

		draftPlayers = allTeams.Sort (headerNum, ascending, draftPlayers);
		allTeams.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, true);
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
			if (allTeams.teams.Count > draftPlayers.Count)
				count = draftPlayers.Count - 1;
			else
				count = allTeams.teams.Count - 1;

			for (int i = 0; i < count; i++)
				DraftPlayer (GameObject.Find ("draft" + i), 0);
		}
		else
		{
			if (allTeams.teams.Count > draftPlayers.Count)
				count = draftPlayers.Count;
			else
				count = allTeams.teams.Count;

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
		if (pickOrder [currIndex] == 0)
			Debug.Log (playerNum + " " + draftPlayers [playerNum].Name);
		newPlayers[pickOrder[currIndex]].Add(draftPlayers[playerNum]);
		draftPlayers.RemoveAt (playerNum);

		if (pickOrder [currIndex] == 0)
		{
			Player test = newPlayers [0] [newPlayers [0].Count - 1];
			Debug.Log (test.Name);
		}

		if (draftPlayers.Count == 0)
        {
            int numTeams = allTeams.GetNumTeams();

            for (int i = 0; i < numTeams; i++)
            {
				Debug.Log (newPlayers[i].Count);
                while (newPlayers[i].Count > 0)
                {
                    Player newPlayer = newPlayers[i].First();
					newPlayer.PlayerIndex = allTeams.teams [i].players.Count;
					newPlayer.SavePlayer (i);
                    allTeams.teams[i].players.Add(newPlayer);
                    newPlayers[i].RemoveAt(0);
				}

                PlayerPrefs.SetInt("NumPlayers" + i, allTeams.teams[i].players.Count);
            }

            for (int i = 0; i < initialPlayers; i++)
                PlayerPrefs.DeleteKey("Draft" + i.ToString());

			PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
			PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

            allTeams.needDraft = false;
            PlayerPrefs.SetString("NeedDraft", allTeams.needDraft.ToString());
            PlayerPrefs.Save();

            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(4);
        }

		currIndex = (currIndex + 1) % allTeams.teams.Count;
		Object.Destroy(player);
    }

	public void Display()
	{
		allTeams.DisplayHeaders (header, draftListParentRect, true);
		allTeams.DisplayPlayers(draftPlayers, draftList, draftListRect, draftListParentRect, true);
	}

	public void SetDraftPlayerObjects(Transform _draftList, Transform _header, RectTransform _draftListRect, RectTransform _draftListParentRect)
	{
		draftList = _draftList;
		header = _header;
		draftListRect = _draftListRect;
		draftListParentRect = _draftListParentRect;
	}
}