using UnityEngine;
using System.Collections;
using System.IO;

public class Stats : MonoBehaviour {

    public string[] statList, firstNames, lastNames;

	// Use this for initialization
	void Start () {
        statList = File.ReadAllLines("Stats.txt");
        firstNames = File.ReadAllLines("FirstNames.txt");
        lastNames = File.ReadAllLines("LastNames.txt");
    }
}
