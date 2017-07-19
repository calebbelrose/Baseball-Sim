using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamLogo : MonoBehaviour
{
	TeamInfo teamInfo;	// The team's logo

	void Start ()
	{
		teamInfo = Manager.Instance.gameObject.GetComponent<TeamInfo> ();
		GetTeamLogo ();
	}

	// Gets the team's logo
	public void GetTeamLogo()
	{
		GetComponent<Image> ().sprite = teamInfo.teamLogo;
	}

	// Sets the team's logo
	public void SetTeamLogo(UnityEngine.UI.Slider slider)
	{
		teamInfo.teamLogo = Resources.Load<Sprite> ("team" + slider.value);
		PlayerPrefs.SetString ("Logo", slider.value.ToString ());
		PlayerPrefs.Save ();
	}
}
