using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateTeams : MonoBehaviour {

	// Populates dropdown with all teams
	// Use this for initialization
	void Start () {
        Dropdown dropdown = GetComponent<Dropdown>();

        for (int i = 1; i < Manager.Instance.teams.Count; i++)
            dropdown.options.Add(new Dropdown.OptionData() { text = Manager.Instance.teams[i].CityName + " " + Manager.Instance.teams[i].TeamName });

        dropdown.value = 1;
        dropdown.value = 0;
    }
}
