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

    // @formatter:off
    [Header("Main Menu")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject rulesMenu;

    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private Button startButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button exitAppButton;

    [Header("Game HUD")] 
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelsText;
    [SerializeField] private TMP_Text dogTreatsText;

    [SerializeField] private Button showMenuButton;
    [SerializeField] private Button exitGameButton;
    
    [Header("ExitScreen")] 
    [SerializeField] private GameObject exitScreenCanvas;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    // @formatter:on

    private void Awake()
    {
        inputActionAsset.Disable();

        Time.timeScale = 0;

        mainMenuCanvas.SetActive(true);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        rulesButton.onClick.AddListener(OnRules);
        exitAppButton.onClick.AddListener(OnExitApplication);

        showMenuButton.onClick.AddListener(ShowMenu);
        exitGameButton.onClick.AddListener(ShowExitScreen);
        
        confirmButton.onClick.AddListener(ShowMenu);
        backButton.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartGame);
        rulesButton.onClick.RemoveListener(StartGame);
        exitAppButton.onClick.RemoveListener(OnExitApplication);

        showMenuButton.onClick.RemoveListener(ShowMenu);
        exitGameButton.onClick.RemoveListener(ShowExitScreen);
        
        confirmButton.onClick.RemoveListener(ShowMenu);
        backButton.onClick.RemoveListener(StartGame);
    }

    private void Update()
    {
        dogTreatsText.text = $"x{App.GameManager.PackagesDelivered}";
    }

    private void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        exitScreenCanvas.SetActive(false);
        
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

    private void ShowMenu()
    {
        mainMenuCanvas.SetActive(true);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
        
        inputActionAsset.Disable();
        
        Time.timeScale = 0;
    }
    
    private void ShowExitScreen()
    {
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(true);
        
        inputActionAsset.Disable();
        
        Time.timeScale = 0;
    }
}