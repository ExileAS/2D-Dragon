using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.IO.Compression;
using System;
using System.Collections;
using System.Xml.Serialization;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private GameData gameData;
    private FileDataHandler dataHandler;
    public readonly string fileNameStart = "save";
    public readonly string fileExtension = ".game";
    public readonly string imgFileExtension = ".png";
    public readonly string compressExtension = ".gz";
    private readonly bool useEncryption = true;
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
        // StartCoroutine(CompressAndSaveImage(imgPath));
        dataHandler.SaveToFile(gameData, useEncryption);
    }

    private IEnumerator CompressAndSaveImage(string imgPath) {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();
        Texture2D texture = Capture();
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

    private Texture2D Capture() {
        RenderTexture renderTexture = new RenderTexture(Screen.width/2, Screen.height/2, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenShot = new Texture2D(Screen.width/2, Screen.height/2, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width/2, Screen.height/2), 0, 0);
        screenShot.Apply();

        FindObjectOfType<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        return screenShot;
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
            SerializableSpans serializableSpans = new SerializableSpans{spanArray = LoadMenuManager.timeSpans};
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableSpans));
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                serializer.Serialize(stream, serializableSpans);
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
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableSpans));
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    SerializableSpans spans = serializer.Deserialize(stream) as SerializableSpans;
                    loadedSpans = spans.spanArray;
                }
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
