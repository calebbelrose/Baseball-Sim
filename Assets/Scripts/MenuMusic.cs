using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour
{
    void Awake()
    {
        GetComponent<AudioSource>().Play();
        DontDestroyOnLoad(GetComponent<AudioSource>());
    }
}
