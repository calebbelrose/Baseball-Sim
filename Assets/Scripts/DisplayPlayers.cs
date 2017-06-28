using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayers : MonoBehaviour
{
	AllTeams allTeams;
	bool needDisplayObjects = true;
	public Transform teamList, teamListHeader;
	public RectTransform teamListRect, teamListParentRect;

	void Awake ()
	{
		if (allTeams == null)
			allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();

		allTeams.LoadPlayerDisplay (allTeams);

		if (needDisplayObjects)
		{
			allTeams.SetPlayerDisplayObjects (teamList, teamListHeader, teamListRect, teamListParentRect);
			needDisplayObjects = false;
		}

		allTeams.DisplayPlayers ();
	}
}
