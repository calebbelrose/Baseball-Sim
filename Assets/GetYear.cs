using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetYear : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = GameObject.Find("_Manager").GetComponent<AllTeams>().year.ToString();
    }
}
