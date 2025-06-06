using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

using Fusion;

public class DeckManager : MonoBehaviour
{
    private List<Cards> deck = new List<Cards>();
    private System.Random rng = new System.Random();
    private GameManager gameManager;

    public Transform hand;
    public GameObject CardPrefab;

    void Awake()
    {
        InicializarDeck();
        BarajarDeck();
        InitialHand();
    }

    void Update()
    {
        if (gameManager == null) 
        { 
            InitializeGameManager();
        }
    } 
    public void  InitializeSecurity()
    {
        if (gameManager.Runner.IsServer)
        {
            for (int index = 0; index < 5; index++)
            {
                Cards nextCard = deck[0];
                deck.RemoveAt(0);
                gameManager.RPC_AddHostSecurity(index, nextCard.GetCardData());
                Debug.Log("añadida "+nextCard.NameCard);
            }
        }
        else
        {
            for (int index = 0; index < 5; index++)
            {
                Cards nextCard = deck[0];
                deck.RemoveAt(0);

                gameManager.RPC_AddClientSecurity(index, nextCard.GetCardData());
                Debug.Log("añadida " + nextCard.NameCard);
            }
        }
    }

    public void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager != null)
        {
            InitializeSecurity();   
        }
    }
    void InicializarDeck()
    {
        AddDigimon("BT1-009", "Monodramon", 2, CardColor.red, 3000, 3, 4);
        AddDigimon("BT1-013", "Muchomon", 3, CardColor.red, 5000, 3, 4);
        AddDigimon("BT1-014", "Kokatorimon", 3, CardColor.red, 4000, 4, 4);
        AddDigimon("BT1-019", "DarkTyrannomon", 6, CardColor.red, 6000, 4, 4);
        AddDigimon("BT1-020", "Groundramon", 5, CardColor.red, 6000, 5, 4);
        AddDigimon("BT1-026", "Breakdramon", 12, CardColor.red, 11000, 5, 4);
        AddDigimon("BT1-024", "MetalTyrannomon", 7, CardColor.red, 10000, 6, 4);
    }

    void AddDigimon(string id, string nombre, int costo, CardColor color,int poder, int level, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            deck.Add(new Digimon(id, nombre, costo, color, poder, level));
        }
    }

    void BarajarDeck()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }
    }

    void InitialHand()
    {
        for(int i = 0;i < 5;i++)
        {
            Cards nextCard = deck[0];
            deck.RemoveAt(0);

            GameObject go = Instantiate(CardPrefab, hand); 
            go.transform.localScale = Vector3.one;

            CardsActions view = go.GetComponent<CardsActions>();   
            if (view != null)
                view.SetData(nextCard);
        }
    }

    public void TurnStarted(bool isHostTurn, GameManager gameManager)
    {
        if (this.gameManager == null) 
        { 
            this.gameManager = gameManager;
        }
       if (gameManager.Runner.IsServer && isHostTurn || 
            !gameManager.Runner.IsServer && !isHostTurn)
       {
            Cards nextCard = deck[0];
            deck.RemoveAt(0);

            GameObject go = Instantiate(CardPrefab, hand);
            go.transform.localScale = Vector3.one;

            CardsActions view = go.GetComponent<CardsActions>();
            if (view != null)
                view.SetData(nextCard);
        }
    }
}
