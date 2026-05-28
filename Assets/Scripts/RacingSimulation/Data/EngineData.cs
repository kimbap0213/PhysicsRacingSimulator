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

        public void Init(float idleThrottle, float throttleStability, float idleRpm, float redlineRpm,
            float throttleCutoffDuration, float starterTorque, float initialFrictionTorque,
            float frictionLossCoefficient, float inertia)
        {
            this.idleThrottle = idleThrottle;
            this.throttleStability = throttleStability;
            this.idleRpm = idleRpm;
            this.redlineRpm = redlineRpm;
            this.throttleCutoffDuration = throttleCutoffDuration;
            this.starterTorque = starterTorque;
            this.initialFrictionTorque = initialFrictionTorque;
            this.frictionLossCoefficient = frictionLossCoefficient;
            this.inertia = inertia;
        }

        public void SetTorqueCurve(AnimationCurve torqueCurve)
        {
            this.torqueCurve = torqueCurve;
        }
    }
}