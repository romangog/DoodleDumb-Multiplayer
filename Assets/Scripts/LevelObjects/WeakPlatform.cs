using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : LevelObject
{
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private SpriteRenderer _defaultSprite;
    [SerializeField] private Rigidbody2D [] _destroyParts;

    public override void JumpInteract()
    {
        // Destroy On Touch
        RPC_DestroyWeakPlatform(Object.Id);
        //Runner.Despawn(this.Object);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DestroyWeakPlatform(NetworkId id, RpcInfo info = default)
    {
        if (Object.Id != id) return;
        _defaultSprite.gameObject.SetActive(false);
        _hitbox.SetActive(false);
        foreach (var part in _destroyParts)
        {
            part.simulated = true;
            float torque = -(part.transform.position.x - this.transform.position.x);
            part.AddTorque(torque * 1000);
            part.AddForce((part.transform.position - this.transform.position).normalized * 50);
        }
        // Play Animation
    }
}
