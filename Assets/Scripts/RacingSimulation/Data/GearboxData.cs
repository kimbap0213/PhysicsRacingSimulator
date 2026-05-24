using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "GearboxData", menuName = "RacingSimulation/GearboxData", order = 1)]
    public class GearboxData : ScriptableObject
    {
        [SerializeField] private float[] gearRatios;
        [SerializeField] private float shiftDuration;
    }
}