using UnityEngine;

namespace RacingSimulation.Utils
{
    public static class InputUtil
    {
        public static float GetVerticalInput() => Input.GetAxisRaw("Vertical");

        public static float GetHorizontalInput() => Input.GetAxisRaw("Horizontal");
        
        public static float GetKeyInput(KeyCode key) => Input.GetKey(key) ? 1f : 0f;
    }
}