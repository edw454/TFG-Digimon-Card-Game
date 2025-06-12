using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashOpponentCount : MonoBehaviour, IPointerDownHandler
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
            GameObject counterInstance = Instantiate(counterPrefab, transform);
            currentCounter = counterInstance.GetComponent<ChangeCount>();
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
            foreach (CardData card in gameManager.ClientTrash)
            {
                if (!card.Equals(default(CardData)))
                    count++;
            }
        }
        else
        {
            foreach (CardData card in gameManager.HostTrash)
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
