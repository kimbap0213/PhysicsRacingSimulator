using System.Collections;
using RacingSimulation.Data;
using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Gearbox : MonoBehaviour
    {
        public bool InGear { get; private set; }

        private GearboxData _data;
        private int _currentGear;
        private float _currentGearRatio;
        private Coroutine _currentShiftCoroutine = null;

        public void Init(GearboxData data)
        {
            _data = data;
            InGear = false;
            _currentGear = 1;
        }

        public void UpdatePhysics()
        {
            //Geartrain
            _currentGearRatio = InGear ? _data.GearRatios[_currentGear] : 0.0f;
        }
    
        public float GetDownstreamTorque(float torque) //Uncomment once drivetrain is complete
        {
            return torque * _currentGearRatio;
        }

        public float GetUpstreamAngularVelocity(float angularVelocity) //Uncomment once drivetrain is complete
        {
            return angularVelocity * _currentGearRatio;
        }

        public void ShiftUp()
        {
            //If not currently in top gear,
            if (_currentShiftCoroutine != null || _currentGear >= _data.MaxGear)
                return;

            _currentShiftCoroutine = StartCoroutine(ShiftUpCoroutine());
        }
        
        public void ShiftDown()
        {
            //If not currently in bottom gear,
            if (_currentShiftCoroutine != null || _currentGear <= 0)
                return;
            
            _currentShiftCoroutine = StartCoroutine(ShiftDownCoroutine());
        }

        private IEnumerator ShiftUpCoroutine()
        {
            //Shift to neutral,
            InGear = false;
            int nextGear = _currentGear + 1;
            _currentGear = 1;

            //Wait,
            yield return new WaitForSeconds(_data.ShiftDuration);

            //Shift up
            _currentGear = nextGear;
            InGear = true;
                
            _currentShiftCoroutine = null;
        }

        private IEnumerator ShiftDownCoroutine()
        {
            //Shift to neutral,
            InGear = false;
            int nextGear = _currentGear - 1;
            _currentGear = 1;

            //Wait,
            yield return new WaitForSeconds(_data.ShiftDuration);

            //Shift down
            _currentGear = nextGear;
            InGear = true;

            _currentShiftCoroutine = null;
        }
    }
}
