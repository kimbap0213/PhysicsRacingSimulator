using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "EngineData", menuName = "RacingSimulation/EngineData", order = 1)]
    public class EngineData : ScriptableObject
    {
        [Header("Idler Circuit")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float idleThrottle;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float throttleStability;
        [SerializeField] private float idleRpm;
        
        public float IdleThrottle => idleThrottle;
        public float ThrottleStability => throttleStability;
        public float IdleRpm => idleRpm;

        [Header("Rev Limiter")]
        [SerializeField] private float redlineRpm;
        [SerializeField] private float throttleCutoffDuration;
        
        public float RedlineRpm => redlineRpm; 
        public float ThrottleCutoffDuration => throttleCutoffDuration;

        [Header("Engine Parameters")]
        [SerializeField] private AnimationCurve torqueCurve;
        [SerializeField] private float starterTorque;
        [SerializeField] private float initialFrictionTorque;
        [SerializeField] private float frictionLossCoefficient;
        [SerializeField] private float inertia;
        
        public AnimationCurve TorqueCurve => torqueCurve;
        public float StarterTorque => starterTorque;
        public float InitialFrictionTorque => initialFrictionTorque;
        public float FrictionLossCoefficient => frictionLossCoefficient;
        public float Inertia => inertia;
    }
}