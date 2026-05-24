using UnityEngine;

namespace RacingSimulation.Utils
{
    public static class CalculateUtil
    {
        //Rad/s -> RPM
        public static float Rads2Rpm(float rads) => rads * (60.0f / (Mathf.PI * 2.0f));
        
        //RPM -> Rad/s
        public static float Rpm2Rads(float rpm) => rpm * ((Mathf.PI * 2.0f) / 60.0f);
        
        //Maps a value from one range to another
        public static float MapRangeClamped(float value, float inRangeA, float inRangeB, float outRangeA, float outRangeB)
            => Mathf.Lerp(outRangeA, outRangeB, Mathf.InverseLerp(inRangeA, inRangeB, value));
    }
}