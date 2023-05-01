using System;
using UnityEngine;

public class App
{
    public static GameManager GameManager { get; private set; }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        var target = UnityEngine.Object.Instantiate(Resources.Load("App")) as GameObject;
        if (target == null)
            throw new ApplicationException();
        target.name = "App";

        GameManager = target.GetComponent<GameManager>();

        UnityEngine.Object.DontDestroyOnLoad(target);
    }
}