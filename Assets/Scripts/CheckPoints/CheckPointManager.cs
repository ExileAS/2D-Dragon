using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour, IDataPersistence
{
    private List<Transform> checkPoints = new List<Transform>();
    private Transform activeCheckPoint;
    private static int checkPointCount;
    private static int activeCheckPointCount;
    private void Awake() {
        checkPointCount = 0;
        int i = 1;
        foreach (Transform child in transform)
        {
            checkPoints.Add(child);
            child.GetComponent<CheckPoint>().index = i;
            checkPointCount++;
            i++;
        }
    }

    public void DeactivateOldFlags(int index) {
        int pointsActivated = 0;
        foreach (Transform CheckPoint in checkPoints)
        {
            CheckPoint point = CheckPoint.GetComponent<CheckPoint>();
            if(point.index < index && !point.isActive) {
                CheckPoint.GetComponent<BoxCollider2D>().enabled = false;
                CheckPoint.GetComponent<Animator>().SetTrigger("activate");
                point.isActive = true;
            }
            if(point.isActive) pointsActivated++;
        }
        activeCheckPointCount = pointsActivated;
    }

    public void SetActivePoint(Transform point) {
        activeCheckPoint = point;
    }

    public Vector2? GetCheckPointPosition() {
        return activeCheckPoint == null ? null : activeCheckPoint.position;
    }

    public void SaveState(ref GameData data) {
        if(activeCheckPoint == null) return;
        data.latestCheckPoint = new Vector2(activeCheckPoint.position.x, activeCheckPoint.position.y + 0.8f);
    }

    public void LoadState(GameData data) {}

    public static int GetActivePercent() {
        return (int) Mathf.Floor(activeCheckPointCount * 100 / checkPointCount);
    }
}
