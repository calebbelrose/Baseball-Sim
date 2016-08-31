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
}
