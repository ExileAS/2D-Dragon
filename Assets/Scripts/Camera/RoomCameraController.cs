using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    private float nextPositionX;

    private void Awake() {
        nextPositionX = transform.position.x;
    }
    private Vector3 velocity = Vector3.zero;

    private void Update() {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(nextPositionX, transform.position.y, transform.position.z), ref velocity, smoothTime);
    }

    public void MoveCamera(Transform cameraTransform) {
        nextPositionX = cameraTransform.position.x;
    }
}
