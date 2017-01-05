using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetFinances : MonoBehaviour {
	public GameObject currentCash, currentRevenues, currentExpenses, currentProfitLoss,	// Game objects to display the corresponding financial information
	ticketPriceObj, foodPriceObj, drinkPriceObj, uniformPriceObj;						// Game objects to display the corresponding financial information
	InputField ticketPrice, foodPrice, drinkPrice, uniformPrice;						// Game objects to retrieve the corresponding financial information
	Team team;																			// The player's team

	// Use this for initialization
	void Start () {
		Text profitLoss, cash, revenues, expenses;

		profitLoss = currentProfitLoss.GetComponent<Text>();
			cash = currentCash.GetComponent<Text>();

		team = GameObject.Find ("_Manager").GetComponent<AllTeams>().teams [0];

		// Gets the input fields
		ticketPrice = ticketPriceObj.GetComponent<InputField> ();
		foodPrice = foodPriceObj.GetComponent<InputField> ();
		drinkPrice = drinkPriceObj.GetComponent<InputField> ();
		uniformPrice = uniformPriceObj.GetComponent<InputField> ();

		// Formats the prices
		ticketPrice.text = team.ticketPrice.ToString("0.00");
		foodPrice.text = team.foodPrice.ToString("0.00");
		drinkPrice.text = team.drinkPrice.ToString("0.00");
		uniformPrice.text = team.uniformPrice.ToString("0.00");

		// Gets the text fields
		revenues = currentRevenues.GetComponent<Text> ();
		expenses = currentExpenses.GetComponent<Text> ();

		// Formats the revenue field
		revenues.text = team.revenues.ToString("0.00");

		// Changes the colour of text based on whether they're positive/neutral/negative
		if (team.revenues == 0.00)
			revenues.color = Color.white;

		if (team.expenses == 0.00)
		{
			RectTransform rt = currentExpenses.GetComponent<RectTransform>();
			expenses.text = "0.00";
			expenses.color = Color.white;
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
			expenses.text = "(" + team.expenses.ToString ("0.00") + ")";

		if (team.cash > 0.00)
		{
			cash.color = Color.green;
			cash.text  = team.cash.ToString ("0.00");
		}
		else if (team.cash < 0.00)
		{
			RectTransform rt = currentCash.GetComponent<RectTransform>();
			cash.color = Color.red;
			cash.text  = "(" + team.cash.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			cash.color = Color.white;
			cash.text = team.cash.ToString("0.00");
		}

		if (team.profit > 0.00)
		{
			profitLoss.color = Color.green;
			profitLoss.text  = team.profit.ToString ("0.00");
		}
		else if (team.profit < 0.00)
		{
			RectTransform rt = currentProfitLoss.GetComponent<RectTransform>();
			profitLoss.color = Color.red;
			profitLoss.text  = "(" + team.profit.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			profitLoss.color = Color.white;
			profitLoss.text = team.profit.ToString("0.00");
		}
	}

	// Sets the prices based on the entered prices
	public void SetPrices()
	{
		team.ticketPrice = double.Parse(ticketPrice.text);
		team.foodPrice = double.Parse(foodPrice.text);
		team.drinkPrice = double.Parse(drinkPrice.text);
		team.uniformPrice = double.Parse(uniformPrice.text);
	}
}
