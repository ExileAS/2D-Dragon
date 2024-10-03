using UnityEngine;

public class SawWithCorners : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxDistanceVertical;
    private float leftEdge;
    private float rightEdge;
    private float bottomEdge;
    private bool moveLeft;
    private bool verticalMove;
    private bool goUp;


    private void Start() {
        leftEdge = transform.position.x - maxDistance;
        rightEdge = transform.position.x + maxDistance;
        bottomEdge = transform.position.y - maxDistanceVertical;
    }

    private void Update() {
        if(verticalMove) {
            VerticalMovement();
        } else {
            HorizontalMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("edge")) {
            verticalMove = !verticalMove;
            goUp = false;
        }
        if(other.CompareTag("upEdge")) {
            if(verticalMove) {
                verticalMove = false;
            }
        }
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

    private void VerticalMovement() {
        if(goUp) {
            transform.Translate(0, speed * Time.deltaTime, 0);
        } else {
            if(transform.position.y < bottomEdge) {
                goUp = true;
            } else {
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
        }
    }
}
