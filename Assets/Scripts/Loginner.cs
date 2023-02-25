using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Zenject;
using System;

public class Loginner : NetworkBehaviour
{
    public static Action<PlayerRef> LoginnedEvent;

    [Rpc]
    public static void RPC_Login(NetworkRunner runner, string nickame, PlayerRef player)
    {
        Debug.Log("RPC_LOGIN called: is server = " + runner.IsSharedModeMasterClient);
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        spawner.SetActivePlayer(player, nickame);

        LoginnedEvent?.Invoke(player);
    }
}
