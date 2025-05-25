using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class AssignEnemy : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI memoryCounter;

    void Update()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            Debug.Log("No se encontr� un NetworkRunner en la escena.");
            return;
        }

        // Determina el rol del jugador
        if (runner.IsServer)
        {
            Debug.Log("Soy el Host - A�adiendo HostTurnBar");
           // HostTurnBar hostComponent = gameObject.AddComponent<HostTurnBar>();
            //hostComponent.Initialize(memoryCounter);
        }
        else
        {
            Debug.Log("Soy el Cliente - A�adiendo ClientTurnBar");
            //ClientTurnBar clientComponent = gameObject.AddComponent<ClientTurnBar>();
            //clientComponent.Initialize(memoryCounter);
        }
    }
}
