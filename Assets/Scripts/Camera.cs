using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.Camera))]
public class Camera : MonoBehaviour
{
    public GameObject player;
    private UnityEngine.Camera camera;
    private Vector3 velocity;
    private Vector3 lastFollowedPosition;
    private Vector3? snapTarget;
    private float angleVelocity;
    private float fovVelocity;
    private Vector3 currentShift;
    public bool following = true;
    public Material skyboxMaterial;

    void Start()
    {
        camera = GetComponent<UnityEngine.Camera>();
    }

    void LateUpdate()
    {
        if (following)
        {
            if (snapTarget == null)
            {
                lastFollowedPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z) + currentShift;
            }
            else if (snapTarget != null)
            {
                lastFollowedPosition = snapTarget ?? lastFollowedPosition;
            }
        }
        transform.position = Vector3.SmoothDamp(transform.position, lastFollowedPosition, ref velocity, 0.25f);

        SortedDictionary<float, CameraRotationControlPoint> pointOrientations = new();
        foreach (var point in FindObjectsByType<CameraRotationControlPoint>())
        {
            pointOrientations.Add(Vector2.Distance(transform.position, point.transform.position), point);
        }
        if (pointOrientations.Count >= 2)
        {
            KeyValuePair<float, CameraRotationControlPoint> point0 = pointOrientations.First();
            KeyValuePair<float, CameraRotationControlPoint> point1 = pointOrientations.Skip(1).First();
            float factor = Vector2.Dot(transform.position - point0.Value.transform.position, point1.Value.transform.position - point0.Value.transform.position) / Mathf.Pow(Vector2.Distance(point0.Value.transform.position, point1.Value.transform.position), 2);
            factor = Mathf.Clamp(factor, 0, 1);

            float angleGoal = Mathf.LerpAngle(point0.Value.transform.eulerAngles.z, point1.Value.transform.eulerAngles.z, factor);
            transform.rotation = Quaternion.AngleAxis(Mathf.SmoothDampAngle(transform.eulerAngles.z, angleGoal, ref angleVelocity, 1f), Vector3.forward);
            skyboxMaterial.SetFloat("_CameraRoll", transform.eulerAngles.z);

            float fovGoal = (1 - factor) * point0.Value.fov + factor * point1.Value.fov;
            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fovGoal, ref fovVelocity, 1f);

            currentShift = (1 - factor) * point0.Value.shift + factor * point1.Value.shift;

            if (factor <= 0.5 && point0.Value.snap)
            {
                snapTarget = new Vector3(point0.Value.transform.position.x, point0.Value.transform.position.y, transform.position.z);
            }
            else if (factor > 0.5 && point1.Value.snap)
            {
                snapTarget = new Vector3(point1.Value.transform.position.x, point1.Value.transform.position.y, transform.position.z);
            }
            else
            {
                snapTarget = null;
            }
        }
    }
}
