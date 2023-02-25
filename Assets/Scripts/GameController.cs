using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Zenject;
using System.Threading.Tasks;
using System;

public class GameController : IInitializable, ITickable
{
    private readonly UIController _uiController;
    private readonly Loginner _loginner;
    private NetworkRunner _runner;

    private NetworkRunnerFactory _runnerFactory;

    public GameController(UIController uiController,
        Loginner loginner)
    {
        this._uiController = uiController;
        this._loginner = loginner;

        Loginner.LoginnedEvent += OnPlayerLoggedIn;
    }

    private void OnPlayerLoggedIn(PlayerRef playerRef)
    {
        if (_runner.LocalPlayer == playerRef)
            _uiController.SetGameView();
    }

    public void Initialize()
    {
        _runner = _runnerFactory.Create();

        GameObject sceneManager = new GameObject("SceneManager");
        GameObject entryPoint = new GameObject("EntryPoint");

        entryPoint.AddComponent<UnityExecutionEntryPoint>().StartEvent += OnStart;
        
        StartGameArgs args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = sceneManager.AddComponent<NetworkSceneManagerDefault>()
        };

        StartGame(args);
    }

    private void OnStart()
    {
        _uiController.SetLoginView();
    }

    private async void StartGame(StartGameArgs args)
    {
        var result = await _runner.StartGame(args);

        if (result.Ok)
        {
            _uiController.LoginScreen.SetFailedToConenctView();
        }
        else
        {
            _uiController.LoginScreen.SetFailedToConenctView();
        }
    }

    [Inject]
    private void Construct(NetworkRunnerFactory runnerFactory)
    {
        _runnerFactory = runnerFactory;
    }

    public void Tick()
    {
        _uiController.GameplayScreen.MasterText.text = "Master: " + _runner.IsSharedModeMasterClient.ToString();
    }
}

public class NetworkRunnerFactory : PlaceholderFactory<NetworkRunner>
{
    [Inject]
    private DiContainer Container;
    public override NetworkRunner Create()
    {
        return Container.InstantiatePrefabResourceForComponent<NetworkRunner>("Runner");
    }
}
