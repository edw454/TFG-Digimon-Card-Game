using System;
using UnityEngine;

[System.Serializable]
public class Cards
{

    private string code;
    private string nameCard;
    private int cost;
    private CardColor color;
    private Action onPlay;

    public string Code {  get { return code; } set { code = value; } }

    public string NameCard { get { return nameCard; } set { nameCard = value; } }

    public int Cost { get { return cost; } set { cost = value; } }
    
    public CardColor Color { get { return color; } set { color = value; } }
    
    public void SetOnPlayAction(Action action)
    {
        onPlay = action;
    }

    public void Play()
    {
        onPlay?.Invoke();
    }

    public Cards(string code, string nameCard, int cost, CardColor color)
    {
        this.code = code;
        this.nameCard = nameCard;
        this.cost = cost;
        this.color = color; 
        this.onPlay = null;  // Inicializado como null
    }

    public virtual CardData GetCardData()
    {
        // Devolver el CardData con la conversión de vuelta a byte[].
        return new CardData
        {
            Code = this.code,
            NameByte = this.nameCard,
            Dp = 0,
            Cost = this.cost
        };
    }
}

