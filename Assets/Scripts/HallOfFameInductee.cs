using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallOfFameInductee
{
	private int year;		// Year inducted
	private int playerID;	// Player ID

	public HallOfFameInductee (int _year, int _playerID)
	{
		year = _year;
		playerID = _playerID;
	}

	public override string ToString ()
	{
		return year + " - " + Manager.Instance.Players [playerID].Name;
	}
}
