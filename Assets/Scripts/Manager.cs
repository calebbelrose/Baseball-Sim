using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour
{
	public static Manager instance;

	// Makes sure the manager persists through scenes
    void Awake()
    {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
    }
}
