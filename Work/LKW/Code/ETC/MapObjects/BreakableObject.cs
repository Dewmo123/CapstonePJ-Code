using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using DG.Tweening;
using Scripts.Effects;
using Scripts.GameSystem;
using UnityEngine;
using Code.ETC.MapObjects;
using Work.LKW.Code.Items;
using Work.LKW.Code.Items.ItemInfo;
using Random = UnityEngine.Random;

namespace Code.ETC.MapObjects
{
    public class BreakableObject : HittableObject
    {
        [Inject]
        private PoolManagerMono _poolManagerMono;

        [Header("Fragments")]
        [SerializeField] private List<GameObject> fragmentPrefabs;
        [SerializeField] private int minFragments = 3;
        [SerializeField] private int maxFragments = 6;
        [SerializeField] private float minFragmentForce = 0.1f;
        [SerializeField] private float maxFragmentForce = 0.5f;
        [SerializeField] private float minFragmentTorque = 0.1f;
        [SerializeField] private float maxFragmentTorque = 0.5f;

        [Header("Effect")]
        [SerializeField] private PoolItemSO breakEffect;

        [Header("Fade")]
        [SerializeField] private float minFadeDelay = 0.5f;
        [SerializeField] private float maxFadeDelay = 1f;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("Item Drop")]
        [SerializeField] private ItemDataBaseSO itemDB;
        [SerializeField] private List<ItemType> dropTypes;
        [SerializeField] private PoolItemSO previewItemPool;

        protected override void OnDeath()
        {
            StartCoroutine(BreakCoroutine());
        }

        private IEnumerator BreakCoroutine()
        {
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
            foreach (var c in GetComponentsInChildren<Collider>())
                c.enabled = false;

            if (breakEffect != null)
            {
                var effect = _poolManagerMono.Pop<PoolingEffect>(breakEffect);
                effect?.PlayVFX(transform.position, Quaternion.identity);
            }
            
            GetComponent<Collider>().enabled = false;

            int count = Random.Range(minFragments, maxFragments + 1);
            var fragments = new List<GameObject>(count);

            for (int i = 0; i < count; i++)
            {
                if (fragmentPrefabs == null || fragmentPrefabs.Count == 0) break;

                Vector3 spawnOffset = Random.insideUnitSphere * 0.1f;

                var prefab = fragmentPrefabs[Random.Range(0, fragmentPrefabs.Count)];
                var fragment = Instantiate(prefab,transform.position + spawnOffset, Random.rotation);
                fragments.Add(fragment);

                if (fragment.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 dir = Random.onUnitSphere;
                    dir.y = Mathf.Abs(dir.y);
                    
                    float fragmentForce = Random.Range(minFragmentForce, maxFragmentForce);
                    float fragmentTorque = Random.Range(minFragmentTorque, maxFragmentTorque);
                    rb.AddForce(dir * fragmentForce, ForceMode.Impulse);
                    rb.AddTorque(Random.onUnitSphere * fragmentTorque, ForceMode.Force);
                }
            }

            if (dropTypes != null && dropTypes.Count > 0 && itemDB != null)
                SpawnItemDrop();

            yield return new WaitForSeconds(Random.Range(minFadeDelay, maxFadeDelay));
            yield return FadeOutFragments(fragments, fadeDuration);

            foreach (var f in fragments)
                if (f != null) Destroy(f);

            Destroy(gameObject);
        }

        private IEnumerator FadeOutFragments(List<GameObject> fragments, float duration)
        {
            var materials = new List<Material>();
            foreach (var f in fragments)
            {
                if (f == null) continue;
                foreach (var r in f.GetComponentsInChildren<Renderer>())
                    materials.AddRange(r.materials);
            }

            var tweens = new List<Tweener>(materials.Count);
            foreach (var mat in materials)
                tweens.Add(mat.DOFade(0f, duration));

            yield return new WaitForSeconds(duration);

            foreach (var t in tweens)
                t?.Kill();
        }

        private void SpawnItemDrop()
        {
            var type = dropTypes[Random.Range(0, dropTypes.Count)];
            var itemData = itemDB.GetRandomItems(type, 1).FirstOrDefault();
            if (itemData == null) return;

            var createData = itemData.CreateItem();
            var spawnedItem = _poolManagerMono.Pop<PreviewItem>(previewItemPool);
            spawnedItem?.Discard(transform.position, createData.Item, createData.Stack);
        }
    }
}
