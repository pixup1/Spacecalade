#nullable enable

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private readonly int firstLevel = 1;
    private readonly int lastLevel = 3;
    public Material skyboxMaterial;
    private InputAction exitAction;
    public GameObject exitMenuPrefab;
    private GameObject? exitMenu;
    public AudioSource drums;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        exitAction = InputSystem.actions.FindAction("Exit");

        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", firstLevel);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            HideCursor();
            skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0, 0));
            UnmuteDrums();
        }
        else
        {
            skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0.1f, 0));
            MuteDrums();
        }
    }

    void Update()
    {
        if (exitAction.WasPressedThisFrame())
        {
            if (exitMenu == null)
            {
                exitMenu = Instantiate(exitMenuPrefab);
                UnHideCursor();
            }
            else
            {
                Destroy(exitMenu);
                HideCursor();
            }
        }
    }
    
    public void ReturnToMainMenu()
    {
        if (exitMenu != null)
        {
            Destroy(exitMenu);
        }
        UnHideCursor();
        skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0.1f, 0));
        SceneManager.LoadScene("Menu");
        MuteDrums();
    }
    
    public void CloseExitMenu()
    {
        Destroy(exitMenu);
        HideCursor();
    }

    public void LaunchGame()
    {
        int level = GetCurrentLevel();
        LaunchLevel(level);
    }

    public void LaunchLevel(int level)
    {
        if (level >= firstLevel && level <= lastLevel)
        {
            skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0, 0));
            SceneManager.LoadScene("Level" + level);
            UnmuteDrums();
            HideCursor();
        }
    }

    public void NextLevel()
    {
        int level = GetCurrentLevel();
        if (level < lastLevel)
        {
            level++;
            PlayerPrefs.SetInt("CurrentLevel", level);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level" + level);
        }
    }

    private void UnmuteDrums()
    {
        drums.mute = false;
    }
    
    private void MuteDrums()
    {
        drums.mute = true;
    }

    public (int, int) GetLevelRange()
    {
        return (firstLevel, lastLevel);
    }

    public void ResetLevel()
    {
        FindAnyObjectByType<Player>().Reset();
        FindAnyObjectByType<PlayerArms>().Reset();
        foreach (DynamicGrabbable g in FindObjectsByType<DynamicGrabbable>())
        {
            g.Reset();
        }
        foreach (DynamicSlippy g in FindObjectsByType<DynamicSlippy>())
        {
            g.Reset();
        }
    }

    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevel");
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnHideCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
