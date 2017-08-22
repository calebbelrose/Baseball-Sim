using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Calendar : MonoBehaviour
{
	public GameObject SlotPrefab;																// Slot prefab
	public GameObject PreviousButton;															// Previous button
	public GameObject NextButton;																// Next button
	public Text MonthText;																		// Text to display name of month
	public Text SimulateText;																	// Text for the simulate button

	private Color fadedColour = new Color (Color.white.r, Color.white.g, Color.white.b, 0.5f);	// Faded colour for days in the calendar that aren't in the displayed month
	private int selectedSlot = -1;																// Selected slot
	private int selectedMonth;																	// Month of the selected slot
	private int currMonth;																		// Currently displayed month
	private DateTime simulationDate;															// Date to simulate to
	private Thread thread;																		// Thread for performing tasks in the background, used for simulating
	private bool notThreading = true;															// Whether the thread is done or still running
	private List<GameObject> calendarSlots = new List<GameObject> ();							// Slots in the calendar

	public static GameObject Button = null;														// Static reference to next button

	void Start ()
	{
		if (Button == null)
			Button = NextButton;

		simulationDate = Manager.Instance.Days [Manager.Instance.DayIndex].Date;
		currMonth = simulationDate.Month;

		if (currMonth == 1)
			PreviousButton.SetActive (false);
		else if (currMonth == 12)
			NextButton.SetActive (false);
		
		DisplayCalendar (currMonth);
	}

	// Simulates days until the simulation date
	void Update ()
	{
		if (simulationDate > Manager.Instance.Days [Manager.Instance.DayIndex].Date)
		{
			if (notThreading)
			{
				notThreading = false;

				thread = new Thread (() => {
					Simulate ();
				});
				thread.Start ();
			}
		}
		else
			SimulateText.text = "Simulate";
	}

	// Displays the calendar
	void DisplayCalendar (int month)
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();

		int startOfCalendar = (DateTime.Parse (Manager.Instance.Year + "/" + month + "/01") - Manager.Instance.StartOfYear).Days;
		int endOfCalendar = startOfCalendar + DateTime.DaysInMonth (Manager.Instance.Year, month) - 1;
		int maxIndex = Manager.Instance.Days.Count - 1;
		int offset = 0;

		MonthText.text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName (month);

		while (Manager.Instance.Days [startOfCalendar].Date.AddDays (-offset).DayOfWeek != DayOfWeek.Sunday)
			offset++;

		startOfCalendar -= offset;

		while (startOfCalendar < 0)
		{
			GameObject obj = Instantiate (SlotPrefab, Vector3.zero, Quaternion.identity, transform);
			Day day = new Day (Manager.Instance.Days [0].Date.AddDays (startOfCalendar));

			obj.GetComponent<Image> ().color = fadedColour;
			obj.GetComponent<Slot> ().Setup (calendarSlots.Count, day, this, false);
			calendarSlots.Add (obj);
			startOfCalendar++;
		}

		while (endOfCalendar < maxIndex && Manager.Instance.Days [endOfCalendar].Date.DayOfWeek != DayOfWeek.Saturday)
			endOfCalendar++;

		for (int day = startOfCalendar; day <= endOfCalendar; day++)
		{
			GameObject obj = Instantiate (SlotPrefab, Vector3.zero, Quaternion.identity, transform);

			if (Manager.Instance.Days [day].Date.Month != month)
			{
				obj.GetComponent<Image> ().color = fadedColour;
				obj.GetComponent<Slot> ().Setup (calendarSlots.Count, Manager.Instance.Days [day], this, false);
			}
			else
				obj.GetComponent<Slot> ().Setup (calendarSlots.Count, Manager.Instance.Days [day], this, true);

			obj.GetComponent<Slot> ().Display ();
			
			calendarSlots.Add (obj);
		}

		if (currMonth == selectedMonth)
			HighlightSlot ();
		else if (selectedSlot == -1 && currMonth == Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month)
			SelectSlot (Manager.Instance.Days [Manager.Instance.DayIndex].Date.Day + offset - 1);

		offset = 0;

		while (Manager.Instance.Days [endOfCalendar].Date.AddDays (offset).DayOfWeek != DayOfWeek.Saturday)
			offset++;

		for (int i = 0; i < offset; i++)
		{
			GameObject obj = Instantiate (SlotPrefab, Vector3.zero, Quaternion.identity, transform);
			Day day = new Day (Manager.Instance.Days [maxIndex].Date.AddDays (i));

			obj.GetComponent<Image> ().color = fadedColour;
			obj.GetComponent<Slot> ().Setup (calendarSlots.Count, day, this, false);
			calendarSlots.Add (obj);
		}
	}

	public void SetSimulationDate ()
	{
		if (selectedSlot != -1)
		{
			simulationDate = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date.AddDays (1);
			SimulateText.text = "Stop Simulation";

			if (Manager.Instance.DayIndex < Manager.Instance.FYPDIndex && simulationDate > Manager.Instance.Days [Manager.Instance.FYPDIndex].Date)
				simulationDate = Manager.Instance.Days [Manager.Instance.FYPDIndex].Date;
		}
	}

	// Simulates the days until the selected date
	void Simulate ()
	{
		Manager.Instance.SimulateDay ();
		currMonth = Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month;
		selectedMonth = -1;
		selectedSlot = -1;
		Manager.Instance.ExecuteOnMainThread.Enqueue (() => {
			MainThreadDisplay ();
		});
	}

	void MainThreadDisplay ()
	{
		DisplayCalendar (Manager.Instance.Days [Manager.Instance.DayIndex].Date.Month);
		notThreading = true;
	}

	// Displays the next month
	public void NextMonth ()
	{
		if (++currMonth == 12)
			NextButton.SetActive (false);
		else if (currMonth == 2)
			PreviousButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	// Displays the previous month
	public void PreviousMonth ()
	{
		if (--currMonth == 1)
			PreviousButton.SetActive (false);
		else if (currMonth == 11)
			NextButton.SetActive (true);
		
		DisplayCalendar (currMonth);
	}

	// Activates the next button
	public static void ActivateNextButton ()
	{
		Button.SetActive (true);
	}

	// Destroys all of the slots in the calendar
	void DestroySlots ()
	{
		foreach (GameObject slot in calendarSlots)
			Destroy (slot.gameObject);

		calendarSlots.Clear ();
	}

	// Selects a slot in the calendar
	public void SelectSlot (int index)
	{
		if (selectedSlot != -1 && currMonth == selectedMonth)
			calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.white;
	
		selectedSlot = index;
		selectedMonth = calendarSlots [selectedSlot].GetComponent<Slot> ().Day.Date.Month;
		HighlightSlot ();
	}

	// Highlights the selected slot in the calendar
	public void HighlightSlot ()
	{
		calendarSlots [selectedSlot].GetComponent<Image> ().color = Color.yellow;
	}
}
