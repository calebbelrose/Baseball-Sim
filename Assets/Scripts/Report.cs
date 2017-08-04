using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Report : MonoBehaviour
{
	List<Player> players;
	List<int> sections = new List<int> ();

	void Start ()
	{
		players = new List<Player> ();
		GenerateReport ();
	}

	// Generates a report to show worst/best players/teams
	void GenerateReport ()
	{
		List<Player> result;
		List<Team> resultT;
		string text = "";
		Text report = GameObject.Find ("test").GetComponent<Text> ();
		int height;
		RectTransform rect;

		for (int i = 1; i < 25; i++)
			sections.Add (i);

		while (Manager.Instance.tradeList.Count > 0)
		{
			//text += Manager.Instance.tradeList [0];
			Manager.Instance.tradeList.RemoveAt (0);
		}

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			for (int j = 0; j < Manager.Instance.Teams [0] [i].Players.Count; j++)
				players.Add (Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]]);

		for (int k = 0; k < 10; k++)
		{
			int r = (int) (Random.value * sections.Count);

			switch (sections [r])
			{
			case 1:
			// Power/Speed Number
				text += "Highest Power/Speed Number\n";
				result = players.OrderByDescending (playerX => playerX.PSN).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].PSN.ToString ("0.00") + "\n";
				break;
			case 2:
			// Most Homeruns
				text += "Most Homeruns\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [7]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [7] + "\n";
				break;

			case 3:
			// Most Innings Pitcher
				text += "Most Innings Pitched\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [20]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [20] / 3 + "." + result [i].Stats [0] [20] % 3 + "\n";
				break;

			case 4:
			// Lowest WHIP
				text += "Lowest WHIP\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => playerX.WHIP).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + ((result [i].Stats [0] [26] + result [i].Stats [0] [22]) / (result [i].Stats [0] [20] / 3.0f)).ToString ("0.00") + "\n";
				break;

			case 5:
			// RC/27
				text += "Highest RC/27\n";
				result = players.OrderByDescending (playerX => playerX.RC27).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].RC27.ToString ("0.00") + "\n";
				break;

			case 6:
			// Pitcher Appearances
				text += "Most Pitcher Appearances\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenByDescending (playerX => playerX.Stats [0] [0]).ToList ();
				for (int i = 0; i < 10; i++)
						text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [0] + "\n";
				break;
			case 7:
			// RBIs
				text += "Most RBIs\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [9]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [9] + "\n";
				break;

			case 8:
			// Isoated Power
				text += "Most Isolated Power\n";
				result = players.OrderByDescending (playerX => playerX.ISO).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].ISO.ToString ("0.000") + "\n";
				break;

			case 9:
			// Batting Average
				text += "Highest Batting Average\n";
				result = players.OrderByDescending (playerX => playerX.BA).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].BA.ToString ("0.000") + "\n";
				break;

			case 10:
			// Batting Average Against
				text += "Lowest Batting Average Against\n";
				result = players.OrderBy (playerX => playerX.BAA).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].BAA.ToString ("0.000") + "\n";
				break;

			case 11:
			// Homeruns Against
				text += "Least Homeruns Against\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => playerX.Stats [0] [25]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [25] + "\n";
				break;

			case 12:
			// Walks Against
				text += "Lowest Walks Against\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => playerX.Stats [0] [26]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [26] + "\n";
				break;

			case 13:
			// Age
				text += "Oldest Players\n";
				result = players.OrderByDescending (playerX => playerX.Age).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Age + "\n";
				break;

			case 14:
			// Hits Allowed
				text += "Most Hits Allowed\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [22]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [22] + "\n";
				break;

			case 15:
			// ERA
				text += "Lowest Earned Run Average\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => playerX.ERA).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].ERA.ToString("F") + "\n";
				break;

			case 16:
			// Salary
				text += "Most Paid Players\n";
				result = players.OrderByDescending (playerX => playerX.ContractYears [0].Salary).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].ContractYears [0].Salary + "\n";
				break;

			case 17:
			// Hit Streak
				text += "Longest Hit Streaks (Currently " + Manager.Instance.longestHitStreak + " by " + Manager.Instance.hitStreakName + " in " + Manager.Instance.hitStreakYear + "\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [33]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [26] + "\n";
				break;

			case 18:
			// Quality Starts
				text += "Most Quality Starts\n";
				result = players.OrderByDescending (playerX => playerX.Stats [0] [28]).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].Stats [0] [26] + "\n";
				break;

			case 19:
			// Injuries
				text += "Injuries\n";
				result = players.OrderByDescending (playerX => playerX.InjuryLength).ToList ();
				int j = 0;
				while (result [j].InjuryLength != 0)
					text += result [j].FirstName.PadRight (Player.longestFirstName) + " " + result [j].LastName.PadRight (Player.longestLastName) + " is injured with a " + result [j].InjurySeriousness + " " + result [j].InjuryLocation + " injury for " + result [j++].InjuryLength + " days\n";
				break;
			case 20:
			// Linear Weights Ratio
				text += "Highest Linear Weights Ratio\n";
				result = players.OrderByDescending (playerX => playerX.LWR).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].LWR.ToString ("0.000") + "\n";
				break;
			case 21:
			// Walk-to-strikeout ratio
				text += "Highest Walk-to-Strikeout Ratio\n";
				result = players.OrderByDescending (playerX => playerX.BBToK).ToList ();
				for (int i = 0; i < 10; i++)
					text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + " " + result [i].BBToK.ToString ("0.00") + "\n";
				break;
			case 22:
			// Pitching Prospects
				text += "Best Pitching Prospects\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => playerX.Stats [0] [0]).ThenByDescending (playerY => (playerY.Skills [0] + playerY.Skills [1] + playerY.Skills [2] + playerY.Skills [3] + playerY.Skills [4] + playerY.Skills [5] + playerY.Skills [6] + playerY.Potential)).ToList ();
				for (int i = 0; i < 10; i++)
						text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + (result [i].Skills [0] + result [i].Skills [1] + result [i].Skills [2] + result [i].Skills [3] + result [i].Skills [4] + result [i].Skills [5] + result [i].Skills [6] + result [i].Potential) + "\n";
				break;
			case 23:
			// Batting Prospects
				text += "Best Batting Prospects\n";
				result = players.OrderByDescending (playerX => playerX.IsPitcher).ThenBy (playerX => (playerX.Skills [0] + playerX.Skills [1] + playerX.Skills [2] + playerX.Skills [3] + playerX.Skills [4] + playerX.Skills [5] + playerX.Skills [6] + playerX.Potential)).ToList ();
				for (int i = 0; i < 10; i++)
						text += result [i].FirstName.PadRight (Player.longestFirstName) + " " + result [i].LastName.PadRight (Player.longestLastName) + (result [i].Skills [0] + result [i].Skills [1] + result [i].Skills [2] + result [i].Skills [3] + result [i].Skills [4] + result [i].Skills [5] + result [i].Skills [6] + result [i].Potential) + "\n";
				break;
			case 24:
			// Projected Wins
				text += "Projected Wins\n";
				float total = 0;
				resultT = Manager.Instance.Teams [0].OrderByDescending (teamX => teamX.Overalls [0]).ToList ();
				for (int i = 0; i < resultT.Count; i++)
					total += resultT [i].Overalls [0];
				total /= resultT.Count;
				for (int i = 0; i < resultT.Count; i++)
				{
					int gamesLeft = 162 - resultT [i].Wins - resultT [i].Losses;
					int projectedWinsLeft = (int)(((resultT [i].Overalls [0] - total) / 100 + 0.5f) * gamesLeft);
					int projectedLossesLeft = gamesLeft - projectedWinsLeft;

					text += resultT [i].CityName.PadRight (Team.longestCityName) + " " + resultT [i].TeamName.PadRight (Team.longestTeamName) + " " + ((projectedWinsLeft + resultT [i].Wins) / (projectedLossesLeft + resultT [i].Losses)).ToString ("0.000") + "\n";
				}
				break;
			}

			sections.RemoveAt (r);
			text += '\n';
		}

		report.text = text;
		height = text.Split ('\n').Length * 30;
		rect = report.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (500, height);
		//rect.Position = new Vector3 (rect.Position.x, -height / 2 + 80, rect.Position.z);
	}
}