using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamInfo : MonoBehaviour {

    public static string yourName, cityName, teamName, teamLogo;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}

    public void SetTeamInfo()
    {
        float teamNum = -(GameObject.Find("viewportTeamLogo").GetComponent<RectTransform>().offsetMin.x) / 160.0f + 1.0f;
        yourName = GameObject.Find("inYourName").GetComponent<Text>().text;
        cityName = GameObject.Find("inCityName").GetComponent<Text>().text;
        teamName = GameObject.Find("inTeamName").GetComponent<Text>().text;
        teamLogo = "team" + (int)teamNum;
    }
}
