using UnityEngine;
using UnityEngine.UI;

public class MuteControl : MonoBehaviour 
{
	public AudioSource musicPlayer;

	float previousVolume;	// Stores the previous volume level
	UnityEngine.UI.Slider slider;

	// Sets the slider value to the current volume level
	void Awake ()
	{
		slider = GetComponent<UnityEngine.UI.Slider> ();

		if (musicPlayer != null)
			GetComponent<UnityEngine.UI.Slider> ().value = musicPlayer.volume;
	}

	// Sets the volume to 0 when muted and restore to the previous volume level when unmuted
	public void Mute (Toggle toggle)
	{
		if (toggle.isOn)
			slider.value = previousVolume;
		else
		{
			previousVolume = slider.value;
			slider.value = 0.0f;
		}
	}
}
