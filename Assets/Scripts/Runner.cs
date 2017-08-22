using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner
{
	public int PlayerID;		// Player ID

	private int batterIndex;	// Index in the batting order
	private bool error;			// Whether the batter advanced on an error or not

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
