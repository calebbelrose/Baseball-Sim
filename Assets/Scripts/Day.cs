using System;
using System.Collections.Generic;

public class Day
{
	List<ScheduledGame> scheduledGames = new List<ScheduledGame>();
	List<SimulatedGame> simulatedGames = new List<SimulatedGame>();
	List<Event> events = new List<Event>();
	DateTime date;
	public static AllTeams allTeams;

	public DateTime Date
	{
		get
		{
			return date;
		}
	}

	public List<Event> Events
	{
		get
		{
			return events;
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

	public Day(DateTime _date)
	{
		date = _date;
	}

	public static void SetAllTeams (AllTeams _allTeams)
	{
		allTeams = _allTeams;
	}

	public void SimulateDay ()
	{
		while(scheduledGames.Count > 0)
		{
			simulatedGames.Add (scheduledGames [0].PlayGame ());
			scheduledGames.RemoveAt (0);
		}

		for (int i = 0; i < events.Count; i++)
			events [i].Action(allTeams);
	}

	public void AddGame(int team1, int team2)
	{
		scheduledGames.Add(new ScheduledGame(allTeams.teams[team1], allTeams.teams[team2]));
	}

	public void AddEvent(Event newEvent)
	{
		events.Add (newEvent);
	}
}
