using EPOOutline;
using Scripts.GameSystem.Structures;
using UnityEngine;

namespace Code.NPC
{
    public class NPC : InvokeCallbackStructure
    {
        [SerializeField] private Transform visualRoot;

        private NPCDataSO _currentData;
        private GameObject _visualInstance;

        public void SetData(NPCDataSO npcData)
        {
            if(_visualInstance != null)
                Destroy(_visualInstance);
            
            if (npcData == null)
            {
                Debug.Log("data is not valid");
                return;
            }
            
            _currentData = npcData;
            
            _visualInstance = Instantiate(npcData.npcVisual, visualRoot);
            _visualInstance.transform.localPosition = Vector3.zero;
            _visualInstance.transform.localRotation = Quaternion.identity;
            
            // 생성된 NPC 외형 아웃라인 동기화 해주기
            var renderers = _visualInstance.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                Outlinable.AddTarget(new OutlineTarget(renderer));
            }
        }

        public override void Despawn()
        {
            // despawn effect play
            base.Despawn();
        }
    }
}