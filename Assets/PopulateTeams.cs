using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateTeams : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Dropdown dropdown = GetComponent<Dropdown>();
        AllTeams allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();

        for (int i = 1; i < allTeams.teams.Length; i++)
            dropdown.options.Add(new Dropdown.OptionData() { text = allTeams.teams[i].cityName + " " + allTeams.teams[i].teamName });

        dropdown.value = 1;
        dropdown.value = 0;
    }
}
