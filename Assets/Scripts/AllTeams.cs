using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class AllTeams : MonoBehaviour {

    static int numTeams = 30;								// Number of teams
	public List<Team> teams = new List<Team>();				// List of all teams
    public int[,] schedule = new int[numTeams / 2, 2];		// Schedule of the next games to be played
    public int numPlays, numStats, currStarter;		// 
    public bool needDraft, inFinals;
    public string[] stats = File.ReadAllLines("Stats.txt");
	public List<string> tradeList;
	public List<string> injuries;
	public int longestHitStreak = 0, hitStreakYear;
	public string hitStreakName;
	public TradeDeadline tradeDeadline;
	public int rosterSize = 25;
	public List<Player> hallOfFameCandidates;
	public List<HallOfFameInductee> hallOfFameInductees = new List<HallOfFameInductee>();

	private float newWidth;
	private Draft draft;
	private PlayerDisplay playerDisplay;
	private List<Day> days = new List<Day>();
	private int dayIndex;
	private int year;

	public int Year
	{
		get
		{
			return year;
		}
	}

	public int DayIndex
	{
		get
		{
			return dayIndex;
		}
	}

	public List<Day> Days
	{
		get
		{
			return days;
		}
	}

    // Use this for initialization
    void Awake ()
	{
		// Loads everything after starting the game after having already played
        if (PlayerPrefs.HasKey("Year"))
        {
            year = int.Parse(PlayerPrefs.GetString("Year"));
            numPlays = PlayerPrefs.GetInt("NumPlays");
            currStarter = PlayerPrefs.GetInt("CurrStarter");
            needDraft = bool.Parse(PlayerPrefs.GetString("NeedDraft"));
            inFinals = bool.Parse(PlayerPrefs.GetString("InFinals"));
            
            for (int i = 0; i < numTeams; i++)
            {
                Team team = new Team();
                int numPlayers = PlayerPrefs.GetInt("NumPlayers" + i);
                string teamInfo = PlayerPrefs.GetString("Team" + i);
                string teamOveralls = PlayerPrefs.GetString("Overalls" + i);
                string[] teamInfoSplit = teamInfo.Split(',');
                string[] teamOverallsSplit = teamOveralls.Split(',');
                string[] wl;
                string[] splitName;

                for (int j = 0; j < numPlayers; j++)
                {
					Player newPlayer = new Player ();

					newPlayer.LoadPlayer (i, j);

                    team.players.Add(newPlayer);
					team.currentSalary += newPlayer.salary;
                }

                team.id = int.Parse(teamInfoSplit[0]);
                team.cityName = teamInfoSplit[1];
                team.teamName = teamInfoSplit[2];
                team.Pick = int.Parse(teamInfoSplit[3]);
                team.overalls[0] = float.Parse(teamOverallsSplit[0]);
                team.overalls[1] = float.Parse(teamOverallsSplit[1]);
                team.overalls[2] = float.Parse(teamOverallsSplit[2]);
                splitName = (team.cityName + " " + team.teamName).Split(' ');

                for (int j = 0; j < splitName.Length; j++)
                    if (System.Char.IsLetter(splitName[j][0]) && System.Char.IsUpper(splitName[j][0]))
                        team.shortform += splitName[j][0];
                
                wl = PlayerPrefs.GetString("WL" + i).Split(',');
				team.wins = int.Parse(wl [0]);
				team.losses = int.Parse(wl [1]);

                for(int j = 0; j < 5; j++)
                    team.SP.Add(PlayerPrefs.GetInt("SP" + i + "-" + j));

                for(int j = 0; j < 9; j++)
                    team.Batters.Add(PlayerPrefs.GetInt("Batter" + i + "-" + j));

                for (int j = 0; j < 3; j++)
                    team.RP.Add(PlayerPrefs.GetInt("RP" + i + "-" + j));

                for (int j = 0; j < 1; j++)
                    team.CP.Add(PlayerPrefs.GetInt("CP" + i + "-" + j));

				teams.Add (team);
            }

			Player.longestFirstName = PlayerPrefs.GetInt ("LongestFirstName");
			Player.longestLastName = PlayerPrefs.GetInt ("LongestLastName");
            
            string fullSchedule = PlayerPrefs.GetString("Schedule");
            string[] tempSchedule = fullSchedule.Split(',');
            for (int i = 0; i < tempSchedule.Length; i++)
            {
                schedule[i / 2, i % 2] = int.Parse(tempSchedule[i]);
            }
        }
		// Creates a new game if it is the first time playing
        else
            Restart();

		Day.SetAllTeams (this);

		draft = new Draft (this);
		playerDisplay = new PlayerDisplay (this);

		int numDays = (DateTime.Parse (year + "/12/31") - DateTime.Parse (year + "/01/01")).Days;

		DateTime currDate = DateTime.Parse (year + "/01/01");

		for (int i = 0; i <= numDays; i++)
		{
			days.Add (new Day (currDate));
			currDate = currDate.AddDays(1);
		}
    }

	// Returns the number of teams
	public int GetNumTeams()
    {
        return numTeams;
    }

    public void Restart()
	{
		List<int> picksLeft = new List<int> ();	// List of picks left for the draft
		string strSchedule = "";				// Schedule in string format

		dayIndex = 0;
		year = DateTime.Now.Year;
		PlayerPrefs.SetString ("Year", year.ToString ());
		numPlays = 0;
		PlayerPrefs.SetString ("NumPlays", numPlays.ToString ());
		needDraft = true;
		PlayerPrefs.SetString ("NeedDraft", needDraft.ToString ());
		inFinals = false;
		PlayerPrefs.SetString ("InFinals", inFinals.ToString ());
		currStarter = 0;
		PlayerPrefs.SetInt ("CurrStarter", currStarter);
		tradeList = new List<string> ();

		// Adds the draft picks to the list
		for (int i = 0; i < numTeams; i++)
			picksLeft.Add (i);

		// Creates each team with 1 of batting position, 5 Starting Pitcher, 3 Relief Pitcher and 1 Closing Pitcher
		for (int i = 0; i < numTeams; i++) {
			Team team = new Team ();
			string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
			Player newPlayer;
			string[] splitName = (team.cityName + " " + team.teamName).Split(' ');
			int pickIndex = (int)(UnityEngine.Random.value * picksLeft.Count);

			team.id = i;
			team.Pick = picksLeft [pickIndex];
			picksLeft.RemoveAt (pickIndex);
			for (int j = 0; j < splitName.Length; j++)
				if (System.Char.IsLetter(splitName[j][0]) && System.Char.IsUpper(splitName[j][0]))
					team.shortform += splitName[j][0];

			for (int j = 3; j < positions.Length; j++) {
				newPlayer = new Player (positions [j], 23, 22, team.players.Count);

				newPlayer.SavePlayer (i);
				PlayerPrefs.SetInt ("Batter" + i + "-" + team.Batters.Count, newPlayer.PlayerIndex);
				team.AddToMajors (newPlayer.PlayerIndex);
				team.Batters.Add (newPlayer.PlayerIndex);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			for (int j = 0; j < 5; j++) {
				newPlayer = new Player ("SP", 23, 22, team.players.Count);

				newPlayer.SavePlayer (i);
				PlayerPrefs.SetInt ("SP" + i + "-" + team.SP.Count, newPlayer.PlayerIndex);
				team.AddToMajors (newPlayer.PlayerIndex);
				team.SP.Add (newPlayer.PlayerIndex);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			for (int j = 0; j < 3; j++) {
				newPlayer = new Player ("RP", 23, 22, team.players.Count);

				newPlayer.SavePlayer (i);
				PlayerPrefs.SetInt ("RP" + i + "-" + team.RP.Count, newPlayer.PlayerIndex);
				team.AddToMajors (newPlayer.PlayerIndex);
				team.RP.Add (newPlayer.PlayerIndex);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			newPlayer = new Player ("CP", 23, 22, team.players.Count);

			newPlayer.SavePlayer (i);
			PlayerPrefs.SetInt ("CP" + i + "-" + team.CP.Count, newPlayer.PlayerIndex);
			team.AddToMajors (newPlayer.PlayerIndex);
			team.CP.Add (newPlayer.PlayerIndex);
			team.players.Add (newPlayer);
			team.currentSalary += newPlayer.salary;

			team.CalculateOveralls ();

			PlayerPrefs.SetString ("Team" + team.id, team.id + "," + team.cityName + "," + team.teamName + "," + team.Pick);
			PlayerPrefs.SetString ("Overalls" + team.id, team.overalls [0] + "," + team.overalls [1] + "," + team.overalls [2]);
			PlayerPrefs.SetString ("WL" + team.id.ToString (), "0,0");
			PlayerPrefs.SetInt ("NumPlayers" + i, team.players.Count);

			teams.Add (team);
		}

		// Stores the length of the longest first and last name to be able to display them properly
		PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
		PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

		// Creates the schedule for the first games
        for (int i = 0; i < numTeams; i++)
        {
            schedule[i / 2, i % 2] = i;
            strSchedule += i + ",";
        }

        strSchedule = strSchedule.Remove(strSchedule.Length - 1, 1);
        PlayerPrefs.SetString("Schedule", strSchedule);

        PlayerPrefs.Save();
    }

	public void DisplayHeaders(Transform headerTrans, RectTransform parentsParentRect, bool draftHeaders)
	{
		int statHeaderLength = 0;
		float characterWidth = 8.03f;

		newWidth = 0.0f;

		statHeaderLength += Player.longestFirstName + Player.longestLastName + 2;

        for (int i = 2; i < stats.Length; i++)
			statHeaderLength += stats[i].Length + 1;

        UnityEngine.Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = 5.0f;
        float totalWidth = (characterWidth * (statHeaderLength + 1.0f));
        totalWidth /= -2.0f;

		for (int i = 0; i < stats.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
			int headerNum = i;

            statHeader.name = "header" + i.ToString();
			statHeader.transform.SetParent(headerTrans);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = stats[i];

			if(draftHeaders)
				statHeader.GetComponent<Button>().onClick.AddListener(() => draft.Sort(headerNum));
			else
				statHeader.GetComponent<Button>().onClick.AddListener(() => playerDisplay.Sort(headerNum));

            float currWidth;
            if (i > 1)
				currWidth = (characterWidth * (stats[i].Length + 1));
            else if (i == 1)
				currWidth = (characterWidth * (Player.longestLastName + 1));
            else
				currWidth = (characterWidth * (Player.longestFirstName + 1));

            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

		newWidth -= parentsParentRect.rect.width;
	}

	public void DisplayPlayers(List<Player> players, Transform parentTrans, RectTransform parentRect, RectTransform parentsParentRect, bool draftPlayers)
	{
		GameObject[] currPlayers;
		UnityEngine.Object playerButton;
		string playerName;

		if (draftPlayers)
		{
			playerButton= Resources.Load("DraftPlayer", typeof(GameObject));
			currPlayers = GameObject.FindGameObjectsWithTag ("DraftPlayer");
			playerName = "draft";
		}
		else
		{
			playerButton = Resources.Load("Player", typeof(GameObject));
			currPlayers = GameObject.FindGameObjectsWithTag ("Player");
			playerName = "player";
		}

        for (int i = 0; i < currPlayers.Length; i++)
            Destroy(currPlayers[i]);

		for (int i = 0; i < players.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
			string playerListing;

			newPlayer.name = playerName + i.ToString();
			newPlayer.transform.SetParent(parentTrans);

			playerListing = players[i].firstName;

			for (int j = players[i].firstName.Length; j < Player.longestFirstName; j++)
				playerListing += " ";

			playerListing += " " + players[i].lastName;

			for (int j = players[i].lastName.Length; j < Player.longestLastName; j++)
				playerListing += " ";

			playerListing += " " + players[i].position;

			for (int k = players[i].position.Length; k < stats[2].Length; k++)
				playerListing += " ";

			playerListing += " " + players[i].overall;

			for (int k = players[i].overall.ToString().Length; k < stats[3].Length; k++)
				playerListing += " ";

			playerListing += " " + players[i].offense;

			for (int k = players[i].offense.ToString().Length; k < stats[4].Length; k++)
				playerListing += " ";

			playerListing += " " + players[i].defense;

			for (int k = players[i].defense.ToString().Length; k < stats[5].Length; k++)
				playerListing += " ";

			playerListing += " " + players[i].potential;

			for (int k = players[i].potential.ToString().Length; k < stats[6].Length; k++)
				playerListing += " ";

			playerListing += " " + players[i].age;

			for (int k = players[i].age.ToString().Length; k < stats[7].Length; k++)
				playerListing += " ";

			for (int j = 0; j < players[i].skills.Length - 1; j++)
			{
				playerListing += " " + players[i].skills[j];

				for (int k = players [i].skills [j].ToString ().Length; k < stats [j + 8].Length; k++)
					playerListing += " ";
			}

			playerListing += " " + players[i].skills[players[i].skills.Length - 1]; 
            
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            
			if(draftPlayers)
				newPlayer.GetComponent<Button> ().onClick.AddListener(() => draft.PlayerDraft(newPlayer, playerListing));
			else
				newPlayer.GetComponent<Button> ().interactable = false;
        }

		parentRect.offsetMin = new Vector2(0, -(20 * (players.Count + 1) - parentsParentRect.rect.height));
		parentRect.offsetMax = new Vector2(newWidth, 0);
	}

	public List<Player> Sort(int headerNum, bool ascending, List<Player> players)
	{
		if (ascending)
			switch (headerNum)
			{
			case 0:
				players = players.OrderBy (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				players = players.OrderBy (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				players = players.OrderBy (playerX => playerX.position).ToList ();
				break;
			case 3:
				players = players.OrderBy (playerX => playerX.overall).ToList ();
				break;
			case 4:
				players = players.OrderBy (playerX => playerX.offense).ToList ();
				break;
			case 5:
				players = players.OrderBy (playerX => playerX.defense).ToList ();
				break;
			case 6:
				players = players.OrderBy (playerX => playerX.potential).ToList ();
				break;
			case 7:
				players = players.OrderBy (playerX => playerX.age).ToList ();
				break;
			default:
				players = players.OrderBy (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
			{
			case 0:
				players = players.OrderByDescending (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				players = players.OrderByDescending (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				players = players.OrderByDescending (playerX => playerX.position).ToList ();
				break;
			case 3:
				players = players.OrderByDescending (playerX => playerX.overall).ToList ();
				break;
			case 4:
				players = players.OrderByDescending (playerX => playerX.offense).ToList ();
				break;
			case 5:
				players = players.OrderByDescending (playerX => playerX.defense).ToList ();
				break;
			case 6:
				players = players.OrderByDescending (playerX => playerX.potential).ToList ();
				break;
			case 7:
				players = players.OrderByDescending (playerX => playerX.age).ToList ();
				break;
			default:
				players = players.OrderByDescending (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}

		return players;
	}

	public void SetDraftPlayerObjects(Transform draftList, Transform header, RectTransform draftListRect, RectTransform draftListParentRect)
	{
		draft.SetDraftPlayerObjects (draftList, header, draftListRect, draftListParentRect);
	}

	public void StartDraft()
	{
		draft.StartDraft ();
	}

	public void SetPlayerDisplayObjects(Transform teamList, Transform header, RectTransform teamListRect, RectTransform teamListParentRect)
	{
		playerDisplay.SetPlayerDisplayObjects (teamList, header, teamListRect, teamListParentRect);
	}

	public void DisplayPlayers()
	{
		playerDisplay.Display ();
	}

	public void LoadPlayerDisplay(AllTeams allTeams)
	{
		playerDisplay = new PlayerDisplay (allTeams);
	}

	public void ChangeRosterSize(int size)
	{
		rosterSize = size;
	}

	public void SimulateDay()
	{
		days [dayIndex].SimulateDay ();
		dayIndex++;
	}

	public void NewYear()
	{
		year++;
	}
}