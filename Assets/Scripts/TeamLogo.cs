using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamLogo : MonoBehaviour
{
    TeamInfo teamInfo;

    void Start()
    {
        teamInfo = GameObject.Find("_Manager").GetComponent<TeamInfo>();
        GetTeamLogo();
    }

    // Use this for initialization
    public void GetTeamLogo()
    {
        GetComponent<Image>().sprite = teamInfo.teamLogo;
    }

    public void SetTeamLogo(Slider slider)
    {
        teamInfo.teamLogo = Resources.Load<Sprite>("team" + slider.value);
        PlayerPrefs.SetString("Logo", slider.value.ToString());
        PlayerPrefs.Save();
    }
}
