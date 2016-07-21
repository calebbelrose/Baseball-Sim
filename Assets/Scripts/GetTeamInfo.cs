using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GetComponent<Text>().text = TeamInfo.yourName + "'s " + TeamInfo.cityName + " " + TeamInfo.teamName;
    }
}
