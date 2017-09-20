using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SetTeamInfo : MonoBehaviour
{
	public InputField yourName;	// User's name
	public InputField cityName;	// User's city name
	public InputField teamName;	// User's team name
	public Dropdown league;		// User's league
	public Dropdown division;	// User's division

	// Sets the info for the user's team
	void SetInfo ()
	{
		string tempYourName = yourName.text, tempCityName = cityName.text, tempTeamName = teamName.text;
		int newLeague, newDivision;

		// Sets user's name to the entered name, otherwise to a random name
		if (tempYourName != "")
			Manager.Instance.YourName = tempYourName;
		else
		{
			string [] firstNames = File.ReadAllLines ("FirstNames.txt"),
			lastNames = File.ReadAllLines ("LastNames.txt");
			Manager.Instance.YourName = firstNames [ (int) (Random.value * firstNames.Length)] + " " + lastNames [ (int) (Random.value * lastNames.Length)];
		}

		PlayerPrefs.SetString ("Your Name", Manager.Instance.YourName);
		PlayerPrefs.SetString ("Team Colour", Manager.Instance.TeamColour.r + "," + Manager.Instance.TeamColour.g + "," + Manager.Instance.TeamColour.b);

		// Sets the city name to the entered name
		if (tempCityName != "")
			Manager.Instance.Teams [0] [0].CityName = tempCityName;
		
		// Sets the team name to the entered name
		if (tempTeamName != "")
			Manager.Instance.Teams [0] [0].TeamName = tempTeamName;

		if (league.value == 0)
			newLeague = (int)(Random.value * 2);
		else
			newLeague = league.value - 1;

		if (division.value == 0)
			newDivision = (int)(Random.value * 3);
		else
			newDivision = division.value - 1;
		
		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
		{
			Manager.Instance.Teams [0] [i].Division = (Division)newDivision;
			Manager.Instance.Teams [0] [i].League = (League)newLeague;

			newDivision = (newDivision++) % 2;
			newLeague = (newLeague++) % 2;
			Manager.Instance.Teams [0] [i].Save ();
		}

		PlayerPrefs.Save ();
	}
}
