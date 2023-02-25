using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class TextInputFieldMod : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;

    private void Start()
    {
        _inputField.onValueChanged.AddListener(OnInputChanged);
    }

    private string CleanInput(string strIn)
    {
        return Regex.Replace(strIn,
              @"[^a-zA-Z0-9]", "");
    }


    private void OnInputChanged(string input)
    {
        _inputField.text = CleanInput(input);
    }
}
