using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuteControl : MonoBehaviour {

    float previousVolume;

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
