using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractYear
{
	private ContractType type;	// Type
	private double salary;		// Salary

	// 2-Arg Constructor
	public ContractYear (ContractType _type, double _salary)
	{
		type = _type;
		salary = _salary;
	}

	// Getters
	public ContractType Type
	{
		get
		{
			return type;
		}
	}

	public double Salary
	{
		get
		{
			return salary;
		}
	}

	// Override ToString
	public override string ToString ()
	{
		string text = type.ToString ();

		for (int i = 1; i < text.Length; i++)
			if (char.IsUpper (text [i]))
				text = text.Insert (i++, " ");
		
		return text;
	}
}

public enum ContractType
{
	NoOption = 0,
	PlayerOption = 1,
	ClubOption = 2,
	MutualOption = 3,
	VestingOption = 4,
	MinorLeague = 5
}