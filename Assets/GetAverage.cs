using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetAverage : MonoBehaviour {

    void Start()
    {
        GetComponent<Text>().text = GameObject.Find("_Manager").GetComponent<AllTeams>().teams[0].overalls[0].ToString();
    }
}