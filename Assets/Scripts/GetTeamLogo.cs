using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetTeamLogo : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Sprite teamLogo = Resources.Load<Sprite>(TeamInfo.teamLogo);
        GetComponent<Image>().sprite = teamLogo;
    }
}
