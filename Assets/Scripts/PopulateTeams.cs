using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateTeams : MonoBehaviour
{
	// Populates dropdown with all teams
	void Start ()
	{
		Dropdown dropdown = GetComponent<Dropdown> ();

		for (int i = 1; i < Manager.Instance.Teams [0].Count; i++)
			dropdown.options.Add (new Dropdown.OptionData () { text = Manager.Instance.Teams [0] [i].CityName + " " + Manager.Instance.Teams [0] [i].TeamName });

		dropdown.value = 1;
		dropdown.value = 0;
	}
}
