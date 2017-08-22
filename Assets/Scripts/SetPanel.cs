using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPanel : MonoBehaviour
{
	public GameObject panel;	// Panel to set

	void Start ()
	{
		Manager.Instance.SetPanel (panel);
	}
}
