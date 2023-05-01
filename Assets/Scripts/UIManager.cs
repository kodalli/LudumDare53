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

    [SerializeField] private Button buffDogButton;
    [SerializeField] private Button littleDogButton;

    [Header("ExitScreen")]
    [SerializeField] private GameObject exitScreenCanvas;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;
    private bool hasStarted = false;
    public AudioClip clickSound;
    public AudioClip BuffBuySound;
    public AudioClip smallDogBuySound;

    public Button returnButtonLoss;
    public Button returnButtonWin;
    public GameObject gameOverCanvas;
    public GameObject wonGameCanvas;

    // @formatter:on

    private void Awake()
    {
        inputActionAsset.Disable();

        Time.timeScale = 0;

        mainMenuCanvas.SetActive(true);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        wonGameCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);

        startButton.onClick.AddListener(PlayClickSound);
        rulesButton.onClick.AddListener(PlayClickSound);
        exitAppButton.onClick.AddListener(PlayClickSound);
        showMenuButton.onClick.AddListener(PlayClickSound);
        exitGameButton.onClick.AddListener(PlayClickSound);
        confirmButton.onClick.AddListener(PlayClickSound);
        backButton.onClick.AddListener(PlayClickSound);
        resumeButton.onClick.AddListener(PlayClickSound);
        littleDogButton.onClick.AddListener(PuppyDogSound);
        buffDogButton.onClick.AddListener(BuffDogSound);
        returnButtonLoss.onClick.AddListener(RestartGame);
        returnButtonWin.onClick.AddListener(RestartGame);
    }

    private void PlayClickSound()
    {
        SoundManager.Instance.PlaySound(clickSound);
    }
    private void BuffDogSound()
    {
        SoundManager.Instance.PlaySound(BuffBuySound);
    }
    private void PuppyDogSound()
    {
        SoundManager.Instance.PlaySound(smallDogBuySound);
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

        buffDogButton.onClick.AddListener(PurchaseBuffDog);
        littleDogButton.onClick.AddListener(PurchaseLittleDog);
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

        buffDogButton.onClick.RemoveListener(PurchaseBuffDog);
        littleDogButton.onClick.RemoveListener(PurchaseLittleDog);
    }

    private void PurchaseBuffDog()
    {
        App.GameManager.OnPurchaseBuffDogEvent();
    }
    private void PurchaseLittleDog()
    {
        App.GameManager.OnPurchaseLittleDogEvent();
    }

    private void Update()
    {
        var gm = App.GameManager;
        dogTreatsText.text = $"x{gm.DogTreats}";
        packagesLeftText.text = $"Packages Left: {gm.PackagesLeft}";
        wavePackageDeliveryGoalText.text = $"Delivery Goal: {gm.WavePackageGoal}";
        waveText.text = $"Wave: {gm.CurrentWave}";
        deliveredText.text = $"Delivered: {gm.PackagesDelivered}";
        if (gm.PackagesLeft <= 0 && gm.WavePackageGoal > gm.PackagesDelivered)
        {
            GameOver();
        }
        if (gm.CurrentWave > 5)
        {
            WinGame();
        }
    }

    private void GameOver()
    {
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        inputActionAsset.Disable();
        Time.timeScale = 1;
    }

    private void WinGame()
    {
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(false);
        exitScreenCanvas.SetActive(false);
        wonGameCanvas.SetActive(true);
        inputActionAsset.Disable();
        Time.timeScale = 1;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
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