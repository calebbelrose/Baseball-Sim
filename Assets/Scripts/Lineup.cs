﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lineup : MonoBehaviour
{
	public List<GameObject> batterSlots;					// Batter slots

	private List<string> positions = new List<string> ();	// Available positions

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
				BatterSlot batterSlot = batterSlots [i].GetComponent<BatterSlot> ();

				batterSlots [i].GetComponentInChildren<Text> ().text = Manager.Instance.Players [Manager.Instance.Teams [0] [0].Batters [i] [0]].DisplayString ();
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
