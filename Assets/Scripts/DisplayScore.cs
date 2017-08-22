using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
	public Text textYourScore;			// Displays user's score
	public Text textTheirScore;			// Displays their score
	public Text textResult;				// Displays whether you won or lost
	public Text textWL;					// Displays user's total wins and losses

	private static Text txtYourScore;	// Static reference to the text to display user's score
	private static Text txtTheirScore;	// Static reference to the text to display their score
	private static Text txtResult;		// Static reference to the text to display whether you won or lost
	private static Text txtWL;			// Static reference to the text to display user's total wins and losses

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
