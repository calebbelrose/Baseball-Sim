using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    void Awake()
    {
        if (GameObject.Find("_Manager") == null)
        {
            gameObject.name = "_Manager";
            DontDestroyOnLoad(gameObject);
        }
        else if (gameObject.name != "_Manager")
        {
            Destroy(this.gameObject);
        }
    }
}
