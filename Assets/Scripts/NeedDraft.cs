using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedDraft : MonoBehaviour
{
	AllTeams allTeams = null;
	private bool needDraftObjects = true;
	public Transform draftList, draftListHeader;
	public RectTransform draftListRect, draftListParentRect;

	void Awake()
	{
		if (allTeams == null)
			allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();

		if (needDraftObjects)
		{
			allTeams.SetDraftPlayerObjects (draftList, draftListHeader, draftListRect, draftListParentRect);
			needDraftObjects = false;
		}

			allTeams.StartDraft ();
	}
}
