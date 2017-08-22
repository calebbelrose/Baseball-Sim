using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetAverage : MonoBehaviour
{
	// Gets the overall for user's team
	void Start ()
	{
		GetComponent<Text> ().text = "Overall: " + Manager.Instance.Teams [0] [0].Overalls [0].ToString ("F");
	}
}