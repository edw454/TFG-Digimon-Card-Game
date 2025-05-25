using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI memoryCounter;
    [SerializeField] private Image panelImage;
    [SerializeField] private Color opponentColor = Color.red;
    [SerializeField] private Color localColor = Color.blue;

    private GameManager gameManager;

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (memoryCounter == null || panelImage == null)
        {
            Debug.LogError("Componentes no asignados en el Inspector!");
            Destroy(this);
        }
    }

    // Llamado desde RPC
    public void SetMemory(int value)
    {
        memoryCounter.text = value.ToString();
    }

    // Llamado desde RPC
    public void SetTurnColor(bool isHostTurn)
    {

        panelImage.color = (gameManager.Runner.IsServer && isHostTurn || !gameManager.Runner.IsServer && !isHostTurn) ? localColor : opponentColor;
    }
}
