using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour
{

    int maxPlays = 163;
    AllTeams allTeams;
	int totalInnings = 0, totalScore = 0;
	int atBats = 0, singles = 0, doubles = 0, triples = 0, homeruns = 0, walks = 0, strikeouts = 0, hbp = 0, gidp = 0;
	bool home = true;

    public void PlayGame()
	{
		GameObject manager = GameObject.Find ("_Manager");
		allTeams = manager.GetComponent<AllTeams> ();
		Text txtResult;
		int you = 0, them = 0, numRows;
		int[,] temp;
		string result = "", strSchedule = "";
		for(int q = 0; q < 162; q++)
		{
		for (int i = 0; i < allTeams.GetNumTeams () / 2; i++) {
			int inning = 1;
			int[] scores = new int[2] { 0, 0 };
			int[] batters = new int[2] { 0, 0 };
			int[] relievers = new int[2] { 0, 0 };
			int[] teams = new int[2] { 0, 0 };
			int[] pitchers = new int[2] { 0, 0 };
			List<bool>[] hit = new List<bool>[2];

			hit [0] = new List<bool> ();
			hit [1] = new List<bool> ();

			teams [0] = allTeams.schedule [i, 0];
			teams [1] = allTeams.schedule [i, 1];

			pitchers [0] = allTeams.teams [teams [0]].SP [allTeams.currStarter];
			pitchers [1] = allTeams.teams [teams [1]].SP [allTeams.currStarter];

			allTeams.teams [teams [0]].players [pitchers [0]].games++;
			allTeams.teams [teams [0]].players [pitchers [0]].gamesStarted++;

			allTeams.teams [teams [1]].players [pitchers [1]].games++;
			allTeams.teams [teams [1]].players [pitchers [1]].gamesStarted++;

			for (int j = 0; j < 2; j++) {
				for (int k = 0; k < allTeams.teams [teams [j]].Batters.Count; k++)
					allTeams.teams [teams [j]].players [allTeams.teams [teams [j]].Batters [k]].games++;

				for (int k = 0; k < allTeams.teams [teams [j]].players.Count; k++)
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

					if (inning == 9 && pitchers [thisTeam] != allTeams.teams [teams [thisTeam]].SP [allTeams.currStarter] && scores [thisTeam] > scores [otherTeam]) {
						pitchers [thisTeam] = allTeams.teams [teams [thisTeam]].CP [0];
					}

					while (outs < 3) {
						bool strike, swing, atBatter = false, wildPitch = false;
						float thisEye, thisContact, thisPower, thisPitchPower, thisMovement, thisAccuracy, thisLocation;
						float accuracy = allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [6];

						bases [0] = true;

						if (allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [8] <= 0 && strikes == 0 && balls == 0 && relievers [thisTeam] < 2) {
							if (pitchers [thisTeam] == allTeams.teams [teams [thisTeam]].SP [allTeams.currStarter] && inning > 6 && scores [otherTeam] <= 3)
								allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].qualityStarts++;
							pitchers [thisTeam] = allTeams.teams [teams [thisTeam]].RP [relievers [thisTeam]];
							relievers [thisTeam]++;
						}

						thisLocation = Random.value;

						if (balls == 3 && strikes <= 1) {
							if (allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [1] > 75 && (allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [0] > 75 || allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [3] > 75)) {
								thisLocation /= 2;
								strike = false;
							} else {
								thisLocation *= 2;
								strike = true;
							}
						} else if (strikes == 2 && balls <= 3 && allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [2] < 75) {
							strike = false;
							thisLocation /= 2;
						} else {
							if (thisLocation < 0.5f)
								strike = true;
							else
								strike = false;
						}

						thisAccuracy = Random.value;

						if (thisAccuracy > accuracy * (allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [8] / allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [8] / 5.0f + 0.95f)) {
							if (thisAccuracy > 0.975f) {
								strike = false;
								if (Random.value > 0.5f)
									wildPitch = true;
								else
									atBatter = true;
							} else
								strike = !strike;
						}

						thisEye = Random.value * allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [2] / 100.0f;
						thisPitchPower = Random.value * allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [5] / 100.0f;
						thisMovement = Random.value * allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [7] / 100.0f;

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
								thisContact = Random.value * allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [1] / 100.0f;
								if (!strike)
									thisContact *= (1 - thisLocation) * 2 + thisLocation;
									
								if (thisContact > thisMovement && thisContact > thisPitchPower) {
									int numBases;
									bool fly = false;
									float powerRandom = Random.value;

									thisPower = powerRandom * allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [0] / 100.0f;

									if (powerRandom > 0.95f || thisPower > 0.8f) {
										/*if (i == 0)
                                            Debug.Log(strikes + "-" + balls + " Homerun");*/
										allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].homeruns++;
										homeruns++;
										allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].totalBases += 4;
										allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].hits++;
										allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].hitsAgainst++;
										allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].homerunsAgainst++;
										hit [otherTeam] [batters [otherTeam]] = true;
										numBases = 4;
									} else {
										thisPower = thisPower * 2 / 3 + allTeams.teams [teams [otherTeam]].players [allTeams.teams [teams [otherTeam]].Batters [batters [otherTeam]]].skills [3] / 300.0f;

										if (thisPower > 0.75f) {
											/*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Triple");*/
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].triples++;
											triples++;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].totalBases += 3;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].hits++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].hitsAgainst++;
											hit [otherTeam] [batters [otherTeam]] = true;
											numBases = 4;
										} else if (thisPower > 0.65f) {
											/*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Flyout");*/
											fly = true;
											numBases = 1;
											bases [0] = false;
											outs++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched++;
										} else if (thisPower > 0.6f) {
											numBases = 2;
											/*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Double");*/
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].doubles++;
											doubles++;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].totalBases += 2;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].hits++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].hitsAgainst++;
											hit [otherTeam] [batters [otherTeam]] = true;
										} else if (thisPower > 0.45f) {
											/*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Popout");*/
											numBases = 0;
											outs++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched++;
										} else if (thisPower > 0.35f) {
											numBases = 1;
											/*if (i == 0)
                                                Debug.Log(strikes + "-" + balls + " Single");*/
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].singles++;
											singles++;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].totalBases++;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].hits++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].hitsAgainst++;
											hit [otherTeam] [batters [otherTeam]] = true;
										} else if (thisPower > 0.25f) {
											numBases = 0;
											outs++;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched++;
											/*if (i == 0)
                                                Debug.Log("Lineout");*/
										} else {
											numBases = 0;
											if (bases [1]) {
												outs += 2;
												allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched += 2;
												bases [1] = false;
												/*if (i == 0)
                                                    Debug.Log("Double Play");*/
												gidp++;
											} else {
												outs++;
												allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched++;
												/*if (i == 0)
                                                    Debug.Log("Groundout");*/
											}
										}
										allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].atBats++;
										atBats++;
										allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].atBatsAgainst++;
									}

									if (outs < 3) {
										bool advanced = false;
										for (int k = 3; k >= 0; k--)
											if (bases [k]) {
												advanced = true;
												bases [k] = false;
												if (k + numBases > 3) {
													scores [otherTeam]++;
													allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].runsBattedIn++;
													allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].runsAgainst++;
													allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].earnedRuns++;
												} else
													bases [k + numBases] = true;
											}
										if (fly && advanced) {
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].sacrifices++;
											allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].atBats--;
											atBats--;
											allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].atBatsAgainst--;
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
										allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].runsBattedIn++;
										allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].runsAgainst++;
										allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].earnedRuns++;
									} else
										bases [k + 1] = true;
								}
						}

						if (strikes == 3) {
							outs++;
							allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].inningsPitched++;
							allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].atBatsAgainst++;
							allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].atBats++;
							/*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Strikeout");*/
							batters [otherTeam] = (batters [otherTeam] + 1) % 9;
							allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].strikeouts++;
							strikeouts++;
							allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].strikeoutsAgainst++;
							strikes = 0;
							balls = 0;
						} else if (balls == 4) {
							int currBase = 0;

							while (currBase < bases.Length && bases [currBase])
								currBase++;
							if (currBase == 4) {
								scores [otherTeam]++;
								allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].runsBattedIn++;
							} else
								for (int k = currBase; k > 0; k--) {
									bases [k] = true;
									bases [k - 1] = false;
								}

							/*if (i == 0)
                                    Debug.Log(strikes + "-" + balls + " Walk");*/

							if (atBatter) {
								allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].hitByPitch++;
								hbp++;
							} else {
								allTeams.teams [teams [otherTeam]].players [batters [otherTeam]].walks++;
								walks++;
							}
							allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].walksAgainst++;

							batters [otherTeam] = (batters [otherTeam] + 1) % 9;
							strikes = 0;
							balls = 0;
						}
						allTeams.teams [teams [thisTeam]].players [pitchers [thisTeam]].skills [8]--;
					}
				}

				inning++;
			}
				if (home)
				{
					allTeams.teams [teams [0]].AddRevenue (allTeams.teams [teams [1]].hype);
					allTeams.teams [teams [1]].SubtractExpenses ();
				}
				else
				{
					allTeams.teams [teams [1]].AddRevenue (allTeams.teams [teams [0]].hype);
					allTeams.teams [teams [0]].SubtractExpenses ();
				}
				home = !home;

			if (i == 0) {
				you = scores [0];
				them = scores [1];
				if (scores [0] > scores [1]) {
						allTeams.teams [teams [0]].Win ();
						allTeams.teams [teams [1]].Loss ();
					result = "Win";
				} else {
						allTeams.teams [teams [0]].Loss ();
						allTeams.teams [teams [1]].Win ();
					result = "Loss";
				}
			} else {
				if (scores [0] > scores [1]) {
						allTeams.teams [teams [0]].Win ();
						allTeams.teams [teams [1]].Loss ();
				} else {
						allTeams.teams [teams [0]].Win ();
						allTeams.teams [teams [1]].Loss ();
				}
			}

			PlayerPrefs.SetString ("WL" + allTeams.teams [teams [0]].id.ToString (), allTeams.teams [teams [0]].wins + "," + allTeams.teams [teams [0]].losses);
			PlayerPrefs.SetString ("WL" + allTeams.teams [teams [1]].id.ToString (), allTeams.teams [teams [1]].wins + "," + allTeams.teams [teams [1]].losses);

			for (int j = 0; j < teams.Length; j++)
				for (int k = 0; k < allTeams.teams [teams [j]].players.Count; k++) {
					if (hit [j] [k]) {
						allTeams.teams [teams [j]].players [k].hitStreak++;
						if (allTeams.teams [teams [j]].players [k].hitStreak > allTeams.longestHitStreak) {
							allTeams.longestHitStreak = allTeams.teams [teams [j]].players [k].hitStreak;
							allTeams.hitStreakYear = allTeams.year;
							allTeams.hitStreakName = allTeams.teams [teams [j]].players [k].firstName + " " + allTeams.teams [teams [j]].players [k].lastName;
						}
					} else
						allTeams.teams [teams [j]].players [k].hitStreak = 0;

					if (allTeams.teams [teams [j]].players [k].injuryLength > 0)
						allTeams.teams [teams [j]].players [k].injuryLength--;

					allTeams.teams [teams [j]].players [k].SaveStats (i, k);
				}


			totalInnings += inning;
			totalScore += scores [0] + scores [1];
			if (pitchers [0] == allTeams.teams [teams [0]].SP [allTeams.currStarter])
				allTeams.teams [teams [0]].players [pitchers [0]].completeGames++;

			if (pitchers [1] == allTeams.teams [teams [1]].SP [allTeams.currStarter])
				allTeams.teams [teams [1]].players [pitchers [1]].completeGames++;
		}

		allTeams.currStarter = (allTeams.currStarter + 1) % 5;
		PlayerPrefs.SetInt ("CurrStarter", allTeams.currStarter);

		GameObject.Find ("txtYourScore").GetComponent<Text> ().text = "You: " + you;
		txtResult = GameObject.Find ("txtResult").GetComponent<Text> ();
		txtResult.text = result;

		if (result == "Win")
			txtResult.color = Color.green;
		else if (result == "Loss")
			txtResult.color = Color.red;
		else
			txtResult.color = Color.white;

		GameObject.Find ("txtTheirScore").GetComponent<Text> ().text = "Them: " + them;
		double bottom = allTeams.teams [0].wins + allTeams.teams [0].losses;
		double ratio = System.Math.Round (allTeams.teams [0].wins / bottom, 3);
		GameObject.Find ("txtWL").GetComponent<Text> ().text = "W/L: " + allTeams.teams [0].wins + "/" + allTeams.teams [0].losses + " (" + ratio + ")";
		temp = new int[allTeams.GetNumTeams () / 2, 2];
		System.Array.Copy (allTeams.schedule, 0, temp, 0, allTeams.schedule.Length);
		numRows = ((temp.Length + 1) / 2) - 1;
		strSchedule = "";
		for (int i = 0; i < allTeams.GetNumTeams (); i++) {
			int x = i % 2, y = i / 2;
			if (y == 0) {
				if (x == 1)
					allTeams.schedule [0, 1] = temp [1, 0];
			} else if (x == 1)
				allTeams.schedule [y, 1] = temp [y - 1, 1];
			else if (y < numRows)
				allTeams.schedule [y, 0] = temp [y + 1, 0];
			else
				allTeams.schedule [y, 0] = temp [y, 1];
			strSchedule += allTeams.schedule [y, x] + ",";
		}

		strSchedule = strSchedule.Remove (strSchedule.Length - 1, 1);
	}
			PlayerPrefs.SetString ("Schedule", strSchedule);
			allTeams.numPlays++;
		
		float numGames = allTeams.numPlays * 162 * 30;
		double lowest = 99999999999999.0, highest = 0.0, average = 0.0;

		for (int i = 0; i < 30; i++) {
			average += allTeams.teams [i].cash;
			if (allTeams.teams [i].cash < lowest)
				lowest = allTeams.teams [i].cash;
			else if (allTeams.teams [i].cash > highest)
				highest = allTeams.teams [i].cash;
			Debug.Log (allTeams.teams [i].homeGames);
		}

		average /= 30;

		System.IO.StreamWriter file =
			new System.IO.StreamWriter ("test.csv");
		file.WriteLine ("AB,total score,singles,doubles,triples,homeruns,strikeouts,walks,hbp,GIDP");
		file.WriteLine (atBats / numGames + "," + totalScore / numGames + "," + singles / numGames + "," + doubles / numGames + "," + triples / numGames + "," + homeruns / numGames + "," + strikeouts / numGames + "," + walks / numGames + "," + hbp/numGames + "," + gidp/numGames);
		file.WriteLine ((float)5519 / 162 + "," + (float)725 / 162+ "," + (float)918 / 162+ "," + (float)275 / 162+ "," + (float)29 / 162+ "," + (float)187 / 162+ "," + (float)1299 / 162+ "," + (float)503 / 162 + "," + (float)55 / 162 + "," + (float)124/162);
		file.Write (lowest + "," + highest + "," + average);
		file.Close ();

        if (allTeams.numPlays >= maxPlays)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);

        PlayerPrefs.SetInt("NumPlays", allTeams.numPlays);
        PlayerPrefs.Save();
		Debug.Log(((double)totalInnings / (numGames / 2)).ToString("0.00") + " " + ((double)totalScore / numGames).ToString("0.00"));
	}
}