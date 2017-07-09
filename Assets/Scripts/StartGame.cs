using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Changes to the scene that corresponds to where the player is in the game (draft, normal season, finals, team selection)
    public void ChangeScene()
    {
        if (PlayerPrefs.HasKey("Your Name"))
        {
            if (Manager.Instance.inFinals)
				Manager.ChangeToScene(6);
            else
				Manager.ChangeToScene(4);
        }
        else
			Manager.ChangeToScene(2);
    }
}
