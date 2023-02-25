using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatformSpawner : LevelObjectsSpawner
{
    private float _nextObjectHeight = 50;
    private float _xOffset;
    private float _minHeight = 1f;
    private float _maxHeight = 15f;
    public WeakPlatformSpawner(NetworkRunner runner, Prefabs prefabs) : base(runner, prefabs)
    {

    }

    public override void GenerateAtStart()
    {

        GenerateObject();
    }

    public override void CheckTick(float currentMaxHeight)
    {
        if (currentMaxHeight + _maxHeight * 2 > _nextObjectHeight)
        {
            GenerateObject();
        }
    }


    public override void GenerateObject()
    {
        var platform = Runner.Spawn(Prefabs.WeakPlatform, new Vector3(_xOffset, _nextObjectHeight, 0f), inputAuthority: PlayerRef.None);
        //platform.GetBehaviour<NetworkTransform>().Transform.SetParent(this.transform);
        _xOffset = UnityEngine.Random.Range(-1.5f, 1.5f);
        _nextObjectHeight += UnityEngine.Random.Range(_minHeight, _maxHeight);
    }
}
