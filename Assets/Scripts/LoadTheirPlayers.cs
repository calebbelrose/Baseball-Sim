using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadTheirPlayers : MonoBehaviour
{
	public GameObject teamList, teamListHeader;
	int currSortedStat = 3;
	bool ascending = true;
	int theirTeam;
	public Trade trade;
	public List<int> theirPlayers;

	void Start ()
	{
		theirPlayers = new List<int> ();
		theirTeam = 1;
		DisplayHeader ();
		Sort (3);
	}

	// Refreshes players
	public void Refresh (Dropdown dropdown)
	{
		theirTeam = dropdown.value + 1;
		Sort (currSortedStat);
	}

	// Displays header
	void DisplayHeader ()
	{
		int statHeaderLength = 0;

		int [] headerLengths = new int [Manager.Instance.Skills.Length];

		for (int i = 2; i < Manager.Instance.Skills.Length; i++)
		{
			headerLengths [i] = Manager.Instance.Skills [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;

		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		Object header = Resources.Load ("Header", typeof (GameObject));
		float prevWidth = 5.0f, newWidth = 0.0f;
		float totalWidth = (8.03f * (statHeaderLength + 1.0f));
		teamList.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, - (20 * (Manager.Instance.Teams [0] [theirTeam].Players.Count + 1) - teamList.transform.parent.gameObject.GetComponent<RectTransform> ().rect.height));
		teamList.GetComponent<RectTransform> ().offsetMax = new Vector2 (totalWidth - 160.0f, 0);
		totalWidth /= -2.0f;

		for (int i = 0; i < Manager.Instance.Skills.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (teamListHeader.transform);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = Manager.Instance.Skills [i];
			statHeader.GetComponent<Button> ().onClick.AddListener (() => StartSorting (statHeader));

			float currWidth = (8.03f * headerLengths [i]);
			newWidth += currWidth;
			totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
			prevWidth = currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
			statHeader.GetComponent<RectTransform> ().transform.localPosition = new Vector3 (totalWidth, 0.0f, 0.0f);
		}

		teamList.GetComponent<RectTransform> ().offsetMax = new Vector2 (newWidth - 160.0f, 0);
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("TheirPlayer");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < theirPlayers.Count; i++)
		{
			Object playerButton = Resources.Load ("TheirPlayer", typeof (GameObject));
			GameObject newPlayer = Instantiate (playerButton) as GameObject;
			newPlayer.name = "player" + i.ToString ();
			newPlayer.transform.SetParent (teamList.transform);
			string playerString = Manager.Instance.Players [theirPlayers [i]].FirstName;

			for (int j = Manager.Instance.Players [theirPlayers [i]].FirstName.Length; j < Player.longestFirstName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].LastName;

			for (int j = Manager.Instance.Players [theirPlayers [i]].LastName.Length; j < Player.longestLastName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Position;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Position.Length; k < Manager.Instance.Skills [2].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Overall;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Overall.ToString ().Length; k < Manager.Instance.Skills [3].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Offense;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Offense.ToString ().Length; k < Manager.Instance.Skills [4].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Defense;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Defense.ToString ().Length; k < Manager.Instance.Skills [5].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Potential;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Potential.ToString ().Length; k < Manager.Instance.Skills [6].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Age;

			for (int k = Manager.Instance.Players [theirPlayers [i]].Age.ToString ().Length; k < Manager.Instance.Skills [7].Length; k++)
				playerString += " ";

			for (int j = 0; j < Manager.Instance.Players [theirPlayers [i]].Skills.Length - 1; j++)
			{
				playerString += " " + Manager.Instance.Players [theirPlayers [i]].Skills [j];

				for (int k = Manager.Instance.Players [theirPlayers [i]].Skills [j].ToString ().Length; k < Manager.Instance.Skills [j + 8].Length; k++)
					playerString += " ";
			}

			playerString += " " + Manager.Instance.Players [theirPlayers [i]].Skills [Manager.Instance.Players [theirPlayers [i]].Skills.Length - 1];
			newPlayer.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerString;
			newPlayer.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			TradePlayerInfo tradeInfo = newPlayer.GetComponent<TradePlayerInfo> ();
			tradeInfo.teamNum = theirTeam;
			tradeInfo.playerNum = i;
			if (trade.theirTrades.Contains (i))
				tradeInfo.ChangeButtonColour ();
		}
	}

	// Starts Sorting players
	public void StartSorting (GameObject other)
	{
		Sort (int.Parse (other.name.Remove (0, 6)));
	}

	// Sorts players
	void Sort (int headerNum)
	{
		bool notString;

		if (headerNum <= 2)
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

		if (ascending)
			switch (headerNum)
		{
			case 0:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].FirstName).ToList ();
				break;
			case 1:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].LastName).ToList ();
				break;
			case 2:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Position).ToList ();
				break;
			case 3:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Overall).ToList ();
				break;
			case 4:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Offense).ToList ();
				break;
			case 5:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Defense).ToList ();
				break;
			case 6:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Potential).ToList ();
				break;
			case 7:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Age).ToList ();
				break;
			default:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
		{
			case 0:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].FirstName).ToList ();
				break;
			case 1:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].LastName).ToList ();
				break;
			case 2:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Position).ToList ();
				break;
			case 3:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Overall).ToList ();
				break;
			case 4:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Offense).ToList ();
				break;
			case 5:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Defense).ToList ();
				break;
			case 6:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Potential).ToList ();
				break;
			case 7:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Age).ToList ();
				break;
			default:
				theirPlayers = Manager.Instance.Teams [0] [theirTeam].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}

		DisplayPlayers ();
	}
}
