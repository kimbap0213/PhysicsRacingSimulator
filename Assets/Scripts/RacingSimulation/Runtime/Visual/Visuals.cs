using RacingSimulation.Runtime.Physics;
using UnityEngine;

namespace RacingSimulation.Runtime.Visual
{
    public class Visuals : MonoBehaviour
    {
        private Wheel _wheel;
        private Steering _steering;
        private float _wheelRot;

        public void Init(Wheel wheel, Steering steering)
        {
            _wheel = wheel;
            _steering = steering;
        }

        public void UpdateVisuals(float delta)
        {
            //Lateral = Wheel Spacers (X); Vertical = Suspension Motion (Y); Longitudinal = Unused (Z)
            transform.localPosition = new Vector3(transform.localPosition.x, _wheel.transform.localPosition.y - _wheel.CurrentLength, _wheel.transform.localPosition.z);

            //Integrate Wheel Rotation
            _wheelRot += _wheel.WheelAngularVelocity * Mathf.Rad2Deg * delta;
            if (Mathf.Abs(_wheelRot) > 360.0f) //Prevent from reaching absurd values
            {
                _wheelRot -= 360.0f * Mathf.Sign(_wheelRot);
            }

            //Roll = Tire Roll (X); Yaw = Steering (Y); Pitch = Camber (Z) NOTE: If you want to have non-zero camber, place an empty at the wheel's origin, parent the wheel to it, and rotate the empty on the Z
            transform.localRotation = Quaternion.Euler(new Vector3(_wheelRot, _steering.SteerAngle, 0.0f)); //IMPORTANT: 0.0 fixes gimbal lock
        }
    }
}
