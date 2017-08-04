using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
	public Text textYourScore, textTheirScore, textResult, textWL;
	private static Text txtYourScore, txtTheirScore, txtResult, txtWL;

	void Awake ()
	{
		txtYourScore = textYourScore;
		txtTheirScore = textTheirScore;
		txtResult = textResult;
		txtWL = textWL;
	}

	// Displays the score of a player's game
	public static void Display (int yourWins, int yourLosses, int yourScore, int theirScore)
	{
		string result;

		txtYourScore.text = "You: " + yourScore;

		if (yourScore > theirScore)
		{
			result = "Win";
			txtResult.color = Color.green;
		}
		else
		{
			result = "Loss";
			txtResult.color = Color.red;
		}

		txtResult.text = result;
		txtTheirScore.text = "Them: " + theirScore;
		double bottom = yourWins + yourLosses;
		double ratio = System.Math.Round (yourWins / bottom, 3);
		txtWL.text = "W/L: " + yourWins + "/" + yourLosses + " (" + ratio + ")";
	}
}
