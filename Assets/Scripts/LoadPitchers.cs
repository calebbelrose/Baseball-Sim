using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPitchers : MonoBehaviour
{
	UnityEngine.Object playerButton;
	public RectTransform teamList;
	public Transform teamListHeader;

	void Start ()
	{
		float y = 320f;

		if (Manager.Instance.Teams [0] [0].SP.Count == 0)
			Manager.Instance.Teams [0] [0].SetRoster ();

		PitcherSlot.MaxIndex = 0;
		DisplayHeader ();
		playerButton = Resources.Load ("Pitcher", typeof (GameObject));

		for (int i = 0; i < Manager.Instance.Teams [0] [0].SP.Count; i++)
			DisplayPlayer (Manager.Instance.Teams [0] [0].SP [i], (y -= 20f));

		y -= 5f;

		for (int i = 0; i < Manager.Instance.Teams [0] [0].RP.Count; i++)
			DisplayPlayer (Manager.Instance.Teams [0] [0].RP [i], (y -= 20f));

		y -= 5f;
		DisplayPlayer (Manager.Instance.Teams [0] [0].CP, (y -= 20f));
		PitcherSlot.MaxIndex--;
	}

	// Displays header
	void DisplayHeader ()
	{
		int statHeaderLength = 0;
		int [] headerLengths = new int [Manager.Instance.Skills.Length];
		Object header;
		float prevWidth, newWidth;
		float totalWidth;

		for (int i = 2; i < Manager.Instance.Skills.Length; i++)
		{
			headerLengths [i] = Manager.Instance.Skills [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;

		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		header = Resources.Load ("Header", typeof (GameObject));
		prevWidth = 5.0f;
		newWidth = 0.0f;
		totalWidth = (8.03f * (statHeaderLength + 1.0f));
		teamList.offsetMin = new Vector2 (0, - (20 * (Manager.Instance.Teams [0] [0].SP.Count + Manager.Instance.Teams [0] [0].RP.Count + 3) - 170.0f));
		teamList.offsetMax = new Vector2 (totalWidth - 160.0f, 0);
		totalWidth /= -2.0f;

		for (int i = 0; i < Manager.Instance.Skills.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			float currWidth = (8.03f * headerLengths [i]);

			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (teamListHeader.transform);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = Manager.Instance.Skills [i];
			newWidth += currWidth;
			totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
			prevWidth = currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}
			
		teamList.offsetMax = new Vector2 (newWidth - 160.0f, 0);
	}

	void DisplayPlayer (int index, float y)
	{
		GameObject newPlayer = Instantiate (playerButton) as GameObject;
		string playerString = Manager.Instance.Players [index].FirstName;
		PitcherSlot pitcherSlot = newPlayer.AddComponent<PitcherSlot> ();

		newPlayer.name = "pitcher" + PitcherSlot.MaxIndex.ToString ();

		for (int j = Manager.Instance.Players [index].FirstName.Length; j < Player.longestFirstName; j++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].LastName;

		for (int j = Manager.Instance.Players [index].LastName.Length; j < Player.longestLastName; j++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Position;

		for (int k = Manager.Instance.Players [index].Position.Length; k < Manager.Instance.Skills [2].Length; k++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Overall;

		for (int k = Manager.Instance.Players [index].Overall.ToString ().Length; k < Manager.Instance.Skills [3].Length; k++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Offense;

		for (int k = Manager.Instance.Players [index].Offense.ToString ().Length; k < Manager.Instance.Skills [4].Length; k++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Defense;

		for (int k = Manager.Instance.Players [index].Defense.ToString ().Length; k < Manager.Instance.Skills [5].Length; k++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Potential;

		for (int k = Manager.Instance.Players [index].Potential.ToString ().Length; k < Manager.Instance.Skills [6].Length; k++)
			playerString += " ";

		playerString += " " + Manager.Instance.Players [index].Age;

		for (int k = Manager.Instance.Players [index].Age.ToString ().Length; k < Manager.Instance.Skills [7].Length; k++)
			playerString += " ";

		for (int j = 0; j < Manager.Instance.Players [index].Skills.Length - 1; j++) {
			playerString += " " + Manager.Instance.Players [index].Skills [j];

			for (int k = Manager.Instance.Players [index].Skills [j].ToString ().Length; k < Manager.Instance.Skills [j + 8].Length; k++)
				playerString += " ";
		}

		newPlayer.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerString;
		newPlayer.transform.SetParent (gameObject.transform);
		newPlayer.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		newPlayer.transform.localPosition = new Vector3 (80f, y, 0.0f);
	
		playerString += " " + Manager.Instance.Players [index].Skills [Manager.Instance.Players [index].Skills.Length - 1];
		pitcherSlot.PlayerID = index;
		pitcherSlot.Slot = PitcherSlot.MaxIndex++;
		newPlayer.AddComponent<CanvasGroup> ();
	}
}
