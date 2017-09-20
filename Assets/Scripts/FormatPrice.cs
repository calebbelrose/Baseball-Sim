using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FormatPrice : MonoBehaviour
{
	public InputField inputField;

	// Formats string to be in a price format
	public void NewPrice (string text)
	{
		double value = double.Parse (text);

		if (value <= 0.00)
			inputField.text = "0.00";
		else
			inputField.text = double.Parse (text).ToString ("F");
	}
}
