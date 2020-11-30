using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour
{
    [SerializeField] Text ironIngotNumber, robotPlanNumber, munitionsNumber;
    public GameObject PanelInventory, PanelPause;
    InventoryManager inventoryManager;
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource;

    void Awake()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        if (PlayerPrefs.GetInt("IsVolumeSave") == 1)
        {
            sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume");
            sliderSoundsEffects.value = PlayerPrefs.GetFloat("SoundsEffectsVolume");
        } else
        {
            sliderMusic.value = 1f;
            sliderSoundsEffects.value = 1f;
        }
    }

    private void Start()
    {
        OnChangeMusicSlider();
        OnChangeSoundsEffectsSlider();
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

    public void OnChangeSoundsEffectsSlider()
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        AudioSource[] sources;
        sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource asource in sources)
        {
            if(asource.tag != "Music")
            {
                asource.volume = sliderSoundsEffects.value;
            }
        }
        PlayerPrefs.SetFloat("SoundsEffectsVolume", sliderSoundsEffects.value);
    }

    public void OnChangeMusicSlider()
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicAudioSource.volume = sliderMusic.value;
        PlayerPrefs.SetFloat("MusicVolume", sliderMusic.value);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("IsVolumeSave", 1);
    }
}
