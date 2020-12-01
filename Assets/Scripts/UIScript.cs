﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour
{
    [SerializeField] Text ironIngotNumber, robotPlanNumber, munitionsNumber;
    public GameObject PanelInventory, PanelPause;
    [SerializeField] GameObject panelParameters;
    InventoryManager inventoryManager;
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource;
    AudioSource[] sources;
    float musicVolume, soundsEffectsVolume;

    void Awake()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    private void Start()
    {
        initVolumes();
    }

    void initVolumes()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        } else
        {
            musicVolume = 1f;
        }
        if (PlayerPrefs.HasKey("SoundsEffectsVolume"))
        {
            soundsEffectsVolume = PlayerPrefs.GetFloat("SoundsEffectsVolume");
        } else
        {
            soundsEffectsVolume = 1f;
        }
        sliderSoundsEffects.value = soundsEffectsVolume;
        sliderMusic.value = musicVolume;
        updateMusicVolume();
        updateSoundsEffectsVolume();
    }

    public void UpdateInventoryUI()
    {
        ironIngotNumber.text = inventoryManager.IronIngotNumber.ToString();
        robotPlanNumber.text = inventoryManager.RobotPlanNumber.ToString();
        munitionsNumber.text = inventoryManager.MunitionsNumber.ToString();
    }

    public void PauseGame()
    {
        if (!PanelPause.activeInHierarchy)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            panelParameters.SetActive(false);
        }
        PanelPause.SetActive(!PanelPause.activeInHierarchy);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnChangeSoundsEffectsSlider(System.Single vol)
    {
        soundsEffectsVolume = vol;
        updateSoundsEffectsVolume();
    }

    public void OnChangeMusicSlider(System.Single vol)
    {
        musicVolume = vol;
        updateMusicVolume();
    }

    void updateSoundsEffectsVolume()
    {
        sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource asource in sources)
        {
            if (asource.tag != "Music")
            {
                asource.volume = soundsEffectsVolume;
            }
        }
    }

    void updateMusicVolume()
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("MusicVolume", sliderMusic.value);
        PlayerPrefs.SetFloat("SoundsEffectsVolume", sliderSoundsEffects.value);
    }
}
