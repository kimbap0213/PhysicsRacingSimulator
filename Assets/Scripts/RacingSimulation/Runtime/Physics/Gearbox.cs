using System.Collections;
using UnityEngine;

namespace RacingSimulation.Runtime.Physics
{
    public class Gearbox : MonoBehaviour
    {
        [SerializeField] private float[] gearRatios;
        [SerializeField] private float shiftDuration;

        public bool InGear { get; private set; }
        
        private int _currentGear;
        private float _currentGearRatio;
        private Coroutine _currentShiftCoroutine = null;

        public void Init()
        {
            InGear = false;
            _currentGear = 1;
        }

        public void UpdatePhysics()
        {
            //Geartrain
            _currentGearRatio = InGear ? gearRatios[_currentGear] : 0.0f;
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
            if (_currentShiftCoroutine != null || _currentGear >= gearRatios.Length - 1)
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
            yield return new WaitForSeconds(shiftDuration);

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
            yield return new WaitForSeconds(shiftDuration);

            //Shift down
            _currentGear = nextGear;
            InGear = true;

            _currentShiftCoroutine = null;
        }
    }
}
