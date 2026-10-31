using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Grabbable : MonoBehaviour
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
