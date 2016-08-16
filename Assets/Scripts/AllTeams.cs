using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AllTeams : MonoBehaviour {

    static int numTeams = 30;
    public Team[] teams = new Team[numTeams];
    public int[,] schedule = new int[numTeams / 2, 2];
    public int year;
    public bool needDraft;
    public bool inFinals;
    string[] stats = File.ReadAllLines("Stats.txt");

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("Year"))
        {
            year = int.Parse(PlayerPrefs.GetString("Year"));
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

                for (int j = 0; j < numPlayers; j++)
                {
                    string player = PlayerPrefs.GetString("Player" + i + "-" + j);
                    string[] newPlayer = player.Split(',');
                    teams[i].players.Add(newPlayer);
                }

                teams[i].id = i;
                teams[i].cityName = teamInfoSplit[0];
                teams[i].teamName = teamInfoSplit[1];
                teams[i].pick = int.Parse(teamInfoSplit[2]);
                teams[i].overalls[0] = float.Parse(teamOverallsSplit[0]);
                teams[i].overalls[1] = float.Parse(teamOverallsSplit[1]);
                teams[i].overalls[2] = float.Parse(teamOverallsSplit[2]);
                
                pwl = PlayerPrefs.GetString("PWL" + i).Split(',');
                for(int j = 0; j < pwl.Length; j++)
                    teams[i].pwl[j] = int.Parse(pwl[j]);
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

        for (int i = 0; i < teams.Length; i++)
            picksLeft.Add(i);

        for (int i = 0; i < teams.Length; i++)
        {
            teams[i] = new Team();
            string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
            string[] firstNames, lastNames, cityNames, teamNames;

            firstNames = File.ReadAllLines("FirstNames.txt");
            lastNames = File.ReadAllLines("LastNames.txt");
            cityNames = File.ReadAllLines("CityNames.txt");
            teamNames = File.ReadAllLines("TeamNames.txt");
            teams[i].cityName = cityNames[(int)(Random.value * cityNames.Length)];
            teams[i].teamName = teamNames[(int)(Random.value * teamNames.Length)];
            teams[i].id = i;
            teams[i].pick = picksLeft[(int)(Random.value * picksLeft.Count)];

            for (int j = 0; j < positions.Length; j++)
            {
                string[] newPlayer = new string[stats.Length];
                string playerString = "";
                float totalStats = 0;
                int age;

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];
                newPlayer[2] = positions[j];

                age = (int)(Random.value * 27) + 18;
                newPlayer[5] = age.ToString();

                for (int k = 6; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age + 1) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                }

                int potential = (int)(Random.value * 25 + (43 - age) * 3 + 1);
                if (potential < 0)
                    potential = 0;
                newPlayer[4] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 6))).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                teams[i].players.Add(newPlayer);
            }

            for(int j = 0; j < 4; j++)
            {
                string[] newPlayer = new string[stats.Length];
                string playerString = "";
                float totalStats = 0;
                int age;

                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];

                newPlayer[2] = positions[0];

                age = (int)(Random.value * 27) + 18;
                newPlayer[5] = age.ToString();

                for (int k = 6; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                }

                int potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[4] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 6))).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                teams[i].players.Add(newPlayer);
            }

            for (int j = 0; j < 2; j++)
            {
                string[] newPlayer = new string[stats.Length];
                string playerString = "";
                float totalStats = 0;
                int age;
                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];

                newPlayer[2] = positions[1];

                age = (int)(Random.value * 27) + 18;
                newPlayer[5] = age.ToString();

                for (int k = 6; k < stats.Length; k++)
                {
                    int currStat = (int)(Random.value * age) + 55;
                    newPlayer[k] = currStat.ToString();
                    totalStats += currStat;
                }

                int potential = (int)(Random.value * 25 + (43 - age) * 3);
                if (potential < 0)
                    potential = 0;
                newPlayer[4] = potential.ToString();

                newPlayer[3] = ((totalStats / (stats.Length - 6))).ToString("0.00");

                for (int k = 0; k < newPlayer.Length - 1; k++)
                    playerString += newPlayer[k] + ",";
                playerString += newPlayer[newPlayer.Length - 1];

                PlayerPrefs.SetString("Player" + i + "-" + teams[i].players.Count, playerString);
                teams[i].players.Add(newPlayer);
            }

            PlayerPrefs.SetString("Team" + teams[i].id, teams[i].id + "," + teams[i].cityName + " " + teams[i].teamName + "," + teams[i].pick);
            PlayerPrefs.SetString("Overalls" + teams[i].id, teams[i].overalls[0] + "," + teams[i].overalls[1] + "," + teams[i].overalls[2]);
            PlayerPrefs.SetString("PWL" + teams[i].id.ToString(), "0,0,0");
            PlayerPrefs.SetInt("NumPlayers" + i, teams[i].players.Count);
        }


        for(int i = 0; i < numTeams; i++)
        {
            schedule[i / 2, i % 2] = i;
            strSchedule += i + ",";
        }

        strSchedule = strSchedule.Remove(strSchedule.Length - 1, 1);
        PlayerPrefs.SetString("Schedule", strSchedule);
        PlayerPrefs.Save();
    }
}
