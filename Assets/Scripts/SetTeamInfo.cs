using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SetTeamInfo : MonoBehaviour
{
	// Sets the info for the player's team
	void SetInfo ()
	{
		TeamInfo teamInfo = Manager.Instance.gameObject.GetComponent<TeamInfo> ();

		string tempYourName = GameObject.Find ("fldYourName").GetComponent<InputField> ().text,
		tempCityName = GameObject.Find ("fldCityName").GetComponent<InputField> ().text,
		tempTeamName = GameObject.Find ("fldTeamName").GetComponent<InputField> ().text;

		// Sets your name to the entered name, otherwise to a random name
		if (tempYourName != "")
			teamInfo.yourName = tempYourName;
		else
		{
			string [] firstNames = File.ReadAllLines ("FirstNames.txt"),
			lastNames = File.ReadAllLines ("LastNames.txt");
			teamInfo.yourName = firstNames [ (int) (Random.value * firstNames.Length)] + " " + lastNames [ (int) (Random.value * lastNames.Length)];
		}

		PlayerPrefs.SetString ("Your Name", teamInfo.yourName);

		// Sets the city name to the entered name
		if (tempCityName != "")
			Manager.Instance.Teams [0] [0].CityName = tempCityName;
		
		// Sets the team name to the entered name
		if (tempTeamName != "")
			Manager.Instance.Teams [0] [0].TeamName = tempTeamName;

		Manager.Instance.Teams [0] [0].Save ();
		PlayerPrefs.Save ();
	}
}
