using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;

public class SecurityButton : MonoBehaviour, IPointerDownHandler
{
    public GameObject SecurityCardPrefab;
    private GameObject SecurityCard;

    private int index = 0;
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

    void CardCreation(CardData cardData)
    {
        SecurityCard = Instantiate(SecurityCardPrefab, transform);
        SecurityCard.transform.localScale = Vector3.one;
        RectTransform cardRect = SecurityCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.localScale = Vector3.one;

        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);

        CardSecurity view = SecurityCard.GetComponent<CardSecurity>();
        if (view != null)
        {
            view.SetCardData(cardData);
            view.LoadCardImage();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameManager == null)
            InitializeGameManager();

        if (selectEnemy)
        {
            if (CheckLastSecurityCard(gameManager.Runner.IsServer))
            {
                Debug.Log("Ya no hay security");
                gameManager.RPC_NotifyEndGame(gameManager.Runner.IsServer);
                return;
            }
            Debug.Log("Ataque a security iniciado");
            CardCreation(gameManager.Runner.IsServer ? gameManager.HostSecurity[index]: gameManager.ClientSecurity[index]);
            gameManager.RPC_SecurityBattle(gameManager.Runner.IsServer);
            EnemySlotCard.SelectEnemy = false;
            selectEnemy = false;
            index++;
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