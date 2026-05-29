using System;
using System.Collections;
using RacingSimulation.Runtime;
using RacingSimulation.Runtime.Physics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RacingSimulation.Core.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Transform inGameTextPivot;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI throttleText;
        [SerializeField] private TextMeshProUGUI brakeText;
        [SerializeField] private TextMeshProUGUI clutchText;
        [SerializeField] private TextMeshProUGUI steeringText;
        [SerializeField] private TextMeshProUGUI starterText;
        [SerializeField] private TextMeshProUGUI gearboxText;
        [SerializeField] private Button loadButton;
        [SerializeField] private TextMeshProUGUI countDownText;

        private Action _onFixedUpdate;
        private float _startTime;
        private Coroutine _countDownCoroutine = null;

        public void StartCountDown(Action onCountDownEnd = null)
        {
            if (_countDownCoroutine != null)
                return;
            
            _countDownCoroutine = StartCoroutine(CountDownCoroutine(onCountDownEnd));
        }

        private IEnumerator CountDownCoroutine(Action onCountDownEnd)
        {
            float elapsed = 0f;
            do
            {
                float factor = Mathf.SmoothStep(1f, 0f, (Mathf.Pow((0.5f - elapsed % 1f) * 2f, 5f) + 1f) / 2f);
                countDownText.text = Mathf.CeilToInt(3f - elapsed).ToString();
                countDownText.rectTransform.anchoredPosition = Vector3.Lerp(new Vector2(-100f, 0f), new Vector2(100f, 0f), factor);
                countDownText.color = Color.Lerp(Color.clear, Color.white, Mathf.PingPong(factor * 2f, 1f));
                elapsed += Time.deltaTime;
                yield return null;
            } while (elapsed < 3f);
            
            countDownText.gameObject.SetActive(false);
            _countDownCoroutine = null;
            onCountDownEnd?.Invoke();
        }

        public void Init(Vehicle vehicle, Rigidbody rb)
        {
            _onFixedUpdate = () => OnFixedUpdate(vehicle, rb);
        }

        private void OnFixedUpdate(Vehicle vehicle, Rigidbody rb)
        {
            int count = 0;
            count = SetSpeed(Mathf.RoundToInt(rb.linearVelocity.magnitude * 3.6f), Mathf.RoundToInt(vehicle.EngineRpm), count);
            count = SetGearbox(vehicle.CurrentGear, count);
            count = SetThrottle(vehicle.ThrottleInput, count);
            count = SetBrake(vehicle.BreakInput, count);
            count = SetClutch(vehicle.ClutchInput, count);
            count = SetSteering(vehicle.SteeringInput, count);
            count = SetStarter(vehicle.StarterInput, count);
            SetButtonLocation(count);
        }

        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        public void AddLoadAction(Action action)
        {
            loadButton.onClick.AddListener(() => action?.Invoke());
        }

        private void SetButtonLocation(int count)
        {
            loadButton.image.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
        }

        private int SetSpeed(int speed, int rpm, int count)
        {
            speedText.text = $"Speed: {speed} km/h, {rpm} RPM";
            speedText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return count + 1;
        }

        private int SetGearbox(int gearbox, int count) 
        {
            gearboxText.text = $"Gear: {gearbox}";
            gearboxText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return count + 1;
        }

        private int SetThrottle(float throttle, int count)
        {
            throttleText.text = $"Throttle Input: {throttle > 0.001f}";
            throttleText.gameObject.SetActive(throttle > 0.001f);
            throttleText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return throttle > 0.001f ? count + 1 : count;
        }

        private int SetBrake(float brake, int count)
        {
            brakeText.text = $"Brake Input: {brake > 0.001f}";
            brakeText.gameObject.SetActive(brake > 0.001f);
            brakeText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return brake > 0.001f ? count + 1 : count;
        }   

        private int SetClutch(float clutch, int count)
        {
            clutchText.text = $"Clutch Input: {clutch > 0.001f}";
            clutchText.gameObject.SetActive(clutch > 0.001f);
            clutchText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return clutch > 0.001f ? count + 1 : count;
        }

        private int SetSteering(float steering, int count)
        {
            steeringText.text = $"Steering Input: {steering > 0.001f}";
            steeringText.gameObject.SetActive(steering > 0.001f);
            steeringText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return steering > 0.001f ? count + 1 : count;
        }

        private int SetStarter(float starter, int count)
        {
            starterText.text = $"Starter Input: {starter > 0.001f}";
            starterText.gameObject.SetActive(starter > 0.001f);
            starterText.rectTransform.anchoredPosition = new Vector2(10f, -40f * count);
            return starter > 0.001f ? count + 1 : count;
        }
    }
}