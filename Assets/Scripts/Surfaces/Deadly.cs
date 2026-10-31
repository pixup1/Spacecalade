using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Deadly : MonoBehaviour
{
    void Start()
    {
        if (!Application.isEditor)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Player>(out var player))
        {
            player.Reset();
        }
    }
}
