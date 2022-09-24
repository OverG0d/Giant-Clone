using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    public bool gameStart;
    public int numLittleGuys;
    public int numGiants;
    public GameObject lose;
    public GameObject win;

    private void Awake()
    {
        current = this;
    }

    public event Action<int, int> onHealthDecrease;
    public void HealthDecrease(int health, int id)
    {
        if(onHealthDecrease != null)
        {
            onHealthDecrease(health, id);
        }
    }

    public void Win()
    {
        if(numGiants <= 0)
        {
            win.SetActive(true);
            gameStart = false;
        }
    }

    public void Lose()
    {
        if (numLittleGuys <= 0)
        {
            lose.SetActive(true);
            gameStart = false;
        }
    }
}
