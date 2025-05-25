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

    public static bool SelectEnemy {  get { return selectEnemy; } set { selectEnemy = value; } }

    private void Start()
    {
        selectEnemy = false;
        currentCardData = default(CardData);
    }

    void CardCreation( CardData cardData)
    {
        currentCardData = cardData;
        //Cards enemyCard = new Cards("BT1-016", "Agumon", 1000, 3);
        EnemyCard = Instantiate(EnemyCardPrefab, transform);
        EnemyCard.transform.localScale = Vector3.one;
        RectTransform cardRect = EnemyCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.localScale = Vector3.one;

        // Configurar anclaje al centro (si no está ya configurado en el prefab)
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);

        // 3) Obtener el componente CardView y pasarle los datos
        CardEnemy view = EnemyCard.GetComponent<CardEnemy>();
        if (view != null)
        {
            view.SetCardData(cardData);
            view.LoadCardImage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
            InitializeGameManager();
        else 
        {
            if (IsCardDataDefault(currentCardData))
            {
                PlayEnemyCard();
            }
            else
            {
                DeleteEnemyCard();
            }
        }   
    }

    public void PlayEnemyCard()
    {
        int arrayIndex = numEnemySlot - 1;
        if (gameManager.Runner.IsServer)
        {
            if (!IsCardDataDefault(gameManager.ClientCards[arrayIndex]))
            {
                CardCreation(gameManager.ClientCards[arrayIndex]);
            }
        }
        else
        {
            if (!IsCardDataDefault(gameManager.HostCards[arrayIndex]))
            {
                CardCreation(gameManager.HostCards[arrayIndex]);
            }
        }
    }

    public void DeleteEnemyCard()
    {
        int arrayIndex = numEnemySlot - 1;
        if (gameManager.Runner.IsServer)
        {
            if (IsCardDataDefault(gameManager.ClientCards[arrayIndex]))
            {
                Destroy(EnemyCard);
                EnemyCard = null;
                currentCardData = default(CardData);
            }
        }
        else
        {
            if (IsCardDataDefault(gameManager.HostCards[arrayIndex]))
            {
                Destroy(EnemyCard);
                EnemyCard = null;
                currentCardData = default(CardData);
            }
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
            selectEnemy = false;
        }
    }
}