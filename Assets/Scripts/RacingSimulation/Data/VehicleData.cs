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
        [SerializeField] private float brakeSensitivity;
        [SerializeField] private float clutchSensitivity;
        [SerializeField] private float steeringSensitivity;
        [SerializeField] private float finalDriveRatio;
        
        public float ThrottleSensitivity => throttleSensitivity;
        public float BrakeSensitivity => brakeSensitivity;
        public float ClutchSensitivity => clutchSensitivity;
        public float SteeringSensitivity => steeringSensitivity;
        public float FinalDriveRatio => finalDriveRatio;

        public void Initialize(EngineData engineData, ClutchData clutchData, GearboxData gearboxData,
            WheelData[] wheelDatas, SteeringData[] steeringDatas, float throttleSensitivity, float brakeSensitivity,
            float clutchSensitivity, float steeringSensitivity, float finalDriveRatio)

        {
            this.engineData = engineData;
            this.clutchData = clutchData;
            this.gearboxData = gearboxData;
            this.wheelDatas = wheelDatas;
            this.steeringDatas = steeringDatas;
            this.throttleSensitivity = throttleSensitivity;
            this.brakeSensitivity = brakeSensitivity;
            this.clutchSensitivity = clutchSensitivity;
            this.steeringSensitivity = steeringSensitivity;
            this.finalDriveRatio = finalDriveRatio;
        }
    }
}