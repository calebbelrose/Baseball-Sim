using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour
{
	// Gets the info for the user's team
	void Start ()
	{
		Team team = Manager.Instance.Teams [0] [0];

		GetComponent<Text> ().text = Manager.Instance.YourName + "'s " + team.CityName + " " + team.TeamName;
	}
}
