using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.ETC.MapObjects;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using DG.Tweening;
using Scripts.Combat.Datas;
using Scripts.Effects;
using Scripts.Entities;
using Unity.Mathematics;
using UnityEngine;
using Work.LKW.Code.Items;
using Work.LKW.Code.Items.ItemInfo;
using Random = UnityEngine.Random;

namespace Code.ETC.MapObjects
{
    public class VendingMachine : InteractableHittableObject
    {
        [Inject]
        private PoolManagerMono _poolManagerMono;
        

        [Header("Reference")]
        [SerializeField] private PoolItemSO explosiveItem;
        [SerializeField] private LayerMask whatIsBullet;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private ItemDataBaseSO itemDB;
        [SerializeField] private PoolItemSO previewItem;
        [SerializeField] private Transform discardPoint;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float offDuration = 1f;
        [SerializeField] private float discardRange = 0.5f;
        [SerializeField] private float interactCooldown = 1f;
        [SerializeField] private List<ItemType> allowedTypes;

        private readonly int _maxSpawnCount = 5;
        private bool _isOff = false;
        private float _lastInteractTime = float.NegativeInfinity;
        private Material _material;

        protected override void Awake()
        {
            base.Awake();
            _material = meshRenderer.material;
        }

        protected override void Start()
        {
            base.Start();
            Init();
        }

        private void Init()
        {
            _material.EnableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", Color.white * 10.0f);
            SetMaxHp(Random.Range(1, _maxSpawnCount));
            _isOff = false;
        }

        public override void Interact(Entity interactor)
        {
            if (_isOff || IsDead) return;
            if (Time.time - _lastInteractTime < interactCooldown) return;
            _lastInteractTime = Time.time;
            ApplyDamage(new DamageData { damage = 1 });
        }

        public override void TakeHit()
        {
            base.TakeHit();
            if (_isOff) return;

            float hitsTaken = MaxHp - CurrentHp;
            if (hitsTaken > 1)
            {
                float explodeChance = (hitsTaken - 1) / MaxHp;
                if (Random.value < explodeChance)
                {
                    Explode();
                    return;
                }
            }

            SpawnItem();
        }

        protected override void OnDeath()
        {
            if (_isOff) return;
            _isOff = true;
            StartCoroutine(MachineOffCoroutine(offDuration));
        }

        private void Explode()
        {
            _isOff = true;
            StartCoroutine(MachineOffCoroutine(offDuration));

            PoolingEffect effect = _poolManagerMono.Pop<PoolingEffect>(explosiveItem);
            if (effect != null) effect.PlayVFX(transform.position, quaternion.identity);

            Kill();
        }

        private void SpawnItem()
        {
            if (allowedTypes == null || allowedTypes.Count == 0) return;

            ItemType type = allowedTypes[Random.Range(0, allowedTypes.Count)];
            var targetItemData = itemDB.GetRandomItems(type, 1).FirstOrDefault();
            if (targetItemData == null) return;

            ItemCreateData createData = targetItemData.CreateItem();
            var spawnPreviewItem = _poolManagerMono.Pop<PreviewItem>(previewItem);

            Vector3 discardPos = discardPoint.position;
            discardPos.x += Random.Range(-discardRange, discardRange);
            discardPos.z += Random.Range(-discardRange, discardRange);
            discardPos.y += 0.2f;

            spawnPreviewItem.Discard(spawnPoint.position, createData.Item, createData.Stack);
            spawnPreviewItem.transform.DOMove(discardPos, 0.25f).SetEase(Ease.InCubic);
        }

        private IEnumerator MachineOffCoroutine(float duration)
        {
            Color startColor = _material.GetColor("_EmissionColor");
            Color endColor = Color.white * 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _material.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, elapsed / duration));
                yield return null;
            }

            _material.SetColor("_EmissionColor", endColor);
            _material.DisableKeyword("_EMISSION");
        }
    }
}
