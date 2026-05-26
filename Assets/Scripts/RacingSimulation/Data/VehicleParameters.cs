using System;

namespace RacingSimulation.Data
{
    [Serializable]
    public class VehicleParameters
    {
        // 차량 기본 물리
        public float Mass = 1500.0f;                   // 차량 무게 (kg)
        public float CenterOfGravityY = -0.1f;         // 무게중심 Y축 오프셋
        
        // 구동계 파라미터 (VehicleData 덮어쓰기 용도)
        public float FinalDriveRatio = 3.5f;           // 종감속비
        
        // 타이어 마찰 계수 승수 (Multiplier)
        public float GripMultiplier = 1.0f;            // 전체 타이어 접지력 배율
    }
}