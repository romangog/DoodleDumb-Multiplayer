using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FollowingCamera : MonoBehaviour
{
    private Transform _target;
    private float _height;

    private bool _IsFollowing;

    private GameSettings _gameSettings;

    [Inject]
    private void Construct(GameSettings gameSettings)
    {
        Debug.Log("Construct");
        _gameSettings = gameSettings;
    }

    public void SetFollowingTarget(Transform target)
    {
        _target = target;
        _height = _target.position.y + (5 - _gameSettings.FallHeightDifference);
        this.transform.position = new Vector3(0, _height, -10);
    }

    private void LateUpdate()
    {
        if (!_IsFollowing) return;

        if (_target)
        {
            _height = Mathf.Max(_height, _target.position.y + (5 - _gameSettings.FallHeightDifference));
            this.transform.position = new Vector3(0, _height, -10);
        }
    }

    internal void StartFollowing()
    {
        _IsFollowing = true;
    }

    internal void StopFollowing()
    {
        _IsFollowing = false;
    }

    
    public class Factory : PlaceholderFactory<FollowingCamera>
    {
        [Inject] DiContainer _container;
        [Inject] Prefabs _prefabs;
        public override FollowingCamera Create()
        {
            FollowingCamera camera =  _container.InstantiatePrefabForComponent<FollowingCamera>(_prefabs.FollowingCamera);
            return camera;
        }
    }
}
