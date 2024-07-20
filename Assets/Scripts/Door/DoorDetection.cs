using UnityEngine;

public class DoorDetection : MonoBehaviour
{
    [SerializeField] private Transform prevRoom;
    [SerializeField] private Transform nextRoom;
    private RoomCameraController cam;

    private void Awake() {
        cam = Camera.main.GetComponent<RoomCameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.CompareTag("Player")) return;

        if(other.transform.position.x < transform.position.x) {
            cam.MoveCamera(nextRoom);
        } else {
            cam.MoveCamera(prevRoom);
        }
    }
}
