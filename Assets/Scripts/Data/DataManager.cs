using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private GameData gameData;
    private FileDataHandler dataHandler;
    private Vector3 playerPosition;
    private readonly string fileNameStart = "save";
    private readonly string fileExtension = ".game";
    private readonly bool useEncryption = false;
    
    private DataManager(){}

    private void Awake() {
        if(SceneManager.GetActiveScene().buildIndex != 0)
            playerPosition = FindObjectOfType<PlayerMovement>().transform.position;
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if(Instance != this) {
            Destroy(gameObject);
        }
        gameData = new GameData(playerPosition);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileNameStart, fileExtension);
    }

    public void SaveData() {
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.SaveState(ref gameData);
        }
        gameData.sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        dataHandler.SaveToFile(gameData, useEncryption);
    }

    public void LoadData(int fileIndex) {
        string fileName = fileNameStart + fileIndex + fileExtension;
        gameData = dataHandler.LoadFromFile(fileName, useEncryption);
        if(gameData == null) { 
            Debug.LogError("no data found!");
            return;
        }
        // Debug.Log(gameData.latestCheckPoint);
        SceneManager.LoadScene(gameData.sceneBuildIndex);
        foreach (IDataPersistence obj in GetPersistenceObjects())
        {
            obj.LoadState(gameData);
            Debug.Log(obj);
        }
    }

    private List<IDataPersistence> GetPersistenceObjects() {
        IEnumerable<IDataPersistence> persistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(persistenceObjects);
    }
}
