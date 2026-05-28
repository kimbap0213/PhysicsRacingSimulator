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
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private UIController uiController;
        [SerializeField] private VehicleData vehicleData;
        
        private void Awake()
        {
            vehicle.Init(vehicleData);
            vehicle.IsInitialized = true;
            
            uiController.AddLoadAction(OnButtonClicked);
            uiController.Init(vehicle, vehicle.GetComponent<Rigidbody>());
        }

        private void OnButtonClicked()
        {
            vehicle.IsInitialized = false;
        
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
            vehicle.Init(data);
            uiController.Init(vehicle, vehicle.GetComponent<Rigidbody>());
        
            StartCoroutine(InitializeCoroutine());
        }

        private IEnumerator InitializeCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        
            vehicle.IsInitialized = true;
        }
    }
}