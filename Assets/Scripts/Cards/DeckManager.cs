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
            }
        }
        else
        {
            for (int index = 0; index < 5; index++)
            {
                Cards nextCard = deck[0];
                deck.RemoveAt(0);

                gameManager.RPC_AddClientSecurity(index, nextCard.GetCardData());
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
        /*for (int i = 0; i < 50; i++)
        {
           deck.Add(new Cards("BT1-010", "Agumon", 1000, 3));
        }*/
        // Cartas con sus respectivas cantidades
        AgregarCarta("BT1-009", "Monodramon", 3000, 2, 4);
        AgregarCarta("BT1-013", "Muchomon", 5000, 3, 4);
        AgregarCarta("BT1-014", "Kokatorimon", 4000, 3, 4);
        AgregarCarta("BT1-019", "DarkTyrannomon", 6000, 6, 4);
        AgregarCarta("BT1-020", "Groundramon", 6000, 5, 4);
        AgregarCarta("BT1-026", "Breakdramon", 11000, 12, 4);
        AgregarCarta("BT1-024", "MetalTyrannomon", 10000, 7, 4);
    }

    void AgregarCarta(string id, string nombre, int poder, int costo, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            deck.Add(new Cards(id, nombre, poder, costo));
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
            //if (deck.Count == 0) break; 
            Cards nextCard = deck[0];
            deck.RemoveAt(0);

            // 2) Instanciar el prefab bajo 'hand'
            GameObject go = Instantiate(CardPrefab, hand);  // :contentReference[oaicite:0]{index=0}
            go.transform.localScale = Vector3.one;

            // 3) Obtener el componente CardView y pasarle los datos
            CardsActions view = go.GetComponent<CardsActions>();    // :contentReference[oaicite:1]{index=1}
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
