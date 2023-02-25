using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;
using Zenject;

public struct PlayerInput : INetworkInput
{
    public float Horizontal;
    public float Vertical;
}

public class PlayerSpawner : SimulationBehaviour, INetworkRunnerCallbacks
{

    private PlayerController _playerController;
    private Doodle.Factory _playerFactory;

    [Inject]
    private void Construct(
        PlayerController playerController,
        Doodle.Factory playerFactory)
    {
        _playerController = playerController;
        _playerFactory = playerFactory;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }



    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.Horizontal = Input.GetAxis("Horizontal");
        playerInput.Vertical = Input.GetAxis("Vertical");
        input.Set(playerInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (Runner.LocalPlayer == player)
        {
            var playerObject = _playerFactory.Create(Vector3.up * 2f, Quaternion.identity, player, Runner);
            PlayerData data = playerObject.GetBehaviour<PlayerData>();
            Doodle doodle = playerObject.GetBehaviour<Doodle>();
            AllPlayersData.Data.Add(player, data);
            _playerController.SetPlayer(doodle);
            FindObjectOfType<LoginScreen>().LoginEvent += (name) => OnLoggedIn(name, player);
        }
    }

    // Activates everyone else for logged player
    private void OnLoggedIn(string nickame, PlayerRef player)
    {
        Doodle[] allDoodles = FindObjectsOfType<Doodle>();
        Doodle playerDoodle = null;
        PlayerData playerData = null;
        foreach (var doodle in allDoodles)
        {
            if (doodle.Object.InputAuthority == player)
            {
                playerDoodle = doodle;
                playerData = doodle.GetBehaviour<PlayerData>();
                Debug.Log("SetPlayerobject : " + player);
                //Runner.SetPlayerObject(player, doodle.Object);
            }
        }

        playerDoodle.SetData(playerData);

        //if (!AllPlayersData.Data.ContainsKey(player))
        //    AllPlayersData.Data.Add(player, playerData);

        Debug.Log("OnLogged In " + nickame);
        if (Runner.IsPlayer)
            Loginner.RPC_Login(Runner, nickame, player);

        AllPlayersData.Data[player].Nickname = nickame;
        AllPlayersData.Data[player].Logged = true;

        // Set other players
        foreach (var doodle in allDoodles)
        {
            if (doodle.Object.InputAuthority == player) continue;
            PlayerData data = doodle.GetBehaviour<PlayerData>();
            Debug.Log("Activate: " + doodle.gameObject.name);
            if (!AllPlayersData.Data.ContainsKey(data.Object.InputAuthority))
                AllPlayersData.Data.Add(data.Object.InputAuthority, data);
            if (data.Logged)
            {
                doodle.SetNickname(data.Nickname.Value);
                doodle.StartMoving();
            }
        }
    }

    // All. Nickname passed straight to set it immediately
    // Activates logged player
    public void SetActivePlayer(PlayerRef player, string nickname)
    {
        Debug.Log("SpawnPlayer");
        if (!AllPlayersData.Data.ContainsKey(player))
        {
            Doodle[] allDoodles = FindObjectsOfType<Doodle>();
            foreach (var otherDoodle in allDoodles)
            {
                if (otherDoodle.Object.InputAuthority == player)
                {
                    AllPlayersData.Data.Add(player, otherDoodle.GetBehaviour<PlayerData>());
                }
            }
        }
        PlayerData data = AllPlayersData.Data[player];
        Doodle doodle = data.GetBehaviour<Doodle>();
        doodle.SetNickname(nickname);
        doodle.StartMoving();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Doodle[] allDoodles = FindObjectsOfType<Doodle>();

        foreach (var doodle in allDoodles)
        {
            if (doodle.Object.InputAuthority == player)
            {
                //Runner.Despawn(doodle.Object);
                doodle.TriggerDespawn();
                AllPlayersData.Data.Remove(player);
            }
        }


        // Reset Player Object
        //Runner.SetPlayerObject(player, null);
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }


}
