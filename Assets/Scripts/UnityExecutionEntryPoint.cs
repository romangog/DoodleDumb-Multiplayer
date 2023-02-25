using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UnityExecutionEntryPoint : MonoBehaviour
{
    public Action StartEvent;

    void Start()
    {
        StartEvent?.Invoke();
    }
}
