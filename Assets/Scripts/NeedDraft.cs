using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedDraft : MonoBehaviour
{
	private bool needDraftObjects = true;
	public Transform draftList, draftListHeader;
	public RectTransform draftListRect, draftListParentRect;

	void Awake ()
	{
		if (needDraftObjects)
		{
			Manager.Instance.SetDraftPlayerObjects (draftList, draftListHeader, draftListRect, draftListParentRect);
			needDraftObjects = false;
		}

		Manager.Instance.StartDraft ();
	}
}
