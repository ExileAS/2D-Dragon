using UnityEngine;

public class DoorDetection : MonoBehaviour
{
    [SerializeField] private Transform prevRoom;
    [SerializeField] private Transform nextRoom;
    private RoomCameraController cam;
    [SerializeField] private SpikeHead spikeHead;

    private void Awake() {
        cam = Camera.main.GetComponent<RoomCameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.CompareTag("Player")) return;

        if(other.transform.position.x < transform.position.x) {
            if(cam.isActiveAndEnabled) cam.MoveCamera(nextRoom);
            if(spikeHead) spikeHead.ResetObject(false);
        } else {
            if(cam.isActiveAndEnabled) cam.MoveCamera(prevRoom);
            if(spikeHead) spikeHead.ResetObject(true);
        }
    }
}
