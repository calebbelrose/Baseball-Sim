﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamInfo : MonoBehaviour
{
	// Gets the info for the player's team
	void Start ()
	{
		Team team = Manager.Instance.Teams [0] [0];

		GetComponent<Text> ().text = Manager.Instance.gameObject.GetComponent<TeamInfo> ().yourName + "'s " + team.CityName + " " + team.TeamName;
	}
}
