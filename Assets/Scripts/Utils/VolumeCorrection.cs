using UnityEngine;

public static class VolumeCorrection {
    public static float CorrectVolumeValue(float value) {
        if(value < -0.05) value = 1;
        if(value > 1.05) value = 0;
        return value;
    }

    public static string GetVolumeToDisplay(float value) {
        return Mathf.Round(value * 100).ToString();
    }
}
