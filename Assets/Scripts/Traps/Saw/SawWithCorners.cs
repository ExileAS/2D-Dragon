using UnityEngine;

public class SawWithCorners : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform leftEdgeTransform;
    private Transform rightEdgeTransform;
    private Transform bottomEdgeTransform;
    private float leftEdge;
    private float rightEdge;
    private float bottomEdge;
    private bool verticalMove;
    private bool moveLeft;
    private bool goUp;
    private float initialX;
    private float initialY;


    private void Start() {
        foreach (Transform item in transform.parent)
        {
            if(item.gameObject.CompareTag("LeftEdge")) leftEdgeTransform = item;
            if(item.gameObject.CompareTag("RightEdge")) rightEdgeTransform = item;
            if(item.gameObject.CompareTag("BottomEdge")) bottomEdgeTransform = item;
        }
        
        leftEdge = leftEdgeTransform.position.x;
        rightEdge = rightEdgeTransform.position.x;
        bottomEdge = bottomEdgeTransform.position.y;
        initialX = transform.position.x;
        initialY = transform.position.y;
    }

    private void Update() {
        if(verticalMove) {
            VerticalMovement();
        } else {
            HorizontalMovement();
        }
    }

    private void HorizontalMovement() {
        goUp = false;
        if(moveLeft) {
            if(transform.position.x <= leftEdge) {
                verticalMove = true;
            } else {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
        } else {
            if(transform.position.x >= rightEdge) {
                verticalMove = true;
            } else {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
        }
    }

    private void VerticalMovement() {
        if(goUp) {
            if(transform.position.y >= initialY) {
                verticalMove = false;
                moveLeft = transform.position.x > initialX;
            }
            else {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
        } else {
            if(transform.position.y <= bottomEdge) {
                goUp = true;
            } else {
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
        }
    }
}
