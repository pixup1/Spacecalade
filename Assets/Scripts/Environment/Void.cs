using System.Collections;
using UnityEngine;

public class Void : MonoBehaviour
{
    private Camera camera;

    void Start()
    {
        camera = FindAnyObjectByType<Camera>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Player>(out var player))
        {
            StartCoroutine(KillPlayerController(player));
        }
    }

    IEnumerator KillPlayerController(Player player)
    {
        player.canGrab = false;
        camera.following = false;
        yield return new WaitForSeconds(1.5f);
        //TODO: make player not disappear while camera is still panning
        player.Kill();
        camera.following = true;
    }
}
