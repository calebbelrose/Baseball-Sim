using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadMajorLeaguePlayers : MonoBehaviour
{
	public RectTransform viewport; 		// Viewport for the 
	public RectTransform teamList;		// Holds the team list header and players not in batting order
	public RectTransform batterList;	// Holds the batter list header and players in the batting order
	public Transform teamListHeader;	// Team list header
	public Transform batterListHeader;	// Batter list header

	private int currSortedStat = 3;		// Current sorted stat
	private bool ascending = true;		// Whether it's sorted ascending or descending
	private List<int> yourPlayers;		// User's players
	private Object playerButton;		// Player button

	void Start ()
	{
		float width = Manager.DisplayHeaders ((GameObject) => StartSorting (GameObject), teamListHeader.transform);

		if (Manager.Instance.Teams [0] [0].SP.Count == 0)
			Manager.Instance.Teams [0] [0].SetRoster ();

		playerButton = Resources.Load ("Player", typeof(GameObject));
		yourPlayers = new List<int> ();

		for(int i = 0; i < teamListHeader.childCount; i++)
		{
			GameObject batterStatHeader;

			batterStatHeader = Instantiate (teamListHeader.GetChild (i).gameObject) as GameObject;
			batterStatHeader.transform.SetParent (batterListHeader);
			batterStatHeader.GetComponent<Button> ().interactable = false;
		}

		teamList.sizeDelta = new Vector2 (width, teamList.sizeDelta.y);
		batterList.sizeDelta = new Vector2 (width, 200.0f);
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].MajorLeagueIndexes);
	}

	// Displays players
	public void DisplayPlayers ()
	{
		GameObject [] currPlayers = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy (currPlayers [i]);

		for (int i = 0; i < yourPlayers.Count; i++)
		{
			if (!Manager.Instance.Players [yourPlayers [i]].IsPitcher && !Manager.Instance.Teams [0] [0].IsBatter (yourPlayers [i]))
			{
				BatterSlot batterSlot = Manager.DisplayPlayer (playerButton, transform, yourPlayers [i]).AddComponent<BatterSlot> ();

				batterSlot.PlayerID = yourPlayers [i];
				batterSlot.Slot = -1;
			}
		}

		teamList.sizeDelta = new Vector2 (teamList.sizeDelta.x, 20 * teamList.transform.childCount);
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
		yourPlayers = Manager.Instance.Sort (currSortedStat, ascending, Manager.Instance.Teams [0] [0].MajorLeagueIndexes);
		DisplayPlayers ();
	}
}
