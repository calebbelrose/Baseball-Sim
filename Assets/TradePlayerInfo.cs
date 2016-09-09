using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TradePlayerInfo : MonoBehaviour {

    bool active = false;
    Color yellow = new Color(1.0f, 1.0f, 0.0f);
    public int playerNum, teamNum;

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

    public void AddToTrade()
    {
        if(active)
        {
            GameObject.Find("btnOffer").GetComponent<Trade>().AddPlayer(playerNum, teamNum);
        }
    }
}
