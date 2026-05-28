using RacingSimulation.Runtime;
using RacingSimulation.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace RacingSimulation.Core.Controllers
{
    public class InputController
    {
        private readonly UnityEvent _onUpdate = new();
        
        public void Init(Vehicle vehicle)
        {
            _onUpdate.AddListener(() => vehicle.SetThrottleInput(InputUtil.GetVerticalInput(), Time.deltaTime));
            _onUpdate.AddListener(() => vehicle.SetClutchInput(InputUtil.GetKeyInput(KeyCode.X), Time.deltaTime));
            _onUpdate.AddListener(() => vehicle.SetSteeringInput(InputUtil.GetHorizontalInput(), Time.deltaTime));
            _onUpdate.AddListener(() => vehicle.SetStarterInput(InputUtil.GetKeyInput(KeyCode.K)));
            _onUpdate.AddListener(() =>
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    vehicle.GearShiftUp();
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    vehicle.GearShiftDown();
                }
            });
        }
        
        public void UpdateInput()
        {
            _onUpdate.Invoke();
        }
    }
}