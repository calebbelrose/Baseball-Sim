using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner
{
	public int PlayerID;

	private int batterIndex;
	private bool error;

	// 2-Arg Constructor
	public Runner (int _playerID, int _batterIndex)
	{
		PlayerID = _playerID;
		batterIndex = _batterIndex;
		error = false;
	}

	public int BatterIndex
	{
		get
		{
			return batterIndex;
		}
	}

	public bool Error
	{
		get
		{
			return error;
		}

		set
		{
			error = value;
		}
	}
}
