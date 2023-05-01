using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int maxSpawnCount = 100;
    [SerializeField] private float spawnRate = 5f;
    // [SerializeField] private Transform target;
    private float spawnCountDown = 0f;
    private int spawnCount = 0;

    // Update is called once per frame
    private void FixedUpdate()
    {
        spawnCountDown -= Time.fixedDeltaTime;

        if (spawnCountDown > 0f || spawnCount > maxSpawnCount)
        {
            return;
        }

        var obj = GameObject.Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        spawnCountDown = spawnRate;
        spawnCount++;
    }
}
