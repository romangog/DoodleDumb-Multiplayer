using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerObserver : MonoBehaviour
{
    public Action<Doodle> PlayerTriggeredEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.parent.TryGetComponent(out Doodle doodle))
        {
            PlayerTriggeredEvent?.Invoke(doodle);
        }
    }
}
