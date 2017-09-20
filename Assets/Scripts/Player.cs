using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Player
{
	public Country Country;																							// Country
	public double Offer;																							// Salary offer
	public int Team;																								// Team ID
	public int OfferTime;																							// Time left to consider offer
	public bool OnShortDisabledList;																				// Whether the player is on the short disabled list or not
	public bool OnLongDisabledList;																					// Whether the player is on the long disabled list or not

	private int playerID;																							// Player ID
	private int age;																								// Age
	private int injuryLength;																						// Injury Length
	private int potential;																							// Potential
	private int draftYear = -1;																						// Draft year
	private int batSplit;																							// Split for batting stats
	private int pitchSplit;																							// Split for pitching stats
	private int skin;																								// Index of image for skin
	private int freckles;																							// Index of image for freckles
	private int ear;																								// Index of image for ears
	private int face;																								// Index of image for face
	private int eyeShape;																							// Index of image for eye shape
	private int eyeColour;																							// Index of image for eye colou
	private int hair;																								// Index of image for hair
	private int mouth;																								// Index of image for mouth
	private int nose;																								// Index of image for nose
	private char bats;																								// Hand to bat with
	private char throws;																							// Hand to throw with
	private float fieldingChance;																					// Chance to field
	private float catchingChance;																					// Chance to catch
	private float injuredFieldingChance;																			// Chance to field while injured
	private float injuredCatchingChance;																			// Chance to catch while injured
	private float offense;																							// Offense
	private float defense;																							// Defense
	private float overall;																							// Overall
	private float popularity;																						// Popularity
	private float personality;																						// Personality
	private float tradeValue;																						// Trade value
	private double expectedSalary;																					// Expected salary
	private List<ContractYear> contractYears = new List<ContractYear> ();											// Contract years
	private int [,,] runExpectancy = new int [2,3,8];																// Run expectancy matrix
	private string firstName;																						// First name
	private string lastName;																						// Last name
	private string position;																						// Position
	private string injuryLocation;																					// Location of injury
	private string injurySeriousness;																				// Seriousness of injury
	private List<int []> stats = new List<int []> (); 																// 0games, 1atBats, 2runs, 3hits, 4singles, 5doubles, 6triples, 7homeruns, 8totalBases, 9runsBattedIn, 10walks, 11strikeouts, 12stolenBases, 13caughtStealing, 14sacrifices, 15wins, 16losses, 17gamesStarted, 18saves, 19saveOpportunities, 20inningsPitched, 21atBatsAgainst, 22hitsAgainst, 23runsAgainst, 24earnedRuns, 25homerunsAgainst, 26walksAgainst, 27strikeoutsAgainst, 28qualityStarts, 29completeGames, 30hitStreak, 31reachedOnError, 32hitByPitch, 33longestHitStreak, 34hitStreakYear, 35noHitters, 36errors;
	private List<int [] []> statSplits = new List<int [] []> ();													// 0atBats, 1runs, 2hits, 3singles, 4doubles, 5triples, 6homeruns, 7totalBases, 8runsBattedIn, 9walks, 10strikeouts, 11stolenBases, 12caughtStealing, 13sacrifices, 14inningsPitched, 15atBatsAgainst, 16hitsAgainst, 17runsAgainst, 18earnedRuns, 19homerunsAgainst, 20walksAgainst, 21strikeoutsAgainst, 22reachedOnError, 23hitByPitch;
	private int [] skills = new int [11]; 																			// 0power, 1contact, 2eye, 3speed, 4catching, 5throwing power, 6accuracy, 7movement, 8durability 9energy, 10endurance
	private List<Pitch> pitches;																					// Pitches learned
	private List<int> pitchesAvailable;																				// Pitches available to learn
	private bool isPitcher;																							// Whether the player is a pitcher or not
	private bool onWaivers;																							// Whether the player is on waivers or not
	private bool clearedWaivers;																					// Whether the player has cleared waivers this year or not
	private bool firstTimeOnWaivers;																				// Whether it would be the player's first time on waivers this year or not (will be put on irrevocable waivers the 2nd time)

	public static int longestFirstName = 10;																		// Longest first name
	public static int longestLastName = 9;																			// Longest last name
	public static double MinSalary;																					// Minimum salary

	private static string [] firstNames = File.ReadAllLines ("FirstNames.txt");										// Available first names
	private static string [] lastNames = File.ReadAllLines ("LastNames.txt");										// Available last names
	private static string [] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };	// Available positions

	// 0-Arg Conbstructor
	public Player ()
	{
	}

	// 4/5-arg constructor for new player
	public Player (string newPosition, int minAge, int ageRange, int _playerID, int _team = -1)
	{
		double random;
		StreamWriter sw;

		firstName = firstNames [(int)(Manager.Instance.RandomGen.NextDouble () * firstNames.Length)];
		lastName = lastNames [(int)(Manager.Instance.RandomGen.NextDouble () * lastNames.Length)];
		position = newPosition;
		age = (int)(Manager.Instance.RandomGen.NextDouble () * ageRange) + minAge;

		for (int i = 0; i < skills.Length - 1; i++)
			skills [i] = (int)(Manager.Instance.RandomGen.NextDouble () * age) + 55;
		
		potential = (int)(Manager.Instance.RandomGen.NextDouble () * 25 + (43 - age) * 3);

		if (potential < 0)
			potential = 0;

		CalculateOverall ();

		skills [10] = skills [9];

		if (overall < 65.0f)
			expectedSalary = MinSalary;
		else
			expectedSalary = System.Math.Round (((overall - 65.0f) / 40) * (25000000 - MinSalary + 5000000 * Manager.Instance.RandomGen.NextDouble ()), 2);
		
		injuryLength = 0;
		Team = _team;
		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 6000.0f;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 4000.0f;
		injuredFieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 12000.0f;
		injuredCatchingChance = 0.95f + (skills [3] + skills [4]) / 8000.0f;
		draftYear = Manager.Instance.Year;

		random = Manager.Instance.RandomGen.NextDouble ();
		if (random <= 0.425)
		{
			bats = 'R';
			batSplit = 0;
		}
		else if (random <= 0.85)
		{
			bats = 'L';
			batSplit = 1;
		}
		else
		{
			bats = 'S';
			batSplit = 0;
		}

		random = Manager.Instance.RandomGen.NextDouble ();
		if (random <= 0.495)
		{
			throws = 'R';
			pitchSplit = 0;
		}
		else if (random <= 0.99)
		{
			throws = 'L';
			pitchSplit = 1;
		}
		else
		{
			throws = 'S';
			pitchSplit = 0;
		}

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

			sw = new StreamWriter (@"Save\Pitches" + playerID + ".txt");

			if (Manager.Instance.RandomGen.NextDouble () > 0.5)
			{
				NewPitch (0);
				sw.Write (0 + "," + pitches [pitches.Count - 1].Effectiveness);
			}
			else
			{
				NewPitch (1);
				sw.Write (1 + "," + pitches [pitches.Count - 1].Effectiveness);
			}

			for (int i = 0; i < 2 + (int) (Manager.Instance.RandomGen.NextDouble () * 3); i++)
			{
				int pitchNum = (int) (Manager.Instance.RandomGen.NextDouble () * pitchesAvailable.Count);

				NewPitch (pitchNum);
				sw.Write ("\n" + pitchNum + "," + pitches [pitches.Count - 1].Effectiveness);
			}

			sw.Close ();
			pitches.Sort ();
		}
		else
			isPitcher = false;

		skin = (int)(Manager.Instance.RandomGen.NextDouble () * 5);
		freckles = (int)(Manager.Instance.RandomGen.NextDouble () * 4);
		ear = (int)(Manager.Instance.RandomGen.NextDouble () * 16) + skin;
			face = (int)(Manager.Instance.RandomGen.NextDouble () * 16) + skin;
		eyeShape = (int)(Manager.Instance.RandomGen.NextDouble () * 7);
		eyeColour = (int)(Manager.Instance.RandomGen.NextDouble () * 6);
		hair = (int)(Manager.Instance.RandomGen.NextDouble () * 5);
		mouth = (int)(Manager.Instance.RandomGen.NextDouble () * 10);
		nose = (int)(Manager.Instance.RandomGen.NextDouble () * 6);

		sw = new StreamWriter (@"Save\PlayerFace" + playerID + ".txt");
		sw.Write(skin + "," + freckles + "," + ear + "," + face + "," + eyeShape + "," + eyeColour + "," + hair + "," + mouth + "," + nose);
		sw.Close ();

		statSplits.Add(new int[2] []);
		statSplits [0] [0] = new int [24];
		statSplits [0] [1] = new int [24];

		popularity = (float)Manager.Instance.RandomGen.NextDouble () * 2 -1.0f;
		personality = (float)Manager.Instance.RandomGen.NextDouble () * 2 -1.0f;
		CalculateTradeValue ();

		NewYear ();
	}

	// Calculates the player's overall
	void CalculateOverall ()
	{
		offense = skills [0] + skills [1] + skills [2] + skills [3] + skills [7];
		defense = skills [4] + skills [5] + skills [6];
		overall = offense + defense;
		defense += skills [3] + skills [7];
		offense = Mathf.Round (offense / 5f);
		defense = Mathf.Round (defense / 5f);
		overall = Mathf.Round (overall / 8f);
	}

	// Adds a new pitch
	public void NewPitch (int index, float effectiveness = -1.0f)
	{
		switch (pitchesAvailable [index])
		{
		case 0:
			pitches.Add (new FourSeam (skills [5], skills [7], effectiveness));
			break;
		case 1:
			pitches.Add (new TwoSeam (skills [5], skills [7], effectiveness));
			break;
		case 2:
			pitches.Add (new Cutter (skills [5], skills [7], effectiveness));
			break;
		case 3:
			pitches.Add (new Shuuto (skills [5], skills [7], effectiveness));
			break;
		case 4:
			pitches.Add (new Sinker (skills [5], skills [7], effectiveness));
			break;
		case 5:
			pitches.Add (new Splitter (skills [5], skills [7], effectiveness));
			break;
		case 6:
			pitches.Add (new Gyroball (skills [5], skills [7], effectiveness));
			break;
		case 7:
			pitches.Add (new Curveball (skills [5], skills [7], effectiveness));
			break;
		case 8:
			pitches.Add (new KnuckleCurve (skills [5], skills [7], effectiveness));
			break;
		case 9:
			pitches.Add (new Screwball (skills [5], skills [7], effectiveness));
			break;
		case 10:
			pitches.Add (new Slider (skills [5], skills [7], effectiveness));
			break;
		case 11:
			pitches.Add (new Slurve (skills [5], skills [7], effectiveness));
			break;
		case 12:
			pitches.Add (new CircleChangeup (skills [5], skills [7], effectiveness));
			break;
		case 13:
			pitches.Add (new Forkball (skills [5], skills [7], effectiveness));
			break;
		case 14:
			pitches.Add (new Fosh (skills [5], skills [7], effectiveness));
			break;
		case 15:
			pitches.Add (new Palmball (skills [5], skills [7], effectiveness));
			break;
		case 16:
			pitches.Add (new VulcanChangeup (skills [5], skills [7], effectiveness));
			break;
		case 17:
			pitches.Add (new Eephus (skills [5], skills [7], effectiveness));
			break;
		case 18:
			pitches.Add (new Knuckleball (skills [5], skills [7], effectiveness));
			break;
		default:
			Debug.Log (index);
			break;
		}

		pitchesAvailable.RemoveAt (index);
	}
 
	// Resets player's stats
	public void NewYear ()
	{
		StreamWriter sw;

		for (int i = 0; i < stats.Count; i++)
		{
			sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-" + i + ".txt");

			for (int j = 0; j < 2; j++)
			{
				sw.Write (statSplits [j] [i] [0]);

				for (int k = 1; k < statSplits [j] [i].Length; k++)
					sw.Write ("," + statSplits [j] [i] [k]);

				sw.WriteLine ("");
			}

			sw.Write (stats [i] [0]);

			for (int j = 1; j < stats [i].Length; j++)
				sw.Write ("," + stats [i] [j]);

			sw.Close ();
		}

		stats.Insert (0, new int [37]);
		statSplits.Insert (0, new int[2] []);
		statSplits [0] [0] = new int [24];
		statSplits [0] [1] = new int [24];
		sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-" + (stats.Count - 1) + ".txt");

		for (int i = 0; i < 2; i++)
		{
			statSplits [0] [i] [0] = 0;
			sw.Write ("0");

			for (int j = 1; j < statSplits [0] [i].Length; j++)
			{
				statSplits [0] [i] [j] = 0;
				sw.Write (",0");
			}
			sw.WriteLine ("");
		}

		stats [0] [0] = 0;
		sw.Write ("0");

		for (int i = 1; i < stats [0].Length; i++)
		{
			stats [0] [i] = 0;
			sw.Write (",0");
		}

		sw.Close ();

		clearedWaivers = false;
	}

	public void CalculatePotential ()
	{
		int increase;

		skills [9] = skills [10];

		if (potential <= 0)
			for (int i = 0; i < (int) (Random.value * 10); i++)
				skills [ (int) (Random.value * (skills.Length - 1))]--;
		else
		{
			increase = (int)Mathf.Ceil (potential * 4 / 22f);
			potential -= increase;

			for (int m = 0; m < increase; m++)
				skills [ (int) (Random.value * (skills.Length - 1))]++;
		}

		skills [10] = skills [9];

		CalculateOverall ();
		CalculateTradeValue ();

		SavePlayer ();
	}

	// Saves player
	public void SavePlayer ()
	{
		StreamWriter sw;

		sw = new StreamWriter (@"Save\Player" + playerID + ".txt");
		sw.Write (firstName + "," + lastName + "," + position + "," + potential + "," + age + "," + skills [0] + "," + skills [1] + "," + skills [2] + "," + skills [3] + "," + skills [4] + "," + skills [5] + "," + skills [6] + "," + skills [7] + "," + skills [8] + "," + skills [9] + "," + skills [10] + "," + offense + "," + defense + "," + overall + "," + expectedSalary + "," + injuryLength + "," + (int)Country + "," + Team + "," + draftYear + "," + onWaivers + "," + contractYears.Count + "," + pitches.Count + "," + popularity + "," + personality + "," + bats + "," + throws + "," + clearedWaivers);
		sw.Close ();
	}

	public void SaveContract ()
	{
		StreamWriter sw;

		sw = new StreamWriter (@"Save\PlayerContracts" + playerID + ".txt");

		if (contractYears.Count > 0)
			for (int i = 0; i < contractYears.Count; i++)
				sw.WriteLine ((int)contractYears [i].Type + "," + contractYears [i].Salary);
		
		sw.Close ();
	}

	// Saves stats
	public void SaveStats ()
	{
		StreamWriter sw = new StreamWriter (@"Save\PlayerStats" + playerID + "-0.txt");

		for (int i = 0; i < 2; i++)
		{
			sw.Write (statSplits [0] [i] [0]);

			for (int j = 1; j < statSplits [0] [i].Length; j++)
				sw.Write ("," + statSplits [0] [i] [j]);

			sw.WriteLine ("");
		}

		sw.Write (stats [0] [0]);

		for (int i = 1; i < stats [0].Length; i++)
			sw.Write ("," + stats [0] [i]);

		sw.Close ();
	}

	// Loads player and stats
	public void LoadPlayer (int _playerID)
	{
		string [] split = File.ReadAllLines (@"Save\Player" + _playerID + ".txt") [0].Split (',');
		int numContractYears, numPitches;

		firstName = split [0];
		lastName = split [1];
		position = split [2];
		potential = int.Parse (split [3]);
		age = int.Parse (split [4]);
		skills [0] = int.Parse (split [5]);
		skills [1] = int.Parse (split [6]);
		skills [2] = int.Parse (split [7]);
		skills [3] = int.Parse (split [8]);
		skills [4] = int.Parse (split [9]);
		skills [5] = int.Parse (split [10]);
		skills [6] = int.Parse (split [11]);
		skills [7] = int.Parse (split [12]);
		skills [8] = int.Parse (split [13]);
		skills [9] = int.Parse (split [14]);
		skills [10] = int.Parse (split [15]);
		offense = float.Parse (split [16]);
		defense = float.Parse (split [17]);
		overall = float.Parse (split [18]); 
		expectedSalary = double.Parse (split [19]);
		injuryLength = int.Parse (split [20]);
		Country = (Country)int.Parse (split [21]);
		Team = int.Parse (split [22]);
		draftYear = int.Parse (split [23]);
		onWaivers = bool.Parse (split [24]);
		numContractYears = int.Parse (split [25]);
		numPitches = int.Parse (split [26]);
		popularity = float.Parse (split [27]);
		personality = float.Parse (split [28]);
		bats = split [29] [0];
		throws = split [30] [0];
		clearedWaivers = bool.Parse (split [31]);

		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 6.0f * 5;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 4.0f * 5;
		fieldingChance = 0.95f + (skills [3] + skills [5] + skills [6]) / 6.0f * 5;
		catchingChance = 0.95f + (skills [3] + skills [4]) / 4.0f * 5;

		for (int i = 0; i < Manager.Instance.Year - draftYear + 1; i++)
		{
			string [] allStats = File.ReadAllLines (@"Save\PlayerStats" + _playerID + "-" + i + ".txt");

			statSplits.Add(new int[2] []);

			for(int j = 0; j < 2; j++)
			{
				split = allStats [j].Split (',');
				statSplits [0] [j] = new int [split.Length];

				for (int k = 0; k < split.Length; k++)
					statSplits [i] [j] [k] = int.Parse (split [k]);
			}

			split = allStats [2].Split (',');
			stats.Add (new int [split.Length]);

			for (int j = 0; j < split.Length; j++)
				stats [i] [j] = int.Parse (split [j]);
		}

		for (int i = 0; i < numContractYears; i++)
		{
			split = File.ReadAllLines (@"Save\PlayerContracts" + _playerID + ".txt") [i].Split (',');
			contractYears.Insert (0, new ContractYear ((ContractType)int.Parse (split [0]), double.Parse (split [1])));
		}

		pitches = new List<Pitch> ();

		if (position.Length > 1 && position.Substring (1, 1) == "P")
		{
			split = File.ReadAllLines (@"Save\Pitches" + _playerID + ".txt");
			isPitcher = true;
			pitchesAvailable = new List<int> ();

			for (int i = 0; i < 19; i++)
				pitchesAvailable.Add (i);

			for (int i = 0; i < numPitches; i++)
			{
				string[] pitch = split [i].Split (',');

				NewPitch (int.Parse (pitch [0]), float.Parse (pitch [1]));
			}

			pitches.Sort ();
		}

		split = File.ReadAllLines (@"Save\PlayerFace" + _playerID + ".txt") [0].Split (',');
		skin = int.Parse(split [0]);
		freckles = int.Parse(split [1]);
		ear = int.Parse(split [2]);
		face = int.Parse(split [3]);
		eyeShape = int.Parse(split [4]);
		eyeColour = int.Parse(split [5]);
		hair = int.Parse(split [6]);
		mouth = int.Parse(split [7]);
		nose = int.Parse(split [8]);

		CalculateTradeValue ();

		playerID = _playerID;
	}

	// Injures player
	public void Injure ()
	{
		if (injuryLength == 0)
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
				else if (injuryLength <= 14)
					injurySeriousness = "Minor";
				else if (injuryLength <= 21)
					injurySeriousness = "Moderate";
				else if (injuryLength <= 30)
					injurySeriousness = "Serious";
				else
					injurySeriousness = "Very Serious";
			}
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
	public float BBToK
	{
		get
		{
			if (stats [0] [11] != 0)
				return (float)stats [0] [10] / stats [0] [11];
			else if (stats [0] [10] != 0)
				return (float)stats [0] [10];
			else
				return 0.0f;
		}
	}

	// Calculates batting average against
	public float BAA
	{
		get
		{
			if (stats [0] [21] != 0)
				return (float)stats [0] [22] / (float)stats [0] [21];
			else
				return 1.0f;
		}
	}

	// Calculates on base percentage
	public float OBP
	{
		get
		{
			float denominator = stats [0] [1] + stats [0] [10] + stats [0] [32] + stats [0] [14];

			if (denominator == 0f)
				return (stats [0] [3] + stats [0] [10] + stats [0] [32]) / 1f;
			else
				return (stats [0] [3] + stats [0] [10] + stats [0] [32]) / (float)denominator;
		}
	}

	// Calculates slugging percentage
	public float SLUG
	{
		get
		{
			if (stats [0] [1] == 0f)
				return stats [0] [8] / 1f;
			else
				return stats [0] [8] / (float)stats [0] [1];
		}
	}

	// Calculates earned run average
	public float ERA
	{
		get
		{
			if (stats [0] [20] == 0)
				return 99.99f;
			else
				return stats [0] [24] / (float)stats [0] [20];
		}
	}

	// Calculates isolated power
	public float ISO
	{
		get
		{
			if (stats [0] [1] != 0)
				return (float)(stats [0] [5] + stats [0] [6] * 2 + stats [0] [7] * 3) / stats [0] [1];
			else
				return 0.0f;
		}
	}

	// Calculates batting average
	public float BA
	{
		get
		{
			if (stats [0] [1] != 0)
				return (float)stats [0] [3] / stats [0] [1];
			else
				return 0.0f;
		}
	}

	// Calculates linear weights ratio
	public double LWR
	{
		get
		{
			if (position.Length == 1 || (position.Length == 2 && position.Substring (1) != "P"))
				return 0.47 * stats [0] [4] + 0.77 * stats [0] [5] + 1.05 * stats [0] [6] + 1.40 * stats [0] [7] + 0.50 * stats [0] [31] + 0.31 * stats [0] [10] + 0.34 * stats [0] [32] - 0.27 * (stats [0] [1] - stats [0] [3] - stats [0] [31] - stats [0] [11] + stats [0] [14]) - 0.29 * stats [0] [11];
			else
				return 0.0;
		}
	}

	// Calculates power/speed number
	public float PSN
	{
		get
		{
			int denominator = stats [0] [7] + stats [0] [12];
			if (denominator != 0)
				return (float)(2 * stats [0] [7] * stats [0] [12]) / denominator;
			else
				return 0.0f;
		}
	}

	// Calculates walks and hits per innings pitched
	public float WHIP
	{
		get
		{
			if (stats [0] [20] != 0)
				return (stats [0] [26] + stats [0] [22]) / (stats [0] [20] / 3.0f);
			else
				return 999.99f;
		}
	}

	// Calculates runs created per 27 outs (1 game)
	public float RC27
	{
		get
		{
			int denominator = stats [0] [1] + stats [0] [10];

			if (denominator != 0)
				return (stats [0] [3] + stats [0] [10] - stats [0] [13]) * (stats [0] [8] + (0.55f * stats [0] [12])) / (float)denominator / 27;
			else
				return 0.0f;
		}
	}

	// Selects a random country for the player
	public void RandomCountry ()
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

	// Selects a Country for Draft players
	public void DraftCountry ()
	{
		float randomCountry = Random.value;

		if (randomCountry <= 0.927385892f)
			Country = Country.UnitedStates;
		else if (randomCountry <= 0.9792531119f)
			Country = Country.PuertoRico;
		else if (randomCountry <= 0.9958506223f)
			Country = Country.Canada;
		else
			Country = Country.VirginIslands;
	}

	// Selects a country for International players
	public void InternationalCountry ()
	{
		float randomCountry = Random.value;

		if (randomCountry <= 0.561764706f)
			Country = Country.DominicanRepublic;
		else if (randomCountry <= 0.7676470588f)
			Country = Country.Venezuela;
		else if (randomCountry <= 0.8264705883f)
			Country = Country.Cuba;
		else if (randomCountry <= 0.8647058823f)
			Country = Country.Mexico;
		else if (randomCountry <= 0.8882352941f)
			Country = Country.Japan;
		else if (randomCountry <= 0.9088235294f)
			Country = Country.Panama;
		else if (randomCountry <= 0.9264705883f)
			Country = Country.SouthKorea;
		else if (randomCountry <= 0.9382352941f)
			Country = Country.Curaçao;
		else if (randomCountry <= 0.9500000001f)
			Country = Country.Colombia;
		else if (randomCountry <= 0.9588235294f)
			Country = Country.Germany;
		else if (randomCountry <= 0.967647059f)
			Country = Country.Nicaragua;
		else if (randomCountry <= 0.9764705883f)
			Country = Country.Australia;
		else if (randomCountry <= 0.9823529412f)
			Country = Country.Taiwan;
		else if (randomCountry <= 0.9882352943f)
			Country = Country.Brazil;
		else if (randomCountry <= 0.9911764707f)
			Country = Country.SouthAfrica;
		else if (randomCountry <= 0.9941176471f)
			Country = Country.Lithuania;
		else if (randomCountry <= 0.9970588235f)
			Country = Country.Netherlands;
		else
			Country = Country.Aruba;
	}

	// Calculates trade balue
	public void CalculateTradeValue ()
	{
		tradeValue = overall + potential / 7.0f;
	}

	// String used to display the player
	public string DisplayString ()
	{
		string playerString = firstName;

		for (int j = firstName.Length; j < Player.longestFirstName; j++)
			playerString += " ";

		playerString += " " + lastName;

		for (int j = LastName.Length; j < Player.longestLastName; j++)
			playerString += " ";

		playerString += " " + position;

		for (int k = Position.Length; k < Manager.Instance.Skills [2].Length; k++)
			playerString += " ";

		playerString += " " + overall;

		for (int k = Overall.ToString ().Length; k < Manager.Instance.Skills [3].Length; k++)
			playerString += " ";

		playerString += " " + offense;

		for (int k = Offense.ToString ().Length; k < Manager.Instance.Skills [4].Length; k++)
			playerString += " ";

		playerString += " " + defense;

		for (int k = Defense.ToString ().Length; k < Manager.Instance.Skills [5].Length; k++)
			playerString += " ";

		playerString += " " + potential;

		for (int k = Potential.ToString ().Length; k < Manager.Instance.Skills [6].Length; k++)
			playerString += " ";

		playerString += " " + age;

		for (int k = Age.ToString ().Length; k < Manager.Instance.Skills [7].Length; k++)
			playerString += " ";

		for (int j = 0; j < skills.Length - 1; j++)
		{
			playerString += " " + skills [j];

			for (int k = skills [j].ToString ().Length; k < Manager.Instance.Skills [j + 8].Length; k++)
				playerString += " ";
		}

		playerString += " " + skills [skills.Length - 1];

		return playerString;
	}

	// Considers the offer for a draft player to sign
	public bool ConsiderOffer ()
	{
		if (Offer < expectedSalary)
			return false;
		else
			return true;
	}

	// Puts player on waivers
	public void PutOnWaivers ()
	{
		onWaivers = true;
	}

	// Takes player off waivers
	public void TakeOffWaivers ()
	{
		onWaivers = false;
		firstTimeOnWaivers = false;
	}

	// Clears waivers
	public void ClearWaivers ()
	{
		clearedWaivers = true;
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

	public bool FirstTimeOnWaivers
	{
		get
		{
			return firstTimeOnWaivers;
		}
	}

	public float TradeValue
	{
		get
		{
			return tradeValue;
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

	public List<int [] []> StatSplits
	{
		get
		{
			return statSplits;
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

		set
		{
			if (value > injuryLength)
				injuryLength = value;
		}
	}

	public int Skin
	{
		get
		{
			return skin;
		}
	}

	public int Freckles
	{
		get
		{
			return freckles;
		}
	}

	public int Ear
	{
		get
		{
			return ear;
		}
	}

	public int Face
	{
		get
		{
			return face;
		}
	}

	public int EyeShape
	{
		get
		{
			return eyeShape;
		}
	}

	public int EyeColour
	{
		get
		{
			return eyeColour;
		}
	}

	public int Hair
	{
		get
		{
			return hair;
		}
	}

	public int Mouth
	{
		get
		{
			return mouth;
		}
	}

	public int Nose
	{
		get
		{
			return nose;
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

	public float Personality
	{
		get
		{
			return personality;
		}
	}

	public float Popularity
	{
		get
		{
			return popularity;
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

	public bool ClearedWaivers 
	{
		get
		{
			return clearedWaivers;
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

	public int BatSplit
	{
		get
		{
			return batSplit;
		}
	}

	public int PitchSplit
	{
		get
		{
			return pitchSplit;
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
			if (injuryLength > 0)
				return injuredFieldingChance;
			else
				return fieldingChance;
		}
	}

	public float CatchingChance
	{
		get
		{
			if (injuryLength > 0)
				return injuredCatchingChance;
			else
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

	public string InningsPitched
	{
		get
		{
			if (stats [0] [20] != 0)
				return (stats [0] [20] / 3).ToString () + "." + (stats [0] [20] % 3).ToString ();
			else
				return "0.0";
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