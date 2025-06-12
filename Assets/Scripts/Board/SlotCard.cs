using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fusion;
using TMPro.Examples;
using System;

public class SlotCard : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public int numSlot;
    private Canvas canvas;
    public GameObject imagePrefab;

    private CardsActions cardAction;
    private CardData playedCardData;
    private static GameObject currentButton;
    private static GameManager gameManager;
    private Coroutine destroyCoroutine;
    private Coroutine monitorCardCoroutine;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector2 buttonOffset = new Vector2(-10, -10);

    private void Awake()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        InitializeGameManager();
        playedCardData = default(CardData);
    }

    public void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {

        if (gameManager == null)
            InitializeGameManager();

        if (!IsMyTurn())
        { 
            Debug.Log("No es tu turno o no puedes jugar.");
            return;
        }

        if (eventData.pointerDrag != null)
        {
            RectTransform droppedRect = eventData.pointerDrag.GetComponent<RectTransform>();
            cardAction = eventData.pointerDrag.GetComponent<CardsActions>();

            playedCardData = cardAction.CardData.GetCardData();
            cardAction.InPlay = true;
            int cost = cardAction.CardData.Cost;

            if (gameManager.Runner.IsServer)
            {
                gameManager.ModifyMemoryRpc(cost);

                gameManager.RPC_AddHostCard(numSlot-1, playedCardData);
            }
            else
            {
                gameManager.ModifyMemoryRpc(-cost);

                gameManager.RPC_AddClientCard(numSlot-1, playedCardData);
            }

            if (canvas != null)
            {
                droppedRect.SetParent(transform, false);
                droppedRect.anchorMin = new Vector2(0.5f, 0.5f);
                droppedRect.anchorMax = new Vector2(0.5f, 0.5f);
                droppedRect.pivot = new Vector2(0.5f, 0.5f);
                droppedRect.anchoredPosition = Vector2.zero;
            }
            else
            {
                Debug.LogWarning("Canvas no asignado en el inspector.");
            }

            if (monitorCardCoroutine != null)
            {
                StopCoroutine(monitorCardCoroutine);
            }
            monitorCardCoroutine = StartCoroutine(MonitorCardExistence());
        }
    }

    private bool IsMyTurn()
    {
        return (gameManager.Runner.IsServer && gameManager.TurnHost) ||
               (!gameManager.Runner.IsServer && !gameManager.TurnHost);
    }

    private bool IsCardDataDefault(CardData cardData) => cardData.Equals(default(CardData));

    public void OnPointerDown(PointerEventData eventData)
    {
        if ( cardAction != null ) 
        {
            if (currentButton == null)
            {
                CreateButton();
            }
            else
            {
                Destroy(currentButton);
                CreateButton();
            }
        }
    }

    public void CreateButton()
    {
        if (!IsMyTurn())
        {
            return;
        }

        // Detener la corrutina anterior si existe
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }
        // Instancia el botón como hijo del panel
        currentButton = Instantiate(buttonPrefab, transform);

        // Posiciona el botón en la esquina superior derecha
        RectTransform rt = currentButton.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = buttonOffset;

        // Asigna la acción al botón
        Button buttonComponent = currentButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => OnButtonClicked());

        // Destruye el botón tras 5 segundos:
        destroyCoroutine = StartCoroutine(DestroyAfterTime(3f));
    }

    private IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentButton != null)
            Destroy(currentButton.gameObject);
    }

    private void OnButtonClicked()
    {
        gameManager.RPC_AttackingCard(numSlot);
        Destroy(currentButton);
        currentButton = null;
    }

    private IEnumerator MonitorCardExistence()
    {
        int arrayIndex = numSlot - 1;
        bool isServer = gameManager.Runner.IsServer;

        float timeout = 2f; 
        float startTime = Time.time;
        bool dataUpdated = false;

        while (!dataUpdated && (Time.time - startTime < timeout))
        {
            CardData currentData = isServer
                ? gameManager.HostCards[arrayIndex]
                : gameManager.ClientCards[arrayIndex];

            // Verificar si los datos coinciden con la carta jugada
            if (!IsCardDataDefault(currentData) && currentData.Equals(playedCardData))
            {
                dataUpdated = true;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (!dataUpdated)
        {
            Debug.LogError("No se actualizaron los datos a tiempo.");
            yield break;
        }

        // 2. Ahora monitorear eliminación
        while (true)
        {
            CardData currentData = isServer
                ? gameManager.HostCards[arrayIndex]
                : gameManager.ClientCards[arrayIndex];

            if (IsCardDataDefault(currentData))
            {
                cardAction.ShrinkAndDestroy();
                //Destroy(cardAction.gameObject);
                cardAction = null;
                playedCardData = default(CardData);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
