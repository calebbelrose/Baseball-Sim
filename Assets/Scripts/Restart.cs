using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
	public void RestartGame ()
	{
		ScheduledGame.ID = 0;
		Manager.Clear ();
		Manager.Instance.Load ();
	}
}
