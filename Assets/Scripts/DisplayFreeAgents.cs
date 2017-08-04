using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFreeAgents : MonoBehaviour
{
	bool needDisplayObjects = true;
	public Transform playerList, playerListHeader;
	public RectTransform playerListRect, playerListParentRect;
	public GameObject panel;

	void Awake ()
	{
		if (needDisplayObjects)
		{
			Manager.Instance.SetFreeAgentsDisplayObjects (playerList, playerListHeader, playerListRect, playerListParentRect, panel);
			needDisplayObjects = false;
		}

		Manager.Instance.DisplayFreeAgents ();
	}
}