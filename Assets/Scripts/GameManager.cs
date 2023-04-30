using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private UIController uiController;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    public void RegisterUIController(UIController controller)
    {
        uiController = controller;
    }

    [ContextMenu("Change Scene")]
    public void ChangeScene()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
        
    }
}