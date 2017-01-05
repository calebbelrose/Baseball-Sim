using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Report : MonoBehaviour
{
	AllTeams allTeams;
	List<Player> players;
	List<int> sections = new List<int>();

	void Start ()
	{
		allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
		players = new List<Player> ();
		GenerateReport ();
	}

	// Generates a report to show worst/best players/teams
	void GenerateReport()
	{
		List<Player> result;
		List<Team> resultT;
		string text = "";
		Text report = GameObject.Find ("test").GetComponent<Text> ();
		int height;
		RectTransform rect;

		for (int i = 1; i < 25; i++)
			sections.Add (i);

		while(allTeams.tradeList.Count > 0)
		{
			//text += allTeams.tradeList [0];
			allTeams.tradeList.RemoveAt (0);
		}

		for(int i = 0; i < allTeams.teams.Count; i++)
			for(int j = 0; j < allTeams.teams[i].players.Count; j++)
				players.Add(allTeams.teams[i].players[j]);

		for (int k = 0; k < 1; k++) {
			int r = 24 + k;//(int)(Random.value * sections.Count);
			int j, l;

			switch (r) {
			case 1:
			// Power/Speed Number
				text += "Highest Power/Speed Number\n";
				result = players.OrderByDescending (playerX => playerX.PSN()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result[i].PSN().ToString ("0.00") + "\n";
				break;
			case 2:
			// Most Homeruns
				text += "Most Homeruns\n";
				result = players.OrderByDescending (playerX => playerX.homeruns).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].homeruns + "\n";
				break;

			case 3:
			// Most Innings Pitcher
				text += "Most Innings Pitched\n";
				result = players.OrderByDescending (playerX => playerX.inningsPitched).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].inningsPitched / 3 + "." + result [i].inningsPitched % 3 + "\n";
				break;

			case 4:
			// Lowest WHIP
				text += "Lowest WHIP\n";
				result = players.OrderBy (playerX => playerX.WHIP()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + ((result [i].walksAgainst + result [i].hitsAgainst) / (result [i].inningsPitched / 3.0f)).ToString ("0.00") + "\n";
				break;

			case 5:
			// RC/27
				text += "Highest RC/27\n";
				result = players.OrderByDescending (playerX => playerX.RC27()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result[i].RC27().ToString("0.00") + "\n";
				break;

			case 6:
			// Pitcher Appearances
				text += "Most Pitcher Appearances\n";
				result = players.OrderByDescending (playerX => playerX.games).ToList ();
				j = 0;
				l = 0;
				while (j < 10 )
				{
					if (result [l].position.Length == 2 && result [l].position.Substring (1) == "P")
					{
						text += result [l].firstName.PadRight (Player.longestFirstName) + " " + result [l].lastName.PadRight (Player.longestLastName) + " " + result [l].games + "\n";
						j++;
					}
					l++;
				}
				break;
			case 7:
			// RBIs
				text += "Most RBIs\n";
				result = players.OrderByDescending (playerX => playerX.runsBattedIn).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].runsBattedIn + "\n";
				break;

			case 8:
			// Isoated Power
				text += "Most Isolated Power\n";
				result = players.OrderByDescending (playerX => playerX.ISO()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].ISO().ToString("0.000") + "\n";
				break;

			case 9:
			// Batting Average
				text += "Highest Batting Average\n";
				result = players.OrderByDescending (playerX => playerX.BA()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result[i].BA().ToString("0.000") + "\n";
				break;

			case 10:
			// Batting Average Against
				text += "Lowest Batting Average Against\n";
				result = players.OrderBy (playerX => playerX.BAA()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result[i].BAA().ToString("0.000") + "\n";
				break;

			case 11:
			// Homeruns Against
				text += "Least Homeruns Against\n";
				result = players.OrderBy (playerX => playerX.homerunsAgainst).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].homerunsAgainst + "\n";
				break;

			case 12:
			// Walks Against
				text += "Lowest Walks Against\n";
				result = players.OrderBy (playerX => playerX.walksAgainst).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].walksAgainst + "\n";
				break;

			case 13:
			// Age
				text += "Oldest Players\n";
				result = players.OrderByDescending (playerX => playerX.age).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].age + "\n";
				break;

			case 14:
			// Hits Allowed
				text += "Most Hits Allowed\n";
				result = players.OrderByDescending (playerX => playerX.hitsAgainst).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].hitsAgainst + "\n";
				break;

			case 15:
			// ERA
				text += "Lowest Earned Run Average\n";
				result = players.OrderBy (playerX => (playerX.earnedRuns / (playerX.inningsPitched / 3.0f) * 9)).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + (result [i].earnedRuns / (result [i].inningsPitched / 3.0f) * 9) + "\n";
				break;

			case 16:
			// Salary
				text += "Most Paid Players\n";
				result = players.OrderBy (playerX => playerX.salary).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].salary + "\n";
				break;

			case 17:
			// Hit Streak
				text += "Longest Hit Streaks (Currently " + allTeams.longestHitStreak + " by " + allTeams.hitStreakName + " in " + allTeams.hitStreakYear + "\n";
				result = players.OrderByDescending (playerX => playerX.hitStreak).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].hitStreak + "\n";
				break;

			case 18:
			// Quality Starts
				text += "Most Quality Starts\n";
				result = players.OrderByDescending (playerX => playerX.qualityStarts).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].qualityStarts + "\n";
				break;

			case 19:
			// Injuries
				text += "Injuries\n";
				result = players.OrderByDescending (playerX => playerX.injuryLength).ToList ();
				j = 0;
				while (result [j].injuryLength != 0)
				{
					text += result [j].firstName.PadRight (Player.longestFirstName) + " " + result [j].lastName.PadRight (Player.longestLastName) + " is injured with a " + result [j].injurySeriousness + " " + result [j].injuryLocation + " for " + result [j].injuryLength + "\n";
					j++;
				}
				break;
			case 20:
			// Linear Weights Ratio
				text += "Highest Linear Weights Ratio\n";
				result = players.OrderByDescending (playerX => playerX.LWR()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result[i].LWR().ToString("0.000") + "\n";
				break;
			case 21:
			// Walk-to-strikeout ratio
				text += "Highest Walk-to-Strikeout Ratio\n";
				result = players.OrderByDescending (playerX => playerX.BBToK ()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].BBToK().ToString("0.00") + "\n";
				break;
			case 22:
			// Pitching Prospects
				text += "Best Pitching Prospects\n";
				result = players.OrderBy (playerX => playerX.games).ThenByDescending (playerY => (playerY.skills [0] + playerY.skills [1] + playerY.skills [2] + playerY.skills [3] + playerY.skills [4] + playerY.skills [5] + playerY.skills [6] + playerY.potential)).ToList ();
				j = 0;
				l = 0;
				while (j < 10 && result [l].games == 0) {
					if (result [l].position.Length == 2 && result [l].position.Substring (1) == "P")
					{
						text += result [l].firstName.PadRight (Player.longestFirstName) + " " + result [l].lastName.PadRight (Player.longestLastName) + (result [l].skills [0] + result [l].skills [1] + result [l].skills [2] + result [l].skills [3] + result [l].skills [4] + result [l].skills [5] + result [l].skills [6] + result [l].potential) + "\n";
						j++;
					}
					l++;
				}
				break;
			case 23:
			// Batting Prospects
				text += "Best Batting Prospects\n";
				result = players.OrderBy (playerX => playerX.games).ThenByDescending (playerY => (playerY.skills [0] + playerY.skills [1] + playerY.skills [2] + playerY.skills [3] + playerY.skills [4] + playerY.skills [5] + playerY.skills [6] + playerY.potential)).ToList ();
				j = 0;
				l = 0;
				while (j < 10 && result [l].games == 0)
				{
					if (result [l].position.Length == 1 || (result [l].position.Length == 2 && result [l].position.Substring (1) != "P"))
					{
						text += result [l].firstName.PadRight (Player.longestFirstName) + " " + result [l].lastName.PadRight (Player.longestLastName) + (result [l].skills [0] + result [l].skills [1] + result [l].skills [2] + result [l].skills [3] + result [l].skills [4] + result [l].skills [5] + result [l].skills [6] + result [l].potential) + "\n";
						j++;
					}
					l++;
				}
				break;
			case 24:
			// Projected Wins
				text += "Projected Wins\n";
				float total = 0;
				resultT = allTeams.teams.OrderByDescending (teamX => teamX.overalls [0]).ToList ();
				for (int i = 0; i < resultT.Count; i++)
					total += resultT [i].overalls [0];
				total /= resultT.Count;
				for (int i = 0; i < resultT.Count; i++)
					text += resultT [i].cityName.PadRight (Team.longestCityName) + " " + resultT [i].teamName.PadRight (Team.longestTeamName) + " " + ((resultT [i].overalls [0] - total) / 100 + 0.5f).ToString ("0.000") + "\n";
				break;
			}
			//sections.RemoveAt (r);
			text += '\n';
		}

		report.text = text;
		height = text.Split ('\n').Length * 30;
		rect = report.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (500, height);
		//rect.position = new Vector3 (rect.position.x, -height / 2 + 80, rect.position.z);
	}
}