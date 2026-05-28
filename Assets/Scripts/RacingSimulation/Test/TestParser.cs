using UnityEngine;
using System.Windows.Forms;
using System.Xml;
using RacingSimulation.Data;
using RacingSimulation.Parsing;
using RacingSimulation.Runtime;

public class TestParser : MonoBehaviour
{
    [SerializeField] private Vehicle vehicle;

    private void Awake()
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

        vehicle.Init(data);
    }
}
