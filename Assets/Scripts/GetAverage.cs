using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetAverage : MonoBehaviour {

	// Gets the overall for your team
    void Start()
    {
		GetComponent<Text>().text = "Overall: " + System.Math.Round(Manager.Instance.teams[0].Overalls[0], 2).ToString();
    }
}