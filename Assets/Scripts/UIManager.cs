using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Button resumeButton;

    [Header("Game HUD")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private TMP_Text packagesLeftText;
    [SerializeField] private TMP_Text wavePackageDeliveryGoalText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text dogTreatsText;
    [SerializeField] private TMP_Text deliveredText;

    [SerializeField] private Button showMenuButton;
    [SerializeField] private Button exitGameButton;

    [Header("ExitScreen")]
    [SerializeField] private GameObject exitScreenCanvas;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;
    private bool hasStarted = false;

    // @formatter:on

    private void Awake()
    {
        inputActionAsset.Disable();

        Time.timeScale = 0;

        mainMenuCanvas.SetActive(true);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
        resumeButton.gameObject.SetActive(false);
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
        resumeButton.onClick.AddListener(ResumeGame);
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
        resumeButton.onClick.RemoveListener(ResumeGame);
    }

    private void Update()
    {
        var gm = App.GameManager;
        dogTreatsText.text = $"x{gm.DogTreats}";
        packagesLeftText.text = $"Packages Left: {gm.PackagesLeft}";
        wavePackageDeliveryGoalText.text = $"Delivery Goal: {gm.WavePackageGoal}";
        waveText.text = $"Wave: {gm.CurrentWave}";
        deliveredText.text = $"Delivered: {gm.PackagesDelivered}";
    }
    private void ResumeGame()
    {
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        exitScreenCanvas.SetActive(false);
        inputActionAsset.Enable();
        Time.timeScale = 1;
    }
    private void StartGame()
    {
        if (hasStarted)
        {
            // restart game
            SceneManager.LoadScene(0);
            return;
        }
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        exitScreenCanvas.SetActive(false);

        inputActionAsset.Enable();

        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
        Time.timeScale = 1;
        hasStarted = true;
        resumeButton.gameObject.SetActive(true);
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