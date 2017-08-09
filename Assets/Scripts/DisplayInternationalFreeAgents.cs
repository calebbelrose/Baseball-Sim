using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInternationalFreeAgents : MonoBehaviour
{
	bool needDisplayObjects = true;
	public Transform playerList, playerListHeader;
	public RectTransform playerListRect, playerListParentRect;
	public GameObject panel;

	void Start ()
	{
		if (needDisplayObjects)
		{
			Manager.Instance.SetInternationalFreeAgentsDisplayObjects (playerList, playerListHeader, playerListRect, playerListParentRect, panel);
			needDisplayObjects = false;
		}

		Manager.Instance.DisplayInternationalFreeAgents ();
	}
}