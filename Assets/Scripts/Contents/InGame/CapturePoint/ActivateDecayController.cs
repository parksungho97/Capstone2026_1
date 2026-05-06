// 팀 상황을 캐싱하여:
//   - 팀원이 거점을 밟는 동안 → 감소 타이머 초기화
//   - 모두 빠져나가면 → decayDelay 초 대기 후 목표 임계값까지 점진적 감소
// 내부 상태 없음 — Fusion의 [Networked] TeamActivateState를 직접 읽고 반환
public class ActivateDecayController
{
    private readonly float decayDelay;
    private readonly float tickInterval;

    public ActivateDecayController(float decayDelay, float tickInterval)
    {
        this.decayDelay = decayDelay;
        this.tickInterval = tickInterval;
    }

    // 팀원이 거점을 밟고 있을 때: 감소 타이머 초기화 (증가 타이머는 유지)
    public TeamActivateState OnActive(TeamActivateState state)
    {
        state.DecayDelayTimer = 0f;
        state.DecayTickTimer = 0f;
        return state;
    }

    // 양 팀 모두 밟고 있을 때 (대치): 모든 타이머 초기화, 진척도 동결
    public TeamActivateState OnStandoff(TeamActivateState state)
    {
        state.IncreaseTimer = 0f;
        state.DecayDelayTimer = 0f;
        state.DecayTickTimer = 0f;
        return state;
    }

    // 아무도 없을 때: decayDelay초 대기 후 목표 임계값까지 tickInterval마다 1씩 감소
    public TeamActivateState ProcessIdle(TeamActivateState state, float deltaTime)
    {
        state.IncreaseTimer = 0f;

        uint target = ActivateProgressHelper.GetDecayTarget(state.Progress);
        if (state.Progress <= target)
        {
            state.DecayDelayTimer = 0f;
            state.DecayTickTimer = 0f;
            return state;
        }

        // 3초 대기
        state.DecayDelayTimer += deltaTime;
        if (state.DecayDelayTimer < decayDelay)
            return state;

        // 점진적 감소 (틱 기반)
        state.DecayTickTimer += deltaTime;
        if (state.DecayTickTimer >= tickInterval)
        {
            state.DecayTickTimer -= tickInterval;
            state.Progress -= 1;
        }

        return state;
    }
}
