using System;
using System.Collections.Generic;

public class Day
{
	List<ScheduledGame> scheduledGames = new List<ScheduledGame> ();
	List<SimulatedGame> simulatedGames = new List<SimulatedGame> ();
	List<Event> events = new List<Event> ();
	DateTime date;

	// Simulates all of the day's games and events
	public void SimulateDay ()
	{
		while (scheduledGames.Count > 0)
		{
			simulatedGames.Add (scheduledGames [0].PlayGame ());
			scheduledGames.RemoveAt (0);
		}

		for (int i = 0; i < events.Count; i++)
			events [i].Action ();

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
		{
			for (int j = 0; j < Manager.Instance.Teams [0] [i].Players.Count; j++)
			{
				if (Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [9] != Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [10])
				{
					Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [9] = Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [9] + 20;

					if (Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [9] > Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [10])
						Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [9] = Manager.Instance.Players [Manager.Instance.Teams [0] [i].Players [j]].Skills [10];
				}
			}
		}
	}

	// Schedules a game for the day
	public void AddGame (int team1, int team2)
	{
		scheduledGames.Add (new ScheduledGame (Manager.Instance.Teams [0] [team1], Manager.Instance.Teams [0] [team2], GameType.RegularSeason, TeamType.MLB, date.DayOfYear - 1));
	}

	// Schedules an event for the day
	public void AddEvent (Event newEvent, int dayIndex)
	{
		events.Add (newEvent);
		newEvent.Save (dayIndex);
	}

	// Getters and Setters
	public DateTime Date
	{
		get
		{
			return date;
		}
	}

	public List<ScheduledGame> ScheduledGames
	{
		get
		{
			return scheduledGames;
		}
	}

	public List<SimulatedGame> SimulatedGames
	{
		get
		{
			return simulatedGames;
		}
	}

	public List<Event> Events
	{
		get
		{
			return events;
		}
	}

	public Day (DateTime _date)
	{
		date = _date;
	}
}
