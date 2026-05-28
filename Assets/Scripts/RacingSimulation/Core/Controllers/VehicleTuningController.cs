using System;
using RacingSimulation.Data;
using RacingSimulation.Runtime;
using UnityEngine;

namespace RacingSimulation.Core.Controllers
{
    public class VehicleTuningController : MonoBehaviour
    {
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private VehicleData vehicleData;
        
        private void Awake()
        {
            vehicle.Init(vehicleData);
            vehicle.IsInitialized = true;
        }
    }
}