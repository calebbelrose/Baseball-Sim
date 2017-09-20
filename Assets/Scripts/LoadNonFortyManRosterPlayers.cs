using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadNonFortyManRosterPlayers : MonoBehaviour
{
	public RectTransform viewport;		// Viewport for the players
	public RectTransform content;		// Holds the header and player objects
	public Transform teamListHeader;	// Header object

	private int currSortedStat = 3;		// Current sorted stat
	private bool ascending = true;		// Whether it's sorted ascending or descending
	List<int> yourPlayers;				// User's players
	Object playerButton;				// Player button

	void Start ()
	{
		if (Manager.Instance.Teams [0] [0].SP.Count == 0)
			Manager.Instance.Teams [0] [0].SetRoster ();

		playerButton = Resources.Load ("YourPlayer", typeof(GameObject));
		yourPlayers = new List<int> ();
		content.sizeDelta = new Vector2 (Manager.DisplayHeaders ((GameObject) => StartSorting(GameObject), teamListHeader.transform), 20.0f);
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
			if (!Manager.Instance.Teams [0] [0].IsInFortyManRoster (yourPlayers [i]))
			{
				FortyManSlot fortyManSlot = Manager.DisplayPlayer (playerButton, transform, yourPlayers [i]).AddComponent<FortyManSlot> ();

				fortyManSlot.InFortyMan = false;
				fortyManSlot.PlayerID = Manager.Instance.Players [yourPlayers [i]].ID;
			}
		}

		content.sizeDelta = new Vector2 (content.sizeDelta.x, (20.0f * content.childCount) - viewport.rect.height);
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