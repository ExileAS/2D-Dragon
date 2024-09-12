using System.Collections;
using System.Collections.Generic;
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
        if(checkPointManager != null) checkPoint = checkPointManager.GetCheckPointPosition();
        return checkPoint != null;
    }

    private IEnumerator DelayRespawn() {
        //Syncronous
        var playerMovement =  GetComponent<PlayerMovement>();
        var PlayerAttack = GetComponent<PlayerAttack>();
        playerMovement.enabled = false;
        PlayerAttack.enabled = false;
        RespawnEnemies();
        //Asyncronous (non-blocking) makes the caller method non blocking too.
        yield return new WaitForSeconds(2);
        anim.ResetTrigger("die");
        transform.position = (Vector3) checkPoint;
        anim.Play("Respawn");
        playerMovement.enabled = true;
        PlayerAttack.enabled = true;
        health.Heal(3);
    }

    private List<EnemyHealth> GetEnemies() {
        IEnumerable<EnemyHealth> enemies = FindObjectsOfType<EnemyHealth>();
        return new List<EnemyHealth>(enemies);
    }

    private void RespawnEnemies() {
        foreach (EnemyHealth enemy in GetEnemies())
        {
            enemy.Respawn();
        }
    }
}
