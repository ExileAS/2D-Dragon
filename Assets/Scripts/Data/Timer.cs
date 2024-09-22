using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour, IDataPersistence
{
    private DateTime startTime;
    private TimeSpan timeSpan;
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
        timeSpan = new();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.buildIndex == 0) Destroy(Instance.gameObject);
    }

    public void SaveState(ref GameData data) {
        TimeSpan span = timeSpan + (DateTime.Now - startTime);
        string spanText = span.ToString();
        PlayerPrefs.SetString("TimeSpan"+FileDataHandler.fileIndex, spanText);
        data.timeSpan = spanText;
    }
    
    public void LoadState(GameData data) {
        TimeSpan.TryParse(data.timeSpan, out timeSpan);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
