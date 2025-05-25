using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEnemy : MonoBehaviour
{
    private Cards cardData;

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
        Image imageComponent = GetComponent<Image>();
        imageComponent.sprite = spriteCargado;
    }

    public void SetCardData(CardData data)
    {
        Cards newCardData = new Cards(
        data.Code.ToString(),  // Simplificado
        data.NameByte.ToString(),
        data.Dp,
        data.Cost
    );
        cardData = newCardData;
    }
}
