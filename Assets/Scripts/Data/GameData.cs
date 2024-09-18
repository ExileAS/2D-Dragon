using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GameData {
    public Vector3 latestCheckPoint;
    public SerializedDictionary<string, bool> enemyState;
    public SerializedDictionary<string, bool> heartCollectableState;
    public int sceneBuildIndex;

    public GameData(Vector3 initialPlayerPosition)
    {
        latestCheckPoint = initialPlayerPosition;
        enemyState = new SerializedDictionary<string, bool>();
        heartCollectableState = new SerializedDictionary<string, bool>();
        sceneBuildIndex = 1;
    }
}