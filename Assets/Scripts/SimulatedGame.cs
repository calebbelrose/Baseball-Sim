using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedGame
{
	private int [] scores, teams = new int[2];
	private string [] shortforms = new string [2];
	private GameType gameType;
	private int dayIndex, gameID;
	List<string []> [] [] battingStats;
	List<string []> [] pitchingStats;

	private static int id = 0;

	// 1-Arg Constructor
	public SimulatedGame (string game)
	{
		string [] split = game.Split (',');

		teams [0] = int.Parse (split [0]);
		teams [1] = int.Parse (split [1]);
		scores = new int[2];
		scores [0] = int.Parse (split [2]);
		scores [1] = int.Parse (split [3]);
		shortforms [0] = split [4];
		shortforms [1] = split [5];
		gameType = (GameType)int.Parse (split [6]);
		dayIndex = int.Parse (split [7]);

		battingStats = new List<string []> [2] [];
		pitchingStats = new List<string []> [2];

		for (int i = 0; i < 2; i++)
		{
			string [] lines = File.ReadAllLines (@"Save\BattingStats" + gameID + "-" + i + ".txt");
			int prevIndex = -1, numBatters = 0;

			battingStats [i] = new List<string []> [9];
			pitchingStats [i] = new List<string []> ();

			for (int j = 0; j < lines.Length; j++)
			{
				int index;

				split = lines [j].Split (',');
				index = int.Parse (split [split.Length - 1]);

				if (index == prevIndex)
					numBatters++;
				else
				{
					prevIndex = index;
					numBatters = 0;
					battingStats [i] [index] = new List<string []> ();
				}

				battingStats [i] [index].Add (new string[10]);

				for (int k = 0; k < split.Length - 1; k++)
					battingStats [i] [index] [numBatters] [k] = split [k];
			}

			lines = File.ReadAllLines (@"Save\PitchingStats" + gameID + "-" + i + ".txt");

			for (int j = 0; j < lines.Length; j++)
				pitchingStats [i].Add (lines [j].Split (','));
		}

		Manager.Instance.Days [dayIndex].SimulatedGames.Add (this);
	}

	// 9-Arg Constructor
	public SimulatedGame (int [] _scores, int team1, int team2, string shortform1, string shortform2, GameType _gameType, int _dayIndex, List<string []> [] [] strBattingStats, List<string []> [] strPitchingStats)
	{
		StreamWriter sw;

		gameID = id++;
		scores = _scores;
		teams [0] = team1;
		teams [1] = team2;
		shortforms [0] = shortform1;
		shortforms [1] = shortform2;
		gameType = _gameType;
		dayIndex = _dayIndex;
		battingStats = new List<string []>[2] [];
		pitchingStats = new List<string []>[2];
		strBattingStats.CopyTo (battingStats, 0);
		strPitchingStats.CopyTo (pitchingStats, 0);

		/*sw = File.AppendText (@"Save\SimulatedGames.txt");
		sw.WriteLine (teams [0] + "," + teams [1] + "," + scores [0] + "," + scores [1] + "," + shortforms [0] + "," + shortforms [1] + "," + (int)gameType + "," + dayIndex);
		sw.Close ();

		for (int i = 0; i < 2; i++)
		{
			sw = new StreamWriter (@"Save\BattingStats" + gameID + "-" + i + ".txt");

			for (int j = 0; j < battingStats [i].Length; j++)
				for (int k = 0; k < battingStats [i] [j].Count; k++)
					sw.WriteLine (battingStats [i] [j] [k] [0] + "," + battingStats [i] [j] [k] [1] + "," + battingStats [i] [j] [k] [2] + "," + battingStats [i] [j] [k] [3] + "," + battingStats [i] [j] [k] [4] + "," + battingStats [i] [j] [k] [5] + "," + battingStats [i] [j] [k] [6] + "," + battingStats [i] [j] [k] [7] + "," + battingStats [i] [j] [k] [8] + "," + battingStats [i] [j] [k] [9] + "," + j);

			sw.Close ();
			sw = new StreamWriter (@"Save\PitchingStats" + gameID + "-" + i + ".txt");

			for (int j = 1; j < pitchingStats [i].Count; j++)
				sw.WriteLine (pitchingStats [i] [j] [0] + "," + pitchingStats [i] [j] [1] + "," + pitchingStats [i] [j] [2] + "," + pitchingStats [i] [j] [3] + "," + pitchingStats [i] [j] [4] + "," + pitchingStats [i] [j] [5] + "," + pitchingStats [i] [j] [6] + "," + pitchingStats [i] [j] [7] + "," + pitchingStats [i] [j] [8]);
			
			sw.Close ();
		}*/
	}

	public override string ToString ()
	{
		if (scores [0] > scores [1])
			return shortforms [0] + " " + scores [0] + " - " + scores [1] + " " + shortforms [1];
		else
			return shortforms [1] + " " + scores [1] + " - " + scores [0] + " " + shortforms [0];
	}

	// Getters
	public int [] Scores
	{
		get
		{
			return scores;
		}
	}

	public int [] Teams
	{
		get
		{
			return teams;
		}
	}

	public string [] Shortforms
	{
		get
		{
			return shortforms;
		}
	}
}
