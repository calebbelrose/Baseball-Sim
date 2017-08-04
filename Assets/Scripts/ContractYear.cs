using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractYear
{
	ContractType type;
	double salary;

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
}

public enum ContractType
{
	ClubOption = 0,
	PlayerOption = 1,
	MutualOption = 2,
	VestingOption = 3,
	NoOption = 4,
	MinorLeague = 5
}