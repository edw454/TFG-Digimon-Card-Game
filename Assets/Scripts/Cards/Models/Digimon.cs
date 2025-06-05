using System;
using UnityEngine;

[System.Serializable]
public class Digimon : Cards
{
    private int dp;
    private int level;

    public int Dp {  get { return dp; } set { dp = value; } }
    public int Level{ get { return level; } set { level = value; } }

    public Digimon(string code, string nameCard, int cost, CardColor color, int dp, int level)
        : base(code, nameCard, cost, color)
    {
        this.dp = dp;
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