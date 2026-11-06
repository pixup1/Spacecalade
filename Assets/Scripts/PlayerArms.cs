#nullable enable

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerArms : MonoBehaviour
{
    public Player player;
    private InputAction limpAction;
    private float rotationVelocity;
    private float lockedAngle;
    private Rigidbody2D? grabbedRb;

    void Start()
    {
        limpAction = InputSystem.actions.FindAction("GoLimp");
    }

    void Update()
    {
        float angleGoal = lockedAngle;
        if (!player.grab)
        {
            angleGoal = player.transform.eulerAngles.z;
        } else if (grabbedRb != null)
        {
            angleGoal += grabbedRb.transform.eulerAngles.z;
        }

        transform.SetPositionAndRotation(
            player.transform.position,
            Quaternion.AngleAxis(
                Mathf.SmoothDampAngle(transform.eulerAngles.z, angleGoal, ref rotationVelocity, 0.1f),
                Vector3.forward
            )
        );
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (/*!limpAction.IsPressed() &&*/ !player.grab && player.canGrab && collider.GetComponent<Grabbable>() != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.0f);
            if (hit && hit.collider == collider)
            {
                GrabSurface(hit.point, hit.normal, collider.GetComponent<Rigidbody2D>());
            }
        }
    }

    public void GrabSurface(Vector2 point, Vector2 normal, Rigidbody2D rb2)
    {
        player.Grab(point, normal, rb2);
        grabbedRb = rb2;
        lockedAngle = Mathf.Atan2(-normal.y, -normal.x) * Mathf.Rad2Deg - rb2.transform.eulerAngles.z;
    }

    public void SetLockedAngle(float angle)
    {
        lockedAngle = angle;
    }

    public void Reset()
    {
        grabbedRb = null;
    }
}
