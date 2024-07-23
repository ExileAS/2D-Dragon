using UnityEngine;

public class SawWithCorners : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxDistanceVertical;
    private float leftEdge;
    private float rightEdge;
    private float topEdge;
    private float bottomEdge;
    private bool moveLeft;
    private bool moveUp;
    private bool verticalMove;
    private bool nextMovementDown = true;


    private void Start() {
        leftEdge = transform.position.x - maxDistance;
        rightEdge = transform.position.x + maxDistance;
        topEdge = transform.position.y + maxDistanceVertical;
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
        if(other.CompareTag("Player")) {
            other.GetComponent<Health>().TakeDamage(damage);
        } 

        if(other.CompareTag("edge")) {
            if(nextMovementDown) {
                verticalMove = true;
                nextMovementDown = false;
            } else {
                verticalMove = false;
                nextMovementDown = true;
                moveUp = false;
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
        if(moveUp) {
            if(transform.position.y > topEdge) {
                moveUp = false;
            } else {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
        } else {
            if(transform.position.y < bottomEdge) {
                moveUp = true;
            } else {
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
        }
    }
}
