using Fusion;
using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CameraManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UIController>().AsSingle().NonLazy();
        Container.BindFactory<NetworkRunner, NetworkRunnerFactory>().AsSingle().NonLazy();
        Container.BindFactory<Vector2, Quaternion, PlayerRef, NetworkRunner, Doodle, Doodle.Factory>().AsSingle().NonLazy();
        Container.BindFactory<FollowingCamera, FollowingCamera.Factory>().AsSingle().NonLazy();
    }
}