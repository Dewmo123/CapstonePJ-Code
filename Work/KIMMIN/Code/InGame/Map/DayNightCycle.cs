using Code.TimeSystem;
using DewmoLib.Dependencies;
using UnityEngine;

namespace Work.Code.Map
{
    public class DayNightCycle : MonoBehaviour
    {
        [SerializeField] private Light sun;
        [Inject] private TimeController _timeController;
        
        private void Update()
        {
            float normalizedTime = _timeController.DayTime / _timeController.SecondsPerDay;
            UpdateSun(normalizedTime);
        }
        
        private void UpdateSun(float normalizedTime)
        {
            float angle = normalizedTime * 360f;
            float clampedAngle = Mathf.Clamp(angle - 90f, -10f, 170f);
            sun.transform.rotation = Quaternion.Euler(clampedAngle, 0f, 0f);
        }
    }
}