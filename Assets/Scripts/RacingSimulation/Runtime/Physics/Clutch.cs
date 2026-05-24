using RacingSimulation.Data;
using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Clutch : MonoBehaviour
    {
        public float ClutchTorque { get; private set; }
        
        private ClutchData _data;
        private float _clutchDamping;

        public void Init(ClutchData data)
        {
            _data = data;
            _clutchDamping = _data.ClutchDamping;
        }

        public void UpdatePhysics(float clutchInput, bool inGear, float engineVelocity, float transmissionVelocity)
        {
            float clutchEngagement = 1.0f - clutchInput;
            
            //Calculate slip
            float slip = inGear ? engineVelocity - transmissionVelocity : 0f;

            //Calculate torque
            float torque = clutchEngagement * slip * _data.ClutchStiffness; //tau = omega * k
            ClutchTorque += (torque - ClutchTorque) * _clutchDamping; //Damping
            ClutchTorque = Mathf.Clamp(ClutchTorque, -_data.ClutchTorqueCapacity, _data.ClutchTorqueCapacity); //Make sure it doesn't exceed the torque capacity of the clutch
        }
    }
}