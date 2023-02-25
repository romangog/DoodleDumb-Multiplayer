using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPlatformSpawner : LevelObjectsSpawner
{
    private float _nextObjectHeigth;
    private float _xOffset;
    private float _spawnStep = 50f;

    public CheckpointPlatformSpawner(NetworkRunner runner, Prefabs prefabs) : base(runner, prefabs)
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
        if (currentMaxHeight + _spawnStep * 2 > _nextObjectHeigth)
        {
            GenerateObject();
        }
    }


    public override void GenerateObject()
    {
        var platform = Runner.Spawn(Prefabs.CheckpointPlatform, new Vector3(_xOffset, _nextObjectHeigth, 0f), inputAuthority: PlayerRef.None);
        //platform.GetBehaviour<NetworkTransform>().Transform.SetParent(this.transform);
        _xOffset = UnityEngine.Random.Range(-1.5f, 1.5f);
        _nextObjectHeigth += _spawnStep;
    }
}
