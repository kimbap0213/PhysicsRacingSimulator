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
        
        [Header("Inputs")]
        [SerializeField] private float throttleSensitivity;
        [SerializeField] private float clutchSensitivity;
        [SerializeField] private float steeringSensitivity;
        [SerializeField] private float finalDriveRatio;
        
        [Header("Dimensions")]
        [SerializeField] private float wheelbase;
        [SerializeField] private float rearTrackLength;
        [SerializeField] private float turningRadius;
    }
}