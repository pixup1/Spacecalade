using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Missile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public Sprite offSprite;
    private Player player;
    private float angleVelocity;
    private readonly float acceleration = 10.0f;
    private readonly float maxSpeed = 5.0f;
    public bool activated = false;
    public float activationDistance = 5.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        Vector2 toPlayer = player.transform.position - transform.position;
        if (!activated && toPlayer.magnitude <= activationDistance)
        {
            GetComponent<AudioSource>().Play();
            activated = true;
        }
        if (activated)
        {
            Vector2 playerDirection = Vector2.Normalize(toPlayer);
            Vector2 missileDirection = Vector2.Normalize(rb.linearVelocity);
            Vector2 goalDirection = playerDirection - missileDirection;
            if (goalDirection.magnitude < 0.3f)
            {
                goalDirection = playerDirection;
            }
            float goalAngle = Mathf.Atan2(goalDirection.y, goalDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(Mathf.SmoothDampAngle(transform.eulerAngles.z, goalAngle, ref angleVelocity, 0.25f), Vector3.forward);
        }
        
        float speedForward = Vector2.Dot(rb.linearVelocity, transform.up);
        if (speedForward < maxSpeed && activated)
        {
            animator.enabled = true;
            rb.AddRelativeForceY(acceleration);
        }
        else
        {
            animator.enabled = false;
            spriteRenderer.sprite = offSprite;
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     angleVelocity += 1000;
    //     ContactPoint2D contact = collision.GetContact(0);
    //     float angle = Vector2.SignedAngle(transform.up, -contact.normal);
    //     if (Mathf.Abs(angle) < 90)
    //     {
    //         angleVelocity += Mathf.Sign(angle) * 500;
    //     }
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Player>(out var player))
        {
            FindAnyObjectByType<GameManager>().ResetLevel();
        }
    }
}
