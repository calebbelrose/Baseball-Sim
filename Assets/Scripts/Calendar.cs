using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Calendar : MonoBehaviour
{
	public GameObject slotPrefab, previousButton, nextButton;
	public static GameObject button = null;
	public List<GameObject> calendarSlots = new List<GameObject> ();
	public Text monthText;

	private Color fadedColour = new Color (Color.white.r, Color.white.g, Color.white.b, 0.5f);
	private int selectedSlot = -1, selectedMonth, currMonth;

	void Start()
	{
		if (button == null)
			button = nextButton;

		currMonth = Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month;
		DisplayCalendar (currMonth);
	}

	// Displays the calendar
	void DisplayCalendar(int month)
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();

		int startOfCalendar = (DateTime.Parse (Manager.Instance.Year + "/" + month + "/01") - Manager.Instance.StartOfYear).Days;
		int endOfCalendar = startOfCalendar + DateTime.DaysInMonth (Manager.Instance.Year, month) - 1;
		int maxIndex = Manager.Instance.Days.Count - 1;
		int offset = 0;

		monthText.text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName (month);

		while (Manager.Instance.Days [startOfCalendar].Date.AddDays(-offset).DayOfWeek != DayOfWeek.Sunday)
			offset++;

		startOfCalendar -= offset;

		while (startOfCalendar < 0)
		{
			GameObject obj = Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, transform);
			Day day = new Day(Manager.Instance.Days [0].Date.AddDays(startOfCalendar));

			obj.GetComponent<Image> ().color = fadedColour;
			obj.GetComponent<Slot> ().Setup (calendarSlots.Count, day, this, false);
			calendarSlots.Add (obj);
			startOfCalendar++;
		}

		while (endOfCalendar < maxIndex && Manager.Instance.Days [endOfCalendar].Date.DayOfWeek != DayOfWeek.Saturday)
			endOfCalendar++;

		for (int day = startOfCalendar; day <= endOfCalendar; day++)
		{
			GameObject obj = Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, transform);

			if (Manager.Instance.Days [day].Date.Month != month)
			{
				obj.GetComponent<Image> ().color = fadedColour;
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, Manager.Instance.Days [day], this, false);
			}
			else
				obj.GetComponent<Slot> ().Setup(calendarSlots.Count, Manager.Instance.Days [day], this, true);

			obj.GetComponent<Slot> ().Display ();
			
			calendarSlots.Add (obj);
		}

		if (currMonth == selectedMonth)
			HighlightSlot ();
		else if (selectedSlot == -1 && currMonth == Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month)
			SelectSlot(Manager.Instance.Days [Manager.Instance.DayIndex].Date.Day + offset - 1);

		offset = 0;

		while (Manager.Instance.Days [endOfCalendar].Date.AddDays(offset).DayOfWeek != DayOfWeek.Saturday)
			offset++;

		for(int i = 0; i < offset; i++)
		{
			GameObject obj = Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, transform);
			Day day = new Day(Manager.Instance.Days [maxIndex].Date.AddDays(i));

			obj.GetComponent<Image> ().color = fadedColour;
			obj.GetComponent<Slot> ().Setup (calendarSlots.Count, day, this, false);
			calendarSlots.Add (obj);
		}
	}

	// Simulates the days until the selected date
	public void Simulate ()
	{
		if (selectedSlot != -1)
		{
			DateTime date = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date;

			if (Manager.Instance.DayIndex < Manager.Instance.FYPDIndex && date > Manager.Instance.Days [Manager.Instance.FYPDIndex].Date)
				date = Manager.Instance.Days [Manager.Instance.FYPDIndex].Date;
			
			while (Manager.Instance.Days [Manager.Instance.DayIndex].Date <= date)
				Manager.Instance.SimulateDay ();

			currMonth = Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month;
			selectedMonth = -1;
			selectedSlot = -1;

			DisplayCalendar (Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month);
		}
	}

	// Displays the next month
	public void NextMonth ()
	{
		if (++currMonth == 12)
			nextButton.SetActive (false);
		else if (currMonth == 2)
			previousButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	// Displays the previous month
	public void PreviousMonth ()
	{
		if (--currMonth == 1)
			previousButton.SetActive (false);
		else if (currMonth == 11)
			nextButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	// Activates the next button
	public static void ActivateNextButton()
	{
		button.SetActive (true);
	}

	// Destroys all of the slots in the calendar
	void DestroySlots()
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();
	}

	// Selects a slot in the calendar
	public void SelectSlot(int index)
	{
		if (selectedSlot != -1 && currMonth == selectedMonth)
			calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.white;
		
		selectedSlot = index;
		selectedMonth = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date.Month;
		HighlightSlot ();
	}

	// Highlights the selected slot in the calendar
	public void HighlightSlot()
	{
		calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.yellow;
	}
}
