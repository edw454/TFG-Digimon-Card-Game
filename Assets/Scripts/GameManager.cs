using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon.StructWrapping;
using TMPro.Examples;
using System;

public class GameManager : NetworkBehaviour
{
    #region Turn properties
    [Networked, OnChangedRender(nameof(OnTurnHostChanged))] public bool TurnHost { get; set; }
    [Networked, OnChangedRender(nameof(OnMemoryChanged))] public int Memory { get; set; }
    #endregion

    #region Fields cards
    [Networked, Capacity(5)]
    public NetworkArray<CardData> HostCards { get; } = MakeInitializer(new CardData[5]);

    [Networked, Capacity(5)]
    public NetworkArray<CardData> ClientCards { get; } = MakeInitializer(new CardData[5]);
    #endregion

    #region Trash cards
    [Networked, Capacity(40)]
    public NetworkArray<CardData> HostTrash { get; } = MakeInitializer(new CardData[5]);

    [Networked, Capacity(40)]
    public NetworkArray<CardData> ClientTrash { get; } = MakeInitializer(new CardData[5]);
    #endregion

    #region Cards in Battle
    [Networked, OnChangedRender(nameof(OnAttackingCardIndexChanged))] 
    public int AttackingCardIndex { get; set; }

    [Networked, OnChangedRender(nameof(OnAttackedCardIndexChanged))] 
    public int AttackedCardIndex { get; set; }
    #endregion

    #region Security cards
    [Networked, Capacity(5)]
    public NetworkArray<CardData> HostSecurity { get; } = MakeInitializer(new CardData[5]);

    [Networked, Capacity(5)]
    public NetworkArray<CardData> ClientSecurity { get; } = MakeInitializer(new CardData[5]);
    #endregion

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            // Asigna aleatoriamente al iniciar (solo el host)
            TurnHost = UnityEngine.Random.Range(0, 2) == 0; // 50% true/false
            Memory = 0;
            AttackedCardIndex = 0;
            AttackingCardIndex = 0;
        }
    }

    #region OnChanged
    public void OnMemoryChanged()
    {
        if (Memory < 0)
        {
            TurnHost = true;
        } else
        {
            if (Memory > 0)
            {
                TurnHost = false;
            }
        }
        RPC_UpdateMemoryCounter(Memory);
    }

    public void OnTurnHostChanged()
    {
        RPC_UpdateTurnColor(TurnHost);
        RPC_UpdatePassButton(TurnHost);
        if (this.Runner.IsServer) 
        {
            RPC_DrawCard(TurnHost);
        }
    }

    public void OnAttackedCardIndexChanged()
    {
        if (AttackedCardIndex > 0)
        {
            RPC_ThrowBattle();
        }
    }

    public void OnAttackingCardIndexChanged()
    {
        if(AttackingCardIndex > 0)
        {
            RPC_ActiveOpponentsSelect();
        }
    }
    #endregion

    #region Turn and memory
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ModifyMemoryRpc(int amount)
    {
        Memory += amount; // Solo el Host puede modificar [Networked]
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void PassMemoryRpc()
    {
        if (TurnHost)
        {
            Memory = 3;
        }
        else
        {
            Memory = -3;
        }
    }
    #endregion

    #region Cards in Field
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddHostCard(int slotIndex, CardData newCard)
    {
        if (slotIndex >= 0 && slotIndex < HostCards.Length)
            HostCards.Set(slotIndex, newCard);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddClientCard(int slotIndex, CardData newCard)
    {
        if (slotIndex >= 0 && slotIndex < ClientCards.Length)
            ClientCards.Set(slotIndex, newCard);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_ResetSlot(bool isHostSlot, int slotIndex)
    {
        if (isHostSlot && slotIndex < HostCards.Length)
        {
            RPC_TrashCard(isHostSlot, HostCards[slotIndex]);
            HostCards.Set(slotIndex, default);
        }
        else if (!isHostSlot && slotIndex < ClientCards.Length)
        {
            RPC_TrashCard(isHostSlot, ClientCards[slotIndex]);
            ClientCards.Set(slotIndex, default);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void RPC_TrashCard(bool isHostSlot, CardData deletedCard)
    {
        if (isHostSlot)
        {
            for (int slotIndex = 0; slotIndex < HostTrash.Length; slotIndex++)
            {
                if (HostTrash[slotIndex].Equals(default(CardData)))
                {
                    HostTrash.Set(slotIndex, deletedCard);
                    return;
                }
            }
        }
        else if (!isHostSlot)
        {
            for (int slotIndex = 0; slotIndex < HostTrash.Length; slotIndex++)
            {
                if (ClientTrash[slotIndex].Equals(default(CardData)))
                {
                    ClientTrash.Set(slotIndex, deletedCard);
                    return;
                }
            }
        }
    }
    #endregion

    #region Events When property changes
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateMemoryCounter(int value)
    {
        // Busca todos los TurnBar y actualiza su texto
        foreach (var turnBar in FindObjectsOfType<TurnBar>())
        {
            turnBar.SetMemory(value);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateTurnColor(bool isHostTurn)
    {
        foreach (var turnBar in FindObjectsOfType<TurnBar>())
        {
            turnBar.SetTurnColor(isHostTurn);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdatePassButton(bool isHostTurn)
    {
        foreach (var passbutton in FindObjectsOfType<PassAndBlock>())
        {
            passbutton.SetButtonAction(isHostTurn);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DrawCard(bool isHostTurn)
    {
        foreach (var deckDraw in FindObjectsOfType<DeckManager>())
        {
            deckDraw.TurnStarted(isHostTurn, this);
        }
    }
    #endregion

    #region  Battle in Security
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddHostSecurity(int slotIndex, CardData newCard)
    {
        if (slotIndex >= 0 && slotIndex < HostCards.Length)
            HostSecurity.Set(slotIndex, newCard);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddClientSecurity(int slotIndex, CardData newCard)
    {
        if (slotIndex >= 0 && slotIndex < ClientCards.Length)
            ClientSecurity.Set(slotIndex, newCard);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SecurityBattle(bool  isHostAttack)
    { 
        if (isHostAttack)
        {
            for (int i = 0; i < HostSecurity.Length; i++)
            {

                Debug.Log("Host security attack");
                if (!HostSecurity[i].Equals(default(CardData)))
                {
                    if (HostSecurity[i].Dp >= HostCards[AttackingCardIndex-1].Dp)
                    {
                        Debug.Log("Host gana contra security");
                        RPC_ResetSlot(true, (AttackingCardIndex - 1));
                    }
                    HostSecurity.Set(i, default(CardData));
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < ClientSecurity.Length; i++)
            {
                Debug.Log("Cliente security attack");
                if (!ClientSecurity[i].Equals(default(CardData)))
                {
                    if (ClientSecurity[i].Dp >= ClientCards[AttackingCardIndex - 1].Dp)
                    {
                        Debug.Log("Client gana contra security");
                        RPC_ResetSlot(false, (AttackingCardIndex - 1));
                    }
                    ClientSecurity.Set(i, default(CardData));
                    return;
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_NotifyEndGame(bool IsHostWinner)
    {
        EnemySlotCard.InPlay = false;
        BasicSpawner.Instance.ReturnToLobby(IsHostWinner);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_EndGame(bool IsHostWinner)
    {
        BasicSpawner.Instance.ReturnToLobby2(IsHostWinner);
    }

    #endregion

    #region  Battle in Field
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AttackingCard(int index)
    {
        AttackingCardIndex = index; // Solo el Host puede modificar [Networked]
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AttackedCard(int index)
    {
        AttackedCardIndex = index; // Solo el Host puede modificar [Networked]
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ActiveOpponentsSelect()
    {
        if(!this.Runner.IsServer && !TurnHost || this.Runner.IsServer && TurnHost)
        {
            SecurityButton.SelectEnemy = true;
            EnemySlotCard.SelectEnemy = true;
            //foreach (var enemySlot in FindObjectsOfType<EnemySlotCard>())
            //{
            //    enemySlot.SelectEnemy = true;
            //}
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void RPC_ThrowBattle()
    {
        int attackingCardIndex = AttackingCardIndex - 1;
        int attackedCardIndex = AttackedCardIndex - 1;
        if (TurnHost)
        {
            if (HostCards[attackingCardIndex].Dp > ClientCards[attackedCardIndex].Dp)
            {
                RPC_ResetSlot(false, attackedCardIndex);
            }
            else if (HostCards[attackingCardIndex].Dp < ClientCards[attackedCardIndex].Dp)
            {
                RPC_ResetSlot(true, attackingCardIndex);
            }
            else
            {
                RPC_ResetSlot(false, attackedCardIndex);
                RPC_ResetSlot(true, attackingCardIndex);
            }
            
        }
        else
        {
            if (ClientCards[attackingCardIndex].Dp > HostCards[attackedCardIndex].Dp)
            {
                RPC_ResetSlot(true, attackedCardIndex);
            }
            else if (ClientCards[attackingCardIndex].Dp < HostCards[attackedCardIndex].Dp)
            {
              RPC_ResetSlot(false, attackingCardIndex);
            }
            else
            {
               RPC_ResetSlot(true, attackedCardIndex);
               RPC_ResetSlot(false, attackingCardIndex);
            }
            
        }
        AttackedCardIndex = 0;
        AttackingCardIndex = 0;
    }
    #endregion
}

