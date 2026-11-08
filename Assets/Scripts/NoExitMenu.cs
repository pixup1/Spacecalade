using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NoExitMenu : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        FindAnyObjectByType<GameManager>().CloseExitMenu();
    }
}
