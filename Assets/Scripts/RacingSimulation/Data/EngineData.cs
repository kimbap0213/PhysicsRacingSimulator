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

        [Header("Rev Limiter")]
        [SerializeField] private float redlineRpm;
        [SerializeField] private float throttleCutoffDuration;

        [Header("Engine Parameters")]
        [SerializeField] private AnimationCurve torqueCurve;
        [SerializeField] private float starterTorque;
        [SerializeField] private float initialFrictionTorque;
        [SerializeField] private float frictionLossCoefficient;
        [SerializeField] private float inertia;
    }
}