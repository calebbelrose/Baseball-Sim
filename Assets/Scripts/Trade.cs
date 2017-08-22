using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Trade : MonoBehaviour
{
	public LoadYourPlayers loadYourPlayers;		// Used to load user's players
	public LoadTheirPlayers loadTheirPlayers;	// Used to load the other team's players

	TradeOffer tradeOffer;

	public void Clear (Dropdown dropdown)
	{
		tradeOffer = new TradeOffer (0, dropdown.value + 1);
		tradeOffer.Clear ();
		loadYourPlayers.DisplayPlayers ();
	}

	// Offers your players for their players
	public void Offer ()
	{
		tradeOffer.Accept ();
		loadYourPlayers.Refresh ();
		loadTheirPlayers.Refresh ();
	}

	public TradeOffer TradeOffer
	{
		get
		{
			return tradeOffer;
		}
	}
}
