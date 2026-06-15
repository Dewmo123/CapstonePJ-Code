using Ami.BroAudio;
using UnityEngine;

namespace Code.ETC.Sound
{
    public class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private SoundID bgmID;
        [SerializeField] private float fadeTime = 2f;

        private void Start()
        {
            BroAudio.Play(bgmID)
                .AsBGM()
                .SetTransition(Transition.CrossFade, fadeTime);
        }

        private void OnDestroy()
        {
            if (bgmID.IsValid())
                BroAudio.Stop(bgmID, fadeTime);
        }
    }
}