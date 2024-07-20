using UnityEngine;

public class FollowPlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float lookAhead;
    [SerializeField] private float lookAheadDistance;
    [SerializeField] private float camSpeed;

    private void Update() {
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, lookAheadDistance * player.localScale.x, camSpeed * Time.deltaTime);
    }
}
