using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BatterSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
	public int Slot, PlayerID;			// Slot (0-8) in the batting order
	public Dropdown PositionDropdown;	// Position dropdown
	public GameObject Placeholder;		// Placeholder for when the GameObject's being moved

	private Vector2 offset;				// Offset from the mouse
	private CanvasGroup cg;				// Canvas group, to enable/disable blocking raycasts
	private Transform startParent;		// Transform of the parent
	private int childIndex;				// Child index

	void Start ()
	{
		cg = GetComponent<CanvasGroup> ();
	}

	// Prepares the GameObject to be moved
	public void OnPointerDown (PointerEventData eventData)
	{
		if(Slot == -1)
		{
			RectTransform rect = transform.parent.GetComponent<RectTransform> ();
			rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y + 20);
		}
		else
			Placeholder.SetActive (true);

		offset = new Vector2 (eventData.position.x - transform.position.x, eventData.position.y - transform.position.y);
		transform.position = eventData.position - offset;
		cg.blocksRaycasts = false;
		startParent = transform.parent;
		childIndex = transform.GetSiblingIndex ();
		transform.SetParent (transform.parent.parent.parent);
	}

	// Moves the GameObject back to where it started
	public void OnPointerUp (PointerEventData eventData)
	{
		if(Slot == -1)
		{
			RectTransform rect = transform.parent.GetComponent<RectTransform> ();
			rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y - 20);
		}
		else
			Placeholder.SetActive (false);
		
		transform.SetParent (startParent);
		cg.blocksRaycasts = true;
		transform.SetSiblingIndex (childIndex);
	}

	// Follows the mouse
	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position - offset;
	}

	// Swaps the text/playerID if the GameObject was dropped onto another BatterSlot
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
			{
				Manager.Instance.Teams [0] [0].Batters [Slot] [0] = PlayerID;
				Placeholder.SetActive (false);
			}
		}
	}
}
