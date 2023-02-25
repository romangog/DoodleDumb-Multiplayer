using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    private readonly Prefabs _prefabs;

    private FollowingCamera _currentFollowingCamera;
    private FollowingCamera.Factory _followingCameraFactory;

    public CameraManager(Prefabs prefabs,
        FollowingCamera.Factory factory)
    {
        this._prefabs = prefabs;
        this._followingCameraFactory = factory;
    }

    public void CreateNewPlayerCamera(Transform playerTransform)
    {
        FollowingCamera newCam = _followingCameraFactory.Create();

        newCam.SetFollowingTarget(playerTransform);

        if(_currentFollowingCamera)
        {
            GameObject.Destroy(_currentFollowingCamera.gameObject, 3f);
        }

        _currentFollowingCamera = newCam;
    }

    public void StartFollowing() => _currentFollowingCamera?.StartFollowing();

    public void StopFollowing() => _currentFollowingCamera?.StopFollowing();
}
