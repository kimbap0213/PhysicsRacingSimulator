using System.Collections;
using RacingSimulation.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace RacingSimulation.Runtime.Physics
{
    public class Engine : MonoBehaviour
    {
        [Header("Idler Circuit")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float idleThrottle;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float throttleStability;
        [SerializeField] private float idleRpm;

        [Header("Rev Limiter")]
        [SerializeField] private float redlineRpm;
        [SerializeField] private float throttleCutoffDuration;

        [Header("Engine Parameters")]
        [SerializeField] private AnimationCurve torqueCurve;
        [SerializeField] private float starterTorque;
        [SerializeField] private float initialFrictionTorque;
        [SerializeField] private float frictionLossCoefficient;
        [SerializeField] private float inertia;

        [Header("Outputs")]
        public float AngularVelocity { get; private set; }
        public float EngineRpm { get; private set; }
        
        public float Throttle { get; private set; }
        private bool _throttleCut;
        private float _instability;
        private float _timer;

        public void Init()
        {
            //Set engine to idle
            AngularVelocity = CalculateUtil.Rpm2Rads(idleRpm);
            EngineRpm = CalculateUtil.Rads2Rpm(AngularVelocity);
        }

        public void UpdatePhysics(float delta, float throttleInput, float starterInput, float loadTorque)
        {
            //Idler Circuit
            float idler = CalculateUtil.MapRangeClamped(EngineRpm, idleRpm - 200.0f, idleRpm + 200.0f, idleThrottle, 0.0f);
            _timer += delta;
            
            if (_timer >= 0.1f) //Update instability every tenth of a second
            {
                _timer = 0.0f;
                _instability = Random.Range(throttleStability, 1.0f);
            }
            
            idler *= _instability; //Add throttle instability

            //Rev Limiter
            if (EngineRpm >= redlineRpm && !_throttleCut)
            {
                StartCoroutine(ThrottleCutoff());
            }

            //Combine player input, idler circuit, and rev limiter into final throttle value
            if (!_throttleCut)
            {
                Throttle = Mathf.MoveTowards(Throttle, Mathf.Max(throttleInput, idler), delta * 10.0f);
            }
            else
            {
                Throttle = Mathf.MoveTowards(Throttle, 0.0f, delta * 10.0f);
            }

            //Calculate engine torque
            float startingTorque = starterInput * starterTorque;
            float grossTorque = torqueCurve.Evaluate(EngineRpm) * Throttle; //Evaluate the engine's current gross torque output based off the current RPM and throttle input
            float frictionLosses = Mathf.Min(Mathf.Abs(initialFrictionTorque + (EngineRpm * frictionLossCoefficient)), Mathf.Abs((AngularVelocity / delta) * inertia)) * Mathf.Sign(AngularVelocity); //loss = constant + (linear * RPM)
            float netTorque = (grossTorque + startingTorque) - frictionLosses - loadTorque;

            //Integrate engine speed
            float angularAcceleration = netTorque / inertia; //Newton's 2nd law of motion
            AngularVelocity += angularAcceleration * delta; //Newton's 1st equation of motion
            EngineRpm = CalculateUtil.Rads2Rpm(AngularVelocity);
        }

        private IEnumerator ThrottleCutoff()
        {
            _throttleCut = true;
            yield return new WaitForSeconds(throttleCutoffDuration);
            _throttleCut = false;
        }
    }
}