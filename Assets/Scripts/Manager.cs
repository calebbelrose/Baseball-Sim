using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Manager : MonoBehaviour
{
	public string [] Skills = File.ReadAllLines ("Skills.txt");								// Skills
	public List<string> TradeList;															// List of all trades
	public List<string> Injuries;															// Player index of all injured players
	public int LongestHitStreak;															// Number of hits in the longest hit streak
	public int HitStreakYear;																// Year of the longest hit streak
	public int FYPDIndex;																	// Day index of the first year player draft
	public string HitStreakName;															// Name of player with longest hit streak
	public string YourName;																	// User's name
	public TradeDeadline TradeDeadline;														// Trade deadline
	public List<int> HallOfFameCandidates;													// Hall of fame candidates
	public List<HallOfFameInductee> HallOfFameInductees = new List<HallOfFameInductee> ();	// Hall of fame inductees
	public Color TeamColour;																// User's team colour
	public Sprite TeamLogo;																	// Logo for the user's team
	public List<Sprite> FrecklesSprites;													// All freckle sprites
	public List<Sprite> EarSprites;															// All ear sprites
	public List<Sprite> FaceSprites;														// All face sprites
	public List<Sprite> EyeShapeSprites;													// All eye shape sprites
	public List<Sprite> EyeColourSprites;													// All eye colour sprites
	public List<Sprite> HairSprites;														// All hair sprites
	public List<Sprite> MouthSprites;														// All mouth sprites
	public List<Sprite> NoseSprites;														// All nose sprites
	public Queue<Action> ExecuteOnMainThread = new Queue<Action> ();						// Used to move actions from another thread back to main thread
	public bool MLRosterChanged = false;													// Whether a major league team has changed their roster or not

	private int year;																		// Current year
	private List<Player> players;															// All players
	private int [] finalsTeams = new int [8];												// Teams in world series
	private List<int []> finalsRounds;														// Used to schedule world series games
	private List<int> cyWinners;															// Cy Young Winners
	private List<int> mvpWinners;															// MVP winners
	private List<int> freeAgents;															// Free agents
	private List<int> internationalFreeAgents;												// International free agents
	private DateTime startOfYear;															// Date at the beginning of year
	private GameObject panel;																// Panel
	private List<Team> [] teams = new List<Team>[Enum.GetNames (typeof (TeamType)).Length];	// All teams
	private List<WaiverPlayer> waivers = new List<WaiverPlayer> (0);						// Players on waivers
	private System.Random randomGen;														// Random number generator required for off the main thread
	private List<int> shortDisabledList;													// Indexes of players on short disabled list
	private List<int> longDisabledList;														// Indexes of players on long disabled list
	private bool shortDisabledListChanged = false;											// Whether the short disabled list has changed
	private bool longDisabledListChanged = false;											// Whether the long disabled list has changed
	private bool rosterChange = false;														// Whether there has been a roster change (used to determine if teams should look to trade0

	private static List<Day> days;															// Days
	private static int dayIndex;															// Index of the current day
	private static Manager instance = null;													// Instance of class
	private static int numTeams = 30;														// Number of teams
	private static int shortDisabledListTime;												// Minimum time a player can be on the short disabled list
	private static int longDisabledListTime;												// Minimum time a player can be on the long disabled list

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

	void Update ()
	{
		while (ExecuteOnMainThread.Count > 0)
			ExecuteOnMainThread.Dequeue ().Invoke ();
	}

	// Loads the game
	public void Load ()
	{
		randomGen = new System.Random ();
		shortDisabledList = new List<int> ();
		longDisabledList = new List<int> ();

		for (int i = 0; i < teams.Length; i++)
			teams [i] = new List<Team> ();

		teams [2].Add (new Team (TeamType.Futures, 0));
		teams [2].Add (new Team (TeamType.Futures, 1));
		teams [2] [0].Shortform = "USA";
		teams [2] [1].Shortform = "WLD";

		teams [3].Add (new Team (TeamType.AllStar, 0));
		teams [3].Add (new Team (TeamType.AllStar, 1));
		teams [3] [0].Shortform = "A.L.";
		teams [3] [1].Shortform = "N.L.";
		finalsRounds = new List<int []> ();
		freeAgents = new List<int> ();
		internationalFreeAgents = new List<int> ();

		// Loads the logo if there is one, otherwise it sets the logo
		if (PlayerPrefs.HasKey ("Logo"))
			TeamLogo = Resources.Load<Sprite> ("Logos/team" + PlayerPrefs.GetString ("Logo"));
		else
		{
			TeamLogo = Resources.Load<Sprite> ("Logos/team1");
			PlayerPrefs.SetString ("Logo", "1");
			PlayerPrefs.Save ();
		}

		// Loads the user's name
		if (PlayerPrefs.HasKey ("Your Name"))
			YourName = PlayerPrefs.GetString ("Your Name");
		else
		{
			YourName = "Unknown";
			PlayerPrefs.SetString ("Your Name", "Unknown");
		}

		//Loads team's colour
		if (PlayerPrefs.HasKey ("Team Colour"))
		{
			string[] split = PlayerPrefs.GetString ("Team Colour").Split (',');

			TeamColour = new Color (float.Parse (split [0]), float.Parse (split [1]), float.Parse (split [2]));
		}

		// Loads everything after starting the game after having already played
		if (PlayerPrefs.HasKey ("Year"))
		{
			int index = 0;

			players = new List<Player> ();
			string[] lines;
			string[] split;

			year = PlayerPrefs.GetInt ("Year");
			dayIndex = PlayerPrefs.GetInt ("DayIndex");
			LongestHitStreak = PlayerPrefs.GetInt ("LongestHitStreak");
			HitStreakYear = PlayerPrefs.GetInt ("HitStreakYear");
			HitStreakName = PlayerPrefs.GetString ("HitStreakName");

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

				if (File.Exists (@"Save\TeamPlayers1-" + j + ".txt"))
					team.LoadTeam ();
				
				teams [1].Add (team);
			}

			index = 0;

			while (File.Exists (@"Save\Player" + index + ".txt"))
			{
				Player player = new Player ();
				player.LoadPlayer (index++);
				players.Add (player);
			}

			index = 0;

			while (File.Exists (@"Save\ScheduledGame" + index++ + ".txt"))
				new ScheduledGame ();

			lines = File.ReadAllLines (@"Save\SimulatedGames.txt");

			for (int i = 0; i < lines.Length; i++)
				new SimulatedGame (lines [i]);

			lines = File.ReadAllLines (@"Save\Events.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				split = lines [i].Split (',');
				int dayIndex = int.Parse (split [0]);

				days [dayIndex].Events.Add (Event.Load (split));
			}

			for (int i = 0; i < teams [0].Count; i++)
				teams [0] [i].SetExpenses ();

			lines = File.ReadAllLines (@"Save\FreeAgents.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				int id;

				split = lines [i].Split(',');
				id = int.Parse (split [0]);
				freeAgents.Add (id);
				players [id].OfferTime = int.Parse (split [1]);
				players [id].Offer = double.Parse (split [2]);
				players [id].Team= int.Parse (split [3]);
			}
			

			lines = File.ReadAllLines (@"Save\InternationalFreeAgents.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				int id;

				split = lines [i].Split(',');
				id = int.Parse (split [0]);
				internationalFreeAgents.Add (id);
				players [id].OfferTime = int.Parse (split [1]);
				players [id].Offer = double.Parse (split [2]);
				players [id].Team= int.Parse (split [3]);
			}

			lines = File.ReadAllLines (@"Save\Waivers.txt");

			for (int i = 0; i < lines.Length; i++)
			{
				int id;

				split = lines [i].Split (',');
				id = int.Parse (split [0]);
				teams [0] [Manager.Instance.Players [id].Team].AddToWaivers (id);
				waivers.Add (new WaiverPlayer (id, int.Parse (split [1]), int.Parse (split [2]), bool.Parse (split [3])));
			}

			Player.longestFirstName = PlayerPrefs.GetInt ("LongestFirstName");
			Player.longestLastName = PlayerPrefs.GetInt ("LongestLastName");
			Player.MinSalary = PlayerPrefs.GetFloat ("MinSalary");
			shortDisabledListTime = PlayerPrefs.GetInt ("ShortDisabledListTime");
			longDisabledListTime = PlayerPrefs.GetInt ("LongDisabledListTime");
		}
		// Creates a new game if it is the first time playing
		else
			Restart ();

		cyWinners = new List<int> ();
		mvpWinners = new List<int> ();
	}

	// Returns the number of teams
	public int GetNumTeams ()
	{
		return numTeams;
	}

	// Restarts the game
	void Restart ()
	{
		List<int> picksLeft = new List<int> ();	// List of picks left for the draft
		StreamWriter sw;

		CreateFiles ();
		sw = new StreamWriter (@"Save\FreeAgents.txt");
		players = new List<Player> ();
		year = DateTime.Now.Year;
		PlayerPrefs.SetInt ("Year", year);
		LongestHitStreak = 0;
		HitStreakYear = year;
		HitStreakName = "Nobody";
		PlayerPrefs.SetInt ("LongestHitStreak", 0);
		PlayerPrefs.SetInt ("HitStreakYear", HitStreakYear);
		PlayerPrefs.SetString ("HitStreakName", "Nobody");
		PlayerPrefs.SetString ("AutomaticRoster", "true");
		CreateDays ();
		TradeList = new List<string> ();
		Player.MinSalary = 535000.00;
		PlayerPrefs.SetFloat ("MinSalary", 535000);
		shortDisabledListTime = 15;
		PlayerPrefs.SetInt ("ShortDisabledListTime", 15);
		longDisabledListTime = 60;
		PlayerPrefs.SetInt ("LongDisabledListTime", 60);

		for (int i = 0; i < Enum.GetNames (typeof (Country)).Length; i++)
		{
			Team newTeam = new Team (TeamType.WorldBaseballClassic, i);

			newTeam.CityName = ((Country)i).ToString ();
			newTeam.Shortform = ((CountryShortforms)i).ToString ();
			newTeam.Save ();
			teams [1].Add (newTeam);
			newTeam.SaveWLHC ();
		}

		for (int i = 0; i < 125 + (int)(UnityEngine.Random.value * 50); i++)
		{
			Player newPlayer = new Player (Player.Positions [(int)(UnityEngine.Random.value * Player.Positions.Length)], 23, 22, players.Count);

			newPlayer.RandomCountry ();
			newPlayer.SavePlayer ();
			newPlayer.SaveContract ();
			newPlayer.SaveStats ();
			NewPlayer (newPlayer);
			freeAgents.Add (newPlayer.ID);
			sw.WriteLine (newPlayer.ID + ",0,0.0,-1");
		}

		sw.Close ();
		sw = new StreamWriter (@"Save\InternationalFreeAgents.txt");

		for (int i = 0; i < 125 + (int)(UnityEngine.Random.value * 50); i++)
			sw.WriteLine (NewInternationalFreeAgent ());

		sw.Close ();

		// Adds the draft picks to the list
		for (int i = 0; i < numTeams; i++)
			picksLeft.Add (i);

		// Creates each team with 1 of each batting position, 5 Starting Pitcher, 3 Relief Pitcher and 1 Closing Pitcher
		for (int i = 0; i < numTeams; i++)
		{
			Team team = new Team (TeamType.MLB, i);
			string [] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
			string [] splitName = (team.CityName + " " + team.TeamName).Split (' ');
			int pickIndex = (int) (UnityEngine.Random.value * picksLeft.Count);

			team.Pick = picksLeft [pickIndex];
			picksLeft.RemoveAt (pickIndex);

			for (int j = 0; j < splitName.Length; j++)
				if (Char.IsLetter (splitName [j] [0]) && Char.IsUpper (splitName [j] [0]))
					team.Shortform += splitName [j] [0];

			for (int j = 3; j < positions.Length; j++)
				team.AddPlayer (CreateStartingPlayer (new Player (positions [j], 23, 22, players.Count, i)));

			for (int j = 0; j < 5; j++)
				team.AddPlayer (CreateStartingPlayer (new Player ("SP", 23, 22, players.Count, i)));

			for (int j = 0; j < 3; j++)
				team.AddPlayer (CreateStartingPlayer (new Player ("RP", 23, 22, players.Count, i)));

			team.AddPlayer (CreateStartingPlayer (new Player ("CP", 23, 22, players.Count, i)));

			for (int j = team.Players.Count; j < Team.RosterSize; j++)
				team.AddPlayer (CreateStartingPlayer (new Player (Player.Positions [ (int) (UnityEngine.Random.value * Player.Positions.Length)], 23, 22, players.Count, i)));

			team.SetRoster ();
			team.Save ();
			team.SaveOveralls ();
			team.SaveWLHC ();
			teams [0].Add (team);
		}

		teams [0] [0].AutomaticRoster = bool.Parse (PlayerPrefs.GetString ("AutomaticRoster"));
		if (!teams [0] [0].AutomaticRoster)
		{
			string[] split;

			split = File.ReadAllLines (@"Save\Batters.txt");

			for (int i = 0; i < 9; i++)
			{
				teams [0] [0].Batters.Add (new List<int> ());
				teams [0] [0].Batters [i].Add (int.Parse (split [i]));
			}

			split = File.ReadAllLines (@"Save\SP.txt");

			for (int i = 0; i < split.Length - 1; i++)
				teams [0] [0].SP.Add (int.Parse (split [i]));

			split = File.ReadAllLines (@"Save\RP.txt");

			for (int i = 0; i < split.Length - 1; i++)
				teams [0] [0].RP.Add (int.Parse (split [i]));

			teams [0] [0].CP = PlayerPrefs.GetInt ("CP");

			split = File.ReadAllLines (@"Save\Substitutes.txt");

			for (int i = 0; i < split.Length - 1; i++)
			{
				int playerID = int.Parse (split [i]);

				teams [0] [0].OffensiveSubstitutes.Add (playerID);
				teams [0] [0].DefensiveSubstitutes.Add (playerID);
				teams [0] [0].PinchRunners.Add (playerID);
			}

			teams [0] [0].SortSubstitutes ();

			split = File.ReadAllLines (@"Save\FortyManRoster.txt");

			for (int i = 0; i < split.Length - 1; i++)
				teams [0] [0].FortyManRoster.Add (int.Parse (split [i]));
		}
			
		dayIndex = 0;
		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		CreateSchedule.ScheduleEvents (days, DateTime.IsLeapYear (year));
		PlayerPrefs.SetInt ("LongestFirstName", Player.longestFirstName);
		PlayerPrefs.SetInt ("LongestLastName", Player.longestLastName);
		PlayerPrefs.SetString ("NeedDraft", false.ToString ());

		PlayerPrefs.Save ();
	}

	public static float DisplayHeaders (Action<GameObject> action, Transform parent)
	{
		int statHeaderLength = 0;
		int [] headerLengths = new int [Manager.Instance.Skills.Length];
		UnityEngine.Object header = Resources.Load ("Header", typeof (GameObject));
		float newWidth = 0.0f;

		for (int i = 2; i < Manager.Instance.Skills.Length; i++)
		{
			headerLengths [i] = Manager.Instance.Skills [i].Length + 1;
			statHeaderLength += headerLengths [i];
		}

		headerLengths [0] += Player.longestFirstName + 1;
		headerLengths [1] += Player.longestLastName + 1;

		statHeaderLength += headerLengths [0];
		statHeaderLength += headerLengths [1];

		for (int i = 0; i < Manager.Instance.Skills.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			float currWidth = (8.03f * headerLengths [i]);

			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (parent);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = Manager.Instance.Skills [i];

			if (action != null)
				statHeader.GetComponent<Button> ().onClick.AddListener (() => action (statHeader));
			else
				statHeader.GetComponent<Button> ().interactable = false;
			
			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		return newWidth;
	}

	public static GameObject DisplayPlayer (UnityEngine.Object playerButton, Transform transform, int playerID)
	{
		GameObject newPlayer = Instantiate (playerButton) as GameObject;

		newPlayer.transform.SetParent (transform);
		newPlayer.transform.GetChild (0).gameObject.GetComponent<Text> ().text = Manager.Instance.Players [playerID].DisplayString ();
		newPlayer.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		newPlayer.AddComponent<CanvasGroup> ();

		return newPlayer;
	}

	// Sorts the players
	public List<int> Sort (int headerNum, bool ascending, List<int> playersToSort)
	{
		List <int> sortedPlayers = new List<int> ();

		if (ascending)
			switch (headerNum)
			{
			case 0:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].FirstName).ToList ();
				break;
			case 1:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].LastName).ToList ();
				break;
			case 2:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Position).ToList ();
				break;
			case 3:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Overall).ToList ();
				break;
			case 4:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Offense).ToList ();
				break;
			case 5:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Defense).ToList ();
				break;
			case 6:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Potential).ToList ();
				break;
			case 7:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Age).ToList ();
				break;
			default:
				sortedPlayers = playersToSort.OrderBy (playerX => players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
			{
			case 0:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].FirstName).ToList ();
				break;
			case 1:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].LastName).ToList ();
				break;
			case 2:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Position).ToList ();
				break;
			case 3:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Overall).ToList ();
				break;
			case 4:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Offense).ToList ();
				break;
			case 5:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Defense).ToList ();
				break;
			case 6:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Potential).ToList ();
				break;
			case 7:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Age).ToList ();
				break;
			default:
				sortedPlayers = playersToSort.OrderByDescending (playerX => players [playerX].Skills [headerNum - 8]).ToList ();
				break;
			}

		return sortedPlayers;
	}

	// Creates a starting player
	public int CreateStartingPlayer (Player newPlayer)
	{
		for (int k = 0; k < UnityEngine.Random.value * 4 + 1; k++)
			if (k != 0 && newPlayer.ContractYears [k - 1].Type != ContractType.NoOption)
				newPlayer.ContractYears.Add (new ContractYear (newPlayer.ContractYears [k - 1].Type, Math.Round(newPlayer.ExpectedSalary * 1.1, 2)));
			else if (UnityEngine.Random.value > 0.5f)
				newPlayer.ContractYears.Add (new ContractYear (ContractType.NoOption, Math.Round(newPlayer.ExpectedSalary * 0.9, 2)));
			else
				newPlayer.ContractYears.Add (new ContractYear ((ContractType) (int) (UnityEngine.Random.value * 3 + 1), Math.Round(newPlayer.ExpectedSalary * 1.1, 2)));

		newPlayer.RandomCountry ();
		newPlayer.SavePlayer ();
		newPlayer.SaveContract ();
		newPlayer.SaveStats ();
		NewPlayer (newPlayer);

		return newPlayer.ID;
	}

	// Displays the player
	public void DisplayPlayer (int id)
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
	public void SimulateDay ()
	{
		StreamWriter sw;

		while (days [dayIndex].ScheduledGames.Count > 0)
			SimulateGame ();
				
		for (int i = 0; i < days [dayIndex].Events.Count; i++)
			days [dayIndex].Events [i].Action ();

		for (int j = 0; j < teams [0] [0].Players.Count; j++)
			if (players [teams [0] [0].Players [j]].Skills [9] != players [teams [0] [0].Players [j]].Skills [10] && (players [teams [0] [0].Players [j]].Skills [9] += 20) > players [teams [0] [0].Players [j]].Skills [10])
				players [teams [0] [0].Players [j]].Skills [9] = players [teams [0] [0].Players [j]].Skills [10];

		for (int thisTeam = 0; thisTeam < teams [0].Count; thisTeam++)
			teams [0] [thisTeam].SetRoster ();

		for (int thisTeam = 1; thisTeam < teams [0].Count; thisTeam++)
		{
			List<int> bestOptions = new List<int> ();

			for (int j = 0; j < teams [0] [thisTeam].LookingFor.Count; j++)
				bestOptions.Add (-1);

			for (int j = 0; j < teams [0] [thisTeam].Players.Count; j++)
				if (players [teams [0] [thisTeam].Players [j]].Skills [9] != players [teams [0] [thisTeam].Players [j]].Skills [10] && (players [teams [0] [thisTeam].Players [j]].Skills [9] += 20) > players [teams [0] [thisTeam].Players [j]].Skills [10])
						players [teams [0] [thisTeam].Players [j]].Skills [9] = players [teams [0] [thisTeam].Players [j]].Skills [10];

			if(rosterChange)
			{
				if (Manager.Instance.TradeDeadline != TradeDeadline.Waiver)
				{
					if (Manager.Instance.TradeDeadline == TradeDeadline.None)
					{
						for (int otherTeam = 1; otherTeam < teams [0].Count; otherTeam++)
							if (otherTeam != thisTeam)
								for (int k = 0; k < teams [0] [thisTeam].LookingFor.Count; k++)
								{
									int index = 0;
									bool notFound = true;

									while (index < teams [0] [otherTeam].TradeBlock.Count && notFound)
										if (players [teams [0] [otherTeam].TradeBlock [index]].Position == teams [0] [thisTeam].LookingFor [k])
										{
											notFound = false;

											if (bestOptions [k] == -1 || players [teams [0] [otherTeam].TradeBlock [index]].Overall > players [bestOptions [k]].Overall)
												bestOptions [k] = teams [0] [otherTeam].TradeBlock [index];
										}
										else
											index++;
								}
					}
					else
						for (int otherTeam = 1; otherTeam < teams [0].Count; otherTeam++)
							if (otherTeam != thisTeam)
								for (int k = 0; k < teams [0] [thisTeam].LookingFor.Count; k++)
								{
									int index = 0;
									bool notFound = true;

									while (index < teams [0] [otherTeam].TradeBlock.Count && notFound)
										if (players [teams [0] [otherTeam].TradeBlock [index]].FirstTimeOnWaivers && players [teams [0] [otherTeam].TradeBlock [index]].Position == teams [0] [thisTeam].LookingFor [k])
										{
											notFound = false;

											if (bestOptions [k] == -1 || players [teams [0] [otherTeam].TradeBlock [index]].Overall > players [bestOptions [k]].Overall)
												bestOptions [k] = teams [0] [otherTeam].TradeBlock [index];
										}
										else
											index++;
								}
					
					for (int j = 0; j < bestOptions.Count; j++)
						if (bestOptions [j] != -1)
						{
							TradeOffer tradeOffer = new TradeOffer (thisTeam, players [bestOptions [j]].Team);
							int currentOffer = teams [0] [thisTeam].TradeBlock.Count - 1;
							float targetValue = players [bestOptions [j]].TradeValue * 0.9f, maxValue = players [bestOptions [j]].TradeValue * 1.1f;
							bool needMatch = true;

							tradeOffer.AddPlayer (bestOptions [j], tradeOffer.TheirTeam);
							tradeOffer.CalculateTheirValue ();

							while (currentOffer >= 0 && players [teams [0] [thisTeam].TradeBlock [currentOffer]].TradeValue <= maxValue && needMatch)
								if ((teams [0] [players [teams [0] [thisTeam].TradeBlock [currentOffer]].Team].LookingFor.Contains (players [teams [0] [thisTeam].TradeBlock [currentOffer]].Position) && players [teams [0] [thisTeam].TradeBlock [currentOffer]].TradeValue >= targetValue) || players [teams [0] [thisTeam].TradeBlock [currentOffer]].TradeValue >= tradeOffer.TheirValue)
								{
									needMatch = false;
									tradeOffer.AddPlayer (players [teams [0] [thisTeam].TradeBlock [currentOffer]].ID, thisTeam);
								}
								else
									currentOffer--;

							if (tradeOffer.HaveOffer)
								teams [0] [tradeOffer.TheirTeam].NewTradeOffer (tradeOffer);
						}
				}
			}
		}

		for (int i = 1; i < teams [0].Count; i++)
			teams [0] [i].AcceptTrades ();

		if (waivers.Count > 0)
		{
			for (int i = 0; i < waivers.Count; i++)
				waivers [i].AdvanceDay ();

			SaveWaivers ();
		}

		sw = new StreamWriter (@"Save\FreeAgents.txt");

		for (int i = 0; i < freeAgents.Count;)
			if (players [freeAgents [i]].OfferTime > 0 && --players [freeAgents [i]].OfferTime == 0)
			{
				teams [0] [players [freeAgents [i]].Team].AddPlayer (players [freeAgents [i]].ID);
				freeAgents.RemoveAt (i);
			}
			else
			{
				sw.WriteLine (freeAgents [i] + "," + players [freeAgents [i]].OfferTime + "," + players[freeAgents [i]].Offer + "," + players [freeAgents [i]].Team);
				i++;
			}

		sw.Close ();
		sw = new StreamWriter (@"Save\InternationalFreeAgents.txt");

		for (int i = 0; i < internationalFreeAgents.Count;)
			if (players [internationalFreeAgents [i]].OfferTime > 0 && --players [internationalFreeAgents [i]].OfferTime == 0)
			{
				teams [0] [players [internationalFreeAgents [i]].Team].AddPlayer (players [internationalFreeAgents [i]].ID);
				internationalFreeAgents.RemoveAt (i);
			}
			else
			{
				sw.WriteLine (internationalFreeAgents [i] + "," + players [internationalFreeAgents [i]].OfferTime + "," + players[internationalFreeAgents [i]].Offer + "," + players [internationalFreeAgents [i]].Team);
				i++;
			}

		sw.Close ();

		if (shortDisabledListChanged)
			SaveShortDisabledList ();

		if (longDisabledListChanged)
			SaveLongDisabledList ();

		dayIndex++;

		ExecuteOnMainThread.Enqueue (() => {
			SaveDayIndexAndHitStreak ();
		});
	}

	void SaveDayIndexAndHitStreak()
	{
		if (days [dayIndex - 1].SimulatedGames.Count > 0)
		{
			PlayerPrefs.SetInt ("LongestHitStreak", Manager.Instance.LongestHitStreak);
			PlayerPrefs.SetInt ("HitStreakYear", Manager.Instance.HitStreakYear);
			PlayerPrefs.SetString ("HitStreakName", Manager.Instance.HitStreakName);
		}

		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		PlayerPrefs.Save ();
	}

	public void SaveShortDisabledList ()
	{
		StreamWriter sw = new StreamWriter (@"Save\ShortDisabledList.txt");

		for (int i = 0; i < shortDisabledList.Count; i++)
			sw.WriteLine (shortDisabledList [i]);

		sw.Close ();

		shortDisabledListChanged = false;
	}

	public void SaveLongDisabledList ()
	{
		StreamWriter sw = new StreamWriter (@"Save\LongDisabledList.txt");

		for (int i = 0; i < longDisabledList.Count; i++)
			sw.WriteLine (longDisabledList [i]);

		sw.Close ();

		longDisabledListChanged = false;
	}

	void SimulateGame ()
	{
		days [dayIndex].SimulatedGames.Add (days [dayIndex].ScheduledGames [0].PlayGame ());
		days [dayIndex].ScheduledGames.RemoveAt (0);
	}

	// Creates all of the days for the current year
	public void CreateDays ()
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
	public void NewYear ()
	{
		StreamWriter sw = File.AppendText (@"Save\InternationalFreeAgents.txt");

		for (int i = 0; i < 40 + (int)(UnityEngine.Random.value * 20); i++)
			sw.WriteLine (NewInternationalFreeAgent ());
		
		sw.Close ();
		year++;
		PlayerPrefs.SetInt ("Year", year);
		finalsRounds = new List<int []> ();

		for (int i = 0; i < teams [0].Count; i++)
		{
			if (teams [0] [i].Cash < 0)
				ExecuteOnMainThread.Enqueue (() =>{
						teams [0] [i].Bankrupt ();
				});
			
			teams [0] [i].NewYear ();
		}

		Player.MinSalary += 10000;
		PlayerPrefs.SetFloat ("MinSalary", (float)Player.MinSalary);
		CreateDays ();
		dayIndex = 0;
		PlayerPrefs.SetInt ("DayIndex", dayIndex);
		PlayerPrefs.Save ();
		CreateSchedule.ScheduleEvents (days, DateTime.IsLeapYear (year));
		Calendar.ActivateNextButton ();
	}

	// Puts a player on waivers
	public void PutOnWaivers (int id, bool trade)
	{
		waivers.Add (new WaiverPlayer (id, trade));
	}

	// Takes a player off waivers
	public void TakeOffWaivers (int id)
	{
		int index = 0;

		while (index < waivers.Count && waivers [index].ID != id)
			index++;

		if (waivers [index].ID == id)
			waivers.RemoveAt (index);
	}

	// Saves waiver players
	public void SaveWaivers ()
	{
		StreamWriter sw = new StreamWriter (@"Save\Waivers.txt");

		for (int i = 0; i < waivers.Count; i++)
			sw.WriteLine (waivers [i].ToString ());
	}

	public void ScheduleFinalsGames (int team1, int team2, int offset, int round)
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
	public void DetermineMVP ()
	{
		double mvpWorth, cyWorth, bestMVP = 0.0, bestCY = 0.0;
		int mvpWinner = 0, cyWinner = 0;

		for (int i = 0; i < teams [0].Count; i++)
			for (int j = 0; j < teams [0] [i].Players.Count; j++)
			{
				if (players [teams [0] [i].Players [j]].Position.Contains ("P"))
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
					double ops = (players [teams [0] [i].Players [j]].Stats [0] [3] + players [teams [0] [i].Players [j]].Stats [0] [10]) / (double) (players [teams [0] [i].Players [j]].Stats [0] [1] + players [teams [0] [i].Players [j]].Stats [0] [10] + players [teams [0] [i].Players [j]].Stats [0] [14]) + (players [teams [0] [i].Players [j]].Stats [0] [3] + players [teams [0] [i].Players [j]].Stats [0] [5] * 2 + players [teams [0] [i].Players [j]].Stats [0] [6] * 3 + players [teams [0] [i].Players [j]].Stats [0] [7] * 4) / (double)players [teams [0] [i].Players [j]].Stats [0] [1];
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
		players.Add (newPlayer);
		teams [1] [(int)newPlayer.Country].AddPlayer (newPlayer.ID);
	}

	// Creates a new International free agent
	public string NewInternationalFreeAgent ()
	{
		Player newPlayer = new Player (Player.Positions [(int)(UnityEngine.Random.value * Player.Positions.Length)], 23, 22, players.Count);

		newPlayer.InternationalCountry ();
		newPlayer.SavePlayer ();
		newPlayer.SaveContract ();
		newPlayer.SaveStats ();
		NewPlayer (newPlayer);
		internationalFreeAgents.Add (newPlayer.ID);

		return newPlayer.ID + ",0,0.0,-1";
	}

	// Adds a player to the short disabled list
	public void AddToShortDisabledList (int index)
	{
		shortDisabledList.Add (index);
		shortDisabledListChanged = true;
		teams [0] [players [index].Team].RemoveFromFortyManRoster (index);
	}

	// Removes a player from the short disabled list
	public void RemoveFromShortDisabledList (int index)
	{
		shortDisabledList.Remove (index);
		shortDisabledListChanged = true;
		teams [0] [players [index].Team].AddToFortyManRoster (index);
	}

	// Removes a player from the short disabled list at an index
	public void RemoveFromShortDisabledListAt (int index)
	{
		shortDisabledList.RemoveAt (index);
		shortDisabledListChanged = true;
	}

	// Gets a player's index on the short disabled list
	public int ShortDisabledListIndex (int index)
	{
		return shortDisabledList.IndexOf(index);
	}

	// Adds a player to the long disabled list
	public void AddToLongDisabledList (int index)
	{
		longDisabledList.Add (index);
		longDisabledListChanged = true;
		teams [0] [players [index].Team].RemoveFromMajorLeague (index);
		teams [0] [players [index].Team].RemoveFromFortyManRoster (index);
	}

	// Removes a player from the long disabled list
	public void RemoveFromLongDisabledList (int index)
	{
		longDisabledList.Remove (index);
		longDisabledListChanged = true;
		teams [0] [players [index].Team].AddToFortyManRoster (index);
	}

	// Removes a player from the long disabled list at an index
	public void RemoveFromLongDisabledListAt (int index)
	{
		longDisabledList.RemoveAt (index);
		longDisabledListChanged = true;
	}

	// Gets a player's index on the long disabled list
	public int LongDisabledListIndex (int index)
	{
		return longDisabledList.IndexOf(index);
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

	public List<int> FreeAgents
	{
		get
		{
			return freeAgents;
		}
	}

	public List<int> InternationalFreeAgents
	{
		get
		{
			return internationalFreeAgents;
		}
	}

	public int [] FinalsTeams
	{
		get
		{
			return finalsTeams;
		}
	}

	public bool RosterChange
	{
		set
		{
			rosterChange = value;
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

	public System.Random RandomGen
	{
		get
		{
			return randomGen;
		}
	}

	public static Manager Instance
	{
		get
		{
			return instance;
		}
	}

	public static int ShortDisabledListTime
	{
		get
		{
			return shortDisabledListTime;
		}
	}

	public static int LongDisabledListTime
	{
		get
		{
			return longDisabledListTime;
		}
	}

	// Changes to the specified scene
	public static void ChangeToScene (int sceneToChangeTo)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneToChangeTo);
	}

	// Clears save
	public static void Clear ()
	{
		PlayerPrefs.DeleteAll ();

		if (Directory.Exists ("Save"))
			Directory.Delete ("Save", true);

		CreateFiles ();
	}

	// Creates files
	public static void CreateFiles ()
	{
		Directory.CreateDirectory ("Save");
		File.Create (@"Save\SimulatedGames.txt").Close ();
		File.Create (@"Save\FreeAgents.txt").Close ();
		File.Create (@"Save\InternationalFreeAgents.txt").Close ();
		File.Create (@"Save\ShortDisabledList.txt").Close ();
		File.Create (@"Save\LongDisabledList.txt").Close ();
		File.Create (@"Save\Events.txt").Close ();
		File.Create (@"Save\Waivers.txt").Close ();
	}
}