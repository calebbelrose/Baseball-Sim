using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayers : MonoBehaviour
{
	bool needDisplayObjects = true;
	public Transform teamList, teamListHeader;
	public RectTransform teamListRect, teamListParentRect;

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
