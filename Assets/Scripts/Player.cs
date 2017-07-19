using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Player
{
	public Country Country;
	public double Offer;
	public int Team;

	private int playerID, age, injuryLength, potential, draftYear = -1;
	private char bats, throws;
	private float fieldingChance, catchingChance;
	private double expectedSalary;
	private bool onWaivers;
	private List<ContractYear> contractYears = new List<ContractYear> ();
	private int [,,] runExpectancy = new int [2,3,8];
	private string firstName, lastName, position, injuryLocation, injurySeriousness;
	private float offense, defense, overall;
	private List<int []> stats = new List<int []> (); //0games, 1atBats, 2runs, 3hits, 4singles, 5doubles, 6triples, 7homeruns, 8totalBases, 9runsBattedIn, 10walks, 11strikeouts, 12stolenBases, 13caughtStealing, 14sacrifices, 15wins, 16losses, 17gamesStarted, 18saves, 19saveOpportunities, 20inningsPitched, 21atBatsAgainst, 22hitsAgainst, 23runsAgainst, 24earnedRuns, 25homerunsAgainst, 26walksAgainst, 27strikeoutsAgainst, 28qualityStarts, 29completeGames, 30hitStreak, 31reachedOnError, 32hitByPitch, 33longestHitStreak, 34hitStreakYear, 35noHitters, 36errors;
	private int [] skills = new int [11]; // 0power, 1contact, 2eye, 3speed, 4catching, 5throwing power, 6accuracy, 7movement, 8durability 9energy, 10endurance
	private List<Pitch> pitches;
	private List<int> pitchesAvailable;
	private bool isPitcher;

	public static int longestFirstName = 10, longestLastName = 9;

	private static string [] firstNames = File.ReadAllLines("FirstNames.txt"), lastNames = File.ReadAllLines("LastNames.txt");
	private static string [] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };

	// 0-Arg Conbstructor
	public Player()
	{
	}

	// 4/5-arg constructor for new player
	public Player(string newPosition, int minAge, int ageRange, int _playerID, int _team = -1)
	{
		float random;

		firstName = firstNames [(int)(Random.value * firstNames.Length)];
		lastName = lastNames [(int)(Random.value * lastNames.Length)];
		position = newPosition;
		age = (int)(Random.value * ageRange) + minAge;

		for(int i = 0; i < skills.Length - 1; i++)
			skills [i] = (int)(Random.value * age) + 55;
		
		potential = (int)(Random.value * 25 + (43 - age) * 3);

		if (potential < 0)
			potential = 0;

		CalculateOverall ();

		skills [10] = skills [9];

		expectedSalary = System.Math.Round((age + overall) * 100000 , 2);
		injuryLength = 0;
		RandomCountry ();
		Team = _team;
		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 3.0f * 5;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 2.0f * 5;
		draftYear = Manager.Instance.Year;

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
		onWaivers = false;
		pitches = new List<Pitch> ();

		if (position.Length > 1 && position.Substring (1, 1) == "P")
		{
			isPitcher = true;
			pitchesAvailable = new List<int> ();

			for (int i = 0; i < 19; i++)
				pitchesAvailable.Add (i);

			StreamWriter sw = new StreamWriter(@"Save\Pitches" + playerID + ".txt");

			if (Random.value > 0.5f)
			{
				NewPitch (0);
				sw.Write (0);
			}
			else
			{
				NewPitch (1);
				sw.Write (1);
			}

			for (int i = 0; i < 2 + (int)(Random.value * 3); i++)
			{
				int pitchNum = (int)(Random.value * pitchesAvailable.Count);

				NewPitch (pitchNum);
				sw.Write ("\n" + pitchNum);
			}

			sw.Close ();
			pitches.Sort ();
		}
		else
			isPitcher = false;

		NewYear ();
	}

	// Calculates the player's overall
	void CalculateOverall ()
	{
		offense = skills [0] + skills [1] + skills [2] + skills [3] + skills [7];
		defense = skills [4] + skills [5] + skills [6];
		overall = offense + defense;
		defense += skills [3] + skills [7];
		offense = Mathf.Round(offense / 5f);
		defense = Mathf.Round(defense / 5f);
		overall = Mathf.Round(overall / 8f);
	}

	// Adds a new pitch
	public void NewPitch (int index)
	{
		switch (pitchesAvailable [index])
		{
		case 0:
			pitches.Add (new FourSeam (skills [5], skills [7]));
			break;
		case 1:
			pitches.Add (new TwoSeam (skills [5], skills [7]));
			break;
		case 2:
			pitches.Add (new Cutter (skills [5], skills [7]));
			break;
		case 3:
			pitches.Add (new Shuuto (skills [5], skills [7]));
			break;
		case 4:
			pitches.Add (new Sinker (skills [5], skills [7]));
			break;
		case 5:
			pitches.Add (new Splitter (skills [5], skills [7]));
			break;
		case 6:
			pitches.Add (new Gyroball (skills [5], skills [7]));
			break;
		case 7:
			pitches.Add (new Curveball (skills [5], skills [7]));
			break;
		case 8:
			pitches.Add (new KnuckleCurve (skills [5], skills [7]));
			break;
		case 9:
			pitches.Add (new Screwball (skills [5], skills [7]));
			break;
		case 10:
			pitches.Add (new Slider (skills [5], skills [7]));
			break;
		case 11:
			pitches.Add (new Slurve (skills [5], skills [7]));
			break;
		case 12:
			pitches.Add (new CircleChangeup (skills [5], skills [7]));
			break;
		case 13:
			pitches.Add (new Forkball (skills [5], skills [7]));
			break;
		case 14:
			pitches.Add (new Fosh (skills [5], skills [7]));
			break;
		case 15:
			pitches.Add (new Palmball (skills [5], skills [7]));
			break;
		case 16:
			pitches.Add (new VulcanChangeup (skills [5], skills [7]));
			break;
		case 17:
			pitches.Add (new Eephus (skills [5], skills [7]));
			break;
		case 18:
			pitches.Add (new Knuckleball (skills [5], skills [7]));
			break;
		default:
			Debug.Log (index);
			break;
		}

		pitchesAvailable.RemoveAt(index);
	}
 
	// Resets player's stats
	public void NewYear()
	{
		StreamWriter sw;

		for(int i = 0; i < stats.Count; i++)
		{
			sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-" + i + ".txt");

			sw.Write (stats [i] [0]);

			for (int j = 1; j < stats [i].Length; j++)
				sw.Write ("," + stats [i] [j]);

			sw.Close();
		}

		stats.Insert (0, new int [37]);
		sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-" + (stats.Count - 1) + ".txt");
		stats [0] [0] = 0;

		for (int i = 1; i < stats [0].Length; i++)
		{
			stats [0] [i] = 0;
			sw.Write (",0");
		}

		sw.Close();
	}

	public void CalculatePotential ()
	{
		int increase;

		skills [9] = skills [10];

		if (potential <= 0)
			for (int i = 0; i < (int)(Random.value * 10); i++)
				skills [(int)(Random.value * (skills.Length - 1))]--;
		else
		{
			increase = (int)Mathf.Ceil (potential * 4 / 22f);
			potential -= increase;

			for (int m = 0; m < increase; m++)
				skills [(int)(Random.value * (skills.Length - 1))]++;
		}

		skills [10] = skills [9];

		CalculateOverall ();

		SavePlayer ();
	}

	// Saves player
	public void SavePlayer()
	{
		StreamWriter sw;

		sw = new StreamWriter (@"Save\Player" + playerID + ".txt");
		sw.Write (firstName + "," + lastName + "," + position + "," + potential + "," + age + "," + skills [0] + "," + skills [1] + "," + skills [2] + "," + skills [3] + "," + skills [4] + "," + skills [5] + "," + skills [6] + "," + skills [7] + "," + skills [8] + "," + skills [9] + "," + skills [10] + "," + offense + "," + defense + "," + overall + "," + expectedSalary + "," + injuryLength + "," + (int)Country + "," + Team + "," + draftYear + "," + onWaivers + "," + contractYears.Count + "," + pitches.Count);
		sw.Close();

		if (contractYears.Count > 0)
		{
			sw = new StreamWriter (@"Save\PlayerContracts" + playerID + ".txt");

			sw.Write ((int)contractYears [0].Type + "," + contractYears [0].Salary);

			for (int i = 1; i < contractYears.Count; i++)
				sw.Write ("\n" + (int)contractYears [i].Type + "," + contractYears [i].Salary);
		}

		sw.Close();
		SaveStats ();
	}

	// Saves stats
	public void SaveStats()
	{
		StreamWriter sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-0.txt");

		sw.Write (stats [0] [0]);

		for (int i = 1; i < stats [0].Length; i++)
			sw.Write("," + stats [0] [i]);

		sw.Close();
	}

	// Loads player and stats
	public void LoadPlayer(int _playerID)
	{
		string [] playerSplit = File.ReadAllLines(@"Save\Player"  + _playerID + ".txt") [0].Split (',');
		int numContractYears, numPitches;

		firstName = playerSplit [0];
		lastName = playerSplit [1];
		position = playerSplit [2];
		potential = int.Parse (playerSplit [3]);
		age = int.Parse (playerSplit [4]);
		skills [0] = int.Parse (playerSplit [5]);
		skills [1] = int.Parse (playerSplit [6]);
		skills [2] = int.Parse (playerSplit [7]);
		skills [3] = int.Parse (playerSplit [8]);
		skills [4] = int.Parse (playerSplit [9]);
		skills [5] = int.Parse (playerSplit [10]);
		skills [6] = int.Parse (playerSplit [11]);
		skills [7] = int.Parse (playerSplit [12]);
		skills [8] = int.Parse (playerSplit [13]);
		skills [9] = int.Parse (playerSplit [14]);
		skills [10] = int.Parse (playerSplit [15]);
		offense = float.Parse (playerSplit [16]);
		defense = float.Parse (playerSplit [17]);
		overall = float.Parse (playerSplit [18]); 
		expectedSalary = double.Parse (playerSplit [19]);
		injuryLength = int.Parse (playerSplit [20]);
		Country = (Country)int.Parse (playerSplit [21]);
		Team = int.Parse (playerSplit [22]);
		draftYear = int.Parse (playerSplit [23]);
		onWaivers = bool.Parse (playerSplit [24]);
		numContractYears = int.Parse (playerSplit [25]);
		numPitches = int.Parse (playerSplit [26]);

		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 3.0f * 5;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 2.0f * 5;

		for(int i = 0; i < Manager.Instance.Year - draftYear + 1; i++)
		{
			string [] splitStats = File.ReadAllLines(@"Save\PlayerStats" + _playerID + "-" + i + ".txt") [0].Split (',');

			stats.Add (new int [splitStats.Length]);

			for (int j = 0; j < splitStats.Length; j++)
				stats [i] [j] = int.Parse (splitStats [j]);
		}

		for(int i = 0; i < numContractYears; i++)
		{
			string [] split = File.ReadAllLines (@"Save\PlayerContracts" + _playerID + ".txt") [i].Split (',');

			contractYears.Insert (0, new ContractYear ((ContractType)int.Parse (split [0]), double.Parse (split [1])));
		}

		pitches = new List<Pitch> ();

		if (position.Length > 1 && position.Substring (1, 1) == "P")
		{
			string [] lines = File.ReadAllLines (@"Save\Pitches" + _playerID + ".txt");
			isPitcher = true;
			pitchesAvailable = new List<int> ();

			for (int i = 0; i < 19; i++)
				pitchesAvailable.Add (i);

			for(int i = 0; i < numPitches; i++)
				NewPitch (int.Parse(lines [i]));

			pitches.Sort ();
		}

		playerID = _playerID;
	}

	// Injures player
	public void Injure ()
	{
		float durability = 1 - (skills [8] / 100f);
		float r1 = (Random.value * 10 + 1) * durability, r2 = (Random.value * 10 + 1) * durability, r3 = Random.value * 5 + 1;
		int seriousness = (int)(r1 * r2), location = (int)r3;

		if (seriousness > 80 && location == 5)
			Debug.Log (firstName + " " + lastName + " Ended");
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

			if (injuryLength <= 7)
				injurySeriousness = "Very Minor";
			else if (injuryLength<= 14)
				injurySeriousness = "Minor";
			else if (injuryLength <= 21)
				injurySeriousness = "Moderate";
			else if (injuryLength <= 30)
				injurySeriousness = "Serious";
			else
				injurySeriousness = "Very Serious";
		}
	}

	// Retires player
	void Retire ()
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
		skills [9] = 0;
		skills [10] = 0;
	}

	// Calculates strikeout-to-walk ratio
	public float BBToK ()
	{
		if (stats [0] [11] != 0)
			return (float)stats [0] [10] / stats [0] [11];
		else if (stats [0] [10] != 0)
			return 999.99f;
		else
			return 0.0f;
	}

	// Calculates batting average against
	public float BAA()
	{
		if (stats [0] [21] != 0)
			return (float)stats [0] [22] / (float)stats [0] [21];
		else
			return 1.0f;
	}

	// Calculates on base percentage
	public float OBP()
	{
		float denominator = stats [0] [1] + stats [0] [10] + stats [0] [32] + stats [0] [14];

		if (denominator == 0f)
			return (stats [0] [3] + stats [0] [10] + stats [0] [32]) / 1f;
		else
			return (stats [0] [3] + stats [0] [10] + stats [0] [32]) / (float)denominator;
	}

	// Calculates slugging percentage
	public float SLUG()
	{
		if (stats [0] [1] == 0f)
			return stats [0] [8] / 1f;
		else
			return stats [0] [8] / (float)stats [0] [1];
	}

	// Calculates earned run average
	public float ERA()
	{
		if (stats [0] [20] == 0)
			return 99.99f;
		else
			return stats [0] [24] / (float)stats [0] [20];
	}

	// Calculates isolated power
	public float ISO()
	{
		if (stats [0] [1] != 0)
			return (float)(stats [0] [5] + stats [0] [6] * 2 + stats [0] [7] * 3) / stats [0] [1];
		else
			return 0.0f;
	}

	// Calculates batting average
	public float BA()
	{
		if (stats [0] [1] != 0)
			return (float)stats [0] [3] / stats [0] [1];
		else
			return 0.0f;
	}

	// Calculates linear weights ratio
	public double LWR()
	{
		if (position.Length == 1 || (position.Length == 2 && position.Substring (1) != "P"))
			return 0.47 * stats [0] [4] + 0.77 * stats [0] [5] + 1.05 * stats [0] [6] + 1.40 * stats [0] [7] + 0.50 * stats [0] [31] + 0.31 * stats [0] [10] + 0.34 * stats [0] [32] - 0.27 * (stats [0] [1] - stats [0] [3] - stats [0] [31] - stats [0] [11] + stats [0] [14]) - 0.29 * stats [0] [11];
		else
			return 0.0;
	}

	// Calculates power/speed number
	public float PSN()
	{
		int denominator = stats [0] [7] + stats [0] [12];
		if (denominator != 0)
			return (float)(2 * stats [0] [7] * stats [0] [12]) / denominator;
		else
			return 0.0f;
	}

	// Calculates walks and hits per innings pitched
	public float WHIP()
	{
		if (stats [0] [20] != 0)
			return (stats [0] [26] + stats [0] [22]) / (stats [0] [20] / 3.0f);
		else
			return 999.99f;
	}

	// Calculates runs created per 27 outs (1 game)
	public float RC27()
	{
		int denominator = stats [0] [1] + stats [0] [10];
		if (denominator != 0)
			return (stats [0] [3] + stats [0] [10] - stats [0] [13]) * (stats [0] [8] + (0.55f * stats [0] [12])) / (float)(stats [0] [1] + stats [0] [10]) / 27;
		else
			return 0.0f;
	}

	// Selects a random country for the player
	public void RandomCountry()
	{
		float randomCountry = Random.value;

		if (randomCountry <= 0.5437956204f)
			Country = Country.UnitedStates;
		else if (randomCountry <= 0.7761557178f)
			Country = Country.DominicanRepublic;
		else if (randomCountry <= 0.8613138686f)
			Country = Country.Venezuela;
		else if (randomCountry <= 0.8917274939f)
			Country = Country.PuertoRico;
		else if (randomCountry <= 0.9160583942f)
			Country = Country.Cuba;
		else if (randomCountry <= 0.9318734793f)
			Country = Country.Mexico;
		else if (randomCountry <= 0.9416058394f)
			Country = Country.Japan;
		else if (randomCountry <= 0.9513381995f)
			Country = Country.Canada;
		else if (randomCountry <= 0.9598540146f)
			Country = Country.Panama;
		else if (randomCountry <= 0.9671532847f)
			Country = Country.SouthKorea;
		else if (randomCountry <= 0.9720194647f)
			Country = Country.Curaçao;
		else if (randomCountry <= 0.9768856448f)
			Country = Country.Colombia;
		else if (randomCountry <= 0.9805352798f)
			Country = Country.Germany;
		else if (randomCountry <= 0.9841849149f)
			Country = Country.Nicaragua;
		else if (randomCountry <= 0.9878345499f)
			Country = Country.Australia;
		else if (randomCountry <= 0.9902676399f)
			Country = Country.Taiwan;
		else if (randomCountry <= 0.9927007299f)
			Country = Country.VirginIslands;
		else if (randomCountry <= 0.99513382f)
			Country = Country.Brazil;
		else if (randomCountry <= 0.996350365f)
			Country = Country.SouthAfrica;
		else if (randomCountry <= 0.99756691f)
			Country = Country.Lithuania;
		else if (randomCountry <= 0.998783455f)
			Country = Country.Netherlands;
		else
			Country = Country.Aruba;
	}

	// Considers the offer for a draft player to sign
	public bool ConsiderOffer()
	{
		if (Offer < expectedSalary)
			return false;
		else
			return true;
	}

	// Puts player on waivers
	public void PutOnWaivers()
	{
		onWaivers = true;
	}

	// Takes player off waivers
	public void TakeOffWaivers()
	{
		onWaivers = false;
	}

	// Reduces player's injury length
	public void ReduceInjuryLength ()
	{
		injuryLength--;
	}

	// Getters
	public List<Pitch> Pitches
	{
		get
		{
			return pitches;
		}
	}

	public bool IsPitcher
	{
		get
		{
			return isPitcher;
		}
	}

	public float Overall
	{
		get
		{
			return overall;
		}
	}

	public float Offense
	{
		get
		{
			return offense;
		}
	}

	public float Defense
	{
		get
		{
			return defense;
		}
	}

	public string FirstName
	{
		get
		{
			return firstName;
		}
	}

	public string LastName
	{
		get
		{
			return lastName;
		}
	}

	public string Position
	{
		get
		{
			return position;
		}
	}

	public List<int []> Stats
	{
		get
		{
			return stats;
		}
	}

	public int [] Skills
	{
		get
		{
			return skills;
		}
	}

	public int [,,] RunExpectancy
	{
		get
		{
			return runExpectancy;
		}
	}

	public int InjuryLength
	{
		get
		{
			return injuryLength;
		}
	}

	public string InjuryLocation
	{
		get
		{
			return injuryLocation;
		}
	}

	public string InjurySeriousness
	{
		get
		{
			return injurySeriousness;
		}
	}

	public int Potential
	{
		get
		{
			return potential;
		}
	}

	public int Age
	{
		get
		{
			return age;
		}
	}

	public List<ContractYear> ContractYears
	{
		get
		{
			return contractYears;
		}
	}

	public bool OnWaivers
	{
		get
		{
			return onWaivers;
		}
	}

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

	public double ExpectedSalary
	{
		get
		{
			return expectedSalary;
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

	public static string [] Positions
	{
		get
		{
			return positions;
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

public enum DeliveryType
{
	Overhand,
	Sidearm,
	Submarine
}