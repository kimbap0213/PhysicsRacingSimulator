using UnityEngine;

namespace RacingSimulation.Utils
{
    public static class InputUtil
    {
        public static float GetVerticalInput() => Mathf.Max(Input.GetAxisRaw("Vertical"), 0.0f);

        public static float GetHorizontalInput() => Input.GetAxisRaw("Horizontal");
        
        public static float GetKeyInput(KeyCode key) => Input.GetKey(key) ? 1f : 0f;
    }
}