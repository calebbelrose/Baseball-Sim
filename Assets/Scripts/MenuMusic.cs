using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour
{
	// Makes sure the music persists through scenes
	void Awake ()
	{
		gameObject.name = "music";
		DontDestroyOnLoad (gameObject);
	}
}
