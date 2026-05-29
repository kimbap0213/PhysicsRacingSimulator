using System;
using System.Collections;
using RacingSimulation.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RacingSimulation.Core.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Transform inGameTextPivot;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI throttleText;
        [SerializeField] private TextMeshProUGUI brakeText;
        [SerializeField] private TextMeshProUGUI clutchText;
        [SerializeField] private TextMeshProUGUI steeringText;
        [SerializeField] private TextMeshProUGUI starterText;
        [SerializeField] private TextMeshProUGUI gearboxText;
        [SerializeField] private Button loadButton;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private Button exitButton;
        [SerializeField] private TextMeshProUGUI recordText;

        private Action _onFixedUpdate;
        private float _startTime;
        private Coroutine _countDownCoroutine = null;
        private Coroutine _recordCoroutine = null;

        private void Awake()
        {
            exitButton.onClick.AddListener(Application.Quit);
        }

        public void ShowRecord(string record)
        {
            recordText.text = record;
            if (_recordCoroutine != null)
                return;
            _recordCoroutine = StartCoroutine(RecordCoroutine());
        }

        private IEnumerator RecordCoroutine()
        {
            Debug.Log($"Record Start");
            float elapsed = 0f;
            do
            {
                elapsed += Time.deltaTime;
                float factor = Mathf.SmoothStep(0f, 1f, Mathf.Lerp(0f, 1f, elapsed * 2f));
                recordText.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(-100f, 0f), Vector2.zero, factor);
                recordText.color = Color.Lerp(Color.clear, Color.white, factor);
                yield return null;
            } while (elapsed < 0.5f);

            _recordCoroutine = null;
        }

        public void AddEvent(EventTriggerType type, Action<BaseEventData> action)
        {
            EventTrigger.Entry entry = new();
            entry.eventID = type;
            entry.callback.AddListener((data) => action?.Invoke(data));
            eventTrigger.triggers.Add(entry);
        }

        public void SetActive(bool active)
        {
            inGameTextPivot.gameObject.SetActive(active);
            countDownText.gameObject.SetActive(!active);
        }

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
                countDownText.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(-100f, 0f), new Vector2(100f, 0f), factor);
                countDownText.color = Color.Lerp(Color.clear, Color.white, Mathf.PingPong(factor * 2f, 1f));
                elapsed += Time.deltaTime;
                yield return null;
            } while (elapsed < 3f);

            _countDownCoroutine = null;
            onCountDownEnd?.Invoke();
        }

        public void Init(Vehicle vehicle, Rigidbody rb, Func<float> time)
        {
            if (_countDownCoroutine != null)
            {
                StopCoroutine(_countDownCoroutine);
                _countDownCoroutine = null;
            }
            
            if (_recordCoroutine != null)
            {
                StopCoroutine(_recordCoroutine);
                _recordCoroutine = null;
            }
            
            recordText.color = Color.clear;
            _onFixedUpdate = () => OnFixedUpdate(vehicle, rb, time);
        }

        private void OnFixedUpdate(Vehicle vehicle, Rigidbody rb, Func<float> time)
        {
            int count = 0;
            count = SetSpeed(Mathf.RoundToInt(rb.linearVelocity.magnitude * 3.6f), Mathf.RoundToInt(vehicle.EngineRpm), count);
            count = SetGearbox(vehicle.CurrentGear, count);
            count = SetThrottle(vehicle.ThrottleInput, count);
            count = SetBrake(vehicle.BreakInput, count);
            count = SetClutch(vehicle.ClutchInput, count);
            count = SetSteering(vehicle.SteeringInput, count);
            count = SetStarter(vehicle.StarterInput, count);
            SetTimer(time);
        }

        private void SetTimer(Func<float> time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - time.Invoke());
            timeText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds:000}";
        }

        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        public void AddLoadAction(Action action)
        {
            loadButton.onClick.AddListener(() => action?.Invoke());
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