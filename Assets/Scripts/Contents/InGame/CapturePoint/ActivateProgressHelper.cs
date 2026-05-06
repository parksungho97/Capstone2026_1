using System;

// 진척도 수식만 담당하는 순수 헬퍼
// 상태를 직접 보관하지 않으며, 항상 새 TeamActivateState를 반환
public static class ActivateProgressHelper
{
    // 현재 진척도 기준으로 감소해야 할 목표 임계값 반환 (0 / 33 / 66 / 99)
    public static uint GetDecayTarget(uint progress)
    {
        if (progress >= 99) return 99;
        if (progress >= 66) return 66;
        if (progress >= 33) return 33;
        return 0;
    }

    // 진척도를 tickInterval마다 1씩 증가
    public static TeamActivateState Increase(TeamActivateState state, float deltaTime, float tickInterval)
    {
        state.IncreaseTimer += deltaTime;
        if (state.IncreaseTimer >= tickInterval)
        {
            state.IncreaseTimer -= tickInterval;
            state.Progress = (uint)Math.Min(state.Progress + 1, 100);
        }
        return state;
    }
}
