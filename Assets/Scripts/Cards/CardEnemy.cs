using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEnemy : MonoBehaviour
{
    private Cards cardData;
    private Image imageComponent;
    private Coroutine shrinkCoroutine;

    public Cards CardData
    {
        get { return cardData; }
        set { cardData = value; }
    }

    void Awake()
    {
        transform.rotation = Quaternion.Euler(0, 0, 180f);
    }

    public void LoadCardImage()
    {
        Sprite spriteCargado = Resources.Load<Sprite>("Sprites/Digimon/" + CardData.Code);
        imageComponent = GetComponent<Image>();
        imageComponent.sprite = spriteCargado;
    }

    public void SetCardData(CardData data)
    {
        Cards newCardData = new Cards(
        data.Code.ToString(),  // Simplificado
        data.NameByte.ToString(),
        data.Cost,
        CardColor.red
        );
        cardData = newCardData;
    }

    public void ShrinkAndDestroy(float duration = 0.5f)
    {
        if (shrinkCoroutine != null)
        {
            StopCoroutine(shrinkCoroutine);
        }
        shrinkCoroutine = StartCoroutine(ShrinkCoroutine(duration));
    }

    private IEnumerator ShrinkCoroutine(float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progress);
            yield return null;
        }

        Destroy(gameObject); // Destruye después de la animación
    }
}