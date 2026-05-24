using RacingSimulation.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace RacingSimulation.Runtime.Physics
{
    public class Wheel : MonoBehaviour
    {
        private float _deltaTime;
        [SerializeField] private Rigidbody vehicleBody;

        [Header("Hit Detection - Inputs")]
        [SerializeField] private LayerMask layerMask;

        [Header("Hit Detection - Outputs")] 
        private bool _isGrounded;
        private RaycastHit _hit;

        [Header("Suspension - Inputs")]
        [SerializeField] private float restLength;
        [SerializeField] private float springStiffness;
        [SerializeField] private float damperStiffness;

        [Header("Suspension - Outputs")] 
        public float CurrentLength { get; private set; }
        private Vector3 _suspensionForce;
        private float _lastLength;

        [Header("Wheel Motion - Inputs")]
        [SerializeField] private float wheelRadius;
        [SerializeField] private float wheelInertia;
        
        [Header("Wheel Motion - Outputs")]
        public float WheelAngularVelocity { get; private set; }

        private Vector3 _linearVelocityLocal;
        private Vector3 _angularVelocityLocal;
        private Vector3 _longitudinalDir;
        private Vector3 _lateralDir;
        
        [Header("Friction - Outputs")]
        private float _lateralGrip;
        private float _longitudinalGrip;

        public void UpdatePhysicsPre(float delta)
        {
            // throttleInput = Input.GetAxisRaw("Vertical"); //Temp; Make sure your vertical axis is defined in the input manager!
            _deltaTime = delta;
            
            //Fire a raycast to get the distance between the toplink and the ground
            _isGrounded = UnityEngine.Physics.Raycast(transform.position, -transform.up, out _hit, restLength + wheelRadius, layerMask);
            
            if (_isGrounded) //If we hit something,
            {
                //Calculate and apply the suspension force (Fz)
                CurrentLength = _hit.distance - wheelRadius;
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

        public void UpdatePhysicsDrivetrain(float delta, float driveTorque)
        {
            _deltaTime = delta;

            if (_isGrounded)
            {
                //Calculate the friction force (Fx, Fy)
                CalculateLateralFriction();
                CalculateLongitudinalFriction(driveTorque);
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
            float springDisplacement = restLength - CurrentLength;
            float springForce = springDisplacement * springStiffness;

            //Damping Equation
            float springVelocity = (_lastLength - CurrentLength) / _deltaTime;
            float damperForce = springVelocity * damperStiffness;

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
            _angularVelocityLocal = _linearVelocityLocal / wheelRadius; // omega = v / r

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

        private void CalculateLongitudinalFriction(float torque)
        {
            //Calculate Torque Acting On Wheel
            float frictionTorque = _longitudinalGrip * Mathf.Max(_suspensionForce.y, 0.0f) * wheelRadius;
            float totalTorque = torque - frictionTorque;

            //Integrate Angular Velocity
            float wheelAngularAcceleration = totalTorque / wheelInertia;
            WheelAngularVelocity += wheelAngularAcceleration * _deltaTime;

            //Calculate Wheel Slip (Longitduinal)
            //Pre-Pacejka: Hard-Coded Peak Slip Speed, Should Be Equal To The Peak In Pacejka Curve
            float slipSpeedPeak = 4.0f;
            float slipSpeed = WheelAngularVelocity - _angularVelocityLocal.z;

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
            _lastLength = CurrentLength = restLength; //Fully extend suspension
            _lateralGrip = _longitudinalGrip = 0.0f; //Set friction coefficients to zero
            _suspensionForce = Vector3.zero; //Set forces to zero
        }

        void GetWheelMotionInAir(float torque)
        {
            float wheelAngularAcceleration = torque / wheelInertia;
            WheelAngularVelocity += wheelAngularAcceleration * _deltaTime;
        }
    }
}
