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
    public readonly string fileNameStart = "save";
    public readonly string fileExtension = ".game";
    public readonly string imgFileExtension = ".png";
    public readonly string compressExtension = ".gz";
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
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileNameStart, fileExtension);
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
        string imgPath = Path.Combine(Application.persistentDataPath, $"{fileNameStart}{FileDataHandler.fileIndex}{compressExtension}");
        StartCoroutine(CompressAndSaveImage(imgPath));
        dataHandler.SaveToFile(gameData, useEncryption);
    }

    private IEnumerator CompressAndSaveImage(string imgPath) {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ZoomAndCapture();
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

    private Texture2D ZoomAndCapture() {
        float fov = Camera.main.fieldOfView;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 10;
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        Camera.main.fieldOfView = fov;
        Camera.main.orthographic = true;
        return texture;
    }

    public void LoadRequestedSave(int _fileIndex) {
        fileIndex = _fileIndex;
        string fileName = fileNameStart + fileIndex + fileExtension;
        GameData data = dataHandler.LoadFromFile(fileName, useEncryption);
        if(data == null) return;
        
        gameData = data;
        gameData.isNewGame = false;
        PlayerPrefs.SetInt("continue", _fileIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameData.sceneBuildIndex);
    }

    public void SaveSpans() {
        string fullPath = Path.Combine(Application.persistentDataPath, "spans.game");

        try
        {
            string spans = JsonUtility.ToJson(new SerializableSpans{spanArray = LoadMenuManager.timeSpans});
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(spans);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public string[] LoadSpans() {
        string fullPath = Path.Combine(Application.persistentDataPath, "spans.game");
        string[] loadedSpans = null;

        if(File.Exists(fullPath)) {
            try
            {
                string loadedData;
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        loadedData = reader.ReadToEnd();
                    }
                }
                SerializableSpans spans = JsonUtility.FromJson<SerializableSpans>(loadedData);
                loadedSpans = spans.spanArray;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        return loadedSpans;
    }

    private List<IDataPersistence> GetPersistenceObjects() {
        IEnumerable<IDataPersistence> persistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(persistenceObjects);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
