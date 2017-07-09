using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Team
{
	public List<int> players;
	public int Pick;
	public List<int> SP, RP, CP, OffensiveSubstitutes, DefensiveSubstitutes;
	public List<string> Positions;
	public List<List<int>> Batters;

	public League League;
	public string Shortform, CityName, TeamName;
    
    
	public int id, streak = 0, stadiumCapacity = 50000, stadiumTier = 0;

	private Division division;
	private float[] overalls;
	private int wins, losses;
	private double currentSalary;

	public static int longestCityName = 0, longestTeamName = 0;

	private double cash = 25000000.00, revenues = 0, expenses = 0, profit = 0, ticketPrice = 100.00, drinkPrice = 10.00, foodPrice = 10.00, uniformPrice = 100.00, newStadiumPrice = 5000000.00;
	private double hype = 0.5;
	private int drinksSold = 0, foodSold = 0, uniformsSold = 0, ticketsSold = 0;
	private int currStarter = 0;
	private List<int> draftPicks = new List<int> ();
	private List<int> majorLeagueIndexes, minorLeagueIndexes, FortyManRoster, waivers;
	private string[] stats;

	static string[] cityNames = File.ReadAllLines("CityNames.txt"), teamNames = File.ReadAllLines("TeamNames.txt");
	static League sLeague = League.American;
	static Division sDivision = Division.East;

	public Division Division
	{
		get
		{
			return division;
		}
	}

	public float[] Overalls
	{
		get
		{
			return overalls;
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
			if(value >= 0)
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
			if(value >= 0)
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
			if(value >= 0)
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
			if(value >= 0)
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

	private static int rosterSize = 25;

	public static int RosterSize
	{
		get
		{
			return rosterSize;
		}
	}

	public int CurrStarter
	{
		get
		{
			return currStarter;
		}
	}

    public Team()
    {
        Reset();
    }

	// Resets team
    void Reset()
    {
		players = new List<int>();
		majorLeagueIndexes = new List<int> ();
		minorLeagueIndexes = new List<int> ();
        SP = new List<int> ();
        RP = new List<int> ();
        CP = new List<int> ();
        Batters = new List<List<int>> ();
		Positions = new List<string> ();
		OffensiveSubstitutes = new List<int> ();
		DefensiveSubstitutes = new List<int> ();
		FortyManRoster = new List<int> ();
		wins = 0;
		losses = 0;
        overalls = new float[3];
        stats = new string[3];

		CityName = cityNames[(int)(Random.value * cityNames.Length)];
		TeamName = teamNames[(int)(Random.value * teamNames.Length)];

		if (CityName.Length > longestCityName)
			longestCityName = CityName.Length;
		
		if (TeamName.Length > longestTeamName)
			longestTeamName = TeamName.Length;
		
		League = sLeague;
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
    }
    
	// Sets the stats for the team
    public void SetStats()
    {
        stats [0] = CityName + " " + TeamName;
		stats [1] = wins.ToString ();;
		stats [2] = losses.ToString ();;
    }

	// Gets the stats for the team
    public string[] GetStats()
    {
        return stats;
    }

	// Orders the starting rotation and batting order
	public void OrderLineup()
	{
		Batters = Batters.OrderByDescending (playerX => Manager.Instance.Players[playerX[0]].offense).ToList();
		SP = SP.OrderByDescending (playerX => Manager.Instance.Players[playerX].skills[5]).ToList();
		RP = RP.OrderByDescending (playerX => Manager.Instance.Players[playerX].skills[5]).ToList();
		OffensiveSubstitutes = OffensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players[playerX].offense).ToList();
		DefensiveSubstitutes = DefensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players[playerX].defense).ToList();
		for (int i = 0; i < Batters.Count; i++)
			Positions.Add (Manager.Instance.Players [Batters [i][0]].position);

		if (Positions.Count < 9)
			Positions.Add ("P");
	}

	public void SaveLineup()
	{
		for(int i = 0; i < SP.Count; i++)
			PlayerPrefs.SetInt ("SP" + id + "-" + i, SP[i]);

		for(int i = 0; i < RP.Count; i++)
			PlayerPrefs.SetInt ("RP" + id + "-" + i, RP[i]);

		for(int i = 0; i < CP.Count; i++)
			PlayerPrefs.SetInt ("CP" + id + "-" + i, CP[i]);

		for(int i = 0; i < Batters.Count; i++)
			PlayerPrefs.SetInt ("Batter" + id + "-" + i, Batters[i][0]);

		for(int i = 0; i < Positions.Count; i++)
			PlayerPrefs.SetString ("Position" + id + "-" + i, Positions[i]);

		for (int i = 0; i < OffensiveSubstitutes.Count; i++)
		{
			PlayerPrefs.SetInt ("OffensiveSub" + id + "-" + i, OffensiveSubstitutes [i]);
			PlayerPrefs.SetInt ("DefensiveSub" + id + "-" + i, DefensiveSubstitutes [i]);
		}
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
		PlayerPrefs.SetString ("WLH" + id.ToString (), wins + "," + losses + "," + hype);
		PlayerPrefs.Save ();
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
		PlayerPrefs.SetString ("WLH" + id.ToString (), wins + "," + losses + "," + hype);
		PlayerPrefs.Save ();
	}

	// Adds revenue
	public void AddRevenue(double otherHype)
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

	// Subtract expenses
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

		if(League == League.American)
			starters.Add("DH");

		benchSpace = Team.RosterSize - starters.Count;

		Batters.Clear ();
		Positions.Clear ();
		SP.Clear ();
		RP.Clear ();
		CP.Clear ();
		OffensiveSubstitutes.Clear ();
		DefensiveSubstitutes.Clear ();

		List<int> result = players.OrderBy(playerX => Manager.Instance.Players[playerX].position).ThenByDescending(playerX => Manager.Instance.Players[playerX].overall).ToList<int>();

		for(int j = 0; j < result.Count; j++)
		{
			if (starters.Contains (Manager.Instance.Players[result [j]].position))
			{
				if (Manager.Instance.Players[result [j]].position == "SP")
				{
					SP.Add (Manager.Instance.Players[result [j]].ID);
					starters.Remove ("SP");
				}
					else if (Manager.Instance.Players[result [j]].position == "RP")
				{
					RP.Add (Manager.Instance.Players[result [j]].ID);
					starters.Remove ("RP");
				}
						else if (Manager.Instance.Players[result [j]].position == "CP")
				{

					CP.Add (Manager.Instance.Players[result [j]].ID);
					starters.Remove ("CP");
				}
				else
				{
					List<int> batterSlot = new List<int> ();
					batterSlot.Add (Manager.Instance.Players[result [j]].ID);
					Batters.Add (batterSlot);
					starters.Remove (Manager.Instance.Players[result [j]].position);
				}
				AddToMajors(Manager.Instance.Players[result [j]].ID);
			}
			else if (benchSpace > 0)
			{
				AddToMajors(Manager.Instance.Players[result [j]].ID);
				OffensiveSubstitutes.Add (Manager.Instance.Players[result [j]].ID);
				DefensiveSubstitutes.Add (Manager.Instance.Players[result [j]].ID);
				benchSpace--;
			}
			else
				AddToMinors (Manager.Instance.Players[result [j]].ID);
		}

		PlayerPrefs.Save ();

		OrderLineup ();
		CalculateOveralls ();
		return starters;
	}

	public void CalculateOveralls()
	{
		float totalOverall = 0.0f, totalOffense = 0.0f, totalDefense = 0.0f;

		for (int i = 0; i < majorLeagueIndexes.Count; i++)
		{
			totalOverall += Manager.Instance.Players[majorLeagueIndexes [i]].overall;
			totalOffense += Manager.Instance.Players[majorLeagueIndexes [i]].offense;
			totalDefense += Manager.Instance.Players[majorLeagueIndexes [i]].defense;
		}

		overalls [0] = totalOverall / majorLeagueIndexes.Count;
		overalls [1] = totalOffense / majorLeagueIndexes.Count;
		overalls [2] = totalDefense / majorLeagueIndexes.Count;

		PlayerPrefs.SetString("Overalls" + id, overalls[0] + "," + overalls[1] + "," + overalls[2]);
	}

	public void AddToMajors(int index)
	{
		if (FortyManRoster.Contains(index) || FortyManRoster.Count < 40)
		{
			int startIndex = minorLeagueIndexes.IndexOf (index);

			if (startIndex != -1)
			{
				minorLeagueIndexes.Remove (index);

				for (int i = startIndex; i < minorLeagueIndexes.Count; i++)
					PlayerPrefs.SetInt ("Minors" + id + "-" + i, minorLeagueIndexes [i]);

				PlayerPrefs.DeleteKey ("Minors" + id + "-" + minorLeagueIndexes.Count);
			}
			PlayerPrefs.SetInt ("Majors" + id + "-" + majorLeagueIndexes.Count, index);

			majorLeagueIndexes.Add (index);
		}
	}

	public void AddToMinors(int index)
	{
		int startIndex = majorLeagueIndexes.IndexOf(index);

		if (startIndex != -1)
		{
			majorLeagueIndexes.Remove (index);

			for (int i = startIndex; i < majorLeagueIndexes.Count; i++)
				PlayerPrefs.SetInt ("Majors" + id + "-" + i, majorLeagueIndexes [i]);

			PlayerPrefs.DeleteKey ("Majors" + id + "-" + majorLeagueIndexes.Count);
		}
		PlayerPrefs.SetInt("Minors" + id + "-" + minorLeagueIndexes.Count, index);

		minorLeagueIndexes.Add (index);
	}

	public void UseStarter()
	{
		currStarter = (currStarter + 1) % 5;
		PlayerPrefs.SetInt ("CurrStarter" + id, currStarter);
	}

	public static void ChangeRosterSize(int size)
	{
		rosterSize = size;
	}

	public void DraftPick(int playerID)
	{
		draftPicks.Add (playerID);
	}

	public void LoadTeam(int _id)
	{
		string teamInfo = PlayerPrefs.GetString("Team" + id);
		string teamOveralls = PlayerPrefs.GetString("Overalls" + id);
		string[] teamInfoSplit = teamInfo.Split(',');
		string[] teamOverallsSplit = teamOveralls.Split(',');
		string[] wlh;
		string[] splitName;
		int index = 0;

		while (PlayerPrefs.HasKey ("TeamPlayers" + id + "-" + index))
			players.Add (PlayerPrefs.GetInt ("TeamPlayers" + id + "-" + index++));

		index = 0;

		CityName = teamInfoSplit[0];
		TeamName = teamInfoSplit[1];
		Pick = int.Parse(teamInfoSplit[2]);
		overalls[0] = float.Parse(teamOverallsSplit[0]);
		overalls[1] = float.Parse(teamOverallsSplit[1]);
		overalls[2] = float.Parse(teamOverallsSplit[2]);
		splitName = (CityName + " " + TeamName).Split(' ');

		for (int i = 0; i < splitName.Length; i++)
			if (System.Char.IsLetter(splitName[i][0]) && System.Char.IsUpper(splitName[i][0]))
				Shortform += splitName[i][0];

		wlh = PlayerPrefs.GetString("WLH" + id).Split(',');
		wins = int.Parse(wlh [0]);
		losses = int.Parse(wlh [1]);
		hype = double.Parse(wlh [2]);

		for(int i = 0; i < 5; i++)
			SP.Add(PlayerPrefs.GetInt("SP" + id + "-" + i));

		for (int i = 0; i < 9; i++)
		{
			List<int> batterSlot = new List<int> ();

			batterSlot.Add (PlayerPrefs.GetInt ("Batter" + id + "-" + i));
			Batters.Add (batterSlot);
		}

		for (int i = 0; i < 9; i++)
			Positions.Add(PlayerPrefs.GetString("Position" + id + "-" + i));

		for (int i = 0; i < 3; i++)
			RP.Add(PlayerPrefs.GetInt("RP" + id + "-" + i));

		for (int i = 0; i < 1; i++)
			CP.Add(PlayerPrefs.GetInt("CP" + id + "-" + i));

		while (PlayerPrefs.HasKey ("Majors" + id + "-" + index))
			AddToMajors (PlayerPrefs.GetInt ("Majors" + id + "-" + index++));

		index = 0;

		while (PlayerPrefs.HasKey ("Minors" + id + "-" + index))
			AddToMinors (PlayerPrefs.GetInt ("Minors" + id + "-" + index++));

		index = 0;

		while (PlayerPrefs.HasKey ("OffensiveSub" + id + "-" + index))
		{
			OffensiveSubstitutes.Add (PlayerPrefs.GetInt ("OffensiveSub" + id + "-" + index));
			DefensiveSubstitutes.Add (PlayerPrefs.GetInt ("DefensiveSub" + id + "-" + index++));
		}
	}

	public bool IsBatter(int id)
	{
		int index = 0;
		bool notBatter = true;

		while (notBatter && index < Batters.Count)
			if (Batters [index++].Contains (id))
				notBatter = false;

		return !notBatter;
	}

	public void PutOnWaivers(int id)
	{
		waivers.Add (id);
		Manager.Instance.waivers.Add (new WaiverPlayer (id));
	}

	public void TakeOffWaivers(int id)
	{
		waivers.Remove (id);
		Manager.Instance.RemoveFromWaivers (id);
	}

	public int GetWorstOverall()
	{
		int[] temp = FortyManRoster.OrderBy (playerX => Manager.Instance.Players [playerX].overall).ToArray();

		return temp [temp.Length - 1];
	}

	public void TransferPlayer(int id)
	{
		players.Remove (id);
	}

	public void SetExpenses()
	{
		currentSalary = 0;

		for (int i = 0; i < FortyManRoster.Count; i++)
			currentSalary += Manager.Instance.Players [FortyManRoster [i]].Salary;
	}

	public void ResetWins()
	{
		wins = 0;
	}

	public void ResetLosses()
	{
		wins = 0;
	}

	public string Transfer (int playerIndex, int teamID)
	{
		Manager.Instance.teams [teamID].players.Add (Manager.Instance.teams [0].players [playerIndex]);
		Manager.Instance.teams [teamID].currentSalary += Manager.Instance.Players[Manager.Instance.teams [0].players [playerIndex]].Salary;
		currentSalary -= Manager.Instance.Players[Manager.Instance.teams [0].players [playerIndex]].Salary;
		players.RemoveAt (playerIndex);
		return Manager.Instance.Players[players[playerIndex]].firstName + " " + Manager.Instance.Players[Manager.Instance.teams[0].players[playerIndex]].lastName + ", ";
	}

	public void RemovePlayer (int playerID)
	{
		
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