using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    private Vector3 velocity;

    void Update()
    {
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), ref velocity, 0.25f);
    }
}
