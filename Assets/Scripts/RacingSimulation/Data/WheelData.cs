using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "WheelData", menuName = "RacingSimulation/WheelData", order = 1)]
    public class WheelData : ScriptableObject
    {
        [Header("Hit Detection - Inputs")]
        [SerializeField] private LayerMask layerMask;
        public LayerMask LayerMask => layerMask;

        [Header("Suspension - Inputs")]
        [SerializeField] private float restLength;
        [SerializeField] private float springStiffness;
        [SerializeField] private float damperStiffness;
        public float RestLength => restLength;
        public float SpringStiffness => springStiffness;
        public float DamperStiffness => damperStiffness;

        [Header("Wheel Motion - Inputs")]
        [SerializeField] private float wheelRadius;
        [SerializeField] private float wheelInertia;
        [SerializeField] private float maxBrakeTorque;
        public float WheelRadius => wheelRadius;
        public float WheelInertia => wheelInertia;
        public float MaxBrakeTorque => maxBrakeTorque;

        public void Initialize(LayerMask layerMask, float restLength, float springStiffness, float damperStiffness,
            float wheelRadius, float wheelInertia, float maxBrakeTorque)
        {
            this.layerMask = layerMask;
            this.restLength = restLength;
            this.springStiffness = springStiffness;
            this.damperStiffness = damperStiffness;
            this.wheelRadius = wheelRadius;
            this.wheelInertia = wheelInertia;
            this.maxBrakeTorque = maxBrakeTorque;
        }
    }
}