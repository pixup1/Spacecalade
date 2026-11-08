using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class DynamicGrabbable : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 startPos;
    Quaternion startRot;
    Vector2 startPosVel;
    float startRotVel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        startRot = transform.rotation;
        startPosVel = rb.linearVelocity;
        startRotVel = rb.angularVelocity;
        if (!Application.isEditor)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
        }
    }
    
    public void Reset()
    {
        transform.SetPositionAndRotation(startPos, startRot);
        rb.linearVelocity = startPosVel;
        rb.angularVelocity = startRotVel;
    }
}
