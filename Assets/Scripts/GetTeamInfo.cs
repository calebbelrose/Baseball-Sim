using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        GetComponent<Text>().text = TeamInfo.yourName + "'s " + allTeams.teams[0].teamName;
    }
}
