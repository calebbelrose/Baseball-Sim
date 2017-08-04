using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
	public void RestartGame ()
	{
		Manager.Clear ();
		Manager.Instance.Load ();
	}
}
