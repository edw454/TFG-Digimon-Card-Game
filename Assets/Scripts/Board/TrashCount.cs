using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashCount : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject counterPrefab;
    private GameManager gameManager;
    private ChangeCount currentCounter;

    private Coroutine destroyCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        InitializeGameManager();
        CreateCounter();
        UpdateCounter();
        destroyCoroutine = StartCoroutine(DestroyAfterTime(3f));
    }

    private void CreateCounter()
    {
        if (counterPrefab != null && currentCounter == null)
        {
            // Obtener el Canvas raíz (padre más alto de la UI)
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                rootCanvas = FindObjectOfType<Canvas>(); // Fallback si no se encuentra
            }

            if (rootCanvas != null)
            {
                // Instanciar el contador como hijo del Canvas raíz
                GameObject counterInstance = Instantiate(counterPrefab, rootCanvas.transform);
                currentCounter = counterInstance.GetComponent<ChangeCount>();

                // Configurar posición y escala
                RectTransform counterRect = counterInstance.GetComponent<RectTransform>();
                RectTransform thisRect = GetComponent<RectTransform>();

                // Convertir la posición del botón de basura (Trash) a coordenadas del Canvas
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                    rootCanvas.worldCamera,
                    thisRect.position
                );

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootCanvas.GetComponent<RectTransform>(),
                    screenPoint,
                    rootCanvas.worldCamera,
                    out Vector2 localPoint
                );

                counterRect.anchoredPosition = localPoint;
                counterRect.localScale = Vector3.one;

                // Asegurar que el contador se renderice encima de otros elementos
                Canvas counterCanvas = counterInstance.GetComponent<Canvas>();
                if (counterCanvas == null)
                {
                    counterCanvas = counterInstance.AddComponent<Canvas>();
                }
                counterCanvas.overrideSorting = true;
                counterCanvas.sortingOrder = 100; // Orden alto para que aparezca encima
            }
        }
    }

    private void UpdateCounter()
    {
        if (gameManager == null)
            InitializeGameManager();

        if (currentCounter != null)
        {
            int count = CountNonDefaultCards();
            currentCounter.GetCountText(count.ToString());
        }
    }

    private int CountNonDefaultCards()
    {
        int count = 0;
        bool isHost = gameManager.Runner.IsServer;

        if (isHost)
        {
            foreach (CardData card in gameManager.HostTrash)
            {
                if (!card.Equals(default(CardData)))
                    count++;
            }
        }
        else
        {
            foreach (CardData card in gameManager.ClientTrash)
            {
                if (!card.Equals(default(CardData)))
                    count++;
            }
        }

        return count;
    }

    private void InitializeGameManager()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentCounter != null)
            Destroy(currentCounter.gameObject);
    }
}
