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

    [Header("Super Fall")]
    [SerializeField] private GameObject fallHitBox;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float IFrames;
    [SerializeField] private float chargeTime;

    private bool hasQueuedAttack;
    private float lastAttackTime;
    private Rigidbody2D body;
    private float currentVelocity;
    
    private struct PlayerInputs // for inputs affecting physics calcs.
    {
        public bool attack;
        public bool plunge;
    }
    private PlayerInputs playerInputs;
    
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        fallHitBox.SetActive(false);
    }

    private void Update() 
    {
        CollectInputs();
        if(CDTimer < attackCD) {
            CDTimer += Time.deltaTime;
            if(playerInputs.attack) {
                hasQueuedAttack = true;
                lastAttackTime = CDTimer;
            }
        }
    }

    private void FixedUpdate() 
    {
        if(playerInputs.plunge && CanPlunge()) {
            SuperFall();
        }

        if(CDTimer >= attackCD && CanAttack()) {
            currentVelocity = body.velocity.x;
            StartCoroutine(StopToAttack());
            Attack();
        }

        ResetInputs();
    }

    private void CollectInputs() {
        if(Input.GetMouseButtonDown(0)) playerInputs.attack = true;
        if(Input.GetKeyDown(KeyCode.C)) playerInputs.plunge = true;
    }

    private void ResetInputs() {
        playerInputs.attack = false;
        playerInputs.plunge = false;
    }

    private bool CanAttack() {
        return playerMovement.CanAttack() &&
        (playerInputs.attack || (hasQueuedAttack && (CDTimer - lastAttackTime) < bufferDuration));
    }

    private bool CanPlunge() {
        return !PlungeMinDistance.touchingGround && playerMovement.IsInAir();
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
            if(Input.GetKey(KeyCode.C)) {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y - fallSpeed);
                timeElapsed += Time.deltaTime;
            } else timeElapsed = 0;
            if(!fallHitBox.activeInHierarchy && timeElapsed >= chargeTime) {
                fallHitBox.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
        }
        fallHitBox.SetActive(false);
        yield return new WaitForSeconds(IFrames);
        Physics2D.IgnoreLayerCollision(PhysicsLayers.player, PhysicsLayers.enemy, false);
    }
}
