using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject loadMenu;

    private void Awake() {
        mainMenu = transform.GetChild(0).gameObject;
        loadMenu = transform.GetChild(1).gameObject;
        mainMenu.SetActive(true);
        loadMenu.SetActive(false);
    }

    private void Update() {
        if(loadMenu.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)) {
            loadMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    #region Button OnClicks
    public void ShowLoadScreen() {
        if(!loadMenu.activeInHierarchy) {
            mainMenu.SetActive(false);
            loadMenu.SetActive(true);
        }
    }

    public void Quit() {
        DataManager.Instance.SaveSpans();
        SFXManager.Instance.SavePlayerPrefs();
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void NewGame() {
        DataManager.Instance.NewGame();
    }

    public void Continue() {
        int fileIndex = PlayerPrefs.GetInt("continue");
        DataManager.Instance.LoadRequestedSave(fileIndex);
    }

    public void LoadGame(int fileIndex) {
        DataManager.Instance.LoadRequestedSave(fileIndex);
    }
    #endregion
}
