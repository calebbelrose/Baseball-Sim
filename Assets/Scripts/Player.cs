using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Player
{
	public List<int[]> stats = new List<int[]> (); //0games, 1atBats, 2runs, 3hits, 4singles, 5doubles, 6triples, 7homeruns, 8totalBases, 9runsBattedIn, 10walks, 11strikeouts, 12stolenBases, 13caughtStealing, 14sacrifices, 15wins, 16losses, 17gamesStarted, 18saves, 19saveOpportunities, 20inningsPitched, 21atBatsAgainst, 22hitsAgainst, 23runsAgainst, 24earnedRuns, 25homerunsAgainst, 26walksAgainst, 27strikeoutsAgainst, 28qualityStarts, 29completeGames, 30hitStreak, 31reachedOnError, 32hitByPitch, 33longestHitStreak, 34hitStreakYear, 35noHitters, 36errors;
	public int potential, age, contractLength;
	public int[] skills = new int[10]; // 0power, 1contact, 2eye, 3speed, 4catching, 5throwing power, 6accuracy, 7movement, 8energy, 9endurance
	public float offense, defense, overall;
	public string firstName, lastName, position;
	public int team, injuryLength;
	public static int longestFirstName = 10, longestLastName = 9;
	public string injuryLocation, injurySeriousness;
	public int[,,] runExpectancy = new int[2,3,8];
	public Country country;
	public bool MajorLeagueContract = false;
	public double Offer;

	private static int year;
	private static string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
	private int playerID;
	private int draftYear = -1;
	private char bats, throws;
	private float fieldingChance, catchingChance;
	private double salary;

	static string[] firstNames = File.ReadAllLines("FirstNames.txt"), lastNames = File.ReadAllLines("LastNames.txt");

	public char Bats
	{
		get
		{
			return bats;
		}
	}

	public char Throws
	{
		get
		{
			return throws;
		}
	}

	public double Salary
	{
		get
		{
			return salary;
		}
	}

	public float FieldingChance
	{
		get
		{
			return fieldingChance;
		}
	}

	public float CatchingChance
	{
		get
		{
			return catchingChance;
		}
	}

	public static string[] Positions
	{
		get
		{
			return positions;
		}
	}

	public static int Year
	{
		get
		{
			return year;
		}
	}

	public string Name
	{
		get
		{
			return firstName + " " + lastName;
		}
	}

	public int ID
	{
		get
		{
			return playerID;
		}
	}

	public static void SetYear(int _year)
	{
		year = _year;
	}

	public Player()
	{
	}

	// 1-arg constructor for new player
	public Player(string newPosition, int minAge, int ageRange, int _playerID, int _team = -1)
	{
		float random;

		firstName = firstNames [(int)(Random.value * firstNames.Length)];
		lastName = lastNames [(int)(Random.value * lastNames.Length)];
		position = newPosition;
		age = (int)(Random.value * ageRange) + minAge;
		skills[0] = (int)(Random.value * age) + 55;
		skills[1] = (int)(Random.value * age) + 55;
		skills[2] = (int)(Random.value * age) + 55;
		skills[3] = (int)(Random.value * age) + 55;
		skills[4] = (int)(Random.value * age) + 55;
		skills[5] = (int)(Random.value * age) + 55;
		skills[6] = (int)(Random.value * age) + 55;
		skills[7] = (int)(Random.value * age) + 55;
		skills[9] = (int)(Random.value * age) + 55;
		potential = (int)(Random.value * 25 + (43 - age) * 3);
		if (potential < 0)
			potential = 0;
		offense = skills[0] + skills[1] + skills[2] + skills[3] + skills[7];
		defense = skills[4] + skills[5] + skills[6];
		overall = offense + defense;
		defense += skills[3] + skills[7];
		offense = Mathf.Round(offense / 5f);
		defense = Mathf.Round(defense / 5f);
		overall = Mathf.Round(overall / 8f);
		skills[8] = skills[9];
		salary = System.Math.Round((age + overall) * 100000 , 2);
		contractLength = (int)(Random.value * 4) + 1;
		injuryLength = 0;
		RandomCountry ();
		team = _team;
		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 3.0f * 5;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 2.0f * 5;

		random = Random.value;
		if (random <= 0.425f)
			bats = 'R';
		else if (random <= 0.85f)
			bats = 'L';
		else
			bats = 'S';

		random = Random.value;
		if (random <= 0.495f)
			throws = 'R';
		else if (random <= 0.99f)
			throws = 'L';
		else
			throws = 'S';

		if (firstName.Length > longestFirstName)
			longestFirstName = firstName.Length;

		if (lastName.Length > longestLastName)
			longestLastName = lastName.Length;

		playerID = _playerID;

		NewYear ();
	}  
 
	// Resets player's stats
	void NewYear()
	{
		for(int i = 0; i < stats.Count; i++)
		{
			string stringStats = stats [i] [0].ToString();

			for (int j = 1; j < stats [i].Length; j++)
				stringStats += "," + stats [i] [j];

			PlayerPrefs.SetString ("PlayerStats" + playerID + "-" + i, stringStats);
		}

		stats.Insert (0, new int[37]);

		for (int i = 0; i < stats [0].Length; i++)
			stats [0] [i] = 0;
	}

	// Saves player
	public void SavePlayer()
	{
		PlayerPrefs.SetString ("Player" + playerID, firstName + "," + lastName + "," + position + "," + potential + "," + age + "," + potential + "," + skills [0] + "," + skills [1] + "," + skills [2] + "," + skills [3] + "," + skills [4] + "," + skills [5] + "," + skills [6] + "," + skills [7] + "," + skills [8] + "," + skills [9] + "," + offense + "," + defense + "," + overall + "," + salary + "," + contractLength + "," + injuryLength + "," + (int)country + "," + team + "," + draftYear);
		SaveStats ();
	}

	public void SetDraftYear(int year = -1)
	{
		if (year != -1)
			draftYear = year;
		else
		{
			draftYear = Manager.Instance.Year - age + (int)(Random.value * 9) + 16;

			if (draftYear > Manager.Instance.Year)
				draftYear = Manager.Instance.Year;
		}
	}

	// Saves stats
	public void SaveStats()
	{
		string stringStats = stats [0] [0].ToString();

		for (int i = 1; i < stats[0].Length; i++)
			stringStats += "," + stats [0] [i];

		PlayerPrefs.SetString ("PlayerStats" + playerID + "-0", stringStats);
	}

	// Loads player and stats
	public void LoadPlayer(int _playerID)
	{
		string player = PlayerPrefs.GetString ("Player" + _playerID);
		string[] playerSplit = player.Split (',');
		int index = 0;

		firstName = playerSplit [0];
		lastName = playerSplit [1];
		position = playerSplit [2];
		potential = int.Parse (playerSplit [3]);
		age = int.Parse (playerSplit [4]);
		potential = int.Parse (playerSplit [5]);
		skills [0] = int.Parse (playerSplit [6]);
		skills [1] = int.Parse (playerSplit [7]);
		skills [2] = int.Parse (playerSplit [8]);
		skills [3] = int.Parse (playerSplit [9]);
		skills [4] = int.Parse (playerSplit [10]);
		skills [5] = int.Parse (playerSplit [11]);
		skills [6] = int.Parse (playerSplit [12]);
		skills [7] = int.Parse (playerSplit [13]);
		skills [8] = int.Parse (playerSplit [14]);
		skills [9] = int.Parse (playerSplit [15]);
		offense = float.Parse (playerSplit [16]);
		defense = float.Parse (playerSplit [17]);
		overall = float.Parse (playerSplit [18]); 
		salary = double.Parse (playerSplit [19]);
		contractLength = int.Parse (playerSplit [20]);
		injuryLength = int.Parse (playerSplit [21]);
		country = (Country)int.Parse(playerSplit [22]);
		team = int.Parse(playerSplit [23]);
		draftYear = int.Parse(playerSplit [23]);

		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 3.0f * 5;

		while(PlayerPrefs.HasKey("PlayerStats" + _playerID + "-" + index))
		{
			string playerStats = PlayerPrefs.GetString("PlayerStats" + _playerID + "-" + index);
			string[] splitStats = playerStats.Split (',');

			stats.Add (new int[splitStats.Length]);

			for (int i = 0; i < splitStats.Length; i++)
				stats [index] [i] = int.Parse(splitStats [i]);

			index++;
		}

		playerID = _playerID;
	}

	// Injures player
	public void Injure()
	{
		float r1 = Random.value * 10 + 1, r2 = Random.value * 10 + 1, r3 = Random.value * 5 + 1;
		int seriousness = (int)(r1 * r2), location = (int)r3;

		if (seriousness == 25 && location == 5)
			Debug.Log(firstName + " " + lastName + " " + Manager.Instance.Players[playerID] + " Ended");
		else
		{
			injuryLength = (int)(r3 * 2 / 3 * r1 * r2);
			switch (location)
			{
			case 1:
				injuryLocation = "Arm";
				break;
			case 2:
				injuryLocation = "Leg";
				break;
			case 3:
				injuryLocation = "Groin";
				break;
			case 4:
				injuryLocation = "Neck";
				break;
			case 5:
				injuryLocation = "Back";
				break;
			}

			switch (seriousness)
			{
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				injurySeriousness = "Very Minor";
				break;
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
				injurySeriousness = "Minor";
				break;
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:		
				injurySeriousness = "Moderate";
				break;
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
				injurySeriousness = "Serious";
				break;
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
				injurySeriousness = "Very Serious";
				break;
			}
			Debug.Log(firstName + " " + lastName + " " + Manager.Instance.Players[playerID] + " " + injurySeriousness + " " + injuryLocation + " " + injuryLength);
		}
	}

	// Retires player
	void Retire()
	{
		skills [0] = 0;
		skills [1] = 0;
		skills [2] = 0;
		skills [3] = 0;
		skills [4] = 0;
		skills [5] = 0;
		skills [6] = 0;
		skills [7] = 0;
		skills [8] = 0;
	}

	// Calculates strikeout-to-walk ratio
	public float BBToK()
	{
		if (stats[0][11] != 0)
			return (float)stats[0][10] / stats[0][11];
		else if (stats[0][10] != 0)
			return 999.99f;
		else
			return 0.0f;
	}

	// Calculates batting average against
	public float BAA()
	{
		if (stats[0][21] != 0)
			return (float)stats[0][22] / stats[0][21];
		else
			return 1.0f;
	}

	// Calculates isolated power
	public float ISO()
	{
		if (stats[0][1] != 0)
			return (float)(stats[0][5] + stats[0][6] * 2 + stats[0][7] * 3) / stats[0][1];
		else
			return 0.0f;
	}

	// Calculates batting average
	public float BA()
	{
		if (stats[0][1] != 0)
			return (float)stats[0][3] / stats[0][1];
		else
			return 0.0f;
	}

	// Calculates linear weights ratio
	public double LWR()
	{
		if (position.Length == 1 || (position.Length == 2 && position.Substring (1) != "P"))
			return 0.47 * stats[0][4] + 0.77 * stats[0][5] + 1.05 * stats[0][6] + 1.40 * stats[0][7] + 0.50 * stats[0][31] + 0.31 * stats[0][10] + 0.34 * stats[0][32] - 0.27 * (stats[0][1] - stats[0][3] - stats[0][31] - stats[0][11] + stats[0][14]) - 0.29 * stats[0][11];
		else
			return 0.0;
	}

	// Calculates power/speed number
	public float PSN()
	{
		int denominator = stats[0][7] + stats[0][12];
		if (denominator != 0)
			return (float)(2 * stats[0][7] * stats[0][12]) / denominator;
		else
			return 0.0f;
	}

	// Calculates walks and hits per innings pitched
	public float WHIP()
	{
		if (stats[0][20] != 0)
			return (stats[0][26] + stats[0][22]) / (stats[0][20] / 3.0f);
		else
			return 999.99f;
	}

	// Calculates runs created per 27 outs (1 game)
	public float RC27()
	{
		int denominator = stats[0][1] + stats[0][10];
		if(denominator != 0)
			return (stats[0][3] + stats[0][10] - stats[0][13]) * (stats[0][8] + (0.55f * stats[0][12])) / (float)(stats[0][1] + stats[0][10]) / 27;
		else
			return 0.0f;
	}

	public void RandomCountry()
	{
		float randomCountry = Random.value;

		if(randomCountry <= 0.5437956204f)
			country = Country.UnitedStates;
		else if(randomCountry <= 0.7761557178f)
			country = Country.DominicanRepublic;
		else if(randomCountry <= 0.8613138686f)
			country = Country.Venezuela;
		else if(randomCountry <= 0.8917274939f)
			country = Country.PuertoRico;
		else if(randomCountry <= 0.9160583942f)
			country = Country.Cuba;
		else if(randomCountry <= 0.9318734793f)
			country = Country.Mexico;
		else if(randomCountry <= 0.9416058394f)
			country = Country.Japan;
		else if(randomCountry <= 0.9513381995f)
			country = Country.Canada;
		else if(randomCountry <= 0.9598540146f)
			country = Country.Panama;
		else if(randomCountry <= 0.9671532847f)
			country = Country.SouthKorea;
		else if(randomCountry <= 0.9720194647f)
			country = Country.Curaçao;
		else if(randomCountry <= 0.9768856448f)
			country = Country.Colombia;
		else if(randomCountry <= 0.9805352798f)
			country = Country.Germany;
		else if(randomCountry <= 0.9841849149f)
			country = Country.Nicaragua;
		else if(randomCountry <= 0.9878345499f)
			country = Country.Australia;
		else if(randomCountry <= 0.9902676399f)
			country = Country.Taiwan;
		else if(randomCountry <= 0.9927007299f)
			country = Country.VirginIslands;
		else if(randomCountry <= 0.99513382f)
			country = Country.Brazil;
		else if(randomCountry <= 0.996350365f)
			country = Country.SouthAfrica;
		else if(randomCountry <= 0.99756691f)
			country = Country.Lithuania;
		else if(randomCountry <= 0.998783455f)
			country = Country.Netherlands;
		else
			country = Country.Aruba;
	}

	public bool ConsiderOffer()
	{
		if (Offer < salary)
			return false;
		else
			return true;
	}
}

public enum Country
{
	UnitedStates = 0,
	DominicanRepublic = 1,
	Venezuela = 2,
	PuertoRico = 3,
	Cuba = 4,
	Mexico = 5,
	Japan = 6,
	Canada = 7,
	Panama = 8,
	SouthKorea = 9,
	Curaçao = 10,
	Colombia = 11,
	Germany = 12,
	Nicaragua = 13,
	Australia = 14,
	Taiwan = 15,
	VirginIslands = 16,
	Brazil = 17,
	SouthAfrica = 18,
	Lithuania = 19,
	Netherlands = 20,
	Aruba = 21
}

public enum CountryShortforms
{
	USA = 0,
	DOM = 1,
	VEN = 2,
	PRI = 3,
	CUB = 4,
	MEX = 5,
	JPN = 6,
	CAN = 7,
	PAN = 8,
	SKO = 9,
	CUW = 10,
	COL = 11,
	GER = 12,
	NCA = 13,
	AUS = 14,
	TWN = 15,
	VIR = 16,
	BRA = 17,
	RSA = 18,
	LTU = 19,
	NED = 20,
	ABW = 21
}