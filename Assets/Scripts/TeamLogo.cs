using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamLogo : MonoBehaviour
{
	void Start ()
	{
		GetTeamLogo ();
	}

	// Gets the team's logo
	public void GetTeamLogo ()
	{
		Image image = GetComponent<Image> ();
		image.sprite = Manager.Instance.TeamLogo;
		image.color = Manager.Instance.TeamColour;
	}

	// Sets the team's logo
	public void SetTeamLogo (UnityEngine.UI.Slider slider)
	{
		Manager.Instance.TeamLogo = Resources.Load<Sprite> ("Logos/team" + slider.value);
		PlayerPrefs.SetString ("Logo", slider.value.ToString ());
		PlayerPrefs.Save ();
	}
}
