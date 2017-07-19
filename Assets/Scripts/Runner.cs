using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner
{
	int playerID, batterIndex;
	bool error;

	// 2-Arg Constructor
	public Runner (int _playerID, int _batterIndex)
	{
		playerID = _playerID;
		batterIndex = _batterIndex;
		error = false;
	}

	// Getters and Setters
	public int PlayerID
	{
		get
		{
			return playerID;
		}
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
