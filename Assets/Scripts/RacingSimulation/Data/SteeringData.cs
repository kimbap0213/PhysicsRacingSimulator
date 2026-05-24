using RacingSimulation.Define;
using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "SteeringData", menuName = "RacingSimulation/SteeringData", order = 1)]
    public class SteeringData : ScriptableObject
    {
        [Header("Inputs")]
        [SerializeField] private SteeringBehavior steeringBehavior;
    }
}