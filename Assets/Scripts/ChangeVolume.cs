using UnityEngine;

public class ChangeVolume : MonoBehaviour
{
	// Changes the volume when the slider is moved
	public void OnVolumeChange (UnityEngine.UI.Slider slider)
	{
		GameObject musicPlayer = GameObject.Find("music");

		if (musicPlayer != null)
		{
			musicPlayer.GetComponent<AudioSource>().volume = slider.value;
		}
	}
}
