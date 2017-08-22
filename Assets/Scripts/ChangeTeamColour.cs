using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTeamColour : MonoBehaviour
{
	// Changes the team's colour
	public void ChangeColour(Color colour)
	{
		Manager.Instance.TeamColour = colour;
	}
}
