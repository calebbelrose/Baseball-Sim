using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class TeamInfo : MonoBehaviour {

    public string yourName;
    public Sprite teamLogo;
    GameObject manager;

    void Start()
    {
        if (PlayerPrefs.HasKey("Logo"))
            teamLogo = Resources.Load<Sprite>("team" + PlayerPrefs.GetString("Logo"));
        else
        {
            teamLogo = Resources.Load<Sprite>("team1");
            PlayerPrefs.SetString("Logo", "1");
            PlayerPrefs.Save();
        }
        manager = GameObject.Find("_Manager");
    }

    public void SetTeamInfo()
    {
        if (PlayerPrefs.HasKey("Your Name"))
            yourName = PlayerPrefs.GetString("Your Name");
        else
        {
            Team team = manager.GetComponent<AllTeams>().teams[0];
            string tempYourName = GameObject.Find("inYourName").GetComponent<Text>().text,
                tempCityName = GameObject.Find("inCityName").GetComponent<Text>().text,
                tempTeamName = GameObject.Find("inTeamName").GetComponent<Text>().text;
            if (tempYourName != "")
                yourName = tempYourName;
            else
            {
                string[] firstNames = File.ReadAllLines("FirstNames.txt"),
                lastNames = File.ReadAllLines("LastNames.txt");
                yourName = firstNames[(int)(Random.value * firstNames.Length)] + " " + lastNames[(int)(Random.value * lastNames.Length)];
            }
            if (tempCityName != "")
                team.cityName = tempCityName;
            if (tempTeamName != "")
                team.teamName = tempTeamName;
            PlayerPrefs.SetString("Your Name", yourName);
        }
    }
}
