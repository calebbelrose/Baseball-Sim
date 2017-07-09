using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedGame
{
	private int[] scores, teams;
	private string[] shortforms;

	public int[] Scores
	{
		get
		{
			return scores;
		}
	}

	public int[] Teams
	{
		get
		{
			return teams;
		}
	}

	public string[] Shortforms
	{
		get
		{
			return shortforms;
		}
	}

	public SimulatedGame(int[] _scores, int team1, int team2, string shortform1, string shortform2)
	{
		teams = new int[2];
		shortforms = new string[2];
		scores = _scores;
		teams[0] = team1;
		teams[1] = team2;
		shortforms[0] = shortform1;
		shortforms[1] = shortform2;
	}

	public override string ToString()
	{
		if (scores [0] > scores [1])
			return shortforms[0] + " " + scores[0] + " - " + scores[1] + " " + shortforms[1];
		else
			return shortforms[1] + " " + scores[1] + " - " + scores[0] + " " + shortforms[0];
	}
}
