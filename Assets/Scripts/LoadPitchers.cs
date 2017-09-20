using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPitchers : MonoBehaviour
{
	public RectTransform viewport;		// Viewport for the players
	public RectTransform content;		// Holds the header and player objects
	public Transform teamListHeader;	// Header object
	public GameObject separator;		// Prefab to separate pitching positions

	private Object playerButton;		// Player button

	void Start ()
	{
		GameObject newSeparator;

		if (Manager.Instance.Teams [0] [0].SP.Count == 0)
			Manager.Instance.Teams [0] [0].SetRoster ();

		PitcherSlot.MaxIndex = 0;
		content.sizeDelta = new Vector2 (Manager.DisplayHeaders (null, teamListHeader.transform), 20 * (Manager.Instance.Teams [0] [0].SP.Count + Manager.Instance.Teams [0] [0].RP.Count + 4) - viewport.rect.height);
		playerButton = Resources.Load ("Pitcher", typeof (GameObject));

		newSeparator = Instantiate (separator, transform) as GameObject;
		newSeparator.transform.GetChild (0).GetComponent<Text> ().text = "Starting Pitchers";

		for (int i = 0; i < Manager.Instance.Teams [0] [0].SP.Count; i++)
			DisplayPlayer (Manager.Instance.Teams [0] [0].SP [i]);

		newSeparator = Instantiate (separator, transform) as GameObject;
		newSeparator.transform.GetChild (0).GetComponent<Text> ().text = "Relief Pitchers";

		for (int i = 0; i < Manager.Instance.Teams [0] [0].RP.Count; i++)
			DisplayPlayer (Manager.Instance.Teams [0] [0].RP [i]);

		newSeparator = Instantiate (separator, transform) as GameObject;
		newSeparator.transform.GetChild (0).GetComponent<Text> ().text = "Closer";
		
		DisplayPlayer (Manager.Instance.Teams [0] [0].CP);
		PitcherSlot.MaxIndex--;
	}

	void DisplayPlayer (int index)
	{
		PitcherSlot pitcherSlot = Manager.DisplayPlayer (playerButton, transform, Manager.Instance.Players [index].ID).AddComponent<PitcherSlot> ();

		pitcherSlot.PlayerID = index;
		pitcherSlot.Slot = PitcherSlot.MaxIndex++;
	}
}
