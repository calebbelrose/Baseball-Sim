using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Team
{
	public string Shortform, CityName, TeamName;
	public int Pick;

	private int id, stadiumCapacity = 50000, stadiumTier = 0;
	private Division division;
	private League league;
	private float [] overalls;
	private double cash, revenues = 0, expenses = 0, profit = 0, ticketPrice = 100.00, drinkPrice = 10.00, foodPrice = 10.00, uniformPrice = 100.00, newStadiumPrice = 5000000.00, hype = 0.5, currentSalary;
	private int drinksSold = 0, foodSold = 0, uniformsSold = 0, ticketsSold = 0, currStarter = 0, wins, losses, streak = 0;
	private List<int> majorLeagueIndexes, minorLeagueIndexes, FortyManRoster, waivers, players, sp, rp, cp, offensiveSubstitutes, defensiveSubstitutes, draftPicks = new List<int> ();
	private string [] stats;
	private List<string> positions;
	private List<List<int>> batters;
	private TeamType teamType;

	public static int longestCityName = 0, longestTeamName = 0;

	private static string [] cityNames = File.ReadAllLines("CityNames.txt"), teamNames = File.ReadAllLines("TeamNames.txt");
	private static League sLeague = League.American;
	private static Division sDivision = Division.East;
	private static int rosterSize = 25;

	public Team(TeamType _teamType, int _id)
	{
		Reset ();
		id = _id;
		teamType = _teamType;
	}

	// Resets team
	void Reset()
	{
		players = new List<int> ();
		majorLeagueIndexes = new List<int> ();
		minorLeagueIndexes = new List<int> ();
		sp = new List<int> ();
		rp = new List<int> ();
		cp = new List<int> ();
		batters = new List<List<int>> ();
		positions = new List<string> ();
		offensiveSubstitutes = new List<int> ();
		defensiveSubstitutes = new List<int> ();
		FortyManRoster = new List<int> ();
		wins = 0;
		losses = 0;
		overalls = new float [3];
		stats = new string [3];

		CityName = cityNames [(int)(Random.value * cityNames.Length)];
		TeamName = teamNames [(int)(Random.value * teamNames.Length)];

		if (CityName.Length > longestCityName)
			longestCityName = CityName.Length;
		
		if (TeamName.Length > longestTeamName)
			longestTeamName = TeamName.Length;
		
		league = sLeague;
		division = sDivision;

		if (sLeague == League.American)
			sLeague = League.National;
		else
			sLeague = League.American;

		if (sDivision == Division.East)
			sDivision = Division.West;
		else if (sDivision == Division.West)
			sDivision = Division.Central;
		else
			sDivision = Division.East;

		cash = 20000000.00 + System.Math.Round (Random.value * 10000000, 2);
	}

	// Sets the stats for the team
	public void SetStats()
	{
		stats [0] = CityName + " " + TeamName;
		stats [1] = wins.ToString ();
		stats [2] = losses.ToString ();
	}

	// Gets the stats for the team
	public string [] GetStats()
	{
		return stats;
	}

	// Sets the order for the starting rotation and batting order
	public void OrderLineup()
	{
		batters = batters.OrderByDescending (playerX => Manager.Instance.Players [playerX[0]].Offense).ToList();
		sp = sp.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [5]).ToList();
		rp = rp.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [5]).ToList();
		offensiveSubstitutes = offensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players [playerX].Offense).ToList();
		defensiveSubstitutes = defensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players [playerX].Defense).ToList();
	}

	// Saves the team's Wins, Losses, Hype and Cash
	public void SaveWLHC()
	{
		StreamWriter sw = new StreamWriter (@"Save\WLHC" + (int)teamType + "-" + id + ".txt");

		sw.Write (wins + "," + losses + "," + hype + "," + cash);
		sw.Close ();
	}

	// Saves the team's overall skill
	public void SaveOveralls()
	{
		StreamWriter sw = new StreamWriter (@"Save\Overalls" + id + ".txt");

		sw.Write (overalls [0] + "," + overalls [1] + "," + overalls [2]);
		sw.Close ();
	}

	// Adds a win
	public void Win()
	{
		if (streak < 0)
			streak = 1;
		else
			streak++;
		
		hype += System.Math.Pow (1.005, streak) - 1.0;

		if (hype > 1)
			hype = 1;
		
		wins++;

		SaveWLHC ();
	}

	// Adds a loss
	public void Loss()
	{
		if (streak > 0)
			streak = -1;
		else
			streak--;
		hype -= System.Math.Pow (1.005, -streak) - 1.0;

		if (hype < 0)
			hype = 0;
		
		losses++;

		SaveWLHC ();
	}

	// Adds revenue
	public void AddRevenue (double otherHype)
	{
		double thisHype = (hype * 2 + otherHype) / 3, thisRevenue;

		if (ticketPrice == 0)
			ticketsSold= stadiumCapacity;
		else
			ticketsSold = (int)System.Math.Round(50000 * 100 / ticketPrice * thisHype);
		
		if (ticketsSold > stadiumCapacity)
			ticketsSold = stadiumCapacity;

		if (foodPrice == 0)
			foodSold = ticketsSold * 2;
		else
			foodSold = (int)System.Math.Round(ticketsSold * (Random.value / 4 + Random.value / 4 + Random.value / 4 + 0.25) / (foodPrice / 10 * (0.75 + thisHype / 2)));

		if (drinkPrice == 0)
			drinksSold = ticketsSold * 2 + foodSold;
		else
			drinksSold = (int)System.Math.Round((ticketsSold * (Random.value / 4 + Random.value / 4 + Random.value / 4 + 0.25) + foodSold) / (drinkPrice / 10 * (0.75 + thisHype / 2)));

		uniformsSold += (int)System.Math.Round (ticketsSold * hype * 100 / uniformPrice);

		thisRevenue = ticketsSold * ticketPrice + foodSold * foodPrice + drinksSold * drinkPrice + uniformsSold * uniformPrice;

		revenues += thisRevenue;
		profit += thisRevenue;
		cash += thisRevenue;

		SubtractExpenses ();
	}

	// Subtracts expenses
	public void SubtractExpenses()
	{
		double thisExpense = (10000000 + currentSalary) / 162 + ticketsSold * 0.25 + foodSold + drinksSold + uniformsSold * 10;



		expenses += thisExpense;
		profit -= thisExpense;
		cash -= thisExpense;

		ticketsSold = 0;
		foodSold = 0;
		drinksSold = 0;
		uniformsSold = 0;
	}

	// Buys a stadium
	public void BuyStadium(int tier)
	{
		stadiumCapacity = (int)(stadiumCapacity * 1.05);
		cash -= newStadiumPrice;
		newStadiumPrice *= 1.25;
		stadiumTier++;
	}

	// Rents a stadium
	public void RentStadium(int tier)
	{
		double price = 250000 * System.Math.Pow (1.25, tier);

		cash -= price;
		stadiumCapacity = (int)(50000 * System.Math.Pow (1.05, tier));
		stadiumTier = tier;

	}

	// Automatically sets the roster
	public List<string> AutomaticRoster()
	{
		List<string> starters = new List<string>();
		int benchSpace;

		majorLeagueIndexes.Clear ();
		minorLeagueIndexes.Clear ();

		starters.Add("SP");
		starters.Add("SP");
		starters.Add("SP");
		starters.Add("SP");
		starters.Add("SP");
		starters.Add("RP");
		starters.Add("RP");
		starters.Add("RP");
		starters.Add("CP");
		starters.Add("C");
		starters.Add("1B");
		starters.Add("2B");
		starters.Add("3B");
		starters.Add("SS");
		starters.Add("LF");
		starters.Add("CF");
		starters.Add("RF");
		starters.Add("DH");

		benchSpace = Team.RosterSize - starters.Count;

		batters.Clear ();
		positions.Clear ();
		sp.Clear ();
		rp.Clear ();
		cp.Clear ();
		offensiveSubstitutes.Clear ();
		defensiveSubstitutes.Clear ();

		List<int> result = players.OrderBy(playerX => Manager.Instance.Players [playerX].InjuryLength).ThenByDescending (playerX => Manager.Instance.Players [playerX].Position).ThenByDescending (playerX => Manager.Instance.Players [playerX].Overall).ToList<int>();

		for(int j = 0; j < result.Count; j++)
		{
			if (starters.Contains (Manager.Instance.Players [result [j]].Position) && AddToMajors(Manager.Instance.Players [result [j]].ID))
			{
				if (Manager.Instance.Players [result [j]].Position == "SP")
				{
					sp.Add (Manager.Instance.Players [result [j]].ID);
					starters.Remove ("SP");
				}
					else if (Manager.Instance.Players [result [j]].Position == "RP")
				{
					rp.Add (Manager.Instance.Players [result [j]].ID);
					starters.Remove ("RP");
				}
						else if (Manager.Instance.Players [result [j]].Position == "CP")
				{

					cp.Add (Manager.Instance.Players [result [j]].ID);
					starters.Remove ("CP");
				}
				else
				{
					List<int> batterSlot = new List<int> ();
					batterSlot.Add (Manager.Instance.Players [result [j]].ID);
					batters.Add (batterSlot);
					positions.Add (Manager.Instance.Players [result [j]].Position);
					starters.Remove (Manager.Instance.Players [result [j]].Position);
				}
			}
			else if (benchSpace > 0 && AddToMajors(Manager.Instance.Players [result [j]].ID))
			{
				offensiveSubstitutes.Add (Manager.Instance.Players [result [j]].ID);
				defensiveSubstitutes.Add (Manager.Instance.Players [result [j]].ID);
				benchSpace--;
			}
			else
				AddToMinors (Manager.Instance.Players [result [j]].ID);
		}

		OrderLineup ();
		CalculateOveralls ();
		return starters;
	}

	// Adds a new year to the team
	public void NewYear()
	{
		ResetWins ();
		ResetLosses ();
		SaveWLHC ();

		for (int i = 0; i < players.Count; i++)
		{
			Manager.Instance.Players [players [i]].NewYear ();
			Manager.Instance.Players [players [i]].CalculatePotential ();
		}
	}

	// Calculates the overall skill for the team
	public void CalculateOveralls()
	{
		float totalOverall = 0.0f, totalOffense = 0.0f, totalDefense = 0.0f;

		for (int i = 0; i < majorLeagueIndexes.Count; i++)
		{
			totalOverall += Manager.Instance.Players [majorLeagueIndexes [i]].Overall;
			totalOffense += Manager.Instance.Players [majorLeagueIndexes [i]].Offense;
			totalDefense += Manager.Instance.Players [majorLeagueIndexes [i]].Defense;
		}

		overalls [0] = totalOverall / majorLeagueIndexes.Count;
		overalls [1] = totalOffense / majorLeagueIndexes.Count;
		overalls [2] = totalDefense / majorLeagueIndexes.Count;

		SaveOveralls ();
	}

	// Adds a player to the major leagues
	public bool AddToMajors(int index)
	{
		if (teamType == TeamType.MLB)
		{
			if (IsInFortyManRoster (index))
			{
				int startIndex = minorLeagueIndexes.IndexOf (index);

				if (startIndex != -1)
					minorLeagueIndexes.Remove (index);

				majorLeagueIndexes.Add (index);
				return true;
			}
			else
				return false;
		}
		else
			return true;
	}

	// Determines whether a player is in the forty man roster
	public bool IsInFortyManRoster(int index)
	{
		if (FortyManRoster.Contains (index))
			return true;
		else if (FortyManRoster.Count < 40)
		{
			FortyManRoster.Add (index);
			return true;
		}
		else
			return false;
	}

	// Adds a player to the minor leagues
	public void AddToMinors(int index)
	{
		int startIndex = majorLeagueIndexes.IndexOf (index);

		if (startIndex != -1)
			majorLeagueIndexes.Remove (index);

		minorLeagueIndexes.Add (index);
	}

	// Uses the current starter
	public void UseStarter()
	{
		currStarter = (currStarter + 1) % 5;
		PlayerPrefs.SetInt ("CurrStarter" + id, currStarter);
	}

	// Adds a player to the list of draft picks
	public void DraftPick (int playerID)
	{
		draftPicks.Add (playerID);
	}

	// Saves the team's information
	public void Save ()
	{
		StreamWriter sw = new StreamWriter (@"Save\Team" + (int)teamType + "-" + id + ".txt");

		sw.Write(CityName + "," + TeamName + "," + Pick + "," + cash);
		sw.Close ();
	}

	// Loads the team's information
	public void LoadTeamInfo()
	{
		string [] teamInfoSplit = File.ReadAllLines(@"Save\Team" + (int)teamType + "-" + id + ".txt") [0].Split(',');

		CityName = teamInfoSplit [0];
		TeamName = teamInfoSplit [1];
		Pick = int.Parse (teamInfoSplit [2]);
		cash = double.Parse (teamInfoSplit [3]);
	}

	// Loads the team
	public void LoadTeam()
	{
		string [] split = File.ReadAllLines (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		LoadTeamInfo ();

		for (int i = 1; i < split.Length; i++)
			players.Add (int.Parse(split [i]));

		split = File.ReadAllLines (@"Save\Overalls" + id + ".txt") [0].Split(',');

		overalls [0] = float.Parse (split [0]);
		overalls [1] = float.Parse (split [1]);
		overalls [2] = float.Parse (split [2]);
		split = (CityName + " " + TeamName).Split(' ');

		for (int i = 0; i < split.Length; i++)
			if (System.Char.IsLetter(split[i] [0]) && System.Char.IsUpper(split [i] [0]))
				Shortform += split [i] [0];

		split = File.ReadAllLines(@"Save\WLHC" + (int)teamType + "-" + id + ".txt") [0].Split(',');

		wins = int.Parse (split [0]);
		losses = int.Parse (split [1]);
		hype = double.Parse (split [2]);
		cash = double.Parse (split [3]);
	}

	// Determines whether a player is currently in the batting order
	public bool IsBatter(int id)
	{
		int index = 0;
		bool notBatter = true;

		while (notBatter && index < batters.Count)
			if (batters [index++].Contains (id))
				notBatter = false;

		return !notBatter;
	}

	// Signed a draft player to the team
	public void SignDraftPlayer(int id)
	{
		AddPlayer (id);
		minorLeagueIndexes.Add (id);
		Manager.Instance.Players [id].ContractYears.Add (new ContractYear(ContractType.MinorLeague, 0.0));
		cash -= Manager.Instance.Players [id].Offer;
	}

	// Adds a player to the team
	public void AddPlayer(int playerID)
	{
		StreamWriter sw = File.AppendText (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		players.Add (playerID);
		sw.Write ("\n" + playerID);
		sw.Close ();
	}

	// Removes a player from the team
	public void RemovePlayer(int playerID)
	{
		players.RemoveAt (players.IndexOf (playerID));
		SavePlayers ();
	}

	// Saves the indexes of the team's players
	void SavePlayers()
	{
		StreamWriter sw = new StreamWriter (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		for (int i = 0; i < players.Count; i++)
			sw.Write ("\n" + players [i]);

		sw.Close ();
	}

	// Puts a player on waivers
	public void PutOnWaivers(int id)
	{
		waivers.Add (id);
		Manager.Instance.PutOnWaivers (id);
		Manager.Instance.Players [id].PutOnWaivers ();
	}

	// Takes a player off waivers
	public void TakeOffWaivers(int playerID)
	{
		waivers.Remove (playerID);
		Manager.Instance.TakeOffWaivers (playerID);
		Manager.Instance.Players [playerID].TakeOffWaivers ();
	}

	// Gets the index of player with the worst overall
	public int GetWorstOverall ()
	{
		int [] temp = FortyManRoster.OrderBy (playerX => Manager.Instance.Players [playerX].Overall).ToArray();

		return temp [temp.Length - 1];
	}

	// Sets the team's expenses
	public void SetExpenses()
	{
		currentSalary = 0;

		for (int i = 0; i < FortyManRoster.Count; i++)
			currentSalary += Manager.Instance.Players [FortyManRoster [i]].ContractYears [0].Salary;
	}

	// Resets the team's wins
	public void ResetWins()
	{
		wins = 0;
	}

	// Resets the team's losses
	public void ResetLosses()
	{
		wins = 0;
	}

	// Transfers a player to another team
	public string Transfer (int playerIndex, int teamID)
	{
		Manager.Instance.Teams [0] [teamID].AddPlayer (Manager.Instance.Teams [0] [0].Players [playerIndex]);
		Manager.Instance.Teams [0] [teamID].currentSalary += Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [playerIndex]].ContractYears [0].Salary;
		currentSalary -= Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [playerIndex]].ContractYears [0].Salary;
		RemovePlayer (playerIndex);
		return Manager.Instance.Players [players [playerIndex]].FirstName + " " + Manager.Instance.Players [Manager.Instance.Teams [0] [0].Players [playerIndex]].LastName + ", ";
	}

	// Bankrupts team, forcing them to get a new owner or ending the game if it's the players team
	public void Bankrupt()
	{
		if (id == 0)
		{
			Manager.Instance.Restart ();
			Manager.ChangeToScene (0);
		}
		else
		{
			string [] splitName;

			CityName = cityNames [(int)(Random.value * cityNames.Length)];
			TeamName = teamNames [(int)(Random.value * teamNames.Length)];

			if (CityName.Length > longestCityName)
				longestCityName = CityName.Length;

			if (TeamName.Length > longestTeamName)
				longestTeamName = TeamName.Length;

			Shortform = "";
			splitName = (CityName + " " + TeamName).Split(' ');

			for (int i = 0; i < splitName.Length; i++)
				if (System.Char.IsLetter(splitName [i] [0]) && System.Char.IsUpper(splitName [i] [0]))
					Shortform += splitName [i] [0];

			Debug.Log ("New owner " + cash);
			cash = 20000000.00 + System.Math.Round(Random.value * 10000000, 2);
		}
	}

	// Getters and Setters
	public List<string> Positions
	{
		get
		{
			return positions;
		}
	}

	public List<List<int>> Batters
	{
		get
		{
			return batters;
		}
	}

	public List<int> DefensiveSubstitutes
	{
		get
		{
			return defensiveSubstitutes;
		}
	}

	public List<int> OffensiveSubstitutes
	{
		get
		{
			return offensiveSubstitutes;
		}
	}

	public List<int> CP
	{
		get
		{
			return cp;
		}
	}

	public List<int> RP
	{
		get
		{
			return rp;
		}
	}

	public List<int> SP
	{
		get
		{
			return sp;
		}
	}

	public List<int> Players
	{
		get
		{
			return players;
		}
	}

	public Division Division
	{
		get
		{
			return division;
		}
	}

	public League League
	{
		get
		{
			return league;
		}
	}

	public float [] Overalls
	{
		get
		{
			return overalls;
		}
	}

	public int StadiumCapacity
	{
		get
		{
			return stadiumCapacity;
		}
	}

	public int StadiumTier
	{
		get
		{
			return stadiumTier;
		}
	}

	public int ID
	{
		get
		{
			return id;
		}
	}

	public int Wins
	{
		get
		{
			return wins;
		}
	}

	public int Losses
	{
		get
		{
			return losses;
		}
	}

	public double CurrentSalary
	{
		get
		{
			return currentSalary;
		}
	}

	public double Cash
	{
		get
		{
			return cash;
		}
	}

	public double Revenues
	{
		get
		{
			return revenues;
		}
	}

	public double Expenses
	{
		get
		{
			return expenses;
		}
	}

	public double Profit
	{
		get
		{
			return profit;
		}
	}

	public double TicketPrice
	{
		get
		{
			return ticketPrice;
		}

		set
		{
			if (value >= 0)
				ticketPrice = value;
		}
	}

	public double DrinkPrice
	{
		get
		{
			return drinkPrice;
		}

		set
		{
			if (value >= 0)
				drinkPrice = value;
		}
	}

	public double FoodPrice
	{
		get
		{
			return foodPrice;
		}

		set
		{
			if (value >= 0)
				foodPrice = value;
		}
	}

	public double UniformPrice
	{
		get
		{
			return uniformPrice;
		}

		set
		{
			if (value >= 0)
				uniformPrice = value;
		}
	}

	public double NewStadiumPrice
	{
		get
		{
			return newStadiumPrice;
		}
	}

	public double Hype
	{
		get
		{
			return hype;
		}
	}

	public List<int> DraftPicks
	{
		get
		{
			return draftPicks;
		}
	}

	public List<int> MinorLeagueIndexes
	{
		get
		{
			return minorLeagueIndexes;
		}
	}

	public int CurrStarter
	{
		get
		{
			return currStarter;
		}
	}

	public TeamType Type
	{
		get
		{
			return teamType;
		}
	}

	public static int RosterSize
	{
		get
		{
			return rosterSize;
		}
	}

	// Changes the maximum roster size for all teams
	public static void ChangeRosterSize (int size)
	{
		rosterSize = size;
	}
}

public enum League
{
	American,
	National
}

public enum Division
{
	East,
	West,
	Central
}

public enum TeamType
{
	MLB = 0,
	WorldBaseballClassic = 1,
	Futures = 2,
	AllStar = 3
}