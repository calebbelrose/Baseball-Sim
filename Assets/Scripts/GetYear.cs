using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetYear : MonoBehaviour {

	// Gets the current year
	// Use this for initialization
	void Start ()
	{
		GetComponent<Text>().text = Manager.Instance.Year.ToString();
    }
}
