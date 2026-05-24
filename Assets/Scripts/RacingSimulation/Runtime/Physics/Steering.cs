using RacingSimulation.Define;
using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Steering : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private SteeringBehavior steeringBehavior;

        [Header("Outputs")]
        public float SteerAngle { get; private set; }
        
        private float _wheelbase; //Distance between front and rear wheels
        private float _rearTrackLength; //Distance between the left and right rear wheels
        private float _turningRadius; //Search up online or set to your preference to control max steering angle

        public void Init(float wheelBase, float rearTrackLength, float turningRadius)
        {
            _wheelbase = wheelBase;
            _rearTrackLength = rearTrackLength;
            _turningRadius = turningRadius;
        }

        public void UpdatePhysics(float steeringInput)
        {
            //Calulate Steering Input
            // steeringInput = Mathf.MoveTowards(steeringInput, input, Time.fixedDeltaTime * steeringSpeed); //Smooth out the raw input data

            //Ackermann Equations
            float inner = Mathf.Atan(_wheelbase / (_turningRadius + (_rearTrackLength / 2.0f))) * Mathf.Rad2Deg * steeringInput;
            float outer = Mathf.Atan(_wheelbase / (_turningRadius - (_rearTrackLength / 2.0f))) * Mathf.Rad2Deg * steeringInput;

            SteerAngle = 0.0f;
            if (steeringBehavior != SteeringBehavior.Disabled)
            {
                if (steeringInput > 0.0f) //Turning Right
                {
                    if (steeringBehavior == SteeringBehavior.Left) //Left wheel
                    {
                        SteerAngle = inner;
                    }
                    if (steeringBehavior == SteeringBehavior.Right) //Right wheel
                    {
                        SteerAngle = outer;
                    }
                }
                if (steeringInput < 0.0f) //Turning Left
                {
                    if (steeringBehavior == SteeringBehavior.Left) //Left wheel
                    {
                        SteerAngle = outer;
                    }
                    if (steeringBehavior == SteeringBehavior.Right) //Right wheel
                    {
                        SteerAngle = inner;
                    }
                }
            }

            // IMPORTANT FOR LATERAL FRICTION!! Set the toplink's rotation accordingly; Allows there to be a lateral velocity at the contact patch
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, SteerAngle, transform.localEulerAngles.z));
        }
    }
}
