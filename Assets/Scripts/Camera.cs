using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    private Vector3 velocity;
    private Vector3 lastFollowedPosition;
    private float angleVelocity;
    public bool following;

    void Start()
    {
        following = true;
    }

    void LateUpdate()
    {
        if (following)
        {
            lastFollowedPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
        transform.position = Vector3.SmoothDamp(transform.position, lastFollowedPosition, ref velocity, 0.25f);

        SortedDictionary<float, CameraRotationControlPoint> pointOrientations = new();
        foreach (var point in FindObjectsByType<CameraRotationControlPoint>())
        {
            pointOrientations.Add(Vector2.Distance(transform.position, point.transform.position), point);
        }
        KeyValuePair<float, CameraRotationControlPoint> point0 = pointOrientations.First();
        KeyValuePair<float, CameraRotationControlPoint> point1 = pointOrientations.Skip(1).First();
        float factor = Vector2.Dot(transform.position - point0.Value.transform.position, point1.Value.transform.position - point0.Value.transform.position) / Mathf.Pow(Vector2.Distance(point0.Value.transform.position, point1.Value.transform.position), 2);
        factor = Mathf.Clamp(factor, 0, 1);
        float angleGoal = (1 - factor) * point0.Value.transform.eulerAngles.z + factor * point1.Value.transform.eulerAngles.z;
        transform.rotation = Quaternion.AngleAxis(Mathf.SmoothDampAngle(transform.eulerAngles.z, angleGoal, ref angleVelocity, 1f), Vector3.forward);
    }
}
