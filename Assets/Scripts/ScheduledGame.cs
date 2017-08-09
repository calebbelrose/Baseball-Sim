using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ScheduledGame
{
    static int id = 0;
    public static int singles = 0, doubles = 0, triples = 0, homeruns = 0, strikeouts = 0, walks = 0, games = 0, innings = 0, runs = 0, lineouts = 0, groundouts = 0, flyouts = 0, popouts = 0, hbp = 0, errors = 0, sb = 0, cs = 0;
    Team [] teams = new Team[2];
    bool trackStats;
    GameType gameType;
    TeamType teamType;
    int dayIndex, gameID;
    List<int> [] pitchers = new List<int> [2];
    List<int []> [] pitchingStats = new List<int []>[2];
    List<int []> [] [] battingStats = new List<int []>[2] [];
    int [] batters = new int [] { 0, 0 };

	// 0-Arg Constructor
    public ScheduledGame ()
    {
		string str = File.ReadAllLines (@"Save\ScheduledGame" + (gameID = id++) + ".txt") [0];
        string [] split = str.Split (',');

        if (bool.Parse (split [5]))
        {
            int typeIndex = int.Parse (split [3]);

            teamType = (TeamType)typeIndex;
            teams [0] = Manager.Instance.Teams [typeIndex] [int.Parse (split [0])];
            teams [1] = Manager.Instance.Teams [typeIndex] [int.Parse (split [1])];
            gameType = (GameType)int.Parse (split [2]);
            dayIndex = int.Parse (split [4]);

            Manager.Instance.Days [dayIndex].ScheduledGames.Add (this);
        }
    }

    // 2-Arg constructor
    public ScheduledGame (Team _team1, Team _team2, GameType _gameType, TeamType _teamType, int _dayIndex)
    {
		StreamWriter sw;

        gameID = id++;
        teams = new Team [2];
        teams [0] = _team1;
        teams [1] = _team2;
        gameType = _gameType;
        teamType = _teamType;
        dayIndex = _dayIndex;

		sw = new StreamWriter (@"Save\ScheduledGame" + gameID + ".txt");
		sw.WriteLine (teams [0].ID +  "," +  teams [1].ID +  "," + (int)gameType +  "," + (int)teamType +  "," +  dayIndex +  "," +  true);
		sw.Close ();
    }

	// Plays the scheduled game
    public SimulatedGame PlayGame ()
    {
        int inning = 1, winningTeam = -1, winningPitcher = -1, losingPitcher = -1;
		int []  scores =  new int [] { 0, 0 },  relievers =  new int [] { 0, 0 };
        List<bool> [] hit = new List<bool>[2];
        bool [] noHitter = new bool [] {true, true};
        float random;
        bool designatedHitter;
		List<string []> [] [] strBattingStats = new List<string []>[2] [];
		List<string []> [] strPitchingStats = new List<string []>[2];
		StreamWriter sw;

        if (gameType == GameType.RegularSeason)
            trackStats = true;
        else
            trackStats = false;

        if (trackStats)
            games += 2;

		if (teams [0].AutomaticRoster)
			teams  [0].SetRoster ();

		if (teams [1].AutomaticRoster)
			teams  [1].SetRoster ();
        
        if (teamType == TeamType.MLB && teams [1].League == League.National)
            designatedHitter = false;
        else
            designatedHitter = true;

        for (int j = 0; j < 2; j++)
        {
            hit [j] = new List<bool> ();
            pitchers [j] = new List<int> ();
            pitchingStats [j] = new List<int []> ();
            battingStats [j] = new List<int []>[9];
			ChangePitcher (j, teams [j].SP [teams [j].CurrStarter]);
            Manager.Instance.Players [pitchers [j] [0]].Stats [0] [17]++;

            if (designatedHitter)
                Manager.Instance.Players [pitchers [j] [0]].Stats [0] [0]++;
            else
            {
                int dhIndex = teams [j].Positions.IndexOf ("DH");
                teams [j].Batters.RemoveAt (dhIndex);
                teams [j].Positions.RemoveAt (dhIndex);
                teams [j].Batters.Add (new List<int> ());
                teams [j].Positions.Add ("SP");
                teams [j].Batters[8].Add (pitchers [j] [0]);
            }

            for (int k = 0; k < teams [j].Batters.Count; k++)
            {
                Manager.Instance.Players [teams [j].Batters [k] [0]].Stats [0] [0]++;
                battingStats [j] [k] = new List<int []> ();
                battingStats [j] [k].Add (new int[10]);
                battingStats [j] [k] [0] [0] = Manager.Instance.Players [teams [j].Batters [k] [0]].ID;
            }

            for (int k = 0; k < teams [j].Players.Count; k++)
                hit [j].Add (false);
        }

        while (inning <= 9 || scores [0] == scores [1])
        {
            if (trackStats)
                innings++;
            for (int j = 0; j < 2; j++)
            {
                /*if (i == 0)
            {
                if (j == 0)
                    Debug.Log ("Top of the " + (inning + 1));
                else
                    Debug.Log ("Bottom of the " + (inning + 1));
            }*/
                int outs = 0, thisTeam = j, otherTeam = (j + 1) % 2, strikes = 0, balls = 0, prevPitch = -1, pitchSplit = -1, batSplit = -1;
                float multiplier = 1.0f;
                Runner [] bases = new Runner[4] { null, null, null, null };

                while (outs < 3)
                {
                    bool strike, swing, atBatter = false, wildPitch = false, error;
                    float thisEye, thisContact, thisPower, thisAccuracy, thisLocation, accuracy = Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [6], pitchEffectiveness;
                    int pitchIndex;

                    if (balls == 0 && strikes == 0)
                    {
						if (inning >= 9 && outs == 0 && !pitchers [thisTeam].Contains(teams [thisTeam].CP) && pitchers [thisTeam] [0] != teams [thisTeam].SP [teams [thisTeam].CurrStarter] && scores [thisTeam] > scores [otherTeam])
                        {
                            Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [0]++;
                            prevPitch = -1;

                            if (designatedHitter)
                                ChangeBatter (thisTeam, 8, pitchers [thisTeam] [0]);
                        }
						else if (((!noHitter [thisTeam] && Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [9] <= 0 && strikes == 0 && balls == 0) || !Manager.Instance.Players [pitchers [thisTeam] [0]].IsPitcher) && relievers [thisTeam] < 2)
                        {
                            if (pitchers [thisTeam] [0] == teams [thisTeam].SP [teams [thisTeam].CurrStarter] && inning > 6 && scores [otherTeam] <= 3)
                                Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [28]++;

                            Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [0]++;
                            relievers [thisTeam]++;

                            if (designatedHitter)
                                ChangeBatter (thisTeam, 8, pitchers [thisTeam] [0]);
                        }

                        if (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [9] <= 0)
                        {
                            if (Random.value > Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [8] / 100.0f)
                                Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Injure ();

							if(teams [otherTeam].OffensiveSubstitutes.Count > 0)
							{
								ChangeBatter (otherTeam, batters [otherTeam], teams [otherTeam].OffensiveSubstitutes [0]);
								teams [otherTeam].DefensiveSubstitutes.Remove (teams [otherTeam].OffensiveSubstitutes [0]);
								teams [otherTeam].PinchRunners.Remove (teams [otherTeam].OffensiveSubstitutes [0]);
								teams [otherTeam].OffensiveSubstitutes.RemoveAt (0);
                            }
                        }

						if (innings >= 7 && scores [otherTeam] <= scores [thisTeam] && outs < 2)
						{
							if (teams [otherTeam].PinchRunners.Count > 0)
							{
								if (bases [1] != null && Manager.Instance.Players [bases [1].PlayerID].Skills [3] < Manager.Instance.Players [teams [otherTeam].PinchRunners [0]].Skills [3])
								{
									ChangeBatter (otherTeam, batters [otherTeam], teams [otherTeam].PinchRunners [0]);
									bases [1].PlayerID = teams [otherTeam].PinchRunners [0];
									teams [otherTeam].DefensiveSubstitutes.Remove (teams [otherTeam].PinchRunners [0]);
									teams [otherTeam].OffensiveSubstitutes.Remove (teams [otherTeam].PinchRunners [0]);
									teams [otherTeam].PinchRunners.RemoveAt (0);
								}

								if (teams [otherTeam].PinchRunners.Count > 0)
								{
									int index;

									if (bases [3] != null)
										index = 3;
									else if (bases [2] != null)
										index = 2;
									else
										index = 1;

									if (index != 1 && Manager.Instance.Players [bases [index].PlayerID].Skills [3] < Manager.Instance.Players [teams [otherTeam].PinchRunners [0]].Skills [3])
									{
										ChangeBatter (otherTeam, batters [otherTeam], teams [otherTeam].PinchRunners [0]);
										bases [index].PlayerID = teams [otherTeam].PinchRunners [0];
										teams [otherTeam].DefensiveSubstitutes.Remove (teams [otherTeam].PinchRunners [0]);
										teams [otherTeam].OffensiveSubstitutes.Remove (teams [otherTeam].PinchRunners [0]);
										teams [otherTeam].PinchRunners.RemoveAt (0);
									}
								}
							}
						}

                        bases [0] = new Runner (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].ID, batters [otherTeam]);

						if (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Bats ==  'S')
						{
							if (Manager.Instance.Players [pitchers [thisTeam] [0]].Throws == 'S')
								pitchSplit = 1;
							else
								pitchSplit = Manager.Instance.Players [pitchers [thisTeam] [0]].PitchSplit;

							batSplit = pitchSplit;
							multiplier =  1.0f;
						}
						else if (Manager.Instance.Players [pitchers [thisTeam] [0]].Throws ==  'S' ||  Manager.Instance.Players [pitchers [thisTeam] [0]].Throws == Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Bats)
						{
							batSplit = Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].BatSplit;
							pitchSplit = (batSplit + 1) % 2;
							multiplier =  0.9f;
						}
						else
						{
							batSplit = Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].BatSplit;
							pitchSplit = Manager.Instance.Players [pitchers [thisTeam] [0]].PitchSplit;
							multiplier =  1.0f;
						}
                    }

					for (int k = 2; k > 0; k--)
						if (bases [k] != null && bases [k + 1] == null) 
						{
							int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ((k + 1) + "B")] [0];
							int catcherIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("C")] [0];

							if (Manager.Instance.Players [bases [k].PlayerID].Skills [3] > (Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [3] + Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [2]) * 0.6f && Manager.Instance.Players [bases [k].PlayerID].Skills [3] > (Manager.Instance.Players [catcherIndex].Skills [5] + Manager.Instance.Players [catcherIndex].Skills [3]) * 0.6f)
							{
								if(Random.value * 100.0f > Manager.Instance.Players [bases [k].PlayerID].Skills [3])
								{
									Manager.Instance.Players [bases [k].PlayerID].Stats [0] [13]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [12]++;
									outs++;
									pitchingStats [thisTeam] [0] [1]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [14]++;

									if (trackStats)
										cs++;
								}
								else if (Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [3] > Manager.Instance.Players [bases [k].PlayerID].Skills [3] || Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [2] > Manager.Instance.Players [bases [k].PlayerID].Skills [3])
								{
									if (Random.value > Manager.Instance.Players [pitchers [thisTeam] [0]].FieldingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
									{
										Manager.Instance.Players [bases [k].PlayerID].Stats [0] [12]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [11]++;
										bases [k + 1] = bases [k];

										if (trackStats)
											sb++;
									}
									else
									{
										Manager.Instance.Players [bases [k].PlayerID].Stats [0] [13]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [12]++;
										outs++;
										pitchingStats [thisTeam] [0] [1]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [14]++;

										if (trackStats)
											cs++;
									}
								}
								else if (Manager.Instance.Players [catcherIndex].Skills [5] > Manager.Instance.Players [bases [k].PlayerID].Skills [3] || Manager.Instance.Players [catcherIndex].Skills [3] > Manager.Instance.Players [bases [k].PlayerID].Skills [3])
								{
									if (Random.value > Manager.Instance.Players [catcherIndex].CatchingChance || Random.value > Manager.Instance.Players [catcherIndex].FieldingChance || Random.value > Manager.Instance.Players [fielderIndex].CatchingChance)
									{
										Manager.Instance.Players [bases [k].PlayerID].Stats [0] [12]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [11]++;
										bases [k + 1] = bases [k];

										if (trackStats)
											sb++;
									}
									else
									{
										Manager.Instance.Players [bases [k].PlayerID].Stats [0] [13]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [12]++;
										outs++;
										pitchingStats [thisTeam] [0] [1]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [14]++;

										if (trackStats)
											cs++;
									}
								}
								else
								{
									Manager.Instance.Players [bases [k].PlayerID].Stats [0] [12]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [11]++;
									bases [k + 1] = bases [k];

									if (trackStats)
										sb++;
								}

								bases [k] = null;
							}
						}
                    
                    Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [9]--;
                    Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [9]--;

                    thisLocation = Random.value;

                    if (balls == 3 && strikes <= 1)
                    {
                        if (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [1] * multiplier > 75 && (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [0] * multiplier > 75 || Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [3] > 75))
                            strike = false;
                        else
                            strike = true;
                    }
                    else if (strikes == 2 && balls <= 3 && Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [2] * multiplier < 75)
                        strike = false;
                    else
                    {
                        if (thisLocation < 0.5f)
                            strike = true;
                        else
                            strike = false;
                    }

                    if (strike)
                        thisLocation *= 2;
                    else
                        thisLocation /= 2;

                    thisAccuracy = Random.value;

                    if (thisAccuracy > accuracy * (Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [9] / Manager.Instance.Players [pitchers [thisTeam] [0]].Skills [10] / 5.0f + 0.95f))
                    {
                        if (thisAccuracy > 0.975f)
                        {
                            strike = false;
                            if (Random.value > 0.5f)
                                wildPitch = true;
                            else
                                atBatter = true;
                        }
                        else
                            strike = !strike;
                    }

                    thisEye = Random.value * Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [2] * multiplier / 100.0f;
                    random = Random.value;
                    pitchIndex = Random.Range (0, Manager.Instance.Players [pitchers [thisTeam] [0]].Pitches.Count);

                    if (pitchIndex != prevPitch)
                        pitchEffectiveness = Random.value * Manager.Instance.Players [pitchers [thisTeam] [0]].Pitches [pitchIndex].Effectiveness / 100.0f;
                    else
                        pitchEffectiveness = Random.value * Manager.Instance.Players [pitchers [thisTeam] [0]].Pitches [pitchIndex].Effectiveness / 100.0f * 0.9f;

                    prevPitch = pitchIndex;

                    if (thisEye > pitchEffectiveness)
                    {
                        if (strike)
                            swing = true;
                        else
                            swing = false;
                    }
                    else
                    {
                        if (thisEye > 0.5f)
                            swing = false;
                        else if (Random.value > 0.5f)
                            swing = true;
                        else
                            swing = false;
                    }

                    if (swing)
                    {
                        if (atBatter)
                            balls = 4;
                        else
                        {
                            thisContact = Random.value * Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [1] * multiplier / 100.0f;

                            if (!strike)
                                thisContact *= (1 - thisLocation) * 2 + thisLocation;

                            if (thisContact > pitchEffectiveness)
                            {
                                int numBases;
                                bool fly = false;
                                float powerRandom = Random.value;

                                thisPower = powerRandom * Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [0] * multiplier / 100.0f;

                                if (powerRandom > 0.95f || thisPower > 0.9f)
                                {
                                    /*if (i == 0)
                                Debug.Log (strikes + "-" + balls + " Homerun");*/
                                    if (trackStats)
                                        homeruns++;
                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [7]++;
                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [8] += 4;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [7] += 4;
                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [3]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [2]++;
                                    battingStats [otherTeam] [batters [otherTeam]] [0] [3]++;
                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [22]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [16]++;
									pitchingStats [thisTeam] [0] [2]++;
                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [25]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [19]++;
									pitchingStats [thisTeam] [0] [7]++;
                                    hit [otherTeam] [batters [otherTeam]] = true;
                                    noHitter [thisTeam] = false;
                                    numBases = 4;
                                    error = false;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [1]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [0]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [21]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [15]++;
									battingStats [otherTeam] [batters [otherTeam]] [0] [1]++;
                                }
                                else
                                {
                                    thisPower = thisPower * 2 / 3 + Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Skills [3] / 300.0f;

                                    if (thisPower > 0.8f)
                                    {
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

                                        /*if (i == 0)
                                    Debug.Log (strikes + "-" + balls + " Triple");*/
                                        if (trackStats)
                                            triples++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [6]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [5]++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [8] += 3;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [7] += 3;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [3]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [2]++;
                                        battingStats [otherTeam] [batters [otherTeam]] [0] [3]++;
                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [22]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [16]++;
										pitchingStats [thisTeam] [0] [2]++;
                                        hit [otherTeam] [batters [otherTeam]] = true;
                                        noHitter [thisTeam] = false;

                                        if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                        {
                                            numBases = 4;
                                            Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
                                            if (trackStats)
                                                errors++;
                                            error = true;
                                        }
                                        else
                                        {
                                            fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("2B")] [0];

                                            if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                            {
                                                numBases = 4;
                                                Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
                                                if (trackStats)
                                                    errors++;
                                                error = true;
                                            }
                                            else
                                            {
                                                fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("C")] [0];

                                                if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance)
                                                {
                                                    numBases = 4;
                                                    Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
                                                    if (trackStats)
                                                        errors++;
                                                    error = true;
                                                }
                                                else
                                                {
                                                    numBases = 3;
                                                    error = false;
                                                }
                                            }
                                        }
                                    }
                                    else if (thisPower > 0.6f)
                                    {
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

                                        /*if (i == 0)
                                    Debug.Log (strikes + "-" + balls + " Flyout");*/
                                        if (trackStats)
                                            flyouts++;

                                        if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                        {
                                            numBases = 2;
											ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                            error = true;
                                        }
                                        else
                                        {
                                            fly = true;
                                            outs++;
											pitchingStats [thisTeam] [0] [1]++;
											for (int k = 1; k < bases.Length; k++)
												if (bases[k] != null)
											 		battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                                            Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                                            numBases = 1;
                                            error = false;
                                        }
                                    }
                                    else if (thisPower > 0.575f)
                                    {
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

                                        /*if (i == 0)
                                    Debug.Log (strikes + "-" + balls + " Double");*/
                                        if (trackStats)
                                            doubles++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [5]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [4]++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [8] += 2;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [7] += 2;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [3]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [2]++;
                                        battingStats [otherTeam] [batters [otherTeam]] [0] [3]++;
                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [22]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [16]++;
										pitchingStats [thisTeam] [0] [2]++;
                                        hit [otherTeam] [batters [otherTeam]] = true;
                                        noHitter [thisTeam] = false;

                                        if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                        {
                                            numBases = 3;
                                            Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
                                            if (trackStats)
                                                errors++;
                                            error = true;
                                        }
                                        else
                                        {
                                            numBases = 2;
                                            error = false;
                                        }
                                    }
                                    else if (thisPower > 0.45f)
                                    {
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomInfielder ())] [0];

                                        /*if (i == 0)
                                    Debug.Log (strikes + "-" + balls + " Popout");*/
                                        if (trackStats)
                                            popouts++;

                                        if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                        {
                                            numBases = 1;
											ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                            error = true;
                                        }
                                        else
                                        {
                                            numBases = 0;
                                            outs++;
											pitchingStats [thisTeam] [0] [1]++;
											for (int k = 1; k < bases.Length; k++)
												if (bases[k] != null)
													battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                                            Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                                            error = false;
                                        }
                                    }
                                    else if (thisPower > 0.35f)
                                    {
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomFielder ())] [0];

                                        /*if (i == 0)
                                    Debug.Log (strikes + "-" + balls + " Single");*/
                                        if (trackStats)
                                            singles++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [4]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [3]++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [8]++;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [3]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [2]++;
                                        battingStats [otherTeam] [batters [otherTeam]] [0] [3]++;
                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [22]++;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [16]++;
										pitchingStats [thisTeam] [0] [2]++;
                                        hit [otherTeam] [batters [otherTeam]] = true;
                                        noHitter [thisTeam] = false;

                                        if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                        {
                                            numBases = 2;
                                            Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
                                            if (trackStats)
                                                errors++;
                                            error = true;
                                        }
                                        else
                                        {
                                            numBases = 1;
                                            error = false;
                                        }
                                    }
                                    else if (thisPower > 0.3f)
                                    {
                                        string fielderPosition = RandomFielder ();
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0];

                                        if (fielderPosition.Substring (1, 1) == "F")
                                        {
                                            if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                            {
                                                numBases = 1;
												ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                                error = true;
                                            }
                                            else
                                            {
                                                numBases = 0;
                                                outs++;
												pitchingStats [thisTeam] [0] [1]++;
												for (int k = 1; k < bases.Length; k++)
													if (bases[k] != null)
														battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                                                Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                                                error = false;
                                            }
                                        }
                                        else if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance)
                                        {
                                            numBases = 1;
											ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                            error = true;
                                        }
                                        else
                                        {
                                            numBases = 0;
                                            outs++;
											pitchingStats [thisTeam] [0] [1]++;
											for (int k = 1; k < bases.Length; k++)
												if (bases[k] != null)
													battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                                            Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                                            error = false;
                                        }
                                        /*if (i == 0)
                                    Debug.Log ("Lineout");*/
                                        if (trackStats)
                                            lineouts++;
                                    }
                                    else
                                    {
                                        string fielderPosition = RandomFielder ();
                                        int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0];

                                        if (bases [1] != null)
                                        {
                                            if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                            {
                                                numBases = 1;
												ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                                error = true;
                                            }
                                            else
                                            {
                                                if (fielderPosition == "2B")
                                                    fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("SS")] [0];
                                                else
                                                    fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("2B")] [0];

                                                if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
                                                {
                                                    numBases = 1;
													ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                                    error = true;
                                                }
                                                else
                                                {
                                                    if (fielderPosition == "1B")
                                                        fielderIndex = pitchers [thisTeam] [0];
                                                    else
                                                        fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("1B")] [0];

                                                    if (Random.value > Manager.Instance.Players [ (fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("1B")] [0])].FieldingChance)
                                                    {
                                                        numBases = 1;
														ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                                        error = true;
                                                    }
                                                    else
                                                    {
                                                        numBases = 0;
                                                        outs += 2;
														pitchingStats [thisTeam] [0] [1] += 2;
                                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20] += 2;
                                                        bases [1] = null;
                                                        error = false;
                                                    }
                                                }
                                            }

                                            /*if (i == 0)
                                        Debug.Log ("Double Play");*/
                                        }
                                        else
                                        {
                                            if ((fielderPosition != "1B" && (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)) || Random.value > Manager.Instance.Players [teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0]].CatchingChance)
                                            {
                                                numBases = 1;
												ReachedOnError (teams [otherTeam].Players [batters [otherTeam]], fielderIndex, pitchers [thisTeam] [0], batSplit);
                                                error = true;
                                            }
                                            else
                                            {
                                                numBases = 0;
                                                outs++;
												pitchingStats [thisTeam] [0] [1]++;
												for (int k = 1; k < bases.Length; k++)
													if (bases[k] != null)
														battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                                                Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                                                error = false;
                                            }

                                            /*if (i == 0)
                                        Debug.Log ("Groundout");*/
                                            if (trackStats)
                                                groundouts++;
                                        }
                                    }

                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [1]++;
                                    battingStats [otherTeam] [batters [otherTeam]] [0] [1]++;
                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [21]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [15]++;
                                }

                                if (outs < 3)
                                {
                                    bool advanced = false;

                                    for (int k = 3; k >= 0; k--)
                                        if (bases [k] != null)
                                        {
                                            advanced = true;

                                            Manager.Instance.Players [bases [k].PlayerID].Skills [9] -= numBases;

                                            if (error)
                                            {
                                                int newBase = k + numBases;

                                                if (newBase > 4)
                                                {
                                                    scores [otherTeam]++;

                                                    if (bases [k] != null && !bases [k].Error)
                                                    {
                                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [9]++;
														Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [8]++;
                                                        Manager.Instance.Players [bases [k].PlayerID].Stats [0] [2]++;
														Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [1]++;
                                                        battingStats [otherTeam] [bases [k].BatterIndex] [0] [2]++;
                                                        battingStats [otherTeam] [batters [otherTeam]] [0] [4]++;
                                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [24]++;
														Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [18]++;
														pitchingStats [thisTeam] [0] [4]++;
                                                    }

                                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [23]++;
													Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [17]++;
													pitchingStats [thisTeam] [0] [3]++;
                                                }
                                                else if (newBase > 3)
                                                {
                                                    scores [otherTeam]++;
                                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [23]++;
													Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [17]++;
													pitchingStats [thisTeam] [0] [3]++;
                                                }
                                                else
                                                   (bases [k + numBases] = bases [k]).Error = true;

                                                bases [k] = null;
                                            }
                                            else if (k + numBases > 3)
                                            {
                                                scores [otherTeam]++;
                                                if (bases [k] != null && !bases [k].Error)
                                                {
                                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [9]++;
													Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [8]++;
                                                    Manager.Instance.Players [bases [k].PlayerID].Stats [0] [2]++;
													Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [1]++;
                                                    battingStats [otherTeam] [bases [k].BatterIndex] [0] [2]++;
                                                    battingStats [otherTeam] [batters [otherTeam]] [0] [4]++;
                                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [24]++;
													Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [18]++;
													pitchingStats [thisTeam] [0] [4]++;
                                                }

                                                Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [23]++;
												Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [17]++;
												pitchingStats [thisTeam] [0] [3]++;
                                            }
                                            else
                                                bases [k + numBases] = bases [k];
                                        }
                                    if (fly && advanced)
                                    {
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [14]++;
										Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [13]++;
										//0atBats, 1runs, 2hits, 4singles, 5doubles, 6triples, 7homeruns, 8totalBases, 9runsBattedIn, 10walks, 10-11strikeouts, 11-12stolenBases, 12-13caughtStealing, 13-14sacrifices, 14-20inningsPitched, 15-21atBatsAgainst, 16-22hitsAgainst, 17-23runsAgainst, 18-24earnedRuns, 19-25homerunsAgainst, 20-26walksAgainst, 21-27strikeoutsAgainst, 22-31reachedOnError, 23-32hitByPitch;
                                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [1]--;
                                        battingStats [otherTeam] [batters [otherTeam]] [0] [1]--;
                                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [21]--;
										Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [15]--;
                                    }
                                }

                                batters [otherTeam] = (batters [otherTeam] + 1) % 9;
                                strikes = 0;
                                balls = 0;
                            }
                            else
                                strikes++;
                        }
                    }
                    else
                    {
                        if (strike)
                            strikes++;
                        else
                            balls++;
                    }

                    if (wildPitch)
                    {
                        for (int k = 3; k >= 1; k--)
                            if (bases [k] != null)
                            {
                                Manager.Instance.Players [bases [k].PlayerID].Skills [9]--;

                                if (k + 1 > 3)
                                {
                                    scores [otherTeam]++;
                                    Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [9]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [8]++;
                                    Manager.Instance.Players [bases [k].PlayerID].Stats [0] [2]++;
									Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [1]++;
                                    battingStats [otherTeam] [bases [k].BatterIndex] [0] [2]++;
                                    battingStats [otherTeam] [batters [otherTeam]] [0] [4]++;
                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [23]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [17]++;
									pitchingStats [thisTeam] [0] [3]++;
                                    Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [24]++;
									Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [18]++;
									pitchingStats [thisTeam] [0] [4]++;
                                }
                                else
                                    bases [k + 1] = bases [k];

                                bases [k] = null;
                            }
                    }

                    if (strikes == 3)
                    {
                        outs++;
						pitchingStats [thisTeam] [0] [1]++;
						for (int k = 1; k < bases.Length; k++)
							if (bases[k] != null)
								battingStats [otherTeam] [batters [otherTeam]] [0] [7]++;
                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [20]++;
                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [21]++;
						Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [15]++;
                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [1]++;
                        battingStats [otherTeam] [batters [otherTeam]] [0] [1]++;
                        /*if (i == 0)
                        Debug.Log (strikes + "-" + balls + " Strikeout");*/
                        if (trackStats)
                            strikeouts++;

                        Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [11]++;

						Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [10]++;
                        battingStats [otherTeam] [batters [otherTeam]] [0] [6]++;
                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [27]++;
						Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [21]++;
						pitchingStats [thisTeam] [0] [6]++;
                        strikes = 0;
                        balls = 0;
						batters [otherTeam] = (batters [otherTeam] + 1) % 9;
                    }
                    else if (balls == 4)
                    {
                        int currBase = 0;

                        while (currBase < bases.Length && bases [currBase] != null)
                            currBase++;
                    
                        if (currBase == 4)
                        {
                            scores [otherTeam]++;
                            Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [9]++;
							Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [8]++;
                            Manager.Instance.Players [bases [3].PlayerID].Stats [0] [2]++;
                            battingStats [otherTeam] [bases [3].BatterIndex] [0] [2]++;
                            battingStats [otherTeam] [batters [otherTeam]] [0] [4]++;
                        }
                        else
                        {
                            for (int k = currBase; k > 0; k--)
                            {
                                bases [k] = bases [k - 1];
                                bases [k - 1] = null;
                            }
                        }

                        /*if (i == 0)
                        Debug.Log (strikes + "-" + balls + " Walk");*/
                    
                        if (atBatter)
                        {
                            Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [32]++;
							Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [23]++;
                            if (trackStats)
                                hbp++;
                        }
                        else
                        {
                            Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].Stats [0] [10]++;
							Manager.Instance.Players [teams [otherTeam].Players [batters [otherTeam]]].StatSplits [0] [pitchSplit] [9]++;
                            battingStats [otherTeam] [batters [otherTeam]] [0] [5]++;
                            if (trackStats)
                                walks++;
                        }
                    
                        Manager.Instance.Players [pitchers [thisTeam] [0]].Stats [0] [26]++;
						Manager.Instance.Players [pitchers [thisTeam] [0]].StatSplits [0] [batSplit] [20]++;
						pitchingStats [thisTeam] [0] [5]++;

                        batters [otherTeam] = (batters [otherTeam] + 1) % 9;
                        strikes = 0;
                        balls = 0;
                    }

					if ((inning > 5 || Manager.Instance.Players [pitchers [otherTeam] [0]].ID != teams [otherTeam].SP [teams [otherTeam].CurrStarter]) && scores [otherTeam] > scores [thisTeam] && otherTeam != winningTeam)
					{
						winningPitcher = Manager.Instance.Players [pitchers [otherTeam] [0]].ID;
						winningTeam = otherTeam;
						losingPitcher = Manager.Instance.Players [pitchers [thisTeam] [0]].ID;
					}
                }
            }

            inning++;
        }

        //Home team gains revenue and pays expenses
        teams [1].AddRevenue (teams [0].Hype);
        teams [0].SubtractExpenses ();

		Manager.Instance.Players [winningPitcher].Stats [0] [15]++;
		Manager.Instance.Players [losingPitcher].Stats [0] [16]++;

        if (trackStats)
            runs += scores [0] + scores [1];

		if (winningTeam == 0)
        {
            teams [0].Win ();
            teams [1].Loss ();
        }
        else
        {
            teams [1].Win ();
            teams [0].Loss ();
        }

        for (int j = 0; j < 2; j++)
        {
			strPitchingStats [j] = new List<string []> ();
			strBattingStats [j] = new List<string []> [9];

			if (!teams [j].AutomaticRoster)
				for (int k = 0; k < 9; k++)
					while (teams [j].Batters [k].Count > 1)
						teams [j].Batters [k].RemoveAt (1);

			/*for (int k = 0; k < battingStats [j].Length; k++)
			{
				strBattingStats [j] [k] = new List<string []> ();

				for (int l = 0; l < battingStats [j] [k].Count; l++)
				{
					strBattingStats [j] [k].Add (new string[10]);

					for (int m = 0; m < 8; m++)
						strBattingStats [j] [k] [l] [m] = battingStats [j] [k] [l] [m].ToString ();

					strBattingStats [j] [k] [l] [8] = Manager.Instance.Players [battingStats [j] [k] [l] [0]].BA.ToString ("F3");
					strBattingStats [j] [k] [l] [9] = (Manager.Instance.Players [battingStats [j] [k] [l] [0]].OBP + Manager.Instance.Players [battingStats [j] [k] [l] [0]].SLUG).ToString ("F3");
				}
			}
			
			for (int k = 0; k < pitchingStats [j].Count; k++)
			{
				strPitchingStats [j].Add (new string[9]);
				strPitchingStats [j] [k] [0] = pitchingStats [j] [k] [0].ToString ();
				strPitchingStats [j] [k] [1] = pitchingStats [j] [k] [1] / 3 + "." + pitchingStats [j] [k] [1] % 3;

				for (int l = 2; l < 8; l++)
					strPitchingStats [j] [k] [l] = pitchingStats [j] [k] [l].ToString ();
				
				strPitchingStats [j] [k] [8] = Manager.Instance.Players [pitchingStats [j] [k] [0]].ERA.ToString ("F");
			}*/

            if (noHitter [j])
                Manager.Instance.Players [pitchers [j] [0]].Stats [0] [35]++;
        
            for (int k = 0; k < teams [j].Players.Count; k++)
            {
                if (hit [j] [k])
                {
                    Manager.Instance.Players [teams [j].Players [k]].Stats [0] [30]++;

                    if (Manager.Instance.Players [teams [j].Players [k]].Stats [0] [30] > Manager.Instance.Players [teams [j].Players [k]].Stats [0] [33])
                    {
                        Manager.Instance.Players [teams [j].Players [k]].Stats [0] [33] = Manager.Instance.Players [teams [j].Players [k]].Stats [0] [30];
                        Manager.Instance.Players [teams [j].Players [k]].Stats [0] [34] = Manager.Instance.Year;

						if (Manager.Instance.Players [teams [j].Players [k]].Stats [0] [33] > Manager.Instance.longestHitStreak)
						{
							Manager.Instance.longestHitStreak = Manager.Instance.Players [teams [j].Players [k]].Stats [0] [33];
							Manager.Instance.hitStreakYear = Manager.Instance.Players [teams [j].Players [k]].Stats [0] [34];
							Manager.Instance.hitStreakName = Manager.Instance.Players [teams [j].Players [k]].Name;
							PlayerPrefs.SetInt ("LongestHitStreak", Manager.Instance.longestHitStreak);
							PlayerPrefs.SetInt ("HitStreakYear", Manager.Instance.hitStreakYear);
							PlayerPrefs.SetString ("HitStreakName", Manager.Instance.hitStreakName);
						}
                    }
                }
                else
                    Manager.Instance.Players [teams [j].Players [k]].Stats [0] [30] = 0;

                if (Manager.Instance.Players [teams [j].Players [k]].InjuryLength > 0)
                    Manager.Instance.Players [teams [j].Players [k]].ReduceInjuryLength ();

                Manager.Instance.Players [teams [j].Players [k]].SaveStats ();
            }
        }

        if (pitchers [0] [0] == teams [0].SP [teams [0].CurrStarter])
            Manager.Instance.Players [pitchers [0] [0]].Stats [0] [29]++;

        if (pitchers [1] [0] == teams [1].SP [teams [1].CurrStarter])
            Manager.Instance.Players [pitchers [1] [0]].Stats [0] [29]++;

        teams [0].UseStarter ();
        teams [1].UseStarter ();

		if (teams[0].Type == TeamType.MLB)
		{
			if (teams [0].ID == 0)
	            DisplayScore.Display (teams [0].Wins, teams [0].Losses, scores [0], scores [1]);
	        else if (teams [1].ID == 0)
	            DisplayScore.Display (teams [1].Wins, teams [1].Losses, scores [1], scores [0]);
		}

		/*sw = new StreamWriter (@"Save\ScheduledGame" + gameID + ".txt");
		sw.WriteLine (teams [0].ID + "," + teams [1].ID + "," + (int)gameType + "," + (int)teamType + "," + dayIndex + "," + false);
		sw.Close ();*/

		return new SimulatedGame (scores, teams [0].ID, teams [1].ID, teams [0].Shortform, teams [1].Shortform, gameType, dayIndex, strBattingStats, strPitchingStats);
    }

	// Determines whether the game contained a team
    public bool ContainsTeam (int team)
    {
        if (team == teams [0].ID || team == teams [1].ID)
            return true;
        else
            return false;
    }

	// Determines whether a team was playing at home
    public bool IsHomeGame (int team)
    {
        if (team == teams [0].ID)
            return false;
        else
            return true;
    }

	// Selects a random outfielder
    string RandomOutfielder ()
    {
        float random = Random.value;

        if (random <= 1 / 3)
            return "LF";
        else if (random <= 2 / 3)
            return "CF";
        else
            return "RF";
    }

	// Selects a random infielder
    string RandomInfielder ()
    {
        float random = Random.value;

        if (random <= 0.25f)
            return "1B";
        else if (random <= 0.5f)
            return "2B";
        else if (random <= 0.75f)
            return "3B";
        else
            return "SS";
    }

	// Selects a random fielder
    string RandomFielder ()
    {
        float random = Random.value;

        if (random <= 1 / 7)
            return "LF";
        else if (random <= 2 / 7)
            return "CF";
        else if (random <= 3 / 7)
            return "RF";
        else if (random <= 4 / 7)
            return "1B";
        else if (random <= 5 / 7)
            return "2B";
        else if (random <= 6 / 7)
            return "3B";
        else
            return "SS";
    }

	// Adds a reached on error to the stats
    void ReachedOnError (int batterIndex, int fielderIndex, int pitcherIndex, int batSplit)
    {
		Manager.Instance.Players [fielderIndex].Stats [0] [36]++;
		Manager.Instance.Players [batterIndex].Stats [0] [31]++;
		Manager.Instance.Players [pitcherIndex].StatSplits [0] [batSplit] [22]++;

		if (trackStats)
			errors++;
    }

	// Changes a team's batter
    void ChangeBatter (int teamIndex, int batterIndex, int playerIndex)
    {
        teams [teamIndex].Batters [batterIndex].Insert (0, playerIndex);
        battingStats [teamIndex] [batterIndex].Insert (0, new int[10]);
        battingStats [teamIndex] [batterIndex] [0] [0] = Manager.Instance.Players [teams [teamIndex].Batters [batterIndex] [0]].ID;
		Manager.Instance.Players [playerIndex].Stats [0] [0]++;
    }

	// Changes a team's pitcher
    int ChangePitcher (int teamIndex, int playerIndex)
    {
        pitchers [teamIndex].Insert (0, playerIndex);
        pitchingStats [teamIndex].Insert (0, new int[9]);
        pitchingStats [teamIndex] [0] [0] = Manager.Instance.Players [pitchers [teamIndex] [0]].ID;
		Manager.Instance.Players [playerIndex].Stats [0] [0]++;

		if (Manager.Instance.Players [playerIndex].Throws == 'L')
			return 1;
		else 
			return 0;
    }

	// Getters
	public Team Team1
	{
		get
		{
			return teams [0];
		}
	}

	public Team Team2
	{
		get
		{
			return teams [1];
		}
	}
}

public enum GameType
{
    RegularSeason = 0,
    SpringTraining = 1,
    WorldSeries = 2,
    AllStar = 3,
    Futures = 4,
    WorldBaseballClassic = 5
} 