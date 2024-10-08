using System.Collections;
using UnityEngine;
using static AnimParamsPlayer;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Cool Down")]
    [SerializeField] private float attackCD;
    private float CDTimer = Mathf.Infinity;
    private PlayerMovement playerMovement;
    private Animator anim;

    [Header("Fire Balls")]
    [SerializeField] private GameObject[] fireBalls = new GameObject[10];
    [SerializeField] private Transform firePoint;

    [Header("Attack Buffer Duration")]
    [SerializeField] private float bufferDuration;

    [Header("SFX")]
    [SerializeField] private AudioClip fireBallAudio;
    private bool hasQueuedAttack;
    private float lastAttackTime;
    private Rigidbody2D body;
    private float currentVelocity;

    [Header("Super Fall")]
    [SerializeField] private float fallSpeed;
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private float IFrames;
    [SerializeField] private float chargeTime;
 
    
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        box.gameObject.SetActive(false);
    }

    private void Update() {
        bool pressedAttack = Input.GetMouseButtonDown(0);

        if(CDTimer > attackCD) {
            if(pressedAttack || (hasQueuedAttack && (CDTimer - lastAttackTime) < bufferDuration)) {
                currentVelocity = body.velocity.x;
                StartCoroutine(StopToAttack());
                Attack();
            }
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
        SFXManager.Instance.PlaySound(fireBallAudio);

        nextFireBall.transform.position = firePoint.position;
        nextFireBall.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x), Mathf.Abs(currentVelocity));
    }

    private IEnumerator StopToAttack() {
        anim.SetBool(run, false);
        body.velocity = new Vector2(0, body.velocity.y);
        playerMovement.enabled = false;
        anim.SetTrigger(attack);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 0.3f);
        playerMovement.enabled = true;
    }

    public void SuperFall() {
        StartCoroutine(SuperFallCoroutine());
    }

    private IEnumerator SuperFallCoroutine() {
        body.velocity = new Vector2(body.velocity.x, body.velocity.y - fallSpeed);
        Physics2D.IgnoreLayerCollision(PhysicsLayers.player, PhysicsLayers.enemy, true);
        float timeElapsed = 0;
        while(body.velocity.y < 0) {
            if(Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.S)) {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y - fallSpeed);
                timeElapsed += Time.deltaTime;
            } else timeElapsed = 0;
            if(!box.gameObject.activeInHierarchy && timeElapsed >= chargeTime) {
                box.gameObject.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
        }
        box.gameObject.SetActive(false);
        yield return new WaitForSeconds(IFrames);
        Physics2D.IgnoreLayerCollision(PhysicsLayers.player, PhysicsLayers.enemy, false);
    }
}
