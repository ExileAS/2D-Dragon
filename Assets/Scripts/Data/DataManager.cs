using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private GameData gameData;
    private FileDataHandler dataHandler;
    private readonly string fileNameStart = "save";
    private readonly string fileExtension = ".game";
    private readonly bool useEncryption = false;
    private int fileIndex;
    
    private DataManager(){}

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        gameData = new GameData();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileNameStart, fileExtension);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void NewGame() {
        gameData = new GameData();
        SceneManager.LoadScene(1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadScene) {
        if(gameData.isNewGame) return;
        if(scene.buildIndex == 0) {
            HandleMainMenuScene();
            return;
        }
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.LoadState(gameData);
        }
    }

    public void SaveData() {
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.SaveState(ref gameData);
        }
        gameData.sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        dataHandler.SaveToFile(gameData, useEncryption);
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
        SceneManager.LoadScene(gameData.sceneBuildIndex);
    }

    private List<IDataPersistence> GetPersistenceObjects() {
        IEnumerable<IDataPersistence> persistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(persistenceObjects);
    }

    private void HandleMainMenuScene() {

    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
