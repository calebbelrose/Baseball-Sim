using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TradePlayerInfo : MonoBehaviour {

    bool active = false;						// Whether the player is part of the trade or not
    Color yellow = new Color(1.0f, 1.0f, 0.0f);	// The colour to change to if the player is part of the trade
    public int playerNum, teamNum;				// The number of the player and their team

	// Changes the colour of the player when it's displayed based on whether it's part of the trade or not
	public void ChangeButtonColour()
    {
        Button button = GetComponent<Button>();
        ColorBlock cb = button.colors;
        active = !active;

        GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);

        if (active)
            cb.normalColor = yellow;
        else
            cb.normalColor = Color.white;

        button.colors = cb;
    }

	// Adds the player to the trade
    public void AddToTrade()
    {
    	GameObject.Find("btnOffer").GetComponent<Trade>().AddPlayer(playerNum, teamNum);
    }
}
