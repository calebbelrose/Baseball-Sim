using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetStats : MonoBehaviour
{
	public RectTransform viewport, content;
	public Transform teamListHeader;

	//                                   0    1     2    3    4     5     6     7     8      9     10    11    12    13     14    15     16     17     18  19    20     21    22    23     24    25    26   27   28    29    30    31    32     33
	string [] headers = new string [] { "G", "AB", "R", "H", "2B", "3B", "HR", "TB", "RBI", "BB", "SO", "SB", "CS", "SAC", "BA", "OBP", "SLG", "OPS", "W", "L", "ERA", "GS", "SV", "SVO", "IP", "AB", "H", "R", "ER", "HR", "BB", "SO", "BAA", "WHIP" };
	string [] playerInfoHeaders = new string [] { "First Name", "Last Name", "Position", "Team" };
	int currSortedStat = 10;
	bool ascending = true;
	Team [] teams;
	int [] playerInfoLengths, headerLengths;
	List<string []> tempStats = new List<string []> ();

	void Start ()
	{
		playerInfoLengths = new int [playerInfoHeaders.Length];
		headerLengths = new int [headers.Length];

		for (int i = 0; i < playerInfoLengths.Length; i++)
			playerInfoLengths [i] = playerInfoHeaders [i].Length;

		for (int i = 0; i < headerLengths.Length; i++)
			headerLengths [i] = headers [i].Length;

		teams = new Team [Manager.Instance.GetNumTeams ()];
		Manager.Instance.Teams [0].CopyTo (teams, 0);

		for (int i = 0; i < teams.Length; i++)
			for (int j = 0; j < teams [i].Players.Count; j++)
			{
				string [] copy = new string [playerInfoHeaders.Length + headers.Length];
				int currStat = 0;
				float obp, slug;

				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].FirstName.ToString ();
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].LastName.ToString ();
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Position.ToString ();
				copy [currStat++] = teams [i].Shortform;

				for (int k = 0; k < 14; k++)
					copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Stats [0] [k].ToString ();

				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].BA.ToString ("0.000");

				obp = Manager.Instance.Players [teams [i].Players [j]].OBP;
				
				copy [currStat++] = obp.ToString ("0.000");

				slug = Manager.Instance.Players [teams [i].Players [j]].SLUG;
				
				copy [currStat++] = (slug).ToString ("0.000");
				copy [currStat++] = (obp + slug).ToString ("0.000");
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Stats [0] [15].ToString ();
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Stats [0] [16].ToString ();
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].ERA.ToString ("F");

				for (int k = 17; k < 20; k++)
					copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Stats [0] [k].ToString ();
				
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].InningsPitched;

				for (int k = 21; k < 28; k++)
					copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].Stats [0] [k].ToString ();

				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].BAA.ToString ("0.000");
				copy [currStat++] = Manager.Instance.Players [teams [i].Players [j]].WHIP.ToString ("0.000");

				tempStats.Add (copy);
			}

		for (int i = 0; i < tempStats.Count; i++)
		{
			for (int j = 0; j < playerInfoLengths.Length; j++)
				if (tempStats [i] [j].Length > playerInfoLengths [j])
					playerInfoLengths [j] = tempStats [i] [j].Length;

			for (int j = 0; j < headers.Length; j++)
				if (tempStats [i] [j + playerInfoLengths.Length].Length > headerLengths [j])
					headerLengths [j] = tempStats [i] [j + playerInfoLengths.Length].Length;
		}

		for (int i = 0; i < Manager.Instance.Teams [0].Count; i++)
			if (Manager.Instance.Teams [0] [i].Shortform.Length > playerInfoLengths [playerInfoLengths.Length - 1])
				playerInfoLengths [playerInfoLengths.Length - 1] = Manager.Instance.Teams [0] [i].Shortform.Length;

		for (int i = 0; i < playerInfoLengths.Length; i++)
			playerInfoLengths [i]++;

		for (int i = 0; i < headerLengths.Length; i++)
			headerLengths [i]++;

		IntSort (10, 0, tempStats.Count - 1);
		DisplayHeader ();
		DisplayTeams ();
	}

	// Displays header
	void DisplayHeader ()
	{
		int standingsHeaderLength = -1;
		int totalPlayers = 0;
		Object header = Resources.Load ("Header", typeof (GameObject));
		float newWidth = 0.0f;

		for (int i = 0; i < playerInfoLengths.Length; i++)
			standingsHeaderLength += playerInfoLengths [i];

		for (int i = 0; i < headerLengths.Length; i++)
			standingsHeaderLength += headerLengths [i];

		for (int i = 0; i < teams.Length; i++)
			totalPlayers += teams [i].Players.Count;

		for (int i = 0; i < playerInfoHeaders.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			statHeader.name = "header" + i.ToString ();
			statHeader.transform.SetParent (teamListHeader.transform);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerInfoHeaders [i];
			statHeader.GetComponent<Button> ().onClick.AddListener (() => StartSorting (statHeader.name));

			float currWidth = (8.03f * (playerInfoLengths [i]));
			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		for (int i = 0; i < headers.Length; i++)
		{
			GameObject statHeader = Instantiate (header) as GameObject;
			statHeader.name = "header" + (i + playerInfoHeaders.Length).ToString ();
			statHeader.transform.SetParent (teamListHeader.transform);
			statHeader.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild (0).gameObject.GetComponent<Text> ().text = headers [i];
			statHeader.GetComponent<Button> ().onClick.AddListener (() => StartSorting (statHeader.name));

			float currWidth = (8.03f * (headerLengths [i]));
			newWidth += currWidth;
			statHeader.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currWidth, 20.0f);
		}

		content.sizeDelta = new Vector2 (newWidth, 20 * (totalPlayers + 1) - viewport.rect.height);
	}

	// Displays players from all teams
	public void DisplayTeams ()
	{
		GameObject [] currTeams = GameObject.FindGameObjectsWithTag ("Player");
		Object teamButton = Resources.Load ("Player", typeof (GameObject));

		for (int i = 0; i < currTeams.Length; i++)
			Destroy (currTeams [i]);

		for (int i = 0; i < tempStats.Count; i++)
		{
			string playerListing = "";
			GameObject newTeam = Instantiate (teamButton) as GameObject;

			newTeam.name = "player" + i.ToString ();
			newTeam.transform.SetParent (transform);

			for (int j = 0; j < playerInfoHeaders.Length; j++)
			{
				playerListing += tempStats [i] [j];

				for (int l = tempStats [i] [j].Length; l < playerInfoLengths [j]; l++)
					playerListing += " ";
			}

			for (int j = 0; j < headers.Length - 1; j++)
			{
				int temp = j + playerInfoHeaders.Length;

				playerListing += tempStats [i] [temp];

				for (int l = tempStats [i] [temp].Length; l < headerLengths [j]; l++)
					playerListing += " ";
			}

			playerListing += tempStats [i] [tempStats [i].Length - 1];

			newTeam.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerListing;
			newTeam.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			newTeam.GetComponent<Button> ().interactable = false;

			if (tempStats [i] [playerInfoHeaders.Length - 1] == Manager.Instance.Teams [0] [0].Shortform)
			{
				Button b = newTeam.GetComponent<Button> ();
				ColorBlock c = b.colors;
				c.disabledColor = new Color (1.0f, 1.0f, 0.0f);
				b.colors = c;
			}
		}
	}

	// Starts sorting the players
	public void StartSorting (string name)
	{
		int left = 0, right = tempStats.Count - 1, statNum = int.Parse (name.Remove (0, 6));
		bool isInt = false, isFloat = false;
		int [] intStats = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 22, 23, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35 };
		isInt = intStats.Contains (statNum);

		if (!isInt)
		{
			int [] floatStats = { 18, 19, 20, 21, 24, 28, 35, 36 };
			isFloat = floatStats.Contains (statNum);
		}

		if (currSortedStat == statNum)
			ascending = !ascending;
		else if (isInt || isFloat)
			ascending = false;
		else
			ascending = true;

		if (isInt)
			IntSort (statNum, left, right);
		else if (isFloat)
			FloatSort (statNum, left, right);
		else
			Sort (statNum, left, right);

		currSortedStat = statNum;
		DisplayTeams ();
	}

	// Sorts the players for string stats
	void Sort (int statNum, int left, int right)
	{
		int i = left, j = right;
		string pivot = tempStats [ (int) (left + (right - left) / 2)] [statNum];

		if (ascending)
			while (i <= j)
			{
				while (string.Compare (tempStats [i] [statNum], pivot) < 0)
					i++;

				while (string.Compare (tempStats [j] [statNum], pivot) > 0)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}
		else
			while (i <= j)
			{
				while (string.Compare (tempStats [i] [statNum], pivot) > 0)
					i++;

				while (string.Compare (tempStats [j] [statNum], pivot) < 0)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}

		if (left < j)
			Sort (statNum, left, j);

		if (i < right)
			Sort (statNum, i, right);
	}

	// Sorts the players for integer stats
	void IntSort (int statNum, int left, int right)
	{
		int i = left, j = right;
		int pivot = int.Parse (tempStats [ (int) (left + (right - left) / 2)] [statNum]);

		if (ascending)
			while (i <= j)
			{
				while (int.Parse (tempStats [i] [statNum]) < pivot)
					i++;

				while (int.Parse (tempStats [j] [statNum]) > pivot)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}
		else
			while (i <= j)
			{
				while (int.Parse (tempStats [i] [statNum]) > pivot)
					i++;

				while (int.Parse (tempStats [j] [statNum]) < pivot)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}

		if (left < j)
			IntSort (statNum, left, j);

		if (i < right)
			IntSort (statNum, i, right);
	}

	// Sorts the players for float stats
	void FloatSort (int statNum, int left, int right)
	{
		int i = left, j = right;
		float pivot = float.Parse (tempStats [ (int) (left + (right - left) / 2)] [statNum]);

		if (ascending)
			while (i <= j)
			{
				while (float.Parse (tempStats [i] [statNum]) < pivot)
					i++;

				while (float.Parse (tempStats [j] [statNum]) > pivot)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}
		else
			while (i <= j)
			{
				while (float.Parse (tempStats [i] [statNum]) > pivot)
					i++;

				while (float.Parse (tempStats [j] [statNum]) < pivot)
					j--;

				if (i <= j)
				{
					string [] temp;

					temp = tempStats [i];
					tempStats [i] = tempStats [j];
					tempStats [j] = temp;

					i++;
					j--;
				}
			}

		if (left < j)
			FloatSort (statNum, left, j);

		if (i < right)
			FloatSort (statNum, i, right);
	}
}
