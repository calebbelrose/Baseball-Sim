using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner
{
	int playerID;
	bool error;

	public int PlayerID
	{
		get
		{
			return playerID;
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

	public Runner (int _playerID)
	{
		playerID = _playerID;
		error = false;
	}
}
