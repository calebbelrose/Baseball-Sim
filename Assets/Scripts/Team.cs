using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team {

    public List<string[]> players;
    public List<int> SP, RP, CP, Batters;
    public int[] pwl;
    public float[] overalls;
    public string cityName, teamName;
    public int id, pick;
    string[] stats;
    public List<int[]> pStats;
    public string shortform;
    string[] pStatList = { "G", "AB", "R", "H", "2B", "3B", "HR", "TB", "RBI", "BB", "SO", "SB", "CS", "SAC", "AVG", "OBP", "SLG", "OPS", "W", "L", "ERA", "GS", "SV", "SVO", "IP", "AB", "H", "R", "ER", "HR", "BB", "SO", "AVG", "WHIP" };

    public Team()
    {
        Reset();
    }

    void Reset()
    {
        players = new List<string[]>();
        SP = new List<int>();
        RP = new List<int>();
        CP = new List<int>();
        Batters = new List<int>();
        pStats = new List<int[]>();
        pwl = new int[3];
        overalls = new float[3];
        for (int i = 0; i < pwl.Length; i++)
            pwl[i] = 0;
        stats = new string[pwl.Length + 1];
    }
    
    public void SetStats()
    {
        stats[0] = cityName + " " + teamName;
        stats[1] = pwl[0].ToString();
        stats[2] = pwl[1].ToString();
        stats[3] = pwl[2].ToString();
    }

    public string[] GetStats()
    {
        return stats;
    }

    public int[] NewEmptyStats()
    {
        int[] emptyStats = new int[pStatList.Length];

        for (int i = 0; i < emptyStats.Length; i++)
            emptyStats[i] = 0;

        return emptyStats;
    }
}
