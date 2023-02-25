using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPart : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private CanvasGroup _canvasGroup;
    private Rigidbody2D _rigidbody;
    private Vector3 _localPos;

    private bool _IsCanvas;

    public void Initialize()
    {
        _localPos = this.transform.localPosition;
        _rigidbody = this.GetComponent<Rigidbody2D>();

        _IsCanvas = this.TryGetComponent(out _canvasGroup);
        if (!_IsCanvas)
            this.TryGetComponent(out _spriteRenderer);
    }

    public void Detach()
    {
        _rigidbody.simulated = true;
        this.gameObject.SetActive(true);
        float torque = -(_localPos.x);
        _rigidbody.AddTorque(torque * 1000);
        _rigidbody.AddForce(((_localPos).normalized + Vector3.up) * 100);

        if (_IsCanvas)
            _canvasGroup.DOFade(0f, 1f).OnComplete(OnCompletedDetaching);
        else
            _spriteRenderer.DOFade(0f, 1f).OnComplete(OnCompletedDetaching);

        void OnCompletedDetaching()
        {
            _rigidbody.simulated = false;
            gameObject.SetActive(false);
        }
    }



    internal void Return()
    {
        this.gameObject.SetActive(true);
        _rigidbody.simulated = false;

        transform.DOLocalMove(_localPos, 1f);
        transform.DOLocalRotate(Vector3.zero, 1f);

        if (_IsCanvas)
            _canvasGroup.DOFade(1f, 1f).OnComplete(OnCompletedReturning);
        else
            _spriteRenderer.DOFade(1f, 1f).OnComplete(OnCompletedReturning);

        void OnCompletedReturning()
        {
            this.gameObject.SetActive(false);
        }
    }


}
