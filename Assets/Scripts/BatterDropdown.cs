using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterDropdown : MonoBehaviour
{
	public string CurrPosition;	// Current position of the batter
	public Lineup Lineup;		// Lineup object to add/remove/get available positions

	// Changes the position of the batter
	public void ChangePosition (Dropdown dropdown)
	{
		if (dropdown.options [dropdown.value].text != "-")
			Lineup.RemovePosition (dropdown.options [dropdown.value].text);
		
		if (CurrPosition != "-" && CurrPosition != dropdown.options [dropdown.value].text)
			Lineup.AddPosition (CurrPosition);

		CurrPosition = dropdown.options [dropdown.value].text;
	}
}