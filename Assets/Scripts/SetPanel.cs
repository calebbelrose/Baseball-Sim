using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPanel : MonoBehaviour
{
	public GameObject panel;

	void Start ()
	{
		Manager.Instance.SetPanel (panel);
	}
}
