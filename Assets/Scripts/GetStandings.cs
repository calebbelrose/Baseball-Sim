using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetStandings : MonoBehaviour {

    GameObject teamList;
    GameObject manager;
    AllTeams allTeams;
    string[] headers = new string[] { "Team Name", "Wins", "Losses", "Games Behind" };
    int currSortedStat = 3;
    char order = 'd';
    int longestTeamName = 0;
	List<Team> teams = new List<Team>();

    void Start()
    {
        teamList = GameObject.Find("TeamList");
        manager = GameObject.Find("_Manager");
        allTeams = manager.GetComponent<AllTeams>();

        for (int i = 0; i < allTeams.teams.Count; i++)
        {
            if (allTeams.teams[i].cityName.Length + allTeams.teams[i].teamName.Length + 1 > longestTeamName)
                longestTeamName = allTeams.teams[i].cityName.Length + allTeams.teams[i].teamName.Length + 1;
        }

		teams = allTeams.teams.OrderBy(teamX => teamX.division).ThenBy (teamY => teamY.league).ThenBy(teamZ => teamZ.wins).ToList ();
        DisplayHeader();
        DisplayTeams();
    }

    void DisplayHeader()
    {
        int standingsHeaderLength = longestTeamName;
        GameObject teamListHeader = GameObject.Find("StandingsHeader");

        for (int i = 1; i < headers.Length; i++)
        {
            standingsHeaderLength += headers[i].Length + 1;
        }

        int[] headerLengths = new int[headers.Length];
        headerLengths[0] = longestTeamName + 1;

        for (int i = 1; i < headers.Length; i++)
            headerLengths[i] = headers[i].Length + 1;

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = -10.0f, newWidth = 0.0f;
        float totalWidth = (8.04f * (standingsHeaderLength));
        teamList.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(20 * (teams.Count + 8) - teamList.transform.parent.gameObject.GetComponent<RectTransform>().rect.height));
        totalWidth /= -2.0f;

        for (int i = 0; i < headers.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(teamListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = headers[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader.name));

            float currWidth = (8.04f * (headerLengths[i]));
            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        teamList.GetComponent<RectTransform>().offsetMax = new Vector2(newWidth - 160.0f, 0);
    }

    public void DisplayTeams()
    {
        GameObject[] currTeams = GameObject.FindGameObjectsWithTag("Team");

        for (int i = 0; i < currTeams.Length; i++)
            Destroy(currTeams[i]);

		for(int l = 0; l < 2; l++)
		{
			Object leagueHeader = Resources.Load("Team", typeof(GameObject));
			GameObject newLeagueHeader = Instantiate(leagueHeader) as GameObject;
			char league;

			if (l == 0) {
				newLeagueHeader.name = "headerAL";
				newLeagueHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "American League";
				league = 'A';
			} else {
				newLeagueHeader.name = "headerNL";
				newLeagueHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "National League";
				league = 'N';
			}

			newLeagueHeader.transform.SetParent(teamList.transform);
			newLeagueHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			newLeagueHeader.GetComponent<Button>().interactable = false;

			for(int m = 0; m < 3; m++)
			{
				int start = l * 15 + m * 5, end = start + 5;
				Object divisionHeader = Resources.Load("Team", typeof(GameObject));
				GameObject newDivisionHeader = Instantiate(divisionHeader) as GameObject;
				int leaderWins, leaderLosses;

				switch (m) {
				case 0:
					newDivisionHeader.name = "header" + league + "C";
					newDivisionHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "Central Division";
					break;
				case 1:
					newDivisionHeader.name = "header" + league + "E";;
					newDivisionHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "East Division";
					break;
				case 2:
					newDivisionHeader.name = "header" + league + "W";;
					newDivisionHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "West Division";
					break;
				}

				newDivisionHeader.transform.SetParent(teamList.transform);
				newDivisionHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				newDivisionHeader.GetComponent<Button>().interactable = false;

				leaderWins = teams [start].wins;
				leaderLosses = teams [start].losses;

				for (int i = start; i < end; i++)
        {
            Object teamButton = Resources.Load("Team", typeof(GameObject));
            GameObject newTeam = Instantiate(teamButton) as GameObject;

            newTeam.name = "team" + i.ToString();
            newTeam.transform.SetParent(teamList.transform);
            teams[i].SetStats();
            string teamListing = teams[i].cityName + " " + teams[i].teamName;

            for (int j = teamListing.Length - 1; j < longestTeamName; j++)
                teamListing += " ";

            for (int j = 1; j < headers.Length - 1; j++)
            {
                teamListing += " " + teams[i].GetStats()[j];

                for (int k = teams[i].GetStats()[j].Length; k < headers[j].Length; k++)
                    teamListing += " ";
            }

					if (i == start)
						teamListing += " -";
					else
						teamListing += " " + (((leaderWins - teams [i].wins) + (teams[i].losses - leaderLosses)) / 2.0).ToString("0.0");


            newTeam.transform.GetChild(0).gameObject.GetComponent<Text>().text = teamListing;
            newTeam.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newTeam.GetComponent<Button>().interactable = false;
            if (teams[i].teamName == allTeams.teams[0].teamName)
            {
                Button b = newTeam.GetComponent<Button>();
                ColorBlock c = b.colors;
                c.disabledColor = new Color(1.0f, 1.0f, 0.0f);
                b.colors = c;
            }
        }
	}
		}
	}

    public void StartSorting(string name)
    {
        int left = 0, right = teams.Count - 1, statNum = int.Parse(name.Remove(0, 6));
        string pivot = teams[(int)(left + (right - left) / 2)].GetStats()[statNum];
        int test;
        bool notString = int.TryParse(pivot, out test);

        if (currSortedStat == statNum)
            if (order == 'a')
                order = 'd';
            else
                order = 'a';
        else
            if (notString)
            order = 'd';
        else
            order = 'a';
        currSortedStat = statNum;
        DisplayTeams();
    }
}
