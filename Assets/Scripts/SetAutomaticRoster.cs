using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAutomaticRoster : MonoBehaviour
{
	public GameObject pitchers, batters;
	public Toggle toggle;

	void Start ()
	{
		toggle.isOn = Manager.Instance.Teams [0] [0].AutomaticRoster;
	}

	public void Set ()
	{
		bool active = !toggle.isOn;

		Manager.Instance.Teams [0] [0].AutomaticRoster = toggle.isOn;
		pitchers.SetActive (active);
		batters.SetActive (active);
	}
}
