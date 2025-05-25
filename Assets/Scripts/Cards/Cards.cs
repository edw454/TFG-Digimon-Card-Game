using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;

[System.Serializable]
public class Cards
{
    public string Code;
    public string NameCard;
    public int Dp;
    public int Cost;

    public Cards(string code, string nameCard, int dp, int cost)
    {
        this.Code = code;
        this.NameCard = nameCard;
        this.Dp = dp;
        this.Cost = cost;
    }

    public CardData GetCardData()
    {
        // Devolver el CardData con la conversión de vuelta a byte[].
        return new CardData
        {
            Code = this.Code,
            NameByte = this.NameCard,
            Dp = this.Dp,
            Cost = this.Cost
        };
    }
}
