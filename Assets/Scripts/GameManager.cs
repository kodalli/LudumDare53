using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PackagesDelivered { get; private set; }
    public int PackagesLeft { get; private set; }
    public int WavePackageGoal { get; private set; }
    public int CurrentWave { get; private set; }
    public int DogTreats { get; private set; }

    private void Awake()
    {
        PackagesDelivered = 0;
        PackagesLeft = 3;
        CurrentWave = 0;
        DogTreats = 0;
    }

    public void NextWave(int packagesToDeliverForWave)
    {
        PackagesDelivered = 0;
        CurrentWave++;
        PackagesLeft = packagesToDeliverForWave * 2;
        WavePackageGoal = packagesToDeliverForWave;
        Debug.Log("Going to next Wave: " + CurrentWave + "!!!!");
    }

    public void DeliverPackage()
    {
        PackagesDelivered++;
        PackagesLeft--;
        AddDogTreat();
        // TODO: check if all delivered go to next wave
        if (PackagesDelivered == WavePackageGoal)
        {
            Debug.Log("Wave Complete, all packages delivered");
        }
    }

    // If 0 end game
    public void PackageStolen()
    {
        if (PackagesLeft <= 0)
        {
            Debug.Log("GameOver!!!!!!!");
            // TODO: End game
        }
        else
        {
            PackagesLeft--;
        }
    }

    // Everytime a package is delivered or kill pirate
    public void AddDogTreat()
    {
        DogTreats++;
    }

    // Use this for buying troops, Tell player if they don't have enough money
    public bool ConsumeDogTreats(int treatsCount)
    {
        if (DogTreats - treatsCount < 0)
        {
            Debug.Log("Not enough treats to purchase");
            return false;
        }
        DogTreats -= treatsCount;
        return true;
    }
}