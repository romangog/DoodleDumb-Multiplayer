using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Fusion;

public class LoginScreen : MonoBehaviour, IUiViewControllable
{
    public Action<string> LoginEvent;

    [SerializeField] private TMP_InputField _nicknameInput;
    [SerializeField] private Button _startButton;
    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private UiView _uiView;

    private bool _IsConnected;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClick);

        _uiView.SetDefaultTransitionProperties(new UiView.UiTransitionProperties()
        {
            Fade = true,
            Scale = false,
            DeactivateAfterHiding = true,
            ActivateBeforeShowing = true,
            Time = 0.5f
        });
    }

    private void OnStartButtonClick()
    {
        if (_nicknameInput.text.Length > 0)
        {
            LoginEvent?.Invoke(_nicknameInput.text);
            Hide();
        }
    }
    private void Update()
    {
        _startButton.interactable = _nicknameInput.text.Length > 0 && _IsConnected;
    }

    public void Show()
    {
        _uiView.Show();
    }

    public void Hide()
    {
        _uiView.Hide();
    }

    internal void OnConnectionFailed()
    {
        _buttonText.text = "CONNECTION FAILED";
    }

    public void SetFailedToConenctView()
    {
        _buttonText.text = "PLAY";
        _startButton.interactable = true;
        _IsConnected = true;
    }
}
