using UnityEngine;
using System.Collections;
using System.IO;

public class Player
{
	public int games, atBats, runs, hits, singles, doubles, triples, homeruns, totalBases, runsBattedIn, walks, strikeouts, stolenBases, caughtStealing, sacrifices, wins, losses, gamesStarted, saves, saveOpportunities, inningsPitched, atBatsAgainst, hitsAgainst, runsAgainst, earnedRuns, homerunsAgainst, walksAgainst, strikeoutsAgainst, qualityStarts, completeGames, hitStreak, reachedOnError, hitByPitch, longestHitStreak, hitStreakYear;
	public int potential, age, contractLength;
	public int[] skills = new int[10]; // 0power, 1contact, 2eye, 3speed, 4catching, 5throwing power, 6accuracy, 7movement, 8energy, 9endurance
	public float offense, defense, overall;
	public double salary;
	public string firstName, lastName, position;
	static string[] firstNames = File.ReadAllLines("FirstNames.txt"), lastNames = File.ReadAllLines("LastNames.txt");
	public int team, injuryLength;
	private int playerIndex;
	public static int longestFirstName = 10, longestLastName = 9;
	public string injuryLocation, injurySeriousness;
	public int[,,] runExpectancy = new int[2,3,8];
	public string country;
	private static int year;

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

	public int PlayerIndex
	{
		get
		{
			return playerIndex;
		}
		set
		{
			playerIndex = value;
		}
	}

	public static void SetYear(int _year)
	{
		year = _year;
	}

	public Player()
	{
	}

	// 1-arg constructor
	public Player(string newPosition, int minAge, int ageRange, int index = 0)
	{
		float randomCountry = Random.value;
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

		if(randomCountry <= 0.5437956204f)
			country = "United States";
		else if(randomCountry <= 0.7761557178f)
			country = "Dominican Republic";
		else if(randomCountry <= 0.8613138686f)
			country = "Venezuela";
		else if(randomCountry <= 0.8917274939f)
			country = "Puerto Rico";
		else if(randomCountry <= 0.9160583942f)
			country = "Cuba";
		else if(randomCountry <= 0.9318734793f)
			country = "Mexico";
		else if(randomCountry <= 0.9416058394f)
			country = "Japan";
		else if(randomCountry <= 0.9513381995f)
			country = "Canada";
		else if(randomCountry <= 0.9598540146f)
			country = "Panama";
		else if(randomCountry <= 0.9671532847f)
			country = "South Korea";
		else if(randomCountry <= 0.9720194647f)
			country = "Curaçao";
		else if(randomCountry <= 0.9768856448f)
			country = "Colombia";
		else if(randomCountry <= 0.9805352798f)
			country = "Germany";
		else if(randomCountry <= 0.9841849149f)
			country = "Nicaragua";
		else if(randomCountry <= 0.9878345499f)
			country = "Oceania";
		else if(randomCountry <= 0.9902676399f)
			country = "Taiwan";
		else if(randomCountry <= 0.9927007299f)
			country = "Virgin Islands";
		else if(randomCountry <= 0.99513382f)
			country = "Brazil";
		else if(randomCountry <= 0.996350365f)
			country = "South Africa";
		else if(randomCountry <= 0.99756691f)
			country = "Lithuania";
		else if(randomCountry <= 0.998783455f)
			country = "Netherlands";
		else
			country = "Aruba";

		if (firstName.Length > longestFirstName)
			longestFirstName = firstName.Length;

		if (lastName.Length > longestLastName)
			longestLastName = lastName.Length;

		playerIndex = index;

		Reset ();
	}

	// Resets player's stats
	void Reset()
	{
		games = 0;
		atBats = 0;
		runs = 0;
		hits = 0;
		doubles = 0;
		triples = 0;
		homeruns = 0;
		totalBases = 0;
		runsBattedIn = 0;
		walks = 0;
		strikeouts = 0;
		stolenBases = 0;
		caughtStealing = 0;
		sacrifices = 0;
		wins = 0;
		losses = 0;
		gamesStarted = 0;
		saves = 0;
		saveOpportunities = 0;
		inningsPitched = 0;
		atBatsAgainst = 0;
		hitsAgainst = 0;
		runsAgainst = 0;
		earnedRuns = 0;
		homerunsAgainst = 0;
		walksAgainst = 0;
		strikeoutsAgainst = 0;
		qualityStarts = 0;
		completeGames = 0;
		hitStreak = 0;
		reachedOnError = 0;
		hitByPitch = 0;
		longestHitStreak = 0;
		hitStreakYear = 0;
	}

	// Saves player
	public void SavePlayer(int teamNum)
	{
		PlayerPrefs.SetString ("Player" + teamNum + "-" + playerIndex, firstName + "," + lastName + "," + position + "," + potential + "," + age + "," + potential + "," + skills [0] + "," + skills [1] + "," + skills [2] + "," + skills [3] + "," + skills [4] + "," + skills [5] + "," + skills [6] + "," + skills [7] + "," + skills [8] + "," + skills [9] + "," + offense + "," + defense + "," + overall + "," + salary + "," + contractLength + "," + injuryLength + "," + country);
		SaveStats (teamNum);
	}

	// Saves stats
	public void SaveStats(int teamNum)
	{
		PlayerPrefs.SetString ("PlayerStats" + teamNum + "-" + playerIndex, games + "," + atBats + "," + runs + "," + hits + "," + doubles + "," + triples + "," + homeruns + "," + totalBases + "," + runsBattedIn + "," + walks + "," + strikeouts + "," + stolenBases + "," + caughtStealing + "," + sacrifices + "," + wins + "," + losses + "," + gamesStarted + "," + saves + "," + saveOpportunities + "," + inningsPitched + "," + atBatsAgainst + "," + hitsAgainst + "," + runsAgainst + "," + earnedRuns + "," + homerunsAgainst + "," + walksAgainst + "," + strikeoutsAgainst + "," + qualityStarts + "," + completeGames + "," + hitStreak + "," + reachedOnError + "," + hitByPitch + "," + longestHitStreak + "," + hitStreakYear); 
	}

	// Loads player and stats
	public void LoadPlayer(int teamNum, int index)
	{
		string player = PlayerPrefs.GetString("Player" + teamNum + "-" + index), stats = PlayerPrefs.GetString("PlayerStats" + teamNum + "-" + index);
		string[] playerSplit = player.Split (','), splitStats = stats.Split (',');
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
		country = playerSplit [22];

		games = int.Parse (splitStats [0]);
		atBats = int.Parse (splitStats [1]);
		runs = int.Parse (splitStats [2]);
		hits = int.Parse (splitStats [3]);
		doubles = int.Parse (splitStats [4]);
		triples = int.Parse (splitStats [5]);
		homeruns = int.Parse (splitStats [6]);
		totalBases = int.Parse (splitStats [7]);
		runsBattedIn = int.Parse (splitStats [8]);
		walks = int.Parse (splitStats [9]);
		strikeouts = int.Parse (splitStats [10]);
		stolenBases = int.Parse (splitStats [11]);
		caughtStealing = int.Parse (splitStats [12]);
		sacrifices = int.Parse (splitStats [13]);
		wins = int.Parse (splitStats [14]);
		losses = int.Parse (splitStats [15]);
		gamesStarted = int.Parse (splitStats [16]);
		saves = int.Parse (splitStats [17]);
		saveOpportunities = int.Parse (splitStats [18]);
		inningsPitched = int.Parse (splitStats [19]);
		atBatsAgainst = int.Parse (splitStats [20]);
		hitsAgainst = int.Parse (splitStats [21]);
		runsAgainst = int.Parse (splitStats [22]);
		earnedRuns = int.Parse (splitStats [23]);
		homerunsAgainst = int.Parse (splitStats [24]);
		walksAgainst = int.Parse (splitStats [25]);
		strikeoutsAgainst = int.Parse (splitStats [26]);
		qualityStarts = int.Parse (splitStats [27]);
		completeGames = int.Parse (splitStats [28]);
		hitStreak = int.Parse (splitStats [29]);
		reachedOnError = int.Parse (splitStats [30]);
		hitByPitch = int.Parse (splitStats [31]);
		longestHitStreak = int.Parse (splitStats [32]);
		hitStreakYear = int.Parse (splitStats [33]);

		playerIndex = index;
		team = teamNum;
	}

	// Injures player
	public void Injure()
	{
		float r1 = Random.value * 10 + 1, r2 = Random.value * 10 + 1, r3 = Random.value * 5 + 1;
		int seriousness = (int)(r1 * r2), location = (int)r3;
		AllTeams allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();


		if (seriousness == 25 && location == 5)
		{
			allTeams.teams [team].players.RemoveAt (playerIndex);
			allTeams.teams [team].OrderLineup ();
			Debug.Log(firstName + " " + lastName + " " + allTeams.teams[team].shortform + "Ended");
		}
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
			Debug.Log(firstName + " " + lastName + " " + allTeams.teams[team].shortform + " " + injurySeriousness + " " + injuryLocation + " " + injuryLength);
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
		if (strikeouts != 0)
			return (float)walks / strikeouts;
		else if (walks != 0)
			return 999.99f;
		else
			return 0.0f;
	}

	// Calculates batting average against
	public float BAA()
	{
		if (atBatsAgainst != 0)
			return (float)hitsAgainst / atBatsAgainst;
		else
			return 1.0f;
	}

	// Calculates isolated power
	public float ISO()
	{
		if (atBats != 0)
			return (float)(doubles + triples * 2 + homeruns * 3) / atBats;
		else
			return 0.0f;
	}

	// Calculates batting average
	public float BA()
	{
		if (atBats != 0)
			return (float)hits / atBats;
		else
			return 0.0f;
	}

	// Calculates linear weights ratio
	public double LWR()
	{
		if (position.Length == 1 || (position.Length == 2 && position.Substring (1) != "P"))
			return 0.47 * singles + 0.77 * doubles + 1.05 * triples + 1.40 * homeruns + 0.50 * reachedOnError + 0.31 * walks + 0.34 * hitByPitch - 0.27 * (atBats - hits - reachedOnError - strikeouts + sacrifices) - 0.29 * strikeouts;
		else
			return 0.0;
	}

	// Calculates power/speed number
	public float PSN()
	{
		int denominator = homeruns + stolenBases;
		if (denominator != 0)
			return (float)(2 * homeruns * stolenBases) / denominator;
		else
			return 0.0f;
	}

	// Calculates walks and hits per innings pitched
	public float WHIP()
	{
		if (inningsPitched != 0)
			return (walksAgainst + hitsAgainst) / (inningsPitched / 3.0f);
		else
			return 999.99f;
	}

	// Calculates runs created per 27 outs (1 game)
	public float RC27()
	{
		int denominator = atBats + walks;
		if(denominator != 0)
			return (hits + walks - caughtStealing) * (totalBases + (0.55f * stolenBases)) / (float)(atBats + walks) / 27;
		else
			return 0.0f;
	}
}