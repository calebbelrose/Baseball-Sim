using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeVolume : MonoBehaviour {

	// Changes the volume when the slider is moved
	public void OnVolumeChange(Slider slider)
    {
        GameObject musicPlayer = GameObject.Find("music");
        if (musicPlayer != null)
        {
            musicPlayer.GetComponent<AudioSource>().volume = slider.value;
        }
    }
}
