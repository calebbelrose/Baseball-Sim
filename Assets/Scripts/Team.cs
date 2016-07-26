using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team {

    public List<string[]> players;
    public int[] wlt;
    public float[] overalls;
    public string teamName;
    string[] stats;

    public Team()
    {
        Reset();
    }

    void Reset()
    {
        players = new List<string[]>();
        wlt = new int[3];
        overalls = new float[3];
        for (int j = 0; j < wlt.Length; j++)
            wlt[j] = 0;
        stats = new string[4];
    }
    
    public void SetStats()
    {
        stats[0] = teamName;
        stats[1] = wlt[0].ToString();
        stats[2] = wlt[1].ToString();
        stats[3] = wlt[2].ToString();
    }

    public string[] GetStats()
    {
        return stats;
    }
}
