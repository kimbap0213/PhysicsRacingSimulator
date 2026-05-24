using RacingSimulation.Data;
using RacingSimulation.Define;
using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Steering : MonoBehaviour
    {
        public float SteerAngle { get; private set; }

        private SteeringData _data;

        public void Init(SteeringData data)
        {
            _data = data;
        }

        public void UpdatePhysics(float steeringInput)
        {
            //Calulate Steering Input
            // steeringInput = Mathf.MoveTowards(steeringInput, input, Time.fixedDeltaTime * steeringSpeed); //Smooth out the raw input data

            //Ackermann Equations
            float inner = Mathf.Atan(_data.Wheelbase / (_data.TurningRadius + (_data.RearTrackLength / 2.0f))) * Mathf.Rad2Deg * steeringInput;
            float outer = Mathf.Atan(_data.Wheelbase / (_data.TurningRadius - (_data.RearTrackLength / 2.0f))) * Mathf.Rad2Deg * steeringInput;

            SteerAngle = 0.0f;
            if (_data.SteeringBehavior != SteeringBehavior.Disabled)
            {
                if (steeringInput > 0.0f) //Turning Right
                {
                    if (_data.SteeringBehavior == SteeringBehavior.Left) //Left wheel
                    {
                        SteerAngle = inner;
                    }
                    if (_data.SteeringBehavior == SteeringBehavior.Right) //Right wheel
                    {
                        SteerAngle = outer;
                    }
                }
                if (steeringInput < 0.0f) //Turning Left
                {
                    if (_data.SteeringBehavior == SteeringBehavior.Left) //Left wheel
                    {
                        SteerAngle = outer;
                    }
                    if (_data.SteeringBehavior == SteeringBehavior.Right) //Right wheel
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
