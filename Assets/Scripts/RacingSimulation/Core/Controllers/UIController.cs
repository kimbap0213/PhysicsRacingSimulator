using System;
using RacingSimulation.Runtime;
using TMPro;
using UnityEngine;

namespace RacingSimulation.Core.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Rigidbody vehicle;
        [SerializeField] private TextMeshProUGUI speedText;

        private void FixedUpdate()
        {
            speedText.text = $"Speed: {Mathf.RoundToInt(vehicle.linearVelocity.magnitude * 3.6f)} km/h";
        }
    }
}