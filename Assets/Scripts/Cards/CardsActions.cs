using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fusion;

public class CardsActions : MonoBehaviour, IBeginDragHandler,
    IEndDragHandler, IDragHandler
{
    #region private properties
    private Cards cardData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool inPlay;
    private bool canPlay;
    #endregion

    #region public properties
    public static Canvas canvas;
    public static Transform hand;
    public static bool CanPlay;

    public bool InPlay
    {   get { return inPlay; } set { inPlay = value; } }

    public Cards CardData
    {   get { return cardData; } }
    #endregion

    public void SetData(Cards cardData)
    {
        this.cardData = cardData;
        Sprite spriteCargado = Resources.Load<Sprite>("Sprites/Digimon/" + CardData.Code);
        Image imageComponent = GetComponent<Image>();
        imageComponent.sprite = spriteCargado;
    }

    private void Awake()
    {
        
        InPlay = false;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        //Find hand
        if (hand == null)
        {
            GameObject handObject = GameObject.Find("Hand");
            if (handObject != null)
            {
                hand = handObject.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró el panel 'Hand' en la escena.");
            }
        }
    }

    #region Move cards
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        if (!InPlay)
        {
            transform.SetParent(canvas.transform, true);
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!InPlay)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!InPlay)
        {
            transform.SetParent(canvas.transform, false);
            transform.SetParent(hand);
        }
    }
    #endregion
}
