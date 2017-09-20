using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAutomaticRoster : MonoBehaviour
{
	public GameObject pitchers;	// Button to change to the scene to change the order of pitchers
	public GameObject batters;	// Button to change to the scene to change the batting order
	public Toggle toggle;		// Toggles automatic roster on/off

	void Start ()
	{
		toggle.isOn = Manager.Instance.Teams [0] [0].AutomaticRoster;
	}

	public void Set ()
	{
		bool active = !toggle.isOn;

		Manager.Instance.Teams [0] [0].AutomaticRoster = toggle.isOn;
		PlayerPrefs.SetString ("AutomaticRoster", Manager.Instance.Teams [0] [0].AutomaticRoster.ToString ());

		if (!Manager.Instance.Teams [0] [0].AutomaticRoster)
		{
			Manager.Instance.Teams [0] [0].SetRoster ();
			Manager.Instance.Teams [0] [0].SaveBatters ();
			Manager.Instance.Teams [0] [0].SaveSP ();
			Manager.Instance.Teams [0] [0].SaveRP ();
			Manager.Instance.Teams [0] [0].SaveSubstitutes ();
		}
		pitchers.SetActive (active);
		batters.SetActive (active);
	}
}
