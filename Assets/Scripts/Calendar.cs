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

	private Color fadedColour = new Color (Color.white.r, Color.white.g, Color.white.b, 0.5f);
	private int selectedSlot;
	private int currMonth;
	private DateTime startOfYear;

	void Start()
	{
		startOfYear = DateTime.Parse (Manager.Instance.Year + "/01/01");

		CreateSchedule.ScheduleEvents (Manager.Instance.Days, DateTime.IsLeapYear(Manager.Instance.Year));

		currMonth = Manager.Instance.Days[Manager.Instance.DayIndex].Date.Month;
		DisplayCalendar (currMonth);
	}

	void DisplayCalendar(int month)
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();

		int startOfCalendar = (DateTime.Parse(Manager.Instance.Year + "/" + month + "/01") - startOfYear).Days;
		int endOfCalendar = startOfCalendar + DateTime.DaysInMonth(Manager.Instance.Year, month) - 1;
		int maxIndex = Manager.Instance.Days.Count - 1;
		int offset = 0;

		monthText.text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

		while (Manager.Instance.Days [startOfCalendar].Date.DayOfWeek != DayOfWeek.Sunday)
		{
			startOfCalendar--;
			offset++;
		}

		while (endOfCalendar < maxIndex && Manager.Instance.Days [endOfCalendar].Date.DayOfWeek != DayOfWeek.Saturday)
			endOfCalendar++;

		for (int day = startOfCalendar; day <= endOfCalendar; day++)
		{
			GameObject obj = Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, transform);

			if (Manager.Instance.Days[day].Date.Month != month)
			{
				obj.GetComponent<Image> ().color = fadedColour;
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, Manager.Instance.Days[day], this, false);
			}
			else
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, Manager.Instance.Days[day], this, true);

			obj.GetComponent<Slot> ().Display (Manager.Instance.teams);
			
			calendarSlots.Add (obj);
		}

		if (currMonth != Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month)
			selectedSlot = -1;
		else
			SelectSlot(Manager.Instance.Days [Manager.Instance.DayIndex].Date.Day + offset - 1);
	}

	public void Simulate()
	{
		if (selectedSlot != -1)
		{
			DateTime date = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date;

			while (Manager.Instance.Days [Manager.Instance.DayIndex].Date <= date)
				Manager.Instance.SimulateDay ();

			currMonth = Manager.Instance.Days[Manager.Instance.DayIndex].Date.Month;

			DisplayCalendar (Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month);
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
		if(selectedSlot != -1 && selectedSlot < calendarSlots.Count)
			calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.white;
		
		selectedSlot = index;
		calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.yellow;
	}
}
