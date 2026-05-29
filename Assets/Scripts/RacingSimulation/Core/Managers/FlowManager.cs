using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using RacingSimulation.Core.Controllers;
using RacingSimulation.Data;
using RacingSimulation.Parsing;
using RacingSimulation.Runtime;
using RacingSimulation.Runtime.Racing;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

namespace RacingSimulation.Core.Managers
{
    public class FlowManager : MonoBehaviour
    {
        [SerializeField] private UIController uiController;
        [SerializeField] private VehicleData vehicleData;
        [SerializeField] private Vehicle[] vehicles;
        [SerializeField] private Transform vehicleParent;
        [SerializeField] private FlagArea[] flagAreas;
        private Vehicle _vehicle;
        private readonly InputController _input = new();
        private bool _isInitialized = false;

        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                _isInitialized = value;
                uiController.SetActive(_isInitialized);
                InitFlagArea();
            }
        }
        
        private float _initializeTime = 0f;

        private void OnEnterFlag()
        {
            foreach (var flag in flagAreas)
            {
                if (!flag.IsActivated)
                    return;
            }
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - _initializeTime);
            uiController.ShowRecord(
                $"Record\n{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}");
        }

        private void InitFlagArea()
        {
            foreach (var flag in flagAreas)
            {
                flag.Initialize(OnEnterFlag);
            }
            
            _initializeTime = Time.time;
        }

        private void Awake()
        {
            IsInitialized = false;
            SetVehicleData(vehicleData);
            _vehicle.IsInitialized = true;
            uiController.AddLoadAction(OnButtonClicked);
            uiController.AddEvent(EventTriggerType.PointerDown, (data) => SetCursorLock(true));
            _input.AddKeyDownEvent(KeyCode.Escape, () => SetCursorLock(false));
        }

        private void SetCursorLock(bool isLock)
        {
            Cursor.visible = !isLock;
            Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Update()
        {
            if (!IsInitialized)
                return;
            
            _input.UpdateInput();
        }

        private void SetVehicleData(VehicleData data)
        {
            IsInitialized = false;
            if (_vehicle != null)
            {
                Destroy(_vehicle.gameObject);
            }
            
            if (data.WheelDatas[0].RestLength < 0.4f)
            {
                _vehicle = Instantiate(vehicles[0], vehicleParent);
            }
            else if (data.WheelDatas[0].RestLength < 0.6f)
            {
                _vehicle = Instantiate(vehicles[1], vehicleParent);
            }
            else
            {
                _vehicle = Instantiate(vehicles[2], vehicleParent);
            }
            
            uiController.Init(_vehicle, _vehicle.GetComponent<Rigidbody>(), () => _initializeTime);
            _vehicle.Init(vehicleData);
            _input.Init(_vehicle);
            _input.AddKeyDownEvent(KeyCode.Escape, () => SetCursorLock(false));
            uiController.StartCountDown(() => IsInitialized = true);
        }

        private void OnButtonClicked()
        {
            SetCursorLock(false);
            IsInitialized = false;
            _vehicle.IsInitialized = false;
        
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                UnityEngine.Application.Quit();
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(openFileDialog.FileName);
            VehicleData data = VehicleDataParser.ParseVehicleData(document);
            data.EngineData.SetTorqueCurve(vehicleData.EngineData.TorqueCurve);
            SetVehicleData(data);
        
            StartCoroutine(InitializeCoroutine());
        }

        private IEnumerator InitializeCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        
            _vehicle.IsInitialized = true;
        }
    }
}