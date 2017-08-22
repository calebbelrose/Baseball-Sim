using System;
using System.Collections;
using System.Collections.Generic;

public class Day
{
	private List<ScheduledGame> scheduledGames = new List<ScheduledGame> ();	// Scheduled games
	private List<SimulatedGame> simulatedGames = new List<SimulatedGame> ();	// Simulated games
	private List<Event> events = new List<Event> ();							// Events
	private DateTime date;														// Date

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
