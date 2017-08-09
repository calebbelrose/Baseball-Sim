using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TradePlayerInfo : MonoBehaviour
{
	public int PlayerID, TeamID;				// The number of the player and their team

	private bool active = false;						// Whether the player is part of the trade or not
	private Color yellow = new Color (1.0f, 1.0f, 0.0f);	// The colour to change to if the player is part of the trade
	private Trade trade;
	private EventSystem eventSystem;

	public void Awake()
	{
		trade = GameObject.Find ("btnOffer").GetComponent<Trade> ();
		eventSystem = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
	}

	// Changes the colour of the player when it's displayed based on whether it's part of the trade or not
	public void ChangeButtonColour ()
	{
		Button button = GetComponent<Button> ();
		ColorBlock cb = button.colors;
		active = !active;

		eventSystem.SetSelectedGameObject (null);

		if (active)
			cb.normalColor = yellow;
		else
			cb.normalColor = Color.white;

		button.colors = cb;
	}

	// Adds the player to the trade
	public void Trade ()
	{
		if (active)
			trade.AddPlayer (PlayerID, TeamID);
		else
			trade.RemovePlayer (PlayerID, TeamID);
	}
}
