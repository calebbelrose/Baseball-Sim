using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetFinances : MonoBehaviour
{
	public GameObject currentCash;			// Displays user cash
	public GameObject currentRevenues;		// Displays user revenues
	public GameObject currentExpenses;		// Displays user expenses
	public GameObject currentProfitLoss;	// Displays user profit (or loss)
	public InputField ticketPrice;			// Gets ticket price from user
	public InputField foodPrice;			// Gets food price from user
	public InputField drinkPrice;			// Gets drink price from user
	public InputField uniformPrice;			// Gets uniform price from user

	void Start ()
	{
		Text profitLoss, cash, revenues, expenses;

		profitLoss = currentProfitLoss.GetComponent<Text> ();
			cash = currentCash.GetComponent<Text> ();

		// Formats the prices
		ticketPrice.text = Manager.Instance.Teams [0] [0].TicketPrice.ToString ("0.00");
		foodPrice.text = Manager.Instance.Teams [0] [0].FoodPrice.ToString ("0.00");
		drinkPrice.text = Manager.Instance.Teams [0] [0].DrinkPrice.ToString ("0.00");
		uniformPrice.text = Manager.Instance.Teams [0] [0].UniformPrice.ToString ("0.00");

		// Gets the text fields
		revenues = currentRevenues.GetComponent<Text> ();
		expenses = currentExpenses.GetComponent<Text> ();

		// Formats the revenue field
		revenues.text = Manager.Instance.Teams [0] [0].Revenues.ToString ("0.00");

		// Changes the colour of text based on whether they're positive/neutral/negative
		if (Manager.Instance.Teams [0] [0].Revenues == 0.00)
			revenues.color = Color.white;

		if (Manager.Instance.Teams [0] [0].Expenses == 0.00)
		{
			RectTransform rt = currentExpenses.GetComponent<RectTransform> ();
			expenses.text = "0.00";
			expenses.color = Color.white;
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
			expenses.text = " (" + Manager.Instance.Teams [0] [0].Expenses.ToString ("0.00") + ")";

		if (Manager.Instance.Teams [0] [0].Cash > 0.00)
		{
			cash.color = Color.green;
			cash.text = Manager.Instance.Teams [0] [0].Cash.ToString ("0.00");
		}
		else if (Manager.Instance.Teams [0] [0].Cash < 0.00)
		{
			RectTransform rt = currentCash.GetComponent<RectTransform> ();
			cash.color = Color.red;
			cash.text = " (" + Manager.Instance.Teams [0] [0].Cash.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			cash.color = Color.white;
			cash.text = Manager.Instance.Teams [0] [0].Cash.ToString ("0.00");
		}

		if (Manager.Instance.Teams [0] [0].Profit > 0.00)
		{
			profitLoss.color = Color.green;
			profitLoss.text = Manager.Instance.Teams [0] [0].Profit.ToString ("0.00");
		}
		else if (Manager.Instance.Teams [0] [0].Profit < 0.00)
		{
			RectTransform rt = currentProfitLoss.GetComponent<RectTransform> ();
			profitLoss.color = Color.red;
			profitLoss.text = " (" + Manager.Instance.Teams [0] [0].Profit.ToString ("0.00") + ")";
			rt.localPosition = new Vector3 (40.0f, rt.localPosition.y, rt.localPosition.z); 
		}
		else
		{
			profitLoss.color = Color.white;
			profitLoss.text = Manager.Instance.Teams [0] [0].Profit.ToString ("0.00");
		}
	}

	// Sets the prices based on the entered prices
	public void SetPrices ()
	{
		Manager.Instance.Teams [0] [0].TicketPrice = double.Parse (ticketPrice.text);
		Manager.Instance.Teams [0] [0].FoodPrice = double.Parse (foodPrice.text);
		Manager.Instance.Teams [0] [0].DrinkPrice = double.Parse (drinkPrice.text);
		Manager.Instance.Teams [0] [0].UniformPrice = double.Parse (uniformPrice.text);
	}
}
