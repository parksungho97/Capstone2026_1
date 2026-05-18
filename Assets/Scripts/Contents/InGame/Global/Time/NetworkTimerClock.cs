using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkTimerClock : NetworkBehaviour
{
    [Header("Default Setting")]
    [SerializeField] private float defaultDurationSeconds = 60f;

    /*
     Fusion에서는 타이머를 동기화할 때 TickTimer를 많이 쓴다. 
    TickTimer는 네트워크 상태로 관리할 수 있어서 각 클라이언트가 같은 종료 시점을 보게 만들기 좋다.
    */
    [Networked] public TickTimer Timer { get; private set; }
    [Networked] public float TotalDurationSeconds { get; private set; }
    [Networked] public NetworkBool IsRunning { get; private set; }

    public override void FixedUpdateNetwork()
    {
        // 상태 권한(서버/호스트)이 있는 쪽에서만 만료 체크
        if (Object.HasStateAuthority && IsRunning)
        {
            if (IsExpired())
            {
                IsRunning = false;
                Timer = TickTimer.None;

                Debug.Log("타이머 종료! 다음 로직 실행");
                // RPC 호출이나 다음 게임 상태로 전환
            }
        }
    }
    /*
     사용방법.
    * Spawned() 이후에 접근하는 게 안전함
        네트워크 상태값은 Spawned()가 호출된 뒤부터 유효하다.
    * 상태 변경은 권한 있는 쪽에서
      보통 Object.HasStateAuthority가 있는 쪽에서 HP/Armor/Timer를 바꾸는 게 맞다. 
      권한 없는 쪽이 바꾸면 예측값처럼 보일 수 있고 나중에 덮어씌워질 수 있다.
     */

    /// 네트워크 스폰 후 기본값만 세팅
    /// 자동 시작은 하지 않음
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            TotalDurationSeconds = defaultDurationSeconds;
            Timer = TickTimer.None;
            IsRunning = false;
        }
    }


    /// 초 단위로 타이머 세팅 후 시작
    public void SetSeconds(float seconds)
    {
        if (!Object.HasStateAuthority) return;
        if (seconds < 0f) seconds = 0f;

        TotalDurationSeconds = seconds;

        if (seconds <= 0f)
        {
            Timer = TickTimer.None;
            IsRunning = false;
            return;
        }

        Timer = TickTimer.CreateFromSeconds(Runner, seconds);
        IsRunning = true;
    }

    /// 분 단위로 타이머 세팅 후 시작
    /// 예: SetMinutes(3f) -> 3분
    public void SetMinutes(float minutes)
    {
        if (!Object.HasStateAuthority) return;
        if (minutes < 0f) minutes = 0f;

        SetSeconds(minutes * 60f);
    }

    /// 현재 남은 시간에 초를 더함
    public void AddSeconds(float seconds)
    {
        if (!Object.HasStateAuthority) return;
        if (seconds <= 0f) return;

        float newRemaining = GetRemainingTime() + seconds;
        TotalDurationSeconds = newRemaining;
        Timer = TickTimer.CreateFromSeconds(Runner, newRemaining);
        IsRunning = newRemaining > 0f;
    }


    /// 현재 남은 시간에 분을 더함
    public void AddMinutes(float minutes)
    {
        if (!Object.HasStateAuthority) return;
        if (minutes <= 0f) return;

        AddSeconds(minutes * 60f);
    }

    /// 현재 남은 시간에서 초를 뺌
    /// 0 이하가 되면 타이머 종료
    public void SubtractSeconds(float seconds)
    {
        if (!Object.HasStateAuthority) return;
        if (seconds <= 0f) return;

        float newRemaining = Mathf.Max(GetRemainingTime() - seconds, 0f);

        TotalDurationSeconds = newRemaining;

        if (newRemaining <= 0f)
        {
            Timer = TickTimer.None;
            IsRunning = false;
            return;
        }

        Timer = TickTimer.CreateFromSeconds(Runner, newRemaining);
        IsRunning = true;
    }


    /// 현재 남은 시간에서 분을 뺌
    public void SubtractMinutes(float minutes)
    {
        if (!Object.HasStateAuthority) return;
        if (minutes <= 0f) return;

        SubtractSeconds(minutes * 60f);
    }


    /// 타이머 정지
    public void StopTimer()
    {
        if (!Object.HasStateAuthority) return;

        Timer = TickTimer.None;
        IsRunning = false;
    }


    /// 현재 남은 시간 반환(초)
    public float GetRemainingTime()
    {
        if (Runner == null || !Runner.IsRunning) return 0f;
        if (!IsRunning) return 0f;

        float remain = Timer.RemainingTime(Runner) ?? 0f;
        return Mathf.Max(remain, 0f);
    }

    /// 현재 남은 시간 반환(분)
    public float GetRemainingMinutes()
    {
        return GetRemainingTime() / 60f;
    }


    /// 만료 여부
    public bool IsExpired()
    {
        if (Runner == null || !Runner.IsRunning) return false;
        return Timer.Expired(Runner);
    }
}