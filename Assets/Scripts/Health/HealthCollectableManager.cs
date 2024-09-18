
using UnityEngine;

public class HealthCollectableManager : MonoBehaviour, IDataPersistence
{
    public void SaveState(ref GameData data) {
        foreach (Transform heartCollectable in transform)
        {
            HealthCollectable heart = heartCollectable.GetComponent<HealthCollectable>();
            if(data.heartCollectableState.ContainsKey(heart.id)) {
                data.heartCollectableState.Remove(heart.id);
            }
            data.heartCollectableState.Add(heart.id, heart.collected);
        }
    }

    public void LoadState(GameData data) {
        foreach (Transform heartCollectable in transform)
        {
            HealthCollectable heart = heartCollectable.GetComponent<HealthCollectable>();
            bool collected;
            data.heartCollectableState.TryGetValue(heart.id, out collected);
            heart.collected = collected;
            if(collected) heartCollectable.gameObject.SetActive(false);
        }
    }
}
