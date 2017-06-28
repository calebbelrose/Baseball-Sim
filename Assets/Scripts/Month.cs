using System;
using System.Collections.Generic;

public class Year
{
	public List<Day> days = new List<Day>();
	int year;

	public Year()
	{
		year = DateTime.Today.Year;
	}

	public Year(int _year)
	{
		year = _year;
	}

	public int GetYear()
	{
		return year;
	}

	public void NewYear()
	{
		year++;
	}
}
