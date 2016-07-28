using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team {

    public List<string[]> players;
    public int[] pwlt;
    public float[] overalls;
    public string teamName;
    public int id;
    public int pick;
    string[] stats;

    public Team()
    {
        Reset();
    }

    void Reset()
    {
        players = new List<string[]>();
        pwlt = new int[4];
        overalls = new float[3];
        for (int j = 0; j < pwlt.Length; j++)
            pwlt[j] = 0;
        stats = new string[5];
    }
    
    public void SetStats()
    {
        stats[0] = teamName;
        stats[1] = pwlt[0].ToString();
        stats[2] = pwlt[1].ToString();
        stats[3] = pwlt[2].ToString();
        stats[4] = pwlt[3].ToString();
    }

    public string[] GetStats()
    {
        return stats;
    }
}
