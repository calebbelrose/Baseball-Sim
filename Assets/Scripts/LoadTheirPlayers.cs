using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadTheirPlayers : MonoBehaviour
{
	public RectTransform viewport;						// Viewport for the players
	public RectTransform content;						// Holds the header and player objects
	public Transform teamListHeader;					// Header object
	public Trade trade;									// Trade
	public List<int> theirPlayers = new List<int> ();	// Other team's players

	private int currSortedStat = 3;						// Current sorted stat
	private bool ascending = true;						// Whether it's sorted ascending or descending
	private int theirTeam;								// ID of other team
	Object playerButton;								// Player button

	void Start ()
	{
		playerButton = Resources.Load ("TheirPlayer", typeof (GameObject));
		content.sizeDelta = new Vector2 (Manager.DisplayHeaders ((GameObject) => StartSorting(GameObject), teamListHeader.transform), 0.0f);
		theirPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [theirTeam].Players);
		DisplayPlayers ();
	}

	// Changes other team
	public void ChangeTeam (Dropdown dropdown)
	{
		theirTeam = dropdown.value + 1;
		Refresh ();
	}

	// Refreshes players
	public void Refresh ()
	{
		ascending = !ascending;
		theirPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [theirTeam].Players);
		DisplayPlayers ();
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("TheirPlayer");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < theirPlayers.Count; i++)
		{
			TradePlayerInfo tradeInfo;

			tradeInfo = Manager.DisplayPlayer (playerButton, transform, theirPlayers [i]).GetComponent<TradePlayerInfo> ();
			tradeInfo.TeamID = theirTeam;
			tradeInfo.PlayerID = Manager.Instance.Players [theirPlayers [i]].ID;

			if (trade.TradeOffer.theirTrades.Contains (i))
				tradeInfo.ChangeButtonColour ();
		}

		content.sizeDelta = new Vector2 (content.sizeDelta.x, 20 * (Manager.Instance.Teams [0] [theirTeam].Players.Count + 1) - viewport.rect.height);
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
		theirPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [theirTeam].Players);
		DisplayPlayers ();
	}
}
