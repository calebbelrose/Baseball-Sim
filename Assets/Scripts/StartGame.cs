using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Changes to the scene that corresponds to where the player is in the game (draft, normal season, finals, team selection)
    public void ChangeScene()
    {
        AllTeams allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        if (PlayerPrefs.HasKey("Your Name"))
        {
            if (allTeams.needDraft)
                GetComponent<ChangeScene>().ChangeToScene(3);
            else if (allTeams.inFinals)
                GetComponent<ChangeScene>().ChangeToScene(6);
            else
                GetComponent<ChangeScene>().ChangeToScene(4);
        }
        else
            GetComponent<ChangeScene>().ChangeToScene(2);
    }
}
