using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "WheelData", menuName = "RacingSimulation/WheelData", order = 1)]
    public class WheelData : ScriptableObject
    {
        [Header("Hit Detection - Inputs")]
        [SerializeField] private LayerMask layerMask;

        [Header("Suspension - Inputs")]
        [SerializeField] private float restLength;
        [SerializeField] private float springStiffness;
        [SerializeField] private float damperStiffness;

        [Header("Wheel Motion - Inputs")]
        [SerializeField] private float wheelRadius;
        [SerializeField] private float wheelInertia;
        
        [Header("Friction - Outputs")]
        private float _lateralGrip;
        private float _longitudinalGrip;
    }
}