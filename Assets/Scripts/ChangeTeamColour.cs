using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTeamColour : MonoBehaviour
{
	public void ChangeColour(Color colour)
	{
		Manager.Instance.TeamColour = colour;
	}
}
