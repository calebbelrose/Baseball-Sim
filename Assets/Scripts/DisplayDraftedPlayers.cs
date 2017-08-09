using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDraftedPlayers : MonoBehaviour
{
	bool needDisplayObjects = true;
	public Transform playerList, playerListHeader;
	public RectTransform playerListRect, playerListParentRect;
	public GameObject panel;

	void Start ()
	{
		if (needDisplayObjects)
		{
			Manager.Instance.SetDraftedPlayerDisplayObjects (playerList, playerListHeader, playerListRect, playerListParentRect, panel);
			needDisplayObjects = false;
		}

		Manager.Instance.DisplayDraftedPlayers ();
	}
}