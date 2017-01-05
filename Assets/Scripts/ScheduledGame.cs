using UnityEngine;
using System.Collections;

public class ScheduledGame {
	int team;	// Team to play against
	bool home;	// Whether the game is played at home or away

	// 2-arg constructor
	public ScheduledGame(int t, bool h)
	{
		team = t;
		home = h;
	}
}
