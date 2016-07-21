using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuteControl : MonoBehaviour {

    float previousVolume;

    void Awake()
    {
        GameObject musicPlayer = GameObject.Find("music");
        if (musicPlayer != null)
        {
            GetComponent<Slider>().value = musicPlayer.GetComponent<AudioSource>().volume;
        }
    }

    public void Mute(Toggle toggle)
    {
        if (toggle.isOn)
        {
            GetComponent<Slider>().value = previousVolume;
        }
        else
        {
            previousVolume = GetComponent<Slider>().value;
            GetComponent<Slider>().value = 0.0f;
        }
    }
}
