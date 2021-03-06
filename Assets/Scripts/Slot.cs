﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler
{
	public Text dayText;		// Shows the day of the month
	public Text displayText;	// Shows the day's events

	private int index;			// Day Index
	private Day day;			// Day
	private Calendar calendar;	// Calendar
	private bool canSelect;		// Whether the slot can be selected (if it's in the current month) or not

	// Displays the 
	public void Display ()
	{
		List<string> eventText = new List<string> ();

		dayText.text = day.Date.Day.ToString ();

		for (int i = 0; i < day.ScheduledGames.Count; i++)
			if(day.ScheduledGames [i].TeamType == TeamType.MLB)
			{
				if (day.ScheduledGames [i].Team2.ID == 0)
					eventText.Add ("vs. " + day.ScheduledGames [i].Team1.Shortform);
				else if (day.ScheduledGames [i].Team1.ID == 0)
					eventText.Add ("@" + day.ScheduledGames [i].Team2.Shortform);
			}

		for (int i = 0; i < day.SimulatedGames.Count; i++)
			if(day.SimulatedGames [i].TeamType == TeamType.MLB)
			{
				if (day.SimulatedGames [i].Teams [0] == 0)
				{
					if (day.SimulatedGames [i].Scores [0] > day.SimulatedGames [i].Scores [1])
						eventText.Add ("Won " + day.SimulatedGames [i].Scores [0] + " - " + day.SimulatedGames [i].Scores [1] + " @" + day.SimulatedGames [i].Shortforms [1]);
					else
						eventText.Add ("Lost " + day.SimulatedGames [i].Scores [0] + " - " + day.SimulatedGames [i].Scores [1] + " @" + day.SimulatedGames [i].Shortforms [1]);
				}
				else if (day.SimulatedGames [i].Teams [1] == 0)
				{
					if (day.SimulatedGames [i].Scores [1] > day.SimulatedGames [i].Scores [0])
						eventText.Add ("Won " + day.SimulatedGames [i].Scores [1] + " - " + day.SimulatedGames [i].Scores [0] + " vs. " + day.SimulatedGames [i].Shortforms [0]);
					else
							eventText.Add ("Lost " + day.SimulatedGames [i].Scores [1] + " - " + day.SimulatedGames [i].Scores [0] + " vs. " + day.SimulatedGames [i].Shortforms [0]);
				}
			}

		for (int i = 0; i < day.Events.Count; i++)
			if (day.Events [i].ToString () != "")
				eventText.Add (day.Events [i].ToString ());

		if (eventText.Count > 0)
		{
			displayText.text += eventText [0];

			for (int i = 1; i < eventText.Count; i++)
				displayText.text += "\n" + eventText [i];
		}
	}

	public void Setup (int _index, Day _day, Calendar _calendar, bool _canSelect)
	{
		calendar = _calendar;
		day = _day;
		index = _index;
		canSelect = _canSelect;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (eventData.button == 0 && canSelect)
			calendar.SelectSlot (index);
	}

	// Getter
	public Day Day
	{
		get
		{
			return day;
		}
	}
}
