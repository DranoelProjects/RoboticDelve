using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour
{
    [SerializeField] Text ironIngotNumber, munitionsNumber, goldNuggetNumber, keyNumber, 
        copperIngotNumber, chlorophyteIngotNumber, leadIngotNumber, cobaltIngotNumber, titaniumIngotNumber, bossName, healthBarBossText, outputPoints, outputLevel;
    public GameObject PanelInventory, PanelPause, PanelBoss;
    [SerializeField] GameObject panelParameters, panelDefeat, panelWin, panelTodo, panelDebug;
    [SerializeField] bool isTutorialScene = false;
    [SerializeField] Image bossFillBar, imageSwapCd;
    InventoryManager inventoryManager;
    [SerializeField] Slider sliderMusic, sliderSoundsEffects;
    AudioSource musicAudioSource, canvasAudioSource;
    [SerializeField] AudioClip sndBtnClicked, music;
    AudioSource[] sources;
    float musicVolume, soundsEffectsVolume, swapCouldownValue, currentSwapCouldDownValue;
    bool isLastPanelBossActive = false, swapCd = false;
    GameManagerScript gameManagerScript;

    void Awake()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    private void Start()
    {
        canvasAudioSource = gameObject.GetComponent<AudioSource>();
        initVolumes();
        UpdatePoints();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PanelInventory.SetActive(!PanelInventory.activeInHierarchy);
            UpdateInventoryUI();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            panelDebug.SetActive(!panelDebug.activeInHierarchy);
        }

        if (swapCd)
        {
            currentSwapCouldDownValue -= Time.deltaTime;
            imageSwapCd.fillAmount = currentSwapCouldDownValue/swapCouldownValue;
            if (currentSwapCouldDownValue < 0)
            {
                swapCd = false;
            }
        }
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
        munitionsNumber.text = inventoryManager.MunitionsNumber.ToString();
        goldNuggetNumber.text = inventoryManager.GoldNuggetAmount.ToString();
        keyNumber.text = inventoryManager.KeyAmount.ToString();
        copperIngotNumber.text = inventoryManager.CopperIngotAmount.ToString();
        chlorophyteIngotNumber.text = inventoryManager.ChlorophyteIngotAmount.ToString();
        leadIngotNumber.text = inventoryManager.LeadIngotAmount.ToString();
        cobaltIngotNumber.text = inventoryManager.CobaltIngotAmount.ToString();
        titaniumIngotNumber.text = inventoryManager.TitaniumIngotAmount.ToString();
    }

    public void PauseGame()
    {
        if (panelDebug.activeInHierarchy)
        {
            panelDebug.SetActive(false);
        }
        PlayerScript playerScript;
        if (gameManagerScript.IsVirtualCamFollowingScientist)
        {
            playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
        else
        {
            playerScript = GameObject.FindGameObjectWithTag("Robot").GetComponent<PlayerScript>();
        }
        if (!PanelPause.activeInHierarchy)
        {
            if (PanelInventory.activeInHierarchy)
            {
                PanelInventory.SetActive(false);
                return;
            }
            playerScript.IsAbleToAttack = false;
            Time.timeScale = 0;
            PanelBoss.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            StartCoroutine(AttackBool(playerScript));
            panelParameters.SetActive(false);
            PanelInventory.SetActive(false);
            if (isLastPanelBossActive)
            {
                PanelBoss.SetActive(true);
            }
        }
        PanelPause.SetActive(!PanelPause.activeInHierarchy);
    }

    IEnumerator AttackBool(PlayerScript playerScript)
    {
        yield return new WaitForSeconds(0.3f);
        playerScript.IsAbleToAttack = true;
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
        if (isTutorialScene)
        {
            panelTodo.SetActive(false);
        }
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

    public void WinLevel()
    {
        if (isTutorialScene)
        {
            panelTodo.SetActive(false);
        }
        GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        gameManagerScript.RobotNumber = 2;
        panelWin.SetActive(true);
    }

    public void StartSwapCouldown(float cdValue)
    {
        swapCd = true;
        currentSwapCouldDownValue = cdValue;
        swapCouldownValue = cdValue;
        imageSwapCd.fillAmount = 1f;
    }

    public void UpdatePoints()
    {
        outputPoints.text = PlayerPrefs.GetInt("Points").ToString();
        outputLevel.text = PlayerPrefs.GetInt("Level").ToString();
    }

    public void OnClickReloadLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
