using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;
    private Animator anim;
    private CheckPointManager manager;
    [SerializeField] private AudioClip activateAudio;
    [HideInInspector] public bool isActive;

    private void Awake() {
        anim = GetComponent<Animator>();
        manager = GetComponentInParent<CheckPointManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        anim.SetTrigger("activate");
        SFXManager.Instance.PlaySound(activateAudio);
        GetComponent<BoxCollider2D>().enabled = false;
        manager.SetActivePoint(transform);
        manager.DeactivateOldFlags(index);
    }
}
