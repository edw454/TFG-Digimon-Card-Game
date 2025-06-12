using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;

public class EnemySlotCard : MonoBehaviour, IPointerDownHandler
{
    public GameObject EnemyCardPrefab;
    public int numEnemySlot;

    private static GameManager gameManager;

    private GameObject EnemyCard;
    private CardData currentCardData;
    private static bool selectEnemy;
    private static bool inPlay;

    public GameObject panelPrefab; 
    private GameObject currentPanel;
    public static bool SelectEnemy 
    {  
        get { return selectEnemy; } 
        set 
        { 
            selectEnemy = value;

            foreach (var slot in FindObjectsOfType<EnemySlotCard>())
            {
                slot.HandleSelectEnemyChanged(value);
            }
        } 
    }

    private void HandleSelectEnemyChanged(bool value)
    {
        if (value && EnemyCard != null && currentPanel == null)
        {
            // Obtener el canvas raíz de la carta enemiga
            Canvas rootCanvas = EnemyCard.GetComponentInParent<Canvas>();

            if (rootCanvas == null)
            {
                rootCanvas = FindObjectOfType<Canvas>();
            }

            if (rootCanvas != null)
            {
                currentPanel = Instantiate(panelPrefab, rootCanvas.transform);

                RectTransform panelRect = currentPanel.GetComponent<RectTransform>();
                RectTransform cardRect = EnemyCard.GetComponent<RectTransform>();

                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                    rootCanvas.worldCamera,
                    cardRect.position
                );

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootCanvas.GetComponent<RectTransform>(),
                    screenPoint,
                    rootCanvas.worldCamera,
                    out Vector2 localPoint
                );

                panelRect.anchoredPosition = localPoint;
                panelRect.localScale = Vector3.one;

                currentPanel.transform.SetAsLastSibling();

                // Ajustar el orden de renderizado
                Canvas panelCanvasComponent = currentPanel.GetComponent<Canvas>();
                if (panelCanvasComponent == null)
                {
                    panelCanvasComponent = currentPanel.AddComponent<Canvas>();
                }
                panelCanvasComponent.overrideSorting = true;
                panelCanvasComponent.sortingOrder = 100; 
            }
        }
        else if (!value && currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
        }
    }

    public static bool InPlay { get { return inPlay; } set { inPlay = value; } }

    private void Start()
    {
        inPlay = true;
        selectEnemy = false;
        currentCardData = default(CardData);
    }

    void CardCreation( CardData cardData)
    {
        currentCardData = cardData;
        EnemyCard = Instantiate(EnemyCardPrefab, transform);
        EnemyCard.transform.localScale = Vector3.one;
        RectTransform cardRect = EnemyCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.localScale = Vector3.one;

        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);

        CardEnemy view = EnemyCard.GetComponent<CardEnemy>();
        if (view != null)
        {
            view.SetCardData(cardData);
            view.LoadCardImage();
        }
    }

    void Update()
    {
        if (inPlay)
        {
            if (gameManager == null)
                InitializeGameManager();
            else
            {
                CheckCardChange();
            }
        }
    }

    private void CheckCardChange()
    {
        int arrayIndex = numEnemySlot - 1;
        CardData newCardData = GetCurrentCardData(arrayIndex);

        // Si no hay carta actual pero hay una nueva en el array
        if (IsCardDataDefault(currentCardData) && !IsCardDataDefault(newCardData))
        {
            PlayEnemyCard();
        }
        // Si hay carta actual pero ahora el array está vacío
        else if (!IsCardDataDefault(currentCardData) && IsCardDataDefault(newCardData))
        {
            DeleteEnemyCard();
        }
        // Si hay carta actual y nueva carta en el array, y son diferentes
        else if (!IsCardDataDefault(currentCardData) && !IsCardDataDefault(newCardData) &&
                 !currentCardData.Equals(newCardData))
        {
            DeleteEnemyCard();
            PlayEnemyCard();
        }
    }

    private CardData GetCurrentCardData(int arrayIndex)
    {
        if (gameManager.Runner.IsServer)
        {
            return gameManager.ClientCards[arrayIndex];
        }
        else
        {
            return gameManager.HostCards[arrayIndex];
        }
    }

    public void PlayEnemyCard()
    {
        int arrayIndex = numEnemySlot - 1;
        CardData cardData = GetCurrentCardData(arrayIndex);

        if (!IsCardDataDefault(cardData))
        {
            CardCreation(cardData);
        }
    }

    public void DeleteEnemyCard()
    {
        if (EnemyCard != null)
        {
            EnemyCard.GetComponent<CardEnemy>().ShrinkAndDestroy();
            EnemyCard = null;
            currentCardData = default(CardData);
        }
    }

    private bool IsCardDataDefault(CardData cardData) => cardData.Equals(default(CardData));

    public void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectEnemy)
        {
            gameManager.RPC_AttackedCard(numEnemySlot);
            SelectEnemy = false;
            SecurityButton.SelectEnemy = false;
        }
    }
}