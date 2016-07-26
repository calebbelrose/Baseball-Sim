using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamInfo : MonoBehaviour {

    public static string yourName, teamLogo;

    public void SetTeamInfo()
    {
        float teamNum = -(GameObject.Find("viewportTeamLogo").GetComponent<RectTransform>().offsetMin.x) / 160.0f + 1.0f;
        GameObject manager = GameObject.Find("_Manager");
        yourName = GameObject.Find("inYourName").GetComponent<Text>().text;
        manager.GetComponent<AllTeams>().teams[0].teamName = GameObject.Find("inCityName").GetComponent<Text>().text + " " + GameObject.Find("inTeamName").GetComponent<Text>().text;
        teamLogo = "team" + (int)teamNum;
    }
}
