using UnityEngine;
using System.Collections;

public class TeamInfo : MonoBehaviour {

    public string yourName;	// The player's name
    public Sprite teamLogo;	// The logo for the player's team

    void Start()
    {
		// Loads the logo if there is one, otherwise it sets the logo
        if (PlayerPrefs.HasKey("Logo"))
            teamLogo = Resources.Load<Sprite>("team" + PlayerPrefs.GetString("Logo"));
        else
        {
            teamLogo = Resources.Load<Sprite>("team1");
            PlayerPrefs.SetString("Logo", "1");
            PlayerPrefs.Save();
        }

		// Loads the player's name
        if (PlayerPrefs.HasKey("Your Name"))
            yourName = PlayerPrefs.GetString("Your Name");
    }
}
