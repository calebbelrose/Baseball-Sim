using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadYourPlayers : MonoBehaviour
{
	public RectTransform viewport;						// Viewport for the players
	public RectTransform content;						// Holds the header and player objects
	public Transform teamListHeader;					// Header object
	public Trade trade;									// Trade

	private int currSortedStat = 3;						// Current sorted stat
	private bool ascending = true;						// Whether it's sorted ascending or descending
	private List<int> yourPlayers = new List<int> ();	// User's players

	void Start ()
	{
		DisplayHeader ();
		Sort (3);
	}

	// Refreshes players
	public void Refresh ()
	{
		ascending = !ascending;
		Sort (currSortedStat);
	}

	// Displays header
	void DisplayHeader ()
	{
		int statHeaderLength = 0;
		int [] headerLengths = new int [Manager.Instance.Skills.Length];
		Object header = Resources.Load ("Header", typeof (GameObject));
		float newWidth = 0.0f;

		for (int i = 2; i < Manager.Instance.Skills.Length; i++)
		{
			headerLengths [i] = Manager.Instance.Skills [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;
		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		for (int i = 0; i < Manager.Instance.Skills.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			float currWidth = (8.03f * headerLengths [i]);

			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (teamListHeader.transform);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = Manager.Instance.Skills [i];
			statHeader.GetComponent<Button> ().onClick.AddListener (() => StartSorting (statHeader));

			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		content.sizeDelta = new Vector2 (newWidth, 20 * (Manager.Instance.Teams [0] [0].Players.Count + 1) - viewport.rect.height);
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("YourPlayer");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < yourPlayers.Count; i++)
		{
			Object playerButton = Resources.Load ("YourPlayer", typeof (GameObject));
			GameObject newPlayer = Instantiate (playerButton) as GameObject;
			TradePlayerInfo tradeInfo;

			newPlayer.name = "player" + i.ToString ();
			newPlayer.transform.SetParent (transform);

			string playerString = Manager.Instance.Players [yourPlayers [i]].FirstName;

			for (int j = Manager.Instance.Players [yourPlayers [i]].FirstName.Length; j < Player.longestFirstName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].LastName;

			for (int j = Manager.Instance.Players [yourPlayers [i]].LastName.Length; j < Player.longestLastName; j++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Position;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Position.Length; k < Manager.Instance.Skills [2].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Overall;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Overall.ToString ().Length; k < Manager.Instance.Skills [3].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Offense;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Offense.ToString ().Length; k < Manager.Instance.Skills [4].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Defense;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Defense.ToString ().Length; k < Manager.Instance.Skills [5].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Potential;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Potential.ToString ().Length; k < Manager.Instance.Skills [6].Length; k++)
				playerString += " ";

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Age;

			for (int k = Manager.Instance.Players [yourPlayers [i]].Age.ToString ().Length; k < Manager.Instance.Skills [7].Length; k++)
				playerString += " ";

			for (int j = 0; j < Manager.Instance.Players [yourPlayers [i]].Skills.Length - 1; j++)
			{
				playerString += " " + Manager.Instance.Players [yourPlayers [i]].Skills [j];

				for (int k = Manager.Instance.Players [yourPlayers [i]].Skills [j].ToString ().Length; k < Manager.Instance.Skills [j + 8].Length; k++)
					playerString += " ";
			}

			playerString += " " + Manager.Instance.Players [yourPlayers [i]].Skills [Manager.Instance.Players [yourPlayers [i]].Skills.Length - 1];
			newPlayer.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerString;
			newPlayer.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			tradeInfo = newPlayer.GetComponent<TradePlayerInfo> ();
			tradeInfo.TeamID = 0;
			tradeInfo.PlayerID = Manager.Instance.Players [yourPlayers [i]].ID;

			if (trade.TradeOffer.yourTrades.Contains (i))
				tradeInfo.ChangeButtonColour ();
		}
	}

	// Starts sorting players
	public void StartSorting (GameObject other)
	{
		Sort (int.Parse (other.name.Remove (0, 6)));
	}

	// Sorts players
	void Sort (int headerNum)
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

		currSortedStat = headerNum;

		if (ascending)
			switch (headerNum)
			{
			case 0:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].FirstName).ToList ();
				break;
			case 1:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].LastName).ToList ();
				break;
			case 2:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Position).ToList ();
				break;
			case 3:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Overall).ToList ();
				break;
			case 4:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Offense).ToList ();
				break;
			case 5:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Defense).ToList ();
				break;
			case 6:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Potential).ToList ();
				break;
			case 7:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Age).ToList ();
				break;
			default:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderBy (playerX => Manager.Instance.Players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
			{
			case 0:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].FirstName).ToList ();
				break;
			case 1:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].LastName).ToList ();
				break;
			case 2:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Position).ToList ();
				break;
			case 3:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Overall).ToList ();
				break;
			case 4:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Offense).ToList ();
				break;
			case 5:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Defense).ToList ();
				break;
			case 6:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Potential).ToList ();
				break;
			case 7:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Age).ToList ();
				break;
			default:
				yourPlayers = Manager.Instance.Teams [0] [0].Players.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}

		DisplayPlayers ();
	}
}
