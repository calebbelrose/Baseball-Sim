using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AllTeams : MonoBehaviour {

    static int numTeams = 30;
	public List<Team> teams = new List<Team>();
    public int[,] schedule = new int[numTeams / 2, 2];
    public int year, numPlays, numStats, currStarter;
    public bool needDraft, inFinals;
    public string[] stats = File.ReadAllLines("Stats.txt");
	public List<string> tradeList;
	public List<string> injuries;
	public int longestHitStreak = 0, hitStreakYear;
	public string hitStreakName;
	int[] league = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
	division = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 1, 1, 1, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 2, 2, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
	home = new int[] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 };

    // Use this for initialization
    void Start ()
	{
		string[] lines = File.ReadAllLines ("test.txt");
		System.IO.StreamWriter file1 = new System.IO.StreamWriter ("test1.txt");
		bool home = false;
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines [i] == "-1")
			{
				home = true;
				file1.WriteLine ("teamSchedule.Add(new ScheduledGame(29,1==1));");
			}
			else if (lines [i] == "vs")
				home = true;
			else if (lines [i] == "@")
				home = false;
			else if (lines [i] != " ") {
				file1.Write ("teamSchedule.Add(new ScheduledGame(");
				switch (lines [i]) {
				case "Toronto":
					file1.Write ("0,");
					break;
				case "Baltimore":
					file1.Write ("1,");
					break;
				case "Tampa Bay":
					file1.Write ("2,");
					break;
				case "Boston":
					file1.Write ("3,");
					break;
				case "NY Yankees":
					file1.Write ("4,");
					break;
				case "Detroit":
					file1.Write ("5,");
					break;
				case "Cleveland":
					file1.Write ("6,");
					break;
				case "White Sox":
					file1.Write ("7,");
					break;
				case "Kansas City":
					file1.Write ("8,");
					break;
				case "Minnesota":
					file1.Write ("9,");
					break;
				case "LA Angels":
					file1.Write ("10,");
					break;
				case "Seattle":
					file1.Write ("11,");
					break;
				case "Texas":
					file1.Write ("12,");
					break;
				case "Oakland":
					file1.Write ("13,");
					break;
				case "Houston":
					file1.Write ("14,");
					break;
				case "Atlanta":
					file1.Write ("15,");
					break;
				case "Washington":
					file1.Write ("16,");
					break;
				case "Miami":
					file1.Write ("17,");
					break;
				case "Philadelphia":
					file1.Write ("18,");
					break;
				case "NY Mets":
					file1.Write ("19,");
					break;
				case "Milwaukee":
					file1.Write ("20,");
					break;
				case "St. Louis":
					file1.Write ("21,");
					break;
				case "Cincinnati":
					file1.Write ("22,");
					break;
				case "Pittsburgh":
					file1.Write ("23,");
					break;
				case "Cubs":
					file1.Write ("24,");
					break;
				case "Arizona":
					file1.Write ("25,");
					break;
				case "San Diego":
					file1.Write ("26,");
					break;
				case "San Francisco":
					file1.Write ("27,");
					break;
				case "LA Dodgers":
					file1.Write ("28,");
					break;
				case "Colorado":
					file1.Write ("29,");
					break;
				default:
					System.IO.StreamWriter file2 = new System.IO.StreamWriter ("test2.txt");
					file2.WriteLine (lines[i]);
					file2.Close ();
					break;
				}
				if (home)
					file1.WriteLine ("1==1));");
				else
					file1.WriteLine ("0==1));");
			}
		}

		file1.Close ();


        if (PlayerPrefs.HasKey("Year"))
        {
            year = int.Parse(PlayerPrefs.GetString("Year"));
            numPlays = PlayerPrefs.GetInt("NumPlays");
            currStarter = PlayerPrefs.GetInt("CurrStarter");
            needDraft = bool.Parse(PlayerPrefs.GetString("NeedDraft"));
            inFinals = bool.Parse(PlayerPrefs.GetString("InFinals"));
            
            for (int i = 0; i < numTeams; i++)
            {
                Team team = new Team();
                int numPlayers = PlayerPrefs.GetInt("NumPlayers" + i);
                string teamInfo = PlayerPrefs.GetString("Team" + i);
                string teamOveralls = PlayerPrefs.GetString("Overalls" + i);
                string[] teamInfoSplit = teamInfo.Split(',');
                string[] teamOverallsSplit = teamOveralls.Split(',');
                string[] wl;
                string[] splitName;

                for (int j = 0; j < numPlayers; j++)
                {
					Player newPlayer = new Player ();

					newPlayer.LoadPlayer (i, j);

                    team.players.Add(newPlayer);
					team.currentSalary += newPlayer.salary;
                }

                team.id = int.Parse(teamInfoSplit[0]);
                team.cityName = teamInfoSplit[1];
                team.teamName = teamInfoSplit[2];
                team.pick = int.Parse(teamInfoSplit[3]);
                team.overalls[0] = float.Parse(teamOverallsSplit[0]);
                team.overalls[1] = float.Parse(teamOverallsSplit[1]);
                team.overalls[2] = float.Parse(teamOverallsSplit[2]);
                splitName = (team.cityName + " " + team.teamName).Split(' ');

                for (int j = 0; j < splitName.Length; j++)
                    if (System.Char.IsLetter(splitName[j][0]) && System.Char.IsUpper(splitName[j][0]))
                        team.shortform += splitName[j][0];
                
                wl = PlayerPrefs.GetString("WL" + i).Split(',');
				team.wins = int.Parse(wl [0]);
				team.losses = int.Parse(wl [1]);

                for(int j = 0; j < 5; j++)
                    team.SP.Add(PlayerPrefs.GetInt("SP" + i + "-" + j));

                for(int j = 0; j < 9; j++)
                    team.Batters.Add(PlayerPrefs.GetInt("Batter" + i + "-" + j));

                for (int j = 0; j < 3; j++)
                    team.RP.Add(PlayerPrefs.GetInt("RP" + i + "-" + j));

                for (int j = 0; j < 1; j++)
                    team.CP.Add(PlayerPrefs.GetInt("CP" + i + "-" + j));

				teams.Add (team);
            }

			Player.longestFirstName = PlayerPrefs.GetInt ("LongestFirstName");
			Player.longestLastName = PlayerPrefs.GetInt ("LongestLastName");
            
            string fullSchedule = PlayerPrefs.GetString("Schedule");
            string[] tempSchedule = fullSchedule.Split(',');
            for (int i = 0; i < tempSchedule.Length; i++)
            {
                schedule[i / 2, i % 2] = int.Parse(tempSchedule[i]);
            }
        }
        else
            Restart();
    }
	
	public int GetNumTeams()
    {
        return numTeams;
    }

    public void Restart()
	{
		List<int> picksLeft = new List<int> ();
		string strSchedule = "";

		year = System.DateTime.Now.Year;
		PlayerPrefs.SetString ("Year", year.ToString ());
		numPlays = 0;
		PlayerPrefs.SetString ("NumPlays", numPlays.ToString ());
		needDraft = true;
		PlayerPrefs.SetString ("NeedDraft", needDraft.ToString ());
		inFinals = false;
		PlayerPrefs.SetString ("InFinals", inFinals.ToString ());
		currStarter = 0;
		PlayerPrefs.SetInt ("CurrStarter", currStarter);
		tradeList = new List<string> ();

		for (int i = 0; i < numTeams; i++)
			picksLeft.Add (i);

		for (int i = 0; i < numTeams; i++) {
			Team team = new Team ();
			string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
			Player newPlayer;

			team.id = i;
			team.pick = picksLeft [(int)(Random.value * picksLeft.Count)];

			for (int j = 3; j < positions.Length; j++) {
				newPlayer = new Player (positions [j]);
				newPlayer.playerNumber = team.players.Count;

				newPlayer.SavePlayer (i, newPlayer.playerNumber);
				PlayerPrefs.SetInt ("Batter" + i + "-" + team.Batters.Count, newPlayer.playerNumber);
				team.Batters.Add (newPlayer.playerNumber);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			for (int j = 0; j < 5; j++) {
				newPlayer = new Player ("SP");
				newPlayer.playerNumber = team.players.Count;

				newPlayer.SavePlayer (i, newPlayer.playerNumber);
				PlayerPrefs.SetInt ("SP" + i + "-" + team.SP.Count, newPlayer.playerNumber);
				team.SP.Add (newPlayer.playerNumber);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			for (int j = 0; j < 3; j++) {
				newPlayer = new Player ("RP");
				newPlayer.playerNumber = team.players.Count;

				newPlayer.SavePlayer (i, newPlayer.playerNumber);
				PlayerPrefs.SetInt ("RP" + i + "-" + team.RP.Count, newPlayer.playerNumber);
				team.RP.Add (newPlayer.playerNumber);
				team.players.Add (newPlayer);
				team.currentSalary += newPlayer.salary;
			}

			newPlayer = new Player ("CP");
			newPlayer.playerNumber = team.players.Count;

			newPlayer.SavePlayer (i, newPlayer.playerNumber);
			PlayerPrefs.SetInt ("CP" + i + "-" + team.CP.Count, newPlayer.playerNumber);
			team.CP.Add (newPlayer.playerNumber);
			team.players.Add (newPlayer);
			team.currentSalary += newPlayer.salary;

			PlayerPrefs.SetString ("Team" + team.id, team.id + "," + team.cityName + "," + team.teamName + "," + team.pick);
			PlayerPrefs.SetString ("Overalls" + team.id, team.overalls [0] + "," + team.overalls [1] + "," + team.overalls [2]);
			PlayerPrefs.SetString ("WL" + team.id.ToString (), "0,0");
			PlayerPrefs.SetInt ("NumPlayers" + i, team.players.Count);

			teams.Add (team);
		}

		PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
		PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

        for (int i = 0; i < numTeams; i++)
        {
            schedule[i / 2, i % 2] = i;
            strSchedule += i + ",";
        }

        strSchedule = strSchedule.Remove(strSchedule.Length - 1, 1);
        PlayerPrefs.SetString("Schedule", strSchedule);

        PlayerPrefs.Save();
    }
}
