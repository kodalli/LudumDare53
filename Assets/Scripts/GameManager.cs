using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int dogTreatsCount;

    private void Awake()
    {
        dogTreatsCount = 0;
    }
}