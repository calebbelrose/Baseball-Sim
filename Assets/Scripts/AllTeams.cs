using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AllTeams : MonoBehaviour {

    static int numTeams = 30;
    public Team[] teams = new Team[numTeams];
    public int[,] schedule = new int[numTeams / 2, 2];
    public int year, numPlays, numStats, currStarter;
    public bool needDraft, inFinals;
    string[] stats = File.ReadAllLines("Stats.txt");

    // Use this for initialization
    void Start () {
        numStats = stats.Length;

        if (PlayerPrefs.HasKey("Year"))
        {
            year = int.Parse(PlayerPrefs.GetString("Year"));
            numPlays = PlayerPrefs.GetInt("NumPlays");
            currStarter = PlayerPrefs.GetInt("CurrStarter");
            needDraft = bool.Parse(PlayerPrefs.GetString("NeedDraft"));
            inFinals = bool.Parse(PlayerPrefs.GetString("InFinals"));
            
            for (int i = 0; i < numTeams; i++)
            {
                teams[i] = new Team();
                int numPlayers = PlayerPrefs.GetInt("NumPlayers" + i);
                string teamInfo = PlayerPrefs.GetString("Team" + i);
                string teamOveralls = PlayerPrefs.GetString("Overalls" + i);
                string[] teamInfoSplit = teamInfo.Split(',');
                string[] teamOverallsSplit = teamOveralls.Split(',');
                string[] pwl;
                string[] splitName;

                for (int j = 0; j < numPlayers; j++)
                {
                    string player = PlayerPrefs.GetString("Player" + i + "-" + j);
                    string[] newPlayer = player.Split(',');
                    string currStats;
                    string[] splitStats;

                    teams[i].players.Add(newPlayer);
                    currStats = PlayerPrefs.GetString("PlayerStats" + i + "-" + teams[i].pStats.Count);
                    splitStats = currStats.Split(',');
                    teams[i].pStats.Add(teams[i].NewEmptyStats());

                    for (int k = 0; k < splitStats.Length; k++)
                        teams[i].pStats[teams[i].pStats.Count - 1][k] = int.Parse(splitStats[k]);
                }

                teams[i].id = int.Parse(teamInfoSplit[0]);
                teams[i].cityName = teamInfoSplit[1];
                teams[i].teamName = teamInfoSplit[2];
                teams[i].pick = int.Parse(teamInfoSplit[3]);
                teams[i].overalls[0] = float.Parse(teamOverallsSplit[0]);
                teams[i].overalls[1] = float.Parse(teamOverallsSplit[1]);
                teams[i].overalls[2] = float.Parse(teamOverallsSplit[2]);
                splitName = (teams[i].cityName + " " + teams[i].teamName).Split(' ');

                for (int j = 0; j < splitName.Length; j++)
                    if (System.Char.IsLetter(splitName[j][0]) && System.Char.IsUpper(splitName[j][0]))
                        teams[i].shortform += splitName[j][0];
                
                pwl = PlayerPrefs.GetString("PWL" + i).Split(',');
                for(int j = 0; j < pwl.Length; j++)
                    teams[i].pwl[j] = int.Parse(pwl[j]);

                for(int j = 0; j < 5; j++)
                    teams[i].SP.Add(PlayerPrefs.GetInt("SP" + i + "-" + j));

                for(int j = 0; j < 9; j++)
                    teams[i].Batters.Add(PlayerPrefs.GetInt("Batter" + i + "-" + j));

                for (int j = 0; j < 3; j++)
                    teams[i].RP.Add(PlayerPrefs.GetInt("RP" + i + "-" + j));

                for (int j = 0; j < 1; j++)
                    teams[i].CP.Add(PlayerPrefs.GetInt("CP" + i + "-" + j));
            }
            
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
        List<int> picksLeft = new List<int>();
        string strSchedule = "";

        year = System.DateTime.Now.Year;
        PlayerPrefs.SetString("Year", year.ToString());
        needDraft = true;
        PlayerPrefs.SetString("NeedDraft", needDraft.ToString());
        inFinals = false;
        PlayerPrefs.SetString("InFinals", inFinals.ToString());
        currStarter = 0;
        PlayerPrefs.SetInt("CurrStarter", currStarter);

        for (int i = 0; i < teams.Length; i++)
            picksLeft.Add(i);

        for (int i = 0; i < teams.Length; i++)
        {
            teams[i] = new Team();
            string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
            string[] firstNames, lastNames, cityNames, teamNames;
            string[] newPlayer = new string[stats.Length];
            string playerString, currStats = "";
            float totalStats, totalOffense, totalDefense;
            int age, potential;

            firstNames = File.ReadAllLines("FirstNames.txt");
            lastNames = File.ReadAllLines("LastNames.txt");
            cityNames = File.ReadAllLines("CityNames.txt");
            teamNames = File.ReadAllLines("TeamNames.txt");
            teams[i].cityName = cityNames[(int)(Random.value * cityNames.Length)];
            teams[i].teamName = teamNames[(int)(Random.value * teamNames.Length)];
            teams[i].id = i;
            teams[i].pick = picksLeft[(int)(Random.value * picksLeft.Count)];

            for (int j = 3; j < positions.Length; j++)
            {
                newPlayer = new string[stats.Length];
                playerString = "";
                totalStats = 0;
                totalOffense = 0;
                totalDefense = 0;

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];
                newPlayer[2] = positions[j];

                age = (int)(Random.value * 27) + 18;
                newPlayer[7] = age.ToString();

                for (int k = 8; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                    if (k < 11)
                        totalOffense += currStat;
                    else if (k > 11)
                        totalDefense += currStat;
                    else
                    {
                        totalOffense += currStat;
                        totalDefense += currStat;
                    }
                }

                potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[6] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 8))).ToString("0.00");
                newPlayer[4] = ((totalOffense / 4)).ToString("0.00");
                newPlayer[5] = ((totalDefense / 4)).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                PlayerPrefs.SetInt("Batter" + i + "-" + teams[i].Batters.Count, teams[i].players.Count);
                teams[i].Batters.Add(teams[i].players.Count);
                teams[i].players.Add(newPlayer);
                teams[i].pStats.Add(teams[i].NewEmptyStats());

                for (int k = 0; k < teams[i].pStats[teams[i].pStats.Count - 1].Length - 1; k++)
                    currStats += teams[i].pStats[teams[i].pStats.Count - 1][k] + ",";

                currStats += teams[i].pStats[teams[i].pStats.Count - 1][teams[i].pStats[teams[i].pStats.Count - 1].Length - 1];

                PlayerPrefs.SetString("PlayerStats" + i + "-" + (teams[i].pStats.Count - 1), currStats);
                currStats = "";
            }

            for (int j = 0; j < 5; j++)
            {
                newPlayer = new string[stats.Length];
                playerString = "";
                totalStats = 0;
                totalOffense = 0;
                totalDefense = 0;

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];
                newPlayer[2] = "SP";

                age = (int)(Random.value * 27) + 18;
                newPlayer[7] = age.ToString();

                for (int k = 8; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                    if (k < 11)
                        totalOffense += currStat;
                    else if (k > 11)
                        totalDefense += currStat;
                    else
                    {
                        totalOffense += currStat;
                        totalDefense += currStat;
                    }
                }

                potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[6] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 8))).ToString("0.00");
                newPlayer[4] = ((totalOffense / 4)).ToString("0.00");
                newPlayer[5] = ((totalDefense / 4)).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                PlayerPrefs.SetInt("Batter" + i + "-" + teams[i].Batters.Count, teams[i].players.Count);
                teams[i].SP.Add(teams[i].players.Count);
                teams[i].players.Add(newPlayer);
                teams[i].pStats.Add(teams[i].NewEmptyStats());

                for (int k = 0; k < teams[i].pStats[teams[i].pStats.Count - 1].Length - 1; k++)
                    currStats += teams[i].pStats[teams[i].pStats.Count - 1][k] + ",";

                currStats += teams[i].pStats[teams[i].pStats.Count - 1][teams[i].pStats[teams[i].pStats.Count - 1].Length - 1];

                PlayerPrefs.SetString("PlayerStats" + i + "-" + (teams[i].pStats.Count - 1), currStats);
                currStats = "";
            }

            for (int j = 0; j < 3; j++)
            {
                newPlayer = new string[stats.Length];
                playerString = "";
                totalStats = 0;
                totalOffense = 0;
                totalDefense = 0;

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];
                newPlayer[2] = "RP";

                age = (int)(Random.value * 27) + 18;
                newPlayer[7] = age.ToString();

                for (int k = 8; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                    if (k < 11)
                        totalOffense += currStat;
                    else if (k > 11)
                        totalDefense += currStat;
                    else
                    {
                        totalOffense += currStat;
                        totalDefense += currStat;
                    }
                }

                potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[6] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 8))).ToString("0.00");
                newPlayer[4] = ((totalOffense / 4)).ToString("0.00");
                newPlayer[5] = ((totalDefense / 4)).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                PlayerPrefs.SetInt("Batter" + i + "-" + teams[i].Batters.Count, teams[i].players.Count);
                teams[i].RP.Add(teams[i].players.Count);
                teams[i].players.Add(newPlayer);
                teams[i].pStats.Add(teams[i].NewEmptyStats());

                for (int k = 0; k < teams[i].pStats[teams[i].pStats.Count - 1].Length - 1; k++)
                    currStats += teams[i].pStats[teams[i].pStats.Count - 1][k] + ",";

                currStats += teams[i].pStats[teams[i].pStats.Count - 1][teams[i].pStats[teams[i].pStats.Count - 1].Length - 1];

                PlayerPrefs.SetString("PlayerStats" + i + "-" + (teams[i].pStats.Count - 1), currStats);
                currStats = "";
            }

            newPlayer = new string[stats.Length];
            playerString = "";
            totalStats = 0;
            totalOffense = 0;
            totalDefense = 0;

            newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
            newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];
            newPlayer[2] = "CP";

            age = (int)(Random.value * 27) + 18;
            newPlayer[7] = age.ToString();

            for (int k = 8; k < stats.Length; k++)
            {
                int currStat = (int)(Random.value * age) + 55;
                newPlayer[k] = currStat.ToString();
                totalStats += currStat;
                if (k < 11)
                    totalOffense += currStat;
                else if (k > 11)
                    totalDefense += currStat;
                else
                {
                    totalOffense += currStat;
                    totalDefense += currStat;
                }
            }

            potential = (int)(Random.value * 25 + (43 - age) * 3);
            if (potential < 0)
                potential = 0;
            newPlayer[6] = potential.ToString();

            newPlayer[3] = ((totalStats / (stats.Length - 8))).ToString("0.00");
            newPlayer[4] = ((totalOffense / 4)).ToString("0.00");
            newPlayer[5] = ((totalDefense / 4)).ToString("0.00");

            for (int k = 0; k < newPlayer.Length - 1; k++)
                playerString += newPlayer[k] + ",";
            playerString += newPlayer[newPlayer.Length - 1];

            PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
            PlayerPrefs.SetInt("Batter" + i + "-" + teams[i].Batters.Count, teams[i].players.Count);
            teams[i].CP.Add(teams[i].players.Count);
            teams[i].players.Add(newPlayer);
            teams[i].pStats.Add(teams[i].NewEmptyStats());

            for (int k = 0; k < teams[i].pStats[teams[i].pStats.Count - 1].Length - 1; k++)
                currStats += teams[i].pStats[teams[i].pStats.Count - 1][k] + ",";

            currStats += teams[i].pStats[teams[i].pStats.Count - 1][teams[i].pStats[teams[i].pStats.Count - 1].Length - 1];

            PlayerPrefs.SetString("PlayerStats" + i + "-" + (teams[i].pStats.Count - 1), currStats);

            PlayerPrefs.SetString("Team" + teams[i].id, teams[i].id + "," + teams[i].cityName + "," + teams[i].teamName + "," + teams[i].pick);
            PlayerPrefs.SetString("Overalls" + teams[i].id, teams[i].overalls[0] + "," + teams[i].overalls[1] + "," + teams[i].overalls[2]);
            PlayerPrefs.SetString("PWL" + teams[i].id.ToString(), "0,0,0");
            PlayerPrefs.SetInt("NumPlayers" + i, teams[i].players.Count);
        }

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
