using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0;
    private float fps = 0;
    private readonly float updateInterval = 1;
    private float timeSinceLastUpdate = 0;

    private void Awake() {
        Application.targetFrameRate = 300;
        Time.timeScale = 1;
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.01f;
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = 1 / deltaTime;
            timeSinceLastUpdate = 0;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new();
        Rect rect = new(0, 0, w, h);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = Color.white;
        string text = string.Format("{0:0} FPS", fps);
        GUI.Label(rect, text, style);
    }
}
