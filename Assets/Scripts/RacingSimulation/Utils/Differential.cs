using UnityEngine;

namespace RacingSimulation.Utils
{
    public static class Differential
    {
        public static Vector2 GetDownstreamTorque(float torque, float driveRatio) //Open Differential; Uncomment once drivetrain is complete
        {
            return new Vector2(torque * driveRatio * 0.5f, torque * driveRatio * 0.5f);
        }

        public static float GetUpstreamAngularVelocity(Vector2 angularVelocity, float driveRatio) //Uncomment once drivetrain is complete
        {
            return (angularVelocity.x + angularVelocity.y) * driveRatio * 0.5f;
        }
    }
}
