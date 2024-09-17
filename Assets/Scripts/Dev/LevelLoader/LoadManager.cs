using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private int sceneCount;

    private static LoadManager Instance;

    private void Awake() {
        sceneCount = SceneManager.sceneCountInBuildSettings;
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("Loaded Scene " + scene.buildIndex);
    }

    private void Update() {
        int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currSceneIndex + 1) == sceneCount ? 0 : currSceneIndex + 1;
        int prevSceneIndex = (currSceneIndex - 1) < 0 ? sceneCount - 1 : currSceneIndex - 1;
        if(Input.GetKeyDown(KeyCode.G)) {
            SceneManager.LoadScene(nextSceneIndex);
        }
        if(Input.GetKeyDown(KeyCode.F)) {
            SceneManager.LoadScene(prevSceneIndex);
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
