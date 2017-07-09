using System;
using System.Collections.Generic;

public class Day
{
	List<ScheduledGame> scheduledGames = new List<ScheduledGame>();
	List<SimulatedGame> simulatedGames = new List<SimulatedGame>();
	List<Event> events = new List<Event>();
	DateTime date;

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

	public void SimulateDay ()
	{
		while(scheduledGames.Count > 0)
		{
			simulatedGames.Add (scheduledGames [0].PlayGame ());
			scheduledGames.RemoveAt (0);
		}

		for (int i = 0; i < events.Count; i++)
			events [i].Action();

		for (int i = 0; i < Manager.Instance.teams.Count; i++)
		{
			for (int j = 0; j < Manager.Instance.teams [i].players.Count; j++)
			{
				Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [8] = Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [8] + 20;

				if (Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [8] > Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [9])
					Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [8] = Manager.Instance.Players[Manager.Instance.teams [i].players [j]].skills [9];
			}
		}
	}

	public void AddGame(int team1, int team2)
	{
		scheduledGames.Add(new ScheduledGame(Manager.Instance.teams[team1], Manager.Instance.teams[team2]));
	}

	public void AddEvent(Event newEvent)
	{
		events.Add (newEvent);
	}
}
