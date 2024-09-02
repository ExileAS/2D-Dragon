using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
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
            }
        }
    }

    public void SetActivePoint(Transform point) {
        activeCheckPoint = point;
    }

    public Vector3? GetCheckPointPosition() {
        return activeCheckPoint == null ? null : activeCheckPoint.position;
    }
}
