using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour, IDataPersistence
{
    private List<Transform> checkPoints = new List<Transform>();
    private Transform activeCheckPoint;
    private void Awake() {
        foreach (Transform child in transform)
        {
            checkPoints.Add(child);
        }
    }

    public void DeactivateOldFlags(int index) {
        foreach (Transform CheckPoint in checkPoints)
        {
            if(CheckPoint.GetComponent<CheckPoint>().index < index) {
                CheckPoint.GetComponent<BoxCollider2D>().enabled = false;
                CheckPoint.GetComponent<Animator>().SetTrigger("activate");
                CheckPoint.GetComponent<CheckPoint>().isActive = true;
            }
        }
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
}
