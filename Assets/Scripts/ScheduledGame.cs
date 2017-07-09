using UnityEngine;
using System.Collections.Generic;

public class ScheduledGame
{
	Team[] teams;

	public Team Team1
	{
		get
		{
			return teams[0];
		}
	}

	public Team Team2
	{
		get
		{
			return teams[1];
		}
	}

	// 2-arg constructor
	public ScheduledGame(Team _team1, Team _team2)
	{
		teams = new Team[2];
		teams[0] = _team1;
		teams[1] = _team2;
	}

	public SimulatedGame PlayGame()
	{
		int inning = 1;
		int[] scores = new int[] { 0, 0 };
		int[] batters = new int[] { 0, 0 };
		int[] relievers = new int[] { 0, 0 };
		int[] pitchers = new int[] { 0, 0 };
		List<bool>[] hit = new List<bool>[2];
		bool[] noHitter = new bool[] {true, true};

		if(teams[0].MinorLeagueIndexes.Count > 0)
			teams[0].AutomaticRoster ();

		if(teams[1].MinorLeagueIndexes.Count > 0)
			teams[1].AutomaticRoster ();

		for (int j = 0; j < 2; j++)
		{
			hit [j] = new List<bool> ();
			pitchers [j] = teams[j].SP [teams[j].CurrStarter];
			Manager.Instance.Players[pitchers [j]].stats[0][0]++;
			Manager.Instance.Players[pitchers [j]].stats[0][17]++;

			if (teams [j].League == League.National)
			{
				List<int> batterSlot = new List<int> ();
				batterSlot.Add (pitchers [j]);
				teams [j].Batters.Add (batterSlot);
			}

			for (int k = 0; k < teams [j].Batters.Count; k++)
				Manager.Instance.Players[teams [j].Batters [k][0]].stats[0][0]++;

			for (int k = 0; k < teams [j].players.Count; k++)
				hit [j].Add (false);
		}

		while (inning <= 9 || scores [0] == scores [1]) {
			for (int j = 0; j < 2; j++)
			{
				/*if (i == 0)
                {
                    if (j == 0)
                        Debug.Log("Top of the " + (inning + 1));
                    else
                        Debug.Log("Bottom of the " + (inning + 1));
                }*/
				int outs = 0, thisTeam = j, otherTeam = (j + 1) % 2, strikes = 0, balls = 0;
				float multiplier = 1.0f;
				Runner[] bases = new Runner[4] { null, null, null, null };

				if (inning == 9 && pitchers [thisTeam] != teams [thisTeam].SP [teams [thisTeam].CurrStarter] && scores [thisTeam] > scores [otherTeam])
				{
					pitchers [thisTeam] = teams [thisTeam].CP [0];
					Manager.Instance.Players[pitchers [thisTeam]].stats[0][0]++;

					if(teams[thisTeam].League == League.National)
						teams[j].Batters[8].Add(pitchers [thisTeam]);
				}

				while (outs < 3)
				{
					bool strike, swing, atBatter = false, wildPitch = false, error;
					float thisEye, thisContact, thisPower, thisPitchPower, thisMovement, thisAccuracy, thisLocation;
					float accuracy = Manager.Instance.Players[pitchers [thisTeam]].skills [6];

					if (!noHitter [thisTeam] && Manager.Instance.Players[pitchers [thisTeam]].skills [8] <= 0 && strikes == 0 && balls == 0 && relievers [thisTeam] < 2)
					{
						if (pitchers [thisTeam] == teams [thisTeam].SP [teams[thisTeam].CurrStarter] && inning > 6 && scores [otherTeam] <= 3)
							Manager.Instance.Players[pitchers [thisTeam]].stats[0][26]++;
						
						pitchers [thisTeam] = teams [thisTeam].RP [relievers [thisTeam]];
						Manager.Instance.Players[pitchers [thisTeam]].stats[0][0]++;
						relievers [thisTeam]++;

						if(teams[thisTeam].League == League.National)
							teams[j].Batters[8].Add(pitchers [thisTeam]);
					}

					if (Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [8] == 0)
					{
						int index = 0;
						bool substituteNotFound = true;

						do
						{
							if(((Manager.Instance.Players[teams[otherTeam].players[index]].position.Length == 1 || Manager.Instance.Players[teams[otherTeam].players[index]].position.Substring(1,1) != "P") && !teams[otherTeam].IsBatter(Manager.Instance.Players[teams[otherTeam].players[index]].ID)) && Manager.Instance.Players[teams[otherTeam].players[index]].skills [8] > 0)
								substituteNotFound = false;
							else
								index++;
						} while(substituteNotFound && index < teams[otherTeam].players.Count);

						if (!substituteNotFound)
						{
							teams [otherTeam].Batters [batters [otherTeam]].Insert (0, index);
							Manager.Instance.Players[teams [otherTeam].players [index]].stats[0][0]++;
						}
					}

					if (balls == 0 && strikes == 0)
					{
						bases [0] = new Runner (batters [otherTeam]);

						if (Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Bats == 'S')
							multiplier = 1.0f;
						else if (Manager.Instance.Players [pitchers [thisTeam]].Throws == 'S' || Manager.Instance.Players [pitchers [thisTeam]].Throws == Manager.Instance.Players [teams [otherTeam].Batters [batters [otherTeam]] [0]].Bats)
							multiplier = 0.9f;
						else
							multiplier = 1.0f;
					}
						
					Manager.Instance.Players[pitchers [thisTeam]].skills [8]--;
					Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [8]--;

					thisLocation = Random.value;

					if (balls == 3 && strikes <= 1)
					{
						if (Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [1] * multiplier > 75 && (Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [0] * multiplier > 75 || Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [3] > 75))
						{
							thisLocation /= 2;
							strike = false;
						}
						else
						{
							thisLocation *= 2;
							strike = true;
						}
					}
					else if (strikes == 2 && balls <= 3 && Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [2] * multiplier < 75)
					{
						strike = false;
						thisLocation /= 2;
					}
					else
					{
						if (thisLocation < 0.5f)
							strike = true;
						else
							strike = false;
					}

					thisAccuracy = Random.value;

					if (thisAccuracy > accuracy * (Manager.Instance.Players[pitchers [thisTeam]].skills [8] / Manager.Instance.Players[pitchers [thisTeam]].skills [9] / 5.0f + 0.95f))
							{
						if (thisAccuracy > 0.975f)
						{
							strike = false;
							if (Random.value > 0.5f)
								wildPitch = true;
							else
								atBatter = true;
						}
						else
							strike = !strike;
					}

					thisEye = Random.value * Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [2] * multiplier / 100.0f;
					thisPitchPower = Random.value * Manager.Instance.Players[pitchers [thisTeam]].skills [5] / 100.0f;
					thisMovement = Random.value * Manager.Instance.Players[pitchers [thisTeam]].skills [7] / 100.0f;

					if (thisEye > thisPitchPower && thisEye > thisMovement)
					{
						if (strike)
							swing = true;
						else
							swing = false;
					}
					else
					{
						if (thisEye > 0.5f)
							swing = false;
						else if (thisMovement > 0.5f && thisPitchPower > 0.5f)
							swing = true;
						else if (Random.value > 0.5f)
							swing = true;
						else
							swing = false;
					}

					if (swing)
					{
						if (atBatter)
							balls = 4;
						else
						{
							thisContact = Random.value * Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [1] * multiplier / 100.0f;
							if (!strike)
								thisContact *= (1 - thisLocation) * 2 + thisLocation;

							if (thisContact > thisMovement && thisContact > thisPitchPower)
							{
								int numBases;
								bool fly = false;
								float powerRandom = Random.value;

								thisPower = powerRandom * Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [0] * multiplier / 100.0f;

								if (powerRandom > 0.95f || thisPower > 0.8f)
								{
									/*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Homerun");*/
									Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][7]++;
									Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][8] += 4;
									Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][3]++;
									Manager.Instance.Players[pitchers [thisTeam]].stats[0][22]++;
									Manager.Instance.Players[pitchers [thisTeam]].stats[0][25]++;
									hit [otherTeam] [batters [otherTeam]] = true;
									noHitter [thisTeam] = false;
									numBases = 4;
									error = false;
								}
								else
								{
									thisPower = thisPower * 2 / 3 + Manager.Instance.Players[teams [otherTeam].Batters [batters [otherTeam]][0]].skills [3] / 300.0f;

									if (thisPower > 0.75f)
									{
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Triple");*/
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][6]++;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][8] += 3;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][3]++;
										Manager.Instance.Players[pitchers [thisTeam]].stats[0][22]++;
										hit [otherTeam] [batters [otherTeam]] = true;
										noHitter [thisTeam] = false;

										if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance|| Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
										{
											numBases = 4;
											Manager.Instance.Players [fielderIndex].stats [0] [36]++;
											error = true;
										}
										else
										{
											fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("2B")] [0];

											if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
											{
												numBases = 4;
												Manager.Instance.Players [fielderIndex].stats [0] [36]++;
												error = true;
											}
											else
											{
												fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("C")] [0];

												if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance)
												{
													numBases = 4;
													Manager.Instance.Players [fielderIndex].stats [0] [36]++;
													error = true;
												}
												else
												{
													numBases = 3;
													error = false;
												}
											}
										}
									}
									else if (thisPower > 0.65f)
									{
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Flyout");*/

										if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
										{
											numBases = 2;
											ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
											error = true;
										}
										else
										{
											fly = true;
											outs++;
											Manager.Instance.Players[pitchers [thisTeam]].stats[0][20]++;
											numBases = 1;
											error = false;
										}
									}
									else if (thisPower > 0.6f)
									{
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomOutfielder ())] [0];

										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Double");*/
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][5]++;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][8] += 2;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][3]++;
										Manager.Instance.Players[pitchers [thisTeam]].stats[0][22]++;
										hit [otherTeam] [batters [otherTeam]] = true;
										noHitter [thisTeam] = false;

										if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
										{
											numBases = 3;
											Manager.Instance.Players [fielderIndex].stats [0] [36]++;
											error = true;
										}
										else
										{
											numBases = 2;
											error = false;
										}
									}
									else if (thisPower > 0.45f)
									{
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomInfielder ())] [0];

										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Popout");*/

										if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
										{
											numBases = 1;
											ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
											error = true;
										}
										else
										{
											numBases = 0;
											outs++;
											Manager.Instance.Players[pitchers [thisTeam]].stats[0][20]++;
											error = false;
										}
									}
									else if (thisPower > 0.35f)
									{
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (RandomFielder ())] [0];

										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Single");*/
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][4]++;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][8]++;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][3]++;
										Manager.Instance.Players[pitchers [thisTeam]].stats[0][22]++;
										hit [otherTeam] [batters [otherTeam]] = true;
										noHitter [thisTeam] = false;

										if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
										{
											numBases = 2;
											Manager.Instance.Players [fielderIndex].stats [0] [36]++;
											error = true;
										}
										else
										{
											numBases = 1;
											error = false;
										}
									}
									else if (thisPower > 0.25f)
									{
										string fielderPosition = RandomFielder ();
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0];

										if (fielderPosition.Substring (1, 1) == "F")
										{
											if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
											{
												numBases = 1;
												ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
												error = true;
											}
											else
											{
												numBases = 0;
												outs++;
												Manager.Instance.Players [pitchers [thisTeam]].stats [0] [20]++;
												error = false;
											}
										}
										else if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance)
										{
											numBases = 1;
											ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
											error = true;
										}
										else
										{
											numBases = 0;
											outs++;
											Manager.Instance.Players [pitchers [thisTeam]].stats [0] [20]++;
											error = false;
										}
										/*if (i == 0)
                                        Debug.Log("Lineout");*/
									}
									else
									{
										string fielderPosition = RandomFielder ();
										int fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0];

										if (bases [1] != null)
										{
											if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
											{
												numBases = 1;
												ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
												error = true;
											}
											else
											{
												if (fielderPosition == "2B")
													fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("SS")] [0];
												else
													fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("2B")] [0];

												if (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)
												{
													numBases = 1;
													ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
													error = true;
												}
												else
												{
													if (fielderPosition == "1B")
														fielderIndex = pitchers[thisTeam];
													else
														fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("1B")] [0];

													if (Random.value > Manager.Instance.Players [(fielderIndex = teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf ("1B")] [0])].FieldingChance)
													{
														numBases = 1;
														ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
														error = true;
													}
													else
													{
														numBases = 0;
														outs += 2;
														Manager.Instance.Players[pitchers [thisTeam]].stats[0][20] += 2;
														bases [1] = null;
														error = false;
													}
												}
											}

											/*if (i == 0)
                                            Debug.Log("Double Play");*/
										}
										else
										{
											if ((fielderPosition != "1B" && (Random.value > Manager.Instance.Players [fielderIndex].CatchingChance || Random.value > Manager.Instance.Players [fielderIndex].FieldingChance)) || Random.value > Manager.Instance.Players [teams [thisTeam].Batters [teams [thisTeam].Positions.IndexOf (fielderPosition)] [0]].CatchingChance)
											{
												numBases = 1;
												ReachedOnError (teams [otherTeam].players [batters [otherTeam]], fielderIndex);
												error = true;
											}
											else
											{
												numBases = 0;
												outs++;
												Manager.Instance.Players[pitchers [thisTeam]].stats[0][20]++;
												error = false;
											}

											/*if (i == 0)
                                            Debug.Log("Groundout");*/
										}
									}

									Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][1]++;
									Manager.Instance.Players[pitchers [thisTeam]].stats[0][21]++;
								}

								if (outs < 3)
								{
									bool advanced = false;
									for (int k = 3; k >= 0; k--)
										if (bases [k] != null)
										{
											advanced = true;
											bases [k] = null;

											if (error)
											{
												int newBase = k + numBases;
												if(newBase > 4)
												{
													scores [otherTeam]++;

													if (bases[k] != null && !bases [k].Error)
													{
														Manager.Instance.Players [teams [otherTeam].players [batters [otherTeam]]].stats [0] [9]++;
														Manager.Instance.Players [pitchers [thisTeam]].stats [0] [24]++;
													}

													Manager.Instance.Players [pitchers [thisTeam]].stats [0] [23]++;
												}
												else if (newBase > 3)
												{
													scores [otherTeam]++;
													Manager.Instance.Players[pitchers [thisTeam]].stats[0][23]++;
												}
												else
													(bases [k + numBases] = bases [k]).Error = true;
											}
											else if (k + numBases > 3)
											{
												scores [otherTeam]++;
												if (bases[k] != null && !bases [k].Error)
												{
													Manager.Instance.Players [teams [otherTeam].players [batters [otherTeam]]].stats [0] [9]++;
													Manager.Instance.Players [pitchers [thisTeam]].stats [0] [24]++;
												}

												Manager.Instance.Players [pitchers [thisTeam]].stats [0] [23]++;
											}
											else
												bases [k + numBases] = bases [k];
										}
									if (fly && advanced)
									{
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][14]++;
										Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][1]--;
										Manager.Instance.Players[pitchers [thisTeam]].stats[0][21]--;
									}
								}

								batters [otherTeam] = (batters [otherTeam] + 1) % 9;
								strikes = 0;
								balls = 0;
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

					if (wildPitch) {
						for (int k = 3; k >= 1; k--)
							if (bases [k] != null)
							{
								bases [k] = null;
								if (k + 1 > 3)
								{
									scores [otherTeam]++;
									Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][9]++;
									Manager.Instance.Players[pitchers [thisTeam]].stats[0][23]++;
									Manager.Instance.Players[pitchers [thisTeam]].stats[0][24]++;
								}
								else
									bases [k + 1] = bases [k];
							}
					}

					if (strikes == 3)
					{
						outs++;
						Manager.Instance.Players[pitchers [thisTeam]].stats[0][20]++;
						Manager.Instance.Players[pitchers [thisTeam]].stats[0][21]++;
						Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][1]++;
						/*if (i == 0)
                            Debug.Log(strikes + "-" + balls + " Strikeout");*/
						batters [otherTeam] = (batters [otherTeam] + 1) % 9;
						Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][11]++;
						Manager.Instance.Players[pitchers [thisTeam]].stats[0][27]++;
						strikes = 0;
						balls = 0;
					}
					else if (balls == 4)
					{
						int currBase = 0;

						while (currBase < bases.Length && bases [currBase] != null)
							currBase++;
						
						if (currBase == 4)
						{
							scores [otherTeam]++;
							Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][9]++;
						}
						else
							for (int k = currBase; k > 0; k--)
							{
								bases [k] = bases [k - 1];
								bases [k - 1] = null;
							}

						/*if (i == 0)
                            Debug.Log(strikes + "-" + balls + " Walk");*/

						if (atBatter)
							Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][32]++;
						else
							Manager.Instance.Players[teams [otherTeam].players [batters [otherTeam]]].stats[0][10]++;
						
						Manager.Instance.Players[pitchers [thisTeam]].stats[0][26]++;

						batters [otherTeam] = (batters [otherTeam] + 1) % 9;
						strikes = 0;
						balls = 0;
					}
				}
			}

			inning++;
		}

		//Home team gains revenue and pays expenses
		teams[1].AddRevenue (teams[0].Hype);
		teams[0].SubtractExpenses ();

		if (scores [0] > scores [1])
		{
			teams[0].Win ();
			teams[1].Loss ();
		}
		else
		{
			teams[1].Win ();
			teams[0].Loss ();
		}

		for (int j = 0; j < teams.Length; j++)
		{
			if (noHitter [j])
				Manager.Instance.Players [pitchers[j]].stats [0] [35]++;
			
			for (int k = 0; k < teams [j].players.Count; k++)
			{
				if (hit [j] [k])
				{
					Manager.Instance.Players [teams [j].players [k]].stats [0] [30]++;
					if (Manager.Instance.Players [teams [j].players [k]].stats [0] [30] > Manager.Instance.Players [teams [j].players [k]].stats [0] [33])
					{
						Manager.Instance.Players [teams [j].players [k]].stats [0] [33] = Manager.Instance.Players [teams [j].players [k]].stats [0] [30];
						Manager.Instance.Players [teams [j].players [k]].stats [0] [34] = Manager.Instance.Year;
					}
				}
				else
					Manager.Instance.Players [teams [j].players [k]].stats [0] [30] = 0;

				if (Manager.Instance.Players [teams [j].players [k]].injuryLength > 0)
					Manager.Instance.Players [teams [j].players [k]].injuryLength--;

				Manager.Instance.Players [teams [j].players [k]].SaveStats ();
			}
		}

		if (pitchers [0] == teams[0].SP [teams[0].CurrStarter])
			Manager.Instance.Players[pitchers [0]].stats[0][26]++;

		if (pitchers [1] == teams[1].SP [teams[1].CurrStarter])
			Manager.Instance.Players[pitchers [1]].stats[0][26]++;

		teams [0].UseStarter ();
		teams [1].UseStarter ();

		if (teams [0].id == 0)
			DisplayScore.Display (teams [0].Wins, teams [0].Losses, scores [0], scores [1]);
		else if (teams [1].id == 0)
			DisplayScore.Display (teams [1].Wins, teams [1].Losses, scores [1], scores [0]);

		PlayerPrefs.Save();

		return new SimulatedGame (scores, teams[0].id, teams[1].id, teams[0].Shortform, teams[1].Shortform);
	}

	public bool ContainsTeam(int team)
	{
		if(team == teams[0].id || team == teams[1].id)
			return true;
		else
			return false;
	}

	public bool IsHomeGame(int team)
	{
		if (team == teams[0].id)
			return false;
		else
			return true;
	}

	string RandomOutfielder()
	{
		float random = Random.value;

		if (random <= 1 / 3)
			return "LF";
		else if (random <= 2 / 3)
			return "CF";
		else
			return "RF";
	}

	string RandomInfielder()
	{
		float random = Random.value;

		if (random <= 0.25f)
			return "1B";
		else if (random <= 0.5f)
			return "2B";
		else if (random <= 0.75f)
			return "3B";
		else
			return "SS";
	}

	string RandomFielder()
	{
		float random = Random.value;

		if (random <= 1 / 7)
			return "LF";
		else if (random <= 2 / 7)
			return "CF";
		else if (random <= 3 / 7)
			return "RF";
		else if (random <= 4 / 7)
			return "1B";
		else if (random <= 5 / 7)
			return "2B";
		else if (random <= 6 / 7)
			return "3B";
		else
			return "SS";
	}

	void ReachedOnError(int batterIndex, int fielderIndex)
	{
		Manager.Instance.Players [fielderIndex].stats [0] [36]++;
		Manager.Instance.Players [batterIndex].stats [0] [31]++;
	}
}
