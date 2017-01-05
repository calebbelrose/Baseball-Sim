using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FormatPrice : MonoBehaviour {

	// Formats string to be in a price format
	public void NewPrice(string text)
	{
		double value = double.Parse(text);

		if(value <= 0.00)
			gameObject.GetComponent<InputField>().text = "0.00";
		else
			gameObject.GetComponent<InputField>().text = double.Parse(text).ToString("0.00");
	}
}
