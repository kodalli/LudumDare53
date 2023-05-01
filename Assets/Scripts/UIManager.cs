using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject rulesMenu;

    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private Button startButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button exitAppButton;

    [Header("Game HUD")] 
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelsText;
    [SerializeField] private TMP_Text dogTreatsText;
    [SerializeField] private Button exitGameButton;

    private void Awake()
    {
        inputActionAsset.Disable();
        
        mainMenuCanvas.SetActive(true);
        HUDCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStart);
        rulesButton.onClick.AddListener(OnRules);
        exitAppButton.onClick.AddListener(OnExitApplication);
        
        exitGameButton.onClick.AddListener(OnExitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStart);
        rulesButton.onClick.RemoveListener(OnStart);
        exitAppButton.onClick.RemoveListener(OnExitApplication);
        
        exitGameButton.onClick.RemoveListener(OnExitGame);
    }

    private void OnStart()
    {
        mainMenuCanvas.SetActive(false);
        HUDCanvas.SetActive(true);
        inputActionAsset.Enable();
        
        Time.timeScale = 1;
    }
    private void OnRules()
    {
        mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        rulesMenu.gameObject.SetActive(!rulesMenu.gameObject.activeSelf);
    }
    
    private static void OnExitApplication()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }
    
    private void OnExitGame()
    {
        mainMenuCanvas.SetActive(true);
        HUDCanvas.SetActive(false);
        inputActionAsset.Disable();

        Time.timeScale = 0;
    }

}
