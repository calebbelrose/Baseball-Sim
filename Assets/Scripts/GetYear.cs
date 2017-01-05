using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetYear : MonoBehaviour {

	// Gets the current year
	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = GameObject.Find("_Manager").GetComponent<AllTeams>().year.ToString();
    }
}
