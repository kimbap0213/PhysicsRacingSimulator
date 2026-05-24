using RacingSimulation.Data;
using RacingSimulation.Runtime.Audio;
using RacingSimulation.Runtime.Physics;
using RacingSimulation.Runtime.Visual;
using RacingSimulation.Utils;
using UnityEngine;

namespace RacingSimulation.Runtime
{
    public class Vehicle : MonoBehaviour
    {
        private const int Substeps = 100; //(physics stable @ physics freq. * substeps = 5000+ Hz)

        [SerializeField] private Engine engine;
        [SerializeField] private Clutch clutch;
        [SerializeField] private Gearbox gearbox;
        [SerializeField] private Wheel[] wheels;
        [SerializeField] private Steering[] steerings;
        [SerializeField] private Visuals[] visuals;
        [SerializeField] private EngineAudio engineAudio;

        private VehicleData _data;
        private float _throttleInput;
        private float _clutchInput;
        private float _steeringInput;
        private float _starterInput;

        public void Init(VehicleData data)
        {
            _data = data;
            
            for (int i = 0; i < wheels.Length; i++)
            {
                steerings[i].Init(_data.SteeringDatas[i]);
                wheels[i].Init(_data.WheelDatas[i]);
                visuals[i].Init(wheels[i], steerings[i]);
            }

            clutch.Init(_data.ClutchData);
            engine.Init(_data.EngineData);
            gearbox.Init(_data.GearboxData);
            engineAudio.Init();
        }

        private void UpdateInputs()
        {
            float delta = Time.deltaTime;
            
            _throttleInput = Mathf.MoveTowards(_throttleInput, InputUtil.GetVerticalInput(), delta * _data.ThrottleSensitivity);
            _clutchInput = Mathf.MoveTowards(_clutchInput, InputUtil.GetKeyInput(KeyCode.X), delta * _data.ClutchSensitivity);
            _steeringInput = Mathf.MoveTowards(_steeringInput, InputUtil.GetHorizontalInput(), delta * _data.SteeringSensitivity);
            _starterInput = InputUtil.GetKeyInput(KeyCode.K);
        }

        private void Update()
        {
            //Update inputs
            UpdateInputs();
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                gearbox.ShiftUp();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                gearbox.ShiftDown();
            }
        }

        private void PreDrivetrainLoop(float delta)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].UpdatePhysicsPre(delta);
                steerings[i].UpdatePhysics(_steeringInput);
            }
        }

        private void DrivetrainLoop(float subDelta)
        {
            for (int i = 0; i < Substeps; i++)
            {
                engine.UpdatePhysics(subDelta, _throttleInput, _starterInput, clutch.ClutchTorque);
                clutch.UpdatePhysics(_clutchInput, gearbox.InGear, engine.AngularVelocity, gearbox.GetUpstreamAngularVelocity(Differential.GetUpstreamAngularVelocity(new Vector2(wheels[2].WheelAngularVelocity, wheels[3].WheelAngularVelocity), _data.FinalDriveRatio)));
                gearbox.UpdatePhysics();
                wheels[0].UpdatePhysicsDrivetrain(subDelta, 0.0f);
                wheels[1].UpdatePhysicsDrivetrain(subDelta, 0.0f);
                wheels[2].UpdatePhysicsDrivetrain(subDelta, Differential.GetDownstreamTorque(gearbox.GetDownstreamTorque(clutch.ClutchTorque), _data.FinalDriveRatio).x);
                wheels[3].UpdatePhysicsDrivetrain(subDelta, Differential.GetDownstreamTorque(gearbox.GetDownstreamTorque(clutch.ClutchTorque), _data.FinalDriveRatio).y);
            }
        }

        private void PostDrivetrainLoop()
        {
            foreach (var wheel in wheels)
            {
                wheel.UpdatePhysicsPost();
            }
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            float subDelta = delta / Substeps;
        
            //Pre-Drivetrain loop
            PreDrivetrainLoop(delta);
            
            //Drivetrain loop (RWD)
            DrivetrainLoop(subDelta);

            //Post-Drivetrain loop
            PostDrivetrainLoop();
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            
            //Update misc.
            foreach (var visual in visuals)
            {
                visual.UpdateVisuals(delta);
            }

            engineAudio.UpdateAudio(engine.EngineRpm, engine.Throttle);
        }
    }
}
