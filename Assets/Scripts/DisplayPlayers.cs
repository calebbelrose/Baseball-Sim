using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayers : MonoBehaviour
{
	public Transform teamList;					// Holds the header and player objects
	public Transform teamListHeader;			// Header object
	public RectTransform teamListRect;			// RectTransform of the playerList
	public RectTransform teamListParentRect;	// RectTransform of the parent of the player list

	private bool needDisplayObjects = true;		// Whether the display objects need to be set or not

	void Awake ()
	{
		if (needDisplayObjects)
		{
			Manager.Instance.SetPlayerDisplayObjects (teamList, teamListHeader, teamListRect, teamListParentRect);
			needDisplayObjects = false;
		}

		Manager.Instance.DisplayPlayers ();
	}
}
