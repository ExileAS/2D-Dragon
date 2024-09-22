using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.IO.Compression;
using System;
using System.Collections;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private GameData gameData;
    private FileDataHandler dataHandler;
    private readonly string fileNameStart = "save";
    private readonly string fileExtension = ".game";
    private readonly string imgFileExtension = ".png";
    private readonly bool useEncryption = false;
    private int fileIndex;
    [HideInInspector] public bool isLoaded;
    
    private DataManager(){}

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        isLoaded = false;
        gameData = new GameData();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileNameStart, fileExtension, imgFileExtension);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void NewGame() {
        gameData = new GameData();
        SceneManager.LoadScene(1);
    }

    public void Restart(int sceneIndex) {
        gameData = new GameData();
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadScene) {
        if(gameData.isNewGame || scene.buildIndex == 0) return;
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.LoadState(gameData);
        }
        isLoaded = true;
    }

    public void SaveData() {
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.SaveState(ref gameData);
        }
        gameData.sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(CompressAndSaveImage(Path.Combine(Application.persistentDataPath, $"Save{FileDataHandler.fileIndex}.dat")));
        dataHandler.SaveToFile(gameData, useEncryption);
    }

    private IEnumerator CompressAndSaveImage(string imgPath) {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = texture.EncodeToPNG();
        byte[] compressedBytes;
        try
        {
            using(MemoryStream memoryStream = new MemoryStream()) {
                using(GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
                    gZipStream.Write(bytes, 0, bytes.Length);
                }
                compressedBytes = memoryStream.ToArray();
            }
            File.WriteAllBytes(imgPath, compressedBytes);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        yield return null;
    }

    public void LoadRequestedSave(int _fileIndex) {
        fileIndex = _fileIndex;
        string fileName = fileNameStart + fileIndex + fileExtension;
        gameData = dataHandler.LoadFromFile(fileName, useEncryption);
        if(gameData == null) { 
            gameData = new GameData();
            return;
        }
        gameData.isNewGame = false;
        PlayerPrefs.SetInt("continue", _fileIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameData.sceneBuildIndex);
    }

    private List<IDataPersistence> GetPersistenceObjects() {
        IEnumerable<IDataPersistence> persistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(persistenceObjects);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
