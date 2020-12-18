using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource, mainMenuAudioSource;
    [SerializeField] AudioClip sndBtnClicked;
    AudioSource[] sources;
    float musicVolume, soundsEffectsVolume;

    void Start()
    {
        mainMenuAudioSource = gameObject.GetComponent<AudioSource>();
        initVolumes();
    }

    void initVolumes()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicVolume = 1f;
        }
        if (PlayerPrefs.HasKey("SoundsEffectsVolume"))
        {
            soundsEffectsVolume = PlayerPrefs.GetFloat("SoundsEffectsVolume");
        }
        else
        {
            soundsEffectsVolume = 1f;
        }
        sliderSoundsEffects.value = soundsEffectsVolume;
        sliderMusic.value = musicVolume;
        updateMusicVolume();
        updateSoundsEffectsVolume();
    }

    public void NewGame()
    {
        PlayButtonClickedSound();
        SceneManager.LoadScene("MapDisplay");
    }

    public void QuitGame()
    {
        PlayButtonClickedSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
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

    public void PlayButtonClickedSound()
    {
        mainMenuAudioSource.volume = soundsEffectsVolume;
        mainMenuAudioSource.PlayOneShot(sndBtnClicked);
    }
}
