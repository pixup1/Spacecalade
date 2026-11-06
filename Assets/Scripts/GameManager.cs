using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private readonly int firstLevel = 1;
    private readonly int lastLevel = 2;

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

    public void LaunchGame()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel");
        SceneManager.LoadScene("Level" + level);
        UnmuteDrums();
    }

    public void LaunchLevel(int level)
    {
        if (level >= firstLevel && level < lastLevel) {
            SceneManager.LoadScene("Level" + level);
            UnmuteDrums();
        }
    }

    public void NextLevel()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel");
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
}
