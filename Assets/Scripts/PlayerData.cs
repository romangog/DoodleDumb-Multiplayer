using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnDataChanged))] public float MaxHeight { get; set; }

    [Networked(OnChanged =nameof(OnDataChanged))] public NetworkBool Logged { get; set; }

    [Networked(OnChanged =nameof(OnDataChanged))] public NetworkString<_16> Nickname { get; set; }

    [Networked] public float CheckpointHeight { get; set; }

    private static GameplayScreen _gameplayScreen;

    public override void Spawned()
    {
        base.Spawned();

        _gameplayScreen = FindObjectOfType<GameplayScreen>();

        _gameplayScreen.SetLeaderboardData(Object.InputAuthority, this);
    }

    public static void OnDataChanged(Changed<PlayerData> playerInfo)
    {
        _gameplayScreen.SetLeaderboardData(playerInfo.Behaviour.Object.InputAuthority, playerInfo.Behaviour);
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        _gameplayScreen.DeleteLeadeboardData(Object.InputAuthority);
        Debug.Log("Despawned");
    }
    public static implicit operator PlayerLeaderboardData(PlayerData data)
    {
        return new PlayerLeaderboardData() { Name = data.Nickname.Value, Distance = Mathf.FloorToInt(data.MaxHeight), Logged = data.Logged };
    }
}
