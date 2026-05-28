using System;
using UnityEngine;

namespace RacingSimulation.Data
{
    [CreateAssetMenu(fileName = "ClutchData", menuName = "RacingSimulation/ClutchData", order = 1)]
    public class ClutchData : ScriptableObject
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float clutchDamping;
        [SerializeField] private float clutchTorqueCapacity;
        [SerializeField] private float clutchStiffness;

        public float ClutchDamping => clutchDamping;
        public float ClutchTorqueCapacity => clutchTorqueCapacity;
        public float ClutchStiffness => clutchStiffness;

        public void Initialize(float clutchDamping, float clutchTorqueCapacity, float clutchStiffness)
        {
            this.clutchDamping = clutchDamping;
            this.clutchTorqueCapacity = clutchTorqueCapacity;
            this.clutchStiffness = clutchStiffness;
        }

        internal void Initialize(object maxTorque, object engagementPoint, object disengagementPoint)
        {
            throw new NotImplementedException();
        }
    }
}