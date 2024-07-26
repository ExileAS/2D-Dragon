using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{   
    [Header("Damage & IFrames")]
    [SerializeField] private float damage;
    [SerializeField] private float IFrameTime;

    [Header("activation")]
    [SerializeField] private float triggerDelay;
    [SerializeField] private float activationPeriod;
    private SpriteRenderer spriteRend;
    private Animator anim;
    private bool isTriggered;
    private bool isAvtive;

    private void Awake() {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(!isTriggered) {
                StartCoroutine(ActivateTrap());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")) {
             if(isAvtive) {
                other.GetComponent<Health>().TakeDamage(damage, IFrameTime);
            }
        }
    }

    private IEnumerator ActivateTrap() {
        isTriggered = true;
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(triggerDelay);
        spriteRend.color = Color.white;
        isAvtive = true;
        anim.SetBool("isActive", true);
        yield return new WaitForSeconds(activationPeriod);
        isTriggered = false;
        isAvtive = false;
        anim.SetBool("isActive", false);
    }
}
