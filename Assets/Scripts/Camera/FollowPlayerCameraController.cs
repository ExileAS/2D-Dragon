using UnityEngine;

public class FollowPlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float lookAhead;
    private float lookDirection = 1;
    [SerializeField] private float lookAheadDistance;
    [SerializeField] private float lookVerticalMaxDistance;
    [SerializeField] private float camSpeed;
    [SerializeField] private float camSpeedVertical;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothTime;
    [SerializeField] private float heightOffset;

    private void Awake() {
        transform.position = new Vector3(player.position.x, player.position.y + heightOffset, transform.position.z);
    }

    private void Update() {
        FollowPlayer();
        LookUpOrDown();
    }

    private void FollowPlayer() {
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, lookAheadDistance * player.localScale.x, camSpeed * Time.deltaTime);
    }

    private void LookUpOrDown() {
        bool lookingUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool lookingDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        if(!lookingUp && !lookingDown) { // also makes camera keep tracking player Y
            transform.position = Vector3.SmoothDamp(transform.position,
                new Vector3(transform.position.x, player.position.y + heightOffset, transform.position.z), ref velocity, smoothTime);
        } else {
            lookDirection = lookingUp ? 1 : -1;
            transform.position = Vector3.SmoothDamp(transform.position,
                new Vector3(transform.position.x, player.position.y + heightOffset + (lookDirection * lookVerticalMaxDistance), transform.position.z), ref velocity, smoothTime);
        }
    }
}
