using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetFinances : MonoBehaviour
{
	public GameObject currentCash, currentRevenues, currentExpenses, currentProfitLoss,	// Game objects to display the corresponding financial information
	ticketPriceObj, foodPriceObj, drinkPriceObj, uniformPriceObj;						// Game objects to display the corresponding financial information
	InputField ticketPrice, foodPrice, drinkPrice, uniformPrice;						// Game objects to retrieve the corresponding financial information
	Team team;																			// The player's team

	void Start ()
	{
		Text profitLoss, cash, revenues, expenses;

		profitLoss = currentProfitLoss.GetComponent<Text>();
			cash = currentCash.GetComponent<Text>();

		team = Manager.Instance.Teams [0] [0];

		// Gets the input fields
		ticketPrice = ticketPriceObj.GetComponent<InputField> ();
		foodPrice = foodPriceObj.GetComponent<InputField> ();
		drinkPrice = drinkPriceObj.GetComponent<InputField> ();
		uniformPrice = uniformPriceObj.GetComponent<InputField> ();

		// Formats the prices
		ticketPrice.text = team.TicketPrice.ToString ("0.00");
		foodPrice.text = team.FoodPrice.ToString ("0.00");
		drinkPrice.text = team.DrinkPrice.ToString ("0.00");
		uniformPrice.text = team.UniformPrice.ToString ("0.00");

		// Gets the text fields
		revenues = currentRevenues.GetComponent<Text> ();
		expenses = currentExpenses.GetComponent<Text> ();

		// Formats the revenue field
		revenues.text = team.Revenues.ToString ("0.00");

		// Changes the colour of text based on whether they're positive/neutral/negative
		if (team.Revenues == 0.00)
			revenues.color = Color.white;

		if (team.Expenses == 0.00)
		{
			RectTransform rt = currentExpenses.GetComponent<RectTransform>();
			expenses.text = "0.00";
			expenses.color = Color.white;
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
			expenses.text = "(" + team.Expenses.ToString ("0.00") + ")";

		if (team.Cash > 0.00)
		{
			cash.color = Color.green;
			cash.text  = team.Cash.ToString ("0.00");
		}
		else if (team.Cash < 0.00)
		{
			RectTransform rt = currentCash.GetComponent<RectTransform>();
			cash.color = Color.red;
			cash.text  = "(" + team.Cash.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			cash.color = Color.white;
			cash.text = team.Cash.ToString ("0.00");
		}

		if (team.Profit > 0.00)
		{
			profitLoss.color = Color.green;
			profitLoss.text  = team.Profit.ToString ("0.00");
		}
		else if (team.Profit < 0.00)
		{
			RectTransform rt = currentProfitLoss.GetComponent<RectTransform>();
			profitLoss.color = Color.red;
			profitLoss.text  = "(" + team.Profit.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			profitLoss.color = Color.white;
			profitLoss.text = team.Profit.ToString ("0.00");
		}
	}

	// Sets the prices based on the entered prices
	public void SetPrices()
	{
		team.TicketPrice = double.Parse (ticketPrice.text);
		team.FoodPrice = double.Parse (foodPrice.text);
		team.DrinkPrice = double.Parse (drinkPrice.text);
		team.UniformPrice = double.Parse (uniformPrice.text);
	}
}
