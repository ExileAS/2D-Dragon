using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    private float leftEdge;
    private float rightEdge;
    private bool moveLeft;

    private void Start() {
        leftEdge = transform.position.x - maxDistance;
        rightEdge = transform.position.x + maxDistance;
    }

    private void Update() {
        HorizontalMovement();
    }

    private void HorizontalMovement() {
        if(moveLeft) {
            if(transform.position.x < leftEdge) {
                moveLeft = false;
            } else {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
        } else {
            if(transform.position.x > rightEdge) {
                moveLeft = true;
            } else {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
        }
    }
}
