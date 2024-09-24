using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour, IDataPersistence
{
    private DateTime startTime;
    public static DateTime additionalTimeStart;
    public static Timer Instance { get; private set; }
    private Timer(){}

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        startTime = DateTime.Now;
        additionalTimeStart = DateTime.Now;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.buildIndex == 0) Destroy(Instance.gameObject);
    }

    public void SaveState(ref GameData data) {
        TimeSpan oldSpan;
        TimeSpan.TryParse(LoadMenuManager.timeSpans[PlayerPrefs.GetInt("continue")], out oldSpan);
        TimeSpan span = oldSpan + (DateTime.Now - startTime);
        string spanText = span.ToString();
        LoadMenuManager.timeSpans[FileDataHandler.fileIndex] = spanText;
        additionalTimeStart = DateTime.Now;
    }
    
    public void LoadState(GameData data) {}

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
