using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour
{
    void Awake()
    {
        if(GameObject.Find("music") == null)
        {
            gameObject.name = "music";
            DontDestroyOnLoad(gameObject);
        }
        else if(gameObject.name != "music")
        {
            Destroy(this.gameObject);
        }
    }
}
