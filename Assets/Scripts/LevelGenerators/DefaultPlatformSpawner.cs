using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlatformSpawner : LevelObjectsSpawner
{
    private float _nextObjectHeigth;
    private float _xOffset;
    private float _minHeightStep = 1f;
    private float _maxHeightStep = 5f;

    public DefaultPlatformSpawner(NetworkRunner runner, Prefabs prefabs) : base(runner, prefabs)
    {

    }

    public override void GenerateAtStart()
    {

        for (int i = 0; i < 3; i++)
        {
            GenerateObject();
        }
    }

    public override void CheckTick(float currentMaxHeight)
    {
        if (currentMaxHeight + _maxHeightStep * 2 > _nextObjectHeigth)
        {
            GenerateObject();
        }
    }


    public override void GenerateObject()
    {
        _xOffset = UnityEngine.Random.Range(-1.5f, 1.5f);
        _nextObjectHeigth += UnityEngine.Random.Range(_minHeightStep, _maxHeightStep);
        var platform = Runner.Spawn(Prefabs.DefaultPlarform, new Vector3(_xOffset, _nextObjectHeigth, 0f), inputAuthority: PlayerRef.None);
        //platform.GetBehaviour<NetworkTransform>().Transform.SetParent(this.transform);
    }
}
