using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lineup : MonoBehaviour
{
	public List<GameObject> batterSlots;
	List<string> positions = new List<string> ();

	// Use this for initialization
	void Start ()
	{
		if (Manager.Instance.Teams [0] [0].SP.Count == 0)
			Manager.Instance.Teams [0] [0].SetRoster ();
		
		positions.Add ("1B");
		positions.Add ("2B");
		positions.Add ("3B");
		positions.Add ("SS");
		positions.Add ("C");
		positions.Add ("LF");
		positions.Add ("CF");
		positions.Add ("RF");
		positions.Add ("DH");

		DisplayPositions ();

		for (int i = 0; i < batterSlots.Count; i++)
		{
			if (Manager.Instance.Teams [0] [0].Batters [i].Count > 0)
			{
				string playerString = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].FirstName;
				BatterSlot batterSlot = batterSlots [i].GetComponent<BatterSlot> ();

				for (int j = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].FirstName.Length; j < Player.longestFirstName; j++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].LastName;

				for (int j = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].LastName.Length; j < Player.longestLastName; j++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Position;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Position.Length; k < Manager.Instance.Skills [2].Length; k++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Overall;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Overall.ToString ().Length; k < Manager.Instance.Skills [3].Length; k++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Offense;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Offense.ToString ().Length; k < Manager.Instance.Skills [4].Length; k++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Defense;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Defense.ToString ().Length; k < Manager.Instance.Skills [5].Length; k++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Potential;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Potential.ToString ().Length; k < Manager.Instance.Skills [6].Length; k++)
					playerString += " ";

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Age;

				for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Age.ToString ().Length; k < Manager.Instance.Skills [7].Length; k++)
					playerString += " ";

				for (int j = 0; j < Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Skills.Length - 1; j++) {
					playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Skills [j];

					for (int k = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Skills [j].ToString ().Length; k < Manager.Instance.Skills [j + 8].Length; k++)
						playerString += " ";
				}

				playerString += " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Skills [Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].Skills.Length - 1];
				batterSlots [i].GetComponentInChildren<Text> ().text = playerString;
				batterSlot.PlayerID = Manager.Instance.Teams [0] [0].Batters [i] [0];
				batterSlot.PositionDropdown.value = positions.IndexOf (Manager.Instance.Players [batterSlot.PlayerID].Position) + 1;
				batterSlots [i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}
		}

		for (int i = 0; i < batterSlots.Count; i++)
			for (int j = 0; j < positions.Count; j++)
				batterSlots [i].GetComponent<BatterSlot> ().PositionDropdown.options.Add (new Dropdown.OptionData (positions [j]));
	}

	public void SetSize (float size)
	{
		Vector2 v2 = new Vector2 (size, 20.0f);

		for (int i = 0; i < 9; i++)
			batterSlots [i].GetComponent<RectTransform> ().sizeDelta = v2;
	}

	public void AddPosition (string position)
	{
		positions.Add (position);
		DisplayPositions ();
	}

	public void RemovePosition (string position)
	{
		positions.Remove (position);
		DisplayPositions ();
	}

	void DisplayPositions ()
	{
		for (int i = 0; i < batterSlots.Count; i++)
		{
			BatterSlot batterSlot = batterSlots [i].GetComponent<BatterSlot> ();
			string currPosition = batterSlot.PositionDropdown.options [batterSlot.PositionDropdown.value].text;

			batterSlot.PositionDropdown.options.Clear ();
			batterSlot.PositionDropdown.options.Add (new Dropdown.OptionData ("-"));

			if (currPosition == "-")
				batterSlot.PositionDropdown.value = 0;
			else
			{
				batterSlot.PositionDropdown.options.Add (new Dropdown.OptionData (currPosition));
				batterSlot.PositionDropdown.value = 1;
			}

			for (int j = 0; j < positions.Count; j++)
				batterSlot.PositionDropdown.options.Add (new Dropdown.OptionData (positions [j]));
		}
	}
}
