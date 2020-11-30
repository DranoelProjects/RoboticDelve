using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource;

    void Start()
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("IsVolumeSave") == 1)
        {
            sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume");
            sliderSoundsEffects.value = PlayerPrefs.GetFloat("SoundsEffectsVolume");
        }
        else
        {
            sliderMusic.value = 1f;
            sliderSoundsEffects.value = 1f;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("MapDisplay");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void OnChangeSoundsEffectsSlider()
    {
        AudioSource[] sources;
        sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource asource in sources)
        {
            if (asource.tag != "Music")
            {
                asource.volume = sliderSoundsEffects.value;
            }
        }
        PlayerPrefs.SetFloat("SoundsEffectsVolume", sliderSoundsEffects.value);
        PlayerPrefs.SetInt("IsVolumeSave", 1);
    }

    public void OnChangeMusicSlider()
    {
        musicAudioSource.volume = sliderMusic.value;
        PlayerPrefs.SetFloat("MusicVolume", sliderMusic.value);
        PlayerPrefs.SetInt("IsVolumeSave", 1);
    }
}
