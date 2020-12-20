using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour
{
    [SerializeField] Text ironIngotNumber, robotPlanNumber, munitionsNumber, goldNuggetNumber, keyNumber, 
        copperIngotNumber, chlorophyteIngotNumber, leadIngotNumber, cobaltIngotNumber, titaniumIngotNumber, bossName, healthBarBossText;
    public GameObject PanelInventory, PanelPause, PanelBoss;
    [SerializeField] GameObject panelParameters, prefabRobotPlanInventory, panelRobotsPlansList, panelDefeat;
    [SerializeField] Image bossFillBar;
    InventoryManager inventoryManager;
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource, canvasAudioSource;
    [SerializeField] AudioClip sndBtnClicked, music;
    AudioSource[] sources;
    float musicVolume, soundsEffectsVolume;
    bool isLastPanelBossActive = false;

    void Awake()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    private void Start()
    {
        canvasAudioSource = gameObject.GetComponent<AudioSource>();
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

    public void UpdateRobotsPlansList()
    {   
        foreach (Transform child in panelRobotsPlansList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<RobotPlanData> playerRobotsPlansArray = inventoryManager.PlayerRobotsPlansArray;

        foreach (RobotPlanData robotData in playerRobotsPlansArray)
        {
            prefabRobotPlanInventory.GetComponentInChildren<Text>().text = robotData.Description;
            Instantiate(prefabRobotPlanInventory, panelRobotsPlansList.transform);
        }
    }

    public void UpdateInventoryUI()
    {
        ironIngotNumber.text = inventoryManager.IronIngotNumber.ToString();
        robotPlanNumber.text = inventoryManager.RobotPlanNumber.ToString();
        munitionsNumber.text = inventoryManager.MunitionsNumber.ToString();
        goldNuggetNumber.text = inventoryManager.GoldNuggetAmount.ToString();
        keyNumber.text = inventoryManager.KeyAmount.ToString();
        copperIngotNumber.text = inventoryManager.CopperIngotAmount.ToString();
        chlorophyteIngotNumber.text = inventoryManager.ChlorophyteIngotAmount.ToString();
        leadIngotNumber.text = inventoryManager.LeadIngotAmount.ToString();
        cobaltIngotNumber.text = inventoryManager.CobaltIngotAmount.ToString();
        titaniumIngotNumber.text = inventoryManager.TitaniumIngotAmount.ToString();
        UpdateRobotsPlansList();
    }

    public void PauseGame()
    {
        if (!PanelPause.activeInHierarchy)
        {
            if (PanelInventory.activeInHierarchy)
            {
                PanelInventory.SetActive(false);
                return;
            }
            Time.timeScale = 0;
            PanelBoss.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            panelParameters.SetActive(false);
            PanelInventory.SetActive(false);
            if (isLastPanelBossActive)
            {
                PanelBoss.SetActive(true);
            }
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

    public void PlayButtonClickedSound()
    {
        canvasAudioSource.volume = soundsEffectsVolume;
        canvasAudioSource.PlayOneShot(sndBtnClicked);
    }

    public void ShowDefeatPanel()
    {
        PanelBoss.SetActive(false);
        panelDefeat.SetActive(!panelDefeat.activeInHierarchy);
    }

    public void PlayBossMusic(AudioClip audioClip)
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicAudioSource.Stop();
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    public void StopBossMusic()
    {
        musicAudioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicAudioSource.Stop();
        musicAudioSource.clip = music;
        musicAudioSource.Play();
    }

    public void ActiveFigthingBossUI(EnemyHealthBarScript healthBarScript, string name)
    {
        isLastPanelBossActive = true;
        PanelBoss.SetActive(true);
        healthBarScript.IsBoss = true;
        healthBarScript.Txt = healthBarBossText;
        healthBarScript.fillBar.enabled = false;
        healthBarScript.FillBarParent.enabled = false;
        healthBarScript.fillBar = bossFillBar;
        bossName.text = name;
    }
}
