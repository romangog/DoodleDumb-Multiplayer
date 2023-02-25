using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Checkpoint : LevelObject
{
    [SerializeField] private SpriteRenderer _mainSprite;
    [SerializeField] private Transform _patternMaskTransform;
    [SerializeField] private PlayerTriggerObserver _playerTriggerObserver;
    [SerializeField] private Transform _flagTransform;
    [SerializeField] private Transform _hitbox;
    private bool _IsActivated;
    
    public override void Spawned()
    {
        _playerTriggerObserver.PlayerTriggeredEvent += OnTriggered;
        base.Spawned();
    }

    private void OnTriggered(Doodle doodle)
    {
        if (_IsActivated || doodle.Object.InputAuthority != Runner.LocalPlayer) return;
        _IsActivated = true;
        doodle.RPC_SetCheckpoint(doodle.Object.InputAuthority, this.transform.position.y + 0.2f);
        ActivateCheckpoint();
    }

    public override void JumpInteract()
    {
        return;
    }

    public void ActivateCheckpoint()
    {
        float sizeMultiplier = 6f;
        float time = 3f;

        _flagTransform.DOLocalRotate(Vector3.zero, time / 2f).SetEase(Ease.OutBack);
        DOTween.To(() => _mainSprite.size, (x) => _mainSprite.size = x, new Vector2(_mainSprite.size.x * sizeMultiplier, _mainSprite.size.y) , time);
        _patternMaskTransform.DOScaleX(sizeMultiplier, time);
        _hitbox.DOScaleX(sizeMultiplier, time);

        // Play Animation
    }
}
