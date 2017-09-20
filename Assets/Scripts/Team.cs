using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Team
{
	public string Shortform;													// Shortform of name
	public string CityName;														// City name
	public string TeamName;														// Team name
	public int Pick;															// Pick in first year player draft
	public bool AutomaticRoster = true;											// Whether to automatically set the roster or not
	public float GamesBehind;													// Number of games behind division leader
	public Division Division;													// Division
	public League League;														// League

	private float [] overalls;													// Overalls
	private double cash;														// Cash
	private double revenues = 0.00;												// Revenues
	private double expenses = 0.00;												// Expenses
	private double profit = 0.00;												// Profit
	private double ticketPrice = 100.00;										// Price for fan to buy ticket
	private double drinkPrice = 10.00;											// Price for fan to buy drink
	private double foodPrice = 10.00;											// Price for fan to buy food
	private double uniformPrice = 100.00;										// Price for fan to buy uniform
	private double newStadiumPrice = 5000000.00;								// Price to buy new stadium
	private double hype = 0.5;													// Hype (used to calculate attendance and therefore amount of merchandise sold)
	private double currentSalary;												// Salary of all players
	private int id;																// Team ID
	private int stadiumCapacity = 50000;										// Stadium capacity
	private int stadiumTier = 0;												// Stadium tier
	private int drinksSold = 0;													// Drinks sold
	private int foodSold = 0;													// Food sold
	private int uniformsSold = 0;												// Uniforms sold
	private int ticketsSold = 0;												// Tickets sold
	private int currStarter = 0;												// Index in SP of the current starter
	private int wins;															// Wins
	private int losses;															// Losses
	private int homeWins;														// Home Wins
	private int homeLosses;														// Home Losses
	private int awayWins;														// Away Wins
	private int awayLosses;														// Away Losses
	private int streak = 0;														// Streak
	private int cp;																// Index of closing pitcher
	private List<int> majorLeagueIndexes;										// Indexes of players in active roster
	private List<int> minorLeagueIndexes;										// Indexes of players in minor league team
	private List<int> fortyManRoster;											// Indexes of players in forty man roster
	private List<int> waivers;													// Indexes of players on waivers
	private List<int> players;													// Indexes of players on team
	private List<int> sp;														// Indexes of pitchers in starting rotation
	private List<int> rp;														// Indexes of pitchers in bullpen
	private List<int> offensiveSubstitutes;										// Indexes of offensive substitutes
	private List<int> defensiveSubstitutes;										// Indexes of defensive substitutes
	private List<int> pinchRunners;												// Indexes of pinch runners
	private List<int> draftPicks = new List<int> ();							// Indexes of players picked in the first year player draft
	private List<int> tradeBlock = new List<int> ();							// Indexes of players on trade block
	private List<bool> lastTen = new List<bool> ();								// Whether the team won each of the last 10 games
	private bool winStreak;														// Whether the team is on a winning or losing streak
	private string [] stats;													// Stats
	private List<string> positions;												// Positions of batters
	private List<string> lookingFor = new List<string> ();						// Positions to trade for
	private List<List<int>> batters;											// Batting order
	private TeamType teamType;													// Team type
	private List<TradeOffer> tradeOffers = new List<TradeOffer> ();				// Trade offers

	public static int longestCityName = 0;										// Length of the longest city name
	public static int longestTeamName = 0;										// Length of the longest team name

	private static string [] cityNames = File.ReadAllLines ("CityNames.txt");	// Possible city names
	private static string [] teamNames = File.ReadAllLines ("TeamNames.txt");	// Possible team names
	private static League sLeague = League.American;							// League
	private static Division sDivision = Division.East;							// Division
	private static int rosterSize = 25;											// Maximum roster size

	public Team (TeamType _teamType, int _id)
	{
		Reset ();
		id = _id;
		teamType = _teamType;
	}

	// Resets team
	void Reset ()
	{
		players = new List<int> ();
		majorLeagueIndexes = new List<int> ();
		minorLeagueIndexes = new List<int> ();
		sp = new List<int> ();
		rp = new List<int> ();
		batters = new List<List<int>> ();
		positions = new List<string> ();
		offensiveSubstitutes = new List<int> ();
		defensiveSubstitutes = new List<int> ();
		pinchRunners = new List<int> ();
		fortyManRoster = new List<int> ();
		wins = 0;
		losses = 0;
		overalls = new float [3];
		stats = new string [3];

		CityName = cityNames [ (int) (Manager.Instance.RandomGen.NextDouble() * cityNames.Length)];
		TeamName = teamNames [ (int) (Manager.Instance.RandomGen.NextDouble() * teamNames.Length)];

		if (CityName.Length > longestCityName)
			longestCityName = CityName.Length;
		
		if (TeamName.Length > longestTeamName)
			longestTeamName = TeamName.Length;
		
		League = sLeague;
		Division = sDivision;

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

		cash = 20000000.00 + System.Math.Round (Manager.Instance.RandomGen.NextDouble() * 10000000, 2);
	}

	// Sets the stats for the team
	public void SetStats ()
	{
		stats [0] = CityName + " " + TeamName;
		stats [1] = wins.ToString ();
		stats [2] = losses.ToString ();
	}

	// Gets the stats for the team
	public string [] GetStats ()
	{
		return stats;
	}

	// Sets the ascending for the starting rotation and batting ascending
	public void ascendingLineup ()
	{
		batters = batters.OrderByDescending (playerX => Manager.Instance.Players [playerX[0]].Offense).ToList ();
		sp = sp.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [5]).ToList ();
		rp = rp.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [5]).ToList ();
		SortSubstitutes ();		
	}

	// Sorts substitute players
	public void SortSubstitutes ()
	{
		offensiveSubstitutes = offensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players [playerX].Offense).ToList ();
		defensiveSubstitutes = defensiveSubstitutes.OrderByDescending (playerX => Manager.Instance.Players [playerX].Defense).ToList ();
		pinchRunners = pinchRunners.OrderByDescending (playerX => Manager.Instance.Players [playerX].Skills [3]).ToList ();	
	}

	// Saves batters
	public void SaveBatters ()
	{
		StreamWriter sw = new StreamWriter (@"Save\Batters.txt");

		for (int i = 0; i < batters.Count; i++)
			sw.WriteLine (batters [i] [0]);

		sw.Close ();
	}

	// Saves starting pitchers
	public void SaveSP ()
	{
		StreamWriter sw = new StreamWriter (@"Save\SP.txt");

		for (int i = 0; i < sp.Count; i++)
			sw.WriteLine (sp [i]);

		sw.Close ();
	}

	// Saves relief pitchers
	public void SaveRP ()
	{
		StreamWriter sw = new StreamWriter (@"Save\RP.txt");

		for (int i = 0; i < rp.Count; i++)
			sw.WriteLine (rp [i]);

		sw.Close ();
	}

	// Saves substitutes
	public void SaveSubstitutes ()
	{
		StreamWriter sw = new StreamWriter (@"Save\OffensiveSubs.txt");

		for (int i = 0; i < offensiveSubstitutes.Count; i++)
			sw.WriteLine (offensiveSubstitutes [i]);

		sw.Close ();

		sw = new StreamWriter (@"Save\DefensiveSubs.txt");

		for (int i = 0; i < defensiveSubstitutes.Count; i++)
			sw.WriteLine (defensiveSubstitutes [i]);

		sw.Close ();

		sw = new StreamWriter (@"Save\Substitutes.txt");

		for (int i = 0; i < offensiveSubstitutes.Count; i++)
			sw.WriteLine (offensiveSubstitutes [i]);

		sw.Close ();
	}

	// Saves batters
	public void SaveFortyManRoster ()
	{
		StreamWriter sw = new StreamWriter (@"Save\FortyManRoster.txt");

		for (int i = 0; i < fortyManRoster.Count; i++)
			sw.WriteLine (fortyManRoster [0]);

		sw.Close ();
	}

	// Removes a player to the list of substitutes
	public void RemoveSub (int playerID)
	{
		offensiveSubstitutes.Remove (playerID);
		defensiveSubstitutes.Remove (playerID);
		pinchRunners.Remove (playerID);
	}

	// Adds a player to the list of substitutes
	public void AddSub (int playerID)
	{
		offensiveSubstitutes.Add (playerID);
		defensiveSubstitutes.Add (playerID);
		pinchRunners.Add (playerID);
	}

	// Saves the team's Wins, Losses, Hype and Cash
	public void SaveWLHC ()
	{
		StreamWriter sw = new StreamWriter (@"Save\WLHC" + (int)teamType + "-" + id + ".txt");

		sw.Write (wins + "," + losses + "," + homeWins + "," + homeLosses + "," + awayWins + "," + awayLosses + "," + streak + "," + winStreak + "," + hype + "," + cash);
		sw.Close ();

		sw = new StreamWriter (@"Save\LastTen" + (int)teamType + "-" + id + ".txt");

		for (int i = 0; i < lastTen.Count; i++)
			sw.WriteLine (lastTen [i]);
		
		sw.Close ();
	}

	// Saves the team's overall skill
	public void SaveOveralls ()
	{
		StreamWriter sw = new StreamWriter (@"Save\Overalls" + id + ".txt");

		sw.Write (overalls [0] + "," + overalls [1] + "," + overalls [2]);
		sw.Close ();
	}

	// Adds a win
	public void Win (bool home)
	{
		if (winStreak)
			streak = 1;
		else
			streak++;

		winStreak = true;

		if (streak < 0)
			streak = 1;
		else
			streak++;
		
		hype += System.Math.Pow (1.005, streak) - 1.0;

		if (hype > 1)
			hype = 1;
		
		wins++;

		if (home)
			homeWins++;
		else
			awayWins++;

		lastTen.Add (true);

		if (lastTen.Count > 10)
			lastTen.RemoveAt (0);

		SaveWLHC ();
	}

	// Adds a loss
	public void Loss (bool home)
	{
		if (!winStreak)
			streak = 1;
		else
			streak++;

		winStreak = false;

		if (streak > 0)
			streak = -1;
		else
			streak--;
		hype -= System.Math.Pow (1.005, -streak) - 1.0;

		if (hype < 0)
			hype = 0;
		
		losses++;

		if (home)
			homeLosses++;
		else
			awayLosses++;

		SaveWLHC ();
	}

	// Adds revenue
	public void AddRevenue (double otherHype)
	{
		double thisHype = (hype * 2 + otherHype) / 3, thisRevenue;

		if (ticketPrice == 0)
			ticketsSold= stadiumCapacity;
		else
			ticketsSold = (int)System.Math.Round (50000 * 100 / ticketPrice * thisHype);
		
		if (ticketsSold > stadiumCapacity)
			ticketsSold = stadiumCapacity;

		if (foodPrice == 0)
			foodSold = ticketsSold * 2;
		else
			foodSold = (int)System.Math.Round (ticketsSold * (Random.value / 4 + Random.value / 4 + Random.value / 4 + 0.25) / (foodPrice / 10 * (0.75 + thisHype / 2)));

		if (drinkPrice == 0)
			drinksSold = ticketsSold * 2 + foodSold;
		else
			drinksSold = (int)System.Math.Round ((ticketsSold * (Random.value / 4 + Random.value / 4 + Random.value / 4 + 0.25) + foodSold) / (drinkPrice / 10 * (0.75 + thisHype / 2)));

		uniformsSold += (int)System.Math.Round (ticketsSold * hype * 100 / uniformPrice);

		thisRevenue = ticketsSold * ticketPrice + foodSold * foodPrice + drinksSold * drinkPrice + uniformsSold * uniformPrice;

		revenues += thisRevenue;
		profit += thisRevenue;
		cash += thisRevenue;

		SubtractExpenses ();
	}

	// Subtracts expenses
	public void SubtractExpenses ()
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
	public void BuyStadium (int tier)
	{
		stadiumCapacity = (int) (stadiumCapacity * 1.05);
		cash -= newStadiumPrice;
		newStadiumPrice *= 1.25;
		stadiumTier++;
	}

	// Rents a stadium
	public void RentStadium (int tier)
	{
		double price = 250000 * System.Math.Pow (1.25, tier);

		cash -= price;
		stadiumCapacity = (int) (50000 * System.Math.Pow (1.05, tier));
		stadiumTier = tier;

	}

	// Automatically sets the roster
	public void SetRoster ()
	{
		List<int> result;
		int benchSpace;

		majorLeagueIndexes.Clear ();
		minorLeagueIndexes.Clear ();
		lookingFor.Clear ();
		tradeBlock.Clear ();
		batters.Clear ();
		positions.Clear ();
		sp.Clear ();
		rp.Clear ();
		offensiveSubstitutes.Clear ();
		defensiveSubstitutes.Clear ();
		pinchRunners.Clear ();

		lookingFor.Add ("SP");
		lookingFor.Add ("SP");
		lookingFor.Add ("SP");
		lookingFor.Add ("SP");
		lookingFor.Add ("SP");
		lookingFor.Add ("RP");
		lookingFor.Add ("RP");
		lookingFor.Add ("RP");
		lookingFor.Add ("CP");
		lookingFor.Add ("C");
		lookingFor.Add ("1B");
		lookingFor.Add ("2B");
		lookingFor.Add ("3B");
		lookingFor.Add ("SS");
		lookingFor.Add ("LF");
		lookingFor.Add ("CF");
		lookingFor.Add ("RF");
		lookingFor.Add ("DH");

		benchSpace = Team.RosterSize - lookingFor.Count;

		result = players.OrderBy (playerX => Manager.Instance.Players [playerX].InjuryLength).ThenByDescending (playerX => Manager.Instance.Players [playerX].Position).ThenByDescending (playerX => Manager.Instance.Players [playerX].Overall).ToList<int> ();

		for (int j = 0; j < result.Count; j++)
		{
			if (lookingFor.Contains (Manager.Instance.Players [result [j]].Position) && AddToMajors (Manager.Instance.Players [result [j]].ID)) {
				if (Manager.Instance.Players [result [j]].Position == "SP")
				{
					sp.Add (Manager.Instance.Players [result [j]].ID);
					lookingFor.Remove ("SP");
				}
				else if (Manager.Instance.Players [result [j]].Position == "RP")
				{
					rp.Add (Manager.Instance.Players [result [j]].ID);
					lookingFor.Remove ("RP");
				}
				else if (Manager.Instance.Players [result [j]].Position == "CP")
				{
					cp = Manager.Instance.Players [result [j]].ID;
					lookingFor.Remove ("CP");
				}
				else
				{
					List<int> batterSlot = new List<int> ();
					batterSlot.Add (Manager.Instance.Players [result [j]].ID);
					batters.Add (batterSlot);
					positions.Add (Manager.Instance.Players [result [j]].Position);
					lookingFor.Remove (Manager.Instance.Players [result [j]].Position);
				}
			}
			else if (benchSpace > 0 && AddToMajors (Manager.Instance.Players [result [j]].ID))
			{
				if (Manager.Instance.Players [result [j]].IsPitcher)
					rp.Add (Manager.Instance.Players [result [j]].ID);
				else
					AddSub (Manager.Instance.Players [result [j]].ID);

				if (Manager.Instance.Players [result [j]].FirstTimeOnWaivers)
					tradeBlock.Add (Manager.Instance.Players [result [j]].ID);
			
				benchSpace--;
			}
			else
				AddToMinors (Manager.Instance.Players [result [j]].ID);
		}

		ascendingLineup ();
		minorLeagueIndexes = minorLeagueIndexes.OrderBy (playerX => Manager.Instance.Players [playerX].Overall).ToList ();
		CalculateOveralls ();
		Manager.Instance.RosterChange = true;
	}

	public void ResetLineup ()
	{
		List<int> tempRPs = new List<int> ();
		bool needReplacement;
		int index, count, playerIndex;

		for (int i = 0; i < 9; i++)
			while (batters [i].Count > 1)
			{
				AddSub (batters [i] [1]);
				batters [i].RemoveAt (1);
			}

		for (int i = 0; i < 9; i++)
		{
			if (Manager.Instance.Players [batters [i] [0]].InjuryLength > 0)
			{
				needReplacement = true;
				index = 0;

				if (Manager.Instance.Players [batters [i] [0]].InjuryLength > 0 && !(Manager.Instance.Players [batters [i] [0]].OnShortDisabledList || Manager.Instance.Players [batters [i] [0]].OnShortDisabledList))
				{
					if (Manager.Instance.Players [batters [i] [0]].InjuryLength >= Manager.LongDisabledListTime)
					{
						Manager.Instance.AddToLongDisabledList (batters [i] [0]);
					}
					else if (Manager.Instance.Players [batters [i] [0]].InjuryLength >= Manager.ShortDisabledListTime)
					{
						Manager.Instance.AddToShortDisabledList (batters [i] [0]);
						majorLeagueIndexes.Remove (batters [i] [0]);
					}
				}

				while (index < offensiveSubstitutes.Count && needReplacement)
				{
					if (Manager.Instance.Players [offensiveSubstitutes [index]].Position == Positions [i] && Manager.Instance.Players [offensiveSubstitutes [index]].InjuryLength == 0)
					{
						batters [i] [0] = offensiveSubstitutes [index];
						RemoveSub (offensiveSubstitutes [index]);
						needReplacement = false;
					}

					index++;
				}

				if (needReplacement)
				{
					if (offensiveSubstitutes.Count > 0)
					{
						batters [i] [0] = offensiveSubstitutes [0];
						RemoveSub (offensiveSubstitutes [0]);
					}
					else
					{
						index = 0;

						while (index < minorLeagueIndexes.Count && needReplacement)
						{
							playerIndex = minorLeagueIndexes [index];

							if (Manager.Instance.Players [playerIndex].Position == Positions [i] && Manager.Instance.Players [playerIndex].InjuryLength == 0 && AddToFortyManRoster(playerIndex))
							{
								batters [i] [0] = playerIndex;
								needReplacement = false;
							}

							index++;
						}
					}
				}
			}
		}

		for (int i = 0; i < sp.Count; i++)
			if (Manager.Instance.Players [sp [i]].InjuryLength > 0)
			{
				needReplacement = true;
				index = 0;

				if (Manager.Instance.Players [sp [i]].InjuryLength >= Manager.LongDisabledListTime)
				{
					Manager.Instance.AddToLongDisabledList (sp [i]);
					majorLeagueIndexes.Remove (sp [i]);
					fortyManRoster.Remove (sp [i]);
				}
				else if (Manager.Instance.Players [sp [i]].InjuryLength >= Manager.ShortDisabledListTime)
				{
					Manager.Instance.AddToShortDisabledList (sp [i]);
					majorLeagueIndexes.Remove (sp [i]);
				}

				if (tempRPs.Count == 0)
					tempRPs = rp.OrderBy (playerX => Manager.Instance.Players [playerX].InjuryLength).ThenByDescending (playerX => Manager.Instance.Players [playerX].Skills [10]).ToList ();

				if (tempRPs.Count > 0)
				{
					sp [i] = tempRPs [0];
					rp.Remove (tempRPs [0]);
					tempRPs.RemoveAt (0);
					needReplacement = false;
				}
				else
				{
					index = 0;

					while (index < minorLeagueIndexes.Count && needReplacement)
					{
						playerIndex = minorLeagueIndexes [index];

						if (Manager.Instance.Players [playerIndex].IsPitcher && Manager.Instance.Players [playerIndex].InjuryLength == 0 && AddToFortyManRoster(playerIndex))
						{
							sp [i] = playerIndex;
							needReplacement = false;
						}

						index++;
					}
				}
			}

		if (Manager.Instance.Players [cp].InjuryLength > 0)
		{
			if (Manager.Instance.Players [cp].InjuryLength >= Manager.LongDisabledListTime)
			{
				Manager.Instance.AddToLongDisabledList (cp);
				majorLeagueIndexes.Remove (cp);
				fortyManRoster.Remove (cp);
			}
			else if (Manager.Instance.Players [cp].InjuryLength >= Manager.ShortDisabledListTime)
			{
				Manager.Instance.AddToShortDisabledList (cp);
				majorLeagueIndexes.Remove (cp);
			}

			if (rp.Count > 0)
			{
				cp = rp [0];
				rp.RemoveAt (0);
			}
			else
			{
				needReplacement = true;
				index = 0;

				while (index < minorLeagueIndexes.Count && needReplacement)
				{
					playerIndex = minorLeagueIndexes [index];

					if (Manager.Instance.Players [playerIndex].IsPitcher && Manager.Instance.Players [playerIndex].InjuryLength == 0 && AddToFortyManRoster(playerIndex))
					{
						cp = playerIndex;
						needReplacement = false;
					}

					index++;
				}
			}
		}

		index = 0;
		count = 15 + rp.Count + offensiveSubstitutes.Count;

		while (count < rosterSize && index < minorLeagueIndexes.Count)
		{
			playerIndex = minorLeagueIndexes [index];

			if (AddToFortyManRoster (playerIndex))
			{
				if (Manager.Instance.Players [playerIndex].IsPitcher)
					rp.Add (playerIndex);
				else
					AddSub (playerIndex);

				count++;
			}

			index++;
		}

		if (count < rosterSize)
		{
			Manager.Instance.Players [Manager.Instance.FreeAgents [0]].Offer = Player.MinSalary;
			AddPlayer (Manager.Instance.FreeAgents [0]);
			Manager.Instance.Players [Manager.Instance.FreeAgents [0]].SavePlayer ();
			Manager.Instance.FreeAgents.RemoveAt (0);
		}
	}

	// Adds a new year to the team
	public void NewYear ()
	{
		ResetWins ();
		ResetLosses ();
		SaveWLHC ();

		for (int i = 0; i < players.Count; i++)
		{
			Manager.Instance.Players [players [i]].NewYear ();

			if (Manager.Instance.Players [players [i]].ContractYears.Count > 0)
			{
				Manager.Instance.Players [players [i]].ContractYears.RemoveAt (0);

				if (Manager.Instance.Players [players [i]].ContractYears.Count == 0)
				{
					Manager.Instance.Players [players [i]].Team = -1;
					Manager.Instance.Players [players [i]].Offer = 0;
					Manager.Instance.Teams [0] [id].RemovePlayer (players [i]);
					Manager.Instance.FreeAgents.Add (players [i]);
				}
			}

			Manager.Instance.Players [players [i]].CalculatePotential ();
		}

		StreamWriter sw = new StreamWriter (@"Save\FreeAgents.txt");

		for (int i = 0; i < Manager.Instance.FreeAgents.Count; i++)
			sw.WriteLine (Manager.Instance.FreeAgents [i]);

		sw.Close ();
	}

	// Calculates the overall skill for the team
	public void CalculateOveralls ()
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
	public bool AddToMajors (int index)
	{
		if (teamType == TeamType.MLB)
		{
			if (AddToFortyManRoster (index))
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
	public bool IsInFortyManRoster (int index)
	{
		if (fortyManRoster.Contains (index))
			return true;
		else
			return false;
	}

	// Adds a player to the forty man roster if there's room and they aren't already on it
	public bool AddToFortyManRoster (int index)
	{
		if (fortyManRoster.Contains (index))
			return true;
		else if (fortyManRoster.Count < 40)
		{
			fortyManRoster.Add (index);
			return true;
		}
		else
			return false;
	}

	public void RemoveFromMajorLeague (int index)
	{
		majorLeagueIndexes.Remove (index);
	}

	public void RemoveFromFortyManRoster (int index)
	{
		fortyManRoster.Remove (index);
	}

	// Adds a player to the minor leagues
	public void AddToMinors (int index)
	{
		majorLeagueIndexes.Remove (index);
		minorLeagueIndexes.Add (index);

		if (Manager.Instance.Players [index].FirstTimeOnWaivers)
			tradeBlock.Add (index);
	}

	// Uses the current starter
	public void UseStarter ()
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

		sw.Write (CityName + "," + TeamName + "," + Pick + "," + (int)Division + "," + (int)League);
		sw.Close ();
	}

	// Loads the team's information
	public void LoadTeamInfo ()
	{
		string [] teamInfoSplit = File.ReadAllLines (@"Save\Team" + (int)teamType + "-" + id + ".txt") [0].Split (',');

		CityName = teamInfoSplit [0];
		TeamName = teamInfoSplit [1];
		Pick = int.Parse (teamInfoSplit [2]);
		Division = (Division)int.Parse (teamInfoSplit [3]);
		League = (League)int.Parse (teamInfoSplit [4]);
	}

	// Loads the team
	public void LoadTeam ()
	{
		string[] split = File.ReadAllLines (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		LoadTeamInfo ();

		for (int i = 0; i < split.Length - 1; i++)
			players.Add (int.Parse (split [i]));

		split = File.ReadAllLines (@"Save\Overalls" + id + ".txt") [0].Split (',');
		overalls [0] = float.Parse (split [0]);
		overalls [1] = float.Parse (split [1]);
		overalls [2] = float.Parse (split [2]);
		split = (CityName + " " + TeamName).Split (' ');

		for (int i = 0; i < split.Length; i++)
			if (System.Char.IsLetter (split [i] [0]) && System.Char.IsUpper (split [i] [0]))
				Shortform += split [i] [0];

		split = File.ReadAllLines (@"Save\WLHC" + (int)teamType + "-" + id + ".txt") [0].Split (',');
		wins = int.Parse (split [0]);
		losses = int.Parse (split [1]);
		homeWins = int.Parse (split [2]);
		homeLosses = int.Parse (split [3]);
		awayWins = int.Parse (split [4]);
		awayLosses = int.Parse (split [5]);
		streak = int.Parse (split [6]);
		winStreak = bool.Parse (split [7]);
		hype = double.Parse (split [8]);
		cash = double.Parse (split [9]);

		split = File.ReadAllLines (@"Save\LastTen" + (int)teamType + "-" + id + ".txt");

		for (int i = 0; i < split.Length - 1; i++)
			lastTen.Add (bool.Parse (split [i]));
	}

	// Determines whether a player is currently in the batting ascending
	public bool IsBatter (int id)
	{
		int index = 0;
		bool notBatter = true;

		while (notBatter && index < batters.Count)
			if (batters [index++].Contains (id))
				notBatter = false;

		return !notBatter;
	}

	// Signed a draft player to the team
	public void SignDraftPlayer (int id)
	{
		AddPlayer (id);
		Manager.Instance.Players [id].ContractYears.Add (new ContractYear (ContractType.MinorLeague, 0.0));
		cash -= Manager.Instance.Players [id].Offer;
	}

	// Adds a player to the team
	public void AddPlayer (int playerID)
	{
		StreamWriter sw = File.AppendText (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		Manager.Instance.Players [playerID].Team = id;
		Manager.Instance.Players [playerID].SavePlayer ();
		players.Add (playerID);
		sw.WriteLine (playerID);
		sw.Close ();
	}

	// Removes a player from the team
	public void RemovePlayer (int playerID)
	{
		players.Remove (playerID);
		SavePlayers ();
	}

	// Saves the indexes of the team's players
	public void SavePlayers ()
	{
		StreamWriter sw = new StreamWriter (@"Save\TeamPlayers" + (int)teamType + "-" + id + ".txt");

		for (int i = 0; i < players.Count; i++)
			sw.WriteLine (players [i]);

		sw.Close ();
	}

	// Puts a player on waivers
	public void PutOnWaivers (int id, bool trade)
	{
		waivers.Add (id);
		Manager.Instance.PutOnWaivers (id, trade);
	}

	// Takes a player off waivers
	public void TakeOffWaivers (int playerID)
	{
		waivers.Remove (playerID);
		Manager.Instance.TakeOffWaivers (playerID);
		Manager.Instance.Players [playerID].TakeOffWaivers ();
	}

	public void AddToWaivers(int id)
	{
		waivers.Add (id);
	}

	// Gets the index of player with the worst overall
	public int GetWorstOverall ()
	{
		int [] temp = fortyManRoster.OrderBy (playerX => Manager.Instance.Players [playerX].Overall).ToArray ();

		return temp [temp.Length - 1];
	}

	// Sets the team's expenses
	public void SetExpenses ()
	{
		currentSalary = 0;

		for (int i = 0; i < fortyManRoster.Count; i++)
			currentSalary += Manager.Instance.Players [fortyManRoster [i]].ContractYears [0].Salary;
	}

	// Resets the team's wins
	public void ResetWins ()
	{
		wins = 0;
	}

	// Resets the team's losses
	public void ResetLosses ()
	{
		wins = 0;
	}

	public void NewTradeOffer(TradeOffer tradeOffer)
	{
		tradeOffers.Add (tradeOffer);
	}

	// Transfers a player to another team
	public string Transfer (int playerID, int teamID)
	{
		Manager.Instance.Teams [0] [teamID].AddPlayer (playerID);
		Manager.Instance.Teams [0] [teamID].currentSalary += Manager.Instance.Players [playerID].ContractYears [0].Salary;
		currentSalary -= Manager.Instance.Players [playerID].ContractYears [0].Salary;
		players.Remove (playerID);
		return Manager.Instance.Players [playerID].FirstName + " " + Manager.Instance.Players [playerID].LastName + ", ";
	}

	// Bankrupts team, forcing them to get a new owner or ending the game if it's the players team
	public void Bankrupt ()
	{
		if (id == 0)
		{
			Manager.Clear ();
			Manager.Instance.Load ();
			Manager.ChangeToScene (0);
		}
		else
		{
			string [] splitName;

			CityName = cityNames [ (int) (Random.value * cityNames.Length)];
			TeamName = teamNames [ (int) (Random.value * teamNames.Length)];

			if (CityName.Length > longestCityName)
				longestCityName = CityName.Length;

			if (TeamName.Length > longestTeamName)
				longestTeamName = TeamName.Length;

			Shortform = "";
			splitName = (CityName + " " + TeamName).Split (' ');

			for (int i = 0; i < splitName.Length; i++)
				if (System.Char.IsLetter (splitName [i] [0]) && System.Char.IsUpper (splitName [i] [0]))
					Shortform += splitName [i] [0];

			Debug.Log ("New owner " + cash);
			cash = 20000000.00 + System.Math.Round (Random.value * 10000000, 2);
		}
	}

	public void AcceptTrades ()
	{
		tradeOffers.Sort ((a, b) => (a.YourValue - a.TheirValue).CompareTo (b.YourValue - b.TheirValue));

		while (tradeOffers.Count > 0)
		{
			tradeOffers [0].Consider ();
			tradeOffers.RemoveAt (0);
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

	public List<string> LookingFor
	{
		get
		{
			return lookingFor;
		}
	}

	public List<List<int>> Batters
	{
		get
		{
			return batters;
		}
	}

	public List<int> TradeBlock
	{
		get
		{
			return tradeBlock;
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

	public List<int> PinchRunners
	{
		get
		{
			return pinchRunners;
		}
	}

	public int CP
	{
		get
		{
			return cp;
		}

		set
		{
			if (!AutomaticRoster)
				cp = value;
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

	public int HomeWins
	{
		get
		{
			return homeWins;
		}
	}

	public int HomeLosses
	{
		get
		{
			return homeLosses;
		}
	}

	public int AwayWins
	{
		get
		{
			return awayWins;
		}
	}

	public int AwayLosses
	{
		get
		{
			return awayLosses;
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

	public List<int> MajorLeagueIndexes
	{
		get
		{
			return majorLeagueIndexes;
		}
	}

	public List<int> FortyManRoster
	{
		get
		{
			return fortyManRoster;
		}
	}

	public int CurrStarter
	{
		get
		{
			return currStarter;
		}
	}

	public int Streak
	{
		get
		{
			return streak;
		}
	}

	public List<bool> LastTen
	{
		get
		{
			return lastTen;
		}
	}

	public bool WinStreak 
	{
		get
		{
			return winStreak;
		}
	}

	public TeamType Type
	{
		get
		{
			return teamType;
		}
	}

	public float WLR
	{
		get
		{
			if (losses == 0)
				return wins;
			else
				return wins / (float)losses;
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
	Central,
	East,
	West,
}

public enum TeamType
{
	MLB = 0,
	WorldBaseballClassic = 1,
	Futures = 2,
	AllStar = 3
}