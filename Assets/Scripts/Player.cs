#nullable enable

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public PlayerArms arms;
    private UnityEngine.Camera camera;
    private StartPosition? start;
    private Rigidbody2D rb;
    private InputAction aimAction;
    private InputAction leapAction;
    private InputAction limpAction;
    private readonly float leapForce = 500f;
    private float sensitivity;
    private Vector2 aimValue;
    private Vector2 aim;
    private Vector2 aimVelocity;
    private readonly float maxAngularAcceleration = 50; // rad/s²
    public bool grab;
    public bool canGrab;
    private Vector2 startPos;
    private Quaternion startRot;
    private AudioSource audio;
    public AudioResource jumpAudio;
    public AudioResource landAudio;
    private Rigidbody2D? grabbedRb;
    private Vector2 grabbedPoint;
    // How much the grabbed rigidbody is allowed to rotate when we jump off it, from 0 to 1
    private readonly float rotationToGrabbed = 0.1f;


    void Start()
    {
        arms = FindAnyObjectByType<PlayerArms>();
        camera = FindAnyObjectByType<UnityEngine.Camera>();
        start = FindAnyObjectByType<StartPosition>();
        rb = GetComponent<Rigidbody2D>();
        aimAction = InputSystem.actions.FindAction("Aim");
        leapAction = InputSystem.actions.FindAction("Leap");
        limpAction = InputSystem.actions.FindAction("GoLimp");
        startPos = transform.position;
        startRot = transform.rotation;
        audio = gameObject.AddComponent<AudioSource>();
        UpdateSensitivity();

        Reset();
    }

    public void UpdateSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivity = 0.06f;
        }
    }

    void Update()
    {
        aimValue = aimAction.ReadValue<Vector2>();

        if (leapAction.WasPressedThisFrame() && grab)
        {
            Leap();
        }
    }

    void FixedUpdate()
    {
        if (!limpAction.IsPressed())
        {
            aim = Vector2.SmoothDamp(aim, aimValue, ref aimVelocity, 0.02f);

            // Both angles go from 0 (pointing right) to 2*pi (counterclockwise ofc)
            float angleGoal = (Mathf.Atan2(aim.y, aim.x) + camera.transform.eulerAngles.z * Mathf.Deg2Rad + 2 * Mathf.PI) % (2 * Mathf.PI);
            float angleCurrent = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            float aimPower = math.tanh(Mathf.Sqrt(Mathf.Pow(aimValue.x, 2f) + Mathf.Pow(aimValue.y, 2f)) * sensitivity);

            float error = angleGoal - angleCurrent;
            error -= 2 * Mathf.PI * Mathf.Floor((error + Mathf.PI) / (2 * Mathf.PI));
            error *= aimPower;
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

    public void Grab(Vector2 point, Vector2 normal, Rigidbody2D rb2)
    {
        audio.resource = landAudio;
        audio.Play();
        grab = true;
        //rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        transform.position = point + normal * 0.5f;
        HingeJoint2D? joint = gameObject.AddComponent(typeof(HingeJoint2D)) as HingeJoint2D;
        joint.connectedBody = rb2;
        joint.enableCollision = false;
        grabbedRb = rb2;
        grabbedPoint = rb2.transform.InverseTransformVector(point);
        //gameObject.layer = LayerMask.NameToLayer("NoCollision");
    }

    void Leap()
    {
        audio.resource = jumpAudio;
        audio.Play();
        //gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        //rb.constraints = 0;
        if (TryGetComponent<HingeJoint2D>(out var hinge))
        {
            Destroy(hinge);
        }
        grab = false;
        if (grabbedRb == null || grabbedRb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.AddRelativeForceX(leapForce);
        }
        else
        {
            rb.AddRelativeForceX(leapForce / 2);
            grabbedRb.AddForceAtPosition(leapForce * rotationToGrabbed * -transform.right / 2, grabbedRb.transform.TransformVector(grabbedPoint));
            grabbedRb.AddForce((1 - rotationToGrabbed) * leapForce * -transform.right / 2);
        }
    }

    public void Reset()
    {
        canGrab = true;
        grabbedRb = null;
        if (TryGetComponent<HingeJoint2D>(out var hinge))
        {
            Destroy(hinge);
        }
        if (start != null)
        {
            grab = start.grab;
            transform.SetPositionAndRotation(start.transform.position, start.transform.rotation);
            arms.SetLockedAngle(startRot.eulerAngles.z);
            arms.transform.rotation = startRot;
            if (!grab)
            {
                rb.linearVelocity = start.linearVelocity;
            }
            else
            {
                rb.linearVelocity = new(0, 0);
            }
            rb.angularVelocity = start.angularVelocity;
        }
        else // This should only happen if the spawn is disabled in the editor, in game the player should always have a spawn
        {
            grab = true;
            transform.SetPositionAndRotation(startPos, startRot);
            rb.linearVelocity = new(0, 0);
            rb.angularVelocity = 0;
        }
    }
}
