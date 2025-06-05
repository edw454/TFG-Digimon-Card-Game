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

        if (!selectEnemy)
        {
            if (CheckLastSecurityCard(gameManager.Runner.IsServer))
            {
                gameManager.RPC_NotifyEndGame(gameManager.Runner.IsServer);
                return;
            }
            gameManager.RPC_SecurityBattle(gameManager.Runner.IsServer);
            EnemySlotCard.SelectEnemy = false;
            selectEnemy = false;
        }
    }

    private bool CheckLastSecurityCard(bool isHost)
    {
        if (isHost)
        {
            CardData lastCard = gameManager.HostSecurity[gameManager.HostSecurity.Length - 1];
            return lastCard.Equals(default(CardData));
        }
        else
        {
            CardData lastCard = gameManager.ClientSecurity[gameManager.ClientSecurity.Length - 1];
            return lastCard.Equals(default(CardData));
        }
    }

    public void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
}