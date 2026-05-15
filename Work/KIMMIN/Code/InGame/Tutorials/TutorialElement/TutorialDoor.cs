using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace Work.Code.Tutorials
{
    public class TutorialDoor : MonoBehaviour
    {
        [SerializeField] private float tweenDuration;
        [SerializeField] private float targetScale;
        [SerializeField] private CinemachineCamera doorCamera;
        
        private float _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale.x;
        }

        public async UniTask OpenDoor()
        {
            doorCamera.Priority = 100;
            gameObject.transform.DOKill();
            
            await UniTask.WaitForSeconds(1.5f);
            gameObject.transform.DOScaleX(targetScale, tweenDuration)
                .OnComplete(() =>
                {
                    doorCamera.Priority = -1;
                });
        }

        public void CloseDoor()
        {
            transform.DOKill();
            transform.DOScaleX(_originalScale, tweenDuration);
        }
    }
}