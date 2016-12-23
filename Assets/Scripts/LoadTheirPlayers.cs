using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadTheirPlayers : MonoBehaviour {

    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    int currSortedStat = 3;
    bool ascending = true;
    int theirTeam;
    Trade trade;
	public List<Player> theirPlayers;

    void Start()
    {
        teamList = GameObject.Find("TheirList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        trade = GameObject.Find("btnOffer").GetComponent<Trade>();
		theirPlayers = new List<Player>();

        theirTeam = 1;
        DisplayHeader();
		Sort (3);
    }

    public void Refresh(Dropdown dropdown)
    {
        teamList = GameObject.Find("TheirList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();
        theirTeam = dropdown.value + 1;
		Sort (currSortedStat);
    }

	void DisplayHeader()
	{
		int statHeaderLength = 0;
		GameObject teamListHeader = GameObject.Find("TheirListHeader");

		int[] headerLengths = new int[allTeams.stats.Length];

		for (int i = 2; i < allTeams.stats.Length; i++)
		{
			headerLengths [i] = allTeams.stats [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;

		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		Object header = Resources.Load("Header", typeof(GameObject));
		float prevWidth = 5.0f, newWidth = 0.0f;
		float totalWidth = (8.04f * (statHeaderLength + 1.0f));
		teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (allTeams.teams[theirTeam].players.Count + 1) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
		teamList.GetComponent<RectTransform>().offsetMax = new Vector2(totalWidth - 160.0f, 0);
		totalWidth /= -2.0f;

		for (int i = 0; i < allTeams.stats.Length; i++)
		{
			GameObject statHeader = Instantiate(header) as GameObject;
			statHeader.name = "header" + i.ToString();
			statHeader.transform.SetParent(teamListHeader.transform);
			statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = allTeams.stats[i];
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
			string allTeamsing = theirPlayers [i].firstName;

			for (int j = theirPlayers [i].firstName.Length; j < Player.longestFirstName; j++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].lastName;

			for (int j = theirPlayers [i].lastName.Length; j < Player.longestLastName; j++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].position;

			for (int k = theirPlayers [i].position.Length; k < allTeams.stats [2].Length; k++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].overall;

			for (int k = theirPlayers [i].overall.ToString ().Length; k < allTeams.stats [3].Length; k++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].offense;

			for (int k = theirPlayers [i].offense.ToString ().Length; k < allTeams.stats [4].Length; k++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].defense;

			for (int k = theirPlayers [i].defense.ToString ().Length; k < allTeams.stats [5].Length; k++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].potential;

			for (int k = theirPlayers [i].potential.ToString ().Length; k < allTeams.stats [6].Length; k++)
				allTeamsing += " ";

			allTeamsing += " " + theirPlayers [i].age;

			for (int k = theirPlayers [i].age.ToString ().Length; k < allTeams.stats [7].Length; k++)
				allTeamsing += " ";

			for (int j = 0; j < theirPlayers[i].skills.Length - 1; j++) {
				allTeamsing += " " + theirPlayers [i].skills [j];

				for (int k = theirPlayers [i].skills [j].ToString ().Length; k < allTeams.stats [j + 8].Length; k++)
					allTeamsing += " ";
			}

			allTeamsing += " " + theirPlayers [i].skills[theirPlayers[i].skills.Length - 1];
			newPlayer.transform.GetChild (0).gameObject.GetComponent<Text> ().text = allTeamsing;
			newPlayer.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			TradePlayerInfo tradeInfo = newPlayer.GetComponent<TradePlayerInfo> ();
			tradeInfo.teamNum = theirTeam;
			tradeInfo.playerNum = i;
			if (trade.theirTrades.Contains (i))
				tradeInfo.ChangeButtonColour ();
		}
	}

	public void StartSorting (GameObject other)
	{
		Sort(int.Parse (other.name.Remove (0, 6)));
	}

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
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.position).ToList ();
				break;
			case 3:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.overall).ToList ();
				break;
			case 4:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.offense).ToList ();
				break;
			case 5:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.defense).ToList ();
				break;
			case 6:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.potential).ToList ();
				break;
			case 7:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.age).ToList ();
				break;
			default:
				theirPlayers = allTeams.teams [theirTeam].players.OrderBy (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum) {
			case 0:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.position).ToList ();
				break;
			case 3:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.overall).ToList ();
				break;
			case 4:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.offense).ToList ();
				break;
			case 5:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.defense).ToList ();
				break;
			case 6:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.potential).ToList ();
				break;
			case 7:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.age).ToList ();
				break;
			default:
				theirPlayers = allTeams.teams [theirTeam].players.OrderByDescending (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}

		DisplayPlayers ();
	}
}
