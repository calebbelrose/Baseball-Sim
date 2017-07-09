using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDraftPlayer : MonoBehaviour
{
	private int playerID;
	public List<Text> textObjects;

	public void SetPlayerID(int id)
	{
		playerID = id;

		for (int i = 0; i < 9; i++)
			textObjects [i].text = Manager.Instance.Players [playerID].skills [i].ToString ();

		textObjects [9].text = Manager.Instance.Players [playerID].Name;
		textObjects [10].text = Manager.Instance.Players [playerID].age.ToString();
		textObjects [11].text = ((CountryShortforms)(int)Manager.Instance.Players [playerID].country).ToString();
		textObjects [12].text = Manager.Instance.Players [playerID].position;
		textObjects [13].text = Manager.Instance.Players [playerID].Bats + "/" + Manager.Instance.Players [playerID].Throws;
		textObjects [14].text = Manager.Instance.Players [playerID].offense.ToString();
		textObjects [15].text = Manager.Instance.Players [playerID].defense.ToString();
		textObjects [16].text = Manager.Instance.Players [playerID].overall.ToString();
		textObjects [17].text = Manager.Instance.Players [playerID].potential.ToString();
	}

	public void SubmitOffer(Text offer)
	{
		Manager.Instance.Players [playerID].Offer = double.Parse (offer.text);
	}
}
