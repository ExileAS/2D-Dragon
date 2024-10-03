using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] clouds;
    [SerializeField] private float spawnRate;
    [SerializeField] private int maxToSpawn;
    [SerializeField] private int heigherOffset;
    [SerializeField] private int lowerOffset;
    [SerializeField] private int xOffsetBounds;
    [HideInInspector] public static Cloud[] inactiveClouds = new Cloud[10];
    [HideInInspector] public static int currIndex;
    private int numToSpawn;
    private List<Cloud> cloudsToReactivate = new();

    private void Start() {
        SpawnCloud(0, 3, 50);
        SpawnCloud(2, -3, 40);
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner() {
        while(true) {
            yield return new WaitForSeconds(spawnRate);
            numToSpawn = Random.Range(1, maxToSpawn+1);
            currIndex = 0;
            int heightOffset, xOffset, cloudIndex;
            if(inactiveClouds.Any(c => c != null))
            {
                int j = 0;
                foreach (Cloud cloud in inactiveClouds)
                {   
                    if(cloud != null) {
                        cloudsToReactivate.Add(cloud);
                        inactiveClouds[j] = null;
                    }
                    j++;
                }
                for(int i = 0; i < numToSpawn; i++)
                {
                    heightOffset = Random.Range(-lowerOffset, heigherOffset);
                    xOffset = Random.Range(-xOffsetBounds, xOffsetBounds);
                    cloudIndex = Random.Range(0, cloudsToReactivate.Count);
                    ReactivateCloud(cloudIndex, heightOffset, xOffset);
                }
                cloudsToReactivate.Clear();
            } else {
                for(int i = 0; i < numToSpawn; i++)
                {
                    heightOffset = Random.Range(-lowerOffset, heigherOffset);
                    xOffset = Random.Range(-xOffsetBounds, xOffsetBounds);
                    cloudIndex = Random.Range(0, clouds.Length);
                    SpawnCloud(cloudIndex, heightOffset, xOffset);
                }
            }
        }
    }

    private void SpawnCloud(int index, int heightOffset, int xOffset) {
        Instantiate(clouds[index], new Vector3(transform.position.x + xOffset, 
        transform.position.y + heightOffset, 
        transform.position.z), Quaternion.identity);
    }

    private void ReactivateCloud(int index, int heightOffset, int xOffset) {
        Cloud cloud = cloudsToReactivate[index];
        cloud.ResetAlpha();
        cloud.transform.position = new Vector3(transform.position.x + xOffset, 
        transform.position.y + heightOffset, transform.position.z);
    }

    // unity stops it by default when object is destroyed
    // private void OnDestroy() {
    //     StopCoroutine(nameof(Spawner));
    // }
}
