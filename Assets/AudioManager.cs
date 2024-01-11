using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundFXSlider;

    // Start is called before the first frame update
    void OnEnable()
    {
        masterMixer.SetFloat("master", Mathf.Log10(PlayerPrefs.GetFloat("masterVol")) * 20);
        masterMixer.SetFloat("music", Mathf.Log10(PlayerPrefs.GetFloat("musicVol")) * 20);
        masterMixer.SetFloat("soundFx", Mathf.Log10(PlayerPrefs.GetFloat("soundFXVol")) * 20);
        if(masterSlider != null){
            masterSlider.value = PlayerPrefs.GetFloat("masterVol");
            musicSlider.value = PlayerPrefs.GetFloat("musicVol");
            soundFXSlider.value = PlayerPrefs.GetFloat("soundFXVol");  
        }

    }
    private void OnDestroy() {
        SaveVolume();
    }
    
    public void SaveVolume(){
        if(masterSlider != null){
            PlayerPrefs.SetFloat("masterVol", masterSlider.value);
            PlayerPrefs.SetFloat("musicVol", musicSlider.value);
            PlayerPrefs.SetFloat("soundFXVol", soundFXSlider.value); 
        }
    }

}
