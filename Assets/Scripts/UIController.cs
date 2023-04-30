using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject rulesMenu;
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        inputActionAsset.Disable();
        
        startButton.onClick.AddListener(OnStart);
        rulesButton.onClick.AddListener(OnRules);
        exitButton.onClick.AddListener(OnExit);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStart);
        rulesButton.onClick.RemoveListener(OnStart);
        exitButton.onClick.RemoveListener(OnExit);
    }

    private void OnStart()
    {
        gameObject.SetActive(false);
        inputActionAsset.Enable();
    }
    private void OnRules()
    {
        mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        rulesMenu.gameObject.SetActive(!rulesMenu.gameObject.activeSelf);
    }
    
    private static void OnExit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }

}
