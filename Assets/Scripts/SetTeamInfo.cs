using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SetTeamInfo : MonoBehaviour
{
	public InputField yourName, cityName, teamName;

	// Sets the info for the player's team
	void SetInfo ()
	{
		string tempYourName = yourName.text, tempCityName = cityName.text, tempTeamName = teamName.text;

		// Sets your name to the entered name, otherwise to a random name
		if (tempYourName != "")
			Manager.Instance.YourName = tempYourName;
		else
		{
			string [] firstNames = File.ReadAllLines ("FirstNames.txt"),
			lastNames = File.ReadAllLines ("LastNames.txt");
			Manager.Instance.YourName = firstNames [ (int) (Random.value * firstNames.Length)] + " " + lastNames [ (int) (Random.value * lastNames.Length)];
		}

		PlayerPrefs.SetString ("Your Name", Manager.Instance.YourName);

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
