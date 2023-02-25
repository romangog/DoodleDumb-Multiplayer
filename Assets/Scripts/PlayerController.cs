using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerController
{
    private readonly CameraManager _cameraManager;

    private Doodle _playerDoodle;

    public PlayerController(CameraManager cameraManager)
    {
        this._cameraManager = cameraManager;
    }

    public void SetPlayer(Doodle doodle)
    {
        _playerDoodle = doodle;
        _playerDoodle.FallenEvent += OnPlayerFallen;
        _playerDoodle.RevivedEvent += OnPlayerRevived;
        _cameraManager.CreateNewPlayerCamera(_playerDoodle.Visual);
        _cameraManager.StartFollowing();
    }

    private void OnPlayerRevived()
    {
        _cameraManager.CreateNewPlayerCamera(_playerDoodle.Visual);

        Observable.Start(_cameraManager.StartFollowing,TimeSpan.FromSeconds(0.2f)).Subscribe();
    }

    private void OnPlayerFallen()
    {
        _cameraManager.StopFollowing();
        _playerDoodle.RPC_Disintegrate(_playerDoodle.Object.InputAuthority);
        _playerDoodle.RPC_Revive(_playerDoodle.Object.InputAuthority);
    }
}
