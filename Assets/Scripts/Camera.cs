using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    private Vector3 velocity;
    private Vector3 lastFollowedPosition;
    public bool following;

    void Start()
    {
        following = true;
    }

    void LateUpdate()
    {
        if (following) {
            lastFollowedPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
        transform.position = Vector3.SmoothDamp(transform.position, lastFollowedPosition, ref velocity, 0.25f);
        print(lastFollowedPosition);
        print(transform.position.z);
    }
}
