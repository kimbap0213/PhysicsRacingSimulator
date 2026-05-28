using System.Collections;
using System.Windows.Forms;
using System.Xml;
using RacingSimulation.Core.Controllers;
using RacingSimulation.Data;
using RacingSimulation.Parsing;
using RacingSimulation.Runtime;
using UnityEngine;

namespace RacingSimulation.Core.Managers
{
    public class FlowManager : MonoBehaviour
    {
        [SerializeField] private UIController uiController;
        [SerializeField] private VehicleData vehicleData;
        [SerializeField] private Vehicle[] vehicles;
        [SerializeField] private Transform vehicleParent;
        private Vehicle _vehicle;
        
        private void Awake()
        {
            SetVehicleData(vehicleData);
            _vehicle.IsInitialized = true;
            
            uiController.AddLoadAction(OnButtonClicked);
            uiController.Init(_vehicle, _vehicle.GetComponent<Rigidbody>());
        }

        private void SetVehicleData(VehicleData data)
        {
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
            Debug.Log(data.WheelDatas[0].RestLength);
            
            _vehicle.Init(vehicleData);
        }

        private void OnButtonClicked()
        {
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
            uiController.Init(_vehicle, _vehicle.GetComponent<Rigidbody>());
        
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