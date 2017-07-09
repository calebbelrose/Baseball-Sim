using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Event
{
	public abstract void Action();
}

public class WorldBaseballClassic : Event
{
	public static List<Team> aTeams = new List<Team> (), bTeams = new List<Team> ();

	public override void Action()
	{
		List<Player> players = new List<Player>(), result;
		Country prevCountry = Country.Canada;
		int countryIndex = -1;
		int numTeams = 16;

		for(int i = 0; i < Manager.Instance.teams.Count; i++)
			for(int j = 0; j < Manager.Instance.teams[i].players.Count; j++)
				players.Add(Manager.Instance.Players[Manager.Instance.teams[i].players[j]]);

		result = players.OrderBy (playerX => playerX.country).ThenByDescending(playerX => playerX.overall).ToList ();

		for (int i = 0; i < result.Count; i++)
		{
			if (result [i].country != prevCountry)
			{
				prevCountry = result [i].country;
				aTeams.Add (new Team ());
				countryIndex++;
				aTeams [countryIndex].CityName = prevCountry.ToString();
				aTeams [countryIndex].Shortform = ((CountryShortforms)(int)prevCountry).ToString();
				aTeams [countryIndex].id = 1;
				aTeams [countryIndex].League = League.American;
			}

			aTeams [countryIndex].players.Add (result [i].ID);
		}

		for (int i = 0; i < aTeams.Count; i++)
		{
			List<string> remainingPositions = aTeams [i].AutomaticRoster ();

			for (int j = 0; j < remainingPositions.Count; j++)
			{
				Player newPlayer = new Player (remainingPositions [j], 16, 3, Manager.Instance.Players.Count);
				aTeams [i].players.Add (Manager.Instance.Players.Count);
				Manager.Instance.Players.Add (newPlayer);
			}

			while (aTeams [i].players.Count < Team.RosterSize)
			{
				Player newPlayer = new Player (Player.Positions [(int)(Random.value * Player.Positions.Length)], 16, 3, Manager.Instance.Players.Count);
				aTeams [i].players.Add (Manager.Instance.Players.Count);
				Manager.Instance.Players.Add (newPlayer);
			}

			aTeams [i].AutomaticRoster ();
		}

		aTeams = aTeams.OrderBy (teamX => teamX.Overalls[0]).ToList ();

		aTeams.RemoveRange (numTeams, aTeams.Count - numTeams);

		for (int i = 0; i < numTeams / 2; i++)
		{
			bTeams.Add (aTeams [i]);
			aTeams.RemoveAt (i);
		}

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (aTeams [0], aTeams [1]).PlayGame());
		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (aTeams [0], aTeams [2]).PlayGame());
		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (aTeams [1], aTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (aTeams [2], aTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (aTeams [3], aTeams [0]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (aTeams [1], aTeams [2]));

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (aTeams [4], aTeams [5]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (aTeams [6], aTeams [4]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (aTeams [5], aTeams [7]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (aTeams [7], aTeams [6]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (aTeams [7], aTeams [4]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (aTeams [6], aTeams [5]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].AddEvent (new WBCR2A ());

		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (bTeams [0], bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (bTeams [2], bTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (bTeams [2], bTeams [0]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (bTeams [3], bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (bTeams [1], bTeams [2]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (bTeams [0], bTeams [3]));

		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (bTeams [0], bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (bTeams [2], bTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (bTeams [2], bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (bTeams [3], bTeams [0]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (bTeams [3], bTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 7].ScheduledGames.Add (new ScheduledGame (bTeams [0], bTeams [2]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 7].AddEvent (new WBCR2B ());
	}
}

public class WBCR2A : Event
{
	public override void Action()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => teamX.Wins).ToList ();
		int halfIndex = WorldBaseballClassic.aTeams.Count / 2;

		if (WorldBaseballClassic.aTeams [halfIndex - 1].Wins == WorldBaseballClassic.aTeams [halfIndex].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [halfIndex - 1], WorldBaseballClassic.aTeams [halfIndex]).PlayGame ());

			if (WorldBaseballClassic.aTeams [halfIndex - 1].Wins < WorldBaseballClassic.aTeams [halfIndex].Wins)
				WorldBaseballClassic.aTeams [halfIndex - 1] = WorldBaseballClassic.aTeams [halfIndex];
		}
		
		WorldBaseballClassic.aTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < WorldBaseballClassic.aTeams.Count; i++)
			WorldBaseballClassic.aTeams [i].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [0], WorldBaseballClassic.aTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [2], WorldBaseballClassic.aTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [3], WorldBaseballClassic.aTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [0], WorldBaseballClassic.aTeams [2]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [3], WorldBaseballClassic.aTeams [0]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [1], WorldBaseballClassic.aTeams [2]));
	}
}

public class WBCR2B : Event
{
	public override void Action()
	{
		WorldBaseballClassic.bTeams = WorldBaseballClassic.bTeams.OrderBy (teamX => teamX.Wins).ToList ();
		int halfIndex = WorldBaseballClassic.bTeams.Count / 2;

		if (WorldBaseballClassic.bTeams [halfIndex - 1].Wins == WorldBaseballClassic.bTeams [halfIndex].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [halfIndex - 1], WorldBaseballClassic.bTeams [halfIndex]).PlayGame ());

			if (WorldBaseballClassic.bTeams [halfIndex - 1].Wins < WorldBaseballClassic.bTeams [halfIndex].Wins)
				WorldBaseballClassic.bTeams [halfIndex - 1] = WorldBaseballClassic.bTeams [halfIndex];
		}

		WorldBaseballClassic.bTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < WorldBaseballClassic.bTeams.Count; i++)
			WorldBaseballClassic.bTeams [i].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [0], WorldBaseballClassic.bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [2], WorldBaseballClassic.bTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [3], WorldBaseballClassic.bTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [0], WorldBaseballClassic.bTeams [2]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [3], WorldBaseballClassic.bTeams [0]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [1], WorldBaseballClassic.bTeams [2]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].AddEvent (new WBCSemiFinal ());
	}
}

public class WBCSemiFinal : Event
{
	public override void Action()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => teamX.Wins).ToList ();
		WorldBaseballClassic.bTeams = WorldBaseballClassic.bTeams.OrderBy (teamX => teamX.Wins).ToList ();
		int halfIndex = WorldBaseballClassic.aTeams.Count / 2;

		if (WorldBaseballClassic.aTeams [halfIndex - 1].Wins == WorldBaseballClassic.aTeams [halfIndex].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [halfIndex - 1], WorldBaseballClassic.aTeams [halfIndex]).PlayGame ());

			if (WorldBaseballClassic.aTeams [halfIndex - 1].Wins < WorldBaseballClassic.aTeams [halfIndex].Wins)
				WorldBaseballClassic.aTeams [halfIndex - 1] = WorldBaseballClassic.aTeams [halfIndex];
		}

		if (WorldBaseballClassic.bTeams [halfIndex - 1].Wins == WorldBaseballClassic.bTeams [halfIndex].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (WorldBaseballClassic.bTeams [halfIndex - 1], WorldBaseballClassic.bTeams [halfIndex]).PlayGame ());

			if (WorldBaseballClassic.bTeams [halfIndex - 1].Wins < WorldBaseballClassic.bTeams [halfIndex].Wins)
				WorldBaseballClassic.bTeams [halfIndex - 1] = WorldBaseballClassic.bTeams [halfIndex];
		}

		WorldBaseballClassic.aTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < halfIndex; i++)
			WorldBaseballClassic.aTeams.Add (WorldBaseballClassic.bTeams [i]);

		for (int i = 0; i < WorldBaseballClassic.aTeams.Count; i++)
			WorldBaseballClassic.aTeams [i].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [0], WorldBaseballClassic.aTeams [1]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [2], WorldBaseballClassic.aTeams [3]));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].AddEvent (new WBCFinal ());
	}
}


public class WBCFinal : Event
{
	private string displayText = "WBC Final";

	public override void Action()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => teamX.Wins).ToList ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (WorldBaseballClassic.aTeams [0], WorldBaseballClassic.aTeams [1]).PlayGame());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames[0].ToString();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class OpeningDay : Event
{
	public override void Action()
	{

	}
}

public class FirstYearPlayerDraft : Event
{
	public override void Action()
	{
		Manager.ChangeToScene (3);
	}
}

public class FuturesGame : Event
{
	private string displayText = "Futures Game";

	public override void Action()
	{
		List<Player> players = new List<Player> ();
		Team[] teams = new Team[2];
		List<string>[] positions = new List<string>[2];
		int index = 0;

		teams [0] = new Team ();
		teams [1] = new Team ();
		teams [0].Shortform = "USA";
		teams [1].Shortform = "WLD";
		positions[0] = Player.Positions.ToList();
		positions[1] = Player.Positions.ToList();

		positions [0].Add ("RP");
		positions [0].Add ("RP");
		positions [1].Add ("RP");
		positions [1].Add ("RP");


		for (int i = 0; i < Manager.Instance.teams.Count; i++)
			for (int j = 0; j < Manager.Instance.teams [i].MinorLeagueIndexes.Count; j++)
				players.Add (Manager.Instance.Players [Manager.Instance.teams [i].MinorLeagueIndexes [j]]);

		while (positions [0].Count > 0)
		{
			bool playerNotFound = true;

			while (index < players.Count && playerNotFound)
			{
				if (positions [0].Contains (players [index].position) && players [index].country == Country.UnitedStates)
					playerNotFound = false;
				else
					index++;
			}
				
			if (!playerNotFound)
			{
				
				teams [0].players.Add (players [index].ID);
				positions [0].Remove (players [index].position);
				players.RemoveAt (index);
			}
			else
				break;
		}

		index = 0;

		while (positions [1].Count > 0)
		{
			bool playerNotFound = true;

			while (index < players.Count && playerNotFound)
			{
				if (positions [1].Contains (players [index].position) && players [index].country == Country.UnitedStates)
					playerNotFound = false;
				else
					index++;
			}

			if (!playerNotFound)
			{
				teams [1].players.Add (players [index].ID);
				positions [1].Remove (players [index].position);
				players.RemoveAt (index);
			}
			else
				break;
		}

		for (int i = 0; i < positions [0].Count; i++)
		{
			Player newPlayer = new Player (positions [0][i], 16, 2, Manager.Instance.Players.Count);
			newPlayer.country = Country.UnitedStates;
			teams [0].players.Add (newPlayer.ID);
			Manager.Instance.Players.Add (newPlayer);
		}

		for (int i = 0; i < positions [1].Count; i++)
		{
			Player newPlayer = new Player (positions [1][i], 16, 2, Manager.Instance.Players.Count);
			while (newPlayer.country == Country.UnitedStates)
				newPlayer.RandomCountry ();
			Manager.Instance.Players.Add (newPlayer);
			teams [1].players.Add (newPlayer.ID);
		}

		index = 0;

		while (teams [0].players.Count < Team.RosterSize)
		{
			while (index < players.Count && players [index].country != Country.UnitedStates)
				index++;

			if (index < players.Count)
				teams [0].players.Add (players [index].ID);
			else
				break;
		}

		while (teams [0].players.Count < Team.RosterSize)
		{
			Player newPlayer = new Player (Player.Positions[(int)(Random.value * Player.Positions.Length)], 16, 2, Manager.Instance.Players.Count);
			newPlayer.country = Country.UnitedStates;
			teams [0].players.Add (newPlayer.ID);
		}

		index = 0;

		while (teams [1].players.Count < Team.RosterSize)
		{
			while (index < players.Count && players [index].country == Country.UnitedStates)
				index++;

			if(index < players.Count)
				teams [1].players.Add (players [index].ID);
			else
				break;
		}

		while (teams [1].players.Count < Team.RosterSize)
		{
			Player newPlayer = new Player (Player.Positions[(int)(Random.value * Player.Positions.Length)], 16, 2, Manager.Instance.Players.Count);
			while (newPlayer.country == Country.UnitedStates)
				newPlayer.RandomCountry ();
			teams [1].players.Add (newPlayer.ID);
		}

		teams [0].AutomaticRoster ();
		teams [1].AutomaticRoster ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (teams [0], teams [1]).PlayGame ());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames[0].ToString();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class HomerunDerby : Event
{
	private string displayText = "Homerun Derby";

	public override void Action()
	{
		List<Player> players = new List<Player>();
		int maxIndex = 7;

		for(int i = 0; i < Manager.Instance.Players.Count; i++)
			players.Add(Manager.Instance.Players[i]);

		players = players.OrderByDescending (playerX => playerX.stats[0][7]).ToList ();

		while (maxIndex != 0)
		{
			List<int> losers = new List<int> ();

			for (int index1 = 0, index2 = maxIndex; index1 < index2; index1++)
			{
				int[] scores = new int[2];
				int[] indexes = new int[2];

				indexes [0] = index1;
				indexes [1] = index2--;

				for (int player = 0; player < 2; player++)
				{
					float time = 240.0f;
					bool needExtraTime = true;
					int extraTimeHomeruns = 0;

					while (time > 0.0f)
					{
						float value = (Random.value * players [indexes [player]].skills [0] + Random.value * players [indexes [player]].skills [1] + Random.value * players [indexes [player]].skills [2]);
						float distance = value * 2.5f;

						if (value > 200.0f)
							scores [player]++;

						if (distance >= 440.0f && needExtraTime && ++extraTimeHomeruns == 2)
						{
							time += 30.0f;
							needExtraTime = false;
						}

						time -= distance / 50;
					}
				}

				if (scores [0] > scores [1])
					losers.Add (indexes [1]);
				else if (scores [1] > scores [0])
					losers.Add (indexes [0]);
				else
				{
					

					for (int player = 0; player < 2; player++)
					{
						float time = 60.0f;

						while (time > 0.0f)
						{
							float value = (Random.value * players [indexes [player]].skills [0] + Random.value * players [indexes [player]].skills [1] + Random.value * players [indexes [player]].skills [2]);

							if (value > 200.0f)
								scores [player]++;

							time -= value * 0.05f;
						}
					}

					if (scores [0] == scores [1])
					{
						int swings = 0;

						while (swings < 3 || scores [0] == scores [1])
						{
							for (int player = 0; player < 2; player++)
							{
								float value = (Random.value * players [indexes [player]].skills [0] + Random.value * players [indexes [player]].skills [1] + Random.value * players [indexes [player]].skills [2]);

								if (value > 200.0f)
									scores [player]++;
								
								swings++;
							}
						}
					}

					if (scores [0] > scores [1])
						losers.Add (indexes [1]);
					else
						losers.Add (indexes [0]);
				}
				Debug.Log (scores [0] + " " + scores [1]);
			}

			losers = losers.OrderByDescending (loser => loser).ToList ();


			while (losers.Count > 0)
			{
				players.RemoveAt (losers [0]);
				losers.RemoveAt (0);
			}

			maxIndex /= 2;
		}

		displayText = "Winner:\n" + players [0].Name;
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class AllStarGame : Event
{
	string displayText = "All-Star Game";

	public override void Action()
	{
		List<Player> players = new List<Player> ();
		Team[] teams = new Team[2];
		List<string>[] positions = new List<string>[2];
		int index = 0;

		teams [0] = new Team ();
		teams [1] = new Team ();
		teams [0].Shortform = "A.L.";
		teams [1].Shortform = "N.L.";
		positions[0] = Player.Positions.ToList();
		positions[1] = Player.Positions.ToList();

		positions [0].Add ("RP");
		positions [0].Add ("RP");
		positions [1].Add ("RP");
		positions [1].Add ("RP");

		for (int i = 0; i < Manager.Instance.Players.Count; i++)
			players.Add (Manager.Instance.Players [i]);

		players = players.OrderByDescending (playerX => playerX.overall).ToList ();

		while (positions [0].Count > 0)
		{
			while (!positions [0].Contains (players [index].position) || Manager.Instance.teams[players [index].team].League != League.American)
				index++;

			teams [0].players.Add (players [index].ID);

			positions [0].Remove (players [index].position);
			players.RemoveAt (index);
		}

		index = 0;

		while (positions [1].Count > 0)
		{
			while (!positions [1].Contains (players [index].position) || Manager.Instance.teams[players [index].team].League != League.American)
				index++;

			teams [1].players.Add (players [index].ID);
			positions [1].Remove (players [index].position);
			players.RemoveAt (index);
		}

		index = 0;

		while (teams [0].players.Count < Team.RosterSize)
		{
			while (Manager.Instance.teams[players [index].team].League != League.American)
				index++;
			
			teams [0].players.Add (players [index].ID);
			index++;
		}

		index = 0;

		while (teams [1].players.Count < Team.RosterSize)
		{
			while (Manager.Instance.teams [players [index].team].League != League.National)
				index++;

			teams [1].players.Add (players [index].ID);
			index++;
		}

		teams [0].AutomaticRoster ();
		teams [1].AutomaticRoster ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (teams [0], teams [1]).PlayGame ());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames[0].ToString();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class SigningDeadline : Event
{
	public override void Action()
	{
		for (int i = 0; i < Manager.Instance.teams.Count; i++)
			while (Manager.Instance.teams [i].DraftPicks.Count > 0)
			{
				if (Manager.Instance.Players [Manager.Instance.teams [i].DraftPicks [0]].ConsiderOffer ())
					Manager.Instance.teams [i].players.Add (Manager.Instance.teams [i].DraftPicks [0]);
				else
					Draft.ReturnToDraft (Manager.Instance.teams [i].DraftPicks [0]);
				
				Manager.Instance.teams [i].DraftPicks.RemoveAt (0);
			}
	}
}

public class HallOfFameInduction : Event
{
	public override void Action()
	{
		double bestScore = 0.0;
		int bestIndex = 0;

		for(int i = 0; i < Manager.Instance.hallOfFameCandidates.Count; i++)
		{
			double score;

			if(Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].position.Contains("P"))
			{
				double era = Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][24] * 27 / (double)Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][20];
				score = (6.0 - era) * 5 + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][20] / (double)8;
			}
			else
			{
				double ops = (Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][3] + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][10]) / (double)(Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][1] + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][10] + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][14]) + (Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][3] + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][5] * 2 + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][6] * 3 + Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][7] * 4) / (double)Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][1];
				score = Manager.Instance.Players[Manager.Instance.hallOfFameCandidates[i]].stats[0][7] / 40.0 + ops * 25;
			}

			if (score > bestScore)
			{
				bestScore = score;
				bestIndex = i;
			}
		}

		if (Manager.Instance.hallOfFameCandidates.Count > 0)
		{
			Manager.Instance.hallOfFameInductees.Add (new HallOfFameInductee (Manager.Instance.Year, Manager.Instance.hallOfFameCandidates [bestIndex]));
			Manager.Instance.hallOfFameCandidates.RemoveAt (bestIndex);
		}
	}
}

public class NonWaiverTradeDeadline : Event
{
	public override void Action()
	{
		Manager.Instance.tradeDeadline = TradeDeadline.NonWaiver;
	}
}

public class WaiverTradeDeadline : Event
{
	public override void Action()
	{
		Manager.Instance.tradeDeadline = TradeDeadline.Waiver;
	}
}

public class ActiveRosterExpansion : Event
{
	public override void Action()
	{
		Team.ChangeRosterSize (40);
	}
}

public class EndOfRegularSeason : Event
{
	public override void Action()
	{
		Team.ChangeRosterSize (25);
		Manager.ChangeToScene(6);
	}
}

public enum TradeDeadline
{
	None,
	Waiver,
	NonWaiver
}