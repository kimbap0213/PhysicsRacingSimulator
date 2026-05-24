using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "VehicleData", menuName = "RacingSimulation/VehicleData", order = 1)]
    public class VehicleData : ScriptableObject
    {
        [SerializeField] private EngineData engineData;
        [SerializeField] private ClutchData clutchData;
        [SerializeField] private GearboxData gearboxData;
        [SerializeField] private WheelData[] wheelDatas;
        [SerializeField] private SteeringData[] steeringDatas;
        
        public EngineData EngineData => engineData;
        public ClutchData ClutchData => clutchData;
        public GearboxData GearboxData => gearboxData;
        public WheelData[] WheelDatas => wheelDatas;
        public SteeringData[] SteeringDatas => steeringDatas;
        
        [Header("Inputs")]
        [SerializeField] private float throttleSensitivity;
        [SerializeField] private float clutchSensitivity;
        [SerializeField] private float steeringSensitivity;
        [SerializeField] private float finalDriveRatio;
        
        public float ThrottleSensitivity => throttleSensitivity;
        public float ClutchSensitivity => clutchSensitivity;
        public float SteeringSensitivity => steeringSensitivity;
        public float FinalDriveRatio => finalDriveRatio;
    }
}