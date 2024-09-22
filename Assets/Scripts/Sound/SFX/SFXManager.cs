using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource soundSource;
    private AudioSource musicSource;
    public float currSoundVolume { get; private set; }
    public float currMusicVolume { get; private set; }
    public static SFXManager Instance { get; private set; }

    private SFXManager() {}

    private void Awake() {
        soundSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        soundSource.volume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        currSoundVolume = soundSource.volume;
        currMusicVolume = musicSource.volume;

        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    public void PlaySound(AudioClip audio) {
        soundSource.PlayOneShot(audio);
    }

    public float AdjustVolume(string name, int amount) {
        AudioSource source = name == "MusicVolume" ? musicSource : soundSource;
        float change = amount * 0.1f;
        float newValue = source.volume + change;
        float correctedValue = VolumeCorrection.CorrectVolumeValue(newValue);
        source.volume = correctedValue;
        return correctedValue;
    }

    public void SavePlayerPrefs() {
        if(currSoundVolume != soundSource.volume) {
            PlayerPrefs.SetFloat("SoundVolume", soundSource.volume);
            currSoundVolume = soundSource.volume;
            PlayerPrefs.Save();
        }
        if(currMusicVolume != musicSource.volume) {
            PlayerPrefs.SetFloat("MusicVolume", musicSource.volume);
            currMusicVolume = musicSource.volume;
            PlayerPrefs.Save();
        }
    }
}
