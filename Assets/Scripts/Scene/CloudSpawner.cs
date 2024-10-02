using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private float timer = Mathf.Infinity;

    private void Start() {
        SpawnCloud(0, 3, 50);
        SpawnCloud(2, -3, 40);
    }

    void Update()
    {
        if(timer < spawnRate) 
        {
            timer += Time.deltaTime;
        } else {
            numToSpawn = Random.Range(1, maxToSpawn+1);
            timer = 0;
            currIndex = 0;
            if(inactiveClouds.Any(c => c != null)) 
            {
                List<Cloud> clouds = new();
                int j = 0;
                foreach (Cloud cloud in inactiveClouds)
                {   
                    if(cloud != null) {
                        clouds.Add(cloud);
                        inactiveClouds[j] = null;
                    }
                    j++;
                } 
                for(int i = 0; i < numToSpawn; i++)
                {
                    int heightOffset = Random.Range(-lowerOffset, heigherOffset);
                    int xOffset = Random.Range(-xOffsetBounds, xOffsetBounds);
                    int cloudIndex = Random.Range(0, clouds.Count);
                    ReactivateCloud(cloudIndex, heightOffset, xOffset, clouds);
                }
            } else {
                for(int i = 0; i < numToSpawn; i++)
                {
                    int heightOffset = Random.Range(-lowerOffset, heigherOffset);
                    int xOffset = Random.Range(-xOffsetBounds, xOffsetBounds);
                    int cloudIndex = Random.Range(0, clouds.Length);
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

    private void ReactivateCloud(int index, int heightOffset, int xOffset, List<Cloud> clouds) {
        Cloud cloud = clouds[index];
        cloud.transform.position = new Vector3(transform.position.x + xOffset, 
        transform.position.y + heightOffset, transform.position.z);
    }
}
