using System;
using System.IO;
using System.IO.Compression;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    private static DateTime now;
    private string fileNameStart;
    private string compressExtension;
    [HideInInspector] public static string[] timeSpans = null;


    private void Awake() {
        now = DateTime.Now;
        fileNameStart = DataManager.Instance.fileNameStart;
        compressExtension = DataManager.Instance.compressExtension;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(timeSpans == null) {
            timeSpans = DataManager.Instance.LoadSpans() ?? new string[11];
        }

        int i = 1;
        foreach (RectTransform option in transform)
        {
            string stage = GetCurrentSaveStage(i);
            option.GetChild(1).GetComponent<TextMeshProUGUI>().text = stage ?? (fileNameStart + i);
            Texture2D texture = LoadImgTexture(fileNameStart + i + compressExtension);
            if(texture != null) {
                Image image = option.GetChild(0).GetComponent<Image>();
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.color = Color.white;
                image.rectTransform.sizeDelta = Vector2.one;
                image.sprite = sprite;
            }
            string timeAgo = GetTimeAgo(i);
            if(timeAgo != null) option.GetChild(2).GetComponent<TextMeshProUGUI>().text = timeAgo + GetProgressPercent(i);
            TextMeshProUGUI Text = option.GetChild(3).GetComponent<TextMeshProUGUI>();
            Text.text = (timeSpans[i] != null && timeSpans[i] != "") ? timeSpans[i].Substring(0,8).Prettify() : "";
            i++;
        }
    }

    private Texture2D LoadImgTexture(string fileName) {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        byte[] compressedBytes;
        byte[] decompressedBytes;
        Texture2D texture;
        
        if(File.Exists(fullPath)) {
            compressedBytes = File.ReadAllBytes(fullPath);
            try
            {
                using(MemoryStream memoryStream = new MemoryStream(compressedBytes)) {
                    using(GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
                        using(MemoryStream decompressedStream = new MemoryStream()) {
                            gZipStream.CopyTo(decompressedStream);
                            decompressedBytes = decompressedStream.ToArray();
                        }
                    }
                }
                texture = new Texture2D(2, 2);
                texture.LoadImage(decompressedBytes);
                return texture;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        } else return null; 
     }

    private string GetTimeAgo(int fileIndex) {
        string dateString = PlayerPrefs.GetString("TIME"+fileIndex);
        if(dateString == "") return null;

        DateTime dateTime;
        DateTime.TryParse(dateString, out dateTime);
        TimeSpan timeDifference = now - dateTime;
        int hours = (int) Mathf.Floor(timeDifference.Hours);
        int minutes = (int) Mathf.Floor(timeDifference.Minutes);
        if(hours > 0) {
            string hoursOrHour = hours > 1 ? "hours" : "hour";
            return $"More Than {hours} {hoursOrHour} Ago.";
        } else if(minutes > 0) {
            string minutesOrMinute = minutes > 1 ? "minutes" : "minute";
            return $"More Than {minutes} {minutesOrMinute} Ago.";
        } else {
            return "A Few Seconds Ago.";
        }
    }

    private string GetProgressPercent(int fileIndex) {
        int progressPercent = PlayerPrefs.GetInt("PROGRESS"+fileIndex, 0);
        if(progressPercent == 0) return "";

        return $" Progress: {progressPercent}%";
    }

    private string GetCurrentSaveStage(int fileIndex) {
        int savedSceneIndex = PlayerPrefs.GetInt("SCENE"+fileIndex, 0);
        if(savedSceneIndex == 0) return null;

        return $"Stage {savedSceneIndex}";
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }
}

