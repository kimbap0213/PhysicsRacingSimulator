using UnityEngine;
using System.Windows.Forms;
using System.Xml;
using RacingSimulation.Data;
using RacingSimulation.Parsing;
using RacingSimulation.Runtime;
using RacingSimulation.Core.Controllers;

public class TestParser : MonoBehaviour
{
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private UIController uiController;
    [SerializeField] private EngineData engineData;

    private void Awake()
    {
        uiController.AddLoadAction(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
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
        data.EngineData.SetTorqueCurve(engineData.TorqueCurve);
        vehicle.Init(data);
    }
}
