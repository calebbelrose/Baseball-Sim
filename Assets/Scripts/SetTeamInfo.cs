﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SetTeamInfo : MonoBehaviour
{
    void SetInfo()
    {
        GameObject manager = GameObject.Find("_Manager");
        AllTeams allTeams = manager.GetComponent<AllTeams>();
        TeamInfo teamInfo = manager.GetComponent<TeamInfo>();

        string tempYourName = GameObject.Find("fldYourName").GetComponent<InputField>().text,
            tempCityName = GameObject.Find("fldCityName").GetComponent<InputField>().text,
            tempTeamName = GameObject.Find("fldTeamName").GetComponent<InputField>().text;

		// Sets your name to the entered name, otherwise to a random name
        if (tempYourName != "")
            teamInfo.yourName = tempYourName;
        else
        {
            string[] firstNames = File.ReadAllLines("FirstNames.txt"),
            lastNames = File.ReadAllLines("LastNames.txt");
            teamInfo.yourName = firstNames[(int)(Random.value * firstNames.Length)] + " " + lastNames[(int)(Random.value * lastNames.Length)];
        }

        PlayerPrefs.SetString("Your Name", teamInfo.yourName);
    
		// Sets the city name to the entered name
        if (tempCityName != "")
            allTeams.teams[0].cityName = tempCityName;
   
		// Sets the team name to the entered name
        if (tempTeamName != "")
            allTeams.teams[0].teamName = tempTeamName;

		PlayerPrefs.SetString("Team" + allTeams.teams[0].id, allTeams.teams[0].id + "," + allTeams.teams[0].cityName + "," + allTeams.teams[0].teamName + "," + allTeams.teams[0].Pick);
        PlayerPrefs.Save();
    }
}
