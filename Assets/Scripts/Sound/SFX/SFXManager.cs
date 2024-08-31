using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource source;
    public static SFXManager Instance {get; private set; }


    private void Awake() {
        source = GetComponent<AudioSource>();

        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        
    }

    public void PlaySound(AudioClip audio) {
        source.PlayOneShot(audio);
    }
}
