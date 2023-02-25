using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Zenject;

public class LevelGenerator : SimulationBehaviour, ISpawned
{
    [SerializeField] private NetworkPrefabRef _plarformPrefab;

    private float CurrentMaxHeight;

    private List<LevelObjectsSpawner> _levelObjectSpawners = new List<LevelObjectsSpawner>();

    private Prefabs _prefabs;

    [Inject]
    private void Construct(Prefabs prefabs)
    {
        _prefabs = prefabs;
    }

    public void Spawned()
    {

        _levelObjectSpawners.Add(new DefaultPlatformSpawner(Runner, _prefabs));
        _levelObjectSpawners.Add(new WeakPlatformSpawner(Runner, _prefabs));
        _levelObjectSpawners.Add(new CheckpointPlatformSpawner(Runner, _prefabs));

        Debug.Log("LevelGenerator: Spawned");
        if (Runner.IsSharedModeMasterClient)
        {
            Debug.Log("LevelGenerator: StartLevelGeneration");
            StartLevelGeneration();
        }
    }

    public void StartLevelGeneration()
    {
    }


    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        foreach (var playerData in AllPlayersData.Data)
        {
            if (playerData.Value.MaxHeight > CurrentMaxHeight)
            {
                CurrentMaxHeight = playerData.Value.MaxHeight;
            }
        }

        foreach (var spawner in _levelObjectSpawners)
        {
            spawner.CheckTick(CurrentMaxHeight);
        }
    }

}
