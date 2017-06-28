using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Calendar : MonoBehaviour
{
	public GameObject slotPrefab, previousButton, nextButton;
	public List<GameObject> calendarSlots = new List<GameObject> ();
	public Text monthText;

	private AllTeams allTeams = null;
	private Color fadedColour = new Color (Color.white.r, Color.white.g, Color.white.b, 0.5f);
	private int selectedSlot;
	private int currMonth;
	private DateTime startOfYear;

	void Start()
	{
		if(allTeams == null)
			allTeams = GameObject.Find ("_Manager").GetComponent<AllTeams> ();
		
		startOfYear = DateTime.Parse (allTeams.Year + "/01/01");

		CreateSchedule.ScheduleEvents (allTeams.Days, DateTime.IsLeapYear(allTeams.Year));

		currMonth = allTeams.Days[allTeams.DayIndex].Date.Month;
		DisplayCalendar (currMonth);
	}

	void DisplayCalendar(int month)
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();

		int startOfCalendar = (DateTime.Parse(allTeams.Year + "/" + month + "/01") - startOfYear).Days;
		int endOfCalendar = startOfCalendar + DateTime.DaysInMonth(allTeams.Year, month) - 1;
		int maxIndex = allTeams.Days.Count - 1;
		int offset = 0;

		monthText.text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

		while (allTeams.Days [startOfCalendar].Date.DayOfWeek != DayOfWeek.Sunday)
		{
			startOfCalendar--;
			offset++;
		}

		while (endOfCalendar < maxIndex && allTeams.Days [endOfCalendar].Date.DayOfWeek != DayOfWeek.Saturday)
			endOfCalendar++;

		for (int day = startOfCalendar; day <= endOfCalendar; day++)
		{
			GameObject obj = Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, transform);

			if (allTeams.Days[day].Date.Month != month)
			{
				obj.GetComponent<Image> ().color = fadedColour;
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, allTeams.Days[day], this, false);
			}
			else
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, allTeams.Days[day], this, true);

			obj.GetComponent<Slot> ().Display (allTeams.teams);
			
			calendarSlots.Add (obj);
		}

		if (currMonth != allTeams.Days [allTeams.DayIndex].Date.Month)
			selectedSlot = -1;
		else
			SelectSlot(allTeams.Days [allTeams.DayIndex].Date.Day + offset - 1);
	}

	public void Simulate()
	{
		if (selectedSlot != -1)
		{
			DateTime date = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date;

			while (allTeams.Days [allTeams.DayIndex].Date <= date)
				allTeams.SimulateDay ();

			DisplayCalendar (allTeams.Days [allTeams.DayIndex].Date.Month);
		}
	}

	public void NextMonth()
	{
		if (++currMonth == 12)
			nextButton.SetActive (false);
		else if (currMonth == 2)
			previousButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	public void PreviousMonth()
	{
		if (--currMonth == 1)
			previousButton.SetActive (false);
		else if (currMonth == 11)
			nextButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	void DestroySlots()
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();
	}

	public void SelectSlot(int index)
	{
		if(selectedSlot != -1)
			calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.white;
		
		selectedSlot = index;
		calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.yellow;
	}
}
