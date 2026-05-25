using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "GearboxData", menuName = "RacingSimulation/GearboxData", order = 1)]
    public class GearboxData : ScriptableObject
    {
        [SerializeField] private float[] gearRatios;
        [SerializeField] private float shiftDuration;
        
        public float[] GearRatios => gearRatios;
        public float ShiftDuration => shiftDuration;
        public int MaxGear => GearRatios.Length - 1;

        public void Initialize(float[] gearRatios, float shiftDuration)
        {
            this.gearRatios = gearRatios;
            this.shiftDuration = shiftDuration;
        }
    }
}