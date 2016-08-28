using UnityEngine;
using System.Collections;

public class TeamInfo : MonoBehaviour {

    public string yourName;
    public Sprite teamLogo;

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
        if (PlayerPrefs.HasKey("Your Name"))
            yourName = PlayerPrefs.GetString("Your Name");
    }
}
