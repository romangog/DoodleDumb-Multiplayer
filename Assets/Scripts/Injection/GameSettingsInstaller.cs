using Cinemachine;
using Fusion;
using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [Header("PREFABS")]
    public Prefabs LeaderboardRow;

    [Header("PREFABS")]
    public Doodle.Settings DoodleSettings;
    
    [Header("Base Settings")]
    public GameSettings GameSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(LeaderboardRow).AsSingle().NonLazy();
        Container.BindInstance(DoodleSettings).AsSingle().NonLazy();
        Container.BindInstance(GameSettings).AsSingle().NonLazy();
    }
}

[Serializable]
public class Prefabs
{
    public TMPro.TMP_Text LeaderboardRow;
    public NetworkPrefabRef DefaultPlarform;
    public NetworkPrefabRef WeakPlatform;
    public NetworkPrefabRef CheckpointPlatform;
    public FollowingCamera FollowingCamera;

    public NetworkPrefabRef PlayerPrefab;
}

[Serializable]
public class GameSettings
{
    public float FallHeightDifference;
}