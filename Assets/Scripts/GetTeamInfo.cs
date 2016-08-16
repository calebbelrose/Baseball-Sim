using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        Team team = allTeams.teams[0];

        GetComponent<Text>().text = manager.GetComponent<TeamInfo>().yourName + "'s " + team.cityName + " " + team.teamName;
    }
}
