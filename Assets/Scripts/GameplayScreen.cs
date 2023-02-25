using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayScreen : MonoBehaviour, IUiViewControllable
{
    [SerializeField] private UiView _view;
    [SerializeField] private Leaderboard _leaderboard;
    public TMPro.TMP_Text MasterText;

    private void Awake()
    {
        _view.SetDefaultTransitionProperties(new UiView.UiTransitionProperties()
        {
            Fade = true,
            Scale = false,
            DeactivateAfterHiding= true,
            ActivateBeforeShowing = true
        });   
    }

    public void SetLeaderboardData(PlayerRef player, PlayerLeaderboardData data) 
        => _leaderboard.SetData(player, data);

    internal void DeleteLeadeboardData(PlayerRef player) => _leaderboard.DeleteData(player);


    public void Show()
    {
        _view.Show();
    }

    public void Hide()
    {
        _view.Hide();
    }

}
