using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayer : MonoBehaviour
{
	public List<Text> textObjects;								// Text objects to display player's information
	public List<Transform> skillBars;							// Bars to display the player's skills
	public List<Transform> overallBars;							// Bars to display the player's overall skills
	public GameObject putOnWaivers;								// Button to put player on waivers
	public GameObject takeOffWaivers;							// Button to take player off waivers
	public GameObject putOnShortDisabledList;					// Button to put player on short disabled list
	public GameObject takeOffShortDisabledList;					// Button to take player off short disabled list
	public GameObject putOnLongDisabledList;					// Button to put player on short disabled list
	public GameObject takeOffLongDisabledList;					// Button to take player off short disabled list
	public GameObject barPrefab;								// Bar prefab to create bars for pitches
	public GameObject contractPrefab;							// Prefab for contracts
	public GameObject newContractPrefab;						// Prefab for new contract
	public GameObject statPanel;								// Panel to hold the players stats
	public Text txtPutOnShortDisabledList;						// Text for button to put player on short disabled list
	public Text txtTakeOffShortDisabledList;					// Text for button to take player off short disabled list
	public Text txtPutOnLongDisabledList;						// Text for button to put player on short disabled list
	public Text txtTakeOffLongDisabledList;						// Text for button to take player off short disabled list
	public Transform contractPanel;								// Panel to hold the contracts
	public Transform pitchPanel;								// Panel to hold the pitches
	public Transform popularity;								// Popularity bar
	public Transform personality;								// Personality bar
	public RectTransform newContractPanel;						// Contracts panel
	public Image freckles;										// Image of player's freckles
	public Image ear;											// Image of player's ears
	public Image face;											// Image of player's face
	public Image eyeShape;										// Image of player's eye shape
	public Image eyeColour;										// Image of player's eye colour
	public Image hair;											// Image of player's hair
	public Image mouth;											// Image of player's mouth
	public Image nose;											// Image of player's nose
	public RectTransform viewport;								// Viewport for the stats
	public RectTransform content;								// Holds the header and player objects
	public Transform statListHeader;							// Header object

	// Stat headers                      0    1     2    3    4     5     6     7     8      9     10    11    12    13     14    15     16     17     18  19    20     21    22    23     24    25    26   27   28    29    30    31    32     33
	private string [] headers = new string [] { "Year", "G", "AB", "R", "H", "2B", "3B", "HR", "TB", "RBI", "BB", "SO", "SB", "CS", "SAC", "BA", "OBP", "SLG", "OPS", "W", "L", "ERA", "GS", "SV", "SVO", "IP", "AB", "H", "R", "ER", "HR", "BB", "SO", "BAA", "WHIP" };
	private int [] headerLengths;								// Lengths of the stat headers
	private List<string []> tempStats;							// Stores all of the stats
	private int playerID;										// ID of the displayed player

	// Displays a players information
	public void SetPlayerID (int id)
	{
		bool active = statPanel.activeSelf;
		List<GameObject> children = new List<GameObject> ();

		playerID = id;

		for (int i = 0; i < skillBars.Count; i++)
		{
			skillBars [i].GetChild (0).GetComponent<Image>().fillAmount = Manager.Instance.Players [playerID].Skills [i] / 100.0f;
			skillBars [i].GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Skills [i].ToString ();
		}

		overallBars [0].GetChild (0).GetComponent<Image>().fillAmount = Manager.Instance.Players [playerID].Offense / 100.0f;
		overallBars [0].GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Offense.ToString ();

		overallBars [1].GetChild (0).GetComponent<Image>().fillAmount = Manager.Instance.Players [playerID].Defense / 100.0f;
		overallBars [1].GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Defense.ToString ();

		overallBars [2].GetChild (0).GetComponent<Image>().fillAmount = Manager.Instance.Players [playerID].Overall / 100.0f;
		overallBars [2].GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Overall.ToString ();

		overallBars [3].GetChild (0).GetComponent<Image>().fillAmount = Manager.Instance.Players [playerID].Potential / 100.0f;
		overallBars [3].GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Potential.ToString ();

		personality.GetChild (0).GetComponent<Image>().fillAmount = (Manager.Instance.Players [playerID].Personality / 2) + 0.5f;
		personality.GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Personality.ToString ("N2");

		popularity.GetChild (0).GetComponent<Image>().fillAmount = (Manager.Instance.Players [playerID].Popularity / 2) + 0.5f;
		popularity.GetChild (1).GetComponent<Text>().text = Manager.Instance.Players [playerID].Popularity.ToString ("N2");

		foreach (Transform child in contractPanel)
			children.Add (child.gameObject);

		foreach (Transform child in pitchPanel)
			children.Add (child.gameObject);

		for (int i = 0; i < children.Count; i++)
			GameObject.Destroy (children [i]);

		children.Clear ();

		for (int i = 0; i < Manager.Instance.Players [playerID].ContractYears.Count; i++)
		{
			GameObject contract = Instantiate (contractPrefab, contractPanel);

			contract.transform.GetChild (0).GetComponent<Text> ().text = (Manager.Instance.Year + i) + " " + Manager.Instance.Players [playerID].ContractYears [i].ToString ();
			contract.transform.GetChild (1).GetComponent<Text> ().text = Manager.Instance.Players [playerID].ContractYears [i].Salary.ToString ("C");
			contract.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}

		if (Manager.Instance.Players [playerID].IsPitcher)
		{
			pitchPanel.gameObject.SetActive (true);

			for (int i = 0; i < Manager.Instance.Players [playerID].Pitches.Count; i++)
			{
				GameObject pitch = Instantiate (barPrefab, pitchPanel);

				pitch.transform.GetChild (1).GetComponent<Image> ().fillAmount = Manager.Instance.Players [playerID].Pitches [i].Effectiveness / 100.0f;
				pitch.transform.GetChild (2).GetComponent<Text> ().text = Manager.Instance.Players [playerID].Pitches [i].Effectiveness.ToString ("N0");
				pitch.transform.GetChild (3).GetComponent<Text> ().text = Manager.Instance.Players [playerID].Pitches [i].ToString ();
				pitch.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}
		}

		textObjects [0].text = Manager.Instance.Players [playerID].Name;
		textObjects [1].text = Manager.Instance.Players [playerID].Age.ToString ();
		textObjects [2].text = ((CountryShortforms) (int)Manager.Instance.Players [playerID].Country).ToString ();
		textObjects [3].text = Manager.Instance.Players [playerID].Position;
		textObjects [4].text = Manager.Instance.Players [playerID].Bats + "/" + Manager.Instance.Players [playerID].Throws;

		freckles.sprite = Manager.Instance.FrecklesSprites [Manager.Instance.Players [playerID].Freckles];
		ear.sprite = Manager.Instance.EarSprites [Manager.Instance.Players [playerID].Ear];
		face.sprite = Manager.Instance.FaceSprites [Manager.Instance.Players [playerID].Face];
		eyeShape.sprite = Manager.Instance.EyeShapeSprites [Manager.Instance.Players [playerID].EyeShape];
		eyeColour.sprite = Manager.Instance.EyeColourSprites [Manager.Instance.Players [playerID].EyeColour];
		hair.sprite = Manager.Instance.HairSprites [Manager.Instance.Players [playerID].Hair];
		mouth.sprite = Manager.Instance.MouthSprites [Manager.Instance.Players [playerID].Mouth];
		nose.sprite = Manager.Instance.NoseSprites [Manager.Instance.Players [playerID].Nose];

		if (putOnWaivers != null)
		{
			if (Manager.Instance.Players [playerID].OnWaivers)
				takeOffWaivers.SetActive (true);
			else
				putOnWaivers.SetActive (true);

			textObjects [5].text = Manager.Instance.Players [playerID].InjuryLength.ToString ();
		}

		if (Manager.Instance.Players [playerID].OnShortDisabledList)
		{
			if (Manager.Instance.Players [playerID].InjuryLength == 0)
			{
				takeOffShortDisabledList.SetActive (true);
				txtTakeOffShortDisabledList.text = "Take off " + Manager.ShortDisabledListTime + " day DL";
			}
			else
				takeOffShortDisabledList.SetActive (false);
			
			takeOffLongDisabledList.SetActive (false);
			putOnShortDisabledList.SetActive (false);
			putOnLongDisabledList.SetActive (false);
		}
		else if (Manager.Instance.Players [playerID].OnLongDisabledList)
		{
			if (Manager.Instance.Players [playerID].InjuryLength == 0)
			{
				takeOffLongDisabledList.SetActive (true);
				txtTakeOffLongDisabledList.text = "Take off " + Manager.LongDisabledListTime + " day DL";
			}
			else
				takeOffLongDisabledList.SetActive (false);

			takeOffShortDisabledList.SetActive (false);
			putOnShortDisabledList.SetActive (false);
			putOnLongDisabledList.SetActive (false);
		}
		else if (Manager.Instance.Players [playerID].InjuryLength > 0)
		{
			putOnShortDisabledList.SetActive (true);
			txtPutOnShortDisabledList.text = "Put on " + Manager.ShortDisabledListTime + " day DL";
			putOnLongDisabledList.SetActive (true);
			txtPutOnLongDisabledList.text = "Put on " + Manager.LongDisabledListTime + " day DL";
			takeOffShortDisabledList.SetActive (false);
			takeOffLongDisabledList.SetActive (false);
		}

		headerLengths = new int [headers.Length];
		tempStats = new List<string []> ();
		statPanel.SetActive (true);

		for (int i = 0; i < headerLengths.Length; i++)
			headerLengths [i] = headers [i].Length;

		for (int i = 0; i < Manager.Instance.Players [playerID].Stats.Count; i++)
		{
			string [] copy = new string [headers.Length];
			int currStat = 0;
			float obp, slug;

			copy [currStat++] = (Manager.Instance.Year - i).ToString ();

			for (int j = 0; j < 14; j++)
				copy [currStat++] = Manager.Instance.Players [playerID].Stats [i] [j].ToString ();

			copy [currStat++] = Manager.Instance.Players [playerID].BA.ToString ("0.000");

			obp = Manager.Instance.Players [playerID].OBP;

			copy [currStat++] = obp.ToString ("0.000");

			slug = Manager.Instance.Players [playerID].SLUG;

			copy [currStat++] = (slug).ToString ("0.000");
			copy [currStat++] = (obp + slug).ToString ("0.000");
			copy [currStat++] = Manager.Instance.Players [playerID].Stats [i] [15].ToString ();
			copy [currStat++] = Manager.Instance.Players [playerID].Stats [i] [16].ToString ();
			copy [currStat++] = Manager.Instance.Players [playerID].ERA.ToString ("F");

			for (int j = 17; j < 20; j++)
				copy [currStat++] = Manager.Instance.Players [playerID].Stats [i] [j].ToString ();

			copy [currStat++] = Manager.Instance.Players [playerID].InningsPitched;

			for (int j = 21; j < 28; j++)
				copy [currStat++] = Manager.Instance.Players [playerID].Stats [i] [j].ToString ();

			copy [currStat++] = Manager.Instance.Players [playerID].BAA.ToString ("0.000");
			copy [currStat++] = Manager.Instance.Players [playerID].WHIP.ToString ("0.000");

			tempStats.Add (copy);
		}

		for (int i = 0; i < tempStats.Count; i++)
		{
			for (int j = 0; j < headers.Length; j++)
				if (tempStats [i] [j].Length > headerLengths [j])
					headerLengths [j] = tempStats [i] [j].Length + 1;
				else
					headerLengths [j]++;
		}

		DisplayHeader ();
		DisplayStats ();
		statPanel.SetActive (active);
	}

	// Submits an offer for a draft player
	public void SubmitOffer ()
	{
		double offer = 0.0, expectedSalary = 0.0;
		int expectedPotential = Manager.Instance.Players [playerID].Potential;
		float expectedOverall = Manager.Instance.Players [playerID].Overall;
		List <ContractYear> newContract = new List<ContractYear> ();

		for (int i = 0; i < newContractPanel.childCount; i++)
		{
			string strSalary = newContractPanel.GetChild (i).GetChild (0).GetComponent<InputField> ().text;

			if (strSalary == "")
				newContract.Add (new ContractYear ((ContractType)newContractPanel.GetChild (i).GetChild (1).GetComponent<Dropdown> ().value, 0.0));
			else
			{
				double salary = double.Parse (strSalary);
				newContract.Add (new ContractYear ((ContractType)newContractPanel.GetChild (i).GetChild (1).GetComponent<Dropdown> ().value, salary));
				offer += salary;
			}

			if (Manager.Instance.Teams [0] [0].Cash >= offer && (offer > Manager.Instance.Players [playerID].Offer || (offer == Manager.Instance.Players [playerID].Offer && Manager.Instance.Teams [0] [0].WLR > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR) || (Manager.Instance.Teams [0] [0].WLR == 0 && Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR == 0 && Manager.Instance.Teams [0] [0].Hype > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].Hype)))
			{
				Manager.Instance.Players [playerID].Offer = offer;
				Manager.Instance.Players [playerID].OfferTime = 7;
				Manager.Instance.Players [playerID].Team = 0;
				Manager.Instance.Players [playerID].SavePlayer ();
			}

			if (expectedOverall < 65.0f)
				expectedSalary += Player.MinSalary;
			else
				expectedSalary += System.Math.Round (((expectedOverall - 65.0f) / 40) * (25000000 - Player.MinSalary + 5000000 * Manager.Instance.RandomGen.NextDouble ()), 2);

			if (expectedPotential <= 0)
				expectedOverall -= ((int)(UnityEngine.Random.value * 10)) / 10.0f;
			else
			{
				int increase = (int)Mathf.Ceil (expectedPotential * 4 / 22f);
				expectedPotential -= increase;
				expectedOverall += increase / 10.0f;
			}

			Debug.Log (expectedPotential + " " + expectedOverall + " " + expectedSalary);
		}

		if (offer > expectedSalary)
		{
			Manager.Instance.Players [playerID].ContractYears.Clear ();

			while(newContract.Count > 0)
			{
				Manager.Instance.Players [playerID].ContractYears.Add (newContract [0]);
				newContract.RemoveAt (0);
			}

			Manager.Instance.Players [playerID].SaveContract ();
			gameObject.SetActive (false);
		}
	}

	public void AddContract ()
	{
		GameObject newContract;

		newContractPanel.sizeDelta = new Vector2 (newContractPanel.sizeDelta.x, newContractPanel.sizeDelta.y + 60.0f);
		newContract = Instantiate (newContractPrefab, newContractPanel.transform);
		newContract.GetComponent<RectTransform> ().localScale = Vector3.one;
		newContract.transform.GetChild (2).GetComponent<Text> ().text = newContractPanel.childCount.ToString ();
	}

	public void RemoveContract ()
	{
		if (newContractPanel.childCount > 0)
		{
			Destroy (newContractPanel.GetChild (newContractPanel.childCount - 1).gameObject);
			newContractPanel.sizeDelta = new Vector2 (newContractPanel.sizeDelta.x, newContractPanel.sizeDelta.y - 60.0f);
		}
	}

	// Puts a player on waivers
	public void PutOnWaivers ()
	{
		if (Manager.Instance.Players [playerID].FirstTimeOnWaivers)
			takeOffWaivers.SetActive (true);
		
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].PutOnWaivers (playerID, false);
		putOnWaivers.SetActive (false);
		Manager.Instance.SaveWaivers ();
	}

	// Takes a player off waivers
	public void TakeOffWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].TakeOffWaivers (playerID);
		takeOffWaivers.SetActive (false);
		putOnWaivers.SetActive (true);
		Manager.Instance.SaveWaivers ();
	}

	public void PutOnShortDisabledList ()
	{
		Manager.Instance.AddToShortDisabledList (playerID);
		Manager.Instance.Players [playerID].InjuryLength = Manager.ShortDisabledListTime;
		Manager.Instance.SaveShortDisabledList ();
	}

	public void TakeOffShortDisabledList ()
	{
		Manager.Instance.RemoveFromShortDisabledListAt (Manager.Instance.ShortDisabledListIndex (playerID));
		Manager.Instance.SaveShortDisabledList ();
	}

	public void PutOnLongDisabledList ()
	{
		Manager.Instance.AddToLongDisabledList (playerID);
		Manager.Instance.Players [playerID].InjuryLength = Manager.LongDisabledListTime;
		Manager.Instance.SaveLongDisabledList ();
	}

	public void TakeOffLongDisabledList ()
	{
		Manager.Instance.RemoveFromLongDisabledListAt (Manager.Instance.ShortDisabledListIndex (playerID));
		Manager.Instance.SaveLongDisabledList ();
	}

	// Displays header
	void DisplayHeader ()
	{
		UnityEngine.Object header = Resources.Load ("Header", typeof (GameObject));
		float newWidth = 0.0f;

		for (int i = 0; i < headers.Length; i++)
		{
			GameObject  statHeader = Instantiate (header, statListHeader.transform) as GameObject;
			statHeader.name = "header" + i.ToString ();
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = headers [i];
			statHeader.GetComponent<Button> ().enabled = false;

			float currWidth = (8.03f * (headerLengths [i]));
			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		content.sizeDelta = new Vector2 (newWidth, 20 * (Manager.Instance.Players [playerID].Stats.Count + 1) - viewport.rect.height);
	}

	// Displays stats from all years
	void DisplayStats ()
	{
		GameObject [] currStats = GameObject.FindGameObjectsWithTag ("Team");
		UnityEngine.Object statButton = Resources.Load ("Team", typeof (GameObject));

		for (int i = 0; i < currStats.Length; i++)
			Destroy (currStats [i]);

		for (int i = 0; i < tempStats.Count; i++)
		{
			string playerListing = "";
			GameObject newStat = Instantiate (statButton, content.transform) as GameObject;

			newStat.name = "player" + i.ToString ();

			for (int j = 0; j < headers.Length - 1; j++)
			{
				int temp = j;

				playerListing += tempStats [i] [temp];

				for (int l = tempStats [i] [temp].Length; l < headerLengths [j]; l++)
					playerListing += " ";
			}

			playerListing += tempStats [i] [tempStats [i].Length - 1];

			newStat.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerListing;
			newStat.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			newStat.GetComponent<Button> ().enabled = false;
		}
	}
}
