using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private readonly int firstLevel = 1;
    private readonly int lastLevel = 2;
    public Material skyboxMaterial;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", firstLevel);
            PlayerPrefs.Save();
        }
    }
    
    void Start()
    {
        skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0.1f, 0));
    }

    public void LaunchGame()
    {
        int level = GetCurrentLevel();
        LaunchLevel(level);
    }

    public void LaunchLevel(int level)
    {
        if (level >= firstLevel && level <= lastLevel) {
            skyboxMaterial.SetVector("_ScrollSpeed", new Vector2(0, 0));
            SceneManager.LoadScene("Level" + level);
            UnmuteDrums();
        }
    }

    public void NextLevel()
    {
        int level = GetCurrentLevel();
        if (level < lastLevel) {
            level++;
            PlayerPrefs.SetInt("CurrentLevel", level);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level" + level);
        }
    }

    private void UnmuteDrums()
    {
        foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
        {
            source.mute = false;
        }
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
}
