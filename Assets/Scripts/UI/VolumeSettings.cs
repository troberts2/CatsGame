using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundFXSlider;

    PlayerMovement pm;

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
    }

    public void Back(){
        pm.pauseMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
