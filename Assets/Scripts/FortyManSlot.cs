using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FortyManSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public int PlayerID;			// ID of the player
	public bool InFortyMan;			// Whether the player is in the forty man roster or not

	private Vector2 offset;			// Offset from the mouse
	private CanvasGroup cg;			// Canvas group
	private Vector3 startPos;		// Position before being moved
	private Transform startParent;	// Parent before being moved
	private bool notSet;			// If the data has been set after being moved

	void Start ()
	{
		cg = GetComponent<CanvasGroup> ();
	}

	void Awake()
	{
		Manager.Instance.Teams [0] [0].SetRoster ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		RectTransform rect = transform.parent.GetComponent<RectTransform> ();

		rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y + 20);
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
		FortyManSlot droppedItem = eventData.pointerDrag.GetComponent<FortyManSlot> ();

		if (droppedItem != null)
		{
			Text otherText = eventData.pointerDrag.GetComponentInChildren<Text> ();
			Text thisText = GetComponentInChildren<Text> ();
			string prevString = thisText.text;
			int prevID = PlayerID;

			PlayerID = droppedItem.PlayerID;
			droppedItem.PlayerID = prevID;
			thisText.text = otherText.text;
			otherText.text = prevString;

			if (InFortyMan)
			{
				if (!droppedItem.InFortyMan)
				{
					droppedItem.InFortyMan = true;
					InFortyMan = false;
					Manager.Instance.Teams [0] [0].AddToFortyManRoster (droppedItem.PlayerID);
					Manager.Instance.Teams [0] [0].FortyManRoster.Remove (PlayerID);
					Manager.Instance.Teams [0] [0].SaveFortyManRoster ();
				}
			}
			else if (droppedItem.InFortyMan)
			{
				InFortyMan = true;
				droppedItem.InFortyMan = false;
				Manager.Instance.Teams [0] [0].AddToFortyManRoster (PlayerID);
				Manager.Instance.Teams [0] [0].FortyManRoster.Remove (droppedItem.PlayerID);
				Manager.Instance.Teams [0] [0].SaveFortyManRoster ();
			}
		}
		else
			Debug.Log(eventData.pointerDrag.name);
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
		RectTransform rect = transform.parent.GetComponent<RectTransform> ();
		rect.offsetMin = new Vector2 (rect.offsetMin.x, rect.offsetMin.y - 20);
	}
}