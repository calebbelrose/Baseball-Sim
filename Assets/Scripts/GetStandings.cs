using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetStandings : MonoBehaviour
{
	public RectTransform viewport;					// Viewport for the standings
	public RectTransform content;					// Holds the header and standing objects
	public RectTransform scrollRect;				// ScrollRect for the standings
	public Transform teamListHeader;				// Holds the headers

	// Headers
	private string [] headers = new string [] { "Team Name", "Wins", "Losses", "Percent", "GB    ", "Last 10", "Streak", "Home ", "Away ", "AVG  ", "ERA" };
	private int currSortedStat = 3;					// Current sorted stat
	private int longestTeamName = 0;				// Length of the longest team name
	private bool ascending = true;					// Whether it's sorted ascending or descending
	private List<Team> teams = new List<Team> ();	// List of all teams

	void Start ()
	{
		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			if (Manager.Instance.Teams [0] [i].CityName.Length + Manager.Instance.Teams [0] [i].TeamName.Length + 1 > longestTeamName)
				longestTeamName = Manager.Instance.Teams [0] [i].CityName.Length + Manager.Instance.Teams [0] [i].TeamName.Length + 1;

		teams = Manager.Instance.Teams [0].OrderBy (teamX => teamX.Division).ThenBy (teamY => teamY.League).ThenBy (teamZ => teamZ.Wins).ToList ();
		DisplayHeader ();
		DisplayTeams ();
	}

	// Displays header
	void DisplayHeader ()
	{
		int standingsHeaderLength = longestTeamName, maxIndex = headers.Length - 1;
		int [] headerLengths = new int [headers.Length];
		Object header = Resources.Load ("Header", typeof (GameObject));
		float newWidth = 0.0f, currWidth;
		GameObject statHeader;

		for (int i = 1; i < headers.Length; i++)
			standingsHeaderLength += headers [i].Length + 1;

		headerLengths [0] = longestTeamName + 1;

		for (int i = 1; i < headers.Length; i++)
			headerLengths [i] = headers [i].Length + 1;

		for (int i = 0; i < maxIndex; i++)
		{
			statHeader = Instantiate (header) as GameObject;
			currWidth = (8.03f * (headerLengths [i]));
			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (teamListHeader);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = headers [i];
			statHeader.GetComponent<Button> ().interactable = false;
			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		statHeader = Instantiate (header) as GameObject;
		statHeader.name = "header" + maxIndex.ToString ();
		statHeader.transform.SetParent (teamListHeader.transform);
		statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = headers [maxIndex];
		statHeader.GetComponent<Button> ().onClick.AddListener (() => StartSorting (statHeader.name));
		statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (scrollRect.rect.width - newWidth, 20.0f);
	}

	// Displays teams
	public void DisplayTeams ()
	{
		GameObject [] currTeams = GameObject.FindGameObjectsWithTag ("Team");

		for (int i = 0; i < currTeams.Length; i++)
			Destroy (currTeams [i]);

		for (int l = 0; l < 2; l++)
		{
			Object leagueHeader = Resources.Load ("Team", typeof (GameObject));
			GameObject newLeagueHeader = Instantiate (leagueHeader) as GameObject;
			char league;
			Text leagueText = newLeagueHeader.transform.GetChild (0).gameObject.GetComponent<Text> ();

			if (l == 0)
			{
				newLeagueHeader.name = "headerAL";
				newLeagueHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "American League";
				league = 'A';
			}
			else
			{
				newLeagueHeader.name = "headerNL";
				newLeagueHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "National League";
				league = 'N';
			}

			leagueText.fontStyle = FontStyle.Bold;
			leagueText.fontSize = 18;
			newLeagueHeader.transform.SetParent (transform);
			newLeagueHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			newLeagueHeader.GetComponent<Button> ().interactable = false;

			for (int m = 0; m < 3; m++)
			{
				int start = l * 15 + m * 5, end = start + 5;
				Object divisionHeader = Resources.Load ("Team", typeof (GameObject));
				GameObject newDivisionHeader = Instantiate (divisionHeader) as GameObject;
				int leaderWins, leaderLosses;
				Text divisionText = newDivisionHeader.transform.GetChild (0).gameObject.GetComponent<Text> ();

				switch (m)
				{
				case 0:
					newDivisionHeader.name = "header" + league + "C";
					divisionText.text = "Central Division";
					break;
				case 1:
					newDivisionHeader.name = "header" + league + "E";
					divisionText.text = "East Division";
					break;
				case 2:
					newDivisionHeader.name = "header" + league + "W";
					divisionText.text = "West Division";
					break;
				}

				divisionText.fontStyle = FontStyle.Bold;
				divisionText.fontSize = 16;
				newDivisionHeader.transform.SetParent (transform);
				newDivisionHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
				newDivisionHeader.GetComponent<Button> ().interactable = false;
				leaderWins = teams [start].Wins;
				leaderLosses = teams [start].Losses;

				for (int i = start; i < end; i++)
				{
					Object teamButton = Resources.Load ("Team", typeof (GameObject));
					GameObject newTeam = Instantiate (teamButton) as GameObject;
					string teamListing = teams [i].CityName + " " + teams [i].TeamName;
					string thisStat;
					int hits = 0, abs = 0, earnedRuns = 0, innings = 0, wins = 0, losses = 0;

					newTeam.name = "team" + i.ToString ();
					newTeam.transform.SetParent (transform);
					teams [i].SetStats ();
					teams [i].GamesBehind = -((leaderWins - teams [i].Wins) + (teams [i].Losses - leaderLosses)) / 2.0f;

					for (int j = teamListing.Length - 1; j < longestTeamName; j++)
						teamListing += " ";

					thisStat = teams [i].Wins.ToString ();
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [1].Length; j++)
						teamListing += " ";

					thisStat = teams [i].Losses.ToString ();
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [2].Length; j++)
						teamListing += " ";

					if (teams [i].Wins == 0)
						thisStat = "0.000";
					else if (teams [i].Losses == 0)
						thisStat = "1.000";
					else
						thisStat = (teams [i].Wins / (float)teams [i].Losses).ToString ("N3");
					
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [3].Length; j++)
						teamListing += " ";

					if (teams [i].GamesBehind == 0)
						thisStat = "-";
					else
						thisStat = teams [i].GamesBehind.ToString ("0.0");

					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [4].Length; j++)
						teamListing += " ";

					for (int k = 0; k < teams [i].Players.Count; k++)
					{
						hits += Manager.Instance.Players [teams [i].Players [k]].Stats [0] [3];
						abs += Manager.Instance.Players [teams [i].Players [k]].Stats [0] [1];
						earnedRuns += Manager.Instance.Players [teams [i].Players [k]].Stats [0] [24];
						innings += Manager.Instance.Players [teams [i].Players [k]].Stats [0] [20];
					}

					for (int k = 0; k < teams [i].LastTen.Count; k++)
						if (teams [i].LastTen [k])
							wins++;
						else
							losses++;

					thisStat = wins + "-" + losses;
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [5].Length; j++)
						teamListing += " ";

					if (teams [i].Streak == 0)
						thisStat = "0";
					else
					{
						if (teams [i].WinStreak)
							thisStat = "W";
						else
							thisStat = "L";
						
						thisStat += teams [i].Streak;
					}

					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [6].Length; j++)
						teamListing += " ";

					thisStat = teams [i].HomeWins + "-" + teams [i].HomeLosses;
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [7].Length; j++)
						teamListing += " ";

					thisStat = teams [i].AwayWins + "-" + teams [i].AwayLosses;
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [8].Length; j++)
						teamListing += " ";

					if (hits == 0 || abs == 0)
						thisStat = "0.000";
					else
						thisStat = (hits / (float)abs).ToString ("N3");
					
					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [9].Length; j++)
						teamListing += " ";

					if (innings == 0)
						thisStat = "0.00";
					else
						thisStat = (earnedRuns / (float)innings).ToString ("N");

					teamListing += " " + thisStat;

					for (int j = thisStat.Length; j < headers [10].Length; j++)
						teamListing += " ";

					//0"Team Name", 1"Wins", 2"Losses", 3"Pct", 4"Games Behind", 5"L10", 6"Streak", 7"Home", 8"Away", 9"AVG", 10"ERA"

					newTeam.transform.GetChild (0).gameObject.GetComponent<Text> ().text = teamListing;
					newTeam.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					newTeam.GetComponent<Button> ().interactable = false;

					if (teams [i].ID == 0)
					{
						Button b = newTeam.GetComponent<Button> ();
						ColorBlock c = b.colors;
						c.disabledColor = new Color (1.0f, 1.0f, 0.0f);
						b.colors = c;
					}
				}
			}
		}

		content.sizeDelta = new Vector2 (viewport.rect.width, 20.0f * (teams.Count + 9));
	}

	// Sorts the teams based on the specified stat
	public void StartSorting (string name)
	{
		int left = 0, right = teams.Count - 1, statNum = int.Parse (name.Remove (0, 6));
		string pivot;
		int test;
		bool notString;

		if (statNum < 3)
			pivot = teams [(int)(left + (right - left) / 2)].GetStats () [statNum];
		else
			pivot = teams [(int)(left + (right - left) / 2)].GamesBehind.ToString ();

		notString = int.TryParse (pivot, out test);

		if (currSortedStat == statNum)
			ascending = !ascending;
		else if (notString)
			ascending = false;
		else
			ascending = true;

		currSortedStat = statNum;
		DisplayTeams ();
	}
}
