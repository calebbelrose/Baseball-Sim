using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFreeAgents : MonoBehaviour
{
	public Transform playerList;				// Holds the header and player objects
	public Transform playerListHeader;			// Header object
	public RectTransform playerListRect;		// RectTransform of the player List
	public RectTransform playerListParentRect;	// RectTransform of the parent of the player list
	public GameObject panel;					// Panel used for displaying a selected player

	private bool needDisplayObjects = true;				// Whether the display objects need to be set or not

	void Start ()
	{
		if (needDisplayObjects)
		{
			Manager.Instance.SetFreeAgentsDisplayObjects (playerList, playerListHeader, playerListRect, playerListParentRect, panel);
			needDisplayObjects = false;
		}

		Manager.Instance.DisplayFreeAgents ();
	}
}