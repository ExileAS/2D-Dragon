using UnityEngine;

public abstract class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float idleDuration;
    [SerializeField] private float maxDistance;

    [Header("Animation")]
    [SerializeField] protected Animator anim;

    private float idleTimer;
    private float leftEdge;
    private float rightEdge;
    private bool moveLeft;
    protected bool canMove = true;
    protected float attackTimer;

    protected virtual void Awake() {
        leftEdge = transform.position.x - maxDistance;
        rightEdge = transform.position.x + maxDistance;
    }

    protected virtual void Update() {
        anim.SetBool("running", canMove);
        if(!canMove) return;

        if(moveLeft) {
            if(transform.position.x > leftEdge) {
                Move(-1);
            } else {
                IdleThenSwitchDirection();
            }
        } else {
            if(transform.position.x < rightEdge) {
                Move(1);
            } else {
                IdleThenSwitchDirection();
            }
        }
    }

    private void Move(float direction) {
        idleTimer = 0;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
        transform.Translate(speed * direction * Time.deltaTime, 0, 0);
    }

    private void IdleThenSwitchDirection() {
        anim.SetBool("running", false);
        idleTimer += Time.deltaTime;

        if(idleTimer > idleDuration) {
            attackTimer = 1.5f;
            moveLeft = !moveLeft;
        }
    }
}
