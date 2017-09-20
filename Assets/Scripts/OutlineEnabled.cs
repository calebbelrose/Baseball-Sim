using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEnabled : MonoBehaviour
{
	public Outline outline;

	public void Set (bool isEnabled)
	{
		outline.enabled = isEnabled;
	}
}
