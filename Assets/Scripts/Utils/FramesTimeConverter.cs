using UnityEngine;

public static class FrameTimeConverter {
    public static float FramesToTime(int frames) {
        return frames * (1 / Application.targetFrameRate);
    }

    public static int TimeToFrames(float time) {
        return (int) Mathf.Round(time * Application.targetFrameRate);
    }
}