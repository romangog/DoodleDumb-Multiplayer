using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Zenject;
using System;

public struct PlayerLeaderboardData
{
    public string Name;
    public int Distance;
    public bool Logged;

    public override string ToString()
    {
        if(Logged)
        return string.Format($"{Name}: {Distance, 10}");
        else
        return string.Format($"Player is connecting...");
    }
}

public class Leaderboard : MonoBehaviour
{
    private Dictionary<PlayerRef, TMP_Text> _textFields = new Dictionary<PlayerRef, TMP_Text>();

    private TMP_Text _rowPrefab;

    [Inject]
    private void Construct(Prefabs prefabs)
    {
        _rowPrefab = prefabs.LeaderboardRow;
    }

    public void SetData(PlayerRef player, PlayerLeaderboardData data)
    {
        if(!_textFields.ContainsKey(player))
        {
            TMP_Text rowInstance = Instantiate(_rowPrefab, this.transform);
            rowInstance.rectTransform.anchoredPosition = new Vector2(0, -22 * _textFields.Count);
            _textFields.Add(player, rowInstance);
        }

        _textFields[player].text = data.ToString();
    }

    internal void DeleteData(PlayerRef player)
    {
        Debug.Log("Delete data for " + player);
        if(_textFields.ContainsKey(player))
        {
            Destroy(_textFields[player].gameObject);
            _textFields.Remove(player);
        }


        int i = 0;
        foreach (var textFieldPair in _textFields)
        {
            textFieldPair.Value.rectTransform.anchoredPosition = new Vector2(0, -22 * i);
            i++;
        }
    }
}
