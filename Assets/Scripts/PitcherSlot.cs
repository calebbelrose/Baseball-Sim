using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PitcherSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public int Slot, PlayerID;

	private Vector2 offset;
	private CanvasGroup cg;
	private Vector3 startPos;
	private Transform startParent;

	public static int MaxIndex;

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
		transform.SetParent (transform.parent.parent.parent);
	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position - offset;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		transform.position = startPos;
		transform.SetParent (startParent);
		BlockRaycasts ();
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
				Manager.Instance.Teams [0] [0].SP [droppedItem.Slot] = droppedItem.PlayerID;
			else if (droppedItem.Slot == MaxIndex)
				Manager.Instance.Teams [0] [0].CP = droppedItem.PlayerID;
			else
				Manager.Instance.Teams [0] [0].RP [droppedItem.Slot - 5] = droppedItem.PlayerID;

			if (Slot < 5)
				Manager.Instance.Teams [0] [0].SP [Slot] = PlayerID;
			else if (Slot == MaxIndex)
				Manager.Instance.Teams [0] [0].CP = PlayerID;
			else
				Manager.Instance.Teams [0] [0].RP [Slot - 5] = PlayerID;
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
}
