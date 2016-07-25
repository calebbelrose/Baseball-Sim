using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AllTeams : MonoBehaviour {

    static int numTeams = 30;
    public List<string[]>[] teams = new List<string[]>[numTeams];
    public int[][] wlt = new int[numTeams][];
    public int[] nextTeam = new int[numTeams];
    public float[][] overalls = new float[numTeams][];

    // Use this for initialization
    void Start () {
        Restart();
    }
	
	public int GetNumTeams()
    {
        return numTeams;
    }

    public void Restart()
    {
        for (int i = 0; i < teams.Length; i++)
        {
            teams[i] = new List<string[]>();
            string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };
            string[] stats, firstNames, lastNames;
            stats = File.ReadAllLines("Stats.txt");
            firstNames = File.ReadAllLines("FirstNames.txt");
            lastNames = File.ReadAllLines("LastNames.txt");
            wlt[i] = new int[3];
            overalls[i] = new float[3];

            for (int j = 0; j < wlt[i].Length; j++)
            {
                wlt[i][j] = 0;
            }

            if (i % 2 == 0)
                nextTeam[i] = (i + 1) % numTeams;
            else
                nextTeam[i] = (i - 1) % numTeams;

            for (int j = 0; j < positions.Length; j++)
            {
                string[] newPlayer = new string[stats.Length];
                float totalStats = 0;
                int age;
                newPlayer[0] = firstNames[(int)(Random.value * firstNames.Length)];
                newPlayer[1] = lastNames[(int)(Random.value * lastNames.Length)];

                newPlayer[2] = positions[j];

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
                teams[i].Add(newPlayer);
            }

            for(int j = 0; j < 4; j++)
            {
                string[] newPlayer = new string[stats.Length];
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
                teams[i].Add(newPlayer);
            }

            for (int j = 0; j < 2; j++)
            {
                string[] newPlayer = new string[stats.Length];
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
                teams[i].Add(newPlayer);
            }
        }
    }
}
