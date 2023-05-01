using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PackagesDelivered;
    public int Health;
    public int CurrentWave;
    private void Awake()
    {
        PackagesDelivered = 0;
        Health = 3;
        CurrentWave = 0;
    }

    private void Update()
    {
        
        if(Health < 0)
        {
            Debug.Log("GameOver!!!!!!!");
        }
    }
    public void nextWave()
    {
        CurrentWave++;
        Debug.Log("Going to next Wave: " + CurrentWave + "!!!!");
    }
}