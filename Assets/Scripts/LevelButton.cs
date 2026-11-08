using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Text))]
public class LevelButton : MonoBehaviour
{
    public int level;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        Button button = GetComponent<Button>();
        Text text = GetComponentInChildren<Text>();
        text.text = level.ToString();
        (int, int) levelRange = gameManager.GetLevelRange();
        if (level < levelRange.Item1 || level > levelRange.Item2 || level > gameManager.GetCurrentLevel())
        {
            button.interactable = false;
        }
    }
    
    public void StartLevel()
    {
        gameManager.LaunchLevel(level);
    }
}
