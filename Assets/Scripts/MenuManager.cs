using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelectionMenu;
    public GameObject settingsMenu;
    private GameManager gameManager;
    public Slider sensitivitySlider;
    public Text sensitivityText;
    public Player player;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Awake()
    {
        ReturnToMainMenu();
    }

    public void LaunchGame()
    {
        gameManager.LaunchGame();
    }

    public void OpenLevelSelection()
    {
        mainMenu.SetActive(false);
        levelSelectionMenu.SetActive(true);
    }

    public void OpenSettings()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = 0.06f;
        }
        sensitivityText.text = (sensitivitySlider.value / 0.06f).ToString("0.0");
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        levelSelectionMenu.SetActive(false);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void SetSensitivityValue()
    {
        sensitivityText.text = (sensitivitySlider.value / 0.06f).ToString("0.0");
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.Save();
        player.UpdateSensitivity();
    }
}
