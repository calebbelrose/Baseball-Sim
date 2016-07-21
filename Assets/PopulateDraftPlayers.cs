using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateDraftPlayers : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int numPlayers = (int)(Random.value * 50.0f) + 250;
        GameObject[] players = new GameObject[numPlayers];
        Object player = Resources.Load("txtPlayer", typeof(GameObject));
        for (int i = 0; i < numPlayers; i++)
            players[i] = Instantiate(player) as GameObject;
	}
}
