using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Clutch : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float clutchDamping;
        [SerializeField] private float clutchTorqueCapacity;
        [SerializeField] private float clutchStiffness;
        
        public float ClutchTorque { get; private set; }

        public void UpdatePhysics(float clutchInput, bool inGear, float engineVelocity, float transmissionVelocity)
        {
            float clutchEngagement = 1.0f - clutchInput;
            
            //Calculate slip
            float slip = inGear ? engineVelocity - transmissionVelocity : 0f;

            //Calculate torque
            float torque = clutchEngagement * slip * clutchStiffness; //tau = omega * k
            ClutchTorque += (torque - ClutchTorque) * clutchDamping; //Damping
            ClutchTorque = Mathf.Clamp(ClutchTorque, -clutchTorqueCapacity, clutchTorqueCapacity); //Make sure it doesn't exceed the torque capacity of the clutch
        }
    }
}