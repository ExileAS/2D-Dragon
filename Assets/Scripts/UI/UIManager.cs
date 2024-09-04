using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseMenu;

    [Header("SFX")]
    [SerializeField] private AudioClip gameOverAudio;
    [SerializeField] private AudioClip pauseAudio;

    // [Header("Text")]
    // [SerializeField] private TextMeshPro soundVolumeText;
    // [SerializeField] private TextMeshPro musicVolumeText;

    private GameObject soundManager;
    private AudioSource soundSource;
    private AudioSource musicSource;

    private void Awake() {
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);
        soundManager = FindObjectOfType<SFXManager>().gameObject;
        soundSource = soundManager.GetComponent<AudioSource>();
        musicSource = soundManager.transform.GetChild(0).GetComponent<AudioSource>();
        Debug.Log(musicSource.name);
        Debug.Log(soundSource.name);
        soundSource.volume = 0.5f;
        musicSource.volume = 0.4f;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    public void GameOverScreen() {
        gameOverScreen.SetActive(true);
        SFXManager.Instance.PlaySound(gameOverAudio);
    }

    private void Pause() {
        if(Time.timeScale == 1) 
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;
        TogglePauseMenu();
    }

    private void TogglePauseMenu() {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        SFXManager.Instance.PlaySound(pauseAudio);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }

    public void Quit() {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public int AdjustVolume(string name, int amount) {
        AudioSource source = name == "MusicVolume" ? musicSource : soundSource;
        float change = amount * 0.1f;
        float newValue = source.volume + change;
        source.volume = newValue;
        if(newValue < -0.05) source.volume = 1;
        if(newValue > 1.05) source.volume = 0;
        return (int) Mathf.Round(source.volume * 100);
    }
}
