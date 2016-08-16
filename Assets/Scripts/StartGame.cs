using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public void ChangeScene()
    {
        AllTeams allTeams = GameObject.Find("_Manager").GetComponent<AllTeams>();
        if (!allTeams.needDraft)
            GetComponent<ChangeScene>().ChangeToScene(4);
        else if (allTeams.inFinals)
            GetComponent<ChangeScene>().ChangeToScene(6);
        else
            GetComponent<ChangeScene>().ChangeToScene(2);
    }
}
