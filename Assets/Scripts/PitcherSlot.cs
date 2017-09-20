using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PitcherSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
	public int Slot;				// Slot the pitcher is in
	public int PlayerID;			// Player ID

	private Vector2 offset;			// Offset from the mouse
	private CanvasGroup cg;			// Canvas group
	private Vector3 startPos;		// Position the object is at when it is clicked
	private Transform startParent;	// Parent when the object is clicked
	private int childIndex;			// Child index when the object is clicked

	public static int MaxIndex;		// Maximum slot index, used for closer

	void Start ()
	{
		cg = GetComponent<CanvasGroup> ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		offset = new Vector2 (eventData.position.x - transform.position.x, eventData.position.y - transform.position.y);
		transform.position = eventData.position - offset;
		cg.blocksRaycasts = false;
		startPos = transform.position;
		startParent = transform.parent;
		childIndex = transform.GetSiblingIndex ();
		transform.SetParent (transform.parent.parent.parent);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		SetReleaseData ();
		transform.SetSiblingIndex (childIndex);
	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position - offset;
	}

	public void OnDrop (PointerEventData eventData)
	{
		PitcherSlot droppedItem = eventData.pointerDrag.GetComponent<PitcherSlot> ();

		if (droppedItem != null)
		{
			Text otherText = eventData.pointerDrag.GetComponentInChildren<Text> ();
			Text thisText = GetComponentInChildren<Text> ();
			string prevString = thisText.text;
			int prev = PlayerID;
			PlayerID = droppedItem.PlayerID;
			droppedItem.PlayerID = prev;
			thisText.text = otherText.text;
			otherText.text = prevString;
			droppedItem.transform.position = droppedItem.StartPos;
			droppedItem.BlockRaycasts ();

			if (droppedItem.Slot < 5)
			{
				Manager.Instance.Teams [0] [0].SP [droppedItem.Slot] = droppedItem.PlayerID;

				if (Slot < 5)
					Manager.Instance.Teams [0] [0].SP [Slot] = PlayerID;
				else if (Slot == MaxIndex)
				{
					Manager.Instance.Teams [0] [0].CP = PlayerID;
					PlayerPrefs.SetInt ("CP", PlayerID);
				}
				else
				{
					Manager.Instance.Teams [0] [0].RP [Slot - 5] = PlayerID;
					Manager.Instance.Teams [0] [0].SaveRP ();
				}

				Manager.Instance.Teams [0] [0].SaveSP ();
			}
			else if (droppedItem.Slot == MaxIndex)
			{
				Manager.Instance.Teams [0] [0].CP = droppedItem.PlayerID;

				if (Slot < 5)
				{
					Manager.Instance.Teams [0] [0].SP [Slot] = PlayerID;
					Manager.Instance.Teams [0] [0].SaveSP ();
				}
				else
				{
					Manager.Instance.Teams [0] [0].RP [Slot - 5] = PlayerID;
					Manager.Instance.Teams [0] [0].SaveRP ();
				}

				PlayerPrefs.SetInt ("CP", droppedItem.PlayerID);
			}
			else
			{
				Manager.Instance.Teams [0] [0].RP [droppedItem.Slot - 5] = droppedItem.PlayerID;

				if (Slot < 5)
				{
					Manager.Instance.Teams [0] [0].SP [Slot] = PlayerID;
					Manager.Instance.Teams [0] [0].SaveSP ();
				}
				else if (Slot == MaxIndex)
				{
					Manager.Instance.Teams [0] [0].CP = PlayerID;
					PlayerPrefs.SetInt ("CP", PlayerID);
				}
				else
					Manager.Instance.Teams [0] [0].RP [Slot - 5] = PlayerID;

				Manager.Instance.Teams [0] [0].SaveRP ();
			}


		}
	}

	public void BlockRaycasts()
	{
		cg.blocksRaycasts = true;
	}

	public Vector3 StartPos
	{
		get
		{
			return startPos;
		}
	}

	void SetReleaseData ()
	{
		transform.position = startPos;
		transform.SetParent (startParent);
		BlockRaycasts ();
	}
}
