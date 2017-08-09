using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateStadiumTiers : MonoBehaviour
{
	Team team;											// Your team
	Dropdown dropdown;									// The dropdown of stadium tiers
	int tier;											// The current tier
	public Text newCapacity, currentCash, newBuyPrice, newRentPrice, currentTier, currentCapacity;	// Capacity of the next tier, the team's cash and the price to buy/rent the next tier stadium

	// Use this for initialization
	void Start ()
	{
		int start;

		// Gets the text fields and your team
		team = Manager.Instance.Teams [0] [0];

		// Displays the stadium tier and capacity
		currentTier.text = team.StadiumTier.ToString ();
		currentCapacity.text = team.StadiumCapacity.ToString ();

		// Gets the dropdown and sets the start of the tiers to display and the current tier
		dropdown = GetComponent<Dropdown> ();
		start = team.StadiumTier - 5;
		tier = team.StadiumTier;

		// Makes sure the tiers aren't too low
		if (start < 1)
			start = 1;

		// Populates the dropdown with the stadium tiers
		for (int i = start; i < start + 10; i++)
			dropdown.options.Add (new Dropdown.OptionData (i.ToString ()));

		dropdown.value = 0;
	}

	// Calculates and displays the capacity and prices for the specified tier
	public void NewTier ()
	{
		int tier = int.Parse (dropdown.options [dropdown.value].text);
		newCapacity.text = ((int) (50000 * System.Math.Pow (1.05, tier))).ToString ();
		currentCash.text = team.Cash.ToString ();
		newBuyPrice.text = ((int) (5000000 * System.Math.Pow (1.05, tier))).ToString ();
		newRentPrice.text = ((int) (250000 * System.Math.Pow (1.05, tier))).ToString ();
	}

	// Buy the stadium
	public void BuyStadium ()
	{
		team.BuyStadium (tier);
	}

	// Rent the stadium
	public void RentStadium ()
	{
		team.RentStadium (tier);
	}
}
