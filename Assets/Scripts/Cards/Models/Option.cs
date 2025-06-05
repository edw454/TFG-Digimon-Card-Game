using System;
using UnityEngine;

[System.Serializable]
public class Option : Cards
{
    public Option(string code, string nameCard, int cost, CardColor color)
        : base(code, nameCard, cost, color)
    {
    }
}