using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputAction aimAction;
    private InputAction leapAction;
    private InputAction noRcsAction;
    private float leapForce = 500f;
    private readonly float aimThreshold = 0.1f;
    //private Vector2[] lastAims;
    //private readonly int aimSmoothingCount = 20;
    private Vector2 aim;
    private Vector2 aimVelocity;
    private readonly float maxAngularAcceleration = 50; // rad/s²

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aimAction = InputSystem.actions.FindAction("Aim");
        leapAction = InputSystem.actions.FindAction("Leap");
        noRcsAction = InputSystem.actions.FindAction("NoRCS");
        //lastAims = new Vector2[aimSmoothingCount];
    }

    void Update()
    {
        Vector2 aimValue = aimAction.ReadValue<Vector2>();

        if (Mathf.Sqrt(Mathf.Pow(aimValue.x, 2f) + Mathf.Pow(aimValue.y, 2f)) > aimThreshold)
        {
            // for (int i = 0; i < aimSmoothingCount - 1; i++)
            // {
            //     lastAims[i] = lastAims[i + 1];
            // }
            // lastAims[4] = aimValue;
            aim = Vector2.SmoothDamp(aim, aimValue, ref aimVelocity, 0.025f);
        }

        if (!noRcsAction.IsPressed())
        {
            float aimX = aim.x;//lastAims.Aggregate(0f, (acc, a) => acc + a.x);
            float aimY = aim.y;//lastAims.Aggregate(0f, (acc, a) => acc + a.y);

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
            //transform.rotation = Quaternion.AngleAxis(angleGoal * Mathf.Rad2Deg, Vector3.forward);
        }

        if (leapAction.WasPressedThisFrame())
        {
            rb.AddRelativeForceX(leapForce);
        }
    }
}
