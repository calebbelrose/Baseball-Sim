using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedDraft : MonoBehaviour
{
	public Transform draftList;					// Holds the header and player objects
	public Transform draftListHeader;			// Header object
	public RectTransform draftListRect;			// RectTransform of the playerList
	public RectTransform draftListParentRect;	// RectTransform of the parent of the player list

	private bool needDraftObjects = true;

	void Start ()
	{
		if (needDraftObjects)
		{
			Manager.Instance.SetDraftPlayerObjects (draftList, draftListHeader, draftListRect, draftListParentRect);
			needDraftObjects = false;
		}

		Manager.Instance.StartDraft ();
	}
}
