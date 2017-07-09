using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class Manager : MonoBehaviour
{
	private static Manager instance = null;
    static int numTeams = 30;								// Number of teams
	public List<Team> teams = new List<Team>();				// List of all teams
    public int numStats;		// 
    public bool needDraft, inFinals;
    public string[] stats = File.ReadAllLines("Stats.txt");
	public List<string> tradeList;
	public List<string> injuries;
	public int longestHitStreak = 0, hitStreakYear;
	public string hitStreakName;
	public TradeDeadline tradeDeadline;
	public List<int> hallOfFameCandidates;
	public List<HallOfFameInductee> hallOfFameInductees = new List<HallOfFameInductee>();
	public List<WaiverPlayer> waivers;

	private float newWidth;
	private Draft draft;
	private PlayerDisplay playerDisplay;
	private DraftedPlayerDisplay draftedPlayerDisplay;
	private static List<Day> days = new List<Day>();
	private static int dayIndex;
	private int year;
	private List<Player> players;
	private bool notLoaded = true;

	public List<Player> Players
	{
		get
		{
			return players;
		}
	}

	public static Manager Instance
	{
		get
		{
			return instance;
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

    // Use this for initialization
    void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		if (notLoaded)
		{
			// Loads everything after starting the game after having already played
			if (PlayerPrefs.HasKey ("Year"))
			{
				int index = 0;

				players = new List<Player> ();

				year = int.Parse (PlayerPrefs.GetString ("Year"));
				needDraft = bool.Parse (PlayerPrefs.GetString ("NeedDraft"));
				inFinals = bool.Parse (PlayerPrefs.GetString ("InFinals"));
				dayIndex = int.Parse (PlayerPrefs.GetString ("DayIndex"));
	            
				for (int i = 0; i < numTeams; i++)
				{
					Team team = new Team ();

					team.LoadTeam (i);
					teams.Add (team);
				}

				while (PlayerPrefs.HasKey ("Player" + index))
				{
					Player player = new Player ();
					player.LoadPlayer (index++);
					players.Add (player);

					if (player.team != -1)
						teams [player.team].players.Add (player.ID);
				}

				for (int i = 0; i < teams.Count; i++)
					teams [i].SetExpenses ();

				Player.longestFirstName = PlayerPrefs.GetInt ("LongestFirstName");
				Player.longestLastName = PlayerPrefs.GetInt ("LongestLastName");
			}
			// Creates a new game if it is the first time playing
	        else
				Restart ();

			notLoaded = false;
			draft = new Draft ();
			playerDisplay = new PlayerDisplay ();
			draftedPlayerDisplay = new DraftedPlayerDisplay ();

			int numDays = (DateTime.Parse (year + "/12/31") - DateTime.Parse (year + "/01/01")).Days;

			DateTime currDate = DateTime.Parse (year + "/01/01");

			for (int i = 0; i <= numDays; i++)
			{
				days.Add (new Day (currDate));
				currDate = currDate.AddDays (1);
			}
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

		players = new List<Player>();

		dayIndex = 0;
		PlayerPrefs.SetString ("DayIndex", "0");
		year = DateTime.Now.Year;
		PlayerPrefs.SetString ("Year", year.ToString ());
		needDraft = true;
		PlayerPrefs.SetString ("NeedDraft", "true");
		inFinals = false;
		PlayerPrefs.SetString ("InFinals", "false");
		tradeList = new List<string> ();

		// Adds the draft picks to the list
		for (int i = 0; i < numTeams; i++)
			picksLeft.Add (i);

		// Creates each team with 1 of batting position, 5 Starting Pitcher, 3 Relief Pitcher and 1 Closing Pitcher
		for (int i = 0; i < numTeams; i++)
		{
			Team team = new Team ();
			string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
			Player newPlayer;
			string[] splitName = (team.CityName + " " + team.TeamName).Split(' ');
			int pickIndex = (int)(UnityEngine.Random.value * picksLeft.Count);

			team.id = i;
			team.Pick = picksLeft [pickIndex];
			picksLeft.RemoveAt (pickIndex);
			for (int j = 0; j < splitName.Length; j++)
				if (System.Char.IsLetter(splitName[j][0]) && System.Char.IsUpper(splitName[j][0]))
					team.Shortform += splitName[j][0];

			for (int j = 3; j < positions.Length; j++)
			{
				newPlayer = new Player (positions [j], 23, 22, Manager.Instance.Players.Count, i);
				newPlayer.SetDraftYear ();
				newPlayer.SavePlayer ();
				players.Add (newPlayer);
				team.players.Add (newPlayer.ID);
			}

			for (int j = 0; j < 5; j++)
			{
				newPlayer = new Player ("SP", 23, 22, Manager.Instance.Players.Count, i);
				newPlayer.SetDraftYear ();
				newPlayer.SavePlayer ();
				players.Add (newPlayer);
				team.players.Add (newPlayer.ID);
			}

			for (int j = 0; j < 3; j++){
				newPlayer = new Player ("RP", 23, 22, Manager.Instance.Players.Count, i);
				newPlayer.SetDraftYear ();
				newPlayer.SavePlayer ();
				players.Add (newPlayer);
				team.players.Add (newPlayer.ID);
			}

			newPlayer = new Player ("CP", 23, 22, Manager.Instance.Players.Count, i);
			newPlayer.SetDraftYear ();
			newPlayer.SavePlayer ();
			players.Add (newPlayer);
			team.players.Add (newPlayer.ID);

			for (int j = team.players.Count; j < Team.RosterSize; j++)
			{
				newPlayer = new Player (Player.Positions[(int)(UnityEngine.Random.value * Player.Positions.Length)], 23, 22, Manager.Instance.Players.Count, i);
				newPlayer.SetDraftYear ();
				newPlayer.SavePlayer ();
				players.Add (newPlayer);
				team.players.Add (newPlayer.ID);
			}

			team.AutomaticRoster ();
			team.SaveLineup ();

			PlayerPrefs.SetString ("Team" + team.id, team.CityName + "," + team.TeamName + "," + team.Pick);
			PlayerPrefs.SetString ("Overalls" + team.id, team.Overalls [0] + "," + team.Overalls [1] + "," + team.Overalls [2]);
			PlayerPrefs.SetString ("WLH" + team.id.ToString (), "0,0,0.5");

			teams.Add (team);
		}

		// Stores the length of the longest first and last name to be able to display them properly
		PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
		PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

        PlayerPrefs.Save();
    }

	public void DisplayHeaders(Transform headerTrans, RectTransform parentsParentRect, DisplayType displayType)
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

			if(displayType == DisplayType.Draft)
				statHeader.GetComponent<Button>().onClick.AddListener(() => draft.Sort(headerNum));
			else if(displayType == DisplayType.Signed)
				statHeader.GetComponent<Button>().onClick.AddListener(() => draftedPlayerDisplay.Sort(headerNum));
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

	public void DisplayPlayers(List<int> players, Transform parentTrans, RectTransform parentRect, RectTransform parentsParentRect, DisplayType displayType)
	{
		GameObject[] currPlayers;
		UnityEngine.Object playerButton;
		string playerName;

		if (displayType == DisplayType.Draft)
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

			playerListing = Manager.Instance.Players[players[i]].firstName;

			for (int j = Manager.Instance.Players[players[i]].firstName.Length; j < Player.longestFirstName; j++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].lastName;

			for (int j = Manager.Instance.Players[players[i]].lastName.Length; j < Player.longestLastName; j++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].position;

			for (int k = Manager.Instance.Players[players[i]].position.Length; k < stats[2].Length; k++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].overall;

			for (int k = Manager.Instance.Players[players[i]].overall.ToString().Length; k < stats[3].Length; k++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].offense;

			for (int k = Manager.Instance.Players[players[i]].offense.ToString().Length; k < stats[4].Length; k++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].defense;

			for (int k = Manager.Instance.Players[players[i]].defense.ToString().Length; k < stats[5].Length; k++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].potential;

			for (int k = Manager.Instance.Players[players[i]].potential.ToString().Length; k < stats[6].Length; k++)
				playerListing += " ";

			playerListing += " " + Manager.Instance.Players[players[i]].age;

			for (int k = Manager.Instance.Players[players[i]].age.ToString().Length; k < stats[7].Length; k++)
				playerListing += " ";

			for (int j = 0; j < Manager.Instance.Players[players[i]].skills.Length - 1; j++)
			{
				playerListing += " " + Manager.Instance.Players[players[i]].skills[j];

				for (int k = Manager.Instance.Players[players [i]].skills [j].ToString ().Length; k < stats [j + 8].Length; k++)
					playerListing += " ";
			}

			playerListing += " " + Manager.Instance.Players[players[i]].skills[Manager.Instance.Players[players[i]].skills.Length - 1]; 
            
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            
			if (displayType == DisplayType.Draft)
				newPlayer.GetComponent<Button> ().onClick.AddListener (() => draft.PlayerDraft (newPlayer, playerListing));
			else if (displayType == DisplayType.Signed)
			{
				int id = players [i];
				newPlayer.GetComponent<Button> ().onClick.AddListener (() => draftedPlayerDisplay.ShowDraftedPlayer (id));
			}
			else
				newPlayer.GetComponent<Button> ().interactable = false;
        }

		parentRect.offsetMin = new Vector2(0, -(20 * (players.Count + 1) - parentsParentRect.rect.height));
		parentRect.offsetMax = new Vector2(newWidth, 0);
	}

	public static List<int> Sort(int headerNum, bool ascending, List<int> players)
	{
		if (ascending)
			switch (headerNum)
			{
			case 0:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].firstName).ToList ();
				break;
			case 1:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].lastName).ToList ();
				break;
			case 2:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].position).ToList ();
				break;
			case 3:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].overall).ToList ();
				break;
			case 4:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].offense).ToList ();
				break;
			case 5:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].defense).ToList ();
				break;
			case 6:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].potential).ToList ();
				break;
			case 7:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].age).ToList ();
				break;
			default:
				players = players.OrderBy (playerX => Manager.Instance.Players[playerX].skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum)
			{
			case 0:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].firstName).ToList ();
				break;
			case 1:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].lastName).ToList ();
				break;
			case 2:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].position).ToList ();
				break;
			case 3:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].overall).ToList ();
				break;
			case 4:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].offense).ToList ();
				break;
			case 5:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].defense).ToList ();
				break;
			case 6:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].potential).ToList ();
				break;
			case 7:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].age).ToList ();
				break;
			default:
				players = players.OrderByDescending (playerX => Manager.Instance.Players[playerX].skills [headerNum - 8]).ToList ();
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

	public void SetDraftedPlayerDisplayObjects(Transform teamList, Transform header, RectTransform teamListRect, RectTransform teamListParentRect, GameObject panel)
	{
		draftedPlayerDisplay.SetPlayerDisplayObjects (teamList, header, teamListRect, teamListParentRect, panel);
	}

	public void DisplayDraftedPlayers()
	{
		draftedPlayerDisplay.Display ();
	}

	public void SimulateDay()
	{
		days [dayIndex].SimulateDay ();
		dayIndex++;
		PlayerPrefs.SetString ("DayIndex", dayIndex.ToString());
		PlayerPrefs.Save ();
	}

	public void NewYear()
	{
		year++;
	}

	public void RemoveFromWaivers(int id)
	{
		int index = 0;

		while (index < waivers.Count && waivers [index].ID != id)
			index++;

		if (waivers [index].ID == id)
			waivers.RemoveAt (index);
	}

	// Changes to the specified scene
	public static void ChangeToScene(int sceneToChangeTo)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToChangeTo);
	}
}

public enum DisplayType
{
	Team,
	Draft,
	Signed
}