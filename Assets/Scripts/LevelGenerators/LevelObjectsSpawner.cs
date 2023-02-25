using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelObjectsSpawner
{
    protected NetworkRunner Runner;
    protected Prefabs Prefabs;

    public LevelObjectsSpawner(NetworkRunner runner, Prefabs prefabs)
    {
        Runner = runner;
        Prefabs = prefabs;
    }

    public abstract void GenerateAtStart();

    public abstract void GenerateObject();

    public abstract void CheckTick(float currentMaxHeight);
}
