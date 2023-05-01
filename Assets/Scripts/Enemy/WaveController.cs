using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // play sports-whistle.wav every wave start
    // wave 0: nothing? for 10 seconds
    /* wave 1: right side one spawner every 30s they spawn up to 2 enemies
     * wave 2: right side two spawners every 30s they spawn up to 2 enemies
     * wave 3: left side two spawners every 30s they spawn up to 3 enemies
     * wave 4: left and right side, one left, 2 right spawners every 30s they spawn up to 2 enemies
     * wave 5: left right and bottom sides, one on every side up to 2 enemies per
     * survival wave n: (n-5)*10 if rand(0,101) left, right and side
    */
    // Start is called before the first frame update
    private bool spawnRight = false;
    private bool spawnLeft = false;
    private bool spawnDown = false;
    [SerializeField] private List<Spawner> rightSideSpawners = new List<Spawner>();
    [SerializeField] private List<Spawner> leftSideSpawners = new List<Spawner>();
    [SerializeField] private List<Spawner> bottomSideSpawners = new List<Spawner>();

    private List<Spawner> allSpawners = new List<Spawner>();

    private List<int> wavePackageCountGoals = new List<int>() { 0, 1, 2, 4, 5, 6 };

    void Start()
    {
        allSpawners.AddRange(rightSideSpawners);
        allSpawners.AddRange(leftSideSpawners);
        allSpawners.AddRange(bottomSideSpawners);
    }

    // Update is called once per frame
    void Update()
    {
        if(App.GameManager.PackagesDelivered == wavePackageCountGoals[App.GameManager.CurrentWave])
        {
            //turn off spawners to turn on right ones
            disableAllSpawners();
            App.GameManager.nextWave();
            // play sports-whistle.wav every wave start
        }
        if(App.GameManager.CurrentWave == 1)
        {
            rightSideSpawners[1].isEnabled = true;
        }

        if(App.GameManager.CurrentWave == 2 || App.GameManager.CurrentWave == 5)
        {
            foreach(var spawner in rightSideSpawners){
                spawner.isEnabled = true;
            }
        }

        if (App.GameManager.CurrentWave == 3 || App.GameManager.CurrentWave == 4)
        {
            foreach (var spawner in leftSideSpawners)
            {
                spawner.isEnabled = true;
                
            }
        }

        

        if ( App.GameManager.CurrentWave == 5)
        {
            foreach (var spawner in bottomSideSpawners)
            {
                spawner.isEnabled = true;
            }
        }

        if (App.GameManager.CurrentWave > 5)
        {
            foreach (var spawner in allSpawners)
            {
                //n*10 + 50 > 50 - wave 
                if (Random.Range((App.GameManager.CurrentWave * 10 + 50), 101) > 50 - App.GameManager.CurrentWave)
                {
                    spawner.isEnabled = true;
                    spawner.maxRandSpawnCount = App.GameManager.CurrentWave - 3;


                    /*f(x) = A * (1 - e^(-B * x))

                        where:

                        f(x) is the output value
                        x is the input value
                        A is the upper limit the function will approach but never reach = 4
                        B is a positive constant that controls the rate of reduction = .5f
                        e is the base of the natural logarithm (approximately 2.71828)
                     */
                    spawner.spawnRate = 30f - (25f * (1 - Mathf.Pow(2.718f, (-.5f * App.GameManager.CurrentWave))));
                }
            }
        }
    }
    public void disableAllSpawners()
    {
        foreach(var spawner in allSpawners)
        {
            spawner.isEnabled = false;
        }
    }
}
