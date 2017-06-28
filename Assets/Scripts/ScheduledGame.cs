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
		int[] scores = new int[2] { 0, 0 };
		int[] batters = new int[2] { 0, 0 };
		int[] relievers = new int[2] { 0, 0 };
		int[] pitchers = new int[2] { 0, 0 };
		List<bool>[] hit = new List<bool>[2];

		hit [0] = new List<bool> ();
		hit [1] = new List<bool> ();

		pitchers [0] = teams[0].SP [teams[0].CurrStarter];
		pitchers [1] = teams[1].SP [teams[1].CurrStarter];

		teams[0].players [pitchers [0]].games++;
		teams[0].players [pitchers [0]].gamesStarted++;

		teams[1].players [pitchers [1]].games++;
		teams[1].players [pitchers [1]].gamesStarted++;

		for (int j = 0; j < 2; j++)
		{
			for (int k = 0; k < teams [j].Batters.Count; k++)
				teams [j].players [teams [j].Batters [k]].games++;

			for (int k = 0; k < teams [j].players.Count; k++)
				hit [j].Add (false);
		}

		while (inning <= 9 || scores [0] == scores [1]) {
			for (int j = 0; j < 2; j++) {
				/*if (i == 0)
                {
                    if (j == 0)
                        Debug.Log("Top of the " + (inning + 1));
                    else
                        Debug.Log("Bottom of the " + (inning + 1));
                }*/
				int outs = 0, thisTeam = j, otherTeam = (j + 1) % 2, strikes = 0, balls = 0;
				bool[] bases = new bool[4] { false, false, false, false };

				if (inning == 9 && pitchers [thisTeam] != teams [thisTeam].SP [teams [thisTeam].CurrStarter] && scores [thisTeam] > scores [otherTeam]) {
					pitchers [thisTeam] = teams [thisTeam].CP [0];
				}

				while (outs < 3) {
					bool strike, swing, atBatter = false, wildPitch = false;
					float thisEye, thisContact, thisPower, thisPitchPower, thisMovement, thisAccuracy, thisLocation;
					float accuracy = teams [thisTeam].players [pitchers [thisTeam]].skills [6];

					bases [0] = true;

					if (teams [thisTeam].players [pitchers [thisTeam]].skills [8] <= 0 && strikes == 0 && balls == 0 && relievers [thisTeam] < 2) {
						if (pitchers [thisTeam] == teams [thisTeam].SP [teams[thisTeam].CurrStarter] && inning > 6 && scores [otherTeam] <= 3)
							teams [thisTeam].players [pitchers [thisTeam]].qualityStarts++;
						pitchers [thisTeam] = teams [thisTeam].RP [relievers [thisTeam]];
						relievers [thisTeam]++;
					}

					thisLocation = Random.value;

					if (balls == 3 && strikes <= 1) {
						if (teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [1] > 75 && (teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [0] > 75 || teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [3] > 75)) {
							thisLocation /= 2;
							strike = false;
						} else {
							thisLocation *= 2;
							strike = true;
						}
					} else if (strikes == 2 && balls <= 3 && teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [2] < 75) {
						strike = false;
						thisLocation /= 2;
					} else {
						if (thisLocation < 0.5f)
							strike = true;
						else
							strike = false;
					}

					thisAccuracy = Random.value;

					if (thisAccuracy > accuracy * (teams [thisTeam].players [pitchers [thisTeam]].skills [8] / teams [thisTeam].players [pitchers [thisTeam]].skills [9] / 5.0f + 0.95f)) {
						if (thisAccuracy > 0.975f) {
							strike = false;
							if (Random.value > 0.5f)
								wildPitch = true;
							else
								atBatter = true;
						} else
							strike = !strike;
					}

					thisEye = Random.value * teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [2] / 100.0f;
					thisPitchPower = Random.value * teams [thisTeam].players [pitchers [thisTeam]].skills [5] / 100.0f;
					thisMovement = Random.value * teams [thisTeam].players [pitchers [thisTeam]].skills [7] / 100.0f;

					if (thisEye > thisPitchPower && thisEye > thisMovement) {
						if (strike)
							swing = true;
						else
							swing = false;
					} else {
						if (thisEye > 0.5f)
							swing = false;
						else if (thisMovement > 0.5f && thisPitchPower > 0.5f)
							swing = true;
						else if (Random.value > 0.5f)
							swing = true;
						else
							swing = false;
					}

					if (swing) {
						if (atBatter)
							balls = 4;
						else {
							thisContact = Random.value * teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [1] / 100.0f;
							if (!strike)
								thisContact *= (1 - thisLocation) * 2 + thisLocation;

							if (thisContact > thisMovement && thisContact > thisPitchPower) {
								int numBases;
								bool fly = false;
								float powerRandom = Random.value;

								thisPower = powerRandom * teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [0] / 100.0f;

								if (powerRandom > 0.95f || thisPower > 0.8f) {
									/*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Homerun");*/
									teams [otherTeam].players [batters [otherTeam]].homeruns++;
									teams [otherTeam].players [batters [otherTeam]].totalBases += 4;
									teams [otherTeam].players [batters [otherTeam]].hits++;
									teams [thisTeam].players [pitchers [thisTeam]].hitsAgainst++;
									teams [thisTeam].players [pitchers [thisTeam]].homerunsAgainst++;
									hit [otherTeam] [batters [otherTeam]] = true;
									numBases = 4;
								} else {
									thisPower = thisPower * 2 / 3 + teams [otherTeam].players [teams [otherTeam].Batters [batters [otherTeam]]].skills [3] / 300.0f;

									if (thisPower > 0.75f) {
										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Triple");*/
										teams [otherTeam].players [batters [otherTeam]].triples++;
										teams [otherTeam].players [batters [otherTeam]].totalBases += 3;
										teams [otherTeam].players [batters [otherTeam]].hits++;
										teams [thisTeam].players [pitchers [thisTeam]].hitsAgainst++;
										hit [otherTeam] [batters [otherTeam]] = true;
										numBases = 4;
									} else if (thisPower > 0.65f) {
										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Flyout");*/
										fly = true;
										numBases = 1;
										bases [0] = false;
										outs++;
										teams [thisTeam].players [pitchers [thisTeam]].inningsPitched++;
									} else if (thisPower > 0.6f) {
										numBases = 2;
										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Double");*/
										teams [otherTeam].players [batters [otherTeam]].doubles++;
										teams [otherTeam].players [batters [otherTeam]].totalBases += 2;
										teams [otherTeam].players [batters [otherTeam]].hits++;
										teams [thisTeam].players [pitchers [thisTeam]].hitsAgainst++;
										hit [otherTeam] [batters [otherTeam]] = true;
									} else if (thisPower > 0.45f) {
										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Popout");*/
										numBases = 0;
										outs++;
										teams [thisTeam].players [pitchers [thisTeam]].inningsPitched++;
									} else if (thisPower > 0.35f) {
										numBases = 1;
										/*if (i == 0)
                                        Debug.Log(strikes + "-" + balls + " Single");*/
										teams [otherTeam].players [batters [otherTeam]].singles++;
										teams [otherTeam].players [batters [otherTeam]].totalBases++;
										teams [otherTeam].players [batters [otherTeam]].hits++;
										teams [thisTeam].players [pitchers [thisTeam]].hitsAgainst++;
										hit [otherTeam] [batters [otherTeam]] = true;
									} else if (thisPower > 0.25f) {
										numBases = 0;
										outs++;
										teams [thisTeam].players [pitchers [thisTeam]].inningsPitched++;
										/*if (i == 0)
                                        Debug.Log("Lineout");*/
									} else {
										numBases = 0;
										if (bases [1]) {
											outs += 2;
											teams [thisTeam].players [pitchers [thisTeam]].inningsPitched += 2;
											bases [1] = false;
											/*if (i == 0)
                                            Debug.Log("Double Play");*/
										} else {
											outs++;
											teams [thisTeam].players [pitchers [thisTeam]].inningsPitched++;
											/*if (i == 0)
                                            Debug.Log("Groundout");*/
										}
									}
									teams [otherTeam].players [batters [otherTeam]].atBats++;
									teams [thisTeam].players [pitchers [thisTeam]].atBatsAgainst++;
								}

								if (outs < 3) {
									bool advanced = false;
									for (int k = 3; k >= 0; k--)
										if (bases [k]) {
											advanced = true;
											bases [k] = false;
											if (k + numBases > 3) {
												scores [otherTeam]++;
												teams [otherTeam].players [batters [otherTeam]].runsBattedIn++;
												teams [thisTeam].players [pitchers [thisTeam]].runsAgainst++;
												teams [thisTeam].players [pitchers [thisTeam]].earnedRuns++;
											} else
												bases [k + numBases] = true;
										}
									if (fly && advanced) {
										teams [otherTeam].players [batters [otherTeam]].sacrifices++;
										teams [otherTeam].players [batters [otherTeam]].atBats--;
										teams [thisTeam].players [pitchers [thisTeam]].atBatsAgainst--;
									}
								}

								batters [otherTeam] = (batters [otherTeam] + 1) % 9;
								strikes = 0;
								balls = 0;
							} else
								strikes++;
						}
					} else {
						if (strike)
							strikes++;
						else
							balls++;
					}

					if (wildPitch) {
						for (int k = 3; k >= 1; k--)
							if (bases [k]) {
								bases [k] = false;
								if (k + 1 > 3) {
									scores [otherTeam]++;
									teams [otherTeam].players [batters [otherTeam]].runsBattedIn++;
									teams [thisTeam].players [pitchers [thisTeam]].runsAgainst++;
									teams [thisTeam].players [pitchers [thisTeam]].earnedRuns++;
								} else
									bases [k + 1] = true;
							}
					}

					if (strikes == 3) {
						outs++;
						teams [thisTeam].players [pitchers [thisTeam]].inningsPitched++;
						teams [thisTeam].players [pitchers [thisTeam]].atBatsAgainst++;
						teams [otherTeam].players [batters [otherTeam]].atBats++;
						/*if (i == 0)
                            Debug.Log(strikes + "-" + balls + " Strikeout");*/
						batters [otherTeam] = (batters [otherTeam] + 1) % 9;
						teams [otherTeam].players [batters [otherTeam]].strikeouts++;
						teams [thisTeam].players [pitchers [thisTeam]].strikeoutsAgainst++;
						strikes = 0;
						balls = 0;
					} else if (balls == 4) {
						int currBase = 0;

						while (currBase < bases.Length && bases [currBase])
							currBase++;
						if (currBase == 4) {
							scores [otherTeam]++;
							teams [otherTeam].players [batters [otherTeam]].runsBattedIn++;
						} else
							for (int k = currBase; k > 0; k--) {
								bases [k] = true;
								bases [k - 1] = false;
							}

						/*if (i == 0)
                            Debug.Log(strikes + "-" + balls + " Walk");*/

						if (atBatter)
							teams [otherTeam].players [batters [otherTeam]].hitByPitch++;
						else
							teams [otherTeam].players [batters [otherTeam]].walks++;
						
						teams [thisTeam].players [pitchers [thisTeam]].walksAgainst++;

						batters [otherTeam] = (batters [otherTeam] + 1) % 9;
						strikes = 0;
						balls = 0;
					}
					teams [thisTeam].players [pitchers [thisTeam]].skills [8]--;
				}
			}

			inning++;
		}

		//Home team gains revenue and pays expenses
		teams[1].AddRevenue (teams[0].hype);
		teams[0].SubtractExpenses ();


		if (scores [0] > scores [1])
		{
			teams[0].Win ();
			teams[1].Loss ();
		}
		else
		{
			teams[0].Win ();
			teams[1].Loss ();
		}

		for (int j = 0; j < teams.Length; j++)
			for (int k = 0; k < teams [j].players.Count; k++) {
				if (hit [j] [k]) {
					teams [j].players [k].hitStreak++;
					if (teams [j].players [k].hitStreak > teams [j].players [k].longestHitStreak)
					{
						teams [j].players [k].longestHitStreak = teams [j].players [k].hitStreak;
						teams [j].players [k].hitStreakYear = Player.Year;
					}
				} else
					teams [j].players [k].hitStreak = 0;

				if (teams [j].players [k].injuryLength > 0)
					teams [j].players [k].injuryLength--;

				teams [j].players [k].SaveStats (teams [j].players [k].PlayerIndex);
			}


		if (pitchers [0] == teams[0].SP [teams[0].CurrStarter])
			teams[0].players [pitchers [0]].completeGames++;

		if (pitchers [1] == teams[1].SP [teams[1].CurrStarter])
			teams[1].players [pitchers [1]].completeGames++;

		teams [0].UseStarter ();
		teams [1].UseStarter ();

		if (teams [0].id == 0)
			DisplayScore.Display (teams [0].wins, teams [0].losses, scores [0], scores [1]);
		else if (teams [1].id == 0)
			DisplayScore.Display (teams [1].wins, teams [1].losses, scores [1], scores [0]);

		PlayerPrefs.Save();

		return new SimulatedGame (scores, teams[0].id, teams[1].id, teams[0].shortform, teams[1].shortform);
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
}
