using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Slippy : MonoBehaviour
{
    void Start()
    {
        if (!Application.isEditor)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
        }
    }
}
