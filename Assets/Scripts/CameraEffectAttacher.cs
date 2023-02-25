using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

[ExecuteInEditMode]
public class CameraEffectAttacher : MonoBehaviour
{
    [SerializeField] private Material[] _colorPaletteChangeEffects;

    private int _colorPaletteIndex = -1;

    private void Awake()
    {
        _colorPaletteChangeEffects[0].SetFloat("_HeightMin", 0f);
        _colorPaletteChangeEffects[0].SetFloat("_HeightMax", 1f);

        foreach (var palette in _colorPaletteChangeEffects)
        {
            palette.SetFloat("_HeightMin", 1f);
            palette.SetFloat("_HeightMax", 1f);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_colorPaletteIndex <= 0)
        {
            Graphics.Blit(source, destination, _colorPaletteChangeEffects[0]);
        }
        else
        {
            var temp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            Graphics.Blit(source, temp, _colorPaletteChangeEffects[_colorPaletteIndex-1]);
            Graphics.Blit(temp, destination, _colorPaletteChangeEffects[_colorPaletteIndex]);
            temp.Release();
        }
    }

    public void SetNextColor()
    {
        _colorPaletteIndex++;
        if (_colorPaletteIndex == 0) return;
        _colorPaletteIndex = Mathf.Min(_colorPaletteIndex, _colorPaletteChangeEffects.Length - 1);
        DOTween.To(
            () => _colorPaletteChangeEffects[_colorPaletteIndex - 1].GetFloat("_HeightMax"),
            (x) => _colorPaletteChangeEffects[_colorPaletteIndex - 1].SetFloat("_HeightMax", x),
            0f,
            1f);

        DOTween.To(
            () => _colorPaletteChangeEffects[_colorPaletteIndex].GetFloat("_HeightMin"),
            (x) => _colorPaletteChangeEffects[_colorPaletteIndex].SetFloat("_HeightMin", x),
            0f,
            1f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            SetNextColor();
        }
    }
}
