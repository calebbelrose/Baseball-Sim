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
	Object playerButton;								// Player button

	void Start ()
	{
		playerButton = Resources.Load ("YourPlayer", typeof (GameObject));
		content.sizeDelta = new Vector2 (Manager.DisplayHeaders ((GameObject) => StartSorting(GameObject), teamListHeader.transform), 20 * (Manager.Instance.Teams [0] [0].Players.Count + 1) - viewport.rect.height);
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].Players);
		DisplayPlayers ();
	}

	// Refreshes players
	public void Refresh ()
	{
		ascending = !ascending;
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].Players);
		DisplayPlayers ();
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("YourPlayer");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < yourPlayers.Count; i++)
		{
			TradePlayerInfo tradeInfo;

			tradeInfo = Manager.DisplayPlayer (playerButton, transform, yourPlayers [i]).GetComponent<TradePlayerInfo> ();
			tradeInfo.TeamID = 0;
			tradeInfo.PlayerID = Manager.Instance.Players [yourPlayers [i]].ID;

			if (trade.TradeOffer.yourTrades.Contains (i))
				tradeInfo.ChangeButtonColour ();
		}
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
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].Players);
		DisplayPlayers ();
	}
}
