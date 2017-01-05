using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Team {

    public List<Player> players;
    public List<int> SP, RP, CP, Batters;
    public int wins, losses;
    public float[] overalls;
    public string cityName, teamName;
	public int id, pick, streak = 0, stadiumCapacity = 50000, stadiumTier = 0;
    string[] stats;
	static char sLeague = 'A', sDivision = 'E';
	public char league, division;
	public string shortform;
	public static int longestCityName = 0, longestTeamName = 0;
	static string[] cityNames = File.ReadAllLines("CityNames.txt"), teamNames = File.ReadAllLines("TeamNames.txt");
	public double currentSalary;
	public double cash = 25000000.00, hype = 0.5, revenues = 0, expenses = 0, profit = 0, ticketPrice = 100.00, drinkPrice = 10.00, foodPrice = 10.00, uniformPrice = 100.00, newStadiumPrice = 5000000.00;
	int drinksSold = 0, foodSold = 0, uniformsSold = 0, ticketsSold = 0;
	public int homeGames = 0;

    public Team()
    {
        Reset();
    }

	// Resets team
    void Reset()
    {
        players = new List<Player>();
        SP = new List<int>();
        RP = new List<int>();
        CP = new List<int>();
        Batters = new List<int>();
		wins = 0;
		losses = 0;
        overalls = new float[3];
        stats = new string[3];

		cityName = cityNames[(int)(Random.value * cityNames.Length)];
		teamName = teamNames[(int)(Random.value * teamNames.Length)];

		if (cityName.Length > longestCityName)
			longestCityName = cityName.Length;
		if (teamName.Length > longestTeamName)
			longestTeamName = teamName.Length;
		league = sLeague;
		division = sDivision;

		if (sLeague == 'A')
			sLeague = 'N';
		else
			sLeague = 'A';

		if (sDivision == 'E')
			sDivision = 'W';
		else if (sDivision == 'W')
			sDivision = 'C';
		else
			sDivision = 'E';
    }
    
	// Sets the stats for the team
    public void SetStats()
    {
        stats [0] = cityName + " " + teamName;
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
		Batters = Batters.OrderByDescending (playerX => players[playerX].offense).ToList();
		SP = SP.OrderByDescending (playerX => players[playerX].skills[5]).ToList();
		RP = RP.OrderByDescending (playerX => players[playerX].skills[5]).ToList();
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
		homeGames++;
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
}