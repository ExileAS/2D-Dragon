using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = 1.0f / deltaTime;
            timeSinceLastUpdate = 0.0f;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 4 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = Color.white;
        string text = string.Format("{0:0} FPS", fps);
        GUI.Label(rect, text, style);
    }
}
