using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController
{
    readonly LoginScreen _loginScreen;
    readonly GameplayScreen _gameplayScreen;

    public LoginScreen LoginScreen => _loginScreen;
    public GameplayScreen GameplayScreen => _gameplayScreen;

    public UIController(
        LoginScreen loginScreen,
        GameplayScreen gameplayScreen)
    {
        _loginScreen = loginScreen;
        _gameplayScreen = gameplayScreen;
    }

    public void SetGameView()
    {
        _loginScreen.Hide();
        _gameplayScreen.Show();
    }

    public void SetLoginView()
    {
        _loginScreen.Show();
        _gameplayScreen.Hide();
    }
}
