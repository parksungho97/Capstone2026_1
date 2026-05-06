using Fusion;

// 팀별 진척도 관련 모든 네트워크 상태를 하나의 구조체로 묶음
// INetworkStruct → Activater의 [Networked] 프로퍼티로 직접 사용 가능
public struct TeamActivateState : INetworkStruct
{
    public uint Progress;       // 0 ~ 100
    public float IncreaseTimer; // 진척도 증가용 누적 시간
    public float DecayDelayTimer; // "아무도 없을 때" 감소 대기 누적 시간 (0 → mDecayDelay)
    public float DecayTickTimer;  // 감소 1포인트당 누적 시간
}
