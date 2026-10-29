using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerArms : MonoBehaviour
{
    public Player player;
    private InputAction limpAction;
    private float rotationVelocity;
    private float lockedAngle;

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
        if (/*!limpAction.IsPressed() &&*/ player.canGrab && collider.GetComponent<Grabbable>() != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.0f);
            if (hit && hit.collider == collider)
            {
                GrabSurface(hit.point, hit.normal);
            }
        }
    }

    void GrabSurface(Vector2 position, Vector2 normal)
    {
        player.Grab(position + normal*0.5f);
        lockedAngle = Mathf.Atan2(-normal.y, -normal.x) * Mathf.Rad2Deg;
    }
}
