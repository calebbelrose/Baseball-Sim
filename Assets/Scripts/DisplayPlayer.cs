using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayer : MonoBehaviour
{
	public List<Text> textObjects;		// Text objects to display player's information
	public List<Transform> skillBars;	// Bars to display the player's skills
	public List<Transform> overallBars;	// Bars to display the player's overall skills
	public GameObject putOnWaivers;		// Button to put player on waivers
	public GameObject takeOffWaivers;	// Button to take player off waivers
	public GameObject barPrefab;		// Bar prefab to create bars for pitches
	public GameObject contractPrefab;	// Prefab for contracts
	public Transform contractPanel;		// Panel to hold the contracts
	public Transform pitchPanel;		// Panel to hold the pitches
	public Transform popularity;		// Popularity bar
	public Transform personality;		// Personality bar
	public Image freckles;				// Image of player's freckles
	public Image ear;					// Image of player's ears
	public Image face;					// Image of player's face
	public Image eyeShape;				// Image of player's eye shape
	public Image eyeColour;				// Image of player's eye colour
	public Image hair;					// Image of player's hair
	public Image mouth;					// Image of player's mouth
	public Image nose;					// Image of player's nose

	private int playerID;				// ID of the displayed player

	// Displays a players information
	public void SetPlayerID (int id)
	{
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
			GameObject contract = Instantiate (contractPrefab);

			contract.transform.SetParent (contractPanel);
			contract.transform.GetChild (0).GetComponent<Text> ().text = (Manager.Instance.Year + i) + " " + Manager.Instance.Players [playerID].ContractYears [i].ToString ();
			contract.transform.GetChild (1).GetComponent<Text> ().text = Manager.Instance.Players [playerID].ContractYears [i].Salary.ToString ("C");
			contract.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}

		if (Manager.Instance.Players [playerID].IsPitcher)
		{
			pitchPanel.gameObject.SetActive (true);

			for (int i = 0; i < Manager.Instance.Players [playerID].Pitches.Count; i++)
			{
				GameObject pitch = Instantiate (barPrefab);

				pitch.transform.SetParent (pitchPanel);
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
		hair.sprite = Manager.Instance.HairSprites[Manager.Instance.Players [playerID].Hair];
		mouth.sprite = Manager.Instance.MouthSprites [Manager.Instance.Players [playerID].Mouth];
		nose.sprite = Manager.Instance.NoseSprites [Manager.Instance.Players [playerID].Nose];

		if (putOnWaivers != null)
		{
			if (Manager.Instance.Players [playerID].OnWaivers)
				takeOffWaivers.SetActive (true);
			else
				putOnWaivers.SetActive (true);
		}
	}

	// Submits an offer for a draft player
	public void SubmitOffer (Text txtOffer)
	{
		double offer = double.Parse (txtOffer.text);

		if (Manager.Instance.Teams [0] [0].Cash >= offer && (offer > Manager.Instance.Players [playerID].Offer || (offer == Manager.Instance.Players [playerID].Offer && Manager.Instance.Teams [0] [0].WLR > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR) || (Manager.Instance.Teams [0] [0].WLR == 0 && Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].WLR == 0 && Manager.Instance.Teams [0] [0].Hype > Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].Hype)))
		{
			Manager.Instance.Players [playerID].Offer = offer;
			Manager.Instance.Players [playerID].OfferTime = 7;
			Manager.Instance.Players [playerID].Team = 0;
			Manager.Instance.Players [playerID].SavePlayer ();
		}
	}

	// Puts a player on waivers
	public void PutOnWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].PutOnWaivers (playerID);
		takeOffWaivers.SetActive (true);
		putOnWaivers.SetActive (false);
	}

	// Takes a player off waivers
	public void TakeOffWaivers ()
	{
		Manager.Instance.Teams [0] [Manager.Instance.Players [playerID].Team].TakeOffWaivers (playerID);
		takeOffWaivers.SetActive (false);
		putOnWaivers.SetActive (true);
	}
}
