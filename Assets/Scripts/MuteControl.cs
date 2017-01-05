using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuteControl : MonoBehaviour {

    float previousVolume;	// Stores the previous volume level

	// Sets the slider value to the current volume level
    void Awake()
    {
        GameObject musicPlayer = GameObject.Find("music");
        if (musicPlayer != null)
        {
            GetComponent<Slider>().value = musicPlayer.GetComponent<AudioSource>().volume;
        }
    }

	// Sets the volume to 0 when muted and restore to the previous volume level when unmuted
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
