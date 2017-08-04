using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayer : MonoBehaviour
{
	private int playerID;
	public List<Text> textObjects;
	public GameObject putOnWaivers, takeOffWaivers;

	// Displays a players information
	public void SetPlayerID (int id)
	{
		playerID = id;

		for (int i = 0; i < 10; i++)
			textObjects [i].text = Manager.Instance.Players [playerID].Skills [i].ToString ();

		textObjects [10].text = Manager.Instance.Players [playerID].Name;
		textObjects [11].text = Manager.Instance.Players [playerID].Age.ToString ();
		textObjects [12].text = ((CountryShortforms) (int)Manager.Instance.Players [playerID].Country).ToString ();
		textObjects [13].text = Manager.Instance.Players [playerID].Position;
		textObjects [14].text = Manager.Instance.Players [playerID].Bats + "/" + Manager.Instance.Players [playerID].Throws;
		textObjects [15].text = Manager.Instance.Players [playerID].Offense.ToString ();
		textObjects [16].text = Manager.Instance.Players [playerID].Defense.ToString ();
		textObjects [17].text = Manager.Instance.Players [playerID].Overall.ToString ();
		textObjects [18].text = Manager.Instance.Players [playerID].Potential.ToString ();

		if (putOnWaivers != null)
		{
			textObjects [19].text = Manager.Instance.Players [playerID].ContractYears [0].Salary.ToString ("N");

			if (Manager.Instance.Players [playerID].OnWaivers)
				takeOffWaivers.SetActive (true);
			else
				putOnWaivers.SetActive (true);
		}
	}

	// Submits an offer for a draft player
	public void SubmitOffer (Text txtOffer)
	{
		double offer = double.Parse (txtOffer.text);

		if (Manager.Instance.Teams [0] [0].Cash >= offer && (offer > Manager.Instance.Players [playerID].Offer || (offer == Manager.Instance.Players [playerID].Offer && Manager.Instance.Teams [0] [0].WLR > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR) || (Manager.Instance.Teams [0] [0].WLR == 0 && Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR == 0 && Manager.Instance.Teams [0] [0].Hype > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].Hype)))
		{
			Manager.Instance.Players [playerID].Offer = offer;
			Manager.Instance.Players [playerID].OfferTime = 7;
			Manager.Instance.Players [playerID].Team = 0;
			Manager.Instance.Players [playerID].SavePlayer ();
		}
	}

	// Puts a player on waivers
	public void PutOnWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].PutOnWaivers (playerID);
		takeOffWaivers.SetActive (true);
		putOnWaivers.SetActive (false);
	}

	// Takes a player off waivers
	public void TakeOffWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].TakeOffWaivers (playerID);
		takeOffWaivers.SetActive (false);
		putOnWaivers.SetActive (true);
	}
}
