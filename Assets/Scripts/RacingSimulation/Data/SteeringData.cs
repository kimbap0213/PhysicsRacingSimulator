using RacingSimulation.Define;
using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "SteeringData", menuName = "RacingSimulation/SteeringData", order = 1)]
    public class SteeringData : ScriptableObject
    {
        [Header("Inputs")]
        [SerializeField] private SteeringBehavior steeringBehavior;
        public SteeringBehavior SteeringBehavior => steeringBehavior;
        
        [Header("Dimensions")]
        [SerializeField] private float wheelbase; //Distance between front and rear wheels
        [SerializeField] private float rearTrackLength; //Distance between the left and right rear wheels
        [SerializeField] private float turningRadius; //Search up online or set to your preference to control max steering angle
        public float Wheelbase => wheelbase;
        public float RearTrackLength => rearTrackLength;
        public float TurningRadius => turningRadius;
    }
}