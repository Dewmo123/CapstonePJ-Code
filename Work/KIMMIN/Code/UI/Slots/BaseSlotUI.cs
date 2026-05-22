using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.UI.Interaction;

namespace Work.Code.UI.Slots
{
    public class BaseSlotUI : DraggableUI
    {
        [SerializeField] protected Image outline;
        [SerializeField] protected Image backgroundEffect;
        
        private const float BackgroundEffectDuration = 1f;
        private Sequence _backgroundEffectSeq;
        protected bool IsBackgroundEffectPlaying { get; private set; }

        public void PlayBackgroundEffect(Color effectColor)
        {
            StopBackgroundEffect();
            _backgroundEffectSeq = DOTween.Sequence();
            backgroundEffect.color = effectColor;
            IsBackgroundEffectPlaying = true;
            
            _backgroundEffectSeq.Append(backgroundEffect.transform.DOScale(1.3f, BackgroundEffectDuration))
                .SetEase(Ease.OutCirc);
            _backgroundEffectSeq.Join(backgroundEffect.DOFade(0f, BackgroundEffectDuration))
                .SetEase(Ease.OutCirc);
            _backgroundEffectSeq.SetLoops(-1, LoopType.Restart);
            _backgroundEffectSeq.Goto(Time.time % BackgroundEffectDuration, true);
        }

        public void StopBackgroundEffect()
        {
            backgroundEffect.transform.localScale = Vector3.one;
            backgroundEffect.color = new Color(0f, 0f, 0f, 0f);
            _backgroundEffectSeq?.Kill();
            IsBackgroundEffectPlaying = false;
        }
    }
}
