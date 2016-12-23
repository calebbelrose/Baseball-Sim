using UnityEngine;
using System.Collections;
using System.IO;

public class Player
{
	public int games, atBats, runs, hits, singles, doubles, triples, homeruns, totalBases, runsBattedIn, walks, strikeouts, stolenBases, caughtStealing, sacrifices, wins, losses, gamesStarted, saves, saveOpportunities, inningsPitched, atBatsAgainst, hitsAgainst, runsAgainst, earnedRuns, homerunsAgainst, walksAgainst, strikeoutsAgainst, qualityStarts, completeGames, hitStreak, reachedOnError, hitByPitch;
	public int potential, age, contractLength;
	public int[] skills = new int[10]; // 0power, 1contact, 2eye, 3speed, 4catching, 5throwing power, 6accuracy, 7movement, 8energy, 9endurance
	public float offense, defense, overall;
	public double salary;
	public string firstName, lastName, position;
	static string[] firstNames = File.ReadAllLines("FirstNames.txt"), lastNames = File.ReadAllLines("LastNames.txt");
	public int playerNumber, team, injuryLength;
	public static int longestFirstName = 9, longestLastName = 10;
	public string injuryLocation, injurySeriousness;
	public int[,,] runExpectancy = new int[2,3,8];

	public Player()
	{
	}

	public Player(string newPosition)
	{
		firstName = firstNames [(int)(Random.value * firstNames.Length)];
		lastName = lastNames [(int)(Random.value * lastNames.Length)];
		position = newPosition;
		age = (int)(Random.value * 27) + 18;
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

		if (firstName.Length > longestFirstName)
			longestFirstName = firstName.Length;

		if (lastName.Length > longestLastName)
			longestLastName = lastName.Length;

		Reset ();
	}

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
	}

	public void SavePlayer(int teamNum, int playerNum)
	{
		PlayerPrefs.SetString ("Player" + teamNum + "-" + playerNum, firstName + "," + lastName + "," + position + "," + potential + "," + age + "," + potential + "," + skills [0] + "," + skills [1] + "," + skills [2] + "," + skills [3] + "," + skills [4] + "," + skills [5] + "," + skills [6] + "," + skills [7] + "," + skills [8] + "," + skills [9] + "," + offense + "," + defense + "," + overall + "," + salary + "," + contractLength + "," + injuryLength);
		SaveStats (teamNum, playerNum);
	}

	public void SaveStats(int teamNum, int playerNum)
	{
		PlayerPrefs.SetString ("PlayerStats" + teamNum + "-" + playerNum, games + "," + atBats + "," + runs + "," + hits + "," + doubles + "," + triples + "," + homeruns + "," + totalBases + "," + runsBattedIn + "," + walks + "," + strikeouts + "," + stolenBases + "," + caughtStealing + "," + sacrifices + "," + wins + "," + losses + "," + gamesStarted + "," + saves + "," + saveOpportunities + "," + inningsPitched + "," + atBatsAgainst + "," + hitsAgainst + "," + runsAgainst + "," + earnedRuns + "," + homerunsAgainst + "," + walksAgainst + "," + strikeoutsAgainst + "," + qualityStarts + "," + completeGames + "," + hitStreak + "," + reachedOnError + "," + hitByPitch); 
	}

	public void LoadPlayer(int teamNum, int playerNum)
	{
		string player = PlayerPrefs.GetString("Player" + teamNum + "-" + playerNum), stats = PlayerPrefs.GetString("PlayerStats" + teamNum + "-" + playerNum);
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

		playerNumber = playerNum;
		team = teamNum;
	}

	public void Injure()
	{
		float r1 = Random.value * 10 + 1, r2 = Random.value * 10 + 1, r3 = Random.value * 5 + 1;
		int seriousness = (int)(r1 * r2), location = (int)r3;
		AllTeams allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();


		if (seriousness == 25 && location == 5)
		{
			allTeams.teams [team].players.RemoveAt (playerNumber);
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

	public float BBToK()
	{
		if (strikeouts != 0)
			return (float)walks / strikeouts;
		else if (walks != 0)
			return 999.99f;
		else
			return 0.0f;
	}

	public float BAA()
	{
		if (atBatsAgainst != 0)
			return (float)hitsAgainst / atBatsAgainst;
		else
			return 1.0f;
	}

	public float ISO()
	{
		if (atBats != 0)
			return (float)(doubles + triples * 2 + homeruns * 3) / atBats;
		else
			return 0.0f;
	}

	public float BA()
	{
		if (atBats != 0)
			return (float)hits / atBats;
		else
			return 0.0f;
	}

	public double LWR()
	{
		if (position.Length == 1 || (position.Length == 2 && position.Substring (1) != "P"))
			return 0.47 * singles + 0.77 * doubles + 1.05 * triples + 1.40 * homeruns + 0.50 * reachedOnError + 0.31 * walks + 0.34 * hitByPitch - 0.27 * (atBats - hits - reachedOnError - strikeouts + sacrifices) - 0.29 * strikeouts;
		else
			return 0.0;
	}

	public float PSN()
	{
		int denominator = homeruns + stolenBases;
		if (denominator != 0)
			return (float)(2 * homeruns * stolenBases) / denominator;
		else
			return 0.0f;
	}

	public float WHIP()
	{
		if (inningsPitched != 0)
			return (walksAgainst + hitsAgainst) / (inningsPitched / 3.0f);
		else
			return 999.99f;
	}

	public float RC27()
	{
		int denominator = atBats + walks;
		if(denominator != 0)
			return (hits + walks - caughtStealing) * (totalBases + (0.55f * stolenBases)) / (float)(atBats + walks) / 27;
		else
			return 0.0f;
	}
}
