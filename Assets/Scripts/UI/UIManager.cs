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

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI soundText;
    [SerializeField] private TextMeshProUGUI musicText;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;

    private void Awake() {
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);
        Application.targetFrameRate = 300;
        Time.timeScale = 1;
        playerAttack.enabled = true;
        soundText.text = VolumeCorrection.GetVolumeToDisplay(VolumeCorrection.CorrectVolumeValue(SFXManager.Instance.currSoundVolume));
        musicText.text = VolumeCorrection.GetVolumeToDisplay(VolumeCorrection.CorrectVolumeValue(SFXManager.Instance.currMusicVolume));
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    public void GameOverScreen() {
        gameOverScreen.SetActive(true);
        SFXManager.Instance.PlaySound(gameOverAudio);
    }

    private void Pause() {
        if(!pauseMenu.activeInHierarchy) {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            playerMovement.enabled = false;
            playerAttack.enabled = false;
        }
        else {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            playerMovement.enabled = true;
            playerAttack.enabled = true;
            SFXManager.Instance.SavePlayerPrefs();
        }
        SFXManager.Instance.PlaySound(pauseAudio);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SFXManager.Instance.SavePlayerPrefs();
    }

    public void MainMenu() {
        SFXManager.Instance.SavePlayerPrefs();
        SceneManager.LoadScene(0);
    }

    public void Quit() {
        SFXManager.Instance.SavePlayerPrefs();
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
