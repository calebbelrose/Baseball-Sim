using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrade : MonoBehaviour
{
	void Awake ()
	{
		if (Manager.Instance.tradeDeadline == TradeDeadline.Waiver)
			gameObject.SetActive (false);
	}
}
