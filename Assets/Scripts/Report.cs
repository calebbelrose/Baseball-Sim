using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Report : MonoBehaviour
{
	List<Player> players;
	List<int> sections = new List<int>();

	void Start ()
	{
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

		while(Manager.Instance.tradeList.Count > 0)
		{
			//text += Manager.Instance.tradeList [0];
			Manager.Instance.tradeList.RemoveAt (0);
		}

		for(int i = 0; i < Manager.Instance.teams.Count; i++)
			for(int j = 0; j < Manager.Instance.teams[i].players.Count; j++)
				players.Add(Manager.Instance.Players[Manager.Instance.teams[i].players[j]]);

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
				result = players.OrderByDescending (playerX => playerX.stats[0][7]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][7] + "\n";
				break;

			case 3:
			// Most Innings Pitcher
				text += "Most Innings Pitched\n";
				result = players.OrderByDescending (playerX => playerX.stats[0][20]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][20] / 3 + "." + result [i].stats[0][20] % 3 + "\n";
				break;

			case 4:
			// Lowest WHIP
				text += "Lowest WHIP\n";
				result = players.OrderBy (playerX => playerX.WHIP()).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + ((result [i].stats[0][26] + result [i].stats[0][22]) / (result [i].stats[0][20] / 3.0f)).ToString ("0.00") + "\n";
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
				result = players.OrderByDescending (playerX => playerX.stats[0][0]).ToList ();
				j = 0;
				l = 0;
				while (j < 10 )
				{
					if (result [l].position.Length == 2 && result [l].position.Substring (1) == "P")
					{
						text += result [l].firstName.PadRight (Player.longestFirstName) + " " + result [l].lastName.PadRight (Player.longestLastName) + " " + result [l].stats[0][0] + "\n";
						j++;
					}
					l++;
				}
				break;
			case 7:
			// RBIs
				text += "Most RBIs\n";
				result = players.OrderByDescending (playerX => playerX.stats[0][9]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][9] + "\n";
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
				result = players.OrderBy (playerX => playerX.stats[0][25]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][25] + "\n";
				break;

			case 12:
			// Walks Against
				text += "Lowest Walks Against\n";
				result = players.OrderBy (playerX => playerX.stats[0][26]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][26] + "\n";
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
				result = players.OrderByDescending (playerX => playerX.stats[0][22]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][22] + "\n";
				break;

			case 15:
			// ERA
				text += "Lowest Earned Run Average\n";
				result = players.OrderBy (playerX => (playerX.stats[0][24] / (playerX.stats[0][20] / 3.0f) * 9)).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + (result [i].stats[0][24] / (result [i].stats[0][20] / 3.0f) * 9) + "\n";
				break;

			case 16:
			// Salary
				text += "Most Paid Players\n";
				result = players.OrderBy (playerX => playerX.Salary).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].Salary + "\n";
				break;

			case 17:
			// Hit Streak
				text += "Longest Hit Streaks (Currently " + Manager.Instance.longestHitStreak + " by " + Manager.Instance.hitStreakName + " in " + Manager.Instance.hitStreakYear + "\n";
				result = players.OrderByDescending (playerX => playerX.stats[0][26]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][26] + "\n";
				break;

			case 18:
			// Quality Starts
				text += "Most Quality Starts\n";
				result = players.OrderByDescending (playerX => playerX.stats[0][26]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].firstName.PadRight (Player.longestFirstName) + " " + result [i].lastName.PadRight (Player.longestLastName) + " " + result [i].stats[0][26] + "\n";
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
				result = players.OrderBy (playerX => playerX.stats[0][0]).ThenByDescending (playerY => (playerY.skills [0] + playerY.skills [1] + playerY.skills [2] + playerY.skills [3] + playerY.skills [4] + playerY.skills [5] + playerY.skills [6] + playerY.potential)).ToList ();
				j = 0;
				l = 0;
				while (j < 10 && result [l].stats[0][0] == 0) {
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
				result = players.OrderBy (playerX => playerX.stats[0][0]).ThenByDescending (playerY => (playerY.skills [0] + playerY.skills [1] + playerY.skills [2] + playerY.skills [3] + playerY.skills [4] + playerY.skills [5] + playerY.skills [6] + playerY.potential)).ToList ();
				j = 0;
				l = 0;
				while (j < 10 && result [l].stats[0][0] == 0)
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
				resultT = Manager.Instance.teams.OrderByDescending (teamX => teamX.Overalls [0]).ToList ();
				for (int i = 0; i < resultT.Count; i++)
					total += resultT [i].Overalls [0];
				total /= resultT.Count;
				for (int i = 0; i < resultT.Count; i++)
					text += resultT [i].CityName.PadRight (Team.longestCityName) + " " + resultT [i].TeamName.PadRight (Team.longestTeamName) + " " + ((resultT [i].Overalls [0] - total) / 100 + 0.5f).ToString ("0.000") + "\n";
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