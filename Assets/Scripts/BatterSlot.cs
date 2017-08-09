using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BatterSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public int Slot, PlayerID;
	public Dropdown PositionDropdown;

	private Vector2 offset;
	private CanvasGroup cg;
	private Vector3 startPos;
	private Transform startParent;
	private bool notSet;

	void Start ()
	{
		cg = GetComponent<CanvasGroup> ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if(Slot == -1)
		{
			RectTransform rect = transform.parent.GetComponent<RectTransform> ();
			rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y + 20);
		}

		offset = new Vector2 (eventData.position.x - transform.position.x, eventData.position.y - transform.position.y);
		transform.position = eventData.position - offset;
		cg.blocksRaycasts = false;
		startPos = transform.position;
		startParent = transform.parent;
		transform.SetParent (transform.parent.parent.parent);
		notSet = true;
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		SetReleaseData ();
		notSet = false;
	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position - offset;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(notSet)
			SetReleaseData ();
	}

	public void OnDrop (PointerEventData eventData)
	{
		BatterSlot droppedItem = eventData.pointerDrag.GetComponent<BatterSlot> ();

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

			if (droppedItem.Slot != -1)
				Manager.Instance.Teams [0] [0].Batters [droppedItem.Slot] [0] = droppedItem.PlayerID;
		
			if (Slot != -1)
				Manager.Instance.Teams [0] [0].Batters [Slot] [0] = PlayerID;
		}
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
		cg.blocksRaycasts = true;

		if(Slot == -1)
		{
			RectTransform rect = transform.parent.GetComponent<RectTransform> ();
			rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y - 20);
		}
	}
}
