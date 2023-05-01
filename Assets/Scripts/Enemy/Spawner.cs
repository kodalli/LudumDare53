using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int maxSpawnCount = 100;
    [SerializeField] public float spawnRate = 5f;
    [SerializeField] private Transform target;
    [SerializeField] private Transform escape;
    private float spawnCountDown = 0f;
    private int spawnCount = 0;
    private int numberToSpawn = 1;
    [SerializeField] public bool isEnabled = false;
    public int maxRandSpawnCount = 2;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }
        spawnCountDown -= Time.fixedDeltaTime;

        if (spawnCountDown > 0f || spawnCount > maxSpawnCount)
        {
            return;
        }
        Debug.Log("Units spawned!");
        numberToSpawn = Random.Range(1, maxRandSpawnCount);
        for(int i = 0; i < numberToSpawn; i++) { 
            var obj = GameObject.Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            var pp = obj.GetComponent<PorchPirateController>();
            pp.chevalPackageBaseLocation = target;
            pp.escapeLocation = escape;
        }
        spawnCountDown = spawnRate;
        spawnCount++;
    }
    
}
