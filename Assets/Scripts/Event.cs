using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public abstract class Event
{
	// Action to perform on the day of the event
	public abstract void Action ();

	// Saves the event
	public abstract void Save (int dayIndex);

	// Loads the event
	public static Event Load (string [] text)
	{
		switch (int.Parse (text [1]))
		{
		case 0:
			return new WorldBaseballClassic ();
		case 1:
			return new WBCR2A ();
		case 2:
			return new WBCR2B ();
		case 3:
			return new WBCSemiFinal ();
		case 4:
			return new WBCFinal ();
		case 5:
			return new OpeningDay ();
		case 6:
			return new FirstYearPlayerDraft ();
		case 7:
			return new FuturesGame ();
		case 8:
			return new HomerunDerby ();
		case 9:
			return new AllStarGame ();
		case 10:
			return new SigningDeadline ();
		case 11:
			return new HallOfFameInduction ();
		case 12:
			return new NonWaiverTradeDeadline ();
		case 13:
			return new WaiverTradeDeadline ();
		case 14:
			return new ActiveRosterExpansion ();
		case 15:
			return new EndOfYear ();
		case 16:
			return new EndOfRegularSeason ();
		case 17:
			return new FinalsCheck (int.Parse (text [2]), int.Parse (text [3]), int.Parse (text [4]));
		case 18:
			return new WorldChampion ();
		case 19:
			return new CheckTieWinner (int.Parse (text [2]), int.Parse (text [3]), int.Parse (text [4]), int.Parse (text [5]));
		default:
			return null;
		}
	}
}

public class WorldBaseballClassic : Event
{
	public static List<int> aTeams = new List<int> (), bTeams = new List<int> ();

	public override void Action ()
	{
		List<Player> players = new List<Player> (), result;
		Country prevCountry = Country.Canada;
		int index = -1;
		int numTeams = 16;

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			for (int j = 0; j < Manager.Instance.Teams [0] [i].Players.Count; j++)
				players.Add (Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]]);

		result = players.OrderBy (playerX => playerX.Country).ThenByDescending (playerX => playerX.Overall).ToList ();

		for (int i = 0; i < result.Count; i++)
		{
			if (result [i].Country != prevCountry)
			{
				prevCountry = result [i].Country;
				index = (int)prevCountry;
				aTeams.Add (index);
			}

			Manager.Instance.Teams [1] [index].AddPlayer (result [i].ID);
		}

		for (int i = 0; i < aTeams.Count; i++)
		{
			Manager.Instance.Teams [1] [aTeams [i]].SetRoster ();

			for (int j = 0; j < Manager.Instance.Teams [1] [aTeams [i]].LookingFor.Count; j++)
			{
				Player newPlayer = new Player (Manager.Instance.Teams [1] [aTeams [i]].LookingFor [j], 16, 3, Manager.Instance.Players.Count);
				newPlayer.Country = (Country)Manager.Instance.Teams [1] [aTeams [i]].ID;
				Manager.Instance.NewPlayer (newPlayer);
				Manager.Instance.Teams [1] [aTeams [i]].AddPlayer (newPlayer.ID);
			}

			while (Manager.Instance.Teams [1] [aTeams [i]].Players.Count < Team.RosterSize)
			{
				Player newPlayer = new Player (Player.Positions [ (int) (Random.value * Player.Positions.Length)], 16, 3, Manager.Instance.Players.Count);
				newPlayer.Country = (Country)Manager.Instance.Teams [1] [aTeams [i]].ID;
				Manager.Instance.NewPlayer (newPlayer);
				Manager.Instance.Teams [1] [aTeams [i]].AddPlayer (newPlayer.ID);
			}

			Manager.Instance.Teams [1] [aTeams [i]].SetRoster ();
		}

		aTeams = aTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Overalls [0]).ToList ();

		aTeams.RemoveRange (numTeams, aTeams.Count - numTeams);

		for (int i = 0; i < numTeams / 2; i++)
		{
			bTeams.Add (aTeams [i]);
			aTeams.RemoveAt (i);
		}

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [0]], Manager.Instance.Teams [1] [aTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());
		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [0]], Manager.Instance.Teams [1] [aTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());
		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [1]], Manager.Instance.Teams [1] [aTeams [3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [2]], Manager.Instance.Teams [1] [aTeams [3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [3]], Manager.Instance.Teams [1] [aTeams [0]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [1]], Manager.Instance.Teams [1] [aTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [4]], Manager.Instance.Teams [1] [aTeams [5]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [6]], Manager.Instance.Teams [1] [aTeams [4]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [5]], Manager.Instance.Teams [1] [aTeams [7]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [7]], Manager.Instance.Teams [1] [aTeams [6]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [7]], Manager.Instance.Teams [1] [aTeams [4]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [aTeams [6]], Manager.Instance.Teams [1] [aTeams [5]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].AddEvent (new WBCR2A (), Manager.Instance.DayIndex + 4);

		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [0]], Manager.Instance.Teams [1] [bTeams[1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [2]], Manager.Instance.Teams [1] [bTeams[3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [2]], Manager.Instance.Teams [1] [bTeams[0]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [3]], Manager.Instance.Teams [1] [bTeams[1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [1]], Manager.Instance.Teams [1] [bTeams[2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 6));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [0]], Manager.Instance.Teams [1] [bTeams[3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 6));

		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [0]], Manager.Instance.Teams [1] [bTeams[1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [2]], Manager.Instance.Teams [1] [bTeams[3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [2]], Manager.Instance.Teams [1] [bTeams[1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [3]], Manager.Instance.Teams [1] [bTeams[0]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [3]], Manager.Instance.Teams [1] [bTeams[3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 6));
		Manager.Instance.Days [Manager.Instance.DayIndex + 7].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [bTeams [0]], Manager.Instance.Teams [1] [bTeams[2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 7));
		Manager.Instance.Days [Manager.Instance.DayIndex + 7].AddEvent (new WBCR2B (), Manager.Instance.DayIndex + 7);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",0");
		sw.Close ();
	}
}

public class WBCR2A : Event
{
	public override void Action ()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Wins).ToList ();
		int halfIndex = WorldBaseballClassic.aTeams.Count / 2;

		if (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]].Wins == Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());

			if (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]].Wins < Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]].Wins)
				WorldBaseballClassic.aTeams [halfIndex - 1] = WorldBaseballClassic.aTeams [halfIndex];
		}
		
		WorldBaseballClassic.aTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < WorldBaseballClassic.aTeams.Count; i++)
			Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [i]].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [2]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [3]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [3]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [0]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [1]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",1");
		sw.Close ();
	}
}

public class WBCR2B : Event
{
	public override void Action ()
	{
		WorldBaseballClassic.bTeams = WorldBaseballClassic.bTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Wins).ToList ();
		int halfIndex = WorldBaseballClassic.bTeams.Count / 2;

		if (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]].Wins == Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());

			if (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]].Wins < Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]].Wins)
				WorldBaseballClassic.bTeams [halfIndex - 1] = WorldBaseballClassic.bTeams [halfIndex];
		}

		WorldBaseballClassic.bTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < WorldBaseballClassic.bTeams.Count; i++)
			Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [i]].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
		Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [2]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [3]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 3));
		Manager.Instance.Days [Manager.Instance.DayIndex + 4].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 4));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [3]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [0]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 5].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [1]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [2]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 5));
		Manager.Instance.Days [Manager.Instance.DayIndex + 6].AddEvent (new WBCSemiFinal (), Manager.Instance.DayIndex + 6);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",2");
		sw.Close ();
	}

	public override string ToString ()
	{
		return "";
	}
}

public class WBCSemiFinal : Event
{
	public override void Action ()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Wins).ToList ();
		WorldBaseballClassic.bTeams = WorldBaseballClassic.bTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Wins).ToList ();
		int halfIndex = WorldBaseballClassic.aTeams.Count / 2;

		if (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]].Wins == Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());

			if (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]].Wins < Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]].Wins)
				Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex - 1]] = Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [halfIndex]];
		}

		if (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]].Wins == Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]].Wins)
		{
			Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]], Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());

			if (Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]].Wins < Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]].Wins)
				Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex - 1]] = Manager.Instance.Teams [1] [WorldBaseballClassic.bTeams [halfIndex]];
		}

		WorldBaseballClassic.aTeams.RemoveRange (halfIndex, halfIndex);

		for (int i = 0; i < halfIndex; i++)
			WorldBaseballClassic.aTeams.Add (WorldBaseballClassic.bTeams [i]);

		for (int i = 0; i < WorldBaseballClassic.aTeams.Count; i++)
			Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [i]].ResetWins ();

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 1));
			Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [2]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [3]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex + 2));
		Manager.Instance.Days [Manager.Instance.DayIndex + 3].AddEvent (new WBCFinal (), Manager.Instance.DayIndex + 3);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",3");
		sw.Close ();
	}

	public override string ToString ()
	{
		return "";
	}
}


public class WBCFinal : Event
{
	private string displayText = "WBC Final";

	public override void Action ()
	{
		WorldBaseballClassic.aTeams = WorldBaseballClassic.aTeams.OrderBy (teamX => Manager.Instance.Teams [1] [teamX].Wins).ToList ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [0]], Manager.Instance.Teams [1] [WorldBaseballClassic.aTeams [1]], GameType.WorldBaseballClassic, TeamType.WorldBaseballClassic, Manager.Instance.DayIndex).PlayGame ());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].ToString ();
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",4");
		sw.Close ();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class OpeningDay : Event
{
	public override void Action ()
	{
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",5");
		sw.Close ();
	}
}

public class FirstYearPlayerDraft : Event
{
	public override void Action ()
	{
		PlayerPrefs.SetString ("NeedDraft", true.ToString ());
		PlayerPrefs.Save ();
		Manager.ChangeToScene (3);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",6");
		sw.Close ();
	}
}

public class FuturesGame : Event
{
	private string displayText = "Futures Game";

	public override void Action ()
	{
		List<Player> players = new List<Player> ();
		List<string> [] positions = new List<string>[2];
		int index = 0;

		Manager.Instance.ExecuteOnMainThread.Enqueue (() => {
			NewFutuesTeams ();
		});

		positions [0] = Player.Positions.ToList ();
		positions [1] = Player.Positions.ToList ();

		positions [0].Add ("RP");
		positions [0].Add ("RP");
		positions [1].Add ("RP");
		positions [1].Add ("RP");

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			for (int j = 0; j < Manager.Instance.Teams [0] [i].MinorLeagueIndexes.Count; j++)
				players.Add (Manager.Instance.Players [Manager.Instance.Teams [0] [i].MinorLeagueIndexes [j]]);

		while (positions [0].Count > 0)
		{
			bool playerNotFound = true;

			while (index < players.Count && playerNotFound)
			{
				if (positions [0].Contains (players [index].Position) && players [index].Country == Country.UnitedStates)
					playerNotFound = false;
				else
					index++;
			}
				
			if (!playerNotFound)
			{
				
				Manager.Instance.Teams [2] [0].AddPlayer (players [index].ID);
				positions [0].Remove (players [index].Position);
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
				if (positions [1].Contains (players [index].Position) && players [index].Country == Country.UnitedStates)
					playerNotFound = false;
				else
					index++;
			}

			if (!playerNotFound)
			{
				Manager.Instance.Teams [2] [1].AddPlayer (players [index].ID);
				positions [1].Remove (players [index].Position);
				players.RemoveAt (index);
			}
			else
				break;
		}

		for (int i = 0; i < positions [0].Count; i++)
		{
			Player newPlayer = new Player (positions [0] [i], 16, 2, Manager.Instance.Players.Count);
			newPlayer.Country = Country.UnitedStates;
			Manager.Instance.Teams [2] [0].AddPlayer (newPlayer.ID);
			Manager.Instance.NewPlayer (newPlayer);
		}

		for (int i = 0; i < positions [1].Count; i++)
		{
			Player newPlayer = new Player (positions [1] [i], 16, 2, Manager.Instance.Players.Count);
			while (newPlayer.Country == Country.UnitedStates)
				newPlayer.RandomCountry ();
			Manager.Instance.Teams [2] [1].AddPlayer (newPlayer.ID);
			Manager.Instance.NewPlayer (newPlayer);
		}

		index = 0;

		while (Manager.Instance.Teams [2] [0].Players.Count < Team.RosterSize)
		{
			while (index < players.Count && players [index].Country != Country.UnitedStates)
				index++;

			if (index < players.Count)
				Manager.Instance.Teams [2] [0].AddPlayer (players [index].ID);
			else
				break;
		}

		while (Manager.Instance.Teams [2] [0].Players.Count < Team.RosterSize)
		{
			Player newPlayer = new Player (Player.Positions [ (int) (Random.value * Player.Positions.Length)], 16, 2, Manager.Instance.Players.Count);
			newPlayer.Country = Country.UnitedStates;
			Manager.Instance.Teams [2] [0].AddPlayer (newPlayer.ID);
			Manager.Instance.NewPlayer (newPlayer);
		}

		index = 0;

		while (Manager.Instance.Teams [2] [1].Players.Count < Team.RosterSize)
		{
			while (index < players.Count && players [index].Country == Country.UnitedStates)
				index++;

			if (index < players.Count)
				Manager.Instance.Teams [2] [1].AddPlayer (players [index].ID);
			else
				break;
		}

		while (Manager.Instance.Teams [2] [1].Players.Count < Team.RosterSize)
		{
			Player newPlayer = new Player (Player.Positions [ (int) (Random.value * Player.Positions.Length)], 16, 2, Manager.Instance.Players.Count);
			while (newPlayer.Country == Country.UnitedStates)
				newPlayer.RandomCountry ();
			Manager.Instance.Teams [2] [1].AddPlayer (newPlayer.ID);
			Manager.Instance.NewPlayer (newPlayer);
		}

		Manager.Instance.Teams [2] [0].SetRoster ();
		Manager.Instance.Teams [2] [1].SetRoster ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [2] [0], Manager.Instance.Teams [2] [1], GameType.WorldBaseballClassic, TeamType.Futures, Manager.Instance.DayIndex).PlayGame ());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].ToString ();
	}

	void NewFutuesTeams ()
	{
		Manager.Instance.Teams [2] [0] = new Team (TeamType.Futures, 0);
		Manager.Instance.Teams [2] [1] = new Team (TeamType.Futures, 1);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",7");
		sw.Close ();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class HomerunDerby : Event
{
	private string displayText = "Homerun Derby";

	public override void Action ()
	{
		List<Player> players = new List<Player> ();
		int maxIndex = 7;

		for (int i = 0; i < Manager.Instance.Players.Count; i++)
			players.Add (Manager.Instance.Players [i]);

		players = players.OrderByDescending (playerX => playerX.Stats [0] [7]).ToList ();

		while (maxIndex != 0)
		{
			List<int> losers = new List<int> ();

			for (int index1 = 0, index2 = maxIndex; index1 < index2; index1++)
			{
				int [] scores = new int [2];
				int [] indexes = new int [2];

				indexes [0] = index1;
				indexes [1] = index2--;

				for (int player = 0; player < 2; player++)
				{
					float time = 240.0f;
					bool needExtraTime = true;
					int extraTimeHomeruns = 0;

					while (time > 0.0f)
					{
						float value = (Random.value * players [indexes [player]].Skills [0] + Random.value * players [indexes [player]].Skills [1] + Random.value * players [indexes [player]].Skills [2]);
						float distance = value * 2.5f;

						if (value > 175.0f)
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
							float value = (Random.value * players [indexes [player]].Skills [0] + Random.value * players [indexes [player]].Skills [1] + Random.value * players [indexes [player]].Skills [2]);

							if (value > 175.0f)
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
								float value = (Random.value * players [indexes [player]].Skills [0] + Random.value * players [indexes [player]].Skills [1] + Random.value * players [indexes [player]].Skills [2]);

								if (value > 175.0f)
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

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",8");
		sw.Close ();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class AllStarGame : Event
{
	string displayText = "All-Star Game";

	public override void Action ()
	{
		List<Player> players = new List<Player> ();
		List<string> [] positions = new List<string>[2];
		int index = 0;

		Manager.Instance.ExecuteOnMainThread.Enqueue (() => {
			NewAllStarTeams ();
		});

		positions [0] = Player.Positions.ToList ();
		positions [1] = Player.Positions.ToList ();

		positions [0].Add ("RP");
		positions [0].Add ("RP");
		positions [1].Add ("RP");
		positions [1].Add ("RP");

		for (int i = 0; i < Manager.Instance.Players.Count; i++)
			players.Add (Manager.Instance.Players [i]);

		players = players.OrderByDescending (playerX => playerX.Overall).ToList ();

		while (positions [0].Count > 0)
		{
			while (!positions [0].Contains (players [index].Position) || Manager.Instance.Teams [0] [players [index].Team].League != League.American)
				index++;

			Manager.Instance.Teams [3] [0].AddPlayer (players [index].ID);

			positions [0].Remove (players [index].Position);
			players.RemoveAt (index);
		}

		index = 0;

		while (positions [1].Count > 0)
		{
			while (!positions [1].Contains (players [index].Position) || Manager.Instance.Teams [0] [players [index].Team].League != League.American)
				index++;

			Manager.Instance.Teams [3] [1].AddPlayer (players [index].ID);
			positions [1].Remove (players [index].Position);
			players.RemoveAt (index);
		}

		index = 0;

		while (Manager.Instance.Teams [3] [0].Players.Count < Team.RosterSize)
		{
			while (Manager.Instance.Teams [0] [players [index].Team].League != League.American)
				index++;
			
			Manager.Instance.Teams [3] [0].AddPlayer (players [index].ID);
			index++;
		}

		index = 0;

		while (Manager.Instance.Teams [3] [1].Players.Count < Team.RosterSize)
		{
			while (Manager.Instance.Teams [0] [players [index].Team].League != League.National)
				index++;

			Manager.Instance.Teams [3] [1].AddPlayer (players [index].ID);
			index++;
		}

		Manager.Instance.Teams [3] [0].SetRoster ();
		Manager.Instance.Teams [3] [1].SetRoster ();

		Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames.Add (new ScheduledGame (Manager.Instance.Teams [3] [0], Manager.Instance.Teams [3] [1], GameType.AllStar, TeamType.AllStar, Manager.Instance.DayIndex).PlayGame ());
		displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].ToString ();
	}

	void NewAllStarTeams ()
	{
		Manager.Instance.Teams [3] [0] = new Team (TeamType.AllStar, 0);
		Manager.Instance.Teams [3] [1] = new Team (TeamType.AllStar, 1);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",9");
		sw.Close ();
	}

	public override string ToString ()
	{
		return displayText;
	}
}

public class SigningDeadline : Event
{
	public override void Action ()
	{
		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			while (Manager.Instance.Teams [0] [i].DraftPicks.Count > 0)
			{
				if (Manager.Instance.Players [Manager.Instance.Teams [0] [i].DraftPicks [0]].ConsiderOffer ())
					Manager.Instance.Teams [0] [i].SignDraftPlayer (Manager.Instance.Teams [0] [i].DraftPicks [0]);
				else
					Draft.ReturnToDraft (Manager.Instance.Teams [0] [i].DraftPicks [0]);
				
				Manager.Instance.Teams [0] [i].DraftPicks.RemoveAt (0);
			}
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",10");
		sw.Close ();
	}
}

public class HallOfFameInduction : Event
{
	public override void Action ()
	{
		double bestScore = 0.0;
		int bestIndex = 0;

		for (int i = 0; i < Manager.Instance.HallOfFameCandidates.Count; i++)
		{
			double score;

			if (Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Position.Contains ("P"))
			{
				double era = Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [24] * 27 / (double)Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [20];
				score = (6.0 - era) * 5 + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [20] / (double)8;
			}
			else
			{
				double ops = (Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [3] + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [10]) / (double) (Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [1] + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [10] + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [14]) + (Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [3] + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [5] * 2 + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [6] * 3 + Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [7] * 4) / (double)Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [1];
				score = Manager.Instance.Players [Manager.Instance.HallOfFameCandidates [i]].Stats [0] [7] / 40.0 + ops * 25;
			}

			if (score > bestScore)
			{
				bestScore = score;
				bestIndex = i;
			}
		}

		if (Manager.Instance.HallOfFameCandidates.Count > 0)
		{
			Manager.Instance.HallOfFameInductees.Add (new HallOfFameInductee (Manager.Instance.Year, Manager.Instance.HallOfFameCandidates [bestIndex]));
			Manager.Instance.HallOfFameCandidates.RemoveAt (bestIndex);
		}
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",11");
		sw.Close ();
	}
}

public class NonWaiverTradeDeadline : Event
{
	public override void Action ()
	{
		Manager.Instance.TradeDeadline = TradeDeadline.NonWaiver;
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",12");
		sw.Close ();
	}
}

public class WaiverTradeDeadline : Event
{
	public override void Action ()
	{
		Manager.Instance.TradeDeadline = TradeDeadline.Waiver;
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",13");
		sw.Close ();
	}
}

public class ActiveRosterExpansion : Event
{
	public override void Action ()
	{
		Team.ChangeRosterSize (40);
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",14");
		sw.Close ();
	}
}

public class EndOfYear : Event
{
	public override void Action ()
	{
		Manager.Instance.NewYear ();
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",15");
		sw.Close ();
	}

	public override string ToString ()
	{
		return "";
	}
}

public class EndOfRegularSeason : Event
{
	public override void Action ()
	{
		Debug.Log (ScheduledGame.sb / (float)ScheduledGame.games + " " + ScheduledGame.cs / (float)ScheduledGame.games);
		Team.ChangeRosterSize (25);
		ScheduleWildcardGames ();
	}

	protected void ScheduleWildcardGames ()
	{
		List<Team> result = new List<Team> (0);
		bool noTie = true;

		Manager.Instance.DetermineMVP ();

		result = Manager.Instance.Teams [0].OrderBy (teamX => teamX.League).ThenBy (teamX => teamX.Division).ThenByDescending (teamX => teamX.Wins).ToList ();

		for (int i = 0; i < 6; i++)
		{
			int team = i * 5;

			if (result [team].Wins == result [team + 1].Wins)
			{
				noTie = false;
				Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (result [team], result [team + 1], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));

				if (result [team].Wins == result [team + 2].Wins)
				{
					if (result [team].Wins == result [team + 3].Wins)
					{
						Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (result [team], result [team + 1], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));
						Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new CheckTieWinner (result [team].ID, result [team + 1].ID, result [team + 2].ID, result [team + 3].ID), Manager.Instance.DayIndex + 1);
					}
					else
						Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new CheckTieWinner (result [team].ID, result [team + 1].ID, result [team + 2].ID), Manager.Instance.DayIndex + 1);
				}
				else
					Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new EndOfRegularSeason (), Manager.Instance.DayIndex + 1);
			}
			else
			{
				if (i < 3)
					Manager.Instance.FinalsTeams [i] = result [team].ID;
				else
					Manager.Instance.FinalsTeams [i + 1] = result [team].ID;
			}
		}

		if (noTie)
		{
			noTie = true;

			result.RemoveAt (25);
			result.RemoveAt (20);
			result.RemoveAt (15);
			result.RemoveAt (10);
			result.RemoveAt (5);
			result.RemoveAt (0);

			result = result.OrderBy (teamX => teamX.League).ThenByDescending (teamX => teamX.Wins).ToList ();

			for (int i = 0; i < 2; i++)
			{
				int team = i * 12;

				if (result [team].Wins == result [team + 1].Wins)
				{
					noTie = false;
					Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (result [team], result [team + 1], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));

					if (result [team].Wins == result [team + 2].Wins)
					{
						if (result [team].Wins == result [team + 3].Wins)
						{
							Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (result [team], result [team + 1], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));
							Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new CheckTieWinner (result [team].ID, result [team + 1].ID, result [team + 2].ID, result [team + 3].ID), Manager.Instance.DayIndex + 1);
						}
						else
							Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new CheckTieWinner (result [team].ID, result [team + 1].ID, result [team + 2].ID), Manager.Instance.DayIndex + 1);
					}
					else
						Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new EndOfRegularSeason (), Manager.Instance.DayIndex + 1);
				}
				else
				{
					if (i == 0)
						Manager.Instance.FinalsTeams [3] = result [team].ID;
					else
						Manager.Instance.FinalsTeams [7] = result [team].ID;
				}
			}
		}

		if (noTie)
		{
			for (int i = 0; i < 8; i++)
				Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [i]].ResetWins ();

			Manager.Instance.ScheduleFinalsGames (0, 3, 1, 1);
			Manager.Instance.ScheduleFinalsGames (1, 2, 1, 1);
			Manager.Instance.ScheduleFinalsGames (4, 7, 2, 1);
			Manager.Instance.ScheduleFinalsGames (5, 6, 2, 1);

			Manager.Instance.FinalsRounds.Add (new int [] { 0, 1, 2, 3, 4, 5, 6, 7 });
			Manager.Instance.FinalsRounds.Add (new int [] { -1, -1, -1, -1 });
			Manager.Instance.FinalsRounds.Add (new int [] { -1, -1 });
		}
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",16");
		sw.Close ();
	}
}

public class FinalsCheck : Event
{
	int team1, team2, round;

	public FinalsCheck (int _team1, int _team2, int _round)
	{
		team1 = _team1;
		team2 = _team2;
		round = _round;
	}

	public override void Action ()
	{
		if (Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team1]].Wins < 4 && Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team2]].Wins < 4)
		{
			if ((Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team1]].Wins + Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team2]].Wins) % 2 == 0)
			{
				Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team1]], Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team2]], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 2));
				Manager.Instance.Days [Manager.Instance.DayIndex + 2].AddEvent (new FinalsCheck (team1, team2, round), Manager.Instance.DayIndex + 2);
			}
			else
			{
				Manager.Instance.Days [Manager.Instance.DayIndex + 2].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team2]], Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team1]], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 2));
				Manager.Instance.Days [Manager.Instance.DayIndex + 2].AddEvent (new FinalsCheck (team1, team2, round), Manager.Instance.DayIndex + 2);
			}
		}
		else
		{
			int winner;

			if (Manager.Instance.Teams [0] [Manager.Instance.FinalsTeams [team1]].Wins == 4)
				winner = team1;
			else
				winner = team2;

			round++;

			if (round == 2)
			{
				int index;

				switch (winner)
				{
				case 0:
				case 3:
					index = 0;
					break;
				case 1:
				case 2:
					index = 1;
					break;
				case 4:
				case 7:
					index = 2;
					break;
				default:
					index = 3;
					break;
				}

				Manager.Instance.FinalsRounds [1] [index] = winner;

				if (index % 2 == 0)
				{
					if (Manager.Instance.FinalsRounds [1] [index + 1] != -1)
						Manager.Instance.ScheduleFinalsGames (Manager.Instance.FinalsRounds [1] [index], Manager.Instance.FinalsRounds [1] [index + 1], 1, 2);
				}
				else if (Manager.Instance.FinalsRounds [1] [index - 1] != -1)
					Manager.Instance.ScheduleFinalsGames (Manager.Instance.FinalsRounds [1] [index - 1], Manager.Instance.FinalsRounds [1] [index], 1, 2);
			}
			else if (round == 3)
			{
				if (winner < 4)
				{
					Manager.Instance.FinalsRounds [2] [0] = winner;

					if (Manager.Instance.FinalsRounds [2] [1] != -1)
						Manager.Instance.ScheduleFinalsGames (Manager.Instance.FinalsRounds [2] [0], Manager.Instance.FinalsRounds [2] [1], 1, 3);
				}
				else
				{
					Manager.Instance.FinalsRounds [2] [1] = winner;

					if (Manager.Instance.FinalsRounds [2] [0] != -1)
						Manager.Instance.ScheduleFinalsGames (Manager.Instance.FinalsRounds [2] [0], Manager.Instance.FinalsRounds [2] [1], 1, 3);
				}
			}
			else
				Manager.Instance.Days [Manager.Instance.DayIndex].AddEvent (new WorldChampion (), Manager.Instance.DayIndex);
		}
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",17," + team1 + "," + team2 + "," + round);
		sw.Close ();
	}

	public override string ToString ()
	{
		return "";
	}
}

public class WorldChampion : Event
{
	public override void Action ()
	{
		Manager.Instance.TradeDeadline = TradeDeadline.None;
	}

	public override string ToString ()
	{
		string displayText = Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].ToString () + "\n";

		if (Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].Scores [0] > Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].Scores [1])
			displayText += Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].Shortforms [0] + " Wins!";
		else
			displayText += Manager.Instance.Days [Manager.Instance.DayIndex].SimulatedGames [0].Shortforms [1] + " Wins!";

		return displayText;
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (Manager.Instance.DayIndex + ",18");
		sw.Close ();
	}
}

public class CheckTieWinner : Event
{
	int team1, team2, team3, team4;

	public override void Action ()
	{
		Team.ChangeRosterSize (40);

		int otherTeam;

		if (team4 != -1)
		{
			if (Manager.Instance.Teams [0] [team3].Wins > Manager.Instance.Teams [0] [team4].Wins)
				otherTeam = team3;
			else
				otherTeam = team4;
		}
		else
			otherTeam = team3;

		if (Manager.Instance.Teams [0] [team1].Wins > Manager.Instance.Teams [0] [team2].Wins)
			Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [0] [team1], Manager.Instance.Teams [0] [otherTeam], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));
		else
			Manager.Instance.Days [Manager.Instance.DayIndex + 1].ScheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [0] [team2], Manager.Instance.Teams [0] [otherTeam], GameType.WorldSeries, TeamType.MLB, Manager.Instance.DayIndex + 1));

		Manager.Instance.Days [Manager.Instance.DayIndex + 1].AddEvent (new EndOfRegularSeason (), Manager.Instance.DayIndex + 1);
	}

	public CheckTieWinner (int _team1, int _team2, int _team3, int _team4 = -1)
	{
		team1 = _team1;
		team2 = _team2;
		team3 = _team3;
		team4 = _team4;
	}

	public override void Save (int dayIndex)
	{
		StreamWriter sw = File.AppendText (@"Save\Events.txt");

		sw.WriteLine (dayIndex + ",19," + team1 + "," + team2 + "," + team3 + "," + team4);
		sw.Close ();
	}
}

public enum TradeDeadline
{
	None,
	Waiver,
	NonWaiver
}