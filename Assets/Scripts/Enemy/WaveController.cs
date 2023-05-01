using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // play sports-whistle.wav every wave start
    // wave 0: nothing? for 10 seconds
    /* wave 1: right side one spawners every 30s they spawn up to 2 enemies
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
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
