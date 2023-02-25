using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;
using DG.Tweening;
using Zenject;
using System.Threading.Tasks;

public class Doodle : NetworkBehaviour
{
    public Action FallenEvent;
    public Action RevivedEvent;

    public Transform Visual => _visual;
    public PlayerData data => Data;
    [SerializeField] private Transform _visual;
    [SerializeField] private GameObject _mainSprite;
    [SerializeField] private GameObject _canvasAsMain;
    [SerializeField] private Transform _hitbox;
    [SerializeField] private NetworkRigidbody2D _rigidbody;
    public PlayerData Data;
    [SerializeField] private LayerMask _platformLayer;
    [SerializeField] private TMP_Text[] _nameTags;

    [SerializeField] private PlayerPart[] _disintegrationParts;

    private float _height;
    private Rigidbody2D _rigidbody2D = null;


    private bool _CanJump;
    private bool _IsMoving;



    private bool _IsFallen;
    private Settings _settings;
    private GameSettings _gameSettings;
    private CameraEffectAttacher _cameraEffectAttacher;
    private FxPool _doodleStepDustFxPool;

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log($"isStateAuthority: {Object.HasStateAuthority}, IsInputAuthority" +
            $" {Object.HasInputAuthority}, R.IsClient: {Runner.IsClient}, R.IsClient: {Runner.IsClient}," +
            $"R.IsPlayer: {Runner.IsPlayer}, R.IsServer: {Runner.IsServer}, R.IsSharedMaster: {Runner.IsSharedModeMasterClient}");
        foreach (var part in _disintegrationParts)
        {
            part.Initialize();
            part.gameObject.SetActive(false);
        }

        if (Object.HasStateAuthority)
        {
            _rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        _visual.gameObject.SetActive(false);
        _hitbox.gameObject.SetActive(false);
    }

    public void Construct(
        Settings settings,
        GameSettings gameSettings,
        CameraEffectAttacher cameraEffectAttacher,
        FxPools fxPools)
    {
        _settings = settings;
        _gameSettings = gameSettings;
        _cameraEffectAttacher = cameraEffectAttacher;
        _doodleStepDustFxPool = fxPools.DoodleStepDustFx;
    }

    public void StartMoving()
    {
        Debug.Log("Start moving " + name);
        _rigidbody.Rigidbody.simulated = true;
        _visual.gameObject.SetActive(true);
        _hitbox.gameObject.SetActive(true);
        _IsMoving = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_IsMoving) return;
        //if (GetInput(out PlayerInput input))
        if (!Object.HasStateAuthority) return;

        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input))
        {
            _CanJump = _rigidbody.Rigidbody.velocity.y < 0;
            //Vector2 newPosition = _rigidbody2D.position + new Vector2(input.Horizontal * _moveSpeed * Runner.DeltaTime, input.Vertical * _moveSpeed * Runner.DeltaTime);
            //newPosition.x = Mathf.Clamp(newPosition.x, -2.55f, 2.55f);
            //_rigidbody2D.position += (newPosition);
            Vector2 newPosition = _rigidbody2D.position + new Vector2(input.Horizontal * _settings.MoveSpeed * Runner.DeltaTime, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, -2.15f, 2.15f);
            _rigidbody2D.position = (newPosition);

            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = false;
            RaycastHit2D hit = Physics2D.BoxCast(_rigidbody2D.position, new Vector2(0.753f, 0.05f), 0f, Vector3.down, -_rigidbody2D.velocity.y * Runner.DeltaTime, _platformLayer);
            if (_CanJump && hit)
            {
                _rigidbody2D.velocity = Vector2.up * _settings.JumpSpeed;
                if (hit.transform.parent && hit.transform.parent.TryGetComponent(out LevelObject levelObject))
                {
                    levelObject.JumpInteract();
                }
                RPC_PlayStepDustFx(Object.InputAuthority, hit.point + Vector2.up * 0.1f);
            }

            CheckFall();

            _rigidbody.Transform.localScale = new Vector3(_rigidbody.Transform.localScale.x,  (1f+ _rigidbody2D.velocity.y/30f), _rigidbody.Transform.localScale.z);

            _height = _rigidbody.ReadPosition().y;
            Data.MaxHeight = Mathf.Max(Data.MaxHeight, _height);
        }
        //Debug.Log("Player update " + name);
        //if (name == "ROMA")
        //{
        //    Debug.Log("Input: " + input.Horizontal);
        //}

    }

    private void CheckFall()
    {
        if (!_IsFallen && _height < Data.MaxHeight - _gameSettings.FallHeightDifference)
        {
            _IsFallen = true;
            FallenEvent?.Invoke();
        }
    }

    internal void SetData(PlayerData data)
    {
        Data = data;
    }

    internal void SetNickname(string value)
    {
        gameObject.name = value;
        foreach (var nameTag in _nameTags)
        {
            nameTag.text = value;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    internal void RPC_PlayStepDustFx(PlayerRef player, Vector2 position)
    {
        if (Object.InputAuthority != player) return;
        _doodleStepDustFxPool.ActivateEffect().transform.position = position;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    internal void RPC_SetCheckpoint(PlayerRef player, float height)
    {
        if (Object.InputAuthority != player) return;
        Data.CheckpointHeight = height;
        _cameraEffectAttacher.SetNextColor();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_Disintegrate(PlayerRef player, RpcInfo info = default)
    {
        if (Object.InputAuthority != player) return;

        foreach (var part in _disintegrationParts)
        {
            part.Detach();
        }
        _IsMoving = false;
        _rigidbody.Rigidbody.simulated = false;
        _mainSprite.SetActive(false);
        _canvasAsMain.SetActive(false);
        _hitbox.gameObject.SetActive(false);
    }

    // Change RPC targets to InputAuthority and remove PlayerRef check
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    internal void RPC_Revive(PlayerRef player, RpcInfo info = default)
    {
        if (Object.InputAuthority != player) return;
        StartCoroutine(ReviveRoutine());

    }

    internal async void TriggerDespawn()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
        else if (Runner.IsSharedModeMasterClient)
        {
            Object.RequestStateAuthority();

            while (Object.HasStateAuthority == false)
            {
                await Task.Delay(100); // wait for Auth transfer
            }

            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }

    private IEnumerator ReviveRoutine()
    {
        yield return new WaitForSeconds(2f);
        _IsFallen = false;
        _rigidbody.TeleportToPosition(new Vector2(0, Data.CheckpointHeight));
        _height = Data.CheckpointHeight;
        Data.MaxHeight = Data.CheckpointHeight;
        RevivedEvent?.Invoke();
        yield return new WaitForSeconds(1f);
        foreach (var part in _disintegrationParts)
            part.Return();
        yield return new WaitForSeconds(1f);
        _IsMoving = true;
        _mainSprite.SetActive(true);
        _canvasAsMain.SetActive(true);
        _hitbox.gameObject.SetActive(true);
        _rigidbody.Rigidbody.simulated = true;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }

    [Serializable]
    public class Settings
    {
        public float JumpSpeed;
        public float MoveSpeed;
    }

    public class Factory : PlaceholderFactory<Vector2, Quaternion, PlayerRef, NetworkRunner, Doodle>
    {
        [Inject] private Prefabs _prefabs;
        [Inject] private Settings _settings;
        [Inject] private GameSettings _gameSettings;
        [Inject] private CameraEffectAttacher _cameraEffectAttacher;
        [Inject] private FxPools _fxPools;

        public override Doodle Create(Vector2 position, Quaternion rotation, PlayerRef playerRef, NetworkRunner runner)
        {
            NetworkObject doodleObject = runner.Spawn(_prefabs.PlayerPrefab, position, rotation, playerRef);
            Doodle doodle = doodleObject.GetBehaviour<Doodle>();
            doodle.Construct(_settings, _gameSettings, _cameraEffectAttacher, _fxPools);
            return doodle;
        }
    }
}
