using System.Collections;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private CheckPointManager checkPointManager;
    private Vector3? checkPoint;
    private Animator anim;
    private Health health;
    private SpriteRenderer spriteRend;

    private void Awake() {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void RespawnPlayer() {
        StartCoroutine(DelayRespawn());
        StartCoroutine(IFrames.CreateIFrames(spriteRend, 4));
    }

    public bool CanRespawn() {
        checkPoint = checkPointManager.GetCheckPointPosition();
        return checkPoint != null;
    }

    private IEnumerator DelayRespawn() {
        //Syncronous
        var playerMovement =  GetComponent<PlayerMovement>();
        var PlayerAttack = GetComponent<PlayerAttack>();
        playerMovement.dead = true;
        PlayerAttack.enabled = false;
        //Asyncronous (non-blocking) makes the caller method non blocking too.
        yield return new WaitForSeconds(2);
        anim.ResetTrigger("die");
        transform.position = (Vector3) checkPoint;
        anim.Play("Respawn");
        playerMovement.dead = false;
        PlayerAttack.enabled = true;
        health.Heal(3);
    }
}
