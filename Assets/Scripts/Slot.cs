using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler
{
	public Text dayText, eventText;

	private int index;
	private Day day;
	private Calendar calendar;
	private bool canSelect;

	public Day Day
	{
		get
		{
			return day;
		}
	}

	public void Display(List<Team> teams)
	{
		dayText.text = day.Date.Day.ToString();

		for (int i = 0; i < day.ScheduledGames.Count; i++)
			if (day.ScheduledGames [i].ContainsTeam (0))
			{
				if (day.ScheduledGames [i].IsHomeGame (0))
					eventText.text = "vs. " + day.ScheduledGames[i].Team1.shortform;
				else
					eventText.text = "@" + day.ScheduledGames[i].Team2.shortform;
			}

		for (int i = 0; i < day.SimulatedGames.Count; i++)
			if (day.SimulatedGames [i].Teams [0] == 0)
			{
				if (day.SimulatedGames [i].Scores [0] > day.SimulatedGames [i].Scores [1])
					eventText.text = "Won " + day.SimulatedGames [i].Scores [0] + " - " + day.SimulatedGames [i].Scores [1] + " @" + day.SimulatedGames [i].Shortforms [1];
				else
					eventText.text = "Lost " + day.SimulatedGames [i].Scores [0] + " - " + day.SimulatedGames [i].Scores [1] + " @" + day.SimulatedGames [i].Shortforms [1];
			}
			else if (day.SimulatedGames [i].Teams [1] == 0)
			{
				if (day.SimulatedGames [i].Scores [1] > day.SimulatedGames [i].Scores [0])
					eventText.text = "Won " + day.SimulatedGames [i].Scores [1] + " - " + day.SimulatedGames [i].Scores [0] + " vs. " + day.SimulatedGames [i].Shortforms [0];
				else
					eventText.text = "Lost " + day.SimulatedGames [i].Scores [1] + " - " + day.SimulatedGames [i].Scores [0] + " vs. " + day.SimulatedGames [i].Shortforms [0];
			}
		
		for (int i = 0; i < day.Events.Count; i++)
			eventText.text += "\n" + day.Events[i].ToString();
	}

	public void Setup(int _index, Day _day, Calendar _calendar, bool _canSelect)
	{
		calendar = _calendar;
		day = _day;
		index = _index;
		canSelect = _canSelect;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if(eventData.button == 0 && canSelect)
			calendar.SelectSlot (index);
	}
}
