using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrade : MonoBehaviour
{
	void Awake ()
	{
		if (Manager.Instance.TradeDeadline == TradeDeadline.Waiver)
			gameObject.SetActive (false);
	}
}
