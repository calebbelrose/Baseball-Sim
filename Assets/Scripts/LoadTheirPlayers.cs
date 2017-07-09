using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadTheirPlayers : MonoBehaviour {

    GameObject teamList;
    int currSortedStat = 3;
    bool ascending = true;
    int theirTeam;
    Trade trade;
	public List<int> theirPlayers;

    void Start()
    {
        teamList = GameObject.Find("TheirList");
        trade = GameObject.Find("btnOffer").GetComponent<Trade>();
		theirPlayers = new List<int>();

        theirTeam = 1;
        DisplayHeader();
		Sort (3);
    }

	// Refreshes players
    public void Refresh(Dropdown dropdown)
    {
        teamList = GameObject.Find("TheirList");
        theirTeam = dropdown.value + 1;
		Sort (currSortedStat);
    }

	// Displays header
	void DisplayHeader()
	{
		int statHeaderLength = 0;
		GameObject teamListHeader = GameObject.Find("TheirListHeader");

		int[] headerLengths = new int[Manager.Instance.stats.Length];

		for (int i = 2; i < Manager.Instance.stats.Length; i++)
		{
			headerLengths [i] = Manager.Instance.stats [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;

		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		Object header = Resources.Load("Header", typeof(GameObject));
		float prevWidth = 5.0f, newWidth = 0.0f;
		float totalWidth = (8.04f * (statHeaderLength + 1.0f));
		teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (Manager.Instance.teams[theirTeam].players.Count + 1) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
		teamList.GetComponent<RectTransform>().offsetMax = new Vector2(totalWidth - 160.0f, 0);
		totalWidth /= -2.0f;

		for (int i = 0; i < Manager.Instance.stats.Length; i++)
		{
			GameObject statHeader = Instantiate(header) as GameObject;
			statHeader.name = "header" + i.ToString();
			statHeader.transform.SetParent(teamListHeader.transform);
			statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = Manager.Instance.stats[i];
			statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader));

			float currWidth = (8.04f * headerLengths[i]);
			newWidth += currWidth;
			totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
			prevWidth = currWidth;
			statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
			statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
		}

		teamList.GetComponent<RectTransform>().offsetMax = new Vector2(newWidth - 160.0f, 0);
	}

	// Displays players
	public void DisplayPlayers()
	{
		GameObject[] currPlayers = GameObject.FindGameObjectsWithTag ("TheirPlayer");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < theirPlayers.Count; i++) {
			Object playerButton = Resources.Load ("TheirPlayer", typeof(GameObject));
			GameObject newPlayer = Instantiate (playerButton) as GameObject;
			newPlayer.name = "player" + i.ToString ();
			newPlayer.transform.SetParent (teamList.transform);
			string playerString = Manager.Instance.Players[theirPlayers [i]].firstName;

			for (int j = Manager.Instance.Players[theirPlayers [i]].firstName.Length; j < Player.longestFirstName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].lastName;

			for (int j = Manager.Instance.Players[theirPlayers [i]].lastName.Length; j < Player.longestLastName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].position;

			for (int k = Manager.Instance.Players[theirPlayers [i]].position.Length; k < Manager.Instance.stats [2].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].overall;

			for (int k = Manager.Instance.Players[theirPlayers [i]].overall.ToString ().Length; k < Manager.Instance.stats [3].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].offense;

			for (int k = Manager.Instance.Players[theirPlayers [i]].offense.ToString ().Length; k < Manager.Instance.stats [4].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].defense;

			for (int k = Manager.Instance.Players[theirPlayers [i]].defense.ToString ().Length; k < Manager.Instance.stats [5].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].potential;

			for (int k = Manager.Instance.Players[theirPlayers [i]].potential.ToString ().Length; k < Manager.Instance.stats [6].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].age;

			for (int k = Manager.Instance.Players[theirPlayers [i]].age.ToString ().Length; k < Manager.Instance.stats [7].Length; k++)
				playerString += " ";

			for (int j = 0; j < Manager.Instance.Players[theirPlayers [i]].skills.Length - 1; j++) {
				playerString += " " + Manager.Instance.Players[theirPlayers [i]].skills [j];

				for (int k = Manager.Instance.Players[theirPlayers [i]].skills [j].ToString ().Length; k < Manager.Instance.stats [j + 8].Length; k++)
					playerString += " ";
			}

			playerString += " " + Manager.Instance.Players[theirPlayers [i]].skills[Manager.Instance.Players[theirPlayers [i]].skills.Length - 1];
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
		Sort(int.Parse (other.name.Remove (0, 6)));
	}

	// Sorts players
	void Sort(int headerNum)
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
			switch (headerNum) {
			case 0:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].firstName).ToList ();
				break;
			case 1:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].lastName).ToList ();
				break;
			case 2:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].position).ToList ();
				break;
			case 3:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].overall).ToList ();
				break;
			case 4:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].offense).ToList ();
				break;
			case 5:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].defense).ToList ();
				break;
			case 6:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].potential).ToList ();
				break;
			case 7:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].age).ToList ();
				break;
			default:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderBy (playerX => Manager.Instance.Players[playerX].skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum) {
			case 0:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].firstName).ToList ();
				break;
			case 1:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].lastName).ToList ();
				break;
			case 2:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].position).ToList ();
				break;
			case 3:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].overall).ToList ();
				break;
			case 4:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].offense).ToList ();
				break;
			case 5:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].defense).ToList ();
				break;
			case 6:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].potential).ToList ();
				break;
			case 7:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].age).ToList ();
				break;
			default:
				theirPlayers = Manager.Instance.teams [theirTeam].players.OrderByDescending (playerX => Manager.Instance.Players[playerX].skills [headerNum - 8]).ToList ();
				break;
			}

		DisplayPlayers ();
	}
}
