using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterDropdown : MonoBehaviour
{
	public string CurrPosition;
	public Lineup lineup;
	public int slot;

	public void ChangePosition (Dropdown dropdown)
	{
		if (dropdown.options [dropdown.value].text != "-")
			lineup.RemovePosition (dropdown.options [dropdown.value].text);
		
		if (CurrPosition != "-" && CurrPosition != dropdown.options [dropdown.value].text)
			lineup.AddPosition (CurrPosition);

		CurrPosition = dropdown.options [dropdown.value].text;
	}
}
