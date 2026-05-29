using System;
using UnityEngine;

namespace RacingSimulation.Runtime.Racing
{
    public class FlagArea : MonoBehaviour
    {
        [SerializeField] private bool isActivatedOnInitialized;
        public bool IsActivated { get; private set; }
        private Action _onTriggerEnter;

        public void Initialize(Action onTriggerEnter)
        {
            IsActivated = isActivatedOnInitialized;
            _onTriggerEnter = onTriggerEnter;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            IsActivated = !IsActivated;
            _onTriggerEnter?.Invoke();
            Debug.Log($"Flag Area {gameObject.name} {IsActivated}");
        }
    }
}
