using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class Manager : MonoBehaviour
{
	public string [] Skills = File.ReadAllLines("Skills.txt");
	public List<string> tradeList;
	public List<string> injuries;
	public int longestHitStreak = 0, hitStreakYear, FYPDIndex;
	public string hitStreakName;
	public TradeDeadline tradeDeadline;
	public List<int> hallOfFameCandidates;
	public List<HallOfFameInductee> hallOfFameInductees = new List<HallOfFameInductee>();

	private float newWidth;
	private Draft draft;
	private PlayerDisplay playerDisplay;
	private DraftedPlayerDisplay draftedPlayerDisplay;
	private int year;
	private List<Player> players;
	private int [] finalsTeams = new int [8];
	private List<int []> finalsRounds;
	private List<int> cyWinners, mvpWinners;
	private DateTime startOfYear;
	private GameObject panel;
	private List<Team> [] teams = new List<Team>[Enum.GetNames(typeof (TeamType)).Length];				// List of all teams
	private List<WaiverPlayer> waivers = new List<WaiverPlayer> (0);

	private static List<Day> days;
	private static int dayIndex;
	private static Manager instance = null;
	private static int numTeams = 30;							// Number of teams

	// Use this for initialization
	void Awake ()
	{
		if (instance != null && instance != this)
			Destroy (gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad (gameObject);
			Load ();
		}
	}

	// Loads the game
	public void Load()
	{
		for (int i = 0; i < teams.Length; i++)
			teams [i] = new List<Team> ();

		teams [2].Add (new Team (TeamType.Futures, 0));
		teams [2].Add (new Team (TeamType.Futures, 1));
		Manager.Instance.Teams [2] [0].Shortform = "USA";
		Manager.Instance.Teams [2] [1].Shortform = "WLD";

		teams [3].Add (new Team (TeamType.AllStar, 0));
		teams [3].Add (new Team (TeamType.AllStar, 1));
		Manager.Instance.Teams [3] [0].Shortform = "A.L.";
		Manager.Instance.Teams [3] [1].Shortform = "N.L.";
		finalsRounds = new List<int []> ();

		// Loads everything after starting the game after having already played
		if (PlayerPrefs.HasKey ("Year"))
		{
			int index = 0;

			players = new List<Player> ();
			string [] lines;

			year = PlayerPrefs.GetInt("Year");
			dayIndex = PlayerPrefs.GetInt ("DayIndex");

			CreateDays ();

			for (int j = 0; j < 30; j++)
			{
				Team team = new Team (TeamType.MLB, index++);
				team.LoadTeam ();
				teams [0].Add (team);
			}

			index = 0;

			for (int j = 0; j < 22; j++)
			{
				Team team = new Team (TeamType.WorldBaseballClassic, index++);
				team.LoadTeam ();
				teams [1].Add (team);
			}

			index = 0;

			while(File.Exists (@"Save\Player" + index + ".txt"))
			{
				Player player = new Player ();
				player.LoadPlayer (index++);
				NewPlayer (player);
			}

			index = 0;

			while(File.Exists (@"Save\ScheduledGame" + index++ + ".txt"))
				new ScheduledGame ();

			lines = File.ReadAllLines (@"Save\SimulatedGames.txt");

			for(int i = 0; i < lines.Length; i++)
				new SimulatedGame (lines [i]);

			lines = File.ReadAllLines (@"Save\Events.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				string [] split = lines [i].Split (',');
				int dayIndex = int.Parse (split [0]);

				days [dayIndex].Events.Add (Event.Load (split));
			}

			for (int i = 0; i < teams [0].Count; i++)
				teams [0] [i].SetExpenses ();

			Player.longestFirstName = PlayerPrefs.GetInt ("LongestFirstName");
			Player.longestLastName = PlayerPrefs.GetInt ("LongestLastName");
		}
		// Creates a new game if it is the first time playing
		else
			Restart ();

		cyWinners = new List<int> ();
		mvpWinners = new List<int> ();
		draft = new Draft ();
		playerDisplay = new PlayerDisplay ();
		draftedPlayerDisplay = new DraftedPlayerDisplay ();
	}

	// Returns the number of teams
	public int GetNumTeams()
	{
		return numTeams;
	}

	// Restarts the game
	public void Restart ()
	{
		List<int> picksLeft = new List<int> ();	// List of picks left for the draft

		players = new List<Player>();

		year = DateTime.Now.Year;
		PlayerPrefs.SetInt ("Year", year);
		CreateDays ();
		tradeList = new List<string> ();

		for (int i = 0; i < Enum.GetNames(typeof (Country)).Length; i++)
		{
			Team newTeam = new Team (TeamType.WorldBaseballClassic, i);

			newTeam.CityName = ((Country)i).ToString ();
			newTeam.Shortform = ((CountryShortforms)i).ToString ();
			newTeam.Save ();
			teams [1].Add (newTeam);
			newTeam.SaveWLHC ();
		}

		// Adds the draft picks to the list
		for (int i = 0; i < numTeams; i++)
			picksLeft.Add (i);

		// Creates each team with 1 of each batting position, 5 Starting Pitcher, 3 Relief Pitcher and 1 Closing Pitcher
		for (int i = 0; i < numTeams; i++)
		{
			Team team = new Team (TeamType.MLB, i);
			string [] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
			string [] splitName = (team.CityName + " " + team.TeamName).Split(' ');
			int pickIndex = (int)(UnityEngine.Random.value * picksLeft.Count);

			team.Pick = picksLeft [pickIndex];
			picksLeft.RemoveAt (pickIndex);

			for (int j = 0; j < splitName.Length; j++)
				if (System.Char.IsLetter(splitName [j] [0]) && System.Char.IsUpper(splitName [j] [0]))
					team.Shortform += splitName [j] [0];

			for (int j = 3; j < positions.Length; j++)
				team.AddPlayer (CreateStartingPlayer (new Player (positions [j], 23, 22, players.Count, i)));

			for (int j = 0; j < 5; j++)
				team.AddPlayer (CreateStartingPlayer (new Player ("SP", 23, 22, players.Count, i)));

			for (int j = 0; j < 3; j++)
				team.AddPlayer (CreateStartingPlayer (new Player ("RP", 23, 22, players.Count, i)));

			team.AddPlayer (CreateStartingPlayer (new Player ("CP", 23, 22, players.Count, i)));

			for (int j = team.Players.Count; j < Team.RosterSize; j++)
				team.AddPlayer (CreateStartingPlayer (new Player (Player.Positions [(int)(UnityEngine.Random.value * Player.Positions.Length)], 23, 22, players.Count, i)));

			team.AutomaticRoster ();
			team.Save ();

			team.SaveOveralls ();
			team.SaveWLHC ();

			teams [0].Add (team);
		}
			
		dayIndex = 0;
		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		CreateSchedule.ScheduleEvents (days, DateTime.IsLeapYear(year));

		// Stores the length of the longest first and last name to be able to display them properly
		PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
		PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

		PlayerPrefs.Save ();
	}

	// Displays the headers
	public void DisplayHeaders(Transform headerTrans, RectTransform parentsParentRect, DisplayType displayType)
	{
		int skillHeaderLength = 0;
		float characterWidth = 8.03f;

		newWidth = 0.0f;

		skillHeaderLength += Player.longestFirstName + Player.longestLastName + 2;

		for (int i = 2; i < Skills.Length; i++)
			skillHeaderLength += Skills [i].Length + 1;

		UnityEngine.Object header = Resources.Load("Header", typeof (GameObject));
		float prevWidth = 5.0f;
		float totalWidth = (characterWidth * (skillHeaderLength + 1.0f));
		totalWidth /= -2.0f;

		for (int i = 0; i < Skills.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			int headerNum = i;

			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent(headerTrans);
			statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = Skills [i];

			if (displayType == DisplayType.Draft)
				statHeader.GetComponent<Button>().onClick.AddListener(() => draft.Sort(headerNum));
			else if (displayType == DisplayType.Signed)
				statHeader.GetComponent<Button>().onClick.AddListener(() => draftedPlayerDisplay.Sort(headerNum));
			else
				statHeader.GetComponent<Button>().onClick.AddListener(() => playerDisplay.Sort(headerNum));

			float currWidth;
			if (i > 1)
				currWidth = (characterWidth * (Skills [i].Length + 1));
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

	// Displays the players
	public void DisplayPlayers(List<int> playersToDisplay, Transform parentTrans, RectTransform parentRect, RectTransform parentsParentRect, DisplayType displayType)
	{
		GameObject [] currPlayers;
		UnityEngine.Object playerButton;
		string playerName;

		if (displayType == DisplayType.Draft)
		{
			playerButton= Resources.Load("DraftPlayer", typeof (GameObject));
			currPlayers = GameObject.FindGameObjectsWithTag ("DraftPlayer");
			playerName = "draft";
		}
		else
		{
			playerButton = Resources.Load("Player", typeof (GameObject));
			currPlayers = GameObject.FindGameObjectsWithTag ("Player");
			playerName = "player";
		}

		for (int i = 0; i < currPlayers.Length; i++)
			Destroy(currPlayers [i]);

		for (int i = 0; i < playersToDisplay.Count; i++)
		{
			GameObject newPlayer = Instantiate (playerButton) as GameObject;
			string playerListing;

			newPlayer.name = playerName + i.ToString ();
			newPlayer.transform.SetParent(parentTrans);

			playerListing = players [playersToDisplay[i]].FirstName;

			for (int j = players [playersToDisplay[i]].FirstName.Length; j < Player.longestFirstName; j++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].LastName;

			for (int j = players [playersToDisplay[i]].LastName.Length; j < Player.longestLastName; j++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Position;

			for (int k = players [playersToDisplay[i]].Position.Length; k < Skills [2].Length; k++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Overall;

			for (int k = players [playersToDisplay[i]].Overall.ToString ().Length; k < Skills [3].Length; k++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Offense;

			for (int k = players [playersToDisplay[i]].Offense.ToString ().Length; k < Skills [4].Length; k++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Defense;

			for (int k = players [playersToDisplay[i]].Defense.ToString ().Length; k < Skills [5].Length; k++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Potential;

			for (int k = players [playersToDisplay[i]].Potential.ToString ().Length; k < Skills [6].Length; k++)
				playerListing += " ";

			playerListing += " " + players [playersToDisplay[i]].Age;

			for (int k = players [playersToDisplay[i]].Age.ToString ().Length; k < Skills [7].Length; k++)
				playerListing += " ";

			for (int j = 0; j < players [playersToDisplay[i]].Skills.Length - 1; j++)
			{
				playerListing += " " + players [playersToDisplay[i]].Skills [j];

				for (int k = players [playersToDisplay [i]].Skills [j].ToString ().Length; k < Skills [j + 8].Length; k++)
					playerListing += " ";
			}

			playerListing += " " + players [playersToDisplay[i]].Skills [players [playersToDisplay[i]].Skills.Length - 1]; 

			newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
			newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

			if (displayType == DisplayType.Draft)
				newPlayer.GetComponent<Button> ().onClick.AddListener (() => draft.PlayerDraft (newPlayer, playerListing));
			else
			{
				int id = playersToDisplay [i];

				if (displayType == DisplayType.Signed)
					newPlayer.GetComponent<Button> ().onClick.AddListener (() => draftedPlayerDisplay.ShowDraftedPlayer (id));
				else
					newPlayer.GetComponent<Button> ().onClick.AddListener (() => DisplayPlayer(id));
			}
		}

		parentRect.offsetMin = new Vector2(0, -(20 * (playersToDisplay.Count + 1) - parentsParentRect.rect.height));
		parentRect.offsetMax = new Vector2(newWidth, 0);
	}

	// Sorts the players
	public List<int> Sort(int headerNum, bool ascending, List<int> playersToSort)
	{
		if (ascending)
			switch (headerNum)
			{
			case 0:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].FirstName).ToList ();
				break;
			case 1:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].LastName).ToList ();
				break;
			case 2:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Position).ToList ();
				break;
			case 3:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Overall).ToList ();
				break;
			case 4:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Offense).ToList ();
				break;
			case 5:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Defense).ToList ();
				break;
			case 6:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Potential).ToList ();
				break;
			case 7:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Age).ToList ();
				break;
			default:
				playersToSort = playersToSort.OrderBy (playerX => players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
			{
			case 0:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].FirstName).ToList ();
				break;
			case 1:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].LastName).ToList ();
				break;
			case 2:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Position).ToList ();
				break;
			case 3:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Overall).ToList ();
				break;
			case 4:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Offense).ToList ();
				break;
			case 5:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Defense).ToList ();
				break;
			case 6:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Potential).ToList ();
				break;
			case 7:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Age).ToList ();
				break;
			default:
				playersToSort = playersToSort.OrderByDescending (playerX => players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}

		return playersToSort;
	}

	// Creates a starting player
	public int CreateStartingPlayer(Player newPlayer)
	{
		for(int k = 0; k < UnityEngine.Random.value * 4 + 1; k++)
			if (k != 0 && newPlayer.ContractYears [k - 1].Type != ContractType.NoOption)
				newPlayer.ContractYears.Add(new ContractYear(newPlayer.ContractYears [k - 1].Type, newPlayer.ExpectedSalary * 1.1));
			else if (UnityEngine.Random.value > 0.5f)
				newPlayer.ContractYears.Add(new ContractYear(ContractType.NoOption, newPlayer.ExpectedSalary * 0.9));
			else
				newPlayer.ContractYears.Add(new ContractYear((ContractType)(int)(UnityEngine.Random.value * 2), newPlayer.ExpectedSalary * 1.1));

		newPlayer.SavePlayer ();
		NewPlayer (newPlayer);
		return newPlayer.ID;
	}

	// Sets the objects for displaying the draft players
	public void SetDraftPlayerObjects(Transform draftList, Transform header, RectTransform draftListRect, RectTransform draftListParentRect)
	{
		draft.SetDraftPlayerObjects (draftList, header, draftListRect, draftListParentRect);
	}

	// Starts the draft
	public void StartDraft ()
	{
		draft.StartDraft ();
	}

	// Sets the objects for displaying the team's players
	public void SetPlayerDisplayObjects(Transform teamList, Transform header, RectTransform teamListRect, RectTransform teamListParentRect)
	{
		playerDisplay.SetPlayerDisplayObjects (teamList, header, teamListRect, teamListParentRect);
	}

	// Dispplays the team's players
	public void DisplayPlayers()
	{
		playerDisplay.Display ();
	}

	// Sets the objects for displaying the drafted players
	public void SetDraftedPlayerDisplayObjects(Transform teamList, Transform header, RectTransform teamListRect, RectTransform teamListParentRect, GameObject panel)
	{
		draftedPlayerDisplay.SetPlayerDisplayObjects (teamList, header, teamListRect, teamListParentRect, panel);
	}

	// Displays the drafted players
	public void DisplayDraftedPlayers()
	{
		draftedPlayerDisplay.Display ();
	}

	// Displays the player
	public void DisplayPlayer(int id)
	{
		panel.SetActive (true);
		panel.GetComponent<DisplayPlayer> ().SetPlayerID (id);
	}

	// Sets the panel
	public void SetPanel (GameObject obj)
	{
		panel = obj;
	}

	// Simulates the day
	public void SimulateDay()
	{
		days [dayIndex].SimulateDay ();

		for (int i = 0; i < waivers.Count; i++)
			waivers [i].AdvanceDay ();

		dayIndex++;
		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		PlayerPrefs.Save ();
	}

	// Creates all of the days for the current year
	public void CreateDays()
	{
		startOfYear = DateTime.Parse (year + "/01/01");
		int numDays = (DateTime.Parse (year + "/12/31") - startOfYear).Days;
		DateTime currDate = startOfYear;

		days = new List<Day> ();

		for (int i = 0; i <= numDays; i++)
		{
			days.Add (new Day (currDate));
			currDate = currDate.AddDays (1);
		}
	}

	// Starts a new year
	public void NewYear()
	{
		year++;
		PlayerPrefs.SetInt ("Year", year);

		finalsRounds = new List<int []> ();

		for (int i = 0; i < teams [0].Count; i++)
		{
			if (teams [0] [i].Cash < 0)
				teams [0] [i].Bankrupt ();
			
			teams [0] [i].NewYear ();
		}

		CreateDays ();
		dayIndex = 0;
		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		PlayerPrefs.Save ();
		CreateSchedule.ScheduleEvents (days, DateTime.IsLeapYear(year));
		Calendar.ActivateNextButton ();
	}

	// Puts a player on waivers
	public void PutOnWaivers(int id)
	{
		waivers.Add (new WaiverPlayer(id));
	}

	// Takes a player off waivers
	public void TakeOffWaivers(int id)
	{
		int index = 0;

		while (index < waivers.Count && waivers [index].ID != id)
			index++;

		if (waivers [index].ID == id)
			waivers.RemoveAt (index);
	}

	public void ScheduleFinalsGames(int team1, int team2, int offset, int round)
	{
		teams [0] [finalsTeams [team1]].ResetWins ();
		teams [0] [finalsTeams [team2]].ResetWins ();

		days [dayIndex + offset].ScheduledGames.Add (new ScheduledGame (teams [0] [finalsTeams [team1]], teams [0] [finalsTeams [team2]], GameType.WorldSeries, TeamType.MLB, dayIndex + offset));
		days [dayIndex + offset + 1].ScheduledGames.Add (new ScheduledGame (teams [0] [finalsTeams [team1]], teams [0] [finalsTeams [team2]], GameType.WorldSeries, TeamType.MLB, dayIndex + offset + 1));
		days [dayIndex + offset + 3].ScheduledGames.Add (new ScheduledGame (teams [0] [finalsTeams [team2]], teams [0] [finalsTeams [team1]], GameType.WorldSeries, TeamType.MLB, dayIndex + offset + 3));
		days [dayIndex + offset + 4].ScheduledGames.Add (new ScheduledGame (teams [0] [finalsTeams [team2]], teams [0] [finalsTeams [team1]], GameType.WorldSeries, TeamType.MLB, dayIndex + offset + 4));

		days [dayIndex + offset + 4].AddEvent (new FinalsCheck (team1, team2, round), dayIndex + offset + 4);
	}

	// Determines who the MVP and Cy Young award winners are
	public void DetermineMVP()
	{
		double mvpWorth, cyWorth, bestMVP = 0.0, bestCY = 0.0;
		int mvpWinner = 0, cyWinner = 0;

		for (int i = 0; i < teams [0].Count; i++)
			for(int j = 0; j < teams [0] [i].Players.Count; j++)
			{
				if (players [teams [0] [i].Players [j]].Position.Contains("P"))
				{
					double era = players [teams [0] [i].Players [j]].Stats [0] [24] * 27 / (double)players [teams [0] [i].Players [j]].Stats [0] [20];
					cyWorth = (6.0 - era) * 5 + players [teams [0] [i].Players [j]].Stats [0] [20] / (double)8;
					if (cyWorth > bestCY)
					{
						bestCY = cyWorth;
						cyWinner = j;
					}
				}
				else
				{
					double ops = (players [teams [0] [i].Players [j]].Stats [0] [3] + players [teams [0] [i].Players [j]].Stats [0] [10]) / (double)(players [teams [0] [i].Players [j]].Stats [0] [1] + players [teams [0] [i].Players [j]].Stats [0] [10] + players [teams [0] [i].Players [j]].Stats [0] [14]) + (players [teams [0] [i].Players [j]].Stats [0] [3] + players [teams [0] [i].Players [j]].Stats [0] [5] * 2 + players [teams [0] [i].Players [j]].Stats [0] [6] * 3 + players [teams [0] [i].Players [j]].Stats [0] [7] * 4) / (double)players [teams [0] [i].Players [j]].Stats [0] [1];
					mvpWorth = players [teams [0] [i].Players [j]].Stats [0] [7] / 40.0 + ops * 25;
					if (mvpWorth > bestMVP)
					{
						bestMVP = mvpWorth;
						mvpWinner = j;
					}
				}
			}
		
		cyWinners.Add (cyWinner);
		mvpWinners.Add (mvpWinner);
	}

	// Adds a new player
	public void NewPlayer (Player newPlayer)
	{
		teams [1] [(int)newPlayer.Country].AddPlayer (newPlayer.ID);
		players.Add (newPlayer);
	}

	// Getters
	public List<Team> [] Teams
	{
		get
		{
			return teams;
		}
	}

	public DateTime StartOfYear
	{
		get
		{
			return startOfYear;
		}
	}

	public List<int []> FinalsRounds
	{
		get
		{
			return finalsRounds;
		}
	}

	public int [] FinalsTeams
	{
		get
		{
			return finalsTeams;
		}
	}

	public List<Player> Players
	{
		get
		{
			return players;
		}
	}

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

	public static Manager Instance
	{
		get
		{
			return instance;
		}
	}

	// Changes to the specified scene
	public static void ChangeToScene (int sceneToChangeTo)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneToChangeTo);
	}
}

public enum DisplayType
{
	Team,
	Draft,
	Signed
}