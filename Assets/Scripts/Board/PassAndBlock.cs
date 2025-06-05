using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassAndBlock : MonoBehaviour
{
    [SerializeField] private Color passButtonColor;
    [SerializeField] private Color blockerButtonColor;
    [SerializeField] private Color nullButtonColor;
    [SerializeField] private Button button;

    private GameManager gameManager;

    void Update()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        //button.onClick.AddListener(() => OnButtonClicked());
    }

    private void OnPassButton()
    {
        if (gameManager != null )
        {
            gameManager.PassMemoryRpc();
        }
        else
        {
            gameManager = FindObjectOfType<GameManager>();
            gameManager.PassMemoryRpc();
        }
    }

    private void OnBlockerButton()
    {
        //En analisis
    }

    private void OnNullButton()
    {
        //El jugador no debe hacer nada 
    }

    public void SetButtonAction(bool isHostTurn)
    {
        bool isLocalTurn = (gameManager.Runner.IsServer && isHostTurn)
                         || (!gameManager.Runner.IsServer && !isHostTurn);

        // Aplicar color según acción
        Color targetColor = isLocalTurn ? passButtonColor : nullButtonColor;
        Image img = button.GetComponent<Image>();
        img.color = targetColor;

        // Configurar listener
        button.onClick.RemoveAllListeners();
        if (isLocalTurn)
        {
            button.onClick.AddListener(OnPassButton);
        }
        else
        {
            button.onClick.AddListener(OnNullButton);
        }

        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = isLocalTurn ? "Pasar Turno" : "Bloqueado";
        }

    }
}
