using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCD;
    private float CDTimer;
    [SerializeField] private GameObject[] Arrows;
    [SerializeField] private Transform firePoint;

    [Header("SFX")]
    [SerializeField] private AudioClip arrowAudio;
  

    private void Update() {
        if(CDTimer > attackCD) {
            CDTimer = 0;
            Attack();
        } else CDTimer += Time.deltaTime;
    }

    private void Attack() {
        int nextInd = ObjectPool.NextInPool(Arrows);
        if(nextInd == -1) return;

        GameObject nextArrow = Arrows[nextInd];

        nextArrow.transform.position = firePoint.position;
        nextArrow.GetComponent<Arrow>().ActivateArrow();
        SFXManager.Instance.PlaySound(arrowAudio);
    }
}
