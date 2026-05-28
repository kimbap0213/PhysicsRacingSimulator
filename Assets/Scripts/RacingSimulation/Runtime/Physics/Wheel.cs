using RacingSimulation.Data;
using RacingSimulation.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace RacingSimulation.Runtime.Physics
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private Rigidbody vehicleBody;
        public float CurrentLength { get; private set; }
        public float WheelAngularVelocity { get; private set; }

        private WheelData _data;
        private float _deltaTime;

        private bool _isGrounded;
        private RaycastHit _hit;
        
        private Vector3 _suspensionForce;
        private float _lastLength;

        private Vector3 _linearVelocityLocal;
        private Vector3 _angularVelocityLocal;
        private Vector3 _longitudinalDir;
        private Vector3 _lateralDir;
        
        private float _lateralGrip;
        private float _longitudinalGrip;

        public void Init(WheelData data)
        {
            _data = data;
            CurrentLength = 0f;
            WheelAngularVelocity = 0f;
            _deltaTime = 0f;
            _isGrounded = false;
            _hit = new RaycastHit();
            _suspensionForce = Vector3.zero;
            _lastLength = 0f;
            _linearVelocityLocal = Vector3.zero;
            _angularVelocityLocal = Vector3.zero;
            _longitudinalDir = Vector3.zero;
            _lateralDir = Vector3.zero;
            _lateralGrip = 0f;
        }

        public void UpdatePhysicsPre(float delta)
        {
            // throttleInput = Input.GetAxisRaw("Vertical"); //Temp; Make sure your vertical axis is defined in the input manager!
            _deltaTime = delta;
            
            //Fire a raycast to get the distance between the toplink and the ground
            _isGrounded = UnityEngine.Physics.Raycast(transform.position, -transform.up, out _hit, _data.RestLength + _data.WheelRadius, _data.LayerMask);
            
            if (_isGrounded) //If we hit something,
            {
                //Calculate and apply the suspension force (Fz)
                CurrentLength = _hit.distance - _data.WheelRadius;
                CalculateSuspensionForce();
                ApplySuspensionForce();

                //Calculate the wheel's velocity and direction vectors
                GetWheelMotionOnGround();
            }
            else //If we don't,
            {
                //Reset values that need resetting
                ResetValues();
            }
        }

        public void UpdatePhysicsDrivetrain(float delta, float driveTorque, float brakeFriction)
        {
            _deltaTime = delta;

            if (_isGrounded)
            {
                //Calculate the friction force (Fx, Fy)
                CalculateLateralFriction();
                CalculateLongitudinalFriction(driveTorque, brakeFriction);
            }
            else
            {
                //Keep the wheel's ability to spin
                GetWheelMotionInAir(driveTorque);
            }
        }

        public void UpdatePhysicsPost()
        { 
            if (_isGrounded)
            {
                //Apply the friction force (Fx, Fy)
                ApplyFrictionForce();
            }
        }

        void CalculateSuspensionForce()
        {
            //Hooke's Law
            float springDisplacement = _data.RestLength - CurrentLength;
            float springForce = springDisplacement * _data.SpringStiffness;

            //Damping Equation
            float springVelocity = (_lastLength - CurrentLength) / _deltaTime;
            float damperForce = springVelocity * _data.DamperStiffness;

            float suspensionForce = springForce + damperForce;
            _suspensionForce = _hit.normal.normalized * suspensionForce; //Suspension force acts perpendicular to the contact patch

            _lastLength = CurrentLength; //Set the lastLength for the next frame
        }
        
        private void ApplySuspensionForce()
        {
            vehicleBody.AddForceAtPosition(_suspensionForce, transform.position); //Apply the suspension force to the vehicle at the toplink position
        }

        private void GetWheelMotionOnGround()
        {
            //Get the velocity of the wheel relative to the ground
            _linearVelocityLocal = transform.InverseTransformDirection(vehicleBody.GetPointVelocity(_hit.point)); //RB.GetPointVelocity Does Not Update w/ Substeps, If There's A Way To Get This Value Without The Use Of RB Functions, We Can Substep The Whole VP Implementation And Keep The Timestep @ 0.02
            _angularVelocityLocal = _linearVelocityLocal / _data.WheelRadius; // omega = v / r

            //Lateral and longitudinal directions of motion of the wheel
            _longitudinalDir = Vector3.ProjectOnPlane(transform.forward, _hit.normal).normalized;
            _lateralDir = Vector3.ProjectOnPlane(transform.right, _hit.normal).normalized;
        }

        private void CalculateLateralFriction()
        {
            //Calculate Wheel Slip (Lateral)
            float slipAnglePeak = 8.0f; //Pre-Pacejka: Hard-Coded Peak Slip Angle, Should Be Equal To The Peak In Pacejka Curve
            float lowSpeedSlipAngle = slipAnglePeak * Mathf.Sign(-_linearVelocityLocal.x) * Mathf.Clamp01(Mathf.Abs(_linearVelocityLocal.x)); //Ramp function that mimics slip angle formula at low speeds
            float highSpeedSlipAngle = 0.0f;
            if (_linearVelocityLocal.z != 0.0f)
            {
                highSpeedSlipAngle = Mathf.Atan(-_linearVelocityLocal.x / Mathf.Abs(_linearVelocityLocal.z)) * Mathf.Rad2Deg;
            }
            
            //Transition Between Low And High Speed Friction Models Based Off Of Wheel Speed
            float slipAngle = Mathf.Lerp(lowSpeedSlipAngle, highSpeedSlipAngle, CalculateUtil.MapRangeClamped(_linearVelocityLocal.magnitude, 3.0f, 6.0f, 0.0f, 1.0f));
            //Map Wheel Slip To Friction Curve
            _lateralGrip = CalculateUtil.MapRangeClamped(Mathf.Abs(slipAngle), 0.0f, slipAnglePeak, 0.0f, 1.0f) * Mathf.Sign(slipAngle); //Pre-Pacejka
        }

        private void CalculateLongitudinalFriction(float torque, float brakeFriction)
        {
            //Calculate Torque Acting On Wheel
            float frictionTorque = _longitudinalGrip * Mathf.Max(_suspensionForce.y, 0.0f) * _data.WheelRadius;
            float brakeTorque = Mathf.Abs(WheelAngularVelocity) > 0.01f
                ? brakeFriction * _data.MaxBrakeTorque * Mathf.Sign(WheelAngularVelocity)
                : 0.0f;
            float totalTorque = torque - frictionTorque - brakeTorque;

            //Integrate Angular Velocity
            float wheelAngularAcceleration = totalTorque / _data.WheelInertia;
            WheelAngularVelocity += wheelAngularAcceleration * _deltaTime;

            //Calculate Wheel Slip (Longitduinal)
            //Pre-Pacejka: Hard-Coded Peak Slip Speed, Should Be Equal To The Peak In Pacejka Curve
            float slipSpeedPeak = 4.0f;
            float slipSpeed = WheelAngularVelocity - _angularVelocityLocal.z;

            // Debug.Log($"Slip Speed: {slipSpeed}, Wheel: {WheelAngularVelocity}, Torque: {totalTorque}");
            //Map Wheel Slip To Friction Curve
            _longitudinalGrip = CalculateUtil.MapRangeClamped(Mathf.Abs(slipSpeed), 0.0f, slipSpeedPeak, 0.0f, 1.0f) * Mathf.Sign(slipSpeed); //Pre-Pacejka
        }

        void ApplyFrictionForce()
        {
            Vector3 lateralFrictionForce = _lateralDir * (_lateralGrip * Mathf.Max(_suspensionForce.y, 0.0f)); //F_lat = u * N * -latDir
            Vector3 longitudinalFrictionForce = _longitudinalDir * (_longitudinalGrip * Mathf.Max(_suspensionForce.y, 0.0f)); // F_long = u * N * -longDir
            vehicleBody.AddForceAtPosition(lateralFrictionForce + longitudinalFrictionForce, _hit.point); //Apply the friction force at the wheel's contact patch
        }

        void ResetValues()
        {
            _lastLength = CurrentLength = _data.RestLength; //Fully extend suspension
            _lateralGrip = _longitudinalGrip = 0.0f; //Set friction coefficients to zero
            _suspensionForce = Vector3.zero; //Set forces to zero
        }

        void GetWheelMotionInAir(float torque)
        {
            float wheelAngularAcceleration = torque / _data.WheelInertia;
            WheelAngularVelocity += wheelAngularAcceleration * _deltaTime;
        }
    }
}
