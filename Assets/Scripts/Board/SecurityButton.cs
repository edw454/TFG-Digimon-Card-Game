using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;

public class SecurityButton : MonoBehaviour, IPointerDownHandler
{
    private static bool selectEnemy;
    private static GameManager gameManager;

    public static bool SelectEnemy{ 
        get { return selectEnemy; } 
        set { selectEnemy = value; } 
    }

    void Start()
    {
        selectEnemy = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameManager == null)
            InitializeGameManager();
        if (selectEnemy)
        {
            gameManager.RPC_SecurityBattle(gameManager.Runner.IsServer);
            EnemySlotCard.SelectEnemy = false;
            selectEnemy = false;
        }
    }

    public void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
}