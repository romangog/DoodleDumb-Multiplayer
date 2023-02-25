using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainView;

    private UiTransitionProperties _defaultTransitionProperties;
    private bool _IsDefault;
    private bool _IsShown;

    public void SetDefaultTransitionProperties(UiTransitionProperties p)
    {
        _defaultTransitionProperties = p;
        _IsDefault = true;
    }

    public virtual void Show(UiTransitionProperties p = default(UiTransitionProperties))
    {
        if (_IsShown) return;

        if (_IsDefault)
            p = _defaultTransitionProperties;

        Sequence sequence = null;

        if (p.Fade)
            sequence.Append(_mainView.DOFade(1f, p.Time));
        else
            sequence.PrependCallback(() => _mainView.alpha = 1f);

        if (p.Scale)
            sequence.Join(_mainView.transform.DOScale(1f, p.Time).SetEase(Ease.OutBack));

        if (p.ActivateBeforeShowing)
            sequence.AppendCallback(() => _mainView.gameObject.SetActive(true));

        _mainView.blocksRaycasts = true;

        _IsShown = true;
    }

    public virtual void Hide(UiTransitionProperties p = default(UiTransitionProperties))
    {
        if (!_IsShown) return;

        if (_IsDefault)
            p = _defaultTransitionProperties;

        Sequence sequence = null;

        if (p.Fade)
            sequence.Append(_mainView.DOFade(0f, p.Time));
        else
            sequence.PrependCallback(() => _mainView.alpha = 0f);

        if (p.Scale)
            sequence.Join(_mainView.transform.DOScale(0f, p.Time).SetEase(Ease.InBack));

        if (p.DeactivateAfterHiding)
            sequence.AppendCallback(() => _mainView.gameObject.SetActive(false));

        _mainView.blocksRaycasts = false;

        _IsShown = false;
    }

    public struct UiTransitionProperties
    {
        public bool Fade;
        public float Time;
        public bool DeactivateAfterHiding;
        public bool ActivateBeforeShowing;
        public bool Scale;
    }
}
