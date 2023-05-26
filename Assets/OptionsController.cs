using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI displayDescription;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            displayDescription.text = "Fullscreen";
        }
        else
        {
            displayDescription.text = "Windowed";
        }
    }

    public void SetVolume()
    {
        Debug.Log("setting volume to " +  volumeSlider.value);
    }

    public void ToggleDisplay()
    {
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        } else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }
}
