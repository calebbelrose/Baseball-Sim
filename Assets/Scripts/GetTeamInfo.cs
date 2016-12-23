using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject manager = GameObject.Find("_Manager");
		Team team = manager.GetComponent<AllTeams>().teams[0];

        GetComponent<Text>().text = manager.GetComponent<TeamInfo>().yourName + "'s " + team.cityName + " " + team.teamName;
    }
}
