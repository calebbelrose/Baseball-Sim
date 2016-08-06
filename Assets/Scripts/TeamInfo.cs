using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamInfo : MonoBehaviour {

    public static string yourName;
    public Sprite teamLogo;
    GameObject manager;

    void Start()
    {
        teamLogo = Resources.Load<Sprite>("team1");
        manager = GameObject.Find("_Manager");
    }

    public void SetTeamInfo()
    {
        
        yourName = GameObject.Find("inYourName").GetComponent<Text>().text;
        manager.GetComponent<AllTeams>().teams[0].teamName = GameObject.Find("inCityName").GetComponent<Text>().text + " " + GameObject.Find("inTeamName").GetComponent<Text>().text;
    }
}
