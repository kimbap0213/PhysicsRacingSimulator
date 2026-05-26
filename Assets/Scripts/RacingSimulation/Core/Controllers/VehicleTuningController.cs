using System.IO;
using UnityEngine;
using RacingSimulation.Data;
using RacingSimulation.Runtime;
using RacingSimulation.Utils;

namespace RacingSimulation.Core.Controllers
{
    public class VehicleTuningController : MonoBehaviour
    {
        [Header("Vehicle References")]
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private VehicleData vehicleDataTemplate; // 원본 (에디터 할당용)
        [SerializeField] private Rigidbody vehicleRigidbody;      // 무게 적용을 위한 RB

        private VehicleData _runtimeVehicleData; // 런타임 복사본 (원본 보호용)
        public VehicleParameters CurrentParameters { get; private set; }

        private void Awake()
        {
            // 1. ScriptableObject 원본 훼손 방지를 위해 인스턴스 복제
            _runtimeVehicleData = Instantiate(vehicleDataTemplate);
            
            // 2. 기본 파라미터 생성
            CurrentParameters = new VehicleParameters();
            
            // 3. 초기 세팅으로 차량 초기화
            ApplyParametersToVehicle(CurrentParameters);
            vehicle.Init(_runtimeVehicleData);
        }

        // 1. 파일에서 불러오기 (JSON, XML, CSV)
        public void LoadFromFile(string fileName, string extension)
        {
            string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.{extension}");
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"파일을 찾을 수 없습니다: {filePath}");
                return;
            }

            string fileContent = File.ReadAllText(filePath);
            VehicleParameters parsedData = null;

            switch (extension.ToLower())
            {
                case "json": parsedData = DataParserUtil.ParseJSON(fileContent); break;
                case "xml": parsedData = DataParserUtil.ParseXML(fileContent); break;
                case "csv": parsedData = DataParserUtil.ParseCSV(fileContent); break;
                default: Debug.LogError("지원하지 않는 확장자입니다."); return;
            }

            if (parsedData != null)
            {
                ApplyParametersToVehicle(parsedData);
                Debug.Log($"{extension.ToUpper()} 파일 로드 및 적용 완료!");
            }
        }

        // 2. 수동 입력 연동 
        public void SetManualParameter(string paramName, float value)
        {
            switch (paramName)
            {
                case "Mass": CurrentParameters.Mass = value; break;
                case "FinalDriveRatio": CurrentParameters.FinalDriveRatio = value; break;
                case "GripMultiplier": CurrentParameters.GripMultiplier = value; break;
            }
            // 값 변경 후 즉시 적용
            ApplyParametersToVehicle(CurrentParameters);
        }

        public void ApplyManualParameters(VehicleParameters manualParams)
        {
            ApplyParametersToVehicle(manualParams);
        }

        // 3. 실제 물리 및 데이터에 값 주입
        private void ApplyParametersToVehicle(VehicleParameters param)
        {
            CurrentParameters = param;

            // A. Rigidbody 물리 파라미터 적용
            if (vehicleRigidbody != null)
            {
                vehicleRigidbody.mass = param.Mass;
                // 무게중심(CoG) 하향 조정 (차량 전복 방지)
                vehicleRigidbody.centerOfMass = new Vector3(0, param.CenterOfGravityY, 0); 
            }

            // B. 구동계/조향계 파라미터 적용 (리플렉션이나 직접 할당 필요)
            if (vehicle != null)
            {
                // 1. Vehicle에 직접 종감속비 주입
                vehicle.SetRuntimeFinalDriveRatio(param.FinalDriveRatio);

                // 2. 모든 바퀴를 순회하며 마찰계수 배율 주입
                var wheels = vehicle.GetWheels();
                if (wheels != null)
                {
                    foreach (var wheel in wheels)
                    {
                        wheel.SetGripMultiplier(param.GripMultiplier);
                    }
                }
            }
        }
    }
}