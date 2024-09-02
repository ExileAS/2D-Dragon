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
        StartCoroutine(IFrames.CreateIFrames(spriteRend, 2));
    }

    public bool CanRespawn() {
        checkPoint = checkPointManager.GetCheckPointPosition();
        return checkPoint != null;
    }

    private IEnumerator DelayRespawn() {
        var playerMovement =  GetComponent<PlayerMovement>();
        var PlayerAttack = GetComponent<PlayerAttack>();
        playerMovement.enabled = false;
        PlayerAttack.enabled = false;
        yield return new WaitForSeconds(2);
        playerMovement.enabled = true;
        PlayerAttack.enabled = true;
        anim.ResetTrigger("die");
        anim.Play("Idle");
        health.Heal(3);
        transform.position = (Vector3) checkPoint;
    }
}
