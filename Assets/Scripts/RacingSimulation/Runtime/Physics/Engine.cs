using System.Collections;
using RacingSimulation.Data;
using RacingSimulation.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace RacingSimulation.Runtime.Physics
{
    public class Engine : MonoBehaviour
    {
        // Output
        public float AngularVelocity { get; private set; }
        public float EngineRpm { get; private set; }
        public float Throttle { get; private set; }

        private EngineData _data;
        private bool _throttleCut;
        private float _instability;
        private float _timer;

        public void Init(EngineData data)
        {
            _data = data;
            
            //Set engine to idle
            AngularVelocity = CalculateUtil.Rpm2Rads(_data.IdleRpm);
            EngineRpm = CalculateUtil.Rads2Rpm(AngularVelocity);
            Throttle = 0.0f;
            _throttleCut = false;
            _instability = 0.0f;
            _timer = 0.0f;            
        }

        public void UpdatePhysics(float delta, float throttleInput, float starterInput, float loadTorque)
        {
            //Idler Circuit
            float idler = CalculateUtil.MapRangeClamped(EngineRpm, _data.IdleRpm - 200.0f, _data.IdleRpm + 200.0f, _data.IdleThrottle, 0.0f);
            _timer += delta;
            
            if (_timer >= 0.1f) //Update instability every tenth of a second
            {
                _timer = 0.0f;
                _instability = Random.Range(_data.ThrottleStability, 1.0f);
            }
            
            idler *= _instability; //Add throttle instability

            //Rev Limiter
            if (EngineRpm >= _data.RedlineRpm && !_throttleCut)
            {
                Debug.Log(EngineRpm + " RPM - "+ _data.RedlineRpm+" Throttle Cutoff Engaged!");
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
            float startingTorque = starterInput * _data.StarterTorque;
            float grossTorque = _data.TorqueCurve.Evaluate(EngineRpm) * Throttle; //Evaluate the engine's current gross torque output based off the current RPM and throttle input
            float frictionLosses = Mathf.Min(Mathf.Abs(_data.InitialFrictionTorque + (EngineRpm * _data.FrictionLossCoefficient)), Mathf.Abs((AngularVelocity / delta) * _data.Inertia)) * Mathf.Sign(AngularVelocity); //loss = constant + (linear * RPM)
            float netTorque = (grossTorque + startingTorque) - frictionLosses - loadTorque;

            //Integrate engine speed
            float angularAcceleration = netTorque / _data.Inertia; //Newton's 2nd law of motion
            AngularVelocity += angularAcceleration * delta; //Newton's 1st equation of motion
            EngineRpm = CalculateUtil.Rads2Rpm(AngularVelocity);
        }

        private IEnumerator ThrottleCutoff()
        {
            _throttleCut = true;
            yield return new WaitForSeconds(_data.ThrottleCutoffDuration);
            _throttleCut = false;
        }
    }
}