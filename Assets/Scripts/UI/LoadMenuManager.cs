
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    private void Start() {
        int i = 1;
        foreach (RectTransform option in transform)
        {
            option.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Save" + i;
            Texture2D texture = LoadImgTexture("Save" + i + ".png");
            if(texture != null) {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
                option.GetChild(0).GetComponent<Image>().sprite = sprite;
            }
            i++;
        }
    }

    private Texture2D LoadImgTexture(string fileName) {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        Texture2D texture = new Texture2D(678, 100);

        if(File.Exists(fullPath)) {
            try
            {
                byte[] bytes = File.ReadAllBytes(fullPath);
                texture.LoadImage(bytes);
                return texture;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return null;
            }
        } else {
            Debug.Log("file doesn't exist");
            return null;
        }
    }
}
