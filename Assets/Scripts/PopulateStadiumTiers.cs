using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateStadiumTiers : MonoBehaviour {
	Team team;										// Your team
	Dropdown dropdown;								// The dropdown of stadium tiers
	int tier;										// The current tier
	Text newCapacity, cash, buyPrice, rentPrice;	// Capacity of the next tier, the team's cash and the price to buy/rent the next tier stadium

	// Use this for initialization
	void Start ()
	{
		int start;

		// Gets the text fields and your team
		newCapacity = GameObject.Find ("txtNewCapacity").GetComponent<Text> ();
		cash = GameObject.Find ("txtCurrentCash").GetComponent<Text> ();
		buyPrice = GameObject.Find ("txtNewBuyPrice").GetComponent<Text> ();
		rentPrice = GameObject.Find ("txtNewRentPrice").GetComponent<Text> ();
		team = GameObject.Find ("_Manager").GetComponent<AllTeams> ().teams [0];

		// Displays the stadium tier and capacity
		GameObject.Find ("txtCurrentTier").GetComponent<Text> ().text = team.stadiumTier.ToString();
		GameObject.Find ("txtCurrentCapacity").GetComponent<Text> ().text = team.stadiumCapacity.ToString();

		// Gets the dropdown and sets the start of the tiers to display and the current tier
		dropdown = GetComponent<Dropdown> ();
		start = team.stadiumTier - 5;
		tier = team.stadiumTier;

		// Makes sure the tiers aren't too low
		if (start < 1)
			start = 1;

		// Populates the dropdown with the stadium tiers
		for (int i = start; i < start + 10; i++)
			dropdown.options.Add (new Dropdown.OptionData (i.ToString ()));

		dropdown.value = 0;
	}

	// Calculates and displays the capacity and prices for the specified tier
	public void NewTier()
	{
		int tier = int.Parse (dropdown.options [dropdown.value].text);
		newCapacity.text = ((int)(50000 * System.Math.Pow (1.05, tier))).ToString();
		cash.text = team.cash.ToString();
		buyPrice.text = ((int)(5000000 * System.Math.Pow (1.05, tier))).ToString();
		rentPrice.text = ((int)(250000 * System.Math.Pow (1.05, tier))).ToString();
	}

	// Buy the stadium
	public void BuyStadium()
	{
		team.BuyStadium (tier);
	}

	// Rent the stadium
	public void RentStadium()
	{
		team.RentStadium (tier);
	}
}
