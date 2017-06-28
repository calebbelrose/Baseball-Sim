using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallOfFameInductee
{
	int year;
	Player player;

	public HallOfFameInductee(int _year, Player _player)
	{
		year = _year;
		player = _player;
	}

	public override string ToString ()
	{
		return year + " - " + player.Name;
	}
}
