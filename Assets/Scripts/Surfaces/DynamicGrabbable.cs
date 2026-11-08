using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class DynamicGrabbable : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    private Quaternion startRot;
    private Vector2 startPosVel;
    private float startRotVel;

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
