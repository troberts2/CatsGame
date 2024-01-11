using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup soundFXMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundFXSlider;

    PlayerMovement pm;
    public static VolumeSettings instance;

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
    }

    public void Back(){
        pm.pauseMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    public void AdjustMaster(float value){
        masterMixer.SetFloat("master", Mathf.Log10(value) * 20);
        SaveVolume();
    }
    public void AdjustMusic(float value){
        masterMixer.SetFloat("music", Mathf.Log10(value) * 20);
        SaveVolume();
    }
    public void AdjustSoundFx(float value){
        masterMixer.SetFloat("soundFx", Mathf.Log10(value) * 20);
        SaveVolume();
    }

    public void SaveVolume(){
        PlayerPrefs.SetFloat("masterVol", masterSlider.value);
        PlayerPrefs.SetFloat("musicVol", musicSlider.value);
        PlayerPrefs.SetFloat("soundFXVol", soundFXSlider.value);
    }
    public void LoadVolume(){
        masterMixer.SetFloat("master", Mathf.Log10(PlayerPrefs.GetFloat("masterVol")) * 20);
        masterSlider.value = PlayerPrefs.GetFloat("masterVol");
        masterMixer.SetFloat("music", Mathf.Log10(PlayerPrefs.GetFloat("musicVol")) * 20);
        musicSlider.value = PlayerPrefs.GetFloat("musicVol");
        masterMixer.SetFloat("soundFx", Mathf.Log10(PlayerPrefs.GetFloat("soundFXVol")) * 20);
        soundFXSlider.value = PlayerPrefs.GetFloat("soundFXVol");
    }
    public void BackToMainMenu(){
        gameObject.SetActive(false);
    }
}
