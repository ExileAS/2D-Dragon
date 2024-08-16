using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCD;
    private float CDTimer = Mathf.Infinity;
    private PlayerMovement playerMovement;
    private Animator anim;
    [SerializeField] private GameObject[] fireBalls = new GameObject[10];
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bufferDuration;
    private bool hasQueuedAttack;
    private float lastAttackTime;

    
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        bool pressedAttack = Input.GetMouseButtonDown(0);

        if(CDTimer > attackCD) {
            if(playerMovement.CanAttack() && (pressedAttack || (hasQueuedAttack && (CDTimer - lastAttackTime) < bufferDuration)))
                Attack();
        } else {
            CDTimer += Time.deltaTime;
            if(pressedAttack) {
                hasQueuedAttack = true;
                lastAttackTime = CDTimer;
            }
        }
    }

    private void Attack() {
        hasQueuedAttack = false;
        int nextInd = ObjectPool.NextInPool(fireBalls);
        if(nextInd == -1) return;

        GameObject nextFireBall = fireBalls[nextInd];

        CDTimer = 0;
        anim.SetTrigger("attack");

        nextFireBall.transform.position = firePoint.position;
        nextFireBall.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
}
