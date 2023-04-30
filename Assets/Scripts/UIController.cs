using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.RegisterUIController(this);
    }
}
