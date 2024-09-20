using System.IO;
using UnityEngine;

public class FileDataHandler{
    private string dirPath;
    private string fileNameStart;
    private string fileExtension;
    private string imgFileExtension;
    private string fullFileName;
    private string fullImgFileName;
    private int fileIndex = 1;
    private readonly string codeWord = "f8fwhfwfqhfanv09f9wvsjnvs";

    public FileDataHandler(string dirPath, string fileNameStart, string fileExtension, string imgFileExtension)
    {
        this.dirPath = dirPath;
        this.fileNameStart = fileNameStart;
        this.fileExtension = fileExtension;
        this.imgFileExtension = imgFileExtension;
        fullFileName = fileNameStart + fileIndex + fileExtension;
        fullImgFileName = fileNameStart + fileIndex + imgFileExtension;
    }

    public GameData LoadFromFile(string fileName, bool decrypt) {
        GameData gameData = null;
        string fullPath = Path.Combine(dirPath, fileName);

        if(File.Exists(fullPath)) {
            try
            {
                string loadedData;
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        loadedData = reader.ReadToEnd();
                    }
                }
                if(decrypt) {
                    loadedData = EncryptDecrypt(loadedData);
                }
                // Debug.Log("Loaded data: " + loadedData);
                gameData = JsonUtility.FromJson<GameData>(loadedData);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        if(gameData != null) {
            fileIndex = PlayerPrefs.GetInt("continue", 0);
            IncrementFileIndex();
        }

        return gameData;
    }

    public void SaveToFile(GameData data, bool encrypt) {
        string fullPath = Path.Combine(dirPath, fullFileName);
        string imgFullPath = Path.Combine(dirPath, fullImgFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToSave = JsonUtility.ToJson(data, true);
            if(encrypt) {
                dataToSave = EncryptDecrypt(dataToSave);
            }
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToSave);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        PlayerPrefs.SetInt("continue", fileIndex);
        ScreenCapture.CaptureScreenshot(imgFullPath);
        IncrementFileIndex();
    }

    private void IncrementFileIndex() {
        fileIndex = (fileIndex + 1) > 10 ? 1 : fileIndex + 1;
        fullFileName = fileNameStart + fileIndex + fileExtension;
        fullImgFileName = fileNameStart + fileIndex + imgFileExtension;
    }

    private string EncryptDecrypt(string originalData) {
        string modifiedData = "";
        for(int i = 0; i < originalData.Length; i++) {
            modifiedData += (char) (originalData[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }
}