
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    private DateTime now;

    private void Awake() {
        now = DateTime.Now;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
        if(scene.buildIndex != 0) return;

        int i = 1;
        foreach (RectTransform option in transform)
        {
            option.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Save" + i;
            Texture2D texture = LoadImgTexture("Save" + i + ".png");
            if(texture != null) {
                Image image = option.GetChild(0).GetComponent<Image>();
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
                image.color = Color.white;
            }
            string timeAgo = GetTimeAgo(i);
            if(timeAgo != null) option.GetChild(2).GetComponent<TextMeshProUGUI>().text = timeAgo;
            i++;
        }
    }

    private Texture2D LoadImgTexture(string fileName) {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        Texture2D texture = new Texture2D(100, 100);

        if(File.Exists(fullPath)) {
            try
            {
                byte[] bytes = File.ReadAllBytes(fullPath);
                texture.LoadImage(bytes);
                return texture;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        } else {
            // Debug.Log("file doesn't exist");
            return null;
        }
    }

    private string GetTimeAgo(int fileIndex) {
        string dateString = PlayerPrefs.GetString(""+fileIndex);
        if(dateString == "") return null;

        DateTime dateTime;
        DateTime.TryParse(dateString, out dateTime);
        TimeSpan timeDifference = now - dateTime;
        int hours = (int) Mathf.Floor(timeDifference.Hours);
        int minutes = (int) Mathf.Floor(timeDifference.Minutes);
        if(hours > 0) {
            return $"More Than {hours} hours Ago.";
        } else if(minutes > 0) {
            return $"More Than {minutes} minutes Ago.";
        } else {
            return "A Few Seconds Ago.";
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
