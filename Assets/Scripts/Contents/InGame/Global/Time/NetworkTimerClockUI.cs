using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkTimerClockUI : NetworkBehaviour
{
    //[Header("UI")]
    //[SerializeField] private TextMeshProUGUI timerText;
    //[SerializeField] private Image circleFillImage; // 선택사항 (Radial 360 이미지)

    [Header("Default Timer")]
    [SerializeField] private float defaultDuration = 60f;

    /*
     Fusion에서는 타이머를 동기화할 때 TickTimer를 많이 쓴다. 
    TickTimer는 네트워크 상태로 관리할 수 있어서 각 클라이언트가 같은 종료 시점을 보게 만들기 좋다.
    */
    [Networked] public TickTimer Timer { get; private set; }
    [Networked] public float TotalDuration { get; private set; }
    [Networked] public NetworkBool IsRunning { get; private set; }

    /*
    -  [Networked]로 타이머 종료 시점 보관
    - UI 텍스트를 mm:ss 형식으로 표시
    - 필요시 원형 Image.fillAmount도 함께 갱신 가능
    -“시계처럼” 계속 줄어드는 느낌으로 작동
     */
    /*
     사용방법.
    * 만들어놓은 Canvas의 타이머 UI 오브젝트에 붙임
    * timerText에 TMP 텍스트 연결
    * 원형 시계처럼 보이고 싶으면 circleFillImage에 Image 연결
        -> Image Type = Filled
        -> Fill Method = Radial 360
    * Spawned() 이후에 접근하는 게 안전함
        네트워크 상태값은 Spawned()가 호출된 뒤부터 유효하다.
    * 상태 변경은 권한 있는 쪽에서
      보통 Object.HasStateAuthority가 있는 쪽에서 HP/Armor/Timer를 바꾸는 게 맞다. 
      권한 없는 쪽이 바꾸면 예측값처럼 보일 수 있고 나중에 덮어씌워질 수 있다.
     */
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            StartTimer(defaultDuration);
        }
        // UpdateUI();
    }

    public override void Render()
    {
        //UpdateUI();
    }

    /// 타이머 시작
    public void StartTimer(float duration)
    {
        if (!Object.HasStateAuthority) return;
        if (duration <= 0f) return;

        TotalDuration = duration;
        Timer = TickTimer.CreateFromSeconds(Runner, duration);
        IsRunning = true;
    }

    /// 타이머 정지
    public void StopTimer()
    {
        if (!Object.HasStateAuthority) return;

        Timer = TickTimer.None;
        IsRunning = false;
    }

    /// 타이머 재시작
    public void RestartTimer()
    {
        if (!Object.HasStateAuthority) return;

        StartTimer(TotalDuration > 0 ? TotalDuration : defaultDuration);
    }

    /// 남은 시간(초)
    public float GetRemainingTime()
    {
        if (Runner == null) return 0f;
        if (!IsRunning) return 0f;

        float remaining = Timer.RemainingTime(Runner) ?? 0f;
        return Mathf.Max(remaining, 0f);
    }

    /// 타이머 종료 여부
    public bool IsExpired()
    {
        if (Runner == null) return false;
        return Timer.Expired(Runner);
    }

    //private void UpdateUI()
    //{
    //    float remain = GetRemainingTime();

    //    if (IsRunning && IsExpired())
    //    {
    //        remain = 0f;
    //    }

    //    int minutes = Mathf.FloorToInt(remain / 60f);
    //    int seconds = Mathf.FloorToInt(remain % 60f);

    //    if (timerText != null)
    //    {
    //        timerText.text = $"{minutes:00}:{seconds:00}";
    //    }

    //    if (circleFillImage != null)
    //    {
    //        if (TotalDuration > 0f)
    //        {
    //            circleFillImage.fillAmount = remain / TotalDuration;
    //        }
    //        else
    //        {
    //            circleFillImage.fillAmount = 0f;
    //        }
    //    }
    //}
}