using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PopulateDraftPlayers : MonoBehaviour {

    List<Player>[] newPlayers;
	List<Player> draftPlayers;
    int  initialPlayers;
    GameObject draftList, manager;
    RectTransform draftListRect, draftListParentRect;
    AllTeams allTeams;
    int numPlayers = 0, longestFirstName = 10, longestLastName = 9, currSortedStat = 3, currTeam = 0;
    float newWidth = 0.0f;
	char order = 'a';

    void Awake()
    {
        if(allTeams != null && allTeams.needDraft)
            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(4);
    }

    // Use this for initialization
    void Start() {
        string[] positions = { "SP", "RP", "CP", "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };

        int statHeaderLength = 0;
        manager = GameObject.Find("_Manager");
        draftList = GameObject.Find("DraftList");
        allTeams = manager.GetComponent<AllTeams>();
        newPlayers = new List<Player>[allTeams.GetNumTeams()];
        draftListRect = draftList.GetComponent<RectTransform>();
        draftListParentRect = draftList.transform.parent.gameObject.GetComponent<RectTransform>();
		draftPlayers = new List<Player> ();

        for (int i = 0; i < newPlayers.Length; i++)
            newPlayers[i] = new List<Player>();

        while (PlayerPrefs.HasKey("Draft" + numPlayers.ToString()))
        {
			string playerString = PlayerPrefs.GetString("Draft" + numPlayers);
			Player newPlayer = new Player();
			string[] splitPlayer;
			int current;

            numPlayers++;
			splitPlayer = playerString.Split(',');
			newPlayer.firstName = splitPlayer [0];
			newPlayer.lastName = splitPlayer[1];
			newPlayer.position = splitPlayer [2];
			newPlayer.potential = int.Parse(splitPlayer[3]);
			newPlayer.age = int.Parse(splitPlayer [4]);
			current = 5;
			for(int i = 0; i < newPlayer.skills.Length; i++)
				newPlayer.skills[i] = int.Parse(splitPlayer [current++]);
			newPlayer.offense = float.Parse(splitPlayer[current++]);
			newPlayer.defense = float.Parse(splitPlayer[current++]);
			newPlayer.overall= float.Parse(splitPlayer[current]);
        }

        if (numPlayers == 0)
        {
            numPlayers = (int)(Random.value * 5.0f) + 250;

            for (int i = 0; i < numPlayers; i++)
            {
				Player newPlayer = new Player(positions[(int)(Random.value * positions.Length)]);
				draftPlayers.Add (newPlayer);

				PlayerPrefs.SetString("Draft" + i.ToString(), newPlayer.firstName + "," + newPlayer.lastName + "," + newPlayer.position + "," + newPlayer.potential + "," + newPlayer.age + "," + newPlayer.skills[0] + "," + newPlayer.skills[1] + "," + newPlayer.skills[2] + "," + newPlayer.skills[3] + "," + newPlayer.skills[4] + "," + newPlayer.skills[5] + "," + newPlayer.skills[6] + "," + newPlayer.skills[7] + "," + newPlayer.offense + "," + newPlayer.defense + "," + newPlayer.overall);
            }
        }
        PlayerPrefs.Save();

        initialPlayers = numPlayers;
        GameObject draftListHeader = GameObject.Find("DraftListHeader");

        statHeaderLength += longestFirstName + longestLastName + 2;

        for (int i = 2; i < allTeams.stats.Length; i++)
        {
			statHeaderLength += allTeams.stats[i].Length + 1;
        }

        Object header = Resources.Load("Header", typeof(GameObject));
        float prevWidth = 5.0f;
        float totalWidth = (8.04f * (statHeaderLength + 1.0f));
        totalWidth /= -2.0f;

		for (int i = 0; i < allTeams.stats.Length; i++)
        {
            GameObject statHeader = Instantiate(header) as GameObject;
            statHeader.name = "header" + i.ToString();
            statHeader.transform.SetParent(draftListHeader.transform);
            statHeader.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			statHeader.transform.GetChild(0).gameObject.GetComponent<Text>().text = allTeams.stats[i];
            statHeader.GetComponent<Button>().onClick.AddListener(() => StartSorting(statHeader));

            float currWidth;
            if (i > 1)
				currWidth = (8.04f * (allTeams.stats[i].Length + 1));
            else if (i == 1)
                currWidth = (8.04f * (longestLastName + 1));
            else
                currWidth = (8.04f * (longestFirstName + 1));

            newWidth += currWidth;
            totalWidth += currWidth / 2.0f + prevWidth / 2.0f;
            prevWidth = currWidth;
            statHeader.GetComponent<RectTransform>().sizeDelta = new Vector2(currWidth, 20.0f);
            statHeader.GetComponent<RectTransform>().transform.localPosition = new Vector3(totalWidth, 0.0f, 0.0f);
        }

        newWidth -= draftList.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;

        DisplayPlayers();
    }

	// Sorts draft players
	public void StartSorting (GameObject other)
	{
		int headerNum = int.Parse (other.name.Remove (0, 6));
		bool notString;

		if (headerNum <= 1)
			notString = false;
		else
			notString = true;

		if (currSortedStat == headerNum)
		if (order == 'a')
			order = 'd';
		else
			order = 'a';
		else if (notString)
			order = 'd';
		else
			order = 'a';

		if (order == 'a')
			switch (headerNum) {
			case 0:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.position).ToList ();
				break;
			case 3:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.overall).ToList ();
				break;
			case 4:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.offense).ToList ();
				break;
			case 5:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.defense).ToList ();
				break;
			case 6:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.potential).ToList ();
				break;
			case 7:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.age).ToList ();
				break;
			default:
				draftPlayers = draftPlayers.OrderBy (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}
		else
			switch (headerNum) {
			case 0:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.firstName).ToList ();
				break;
			case 1:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.lastName).ToList ();
				break;
			case 2:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.position).ToList ();
				break;
			case 3:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.overall).ToList ();
				break;
			case 4:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.offense).ToList ();
				break;
			case 5:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.defense).ToList ();
				break;
			case 6:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.potential).ToList ();
				break;
			case 7:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.age).ToList ();
				break;
			default:
				draftPlayers = draftPlayers.OrderByDescending (playerX => playerX.skills [headerNum - 8]).ToList ();
				break;
			}

		DisplayPlayers ();
	}

	// Displays draft players
    void DisplayPlayers()
    {
        GameObject[] currPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < currPlayers.Length; i++)
            Destroy(currPlayers[i]);

		for (int i = 0; i < draftPlayers.Count; i++)
		{
			if (draftPlayers[i].firstName.Length > longestFirstName)
				longestFirstName = draftPlayers[i].firstName.Length;

			if (draftPlayers[i].lastName.Length > longestLastName)
				longestLastName = draftPlayers[i].lastName.Length;
		}

		for (int i = 0; i < draftPlayers.Count; i++)
        {
            Object playerButton = Resources.Load("Player", typeof(GameObject));
            GameObject newPlayer = Instantiate(playerButton) as GameObject;
			string playerListing;

			newPlayer.name = "player" + i.ToString();
			newPlayer.transform.SetParent(draftList.transform);

			playerListing = draftPlayers[i].firstName;

			for (int j = draftPlayers[i].firstName.Length; j < longestFirstName; j++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].lastName;

			for (int j = draftPlayers[i].lastName.Length; j < longestLastName; j++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].position;

			for (int k = draftPlayers[i].position.Length; k < allTeams.stats[2].Length; k++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].overall;

			for (int k = draftPlayers[i].overall.ToString().Length; k < allTeams.stats[3].Length; k++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].offense;

			for (int k = draftPlayers[i].offense.ToString().Length; k < allTeams.stats[4].Length; k++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].defense;

			for (int k = draftPlayers[i].defense.ToString().Length; k < allTeams.stats[5].Length; k++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].potential;

			for (int k = draftPlayers[i].potential.ToString().Length; k < allTeams.stats[6].Length; k++)
				playerListing += " ";

			playerListing += " " + draftPlayers[i].age;

			for (int k = draftPlayers[i].age.ToString().Length; k < allTeams.stats[7].Length; k++)
				playerListing += " ";

			for (int j = 0; j < draftPlayers[i].skills.Length - 1; j++)
			{
				playerListing += " " + draftPlayers[i].skills[j];

				for (int k = draftPlayers [i].skills [j].ToString ().Length; k < allTeams.stats [j + 8].Length; k++)
					playerListing += " ";
			}

			playerListing += " " + draftPlayers[i].skills[draftPlayers[i].skills.Length - 1]; 
            
            newPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerListing;
            newPlayer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newPlayer.GetComponent<Button>().onClick.AddListener(() => PlayerDraft(newPlayer, playerListing));
        }

        draftListRect.offsetMin = new Vector2(0, -(20 * (numPlayers + 1) - draftListParentRect.rect.height));
        draftListRect.offsetMax = new Vector2(newWidth, 0);
    }

	// Drafts a player for each team, or until no players are left
    public void PlayerDraft(GameObject player, string playerListing)
    {
        int count = numPlayers;

        Draft(player, player.name.Remove(0, 6));

        if (numPlayers > 30)
            count = 30;

        for(int i = 1; i < count; i++)
        {
            Draft(GameObject.Find("player" + i), "0");
        }
        DisplayPlayers();
    }

	// Draft a player
    public void Draft(GameObject player, string name)
    {
        int playerNum = int.Parse(name);

		newPlayers[currTeam].Add(draftPlayers[playerNum]);
		draftPlayers.RemoveAt (playerNum);

		if (draftPlayers.Count == 0)
        {
            int numTeams = allTeams.GetNumTeams();

            for (int i = 0; i < numTeams; i++)
            {
                float totalBestPlayers = 0.0f, offenseBestPlayers = 0.0f, defenseBestPlayers = 0.0f;
                List<string> starters = new List<string>();

                starters.Add("SP");
                starters.Add("SP");
                starters.Add("SP");
                starters.Add("SP");
                starters.Add("SP");
                starters.Add("RP");
                starters.Add("RP");
                starters.Add("RP");
                starters.Add("CP");
                starters.Add("C");
                starters.Add("1B");
                starters.Add("2B");
                starters.Add("3B");
                starters.Add("SS");
                starters.Add("LF");
                starters.Add("CF");
                starters.Add("RF");
                starters.Add("DH");

                allTeams.teams[i].Batters.Clear();
                allTeams.teams[i].SP.Clear();
                allTeams.teams[i].RP.Clear();
                allTeams.teams[i].CP.Clear();

                while (newPlayers[i].Count > 0)
                {
                    Player newPlayer = newPlayers[i].First();
					newPlayer.playerNumber = allTeams.teams [i].players.Count;
					newPlayer.SavePlayer (i, newPlayer.playerNumber);
                    allTeams.teams[i].players.Add(newPlayer);
                    newPlayers[i].RemoveAt(0);
				}

				List<Player> result = allTeams.teams[i].players.OrderBy(playerX => playerX.position).ThenByDescending(playerX => playerX.overall).ToList<Player>();

                PlayerPrefs.SetInt("NumPlayers" + i, allTeams.teams[i].players.Count);

                for(int j = 0; j < result.Count; j++)
                {
					if (starters.Contains(result[j].position))
                    {
						totalBestPlayers += result[j].overall;
						offenseBestPlayers += (float)(result[j].skills[0] + result[j].skills[1] + result[j].skills[2] + result[j].skills[3]);
						defenseBestPlayers += (float)(result[j].skills[3] + result[j].skills[4] + result[j].skills[5] + result[j].skills[6]);

						if (result[j].position == "SP")
                        {
                            PlayerPrefs.SetInt("SP" + i + "-" + allTeams.teams[i].SP.Count, j);
							allTeams.teams[i].SP.Add(result[j].playerNumber);
                            starters.Remove("SP");
                        }
						else if (result[j].position == "RP")
                        {
                            PlayerPrefs.SetInt("RP" + i + "-" + allTeams.teams[i].RP.Count, j);
							allTeams.teams[i].RP.Add(result[j].playerNumber);
                            starters.Remove("RP");
                        }
						else if (result[j].position == "CP")
                        {
                            PlayerPrefs.SetInt("CP" + i + "-" + allTeams.teams[i].CP.Count, j);
							allTeams.teams[i].CP.Add(result[j].playerNumber);
                            starters.Remove("CP");
                        }
                        else
                        {
                            PlayerPrefs.SetInt("Batter" + i + "-" + allTeams.teams[i].Batters.Count, j);
							allTeams.teams[i].Batters.Add(result[j].playerNumber);
							starters.Remove(result[j].position);
                        }
                    }
                }

				allTeams.teams [i].OrderLineup ();

                allTeams.teams[i].overalls[0] = totalBestPlayers / 18.0f;
                allTeams.teams[i].overalls[1] = offenseBestPlayers / 18.0f;
                allTeams.teams[i].overalls[2] = defenseBestPlayers / 18.0f;
                PlayerPrefs.SetString("Overalls" + allTeams.teams[i].id, allTeams.teams[i].overalls[0] + "," + allTeams.teams[i].overalls[1] + "," + allTeams.teams[i].overalls[2]);
            }

            for (int i = 0; i < initialPlayers; i++)
                PlayerPrefs.DeleteKey("Draft" + i.ToString());

			PlayerPrefs.SetInt("LongestFirstName", Player.longestFirstName);
			PlayerPrefs.SetInt("LongestLastName", Player.longestLastName);

            allTeams.needDraft = false;
            PlayerPrefs.SetString("NeedDraft", allTeams.needDraft.ToString());
            PlayerPrefs.Save();

            GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(4);
        }
        currTeam = (currTeam + 1) % 30;
        Destroy(player);
		numPlayers--;
    }
}