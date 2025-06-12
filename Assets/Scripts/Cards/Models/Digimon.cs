using System;
using UnityEngine;

[System.Serializable]
public class Digimon : Cards
{
    private int dp;
    private int evoCost;
    private int level;

    public int Dp {  get { return dp; } set { dp = value; } }

    public int EvoCost { get { return evoCost; } set { evoCost = value; } }

    public int Level{ get { return level; } set { level = value; } }

    public Digimon(string code, string nameCard, int cost, CardColor color, int dp, int evoCost, int level)
        : base(code, nameCard, cost, color)
    {
        this.dp = dp;
        this.evoCost = evoCost;
        this.level = level;
    }

    public override CardData GetCardData()
    {
        return new CardData
        {
            Code = Code,
            NameByte = NameCard,
            Dp = Dp,
            Cost = Cost
        };
    }
}