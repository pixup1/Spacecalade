using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputAction aimAction;
    private InputAction leapAction;
    private InputAction limpAction;
    private float leapForce = 500f;
    private readonly float aimThreshold = 2.5f;
    private Vector2 aimValue;
    private Vector2 aim;
    private Vector2 aimVelocity;
    private readonly float maxAngularAcceleration = 50; // rad/s²
    public bool grab = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aimAction = InputSystem.actions.FindAction("Aim");
        leapAction = InputSystem.actions.FindAction("Leap");
        limpAction = InputSystem.actions.FindAction("GoLimp");
    }

    void Update()
    {
        aimValue = aimAction.ReadValue<Vector2>();

        if (leapAction.WasPressedThisFrame() && grab)
        {
            UnGrab();
            rb.AddRelativeForceX(leapForce);
        }
    }

    void FixedUpdate()
    {
        if (Mathf.Sqrt(Mathf.Pow(aimValue.x, 2f) + Mathf.Pow(aimValue.y, 2f)) > aimThreshold)
        {
            aim = Vector2.SmoothDamp(aim, aimValue, ref aimVelocity, 0.025f);
        }

        if (!limpAction.IsPressed())
        {
            float aimX = aim.x;
            float aimY = aim.y;

            // Both angles go from 0 (pointing right) to 2*pi (counterclockwise ofc)
            float angleGoal = (Mathf.Atan2(aimY, aimX) + 2 * Mathf.PI) % (2 * Mathf.PI);
            float angleCurrent = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

            float error = angleGoal - angleCurrent;
            error -= 2 * Mathf.PI * Mathf.Floor((error + Mathf.PI) / (2 * Mathf.PI));
            float distance = Mathf.Abs(error);

            float direction = Mathf.Sign(error);

            float angularSpeed = rb.angularVelocity * Mathf.Deg2Rad;

            if (distance > 0.7)
            {
                float stoppingDistance = angularSpeed * angularSpeed / (2 * maxAngularAcceleration);

                float velocityTowardGoal = direction * angularSpeed;

                if (velocityTowardGoal > 0 && distance <= stoppingDistance)
                {
                    rb.AddTorque(-Mathf.Sign(angularSpeed) * maxAngularAcceleration);
                }
                else
                {
                    rb.AddTorque(direction * maxAngularAcceleration);
                }
            }
            else // PD controller takes over for accuracy
            {
                float acc = 15 * error - 2 * angularSpeed;

                rb.AddTorque(Mathf.Clamp(acc, -maxAngularAcceleration, maxAngularAcceleration));
            }
        }
    }

    public void Grab(Vector2 position)
    {
        grab = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        transform.position = position;
        gameObject.layer = LayerMask.NameToLayer("NoCollision");
    }

    void UnGrab()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        rb.constraints = 0;
        grab = false;
    }
}
